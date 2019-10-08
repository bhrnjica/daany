using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Ext;
using Microsoft.ML;

namespace Unit.Test.DF
{
    public class DF_Display_Tests
    {
        

        [Fact]
        public void Head_Test01()
        {
            var mlContext = new MLContext();
            var dict = new Dictionary<string, List<object>>
            {
                {"product_id",new List<object>() {1,1,2,2,2,2,2 } },
                { "retail_price",new List<object>() { 2,2,5,5,5,5,5 } },
                { "quantity",new List<object>() { 1,2,4,8,16,32,64 } },
                { "city",new List<object>() { "SF","SJ","SF","SJ","Miami", "Orlando","SJ"} },
                { "state" ,new List<object>() { "CA","CA","CA","CA","FL","FL","PR" } },
            };

            var expected = @"product_id  retail_pricequantity    city        state       
1           2           1           SF          CA          
1           2           2           SJ          CA          
2           5           4           SF          CA          
2           5           8           SJ          CA          
2           5           16          Miami       FL          
2           5           32          Orlando     FL          
2           5           64          SJ          PR          
";
            //
            var df = new DataFrame(dict);
            var actual = df.Head(10);

            Assert.Equal(expected, actual);

            var actual1 = df.Head(7);
            Assert.Equal(expected, actual1);

            var actual2 = df.Tail(10);
            Assert.Equal(expected, actual1);

            var expected1 = @"product_id  retail_pricequantity    city        state       
2           5           4           SF          CA          
2           5           8           SJ          CA          
2           5           16          Miami       FL          
2           5           32          Orlando     FL          
2           5           64          SJ          PR          
";

            var actual3 = df.Tail(5);
            Assert.Equal(expected1, actual3);

        }

    }

}
