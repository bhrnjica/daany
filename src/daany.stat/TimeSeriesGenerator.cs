using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Daany.stl;

namespace Daany.Stat
{
    /// <summary>
    /// Class implementation for common time series operations
    /// </summary>
    public class TimeSeriesGen
    {
        public static double[] SimpleMA(double[] data, int window)
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
        /// Perform STL time series decomposition
        /// </summary>
        /// <param name="values"></param>
        /// <param name="sWindow"></param>
        /// <param name="sDegree"></param>
        /// <param name="tWindow"></param>
        /// <param name="tDegree"></param>
        /// <param name="lWindow"></param>
        /// <param name="lDegree"></param>
        /// <param name="sJump"></param>
        /// <param name="tJump"></param>
        /// <param name="lJump"></param>
        /// <param name="isRobust"></param>
        /// <param name="inner"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        public static TSComponents STLDecomposition(double[] values, 
            int sWindow, int sDegree = 0, int tWindow = 0, int tDegree = 1, int lWindow = 0, int lDegree = 0, 
            int sJump = 0, int tJump = 0, int lJump = 0, bool isRobust = false, int inner = 0, int outer = 0)
        {
            var builder = new SeasonalTrendLoessBuilder();
            builder.PeriodLength = sWindow;    // Data has a period of 12
            builder.Periodic = sWindow > 0;
            //  setSeasonalWidth(12).   // Monthly data smoothed over 35 years
            builder.Robust = isRobust;         // Not expecting outliers, so no robustness iterations


            var smoother = builder.buildSmoother(values);

            var stl = smoother.decompose();
            var retVal = new TSComponents();
            retVal.Residual = stl.Residual;
            retVal.Trend = stl.Trend;
            retVal.Residual = stl.Residual;
            return retVal;

        }
        
        
        public static (List<List<float>> X, List<List<float>> y) ToDataFrame(List<float> tsData, int pastSteps, int futureSteps = 1)
        {
            ///
            var feature = new List<List<float>>();
            var labels = new List<List<float>>();


            for (int i = 0; i < tsData.Count - futureSteps; i++)
            {
                //if timeLagg 
                if (pastSteps > i + 1)
                    continue;
                var fRow = new List<float>();
                for (int j = 1 - pastSteps; j <= 0; j++)
                {
                    float lValue = tsData[i + j];
                    fRow.Add(lValue);
                }
                //add one row to feature dataset
                feature.Add(fRow);

                //label
                var lRow = new List<float>();
                for (int k = 1; k <= futureSteps; k++)
                {
                    float lValue = tsData[i + k];
                    lRow.Add(lValue);
                }

                //add one row to label
                labels.Add(lRow);
            }

            return (feature, labels);
        }

        public static (List<List<float>> X, List<List<float>> y, List<List<float>> Xtest, List<List<float>> ytest) ToTTDataFrame(List<float> tsData, int pastSteps, int futureSteps)
        {

            //split file into test and train part before data processing
            var countData = tsData.Count();
            var train = tsData.Take(countData - futureSteps);
            var test = tsData.Skip(Math.Max(0, countData - futureSteps)).Select(x => x);

            var (X, y) = ToDataFrame(train.ToList(), pastSteps, futureSteps);

            //In order to create test set the number of instances of y must be greater than number of past steps.
            if (pastSteps > y.Count)
                throw new Exception("The number of dataset instances must be greater than the number of past steps");

            //
            var ytest = new List<List<float>>() { test.ToList() };

            //features for the test dataset is generated from the train set. In case the sequence length is greated than ytrain langth
            //the rest of value are taken from the Xfeatures sequences
            var xtest = new List<float>();
            //
            for (int i = 0; i < pastSteps; i++)
            {
                var index = y.Count - 1 - i;
                var value = y[index].Last();
                xtest.Add(value);
            }
            xtest.Reverse();
            var Xtest = new List<List<float>>() { xtest };

            return (X, y, Xtest, ytest);
        }

    }
}
