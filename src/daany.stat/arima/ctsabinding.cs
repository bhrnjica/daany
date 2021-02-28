using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Daany.Stat.Arima
{
    unsafe internal class ctsabinding
    {
        #region Device Management
        [DllImport("ctsa_binding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int daany_arima(int p, int d, int q, double* ts, int tsLength, int arimaMethod, int optMethod, double* regressors);
        #endregion

        public static double[] arima(int p, int d, int q, ARIMAMethod aMethod, OptMethod optMethod,  double[] ts)
        {
            try
            {
                var reg = new double[p + q+1];
               // fixed (double* regressors = reg, tss = ts)
                {
                    int am = (int)aMethod;
                    int om = (int)optMethod;
                    var ret = Task.Factory.StartNew(() =>
                    {
                        fixed (double* regressors = reg, tss = ts)
                        {
                            return daany_arima(p, d, q, tss, ts.Length, am, om, regressors);

                        }

                    });
                    //var retVal = daany_arima(p, d, q, tss, ts.Length, am, om, regressors);
                    ret.Wait();

                return reg;

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
