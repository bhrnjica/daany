//////////////////////////////////////////////////////////////////////////////////////////
// Daany - DAta ANalYtics Library                                                        //
// https://github.com/bhrnjica/daany                                                    //
//                                                                                      //
// Copyright 2019-2020 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/daany/blob/master/LICENSE        //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica at hotmail.com                                                              //
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using Daany.MathStuff;
using XPlot.Plotly;

namespace Daany.Stat.SSA
{
    public enum Forecasting
    {
        Rforecasing,
        Vforecasting
    }
    public class EigenTriple
    {
        public double Li { get; set; }
        public double[] Ui { get; set; } = null!;
        public double[] Vi { get; set; } = null!;
		public float LiContrb { get; set; }//contribution 

    }
    /// <summary>
    /// Class implementation for Singular Spectrum Analysis based on the Algorithm defined at: 
    /// (N. Golyandina and A. Zhigljavsky, Singular Spectrum Analysis for Time Series, SpringerBriefs in Statistics, DOI: 10.1007/978-3-642-34913-3_2)
    /// SSA procedure in three steps:
    /// 1. Embedding  - the time series by forming a Hankel matrix of lagged window(length K) vectors.
    /// 2. Decompose - the embedded time series via Singular Value Decomposition
    /// 3. Eigen-tripple Grouping - is the process of identifying eigenvalue-eigenvector pairs as trend, seasonal and noise                         
    /// 4. Reconstruct the time series -  from the eigenvalue-eigenvector pairs identified as trend and seasonal.
    ///                              This is done through a process called diagonal averaging.
    /// 5. Forecasting - originally defined there are two methods of forecasting: rForecast and VForecast.                              
    /// Description of SSA (from Wikipedia)
    ///
    ///SSA can be used as a model-free technique so that it can be applied to arbitrary time series including 
    ///non-stationary time series. The basic aim of SSA is to decompose the time series into the sum of interpretable 
    ///components such as trend, periodic components and noise with no a-priori assumptions about the parametric form
    ///of these components. 
    /// </summary>
    public class SSA
    {
        //original time series
        private double[] _ts;
        private double[] _ts1;

        //L-trajectory matrix
        private double[,] _XX;

        //SSA parameters
        private int L, K;

        /// <summary>
        /// Elementary Matrices
        /// </summary>
        public Dictionary<int, double[,]> EM
        {
            get
            {
                return _Xs;
            }
        }
        private Dictionary<int, double[,]> _Xs;

        /// <summary>
        /// Trajectory matrix can be expressed as X= X1+X2+...X3, where Xi= si Ui Vi. 
        ///  The matrices Xi have rank 1; therefore they are elementary matrices, 
        ///     - si - singular values of the matrix X
        ///     - Ui(in SSA literature they are called ‘factor empirical orthogonal functions’ or simply EOFs) and 
        ///     - Vi(often called ‘principal components’) stand for the left and right eigenvectors of the trajectory matrix.
        /// The collection(si, Ui, Vi) is called the i-th eigentriple of the matrix X, si (i = 1, . . . , d) are
        /// the singular values of the matrix X and the set {si} is called the spectrum of the matrix X.
        /// If all the eigenvalues have multiplicity one, then the expansion(2.1) is uniquely defined.
        /// </summary>
        public Dictionary<int, EigenTriple> EigenTriple
        {
            get
            {
                return _eigentriple;
            }
        }
        private Dictionary<int, EigenTriple> _eigentriple = new Dictionary<int, EigenTriple>();

        /// <summary>
        /// Singular Spectral Analysis Constructor. 
        /// </summary>
        /// <param name="ts">series of double values, without missing elements</param>
        public SSA(IEnumerable<double> ts)
        {
            _ts = ts.ToArray();
        }

        #region Decompose 
        private void resetParameters()
        {
            _XX = null;
            L = 0; K = 0;
            if(_Xs!=null)
                _Xs.Clear();
            if (_eigentriple != null)
                _eigentriple.Clear();

    }
        /// <summary>
        /// Embedding
        /// <returns></returns>
        public double[,] Embedding(uint embeddingDim = 0)
        {
            if (embeddingDim > _ts.Length / 2)
                throw new Exception($"Embedding cannot be greater than  half time series length.");

            L = (int)embeddingDim;

            //setting default embedding dimension
            if (embeddingDim == 0)
                L = _ts.Length / 2;

            //calculation of K
            K = _ts.Count() - L + 1;
            //
            var retVal = _ts.Hankel(L).Transpose<double>();
            _XX = retVal;
            return retVal;
        }

