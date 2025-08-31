using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;

namespace Unit.Test.DF
{
    public class DataFrame_Series_Tests
    {
       

        [Fact]
        public void CreateCopy_Test01()
        {
            var lst = nc.GenerateIntSeries(0, 100, 1);
            //create series from the list
            var ser = new Series(lst);
            var ser2 = new Series(ser);

            Assert.Equal(ser.Index.ToList(), ser2.Index.ToList());
            Assert.Equal(ser.Index.Name, ser2.Index.Name);
            Assert.Equal(ser.Name, ser2.Name);
            Assert.Equal(ser.ToList(), ser2.ToList());
        }

        [Fact]
        public void ToRegressors_Test01()
        {
            var lst = nc.GenerateIntSeries(1, 11, 1);
            //create series from the list
            var ser = new Series(lst);
            (var X, var Y) = ser.ToRegressors(2);
            Assert.Equal(new float[8,1]{{3f }, {4f }, { 5f }, { 6f }, { 7f }, { 8f }, { 9f }, { 10f } }, Y);

            var xx = new float[8, 3] {
                        {1f,   1f,   2f} ,
                        {1f,   2f,   3f},
                        {1f,   3f,   4f},
                        {1f,   4f,   5f},
                        {1f,   5f,   6f},
                        {1f,   6f,   7f},
                        {1f,   7f,   8f},
                        {1f,   8f,   9f}};


            Assert.Equal(xx, X);
            
        }

        [Fact]
        public void TSToDataFrame_Test()
        {
            var lst = nc.GenerateIntSeries(0, 10, 1);
            //create series from the list
            var ser = new Series(lst);

            var df1 = ser.TSToDataFrame(3);
            //
            Assert.Equal(7, df1.RowCount());
            Assert.Equal(4, df1.ColCount());

            Assert.Equal(new string[] { "series-L3", "series-L2", "series-L1", "series"}, df1.Columns);

            Assert.Equal(8, df1[5,3]);
            Assert.Equal(5, df1[5,0]);

            Assert.Equal(9, df1[6, 3]);
            Assert.Equal(6, df1[6, 0]);

        }
        [Fact]
        public void Create_Test01()
        {
            var lst = nc.GenerateIntSeries(0, 100, 1);
            //create series from the list
            var ser = new Series(lst);

            Assert.Equal(ser.ToList(), lst.ToList());
            Assert.Equal("series", ser.Name);
        }

        [Fact]
        public void Operation_Test01()
        {
            var lst1 = nc.GenerateIntSeries(0, 100, 1).Select(x=>x).ToList();
            var lst2 = nc.GenerateIntSeries(0, 200, 2).Select(x => x).ToList();
            var lst3 = nc.Zeros(2, 50);

            //create series from the list
            var ser1 = new Series(lst1, type:ColType.I32);
            var ser2 = new Series(lst1, type: ColType.I32);

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
            var ser3 = ser1.AppendVertical(ser2);

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
            var df = ser1.AppendHorizontal(ser2);

            Assert.Equal(df.Values, lst3);


        }

        [Fact]
        public void Operation_Rolling_Test01()
        {
            var lst1 = nc.GenerateIntSeries(0, 5, 1);
            var lst3 = new List<object> { DataFrame.NAN, DataFrame.NAN, 3,6,9 };

            //create series from the list
            var ser1 = new Series(lst1, type: ColType.I32) ;
            
            var ser2 = ser1.Rolling(3, Aggregation.Sum);

            //addition
          
            Assert.Equal(ser2.ToList(), lst3);
        }

		[Fact]
		public void Operation_Rolling_Test_Winth_Null_Values()
		{
			var lst1 = new List<object> { DataFrame.NAN,2.0,3.0, DataFrame.NAN, 5.0,6.0};
			var lst3 = new List<object> { DataFrame.NAN, DataFrame.NAN, 2.5, 2.5, 4.0, 5.5 };

			//create series from the list
			var ser1 = new Series(lst1, type: ColType.I32);


			Assert.Throws<FormatException>(() => ser1.Rolling(3, Aggregation.Avg));

		}

		[Fact]
        public void Operation_MissingValues_Test01()
        {
           
            var lst1 = new List<object> { DataFrame.NAN, DataFrame.NAN, 3, DataFrame.NAN, 9 };
            var lst3 = new List<object> { 3, 9 };

            //create series from the list
            var ser1 = new Series(lst1);
            var ser2 = ser1.DropNA();

            //addition


            Assert.Equal(ser2.ToList(), lst3);


        }

