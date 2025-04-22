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
		public void Series_ShouldReturnCorrectValues()
		{
			var ser = new Series([1,2,3],[ "a", "b", "c" ], "TestSeries");

			Assert.Equal(2, ser["b"]);
			Assert.Equal(3, ser[2]);
		}

        [Fact]
        public void Series_ShouldCalculateMeanCorrectly()
        {
            var ser = new Series([ 10, 20, 30 ],[ "x", "y", "z" ], "Numbers");
            Assert.Equal(20.0, ser.Mean());
        }

		[Fact]
		public void Series_ShouldReturnCorrectSum()
		{
			var ser = new Series([10, 20, 30], ["x", "y", "z"], "Numbers");
			Assert.Equal(60.0, ser.Sum());
		}

		[Fact]
		public void Series_ShouldReturnCorrectMean()
		{
			var ser = new Series([10, 20, 30], ["x", "y", "z"], "Numbers");
			Assert.Equal(20.0, ser.Mean());
		}

		[Fact]
		public void Series_ShouldReturnCorrectMedian()
		{
			var serOdd = new Series([3, 1, 2], ["a", "b", "c"], "Numbers");
			var serEven = new Series([ 4, 1, 3, 2 ],[ "w", "x", "y", "z" ], "Numbers");

			Assert.Equal(2.0, serOdd.Median());  // Middle value in sorted order: [1,2,3]
			Assert.Equal(2.5, serEven.Median()); // Middle average in sorted order: [1,2,3,4]
		}

		[Fact]
		public void Series_ShouldSupportFiltering()
		{
			var ser = new Series([ 5, 10, 15, 20 ],[ "a", "b", "c", "d" ], "Values");

			var filtered = ser.Filter(x => Convert.ToInt32(x) > 10);

			Assert.Equal(2, filtered.Count);
			Assert.Equal(15, filtered["c"]);
			Assert.Equal(20, filtered["d"]);
		}

		[Fact]
		public void Series_ShouldHandleIndexingCorrectly()
		{
			var ser = new Series([ "Apple", "Banana", "Cherry" ],[ "first", "second", "third" ], "Fruits");

			Assert.Equal("Banana", ser["second"]);
			Assert.Equal("Cherry", ser[2]);
		}

		[Fact]
		public void Series_ShouldThrowException_WhenIndexIsInvalid()
		{
			var ser = new Series([1,2,3],[ "a", "b", "c" ], "Numbers");

			Assert.Throws<ArgumentOutOfRangeException>(() => ser[5]);
			Assert.Throws<KeyNotFoundException>(() => ser["invalid"]);
		}

		[Fact]
		public void Series_ShouldHandleEmptyDataGracefully()
		{
			var ser = new Series([], [], "EmptySeries");

			Assert.Equal(0, ser.Count);
			Assert.True(double.IsNaN(ser.Sum()));
			Assert.True(double.IsNaN(ser.Mean()));
			Assert.True(double.IsNaN(ser.Median()));
		}

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


        [Fact]
        public void To1DArray_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,4,7,10} },
                { "col2",new List<object>() { 2,5,8,11} },
                { "col3",new List<object>() { 3,6,9,12} },

            };
            //
            DataFrame.qsAlgo = true;
            var df = new DataFrame(dict);
            var lst = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            //
            Assert.Equal(lst, df.To1DArray());
            
        }
    }
}
