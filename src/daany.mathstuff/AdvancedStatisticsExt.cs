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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Math;

namespace Daany.MathStuff
{
    /// <summary>
    /// Implement extension methods for statistics calculation between two data sets X and Y eg. sum of square error, pearson coeff,... 
    /// 
    /// </summary>
    public static class AdvancedStatistics
    {
       
        /// <summary>
        /// Calculate Classification Accuracy
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double CA(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //
            double corected = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                if(obsData[i] == preData[i])
                    corected ++;
            }
            return corected/obsData.Length;
        }

        

        /// <summary>
        /// Calculate Classification Error
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double CE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //
            double corected = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                if (obsData[i] == preData[i])
                    corected++;
            }
            return 1.0 - corected / obsData.Length;
        }

        /// <summary>
        /// Calculate Mean Square Error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double MSE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);
            var se = SE(obsData, preData);
            return se.Sum() / obsData.Length;
        }





        /// <summary>
        /// Calculates Root Mean Square error
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double RMSE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            return Sqrt(MSE(obsData, preData));
        }

        /// <summary>
        /// Calculate Absolute error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double[] AE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //
            double[] ae = new double[obsData.Length];
            for (int i = 0; i < obsData.Length; i++)
            {
                var r = Abs(obsData[i] - preData[i]);
                ae[i]= r;
            }
            return ae;
        }

        /// <summary>
        /// Calculates Mean Absolute error
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double MAE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);
            var ae = AE(obsData, preData);
            //c
            return ae.Sum() / obsData.Length;
        }

        /// <summary>
        /// Calculate Absolute percent error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double[] APE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //
            double[] retVal = new double[obsData.Length];
            for (int i = 0; i < obsData.Length; i++)
            {
                var r = Abs(obsData[i] - preData[i])/ obsData[i];
                retVal[i] = r;
            }
            return retVal;
        }

        /// <summary>
        /// Calculate Mean Absolute Percent Error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double MAPE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);
            var ae = APE(obsData, preData);
            //c
            return ae.Sum() / obsData.Length;
        }

        /// <summary>
        /// Calculate Symmetric Mean Absolute Percent Error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double SMAPE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //
            double retVal = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                var r = Abs(obsData[i] - preData[i]) / (Abs(obsData[i])+ Abs(preData[i]));
                retVal += r;
            }

            return 2 * retVal / (double)obsData.Length;
        }


        /// <summary>
        /// Calculate Squared Log Error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns>Sum of all SLE</returns>
        public static double[] SLE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //
            double[] retVal = new double[obsData.Length];
            for (int i = 0; i < obsData.Length; i++)
            {
                var r = Pow(Log(1.0 + obsData[i]) - Log(1.0 + preData[i]),2);
                retVal[i] = r;
            }

            return retVal;
        }

        /// <summary>
        /// Calculate Mean Squared Log Error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double MSLE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            var retVal = SLE(obsData, preData);

            return retVal.Sum() / obsData.Length;
        }


        /// <summary>
        /// Calculate Mean Squared Log Error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double RMSLE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            return Sqrt(MSLE(obsData, preData));
        }


        
        /// <summary>
        /// Calculates squares of the two vectors
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double[] SE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //calculate sum of the square residuals
            double[] se = new double[obsData.Length];
            for(int i=0; i < obsData.Length; i++)
            {
                var r = (obsData[i] - preData[i]);
                se[i]= r * r;
            }
               
            return se;
        }

        /// <summary>
        /// Calculate Relative Square Error of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double RSE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);
            var mean = obsData.MeanOf();
            var se = SE(obsData, preData);
            //calculate sum of the square residuals
            double ssr = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                var r = (obsData[i] - mean);
                ssr += r * r;
            }


            return se.Sum() / ssr;
        }


        /// <summary>
        /// Calculate Root Relative Squared Error between two vectors
        /// </summary>
        /// <param name="obsData"></param>
        /// <param name="preData"></param>
        /// <returns></returns>
        public static double RRSE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);
            return Sqrt(RSE(obsData, preData));
        }

        /// <summary>
        /// Calculates Relative Absolute Error between two vecotrs
        /// </summary>
        /// <param name="obsData"></param>
        /// <param name="preData"></param>
        /// <returns></returns>
        public static double RAE(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);
            var mean = obsData.MeanOf();
            var se = AE(obsData, preData);
            //calculate sum of the square residuals
            double ssr = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                var r = Abs(obsData[i] - mean);
                ssr += r;
            }


            return se.Sum() / ssr;
        }

        /// <summary>
        /// Mean Absolute Scaled Error
        ///Description
        /// mase computes the mean absolute scaled error between two numeric vectors.
        /// Function is only intended for time series data, where actual and numeric are numeric vectors ordered by time.
        /// </summary>
        /// <param name="obsData"></param>
        /// <param name="preData"></param>
        /// <param name="stepSize">A positive integer that specifies how many observations to look back in time in 
        ///                         order to compute the naive forecast. The default is 1, which means that the 
        ///                         naive forecast for the current time period is the actual value of the previous period.</param>
        /// <returns></returns>
        public static double MASE(this double[] obsData, double[] preData, int stepSize = 1)
        {
            checkDataSets(obsData, preData);

            int n = obsData.Length;
            int m = stepSize;
            var ae = AE(obsData, preData);
            var retVal = 0.0;
            for (int i = m; i < n; i++)
            {
                var r = Abs(obsData[i] - obsData[i - m]);
                retVal += r;
            }

            return ae.Sum() / (n * retVal / (n - m));
        }


        /// <summary>
        /// Calculate Log Loss between two vectors
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double[] LL(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //
            double[] retVal = new double[obsData.Length];
            for (int i = 0; i < obsData.Length; i++)
            {
                retVal[i] = obsData[i] * Log(preData[i]) + (1.0 - obsData[i]) * Log(1.0 - preData[i]);

            }
            return retVal;
        }

        /// <summary>
        /// Calculate Mean Log Loss between two vectors
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double LOGLOS(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //
            double retVal = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                var r = obsData[i] * Log(preData[i]) + (1.0 - obsData[i]) * Log(1.0 - preData[i]);
                retVal += r;

            }
            return retVal;
        }



        /// <summary>
        /// Calculates Pearson correlation coefficient of two data sets
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double R(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            //calculate average for each dataset
            double aav = obsData.MeanOf();
            double bav = preData.MeanOf();

            double corr = 0;
            double ab = 0, aa = 0, bb = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                var a = obsData[i] - aav;
                var b = preData[i] - bav;

                ab += a * b;
                aa += a * a;
                bb += b * b;
            }

            corr = ab / Sqrt(aa * bb);

            return corr;
        }

        /// <summary>
        /// Calculates Coefficient of Determination (Square of Pearson coeff)
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double R2(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            var r = R(obsData, preData);
            return r*r;
        }
        /// <summary>
        /// The Nash-Sutcliffe efficiency (NSE)  proposed by Nash and Sutcliffe (1970) 
        /// is defined as one minus the sum of the absolute squared differences between 
        /// the predicted and observed values normalized by the variance of the observed
        /// values during the period under investigation.
        /// 
        /// The Nash-Sutcliffe efficiency (NSE) is a normalized statistic
        /// that determines the relative magnitude of the residual variance (“noise”) compared to the 
        /// measured data variance (“information”) (Nash and Sutcliffe, 1970). NSE indicates how well 
        /// the plot of observed versus simulated data fits the 1:1 line. 
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns>NSE ranges between −∞ and 1.0 (1 inclusive), with NSE = 1 being the optimal value. 
        /// Values between 0.0 and 1.0 are generally viewed as acceptable levels of performance, 
        /// whereas values lower than 0.0 indicates that the mean observed value is a better predictor 
        /// than the simulated value, which indicates unacceptable performance.</returns>
        public static double NSE (this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            var se = SE(obsData, preData);

            //calculate the mean
            var mean = obsData.MeanOf();
            //calculate sum of square 
            double ose = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                var res = (obsData[i] - mean);

                ose += res * res;
            }

            //calculate NSE
            var nse = 1.0 - se.Sum() / ose;
            return nse;
        }

        /// <summary>
        /// Percent bias (PBIAS) measures the average tendency of the simulated data to be larger or smaller 
        /// than their observed counterparts.The optimal value of PBIAS is 0.0, 
        /// with low-magnitude values indicating accurate model simulation. 
        /// Positive values indicate model underestimation bias, and negative values indicate model 
        /// overestimation bias. 
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double PBIAS(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);
            var ae = AE(obsData, preData);
            var pbias =  ae.Sum() / obsData.Sum();

            return pbias;
        }

        /// <summary>
        /// RSR standardizes RMSE using the observations standard deviation, and it combines both 
        /// an error index and the additional information recommended by Legates and McCabe (1999).
        /// RSR is calculated as the ratio of the RMSE and standard deviation of measured data.
        /// RSR incorporates the benefits of error index statistics and includes a scaling/normalization factor,
        /// so that the resulting statistic and reported values can apply to various constituents. 
        /// RSR varies from the optimal value of 0, which indicates zero RMSE or residual variation 
        /// and therefore perfect model simulation, to a large positive value. The lower RSR, the 
        /// lower the RMSE, and the better the model simulation performance.
        /// </summary>
        /// <param name="obsData">Observer data</param>
        /// <param name="preData">Predicted data</param>
        /// <returns></returns>
        public static double RSR(this double[] obsData, double[] preData)
        {
            checkDataSets(obsData, preData);

            var rmse = RMSE(obsData, preData);
            var rsr = obsData.Stdev();

            return rmse/rsr;
        }

        /// <summary>
        /// Implementation of Moving Average
        /// </summary>
        /// <param name="data"></param>
        /// <param name="window">Window size</param>
        /// <returns></returns>
        public static double[] MA(this double[] data, int window)
        {
            double[] average = new double[data.Length - window + 1];

            // The simple moving average picks up one point from the first "window" points of the original data 
            // and then data.length - window additional points for the rest of the data.

            double windowSum = 0.0;
            for (int i = 0; i < window; ++i)
            {
                windowSum += data[i];
            }

            average[0] = windowSum / window;

            // Now roll through the additional data subtracting the contribution from the index that has left the window
            // and adding the contribution from the next index to enter the window. Last window above was [0, window - 1],
            // so we need to start by removing data[0] and adding data[window] and move forward until we add the
            // last data point; i.e. until windowEnd == data.length - 1.
            int windowEnd = window;
            int windowStart = 0;
            for (int j = 1; j < data.Length - window + 1; ++j)
            {
                // loops data.length - window + 1 - 1 = data.length - window times
                windowSum += (data[windowEnd] - data[windowStart]);
                ++windowStart;
                ++windowEnd;
                average[j] = windowSum / window;
            }

            return average;
        }

        /// <summary>
        /// transforms the 2D row based array into 2D column based array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T[][] ToColumnVector<T>(this T[][] input)
        {
            var colVecData = new List<T[]>();
            for (int j = 0; j < input[0].Length; j++)
            {
                var col = new T[input.Length];

                for (int i = 0; i < input.Length; i++)
                {
                    col[i] = input[i][j];

                }

                colVecData.Add(col);
            }
            return colVecData.ToArray();
        }

        private static void checkDataSets(double[] obsData, double[] preData)
        {
            if (obsData == null || obsData.Length < 2)
                throw new Exception("'observed Data' cannot be null or empty!");

            if (preData == null || preData.Length < 2)
                throw new Exception("'predicted data' cannot be null or empty!");

            if (obsData.Length != preData.Length)
                throw new Exception("Both datasets must be of the same size!");
        }
    }
}
