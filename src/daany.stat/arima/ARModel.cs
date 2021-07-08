using System;
using System.Collections.Generic;
using System.Text;

namespace Daany.Stat.Arima
{
	//this implementation is based on: https://github.com/jsphLim/ARIMA
	internal class ARModel : BaseModel
	{
		public ARModel(double[] data, int p)
		{
			this.data = data;
			this.param = p;
		}

		public override double[] Fit()
		{
			//
			ARMAFoundation ar_math = new ARMAFoundation();
			var maCoe = ar_math.computeARCoe(this.data, this.param);

			return maCoe;
		}
	}
}
