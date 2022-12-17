using System;
using System.Collections.Generic;
using System.Text;

namespace Daany.Stat.stl
{
    public class STL
    {

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
        /// <returns>Tuple of the three components</returns>
        public static (double[] Trend, double[] Seasonal, double[] Residual) Fit(double[] values,
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

            return (stl.Trend, stl.Seasonal, stl.Residual);

        }
    }
}
