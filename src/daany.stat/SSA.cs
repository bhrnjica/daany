//////////////////////////////////////////////////////////////////////////////////////////
// Daany - DAta ANalYtics Library                                                        //
// https://github.com/bhrnjica/daany                                                    //
//                                                                                      //
// Copyright 2006-2018 Bahrudin Hrnjica                                                 //
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
using Daany.MathExt;

namespace Daany.Stat
{
    public class TSComponents
    {
        public double[] Seasonal { get; set; }
        public double[] Trend { get; set; }
        public double[] Residual { get; set; }
    }
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

        double[] _ts;
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
            //norm of ghe embedding matrix
            double frob_norm = X.Euclidean();
            //
            var contr = lambdas.Divide(frob_norm * frob_norm);
            //return only positive contributions
            return contr.Select(x => Math.Round(x, 4)).Where(x => x > 0).ToArray();
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
        /// transform each matrix XIj of the grouped decomposition into a new series of length N
        /// </summary>
        /// <returns>elementary reconstructed series</returns>
        private double[] diagonalAveraging(double[,] signalMatrix)
        {
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
                        yk += Y[i - 1, j - 1];//zero based index
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
                        yk += Y[i - 1, j - 1];//zero based index
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
                        yk += Y[i - 1, j - 1];//zero based index
                    }
                    //
                    y[k - 1] = yk / (N - k + 1);
                }
                else
                    throw new Exception("This should not be happen!");
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
            //initi ts
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
        ///Forecasting by SSA can be applied to time series that approximately satisfy
        /// linear recurrent formulae(LRF). The series Y_T satisfies an LRF of order d if there are numbers a1, . . . , ad such that
        /// 
        /// y_(i+d)=SUM_(k=1)^d(a_k y_(i+d-1) ); (1 <= i <= T-d)
        /// 
        /// </summary>
        /// <param name="stepsAhead"></param>
        /// <param name="singularValues"></param>
        /// <returns></returns>
        public double[] Forecast(int stepsAhead=12, int singularValues=-1)
        {
            //prepare for forecasting by calculation necessary values
            prepareForecast(singularValues);

            //
            var forecast = new List<double>();

            //fill the first element of time series
            forecast.Add(this._ts[0]);
            for (int i=1; i < this._ts.Length+stepsAhead; i++)
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
                    // x = self.R.T * m(self.ts_forecast[max(0, i - self.R.shape[0]): i]).T
                    
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
            _R = _R.Divide((1.0 - vertCoeff));
            X_com_tilde = diagonalAveraging(X_com_hat);
        }

        public void Fit(uint embeddingDim)
        {
            Embedding(embeddingDim);
            Decompose();
        }
    }
}
