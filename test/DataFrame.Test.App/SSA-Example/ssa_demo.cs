using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using Microsoft.ML.Data;
using Microsoft.ML;
using Microsoft.ML.Transforms;

using Accord.Math;
using System.Threading.Tasks;
using OxyPlot;
using System.Windows.Forms;
using Daany;
using Daany.MathExt;
using Daany.Stat;
using Daany.Plot;

namespace ML.Net.App.TimeSeries
{
    //Singular Sectrum Analysis
    public class SSADemo
    {
        static string root = "..\\..\\..\\..\\..\\dataset"; 

        public static void Forecasting()
        {
            //
            var strPath = $"AirPassengers.csv";
            var mlDF = DataFrame.FromCsv(strPath, sep: ",");
            var ts = mlDF["#Passengers"].Select(f => Convert.ToDouble(f));//create time series

            //create Singular Spectrum Analysis object
            var ssa = new SSA(ts);
            //perform analysis
            ssa.Fit(36);

            //helper vars to hold data
            int n = 4;
            var setIndex = Enumerable.Range(0, n);
            double[][] ts_components = new double[setIndex.Count()][];

            //cumulative components array
            double[] cumTS = new double[ts.Count()];

            //take the first n components and plot it
            var models = new List<PlotModel>();
            foreach (var i in setIndex)
            {
                ts_components[i] = ssa.Reconstruct(ssa.Xs[i]);
                cumTS = cumTS.Add(ts_components[i]);
                var m11 = Daany.Plot.ChartComponent.LinePlot("", $"TS Components {i+1}",
                    Enumerable.Range(1,ts_components[i].Length).Select(t=>(double)t).ToArray(), ts_components[i],System.Drawing.Color.Blue, OxyPlot.MarkerType.Plus);
                //
                models.Add(m11);
            }
            //plot component together
            ChartComponent.ShowPlots("Passenger TS Components",models.ToArray()).Wait();
            
            //plot sum of the components
            var x = Enumerable.Range(1, cumTS.Length).Select(t => (double)t).ToArray();
            var m1 = Daany.Plot.ChartComponent.LineSeries("Predicted SSA",
                    x,cumTS, System.Drawing.Color.Blue, OxyPlot.MarkerType.Plus);
            var m2 = Daany.Plot.ChartComponent.LineSeries("Original Time Series",
                   x, ts.ToArray(), System.Drawing.Color.Red, OxyPlot.MarkerType.Plus);

            var model = new PlotModel();
            model.Series.Add(m1);
            model.Series.Add(m2);
            ChartComponent.ShowPlots("Passenger TS vs Predicted",model).Wait();

            //Forecasting of time series
            var stepAHead = 12;
            var predictedTS = ssa.Reconstruct(15);
            var predicted = ssa.Forecast(stepAHead);

            //create three plotmodel: actual TS, predicted TS, Forcast ts ahead
            var mm3 = Daany.Plot.ChartComponent.LineSeries("Predicted Time Series",
                  x, predictedTS, System.Drawing.Color.Green, OxyPlot.MarkerType.Plus);
            var mm2 = Daany.Plot.ChartComponent.LineSeries("Original Time Series",
                   x, ts.ToArray(), System.Drawing.Color.Red, OxyPlot.MarkerType.Plus);
            
            //forecast steps ahead
            var x1 = Enumerable.Range(cumTS.Length, stepAHead+1).Select(t => (double)t).ToArray();
            //start with the last point form the times series
            var y = predicted.Skip(ts.Count()-1).Take(stepAHead+1).ToArray();
            var mm1 = Daany.Plot.ChartComponent.LineSeries($"Forecast of {stepAHead}",
                   x1, y, System.Drawing.Color.Blue, OxyPlot.MarkerType.Plus);
            //add series to model 
            var model1 = new PlotModel();
            model1.Series.Add(mm1);
            model1.Series.Add(mm2);
            model1.Series.Add(mm3);

            //plot forecasting
            ChartComponent.ShowPlots("Passenger TS vs Forecasting", model1).Wait();
        }
    }

  
}
