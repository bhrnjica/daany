using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Daany.Arima
{
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

        /*
         If a time series, has seasonal patterns, seasonal terms has to be added
         It becomes SARIMA model, short for ‘Seasonal ARIMA’ with capitals P, D, and Q.
         */
        //the order of the AR term
        public int P { get; set; }
        //the number of differencing required to make the time series stationary
        public int D { get; set; }
        //the order of the MA term
        public int Q { get; set; }


        public ARIMA(int pp, int dd, int qq, int PP=0, int DD=0, int QQ=0)
        {
            #region Initialization
            p = pp;
            d = dd;
            q = qq;
            //
            P = PP;
            D = DD;
            Q = QQ;
            #endregion
        }
        public ARIMA()
        {

        }

        public float[] AR(Series ts, int order)
        {
            (float[,] X, float[,] Y) = ts.ToRegressors(order);

            ///Least Square Solve
            var retVal = MagmaSharp.LinAlg.Lss(X, Y);

            //
            var args = new List<float>();
            for (int i = 0; i < retVal.GetLength(0); i++)
                args.Add(retVal[i,0]);
            return args.ToArray();
        }
        /// <summary>
        /// Find the best possible ARIMA parameters (p, d, q)
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public (int p, int d, int q) Fit(Series ts)
        {
            
            //first make differencing of the ts
            if(d > 0)
            {
              var dts = differencing(ts);
            }

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
