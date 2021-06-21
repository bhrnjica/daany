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
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Daany;

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
