using Daany;
using System;
using System.Threading.Tasks;
using XPlot.Plotly;
using Xunit;

namespace Unit.Test.DF
{
	public class PLotlyTests
    {
		[Fact(Skip = "Use this tes only if you want to play with plotting capabilities.")]
		public async Task testPLot()
        {
            var url = "https://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data";

            var cols = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "flower_type" };

            var df = await DataFrame.FromWebAsync(url, sep: ',', names: cols);

            //calculate two new columns into dataset
            df.AddCalculatedColumns(new string[] { "SepalArea", "PetalArea" },
                    (r, i) =>
                    {
                        var aRow = new object[2];
                        aRow[0] = Convert.ToSingle(r["sepal_width"]) * Convert.ToSingle(r["sepal_length"]);
                        aRow[1] = Convert.ToSingle(r["petal_width"]) * Convert.ToSingle(r["petal_length"]);
                        return aRow;

                    });
            var featuredDf = df["SepalArea", "PetalArea", "flower_type"];

            var chart = Chart.Plot(
                new Scatter[] {
                    new Scatter
                    {
                        x = df.Filter("flower_type","Iris-virginica", FilterOperator.Equal)["SepalArea"],
                        y = df.Filter("flower_type","Iris-virginica", FilterOperator.Equal)["PetalArea"],
                        mode = "markers",name="Iris-virginica",
                        marker = new Marker(){color=2, colorscale = "Jet"}
                    },
                    new Scatter
                    {
                        x = df.Filter("flower_type","Iris-versicolor", FilterOperator.Equal)["SepalArea"],
                        y = df.Filter("flower_type","Iris-versicolor", FilterOperator.Equal)["PetalArea"],
                        mode = "markers",name="Iris-versicolor",
                        marker = new Marker(){color=2, colorscale = "Jet"}
                    },
                    new Scatter
                    {
                        x = df.Filter("flower_type","Iris-setosa", FilterOperator.Equal)["SepalArea"],
                        y = df.Filter("flower_type","Iris-setosa", FilterOperator.Equal)["PetalArea"],
                        mode = "markers",name="Iris-setosa",
                        marker = new Marker(){ color=3, colorscale = "Jet"}
                    },
                }
            );

            var layout = new Layout.Layout() { title = "Plot Sepal vs. Petal Area & color scale on flower type" };
            chart.WithLayout(layout);
            chart.WithLegend(true);
            chart.WithLabels(new string[3] { "Iris-virginica", "Iris-versicolor", "Iris-setosa" });
            chart.WithXTitle("Sepal Area");
            chart.WithYTitle("Petal Area");
            chart.Width = 800;
            chart.Height = 400;
            chart.Show();
        }

    }
}
