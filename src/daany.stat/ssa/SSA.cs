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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Accord.Math.Decompositions;
using Daany.MathStuff;
using XPlot.Plotly;

namespace Daany.Stat
{
    /// <summary>
    /// This class is modification of the python code found at: https://github.com/aj-cloete/pySSA
    /// Class implementation for singular spectrum analysis, based on the Python version found at: 
    /// SSA procedure in three steps:
    /// 1. Embed  - the time series by forming a Hankel matrix of lagged window(length K) vectors.
    /// 
    /// 2. Decompose - the embedded time series via Singular Value Decomposition
    /// 3. Eigentripple Grouping - is the process of identifying eigenvalue-eigenvector pairs as trend, 
    ///                         seasonal and noise
    ///4. Reconstruct the time series -  from the eigenvalue-eigenvector pairs identified as trend and seasonal.
    ///                              This is done through a process called diagonal averaging.
    ///Description of SSA (from Wikipedia)
    ///
    ///SSA can be used as a model-free technique so that it can be applied to arbitrary time series including 
    ///non-stationary time series. The basic aim of SSA is to decompose the time series into the sum of interpretable 
    ///components such as trend, periodic components and noise with no a-priori assumptions about the parametric form
    ///of these components. 
    /// </summary>
    public class SSA
    {
        //The additive signal elements from the time series
        public Dictionary<int, double[,]> Xs
        {
            get
            {
                return _Xs;
            }
        }

        //The additive signal elements from the time series
        public double[] Contributions
        {
            get
            {
                return _sContributions;
            }
        }

        Dictionary<int, double[,]> _Xs;

        //main time series collection
        double[] _ts;
        //time series for analysis. Sometime it can be different than main times series when we perform training
        double[] _ts1;

        private double[] _sContributions;
        private double[] _R;
        private double[][] _orthonormalBase;
        private double[,] _xCom;
        double[] X_com_tilde;

        /// <summary>
        /// Singular Spectral Analysis Constructor. 
        /// </summary>
        /// <param name="ts">series of double values, without missing elements</param>
        public SSA(IEnumerable<double> ts)
        {
            _ts = ts.ToArray();
        }

        /// <summary>
        /// (PDF) Singular Spectrum Analysis for Time Series.Available from:
        /// https://www.researchgate.net/publication/260124592_Singular_Spectrum_Analysis_for_Time_Series [accessed Sep 10 2019].
        /// 1st step: Embedding
        /// To perform the embedding such that it map the original time series into a sequence of lagged vectors of size L
        /// by forming K = N−L+1 lagged vectors (columns)
        /// L -rows
        /// The matrix X is a Hankel matrix which means that matrix X has equal elements xij on the anti-diagonals. 
        /// </summary>
        /// <param name="embeddingDim">embedding dimension</param>
        /// <param name="suspectedFreq">changes embedding_dimension such that it is divisible by suspected frequency</param>
        /// <returns></returns>
        public double[,] Embedding(uint embeddingDim = 0, uint suspectedFreq = 0)
        {
            int L = (int)embeddingDim;

            //setting the embedding dimension
            if (embeddingDim == 0)
                L = _ts.Length / 2;

            if (suspectedFreq > 0)
                L = (int)Math.Floor((double)embeddingDim/(double)suspectedFreq)*(int)suspectedFreq;

            //calculation of K
            int K = _ts.Count() - L + 1;

            //prepare the embedding matrix
            var retVal = new double[L, K];
            for (int i = 0; i < L; i++)
            {   
                var k = i;
                for (int j = 0; j < K; j++)
                   retVal[i, j] = _ts[k++];
            }
            //store embedding matrix to class field
            _xCom = retVal;

            return retVal;
        }
        /// <summary>
        /// Perform the Singular Value Decomposition and identify the rank of the embedding subspace
        /// Characteristic of projection: the proportion of variance captured in the subspace
        /// </summary>
        public void Decompose()
        {
            //transformation of the embedded matrix
            var XT = _xCom.Transpose();
            //embedding (trajectory) matrix helper var
            var X = _xCom; 

            //S matrix calculation
            double[,] S = _xCom.Dot(XT);

            /*Single Value Decomposition
             * For an m-by-n matrix A with m >= n, the singular value decomposition is 
             * 
             *                      an m-by-n orthogonal matrix U,
             *                      an n-by-n diagonal matrix S, 
             *                  and an n-by-n orthogonal matrix V 
             *                 so that A = U * S * V'. 
             *          
             * The singular values, sigma[k] = S[k,k], are ordered so that sigma[0] >= sigma[1] >= ... >= sigma[n-1].
             */
            var svd = new SingularValueDecomposition(S);

            //summary of the SVD calculation
            //left eigenvector
            double[,] U = svd.LeftSingularVectors;
            double[] s = svd.Diagonal.Sqrt();

            //right eigenvector
            double[,] V = svd.RightSingularVectors;
            int d = svd.Rank;

            //helper variable to calculate characteristics of projection
            double[][] Ys = new double[d][];
            double[][] Zs = new double[d][];
            var Vs = new double[d][];

            //SVD trajectory matrix written as Xs = X1 + X2 + X3 + ... + Xd
            _Xs = new Dictionary<int, double[,]>();

            //calculation of the eigen-triple of the SVD
            for (int i = 0; i < d; i++)
            {
                // 
                Zs[i] = V.GetColumn(i).Multiply(s[i]);

                //
                Vs[i] = XT.Dot(U.GetColumn(i).Divide(s[i]));

                //
                Ys[i] = U.GetColumn(i).Multiply(s[i]);

                //vector of Ys[i] to matrix d x 1
                var y = Ys[i].ToMatrix(asColumnVector: true);

                //vector of Vs[i] to matrix 1 x n
                var v = Vs[i].ToMatrix(asColumnVector: false);

                //matrix multiplication
                var x = y.Dot(v);
                _Xs.Add(i, x);
            }

            //calculate contributions
            _sContributions = getControbutions(X, s);

            int r = _sContributions.Length;
            var firstRValues = s.Take(r).ToArray();
            var _rCharacteristic = Math.Round(firstRValues.Multiply(firstRValues).Sum() / s.Multiply(s).Sum(), 4);

            //
            var orthonormalBase = new double[r][];
            foreach (var ind in Enumerable.Range(0, r))
               orthonormalBase[ind] = U.GetColumn(ind);


            //set final value of the orthonormal matrix
            _orthonormalBase = orthonormalBase;
        }

