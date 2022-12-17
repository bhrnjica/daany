using Daany.Plot;
using DataFrame.Test.App;
using ML.Net.App.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Daany.Test.App
{
    static class Program
    {
      
        
        public class TwoKeyLookup<T1, T2, TOut>
        {
            private ILookup<(T1, T2), TOut> lookup;
            public TwoKeyLookup(IEnumerable<TOut> source, Func<TOut, (T1, T2)> keySelector)
            {
                lookup = source.ToLookup(keySelector);
            }

            public IEnumerable<TOut> this[T1 first, T2 second]
            {
                get
                {
                    return lookup[(first, second)];
                }
            }
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {


          //  ARIMATest.ARIMA_Test01();



            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            //OxyPlot
            ExampleOXYPlot.Run();


            //SSA test
            SSADemo.Forecasting();

            //IList<int> index = new int[] {0,1,2,3,4,5,6,7,8,9 };
            //IList<object> key1 = new object[] { new DateTime(2019,1,1), new DateTime(2019, 1, 2), new DateTime(2019, 1, 3), new DateTime(2019, 1, 4)
            //                                    , new DateTime(2019,1,5), new DateTime(2019,1,6), new DateTime(2019,1,7), new DateTime(2019,1,8),
            //                                    new DateTime(2019,1,9), new DateTime(2019,1,10) };
            //IList<object> key2 = new object[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            //IEnumerable<int> data = new int[] { new MyClass() { items = new object[]{"a",3 } },
            //                                            new MyClass() { items = new object[]{"c",5 }  },
            //                                            new MyClass() { items = new object[]{"c",4 }  },
            //                                          }; //TODO populate with real data




            //var lookup = new TwoKeyLookup<object, object, int>(index
            //    , item => (key1[item],key2[item]));

            //var someValue = lookup[key1[3],key2[3]];



            // DataFrameTest.RunMergeTest();

            //SSADemo.Forecasting();

            //ARIMA Test

          // ARIMATest.ARIMA_Test00();
        }
    }
}
