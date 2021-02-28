using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.stl;
using Daany.Stat;

using Daany.Stat.Arima;

namespace Unit.Test.DF
{
    public class ARIMATest
    {
		[Fact]
		public void ARIM_Differencing_Test()
        {
			var dict = new Dictionary<string, List<object>>
			{
				{ "a",new List<object>() { 1,2,3,4,5,6} },
				{ "b",new List<object>() { 1,1,2,3,5,8} },
				{ "c",new List<object>() { 1,4,9,16,25,36}},
			};
			//
			var df = new DataFrame(dict);

			//seasonal differencing period 1
			var newDf = df.Diff(step:1);
			Assert.Equal(new List<object>() {DataFrame.NAN,1,1,1,1,1}, newDf["a"]);
			Assert.Equal(new List<object>() { DataFrame.NAN, 0, 1, 1, 2, 3 }, newDf["b"]);
			Assert.Equal(new List<object>() { DataFrame.NAN, 3, 5, 7, 9, 11 }, newDf["c"]);

			//seasonal differencing period 2
			 newDf = df.Diff(step: 2);
			Assert.Equal(new List<object>() { DataFrame.NAN,DataFrame.NAN, 2, 2, 2, 2 }, newDf["a"]);
			Assert.Equal(new List<object>() { DataFrame.NAN,DataFrame.NAN, 1, 2, 3, 5 }, newDf["b"]);
			Assert.Equal(new List<object>() { DataFrame.NAN, DataFrame.NAN, 8, 12, 16, 20 }, newDf["c"]);

			//seasonal differencing period 3
			 newDf = df.Diff(step: 3);
			Assert.Equal(new List<object>() { DataFrame.NAN,DataFrame.NAN, DataFrame.NAN, 3, 3, 3 }, newDf["a"]);
			Assert.Equal(new List<object>() { DataFrame.NAN,DataFrame.NAN, DataFrame.NAN, 2, 4, 6 }, newDf["b"]);
			Assert.Equal(new List<object>() { DataFrame.NAN, DataFrame.NAN, DataFrame.NAN, 15, 21,27 }, newDf["c"]);
		}

		[Fact]
		public void ARIM_Diff_Recursive_Test()
		{
			var dict = new Dictionary<string, List<object>>
			{
				{ "a",new List<object>() { 1,2,3,4,5,6} },
				{ "b",new List<object>() { 1,1,2,3,5,8} },
				{ "c",new List<object>() { 1,4,9,16,25,36}},
			};
			//
			var df = new DataFrame(dict);

			//seasonal differencing period 1
			var newDf = df.Diff(1, type: DiffType.Recurrsive);
			Assert.Equal(new List<object>() { DataFrame.NAN, 1, 1, 1, 1, 1 }, newDf["a"]);
			Assert.Equal(new List<object>() { DataFrame.NAN, 0, 1, 1, 2, 3 }, newDf["b"]);
			Assert.Equal(new List<object>() { DataFrame.NAN, 3, 5, 7, 9, 11 }, newDf["c"]);

			//seasonal differencing period 2
			newDf = df.Diff(step: 2, type: DiffType.Recurrsive);
			Assert.Equal(new List<object>() { DataFrame.NAN, DataFrame.NAN, 0, 0, 0, 0 }, newDf["a"]);
			Assert.Equal(new List<object>() { DataFrame.NAN, DataFrame.NAN, 1, 0, 1, 1 }, newDf["b"]);
			Assert.Equal(new List<object>() { DataFrame.NAN, DataFrame.NAN, 2, 2, 2, 2 }, newDf["c"]);

			//seasonal differencing period 3
			newDf = df.Diff(step: 3, type: DiffType.Recurrsive);
			Assert.Equal(new List<object>() { DataFrame.NAN, DataFrame.NAN, DataFrame.NAN, 0, 0, 0 }, newDf["a"]);
			Assert.Equal(new List<object>() { DataFrame.NAN, DataFrame.NAN, DataFrame.NAN, -1, 1, 0 }, newDf["b"]);
			Assert.Equal(new List<object>() { DataFrame.NAN, DataFrame.NAN, DataFrame.NAN, 0, 0, 0 }, newDf["c"]);
		}

		[Fact]
		public void ARIMA_AR_Test01()
        {

			var df = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\earth_quake.txt",sep:'\t', names:null, parseDate: false);
			var newDf = df.SetIndex("Year");
			var ts = Series.FromDataFrame(newDf, "Quakes");

			Series train = new Series(ts.Take(80).ToList());
			Series test = new Series(ts.Skip(80).ToList());

			var arima = new ARIMA(2,1,2);
			var args = arima.AR(train, 3);
			var marg = arima.MA(train,3);
			//float[] predicted = arima.ARPredict(args, series);

			//MagmaSharp.LinAlg.Lss()

			//ARIMAModel model = new ARIMAModel(train.Select(x=>Convert.ToDouble(x)).ToArray());
			//model.getARIMAModel(12, new List<int[]>(), false);
			//var ss = model.predictValue(0, 3, 0);




		}

		[Fact]
		public void ARIMA_MA_Test01()
		{

			//double[] series = new double[] { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f };
			//MAModel ma = new MAModel(series, 1);
			//var coeffs = ma.Fit();
			//var lst = series.Select(x=> 5.4999988 + 0.9999998 * x).ToList();
			//var lst1 = series.Select(x => coeffs[0] + coeffs[1] * x).ToList();
			////float[] predicted = arima.ARPredict(args, series);

			//MagmaSharp.LinAlg.Lss()





		}


		[Fact]
		public void AutoRegression_Test01()
		{
			var lst = nc.GenerateIntSeries(1, 11, 1);
			//create series from the list
			var ser = new Series(lst);
			var ar = new ARIMA();
			
			var coeff = ar.AR(ser, 2);
            Assert.Equal(new float[3] {1.91f,0.91f,0.09f }, coeff.Select(x=>Convert.ToSingle(Math.Round((double)x,2))));

        }


		[Fact]
        public void ARIMA_Test01()
        {
            var df = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\AirPassengers.csv", 
                sep: ',', names: null, parseDate: false);
            //
            var ts = df.ToSeries("#Passengers");//create time series
			
			var arima = new ARIMA(2,1,2);
			arima.Fit(ts);

        }

    }

}
