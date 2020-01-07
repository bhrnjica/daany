using System;
using System.Collections.Generic;
using System.Text;

namespace Daany.Stat
{
	//this implementation is based on: https://github.com/jsphLim/ARIMA
	public class MAModel : BaseModel
	{
		

		public MAModel(double[] data, int q)
		{
			this.data = data;
			this.param = q;
		}

		public override double[] Fit()
		{
			//
			ARMAFoundation ar_math = new ARMAFoundation();
			var maCoe = ar_math.computeMACoe(this.data, this.param);
			
			return maCoe;
		}
	}
}
