using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class MissingValuesTests
    {
		[Fact]
		public void MissingValues_ShouldReturnCountsForColumnsWithMissingValues()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, DataFrame.NAN, 3, 4 },
				new List<string> { "col1" });

			// Act
			var result = df.MissingValues();

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			Assert.True(result.ContainsKey("col1"));
			Assert.Equal(1, result["col1"]);
		}

		[Fact]
		public void MissingValues_ShouldReturnEmptyForDataFrameWithoutMissingValues()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, 2, 3, 4 },
				new List<string> { "col1" });

			// Act
			var result = df.MissingValues();

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public void Drop_ShouldRemoveSpecifiedColumns()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, 2, 3, 4 },
				new List<string> { "col1", "col2" });

			// Act
			var result = df.Drop("col1");

			// Assert
			Assert.NotNull(result);
			Assert.Single(result.Columns);
			Assert.DoesNotContain("col1", result.Columns);
		}

		[Fact]
		public void Drop_ShouldThrowExceptionForNonExistentColumn()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, 2, 3, 4 },
				new List<string> { "col1" });

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => df.Drop("col2"));

		}

		[Fact]
		public void DropNA_ShouldRemoveRowsWithMissingValues()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, DataFrame.NAN, 3, 4 },
				new List<string> { "col1" });

			// Act
			var result = df.DropNA();

			// Assert
			Assert.NotNull(result);
			Assert.Equal(3, result.Index.Count); // One row removed
		}

		[Fact]
		public void DropNA_ShouldRemoveRowsWithMissingValuesInSpecificColumns()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, DataFrame.NAN, 3, 4, DataFrame.NAN,6 },
				new List<string> { "col1", "col2" });

			// Act
			var result = df.DropNA("col1");

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Index.Count); // One row removed
		}

		[Fact]
		public void FillNA_ShouldReplaceAllMissingValuesWithSpecifiedValue()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { DataFrame.NAN, 2, DataFrame.NAN, 4 },
				new List<string> { "col1" });

			// Act
			df.FillNA(0);

			// Assert
			Assert.Equal(new List<object> { 0, 2, 0, 4 }, df["col1"]);
		}


		[Fact]
        public void DetectMissingValue_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() {  1, 11, "", 31, 41, 51, "", 71, 81, "?"} },
                { "col2",new List<object>() {  2, 12, 22,"?", 42, 52, 62, 72, 82, 92 } },
                { "col3",new List<object>() {  3, "", 23, 33, 43, 53, 63, 73, 83, 93 } },
                { "col4",new List<object>() {  4, 14, 24, 34, 44, 54, 64, 74, 84, 94} },
                { "col5",new List<object>() {"?", 15, 25, 35, 45, "", 65, "", 85, 95 } },

            };
            //
            var df = new DataFrame(dict);

            //
            var c1 = new object[] { 1, 11, DataFrame.NAN, 31, 41, 51, DataFrame.NAN, 71, 81, DataFrame.NAN };
            var c2 = new object[] { 3, DataFrame.NAN, 23, 33, 43, 53, 63, 73, 83, 93 };
            var c3 = new object[] { DataFrame.NAN, 15, 25, 35, 45, DataFrame.NAN, 65, DataFrame.NAN, 85, 95 };


            for (int i = 0; i < c1.Length; i++)
                Assert.Equal(c1[i], df["col1",i]);

            for (int i = 0; i < c2.Length; i++)
                Assert.Equal(c2[i], df["col3", i]);

            for (int i = 0; i < c3.Length; i++)
                Assert.Equal(c3[i], df["col5", i]);
        }

		[Fact]
		public void FillNA_ShouldReplaceMissingValuesInSpecificColumn()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { DataFrame.NAN, 2, DataFrame.NAN, 4 },
				new List<object> { "row1", "row2" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.I32 });

			// Act
			df.FillNAByValue("col1", 0);

			// Assert
			Assert.Equal(new List<object> { 0, 0 }, df["col1"]);
			Assert.Equal(new List<object> { 2, 4 }, df["col2"]);
		}

		[Fact]
		public void FillNA_ShouldThrowExceptionForNonExistentColumn()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { DataFrame.NAN, 2, DataFrame.NAN, 4 },
				new List<object> { "row1", "row2", "row3", "row4" },
				new List<string> { "col1" },
				new ColType[] { ColType.I32 });

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => df.FillNAByValue("col2", 0));
			
		}

		[Fact]
		public void FillNAAggregate_ShouldThrowExceptionForNonExistentColumn()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { DataFrame.NAN, 2, DataFrame.NAN, 4 },
				new List<object> { "row1", "row2", "row3", "row4" },
				new List<string> { "col1" },
				new ColType[] { ColType.I32 });

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => df.FillNA("col2", Aggregation.Min));

		}

		[Fact]
		public void FillNA_ShouldReplaceMissingValuesWithAggregation()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { DataFrame.NAN, 2, 4, 6 },
				new List<object> { "row1", "row2", "row3", "row4" },
				new List<string> { "col1" },
				new ColType[] { ColType.I32 });

			// Act
			df.FillNA("col1", Aggregation.Avg);

			// Assert
			Assert.Equal(new List<object> { 4, 2, 4, 6 }, df["col1"]); // Avg of 2, 4, 6 = 4
		}



		[Fact]
        public void RemoveMissingValue_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() {  1, 11, "", 31, 41, 51, "", 71, 81, "?"} },
                { "col2",new List<object>() {  2, 12, 22,"?", 42, 52, 62, 72, 82, 92 } },
                { "col3",new List<object>() {  3, "", 23, 33, 43, 53, 63, 73, 83, 93 } },
                { "col4",new List<object>() {  4, 14, 24, 34, 44, 54, 64, 74, 84, 94} },
                { "col5",new List<object>() {"?", 15, 25, 35, 45, "", 65, "", 85, 95 } },

            };

            //
            var df = new DataFrame(dict).DropNA();

            //
            var c1 = new object[] { 41, 81};
            var c2 = new object[] { 43, 83 };
            var c3 = new object[] { 45, 85 };


            for (int i = 0; i < c1.Length; i++)
                Assert.Equal(c1[i], df["col1", i]);

            for (int i = 0; i < c2.Length; i++)
                Assert.Equal(c2[i], df["col3", i]);

            for (int i = 0; i < c3.Length; i++)
                Assert.Equal(c3[i], df["col5", i]);


        }
        [Fact]
        public void RemoveMissingValues_Test02()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() {  1, 11, "", 31, 41, 51, "", 71, 81, "?"} },
                { "col2",new List<object>() {  2, 12, 22,"?", 42, 52, 62, 72, 82, 92 } },
                { "col3",new List<object>() {  3, "", 23, 33, 43, 53, 63, 73, 83, 93 } },
                { "col4",new List<object>() {  4, 14, 24, 34, 44, 54, 64, 74, 84, 94} },
                { "col5",new List<object>() {"?", 15, 25, 35, 45, "", 65, "", 85, 95 } },

            };
            //
            var df = new DataFrame(dict);

            //
            var c1 = new int[] { 1, 11, 33333, 31, 41, 51, 33333, 71, 81, 33333 };
            var c2 = new object[] { 2, 12, 22, 33333, 42, 52, 62, 72, 82, 92 };
            var c5 = new object[] { 33333, 15, 25, 35, 45, 33333, 65, 33333, 85, 95 };

            df.FillNA(new string[] { "col1", "col2", "col5"}, 33333);

            for (int i = 0; i < c1.Length; i++)
                Assert.Equal(c1[i], df["col1", i]);

            for(int i = 0; i < c2.Length; i++)
                Assert.Equal(c2[i], df["col2", i]);

            for(int i = 0; i < c5.Length; i++)
                Assert.Equal(c5[i], df["col5", i]);


        }
        [Fact]
        public void ReplaceMissingValue_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() {  1, 11, "", 31, 41, 51, "", 71, 81, "?"} },
                { "col2",new List<object>() {  2, 12, 22,"?", 42, 52, 62, 72, 82, 92 } },
                { "col3",new List<object>() {  3, "", 23, 33, 43, 53, 63, 73, 83, 93 } },
                { "col4",new List<object>() {  4, 14, 24, 34, 44, 54, 64, 74, 84, 94} },
                { "col5",new List<object>() {"?", 15, 25, 35, 45, "", 65, "", 85, 95 } },

            };
            //
            var df = new DataFrame(dict);

            int val = (int)df["col1"].Where(x => x!=DataFrame.NAN).Average(x => (int)x);
            df.FillNAByValue("col1", val);
            var col1 = df["col1"].ToList();
            Assert.Equal(col1[2], val);
            Assert.Equal(col1[2], val);
            Assert.Equal(df[2].ElementAt(0), val);
            Assert.Equal(df[6].ElementAt(0), val);


        }

        [Fact]
        public void ReplaceMissingValue_Test02()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() {  1, 11, "", 31, 41, 51, "", 71, 81, "?"} },
                { "col2",new List<object>() {  2, 12, 22,"?", 42, 52, 62, 72, 82, 92 } },
                { "col3",new List<object>() {  3, "", 23, 33, 43, 53, 63, 73, 83, 94 } },
                { "col4",new List<object>() {  4, 14, 24, 34, 44, 54, 64, 74, 84, 94} },
                { "col5",new List<object>() {"?", 15, 25, 35, 45, "", 65, "", 85, 95 } },

            };

            //
            var df = new DataFrame(dict);
            df.FillNA("col1", Aggregation.Max);
            df.FillNA("col2", Aggregation.Min);
            df.FillNA("col3", Aggregation.Avg);

            //
            var c1 = new object[] { 1, 11, 81, 31, 41, 51, 81, 71, 81, 81 };
            var c2 = new object[] { 2, 12, 22, 2, 42, 52, 62, 72, 82, 92 };
            var c3 = new object[] { 3, 52, 23, 33, 43, 53, 63, 73, 83, 94 };


            for (int i = 0; i < c1.Length; i++)
                Assert.Equal(c1[i], df["col1", i]);

            for (int i = 0; i < c2.Length; i++)
                Assert.Equal(c2[i], df["col2", i]);

            for (int i = 0; i < c3.Length; i++)
                Assert.Equal(c3[i], df["col3", i]);


        }

    }

}