        /// <summary>
        /// Perform the Singular Value Decomposition and identify the rank of the embedding subspace
        /// </summary>
        public void Decompose()
        {
            //transformation of the embedded matrix
            var XXT = _XX.Transpose();

            //embedding (trajectory) matrix helper var
            var X = _XX;

            //S matrix calculation
            double[,] S = _XX.Dot(XXT);

            double[,] U=null ; //left singular eigen vector of XX
           // double[,] V=null ; //right singular eigen vector of XX
            double[] s=null;//sqrt of lambda_i, eigenvalues of S,
            double[] ss= null ;
            double frob_norm ;//norm of the embedding matrix
            double normSquared=0 ;
            int d = 0; //rank of the 

            //call Intel MKL library for SVD decomposition
            (double[] _s, double[,] _U, double[,] _V) = Daany.LinA.LinA.Svd(S, true, false);
            U = _U;
            s = _s.Sqrt();
            ss = _s;
            frob_norm = X.Euclidean();
            normSquared = frob_norm * frob_norm;
            d = s.Where(x => x > 0).Count();

            //****summary of the SVD calculation****
            //SVD trajectory matrix written as XX = XX_1 + XX_2 + XX_3 + ... + XX_d
            _Xs = new Dictionary<int, double[,]>();

            //calculation of the eigen-triple of the SVD
            for (int i = 0; i < d; i++)
            {
                //Vi=(X^T).(Ui)/s  - matrix 1 x d
                var Vi = XXT.Dot(U.GetColumn(i).Divide(s[i]));
                //sUi = (si)*Ui - matrix d x 1
                var sUi = U.GetColumn(i).Multiply(s[i]);
                //
                //matrix multiplication sUI dot Vi
                var Xi = sUi.ToMatrix(asColumnVector: true).Dot(Vi.ToMatrix(asColumnVector: false));
                _Xs.Add(i + 1, Xi);

                //calculate contribution of the eigen-triple
                var contrb = (float)Math.Round(ss[i] * 100 / normSquared, 2);
                //add eigen-triple to collection in case it is greather than zero
                if (contrb > 0)
                {
                    var eti = new EigenTriple() { Li = s[i], LiContrb = contrb, Ui = U.GetColumn(i), Vi = Vi };
                    _eigentriple.Add(i + 1, eti);
                }
            }

        }
        #endregion

        #region Reconstruct
        /// <summary>
        /// transform each matrix Xij of the grouped decomposition into a new series of length N
        /// </summary>
        /// <returns>elementary reconstructed series</returns>
        private double[] diagonalAveraging(double[,] eMatrix)
        {
            //calculate number of cols, rows and 
            var LL = eMatrix.GetLength(0);
            var KK = eMatrix.GetLength(1);
            var Y = eMatrix;

            int lStar = Math.Min(LL, KK);
            int kStar = Math.Max(LL, KK);
            int N = LL + KK - 1;
            //
            var newM = MatrixEx.Zeros(L, K);
            //
            if (LL >= KK)
                Y = Y.Transpose();

            //reconstructed series
            var y = new double[N];
            for (int k = 1; k <= N; k++)
            {
                double yk = 0;
                if (k >= 1 && k < lStar)
                {
                    for (int m = 1; m <= k; m++)
                    {
                        int i = m;
                        int j = k - m + 1;
                        yk += Y[i - 1, j - 1];
                    }
                    //
                    y[k - 1] = yk / k;
                }
                else if (k >= lStar && k <= kStar)
                {
                    for (int m = 1; m <= lStar; m++)
                    {
                        int i = m;
                        int j = k - m + 1;
                        yk += Y[i - 1, j - 1];
                    }
                    //
                    y[k - 1] = yk / lStar;
                }
                else if (k > kStar && k <= N)
                {
                    for (int m = k - kStar + 1; m <= N - kStar + 1; m++)
                    {
                        int i = m;
                        int j = k - m + 1;
                        yk += Y[i - 1, j - 1];
                    }
                    //
                    y[k - 1] = yk / (N - k + 1);
                }
                else
                    throw new Exception("This should not be happened!");
            }
            return y;
        }
        /// <summary>
        /// Reconstruct series by providing indices of list of elementary matrices
        /// </summary>
        /// <param name="signalIndex">list of indices of elementary matrices</param>
        /// <returns></returns>
        public double[] Reconstruct(params int[] group)
        {
            //reconstructed ts
            var rts = MatrixEx.Zeros(_ts.Count());

            for (int i = 0; i < group.Length; i++)
            {
                var sm = EM[group[i]];
                var retVal = diagonalAveraging(sm);
                rts = rts.Add(retVal);
            }

            return rts;
        }
        #endregion

