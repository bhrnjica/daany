using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class DataFrame_Series_Tests
    {
      

        [Fact]
        public void Create_Test01()
        {
            var lst = nc.GenerateIntNSeries(0, 1, 100);
            //create series from the list
            var ser = new Series(lst);

            //Assert.Equal(result.Index, result.Index);


            //for (int i = 0; i < result.Values.Count; i++)
            //{
            //    var expected = Convert.ToInt32(df1.Values[i]);
            //    var actual = Convert.ToInt32(result.Values[i]);
            //    Assert.Equal<int>(expected, actual);
            //}
        }
        
    }

}
