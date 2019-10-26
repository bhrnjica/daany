using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Daany.Plot
{
    public class ChartComponent
    {
        public static PlotModel BarPlot(string title, List<object> x, IEnumerable<double> y, string xLabel = "", string yLabel = "")
        {
            var model = new PlotModel { Title = title };

            var barSeries = new ColumnSeries();
            var bars = new List<ColumnItem>();
            for (int i = 0; i < y.Count(); i++)
            {
                var b = new ColumnItem(y.ElementAt(i), i);
                bars.Add(b);
            }
            barSeries.ItemsSource = bars;
            //add category axis
            var xAxes = new CategoryAxis
            {
                Title = xLabel,
                Position = AxisPosition.Bottom,
                Key = "Target",
                ItemsSource = x.ToArray(),
            };
            xAxes.ActualLabels.AddRange(x.Select(xx => xx.ToString()));
            model.Axes.Add(xAxes);


            //add category axis
            var yAxes = new LinearAxis()
            {
                Title = yLabel,
                Position = AxisPosition.Left,


            };
            model.Axes.Add(yAxes);

            model.Series.Add(barSeries);



            //
            return model;

        }

        public static PlotModel ScaterPlot(string title, double[] x, double[] y, System.Drawing.Color markerColor, MarkerType mType, string xLabel = " ", string yLabel = " ")
        {
            var model = new PlotModel { Title = "Class histogram" };
            var scater = ScatterSeries(x, y, markerColor, mType);

            model.Series.Add(scater);
            //add category axis
            var xAxes = new LinearAxis
            {
                Title = xLabel,
                Position = AxisPosition.Bottom,
            };
            model.Axes.Add(xAxes);


            //add category axis
            var yAxes = new LinearAxis()
            {
                Title = yLabel,
                Position = AxisPosition.Left,
            };
            model.Axes.Add(yAxes);
            //
            return model;
        }

        public static PlotModel ScaterPlot(string title, List<object> x, double[] y, System.Drawing.Color markerColor, MarkerType mType, string xLabel = " ", string yLabel = " ")
        {
            var model = new PlotModel { Title = "Class histogram" };
            var scater = ScatterSeries(x, y, markerColor, mType);

            model.Series.Add(scater);
            //add category axis
            var xAxes = new DateTimeAxis
            {
                Title = xLabel,
                Position = AxisPosition.Bottom,

                
            };
            model.Axes.Add(xAxes);


            //add category axis
            var yAxes = new LinearAxis()
            {
                Title = yLabel,
                Position = AxisPosition.Left,
            };
            model.Axes.Add(yAxes);
            //
            return model;
        }

        public static LineSeries LineSeries(string seriesTitle, double[] x, double[] y, System.Drawing.Color markerColor, MarkerType mType)
        {
            var lineSeries = new LineSeries();
            lineSeries.Title = seriesTitle;
            for (int i = 0; i < x.Length; i++)
            {
                var b = new DataPoint(x[i], y[i]);

                lineSeries.Points.Add(b);
            }
            lineSeries.Color= OxyColor.FromRgb(markerColor.R, markerColor.G, markerColor.B);
            //lineSeries.MarkerFill = 
            lineSeries.MarkerType = mType;
            return lineSeries;
        }

        public static LineSeries LineSeries(string seriesTitle, DateTime[] x, double[] y, System.Drawing.Color markerColor, MarkerType mType)
        {
            var lineSeries = new LineSeries();
            lineSeries.Title = seriesTitle;
            for (int i = 0; i < x.Length; i++)
            {
                var b = DateTimeAxis.CreateDataPoint(x[i], y[i]);

                lineSeries.Points.Add(b);
            }
            lineSeries.Color = OxyColor.FromRgb(markerColor.R, markerColor.G, markerColor.B);
            //lineSeries.MarkerFill = 
            lineSeries.MarkerType = mType;
            return lineSeries;
        }

        public static ScatterSeries ScatterSeries(double[] x, double[] y, System.Drawing.Color markerColor, MarkerType mType)
        {
            var scatterSeries = new ScatterSeries();
            // var scatter = new List<DataPoint>();

            for (int i = 0; i < x.Length; i++)
            {
                var b = new ScatterPoint(x[i], y[i], 4.0, 1);

                scatterSeries.Points.Add(b);
            }
            scatterSeries.MarkerFill = OxyColor.FromRgb(markerColor.R, markerColor.G, markerColor.B);
            scatterSeries.MarkerType = mType;
            return scatterSeries;
        }

        public static ScatterSeries ScatterSeries(List<object> x, double[] y, System.Drawing.Color markerColor, MarkerType mType)
        {
            var scatterSeries = new ScatterSeries();
           // var scatterSeries = new List<DataPoint>();

            for (int i = 0; i < x.Count; i++)
            {
                var b = new ScatterPoint(CategoryAxis.ToDouble(x[i]), y[i]);
                scatterSeries.Points.Add(b);
            }
            scatterSeries.MarkerFill = OxyColor.FromRgb(markerColor.R, markerColor.G, markerColor.B);
            scatterSeries.MarkerType = mType;
            return scatterSeries;
        }

        public static PlotModel LinePlot(string titlePlot, string titleSeries, double[] x, double[] y, System.Drawing.Color markerColor, MarkerType mType, string xLabel = " ", string yLabel = " ")
        {
            var model = new PlotModel { Title = titlePlot };
            var scater = LineSeries(titleSeries, x, y, markerColor, mType);

            model.Series.Add(scater);
            //add category axis
            var xAxes = new LinearAxis
            {
                Title = xLabel,
                Position = AxisPosition.Bottom,
            };
            model.Axes.Add(xAxes);


            //add category axis
            var yAxes = new LinearAxis()
            {
                Title = yLabel,
                Position = AxisPosition.Left,
            };
            model.Axes.Add(yAxes);
            //
            return model;
        }

        public static PlotModel LinePlot(string titlePlot, string titleSeries, DateTime[] x, double[] y, System.Drawing.Color markerColor, MarkerType mType, string xLabel = " ", string yLabel = " ")
        {
            var model = new PlotModel { Title = titlePlot };
            var scater = LineSeries(titleSeries, x, y, markerColor, mType);

            model.Series.Add(scater);
            //add category axis
            var xAxes = new DateTimeAxis
            {
                Title = xLabel,
                Position = AxisPosition.Bottom,
            };
            model.Axes.Add(xAxes);


            //add category axis
            var yAxes = new LinearAxis()
            {
                Title = yLabel,
                Position = AxisPosition.Left,
            };
            model.Axes.Add(yAxes);
            //
            return model;
        }

        public static Task ShowPlots(string wndTitlle, params PlotModel[] models)
        {
            Task task = Task.Run(() =>
            {
                //
                var frm = new Form();
                // 
                // tableLayoutPanel1
                // 
                var tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
                tableLayoutPanel1.SuspendLayout();
                frm.SuspendLayout();

                tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
                tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
                tableLayoutPanel1.Name = "tableLayoutPanel1";


                //column/row
                tableLayoutPanel1.ColumnCount = 1;
                tableLayoutPanel1.ColumnStyles.Add(
                    new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

                tableLayoutPanel1.RowCount = models.Length;
                for (int i = 0; i < models.Length; i++)
                    tableLayoutPanel1.RowStyles.Add(
                        new System.Windows.Forms.RowStyle(SizeType.Percent, (float)(1.0 / (float)models.Length)));


                for (int i = 0; i < models.Length; i++)
                    tableLayoutPanel1.Controls.Add(getplotView(models[i]), 0, i);

                tableLayoutPanel1.Size = new System.Drawing.Size(0, 0);
                tableLayoutPanel1.TabIndex = 0;


                frm.Size = new System.Drawing.Size(950, 600);
                frm.WindowState = FormWindowState.Normal;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Text = wndTitlle;
                tableLayoutPanel1.ResumeLayout(false);
                frm.Controls.Add(tableLayoutPanel1);
                frm.ResumeLayout(false);
                frm.ShowDialog();


            });
            return task;

            OxyPlot.WindowsForms.PlotView getplotView(PlotModel model)
            {
                var plot1 = new OxyPlot.WindowsForms.PlotView();
                // 
                // plot1
                // 
                plot1.Dock = System.Windows.Forms.DockStyle.Fill;
                plot1.Location = new System.Drawing.Point(0, 0);
                plot1.Name = "plot1";
                plot1.PanCursor = System.Windows.Forms.Cursors.Hand;
                //plot1.Size = new System.Drawing.Size(1219, 688);
                plot1.TabIndex = 1;
                plot1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
                plot1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
                plot1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
                plot1.Model = model;
                //plot1.Show();
                return plot1;
            }
        }
    }
}