        #region w-Correlation
        /// <summary>
        /// Calculates the correlation between components
        /// </summary>
        /// <returns>Matrix of Correlation values</returns>
        public double[,] WCorrelation(int[] group = null!)
        {
            int[] grp = group;
            if (group != null && group.Length < 2)
                throw new Exception("Group must contain at least two elements.");

            if (grp == null)
                grp = Enumerable.Range(1,EM.Count).ToArray();
                

            //prepare for forecasting by calculation necessary values
            var wi = calculateWeights();
            var count = grp.Length;
            var contrib = new List<double[]>();
            //
            foreach (var g in grp)
            {
                var c = diagonalAveraging(EM[g]);
                contrib.Add(c);
            }

            //
            var Fnorms = new double[count];
            for (int i = 0; i < count; i++)
                Fnorms[i] = wi.Multiply(contrib[i].Multiply(contrib[i])).Sum();

            Fnorms = Fnorms.Pow(-0.5);
            //
            var wCorr = MatrixEx.Identity(count, count);
            for (int i = 0; i < count; i++)
            {
                for (int j = i+1; j < count; j++)
                {
                    wCorr[i, j] =Math.Round(Math.Abs(wi.Multiply(contrib[i].Multiply(contrib[j])).Sum()) * Fnorms[i] * Fnorms[j],4);
                    wCorr[j, i] = wCorr[i, j];
                }
            }
            return wCorr;
        }

        /// <summary>
        /// Calculation of weights
        /// </summary>
        /// <returns></returns>
        private double[] calculateWeights()
        {
            int lStar = Math.Min(L, K);
            int kStar = Math.Max(L, K);
            int N = L + K - 1;
            //
            var wi = new double[N];//initialize weights
            //
            for (int i = 0; i < N; i++)
            {
                if (i >= 0 && i <= lStar-1)
                    wi[i] = i + 1;
                else if (i >= lStar && i <= kStar-1)
                    wi[i] = lStar;
                else if (i >= kStar && i < N)
                    wi[i] = N - i;
                else
                    throw new Exception("This should not be happened!");
            }
            return wi;
        }

        #endregion

        #region Forecasting and Fitting

        public double[] Forecast(int compCount, int stepsAhead, bool wholeSeries = true, Forecasting method = Forecasting.Rforecasing)
        {
            var group = Enumerable.Range(1,compCount).ToArray();
            return Forecast(group,stepsAhead,wholeSeries, method);
        }
        /// <summary>
        /// Forecast from last point of original time series up to steps_ahead.
        /// </summary>
        /// <param name="stepsAhead"></param>
        /// <param name="singularValues"></param>
        /// <returns></returns>
        public double[] Forecast(int[] group, int stepsAhead, bool wholeSeries = true, Forecasting method= Forecasting.Rforecasing)
        {
            //
            
            if (method== Forecasting.Rforecasing)
            {
                return Rforecasting(group, stepsAhead, wholeSeries);
            }
            else
            {
                return Vforecasting(group, stepsAhead, wholeSeries);
            }


        }

        /// <summary>
        /// VForecasting SSA method
        /// </summary>
        /// <param name="group"></param>
        /// <param name="stepsAhead"></param>
        /// <param name="wholeSeries"></param>
        /// <returns></returns>
        private double[] Vforecasting(int[] group, int stepsAhead, bool wholeSeries)
        {
            double[] forecast;
            //prepare for forecasting by calculation necessary values
            (double[,] X, double[,] Pe) = prepareVectorFormula(group);

            //define extended trajectory matrix
            double[,] Xext = new double[L, K + stepsAhead];
            
            //fill the first element of time series
            for (int i = 0; i < K + stepsAhead; i++)
            {
                if (i < K)
                {
                    for (int j = 0; j < L; j++)
                        Xext[j, i] = X[j, i];
                }
                else
                {
                    var Ydelta = Xext.GetColumn(i - 1).ToArray().ToMatrix(true);
                    var Zi = Pe.Dot(Ydelta);

                    for (int j = 0; j < L; j++)
                        Xext[j, i] = Zi[j, 0];
                }

            }
            //perform diagonal averaginig
            forecast = diagonalAveraging(Xext);
            //
            if (wholeSeries)
                return forecast.ToArray();
            else
                return forecast.TakeLast(stepsAhead).ToArray();
        }

