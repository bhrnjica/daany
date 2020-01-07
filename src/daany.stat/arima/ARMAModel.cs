using System;
using System.Collections.Generic;
using System.Text;

namespace Daany.Stat
{
	//this implementation is based on: https://github.com/jsphLim/ARIMA
	public class ARMAModel
	{
		private double[] data;
		private int p;
		private int q;

		public ARMAModel(double[] data, int p, int q)
		{
			this.data = data;
			this.p = p;
			this.q = q;
		}

		public List<double[]> Fit()
		{
			var vec = new List<double[]>();
			ARMAFoundation ar_math = new ARMAFoundation();
			var armaCoe = ar_math.computeARMACoe(this.data, this.p, this.q);

			var arCoe = new double[this.p + 1];
			for (int i = 0; i < arCoe.Length; i++)
			{
				arCoe[i] = armaCoe[i];
			}

			var maCoe = new double[this.q + 1];

			for (int i = 0; i < maCoe.Length; i++)
			{
				maCoe[i] = armaCoe[i + this.p + 1];
			}

			//aggregate coefficients
			vec.Add(arCoe);
			vec.Add(maCoe);

			return vec;
		}
	}

}
