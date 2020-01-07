using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.stl;
using Daany.Stat;

namespace Unit.Test.DF
{
    public class ARIMATest
    {
		//TOdo:
        [Fact]
        public void ARIMA_Test00()
        {
			Console.WriteLine("ARIMA Math");
			int q = 1;
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
            var df = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\AirPassengers.csv", 
                sep: ',', names: null, parseDate: false);
            //
            var ts = df["#Passengers"].Select(f => Convert.ToDouble(f));//create time series



        }

    }

}
