//////////////////////////////////////////////////////////////////////////////
//   ____    _    _   _   _   __  __                                       //
//  |  _ \  / \  | \ | | | \ | |\ \/ /                                     //
//  | | | |/ _ \ |  \| | |  \| | \  /                                      //
//  | |_| / ___ \| |\  | | |\  | | |                                       //
//  |____/_/   \_\_| \_| |_| \_| |_|                                       //
//                                                                         //
//  DAata ANalYtics Library                                                //
//  Daany.DataFrame:Implementation of DataFrame.                           //
//  https://github.com/bhrnjica/daany                                      //
//                                                                         //
//  Copyright © 20019-2025 Bahrudin Hrnjica                                //
//                                                                         //
//  Free. Open Source. MIT Licensed.                                       //
//  https://github.com/bhrnjica/daany/blob/master/LICENSE                  //
//////////////////////////////////////////////////////////////////////////////
using System.Linq;

using XPlot.Plotly;

namespace Daany.Ext
{
	/// <summary>
	/// Set of extension methods for the Daany.DataFrame related to Machine Learning and Data Transformation.
	/// </summary>
	public static partial class DataFrameExt
    {
        public static PlotlyChart Plot(string xLabel, string yLabel, params Series[] series)
        {

            var scatters = series.Select(series => new Scatter()
            {
                name = series.Name,

                x = series.Index.ToArray(),
                y = series.ToArray(),
                mode = "line",
            });



            var chart = XPlot.Plotly.Chart.Plot(scatters);
            chart.WithXTitle(xLabel);
            chart.WithYTitle(yLabel);
            return chart;
        }

        public static PlotlyChart Plot(this Series[] series, string xLabel, string yLabel)
        {

            return Plot(xLabel,yLabel, series);
        }

        public static PlotlyChart Plot(this DataFrame dataframe, string xLabel, string yLabel)
        {
            var series = dataframe.ToSeries(dataframe.Columns.ToArray());
            return Plot(xLabel, yLabel, series);
        }

    }

   
}
