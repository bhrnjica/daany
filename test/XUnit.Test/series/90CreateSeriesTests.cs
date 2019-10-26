using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class CreateSeriesTests
    {
        [Fact]
        public void CreateSeries_Test01()
        {
            var dFrom = new DateTime(2019, 01, 01,05,0,0);
            var dTo = new DateTime(2019, 01, 02, 05, 0, 0);
            var step = TimeSpan.FromHours(3);
            var serie = nc.GenerateDateSeries(dFrom, dTo, step);
            //
            Assert.Equal(new DateTime(2019, 01, 01, 05, 0, 0), serie[0]);
            Assert.Equal(new DateTime(2019, 01, 01, 08, 0, 0), serie[1]);
            Assert.Equal(new DateTime(2019, 01, 01, 11, 0, 0), serie[2]);
            Assert.Equal(new DateTime(2019, 01, 01, 14, 0, 0), serie[3]);
            Assert.Equal(new DateTime(2019, 01, 01, 17, 0, 0), serie[4]);
            Assert.Equal(new DateTime(2019, 01, 01, 20, 0, 0), serie[5]);
            Assert.Equal(new DateTime(2019, 01, 01, 23, 0, 0), serie[6]);
            Assert.Equal(new DateTime(2019, 01, 02, 02, 0, 0), serie[7]);
        }

        [Fact]
        public void CreateDoubleSeries_Test02()
        {
            var dFrom = 5.5;
            var dTo = 13.5;
            var count = 8;
            var serie = nc.GenerateDoubleSeries(dFrom, dTo, count);
            //
            Assert.Equal(5.5, serie[0]);
            Assert.Equal(6.5, serie[1]);
            Assert.Equal(7.5, serie[2]);
            Assert.Equal(8.5, serie[3]);
            Assert.Equal(9.5, serie[4]);
            Assert.Equal(10.5, serie[5]);
            Assert.Equal(11.5, serie[6]);
            Assert.Equal(12.5, serie[7]);
        }

        
    }
}
