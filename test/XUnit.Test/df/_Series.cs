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
            var lst = nc.GenerateIntSeries(0, 100, 1);
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

        [Fact]
        public void Operation_Test01()
        {
            var lst1 = nc.GenerateIntSeries(0, 100, 1);
            var lst2 = nc.GenerateIntSeries(0, 200, 2);
            var lst3 = nc.Zeros(2, 50);

            //create series from the list
            var ser1 = new Series(lst1);
            var ser2 = new Series(lst1);

            //addition
            var ser3 = ser1 + ser2;

            Assert.Equal(ser3.ToList(), lst2.ToList());

            //subtraction
            var ser4 = ser1 - ser2;

            Assert.Equal(ser4.ToList(), lst3.ToList());

            //multiplication
            var ser5 = ser1 * ser2;
            var lst4 = lst1.Zip(lst1, (x1,x2)=> Convert.ToSingle(x1)* Convert.ToSingle(x2)).ToList();
            Assert.Equal(ser5.ToList().Select(x=> Convert.ToSingle(x)), lst4);

        }

        [Fact]
        public void Operation_Test02()
        {
            var lst1 = nc.GenerateIntSeries(0, 100, 1);
            int scalar = 5;
            var lst2 = lst1.Select(x=>(object)(Convert.ToSingle(x) + scalar)).ToList();
            var lst3 = lst1.Select(x => (object)(Convert.ToSingle(x) * scalar)).ToList();

            //create series from the list
            var ser1 = new Series(lst1);
            

            //addition
            var ser3 = ser1 + scalar;

            for(int i=0; i < ser3.Count; i++)
                Assert.Equal(ser3[i], lst2[i]);

            //multiplication
            var ser4 = ser1 * scalar;

            for (int i = 0; i < ser3.Count; i++)
                Assert.Equal(ser4[i], lst3[i]);

        }

        [Fact]
        public void Operation_AppendV_Test01()
        {
            var lst1 = nc.GenerateIntSeries(0, 5, 1);
            var lst2 = nc.GenerateIntSeries(5, 10, 1);
            var lst3 = nc.GenerateIntSeries(0, 10, 1);

            //create series from the list
            var ser1 = new Series(lst1);
            var ser2 = new Series(lst2);

            //addition
            var ser3 = ser1.AppendVerticaly(ser2);

            Assert.Equal(ser3.ToList(), lst3);


        }
        [Fact]
        public void Operation_AppendH_Test01()
        {
            var lst1 = nc.GenerateIntSeries(0, 5, 1);
            var lst2 = nc.GenerateIntSeries(5, 10, 1);
            var lst3 = new List<object> {0,5,1,6,2,7,3,8,4,9 };

            //create series from the list
            var ser1 = new Series(lst1);
            var ser2 = new Series(lst2,name:"Series2");

            //addition
            var df = ser1.AppendHorizontaly(ser2);

            Assert.Equal(df.Values, lst3);


        }
    }

}