        /// <summary>
        /// RForecasting method
        /// </summary>
        /// <param name="group"></param>
        /// <param name="stepsAhead"></param>
        /// <param name="wholeSeries"></param>
        /// <returns></returns>
        private double[] Rforecasting(int[] group, int stepsAhead, bool wholeSeries)
        {
            if (group.Length > EigenTriple.Count)
                throw new Exception("The group is greater than number o eigen triple.");

            var forecast = new List<double>();
            //prepare for forecasting by calculation necessary values
            (double[] cts, double[] R) = prepareRecurrentFormula(group);
            //fill the first element of time series
            forecast.AddRange(cts);
            for (int i = this._ts.Length; i < this._ts.Length + stepsAhead; i++)
            {
                // 
                var tss = forecast.TakeLast(R.Length).ToArray();
                var x = R.ToMatrix().Dot(tss.ToMatrix(true));
                forecast.Add(x[0, 0]);
            }
            //
            if (wholeSeries)
                return forecast.ToArray();
            else
                return forecast.TakeLast(stepsAhead).ToArray();
        }

        (double[] ts, double[] r) prepareRecurrentFormula(int[] iC)
        {
            double vertCoeff = 0;
            double[][] PP = new double[iC.Length][];//orthonormal basis in Lspace

            //trajectory matrix of iC group
            var Xhat = MatrixEx.Zeros(L, K);
            //
            for (int i = 0; i < iC.Length; i++)
                PP[i] = EigenTriple[iC[i]].Ui;

            //create zero matrix
            var R = new double[L - 1];

            //
            for (int i = 0; i < PP.Length; i++)
            {

                var Pi = PP[i];

                var PiPiT = Pi.ToMatrix(true).Dot(Pi.ToMatrix(false));
                var ti = PiPiT.Dot(_XX);
                Xhat = Xhat.Add(ti);

                //pi is the last component of the vector Pi
                var pi = Pi.Last();
                vertCoeff += pi * pi;

                //first L-1 components
                var PI = Pi.Take(L - 1).ToArray();
                var rr = PI.Multiply(pi);
                R = R.Add(rr);

            }

            //the last r calculation step
            R = R.Divide((1.0 - vertCoeff));

            //reconstructed time series
            var cts = diagonalAveraging(Xhat);
            return (cts, R);
        }

        (double[,] X, double[,] Pe) prepareVectorFormula(int[] iC)
        {
            int r = iC.Length;
            double vertCoeff = 0;
            double[][] PP = new double[iC.Length][];//orthonormal basis in Lspace

            //trajectory matrix of iC group
            var Xhat = MatrixEx.Zeros(L, K);
            //
            for (int i = 0; i < r; i++)
                PP[i] = EigenTriple[iC[i]].Ui;

            var Vdelta = new double[L - 1, L - 1];

            //create zero matrix
            var R = new double[L - 1];

            //
            for (int i = 0; i < r; i++)
            {
                var Pi = PP[i];
                var PiPiT = Pi.ToMatrix(true).Dot(Pi.ToMatrix(false));
                var ti = PiPiT.Dot(_XX);
                Xhat = Xhat.Add(ti);

                //pi is the last component of the vector Pi
                var pi = Pi.Last();
                vertCoeff += pi * pi;

                //first L-1 components
                var PI = Pi.Take(L - 1).ToArray();
                var rr = PI.Multiply(pi);
                R = R.Add(rr);

                //
                for (int ii = 0; ii < L - 1; ii++)
                    Vdelta[ii, i] = PI[ii];
            }

            //the last R calculation step
            R = R.Divide((1.0 - vertCoeff));
            //
            var p1 = R.ToMatrix(true).Dot(R.ToMatrix(false)).Multiply((1.0 - vertCoeff));
            var p2 = Vdelta.Dot(Vdelta.Transpose());
            var P = p1.Add(p2);

            //Define Pe operator
            double[,] Pe = new double[L, L];
            for (int i = 0; i < L; i++)
            {
                for (int j = 0; j < L; j++)
                {
                    if (j == 0)
                        Pe[i, j] = 0;
                    else if (i < L - 1)
                        Pe[i, j] = P[i, j - 1];
                    else
                        Pe[i, j] = R[j - 1];
                }
            }
            return (Xhat, Pe);
        }