        [Fact]
        public void Operation_MissingValues_Test02()
        {

            var lst1 = new List<object> { DataFrame.NAN, DataFrame.NAN, 3, DataFrame.NAN, 9 };
            var lst3 = new List<object> {1,1, 3,1, 9 };

            //create series from the list
            var ser1 = new Series(lst1);
            var ser2 = ser1.FillNA(1);

            //addition


            Assert.Equal(ser2.ToList(), lst3);


        }


        [Fact]
        public void TimeSerie_Test02()
        {

            var lst1 = new List<object> { DataFrame.NAN, DataFrame.NAN, 3, DataFrame.NAN, 9 };
            var lst3 = new List<object> { 1, 1, 3, 1, 9 };

            //create series from the list
            var ser1 = new Series(lst1);
            var ser2 = ser1.FillNA(1);

            //addition


            Assert.Equal(ser2.ToList(), lst3);


        }

        [Fact]
        public void SumEx_Test01()
        {
            // Test SumEx with integer values
            var intList = new List<object?> { 1, 2, 3, 4, 5 };
            var intSeries = new Series(intList, type: ColType.I32);
            
            var sumResult = intSeries.SumEx();
            var expectedSum = 15.0;
            
            Assert.Equal(expectedSum, sumResult);
        }

        [Fact]
        public void SumEx_Test02()
        {
            // Test SumEx with double values
            var doubleList = new List<object?> { 1.5, 2.5, 3.0, 4.0 };
            var doubleSeries = new Series(doubleList, type: ColType.DD);
            
            var sumResult = doubleSeries.SumEx();
            var expectedSum = 11.0;
            
            Assert.Equal(expectedSum, sumResult);
        }

        [Fact]
        public void SumEx_Test03()
        {
            // Test SumEx with mixed numeric types
            var mixedList = new List<object?> { 1, 2.5f, 3.0, 4L };
            var mixedSeries = new Series(mixedList, type: ColType.DD);
            
            var sumResult = mixedSeries.SumEx();
            var expectedSum = 10.5;
            
            Assert.Equal(expectedSum, sumResult, 5); // 5 decimal places precision
        }

        [Fact]
        public void SumEx_Test04()
        {
            // Test SumEx with empty series
            var emptySeries = new Series(new List<object?>());
            
            var sumResult = emptySeries.SumEx();
            
            Assert.True(double.IsNaN(sumResult));
        }

        [Fact]
        public void MeanEx_Test01()
        {
            // Test MeanEx with integer values
            var intList = new List<object?> { 2, 4, 6, 8 };
            var intSeries = new Series(intList, type: ColType.I32);
            
            var meanResult = intSeries.MeanEx();
            var expectedMean = 5.0;
            
            Assert.Equal(expectedMean, meanResult);
        }

        [Fact]
        public void MeanEx_Test02()
        {
            // Test MeanEx with double values
            var doubleList = new List<object?> { 1.0, 2.0, 3.0, 4.0 };
            var doubleSeries = new Series(doubleList, type: ColType.DD);
            
            var meanResult = doubleSeries.MeanEx();
            var expectedMean = 2.5;
            
            Assert.Equal(expectedMean, meanResult);
        }

        [Fact]
        public void MeanEx_Test03()
        {
            // Test MeanEx with empty series
            var emptySeries = new Series(new List<object?>());
            
            var meanResult = emptySeries.MeanEx();
            
            Assert.True(double.IsNaN(meanResult));
        }

        [Fact]
        public void SumEx_vs_Sum_Performance_Test()
        {
            // Performance comparison test between Sum() and SumEx()
            var largeList = new List<object?>();
            for (int i = 1; i <= 10000; i++)
            {
                largeList.Add((double)i);
            }
            
            var largeSeries = new Series(largeList, type: ColType.DD);
            
            // Test both methods return same result
            var sumResult = largeSeries.Sum();
            var sumExResult = largeSeries.SumEx();
            
            Assert.Equal(sumResult, sumExResult, 10); // 10 decimal places precision
            
            // Expected sum: 1+2+...+10000 = 10000*10001/2 = 50005000
            Assert.Equal(50005000.0, sumExResult);
        }

        [Fact]
        public void MeanEx_vs_Mean_Performance_Test()
        {
            // Performance comparison test between Mean() and MeanEx()
            var largeList = new List<object?>();
            for (int i = 1; i <= 1000; i++)
            {
                largeList.Add((double)i);
            }
            
            var largeSeries = new Series(largeList, type: ColType.DD);
            
            // Test both methods return same result
            var meanResult = largeSeries.Mean();
            var meanExResult = largeSeries.MeanEx();
            
            Assert.Equal(meanResult, meanExResult, 10); // 10 decimal places precision
            
            // Expected mean: (1+2+...+1000)/1000 = 1000*1001/2/1000 = 500.5
            Assert.Equal(500.5, meanExResult);
        }
    }

}
