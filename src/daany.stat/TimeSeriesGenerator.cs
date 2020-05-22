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
using Daany.stl;

namespace Daany.Stat
{
    public class TSComponents
    {
        public double[] Seasonal { get; set; }
        public double[] Trend { get; set; }
        public double[] Residual { get; set; }
    }

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



    }
}
