using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Daany.Stat;

namespace Daany.Stat.Arima
{
    /*
     * ARIMA Method (method)
     0 - Exact Maximum Likelihood Method (Default)
    1 - Conditional Method - Sum Of Squares
    2 - Box-Jenkins Method
     */
    public enum ARIMAMethod
    {
        MLE=0,
        CSS=1,
        BoxJenkins=2
    }
    /*
     Optimization Method (optmethod)
    optmethod accepts values between 0 and 7 where -
    Method 0 - Nelder-Mead
    Method 1 - Newton Line Search
    Method 2 - Newton Trust Region - Hook Step
    Method 3 - Newton Trust Region - Double Dog-Leg
    Method 4 - Conjugate Gradient
    Method 5 - BFGS
    Method 6 - Limited Memory BFGS
    Method 7 - BFGS Using More Thuente Method (Default)
     */
    public enum OptMethod
    {
        NM = 0,
        NLS = 1,
        NTRHS = 2,
        NTRDDL=3,
        CG=4,
        BFGS=5,
        LMBFGS=6,
        BFGSUMTM=7
    }

    /*
     ‘p’ is the order of the ‘Auto Regressive’ (AR) term. 
      'p' is the number of lags of Y to be used as predictors.
     ‘q’ is the order of the ‘Moving Average’ (MA) term. 
      'q' is the number of lagged forecast errors that should go into the ARIMA Model.
     */
    public class ARIMA
    {
        //the order of the AR term
        public int p { get; set; }
        //the number of differencing required to make the time series stationary
        public int d { get; set; }
        //the order of the MA term
        public int q { get; set; }
        public ARIMAMethod ArimaMehtod { get; set; }
        public OptMethod OptMethod { get; set; }


        ///*
        // If a time series, has seasonal patterns, seasonal terms has to be added
        // It becomes SARIMA model, short for ‘Seasonal ARIMA’ with capitals P, D, and Q.
        // */
        ////the order of the AR term
        //public int P { get; set; }
        ////the number of differencing required to make the time series stationary
        //public int D { get; set; }
        ////the order of the MA term
        //public int Q { get; set; }


        public ARIMA(int pp, int dd, int qq, int PP=0, int DD=0, int QQ=0)
        {
            #region Initialization
            p = pp;
            d = dd;
            q = qq;
            ArimaMehtod = ARIMAMethod.MLE;
            OptMethod = OptMethod.BFGSUMTM;
            //
            //P = PP;
            //D = DD;
            //Q = QQ;
            #endregion
        }
        public ARIMA()
        {

        }

        public float[] AR(Series ts, int order)
        {
            throw new NotImplementedException();
           // (float[,] X, float[,] Y) = ts.ToRegressors(order);

            /////Least Square Solve
            ////var retVal = MagmaSharp.LinAlg.Lss(X, Y);

            ////
            //var args = new List<float>();
            //for (int i = 0; i < retVal.GetLength(0); i++)
            //    args.Add(retVal[i,0]);
            //return args.ToArray();
        }

        public float[] MA(Series ts, int order)
        {
            throw new NotImplementedException();
            //MAModel m = new MAModel(ts.Select(x=> Convert.ToDouble(x)).ToArray(), order);
            //var coeff = m.Fit();

            //return coeff.Select(x=> Convert.ToSingle(x)).ToArray();
        }


        /// <summary>
        /// Find the best possible ARIMA parameters (p, d, q)
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public (int p, int d, int q) Fit(Series ts)
        {
            double[] tds = ts.Select(x => Convert.ToDouble(x)).ToArray();
            var regg = ctsabinding.arima(p, d, q, ArimaMehtod, OptMethod, tds);
            //first make differencing of the ts
            //if(d > 0)
            //{
            //  var dts = differencing(ts);
            //}

            return (0, 0, 0);
        }


        //
        /// <summary>
       
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        private Series differencing(Series ts)
        {
            return null;
        }

    }
}
