using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Ext;
using Microsoft.ML;

namespace Unit.Test.DF
{
    public class DisplayDataFrame_Tests
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

            var expected = @"            product_id  retail_pricequantity    city        state       
0           1           2           1           SF          CA          
1           1           2           2           SJ          CA          
2           2           5           4           SF          CA          
3           2           5           8           SJ          CA          
4           2           5           16          Miami       FL          
5           2           5           32          Orlando     FL          
6           2           5           64          SJ          PR          
";

            var df = new DataFrame(dict);   
            var actual = df.Head(10);



            Assert.Equal(7, actual.RowCount());
            Assert.Equal(expected, actual.ToStringBuilder());

            var actual1 = df.Head(7);
            Assert.Equal(7, actual1.RowCount());
            Assert.Equal(expected, actual1.ToStringBuilder());

            var actual2 = df.Tail(10);

            Assert.Equal(7, actual2.RowCount());
            Assert.Equal(expected, actual1.ToStringBuilder());

            var expected1 = @"            product_id  retail_pricequantity    city        state       
2           2           5           4           SF          CA          
3           2           5           8           SJ          CA          
4           2           5           16          Miami       FL          
5           2           5           32          Orlando     FL          
6           2           5           64          SJ          PR          
";

            var actual3 = df.Tail(5);
            var ss = actual3.ToStringBuilder();
            Assert.Equal(5, actual3.RowCount());
            Assert.Equal(expected1, actual3.ToStringBuilder());

        }

    }

}
