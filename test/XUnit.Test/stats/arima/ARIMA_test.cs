using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.stl;
using Daany.Stat;
using static Daany.LinA.LinA;
using Daany.Arima;

namespace Unit.Test.DF
{
    public class ARIMATest
    {
		[Fact(Skip = "test is not completed")]
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
		[Fact(Skip = "test is not completed")]
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
		[Fact(Skip = "test is not completed")]
		public void ARIMA_AR_Test01()
        {

			var df = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\earth_quake.txt",sep:'\t', names:null, parseDate: false);
			var newDf = df.SetIndex("Year");
			var ts = Series.FromDataFrame(newDf, "Quakes");
			//
			DataFrame tsdf = ts.TSToDataFrame(lags:3);

			var arima = new ARIMA();
			var args = arima.AR(ts, 3);

			//MagmaSharp.LinAlg.Lss()





		}

		[Fact(Skip = "test is not completed")]
		public void AutoRegression_Test01()
		{
			var lst = nc.GenerateIntSeries(1, 11, 1);
			//create series from the list
			var ser = new Series(lst);
			var ar = new ARIMA();
			
			var coeff = ar.AR(ser, 2);
            Assert.Equal(new float[3] {1.91f,0.91f,0.09f }, coeff.Select(x=>Convert.ToSingle(Math.Round((double)x,2))));

        }

		//TOdo:
		[Fact]
        public void ARIMA_Test00()
        {
			Console.WriteLine("ARIMA Math");
			//int q = 1;
			ARMAFoundation am = new ARMAFoundation();

			var dataArray = new double[] { 136, 144, 167, 162, 160, 153, 147, 146, 148, 150, 
											156, 168, 172, 150, 28, 12, 17, 1, 2, 13, 150, 149, 
											149, 154, 165, 165, 152, 156, 151, 151, 155, 165, 
						165, 149, 148, 146, 148, 151, 162, 176, 180, 151, 150, 155, 155, 168 };
			
			ARIMAModel arima = new ARIMAModel(dataArray);
			int period = 7;
			int modelCnt = 5;
			int cnt = 0;
			var list = new List<int[]>();
			int[] tmpPredict = new int[modelCnt];

			for (int k = 0; k < modelCnt; ++k) //Control how many sets of parameters are used to calculate the final result
			{
				var bestModel = arima.getARIMAModel(period, list, (k == 0) ? false : true);

				Console.Write(bestModel.Length);
				Console.Write("\n");

				if (bestModel.Length == 0)
				{
					tmpPredict[k] = (int)dataArray[dataArray.Length - period];
					cnt++;
					break;
				}
				else
				{
					Console.Write(bestModel[0]);
					Console.Write(bestModel[1]);
					Console.Write("\n");
					int predictDiff = arima.predictValue(bestModel[0], bestModel[1], period);
					Console.Write("--");
					Console.Write("\n");
					tmpPredict[k] = arima.aftDeal(predictDiff, period);
					cnt++;
				}
				Console.Write(bestModel[0]);
				Console.Write(" ");
				Console.Write(bestModel[1]);
				Console.Write("\n");
				list.Add(bestModel);
			}

			double sumPredict = 0.0;
			for (int k = 0; k < cnt; ++k)
			{
				sumPredict += ((double)tmpPredict[k]) / (double)cnt;
			}
			int predict = (int)Math.Round(sumPredict);
			Console.Write("Predict value=");
			Console.Write(predict);
			Console.Write("\n");
		}

		//TOdo:
		[Fact]
		public void ARIMA_Test02()
		{
			Console.WriteLine("ARIMA Math");
			int q = 1;
			ARMAFoundation am = new ARMAFoundation();

			var dataArray = new double[] { 136, 144, 167, 162, 160, 153, 147, 146, 148, 150,
											156, 168, 172, 150, 28, 12, 17, 1, 2, 13, 150, 149,
											149, 154, 165, 165, 152, 156, 151, 151, 155, 165,
						165, 149, 148, 146, 148, 151, 162, 176, 180, 151, 150, 155, 155, 168 };

            var coefs = am.computeMACoe(dataArray, q);
            foreach (var c in coefs)
                Console.WriteLine($"coeff: {c}");

            int p = 2;
            var coefs1 = am.computeARCoe(dataArray, p);
            foreach (var c in coefs1)
                Console.WriteLine($"coeff: {c}");

		}

		//TOdo:
		[Fact]
        public void ARIMA_Test01()
        {
            var df = DataFrame.FromCsv(filePath: $"testdata\\AirPassengers.csv", 
                sep: ',', names: null, parseDate: false);
            //
            var ts = df["#Passengers"].Select(f => Convert.ToDouble(f));//create time series



        }

    }

}
