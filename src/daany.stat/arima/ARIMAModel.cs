using System;
using System.Collections.Generic;
using System.Text;

namespace Daany.Stat
{
	//this implementation is based on: https://github.com/jsphLim/ARIMA
	public class ARIMAModel
	{
		double[] dataArray;
		double[] dataFirDiff;

		List<double[]> arima;
		public ARIMAModel(double[] dataArray)
		{
			this.dataArray= dataArray;
		}

		public double[] preFirDiff(double[] preData)
		{
			List<double> res = new List<double>();
			for (int i = 0; i < preData.Length - 1; i++)
			{
				double tmpData = preData[i + 1] - preData[i];
				res.Add(tmpData);
			}
			return res.ToArray();
		}

		public double[] preSeasonDiff(double[] preData)
		{
			List<double> res = new List<double>();

			for (int i = 0; i < preData.Length - 7; i++)
			{

				double tmpData = preData[i + 7] - preData[i];
				res.Add(tmpData);
			}
			return res.ToArray();
		}
		public double[] preDealDiff(int period)
		{
			if (period >= dataArray.Length - 1)
				period = 0;

			switch (period)
			{
				case 0:
					{
						return this.dataArray;
					}
				case 1:
					{
						this.dataFirDiff = this.preFirDiff(this.dataArray);
						return this.dataFirDiff;
					}
				default:
					{
						var preSeasData =  preSeasonDiff(dataArray);
						return preSeasData;
					}
			}
		}

		public int[] getARIMAModel(int period, List<int[]> notModel, bool needNot)
		{
			var data = this.preDealDiff(period);
			// 
			double minAIC = 1.7976931348623157E308;
			var bestModel = new int[3];
			int type = 0;
			List<double[]> coe= new List<double[]>();

			// The model is generated, that is, the corresponding p, q parameters are generated
			int len = data.Length >5 ? 5 : data.Length;

			int size = ((len + 2) * (len + 1)) / 2 - 1;
			List<int[]> model = new List<int[]>();
			//
			for (int i = 0; i < size; i++)
				model.Add(new int[size]);
			
			int cnt = 0;
			for (int i = 0; i <= len; ++i)
			{
				for (int j = 0; j <= len - i; ++j)
				{
					if (i == 0 && j == 0)
						continue;

					model[cnt][0] = i;
					model[cnt++][1] = j;
				}
			}
			// 
			for (int i = 0; i < cnt; ++i)
			{
				// Control selected parameters 
				bool token = false;
				if (needNot)
				{
					for (int k = 0; k < notModel.Count; ++k)
					{
						if (model[i][0] == notModel[k][0] && model[i][1] == notModel[k][1])
						{
							token = true;
							break;
						}
					}
				}
				if (token)
				{
					continue;
				}

				if (model[i][0] == 0)
				{
					MAModel ma = new MAModel(data, model[i][1]);
					//std::vector<std::vector<double>>
					var maC= ma.Fit();
					coe.Add(maC);
					type = 1;
				}
				else if (model[i][1] == 0)
				{
					ARModel ar = new ARModel(data, model[i][0]);
					// 
					var maC = ar.Fit();
					coe.Add(maC);
					type = 2;
				}
				else
				{
					//
					ARMAModel arma = new ARMAModel(data, model[i][0], model[i][1]);
					//
					coe = arma.Fit();
					type = 3;
				}

				ARMAFoundation ar_math = new ARMAFoundation();
				double aic = ar_math.getModelAIC(coe, data, type);
				// If the order is too long during the solution process, NAN or infinity may occur
				if (aic <= 1.7976931348623157E308 && !double.IsNaN(aic) && aic < minAIC)
				{
					minAIC = aic;
					// std::cout<<aic<<std::endl;
					bestModel[0] = model[i][0];
					bestModel[1] = model[i][1];
					bestModel[2] = (int)Math.Round(minAIC);
					this.arima = coe;  
				}
			}
			return bestModel;
		}

		public int aftDeal(int predictValue, int period)
		{
			if (period >= dataArray.Length)
			{
				period = 0;
			}

			switch (period)
			{
				case 0:
					return (int)predictValue;
				case 1:
					return (int)(predictValue + dataArray[dataArray.Length - 1]);
				case 2:
				default:
					return (int)(predictValue + dataArray[dataArray.Length - 7]);
			}
		}

		

		public int predictValue(int p, int q, int period)
		{
			var data = this.preDealDiff(period);
			int n = data.Length;
			int predict = 0;
			double tmpAR = 0.0;
			double tmpMA = 0.0;
			double[] errData = new double[q + 1];

			if (p == 0)
			{
				List<double> maCoe = new List<double>(this.arima[0]);
				for (int k = q; k < n; ++k)
				{
					tmpMA = 0;
					for (int i = 1; i <= q; ++i)
					{
						tmpMA += maCoe[i] * errData[i];
					}
					// 
					for (int j = q; j > 0; --j)
					{
						errData[j] = errData[j - 1];
					}
					errData[0] = ARMAFoundation.gaussrand0() * Math.Sqrt(maCoe[0]);
				}

				predict = (int)(tmpMA); // 
			}
			else if (q == 0)
			{
				List<double> arCoe = new List<double>(this.arima[0]);

				for (int k = p; k < n; ++k)
				{
					tmpAR = 0;
					for (int i = 0; i < p; ++i)
					{
						tmpAR += arCoe[i] * data[k - i - 1];
					}
				}
				predict = (int)(tmpAR);
			}
			else
			{
				List<double> arCoe = new List<double>(this.arima[0]);
				List<double> maCoe = new List<double>(this.arima[1]);

				for (int k = p; k < n; ++k)
				{
					tmpAR = 0;
					tmpMA = 0;
					for (int i = 0; i < p; ++i)
					{
						tmpAR += arCoe[i] * data[k - i - 1];
					}
					for (int i = 1; i <= q; ++i)
					{
						tmpMA += maCoe[i] * errData[i];
					}

					// 
					for (int j = q; j > 0; --j)
					{
						errData[j] = errData[j - 1];
					}

					errData[0] = ARMAFoundation.gaussrand0() * Math.Sqrt(maCoe[0]);
				}

				predict = (int)(tmpAR + tmpMA);
			}

			return predict;
		}
	}

}