        /// <summary>
        /// Perform SSA for N lagged steps
        /// </summary>
        /// <param name="embeddingDim"></param>
        public void Fit(uint embeddingDim)
        {
            resetParameters();

            Embedding(embeddingDim);
            Decompose();
        }

        
        #endregion

        #region Plotting SSA
        /// <summary>
        /// Perform SSA on time series by searching for the best 10 signal decomposition
        /// </summary>
        /// <param name="validCount">The number of last time series element used for validation</param>
        /// <param name="maxSignalCount">Maximum number of signals used in analysis</param>
        /// <param name="reconstructionCount"> Maximum number of signals for time series reconstruction</param>
        /// <returns>Return list of top 10 results based on RMSE of the validation set</returns>
        public List<(int signals, int recoveryCount, double rmse)> Train(int validCount, int maxSignalCount)
        {
            //when training we define validation set in order to measure how Forecast is good for certain signal count
            _ts1 = _ts;

            //main time series become training series 
            _ts = _ts1.Take(_ts.Count() - validCount).ToArray();
            var validTS = _ts1.Skip(_ts.Count()).ToArray();
            var stepsAhead = validTS.Count();

            //create training loop
            var eval = new List<(int i, int rc, double rmse)>();

            foreach (var i in Enumerable.Range(3, maxSignalCount))
            {
                //perform analysis with i signals
                Fit((uint)i);
                var r1 = double.PositiveInfinity;
                var rec = 3;
                foreach (var rc in Enumerable.Range(3, maxSignalCount - 3))
                {
                    //call forecast method and pass the number of time steps of steps ahead for forecasting
                    var predicted = Forecast(Enumerable.Range(1, rc).ToArray(), stepsAhead, method:Forecasting.Rforecasing);
                    var validPredict = predicted.Skip(_ts.Count()).ToArray();

                    //evaluate model with validation set
                    var rmse = validPredict.RMSE(validTS);
                    //
                    if (rmse < r1)
                    {
                        Console.WriteLine($"Number of signals = {i}, recoveryCount={rc}, \t RMSE={rmse}");
                        r1 = rmse;
                        rec = rc;
                    }
                }


                eval.Add((i, rec, r1));
            }

            //return top ten results
            return eval.OrderBy(x => x.rmse).Take(10).ToList();
        }

        /// <summary>
        /// Plot Time series forecast for specific number of decomposed signals
        /// </summary>
        /// <param name="series"></param>
        /// <param name="signals"></param>
        /// <param name="horizont"></param>
        public static PlotlyChart Plot(Series series, int signals, int recoveryCount, int horizont)
        {
            //get time series
            var ts1 = series.Select(f => Convert.ToDouble(f));//create time series
            var xValues = series.Index.ToArray();
            return Plot(ts1, xValues, signals, recoveryCount, horizont);
        }


        public static PlotlyChart Plot(IEnumerable<double> ts1, object[] xValues, int signals, int recoveryCount, int horizont)
        {

            //create two ts sets training and forecasting
            var count = ts1.Count();
            var ts_Train = ts1.Take(count - horizont);
            var ts_Test = ts1.Skip(count - horizont - 1).ToArray();
            uint nmIndex = (uint)signals;


            //create Singular Spectrum Analysis object by passing ts object
            var ssa = new SSA(ts_Train);

            //perform analysis
            ssa.Fit(nmIndex);


            //x axes for plot sum of the components
            var x0 = xValues.Take(ts_Train.Count()).ToArray();
            var x1 = xValues.Skip(ts_Train.Count() - 1).ToArray();

            //Once we calculates signals by using SSA, we have to reconstruct the signals into time series
            var modelValue = ssa.Reconstruct(recoveryCount);
            //call forecast method and pass the number of time steps of steps ahead for forecasting and signals for reconstruction
            var values = ssa.Forecast(Enumerable.Range(1, recoveryCount).ToArray(), horizont, true, method: Forecasting.Vforecasting);
            //
            // var modelValue = values.Take(ts_Train.Count()).ToArray();
            var modelPredict = values.Skip(ts_Train.Count()).ToList();
            modelPredict.Insert(0, modelValue.Last());
            return Plot((x0, ts_Train.ToArray(), modelValue), (x1, ts_Test, modelPredict.ToArray()));
        }

    

