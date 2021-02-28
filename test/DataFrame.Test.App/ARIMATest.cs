using Daany.Stat;
using System;
using System.Collections.Generic;
using System.Text;
using Daany;
using System.Linq;
using Daany.Stat.Arima;

namespace DataFrame.Test.App
{
    public class ARIMATest
    {

		

		

		public static void ARIMA_Test01()
		{
			int processId = 0; // TODO: populate this variable
			var proc = System.Diagnostics.Process.GetCurrentProcess();
			proc.EnableRaisingEvents = true;
			proc.Exited += CurrentDomain_ProcessExit;

			var df = Daany.DataFrame.FromCsv(filePath: $"AirPassengers.csv",
				sep: ',', names: null, parseDate: false);
			////
			var ts = df.ToSeries("#Passengers");//create time series

			int p = 2;
			int d = 1;
			int q = 2;
			var model = new ARIMA(p,d,q);
			model.Fit(ts);


		}

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
