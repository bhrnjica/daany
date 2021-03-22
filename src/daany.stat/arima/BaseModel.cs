using System;
using System.Collections.Generic;
using System.Text;

namespace Daany.Stat
{
    ///this implementation is based on: https://github.com/jsphLim/ARIMA
    internal class BaseModel
    {
        protected double[] data;
        protected int param;

        public virtual double[] Fit()
        {
            return null;
        }
    }
}