        public PlotlyChart PlotwCorrelation(double[,] wValues)
        {
            var s1 = new Heatmap()
            {

                name = "W-correlation matrix",
               // colorscale = @"Viridis",
                z = wValues
            };

            var chart = XPlot.Plotly.Chart.Plot<Trace>(new Trace[] { s1 });
            return chart;
        }

        /// <summary>
        /// Plot train and test sets
        /// </summary>
        /// <param name="train"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        public static PlotlyChart Plot((object[] x, double[] actual, double[] predicted) train, (object[] x, double[] actual, double[] predicted) test)
        {
            var layout = new Layout.Layout();
            layout.title = "Singular Spectrum Analysis Forecast";
            layout.showlegend = true;
            layout.plot_bgcolor = "rgb(223,223,223)";


            var scatters1 = new Scatter() { name = "Actual", x = train.x, y = train.actual, mode = "line", };
            var scatters2 = new Scatter() { name = "Predicted", x = train.x, y = train.predicted, mode = "line", };
            var scatters3 = new Scatter() { name = "Test Actual", x = test.x, y = test.actual, mode = "line", };
            var scatters4 = new Scatter() { name = "Forecast", x = test.x, y = test.predicted, mode = "line", };
            var chart = XPlot.Plotly.Chart.Plot<Trace>(new Trace[] { scatters1, scatters2, scatters3, scatters4 });
            chart.WithLayout(layout);
            return chart;
            //chart.Show();
        }

        public XPlot.Plotly.PlotlyChart PlotSingularValues()
        {

            var layout = new Layout.Layout();
            layout.barmode = "group";
            layout.title = "Logarithms of eigenvalues";
            var x = Enumerable.Range(1, EigenTriple.Count).Select(x => x);
            var bar1 = new Scatter() { name = "Eigenvalues", x = x, y = EigenTriple.Select(x => Math.Log(x.Value.Li)), mode = "lines+markers" };
            var chart = XPlot.Plotly.Chart.Plot<Trace>(new Trace[] { bar1 });
            chart.WithLayout(layout);
            return chart;
        }

        public List<XPlot.Plotly.PlotlyChart> PlotEigenPairs()
        {
            var layout = new Layout.Layout();
            
            layout.showlegend = false;
            layout.width = 100;
            layout.height = 100;
            var lst = new List<XPlot.Plotly.PlotlyChart>();
            foreach (var i in Enumerable.Range(0, EigenTriple.Count()-1))
            {
                var p1 = EigenTriple[i + 1];
                var p2 = EigenTriple[i + 2];
                var s = new Scatter()
                {
                    showlegend = true,
                    name = $"ET{i + 1}({EigenTriple[i + 1].LiContrb}%)-{i + 2}({EigenTriple[i + 2].LiContrb}%)",
                    x = EigenTriple[i + 1].Ui,
                    y = EigenTriple[i + 2].Ui,
                    mode = "line",
                };
                lst.Add(XPlot.Plotly.Chart.Plot<Trace>(new Trace[] { s }));
            }
            //
            return lst;
            //chart.Show();
        }

        public PlotlyChart PlotContributions(bool isScaled= false, bool isCumulative= false)
        {
            var contrib = EM.Select(x => diagonalAveraging(x.Value));
            var layout = new Layout.Layout();
            layout.barmode = "group";
            layout.title = "SSA signal contributions (lambda_i)";
            var x = Enumerable.Range(1, contrib.Count()).Select(x => $"Lambda{x}");
            var bar1 = new Bar() { name = "Contributions", x = x, y = contrib };
            var chart = XPlot.Plotly.Chart.Plot<Trace>(new Trace[] { bar1 });
            chart.WithLayout(layout);
            return chart;
        }

        public PlotlyChart PlotComponents(int signalCount)
        {
            var sCount = EigenTriple.Count() < signalCount? EigenTriple.Count(): signalCount;
            var lst = new List<XPlot.Plotly.PlotlyChart>();
            for (int i = 0; i < sCount; i++)
            {
                var ts = EigenTriple[i + 1].Ui;//diagonalAveraging(EM[i]);
                var s = new Scatter()
                {

                    showlegend = true,
                    name = $"X{i + 1}({EigenTriple[i + 1].LiContrb}%)",
                    y = ts,
                    x = Enumerable.Range(1, ts.Length),
                    mode = "line",
                };
                lst.Add(XPlot.Plotly.Chart.Plot<Trace>(new Trace[] { s }));
            }
            //
            Chart.ShowAll(lst.ToArray());
            return null;
            //chart.Show();

        }

        #endregion
    }
}
