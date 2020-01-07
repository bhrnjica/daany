using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.stl;

namespace Unit.Test.DF
{
    public class ARIMATest
    {

        
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