        /// <summary>
        /// Calculate the relative contribution of each of the singular values
        /// </summary>
        /// <param name="X">Embedded matrix</param>
        /// <param name="s">Eigenvector s</param>
        /// <returns></returns>
        public static double[] getControbutions(double[,] X, double[] s)
        {
            //square of the eigenvector values
            double[] lambdas = s.Pow(2);

            //norm of the embedding matrix
            double frob_norm = X.Euclidean();
            //
            var contr = lambdas.Divide(frob_norm * frob_norm);

            //return only positive contributions
            return contr.Select(x => Math.Round(x, 5)).Where(x => x > 0).ToArray();
        }

        /// <summary>
        /// the contribution of each of the signals (corresponding to each singular value) 
        /// </summary>
        /// <param name="adjustScale"></param>
        /// <returns></returns>
        public double[] SContributions(bool adjustScale = true, bool cumulative = false)
        {
            //
            var contr = _sContributions;
            var posContr = contr.Where(x => x != 0).ToArray();
            
            //in case cumulative flag is enabled
            if (cumulative)
                posContr = posContr.CumulativeSum();
            
            //in case adjusted flag is enabled
            if (adjustScale)
                posContr = posContr.Pow(-1).Substract(posContr.Pow(-1).Max() * 1.1).Multiply(-1);
            
            //
            return posContr;
        }

        
        /// <summary>
        /// transform each matrix Xij of the grouped decomposition into a new series of length N
        /// </summary>
        /// <returns>elementary reconstructed series</returns>
        private double[] diagonalAveraging(double[,] signalMatrix)
        {
            //calculate number of cols, rows and 
            var L = signalMatrix.GetLength(0);
            var K = signalMatrix.GetLength(1);
            var Y = signalMatrix;

            int lStar = Math.Min(L, K);
            int kStar = Math.Max(L, K);
            int N = L + K - 1;
            //
            var newM = MatrixEx.Zeros(L, K);
            //
            if (L >= K)
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
        /// Reconstruct time series component from the signal matrix
        /// </summary>
        /// <param name="xs">signal matrix</param>
        /// <returns></returns>
        public double[] Reconstruct(double[,] xs)
        {
            return diagonalAveraging(xs);
        }

        /// <summary>
        /// Reconstruct time series component from the signal matrix
        /// </summary>
        /// <param name="xs">number of signal matrix</param>
        /// <returns></returns>
        public double[] Reconstruct(int signalCounts = -1)
        {
            double[] tsCumulative=null;
            IEnumerable<KeyValuePair<int, double[,]>> sM = _Xs;
            if (signalCounts > 0)
                sM = _Xs.Take(signalCounts);

            //initial ts
            tsCumulative = MatrixEx.Zeros(_ts.Count());

            foreach (var sMat in sM)
            {
                var retVal = diagonalAveraging(sMat.Value);
                tsCumulative= tsCumulative.Add(retVal);
            }

            return tsCumulative;
        }

        /// <summary>
        /// Forecast from last point of original time series up to steps_ahead using recurrent methodology
        /// Forecasting by SSA can be applied to time series that approximately satisfy
        /// linear recurrent formulae(LRF). 
        /// The series Y_T satisfies an LRF of order d if there are numbers a1, . . . , ad such that
        /// 
        ///  y_(i+d)=SUM_(k=1)^d(a_k y_(i+d-1) ); (1 <= i <= T-d)
        /// 
        /// </summary>
        /// <param name="horizont"></param>
        /// <param name="singularValues"></param>
        /// <returns></returns>
        public double[] Forecast(int horizont = 12, int singularValues = -1)
        {
            //prepare for forecasting by calculation necessary values
            prepareForecast(singularValues);

            //
            var forecast = new List<double>();

            //fill the first element of time series
            forecast.Add(this._ts[0]);
            for (int i=1; i < this._ts.Length + horizont; i++)
            {
                if(i < this._ts.Length)
                {
                    if (double.IsNaN(_ts[i]) && i < _R.Length)
                        throw new Exception($"Missing values must be at greater position than {_R.Length}.");

                    else if (double.IsNaN(_ts[i]))
                    {
                        var tss = forecast.Skip(Math.Max(0, i - _R.Length)).Take(_R.Length).ToArray();
                        var x = _R.ToMatrix().Dot(tss.ToMatrix(true));
                        forecast.Add(x[0, 0]);
                    }
                    else
                        forecast.Add(this._ts[i]);

                }
                else
                {
                    // 
                    var tss = forecast.Skip(i-_R.Length).Take(_R.Length).ToArray();
                    var x = _R.ToMatrix().Dot(tss.ToMatrix(true));
                    forecast.Add(x[0,0]);
                }
            }

            return forecast.ToArray();
        }



        void prepareForecast(int singularsValuesCount)
        {
            double vertCoeff = 0;
            double[][] forecastOrthonormalVal;
            var L = _xCom.GetLength(0);
            var K = _xCom.GetLength(1);
            var X_com_hat = MatrixEx.Zeros(L, K);

            if (singularsValuesCount >= 0)
            {
                //check if the count greater of orthonormal matrix length
                var len = Math.Min(singularsValuesCount, _orthonormalBase.Length);

                forecastOrthonormalVal = new double[len][];
                for (int i = 0; i < len; i++)
                  forecastOrthonormalVal[i] = _orthonormalBase[i];
            }
            else
                forecastOrthonormalVal = _orthonormalBase;

            var valR = MatrixEx.Zeros(forecastOrthonormalVal[0].Length, 1);
            var tmp = valR.GetColumn(valR.GetLength(1) - 1);
            _R = tmp.Take(tmp.Count() - 1).ToArray();

            //
            for (int i = 0; i < forecastOrthonormalVal.Length; i++)
            {
                //
                var PI = forecastOrthonormalVal[i];
                var prod = PI.ToMatrix(true).Dot(PI.ToMatrix(false));
                var temp = prod.Dot(_xCom);
                X_com_hat = X_com_hat.Add(temp);
                //
                var pi = PI.Last();
                vertCoeff += pi * pi;
                var rr = PI.Take(PI.Length - 1).ToArray().Multiply(pi);
                _R = _R.Add(rr);
            }

            //
            _R = _R.Divide((1.0 - vertCoeff));
            X_com_tilde = diagonalAveraging(X_com_hat);
        }

        /// <summary>
        /// Perform SSA for N lagged steps
        /// </summary>
        /// <param name="embeddingDim"></param>
        public void Fit(uint embeddingDim)
        {
            Embedding(embeddingDim);
            Decompose();
        }

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
            var stepAhead = validTS.Count();

            //create training loop
            var eval = new List<(int i, int rc, double rmse)>();
            
            foreach(var i in Enumerable.Range(3,maxSignalCount))
            {
                //perform analysis with i signals
                Fit((uint)i);
                var r1 = double.PositiveInfinity;
                var rec = 3;
                foreach (var rc in Enumerable.Range(3, maxSignalCount-3))
                {
                    //call forecast method and pass the number of time steps of steps ahead for forecasting
                    var predicted = Forecast(stepAhead, rc);
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
                
                
                eval.Add((i, rec,r1));
            }

            //return top ten results
            return eval.OrderBy(x=>x.rmse).Take(10).ToList();
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
            var values = ssa.Forecast(horizont, recoveryCount);
            //
           // var modelValue = values.Take(ts_Train.Count()).ToArray();
            var modelPredict = values.Skip(ts_Train.Count()).ToList();
            modelPredict.Insert(0, modelValue.Last());
            return Plot((x0, ts_Train.ToArray(), modelValue), (x1, ts_Test, modelPredict.ToArray()));
        }

        /// <summary>
        /// Plot train and test sets
        /// </summary>
        /// <param name="train"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        private static PlotlyChart Plot((object[] x, double[] actual, double[] predicted) train, (object[] x, double[] actual, double[] predicted) test)
        {
            var layout = new Layout.Layout();
            layout.title = "Singular Spectrum Analysis Forecast";
            layout.showlegend = true;
            layout.plot_bgcolor = "rgb(223,223,223)";


            var scatters1 = new Graph.Scatter() { name = "Actual", x = train.x, y = train.actual, mode = "line", };
            var scatters2 = new Graph.Scatter() { name = "Predicted", x = train.x, y = train.predicted, mode = "line", };
            var scatters3 = new Graph.Scatter() { name = "Test Actual", x = test.x, y = test.actual, mode = "line", };
            var scatters4 = new Graph.Scatter() { name = "Forecast", x = test.x, y = test.predicted, mode = "line", };
            var chart = XPlot.Plotly.Chart.Plot<Graph.Trace>(new Graph.Trace[] { scatters1, scatters2, scatters3, scatters4 });
            chart.WithLayout(layout);
            return chart;
            //chart.Show();
        }
    }
}
