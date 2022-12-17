using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
namespace Daany.Plot
{
    public class ExampleOXYPlot
    {
        public static void Run()
        {
            //var outputToFile = "test-oxyplot-static-export-file";
            var outputExportStreamOOP = "test-oxyplot-export-stream";

            //var width = 1024;
            //var height = 768;
            var resolutions = new[] { 72d, 96d, 182d };

            var model = HeatMapExample();//BuildPlotModel();

            foreach (var resolution in resolutions)
            {
                // export using the instance methods
                using (var stream = new MemoryStream())
                {
                    var strPath = $"{outputExportStreamOOP}{resolution}.svg";
                    var strFull = Path.Combine(Directory.GetCurrentDirectory(), strPath);
                    var jpegExporter = new SvgExporter();
                    jpegExporter.Export(model, stream);
                    System.IO.File.WriteAllBytes(strFull, stream.ToArray());
                    Process.Start(@"cmd.exe ", @"/c " + strFull);
                }
            }
        }

        private static PlotModel BuildPlotModel()
        {
            var rand = new Random(21);

            var model = new PlotModel { Title = "Cake Type Popularity" };

            var cakePopularity = Enumerable.Range(1, 5).Select(i => rand.NextDouble()).ToArray();
            var sum = cakePopularity.Sum();
            var barItems = cakePopularity.Select(cp => RandomBarItem(cp, sum)).ToArray();
            var barSeries = new BarSeries
            {
                ItemsSource = barItems,
                LabelPlacement = LabelPlacement.Base,
                LabelFormatString = "{0:.00}%"
            };

            model.Series.Add(barSeries);

            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                Key = "CakeAxis",
                ItemsSource = new[]
                {
                "Apple cake",
                "Baumkuchen",
                "Bundt Cake",
                "Chocolate cake",
                "Carrot cake"
            }
            });

            return model;
        }

        private static BarItem RandomBarItem(double cp, double sum)
            => new BarItem { Value = cp / sum * 100, Color = RandomColor() };

        private static OxyColor RandomColor()
        {
            var r = new Random();
            return OxyColor.FromRgb((byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255));
        }
    
       
        
        public static PlotModel HeatMapExample()
        {
            var model = new PlotModel { Title = "Cakes per Weekday" };

            // Weekday axis (horizontal)
            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,

                // Key used for specifying this axis in the HeatMapSeries
                Key = "WeekdayAxis",

                // Array of Categories (see above), mapped to one of the coordinates of the 2D-data array
                ItemsSource = new[]
                {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"
    }
            });

            // Cake type axis (vertical)
            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                Key = "CakeAxis",
                ItemsSource = new[]
                {
            "Apple cake",
            "Baumkuchen",
            "Bundt cake",
            "Chocolate cake",
            "Carrot cake"
    }
            });

            // Color axis
            model.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Hot(200)
            });

            var rand = new Random();
            var data = new double[7, 5];
            for (int x = 0; x < 5; ++x)
            {
                for (int y = 0; y < 7; ++y)
                {
                    data[y, x] = rand.Next(0, 200) * (0.13 * (y + 1));
                }
            }

            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = 6,
                Y0 = 0,
                Y1 = 4,
                XAxisKey = "WeekdayAxis",
                YAxisKey = "CakeAxis",
                RenderMethod = HeatMapRenderMethod.Rectangles,
                LabelFontSize = 0.2, // neccessary to display the label
                Data = data
            };

            model.Series.Add(heatMapSeries);

            return model;
        }
    
    }

}
