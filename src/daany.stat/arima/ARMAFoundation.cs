using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daany.Stat.Arima
{
	//this implementation is based on: https://github.com/jsphLim/ARIMA
	public class ARMAFoundation
	{
		public static Random rnd = new Random(Seed: 1);
		public double avgData(double[] dataArray)
		{
			return this.sumData(dataArray) / dataArray.Length;
		}

		public double sumData(double[] dataArray)
		{
			double sumData = 0;
			for (int i = 0; i < dataArray.Length; i++)
			{
				sumData += dataArray[i];
			}

			return sumData;
		}

		public double stderrData(double[] dataArray)
		{
			var val = this.varerrData(dataArray);
			return Math.Sqrt(val);
		}

		public double varerrData(double[] dataArray)
		{
			if (dataArray.Length <= 1)
			{
				return 0.0;
			}
			double variance = 0;
			double avgsumData = this.avgData(dataArray);

			for (int i = 0; i < dataArray.Length; i++)
			{
				dataArray[i] -= avgsumData;
				variance += dataArray[i] * dataArray[i];
			}
			return variance / (dataArray.Length - 1);

		}

		public double[] autocorData(double[] dataArray, int order)
		{
			var autoCor = new double[order];
			var autoCov = this.autocovData(dataArray, order);
			double varData = this.varerrData(dataArray);
			
			if (varData != 0)
			{
				for (int i = 0; i < order; i++)
				{
					autoCor[i] = autoCov[i] / varData;
				}
			}
			return autoCor;
		}

		public double[] autocovData(double[] dataArray, int order)
		{
			double[] autoCov = new double[order + 1];

			double mu = this.avgData(dataArray);
			for (int i = 0; i <= order; i++)
			{
				autoCov[i] = 0.0;
				for (int j = 0; j < dataArray.Length - i; j++)
				{
					autoCov[i] += (dataArray[j + i] - mu) * (dataArray[j] - mu);
				}
				autoCov[i] /= (dataArray.Length - i);
			}
			return autoCov;
		}

		public double mutalCorr(double[] dataFir, double[] dataSec)
		{
			double sumX = 0.0;
			double sumY = 0.0;
			double sumXY = 0.0;
			double sumXSq = 0.0;
			double sumYSq = 0.0;
			int len = 0;

			if (dataFir.Length != dataSec.Length)
			{
				len = Math.Min(dataFir.Length, dataSec.Length);
			}
			else
			{
				len = dataFir.Length;
			}

			for (int i = 0; i < len; i++)
			{
				sumX += dataFir[i];
				sumY += dataSec[i];
				sumXY += dataFir[i] * dataSec[i];
				sumXSq += dataFir[i] * dataFir[i];
				sumYSq += dataSec[i] * dataSec[i];
			}

			double numerator = sumXY - sumX * sumY / len;
			double denominator = Math.Sqrt((sumXSq - sumX * sumX / len) * (sumYSq - sumY * sumY / len));

			if (denominator == 0)
			{
				return 0.0;
			}
			return numerator / denominator;

		}

		public double getModelAIC(List<double[]> vec, double[] data, int type)
		{
			int n = data.Length;
			int p = 0;
			int q = 0;
			double tmpAR = 0.0;
			double tmpMA = 0.0;
			double sumErr = 0.0;

			if (type == 1)
			{
				double[] maCoe = vec[0];
				q = maCoe.Length;

				double[] errData = new double[q];

				for (int i = q - 1; i < n; i++)
				{
					tmpMA = 0.0;
					for (int j = 1; j < q; j++)
					{
						tmpMA += maCoe[j] * errData[j];
					}
					for (int j = q - 1; j > 0; j--)
					{
						errData[j] = errData[j - 1];
					}

					errData[0] = ARMAFoundation.gaussrand0() * Math.Sqrt(maCoe[0]);
					sumErr += (data[i] - tmpMA) * (data[i] - tmpMA);
				}
				return (n - (q - 1)) * Math.Log(sumErr / (n - (q - 1))) + (q + 1) * 2;
			}
			else if (type == 2)
			{
				double[] arCoe = vec[0];
				p = arCoe.Length;

				for (int i = p - 1; i < n; ++i)
				{
					tmpAR = 0.0;
					for (int j = 0; j < p - 1; ++j)
					{
						tmpAR += arCoe[j] * data[i - j - 1];
					}
					sumErr += (data[i] - tmpAR) * (data[i] - tmpAR);
				}
				//return Math.log(sumErr) + (p + 1) * 2 / n;
				return (n - (p - 1)) * Math.Log(sumErr / (n - (p - 1))) + (p + 1) * 2;
			}
			else
			{
				double[] arCoe = vec[0];
				double[] maCoe = vec[1];
				p = arCoe.Length;
				q = maCoe.Length;
				var errData = new double[q];

				for (int i = p - 1; i < n; ++i)
				{
					tmpAR = 0.0;
					for (int j = 0; j < p - 1; ++j)
					{
						tmpAR += arCoe[j] * data[i - j - 1];
					}
					tmpMA = 0.0;
					for (int j = 1; j < q; ++j)
					{
						tmpMA += maCoe[j] * errData[j];
					}

					for (int j = q - 1; j > 0; --j)
					{
						errData[j] = errData[j - 1];
					}
					errData[0] = ARMAFoundation.gaussrand0() * Math.Sqrt(maCoe[0]);

					sumErr += (data[i] - tmpAR - tmpMA) * (data[i] - tmpAR - tmpMA);
				}
				//return Math.log(sumErr) + (q + p + 1) * 2 / n;
				return (n - (q + p - 1)) * Math.Log(sumErr / (n - (q + p - 1))) + (p + q) * 2;
			}
		}

		public double[][] LevinsonSolve(double[] garma)
		{
			int order = garma.Length - 1;

			double[][] result = new double[order + 1][];
			for (int i = 0; i < order + 1; i++)
				result[i]= new double[order + 1];

			double[] sigmaSq = new double[order + 1];
			sigmaSq[0] = garma[0];
			result[1][1] = garma[1] / sigmaSq[0];
			sigmaSq[1] = sigmaSq[0] * (1.0 - result[1][1] * result[1][1]);

			for (int k = 1; k < order; ++k)
			{
				double sumTop = 0.0;
				double sumSub = 0.0;
				for (int j = 1; j <= k; ++j)
				{
					sumTop += garma[k + 1 - j] * result[k][j];
					sumSub += garma[j] * result[k][j];
				}

				result[k + 1][k + 1] = (garma[k + 1] - sumTop) / (garma[0] - sumSub);
				
				for (int j = 1; j <= k; ++j)
				{
					result[k + 1][j] = result[k][j] - result[k + 1][k + 1] * result[k][k + 1 - j];
				}
				sigmaSq[k + 1] = sigmaSq[k] * (1.0 - result[k + 1][k + 1] * result[k + 1][k + 1]);
			}
			result[0] = sigmaSq;

			return result;
		}

		public double[] computeARCoe(double[] dataArray, int p)
		{
			var garma = this.autocovData(dataArray, p);

			var vsCoefs = this.LevinsonSolve(garma);

			//List<List<double>> result = new List<List<double>>(vsCoefs);

			double[] ARCoe = new double[p + 1];

			for (int i = 0; i < p; i++)
			{
				ARCoe[i] = vsCoefs[p][i + 1];

			}
			ARCoe[p] = vsCoefs[0][p];
			return ARCoe;

		}

		public double[] computeMACoe(double[] dataArray, int q)
		{

			int p = (int)Math.Log(dataArray.Length);
			//Console.WriteLine($"The best p is: {p}");

			var acovD = this.autocovData(dataArray, p);
			var bestResult = this.LevinsonSolve(acovD);
			
			double[] alpha = new double[p + 1];
			alpha[0] = -1;
			for (int i = 1; i <= p; ++i)
			{
				alpha[i] = bestResult[p][i];
			}

			double[] paraGarma = new double[q + 1];
			for (int k = 0; k <= q; ++k)
			{
				double sum = 0.0;
				for (int j = 0; j <= p - k; ++j)
				{
					sum += alpha[j] * alpha[k + j];
				}
				paraGarma[k] = sum / bestResult[0][p];
			}

			var tmp = this.LevinsonSolve(paraGarma);
			var  MACoe = new double [q + 1];
			for (int i = 1; i < MACoe.Length; ++i)
			{
				MACoe[i] = -tmp[q][i];
			}
			MACoe[0] = 1 / tmp[0][q]; // 

			return MACoe;
		}

		public double[] computeARMACoe(double[] dataArray, int p, int q)
		{
			var allGarma = this.autocovData(dataArray, p + q);
			double[] garma = new double[p + 1];
			for (int i = 0; i < garma.Length; ++i)
			{
				garma[i] = allGarma[q + i];
			}
			var arResult = this.LevinsonSolve(garma);

			// AR
			double[] ARCoe = new double[p + 1];
			for (int i = 0; i < p; ++i)
			{
				ARCoe[i] = arResult[p][i + 1];
			}
			ARCoe[p] = arResult[0][p];
			//		double [] ARCoe = this.YWSolve(garma);

			// MA
			double[] alpha = new double[p + 1];
			alpha[0] = -1;
			for (int i = 1; i <= p; ++i)
			{
				alpha[i] = ARCoe[i - 1];
			}

			double[] paraGarma = new double[q + 1];
			for (int k = 0; k <= q; ++k)
			{
				double sum = 0.0;
				for (int i = 0; i <= p; ++i)
				{
					for (int j = 0; j <= p; ++j)
					{
						sum += alpha[i] * alpha[j] * allGarma[Math.Abs(k + i - j)];
					}
				}
				paraGarma[k] = sum;
			}
			var maResult = this.LevinsonSolve(paraGarma);
			var MACoe = new double[q + 1];
			for (int i = 1; i <= q; ++i)
			{
				MACoe[i] = maResult[q][i];
			}
			MACoe[0] = maResult[0][q];

			//		double [] tmp = this.YWSolve(paraGarma);
			//		double [] MACoe = new double[q + 1];
			//		System.arraycopy(tmp, 0, MACoe, 1, tmp.length - 1);
			//		MACoe[0] = tmp[tmp.length - 1];

			var ARMACoe = new double[p + q + 2];
			for (int i = 0; i < ARMACoe.Length; ++i)
			{
				if (i < ARCoe.Length)
				{
					ARMACoe[i] = ARCoe[i];
				}
				else
				{
					ARMACoe[i] = MACoe[i - ARCoe.Length];
				}
			}
			return ARMACoe;
		}

		public static double gaussrand0()
		{
			double V1=0;
			double V2=0;
			double S=0;
			int phase = 0;
			double X;

			if (phase == 0)
			{
				do
				{
					double U1 = rnd.NextDouble();//(double)RandomNumbers.NextNumber() / RAND_MAX;
					double U2 = rnd.NextDouble(); //(double)RandomNumbers.NextNumber() / RAND_MAX;

					V1 = 2 * U1 - 1;
					V2 = 2 * U2 - 1;
					S = V1 * V1 + V2 * V2;
				} while (S >= 1 || S == 0);

				X = V1 * Math.Sqrt(-2 * Math.Log(S) / S);
			}
			else
			{
				X = V2 * Math.Sqrt(-2 * Math.Log(S) / S);
			}

			phase = 1 - phase;

			return X;
		}
	}
}
