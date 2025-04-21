using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using System.Diagnostics;

namespace Unit.Test.DF
{
    public class JoinDataFramesTests
    {
		private DataFrame CreateLeftDataFrame()
		{
			return new DataFrame(
				new List<object> { 1, "A", 2, "B", 3, "C" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });
		}

		private DataFrame CreateRightDataFrame()
		{
			return new DataFrame(
				new List<object> { 2, "D", 3, "E", 4, "F" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });
		}

		private DataFrame CreateOneColumnLeftDataFrame()
		{
			return new DataFrame(
				new List<object> { 1, 2, 3 },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1" },
				new ColType[] { ColType.I32 });
		}

		private DataFrame CreateOneColumnRightDataFrame()
		{
			return new DataFrame(
				new List<object> { "A", "B", "C" },
				new List<object> { "row1", "row3", "row4" },
				new List<string> { "col2" },
				new ColType[] { ColType.STR });
		}
		[Fact]
		public void Merge_ShouldPerformInnerMerge_SingleColumn()
		{
			// Arrange
			var leftDf = new DataFrame(
				new List<object> { 1, 2, 3, 4 },
				new List<object> { "row1", "row2", "row3", "row4" },
				new List<string> { "col1" },
				new ColType[] { ColType.I32 });

			var rightDf = new DataFrame(
				new List<object> { 2, 3, 5 },
				new List<object> { "rowA", "rowB", "rowC" },
				new List<string> { "col1" },
				new ColType[] { ColType.I32 });

			// Act
			var result = leftDf.Merge(rightDf, new[] { "col1" }, new[] { "col1" }, JoinType.Inner);

			// Assert
			// Values should reflect the intersection of col1 values in both DataFrames
			Assert.Equal(new List<object> { 2, 2, 3, 3 }, result.Values);
			// The index from leftDf for common values in col1
			Assert.Equal(new List<object> { "row2", "row3" }, result.Index);
			// Combined columns, with col1 from rightDf renamed using the default suffix "right"
			Assert.Equal(new List<string> { "col1", "col1_right" }, result.Columns);
		}


		[Fact]
		public void Merge_ShouldPerformInnerMerge_TwoColumns()
		{
			// Arrange
			var leftDf = new DataFrame(
				new List<object> { 1, "A", 2, "B", 3, "C" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			var rightDf = new DataFrame(
				new List<object> { 2, "B", 3, "C", 4, "D" },
				new List<object> { "rowA", "rowB", "rowC" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			// Act
			var result = leftDf.Merge(rightDf, new[] { "col1", "col2" }, new[] { "col1", "col2" }, JoinType.Inner);

			// Assert
			Assert.Equal(new List<object> { 2, "B", 2, "B", 3, "C", 3, "C" }, result.Values);
			Assert.Equal(new List<object> { "row2", "row3" }, result.Index);
			Assert.Equal(new List<string> { "col1", "col2", "col1_right", "col2_right" }, result.Columns);
		}

		[Fact]
		public void Merge_ShouldPerformLeftMerge_WithMissingValues()
		{
			// Arrange
			var leftDf = new DataFrame(
				new List<object> { 1, "A", 2, "B", 3, "C" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			var rightDf = new DataFrame(
				new List<object> { 2, "B", 3, "C", 4, "D" },
				new List<object> { "rowA", "rowB", "rowC" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			// Act
			var result = leftDf.Merge(rightDf, new[] { "col1", "col2" }, new[] { "col1", "col2" }, JoinType.Left);

			// Assert
			Assert.Equal(new List<object> { 1, "A", DataFrame.NAN, DataFrame.NAN, 2, "B", 2, "B", 3, "C", 3, "C" }, result.Values);
			Assert.Equal(new List<object> { "row1", "row2", "row3" }, result.Index);
			Assert.Equal(new List<string> { "col1", "col2", "col1_right", "col2_right" }, result.Columns);
		}

		[Fact]
		public void Merge_ShouldPerformInnerMerge_OnSingleColumn()
		{
			// Arrange
			var leftDf = new DataFrame(
				new List<object> { 1, 2, 3 },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1" },
				new ColType[] { ColType.I32 });

			var rightDf = new DataFrame(
				new List<object> { 1, 2, 3 },
				new List<object> { "rowA", "rowB", "rowC" },
				new List<string> { "col1" },
				new ColType[] { ColType.I32 });

			// Act
			var result = leftDf.Merge(rightDf, new[] { "col1" }, new[] { "col1" }, JoinType.Inner);

			// Assert
			Assert.Equal(new List<object> { 1, 1, 2, 2, 3, 3 }, result.Values);
			Assert.Equal(new List<object> { "row1", "row2", "row3" }, result.Index);
			Assert.Equal(new List<string> { "col1", "col1_right" }, result.Columns);
		}


		[Fact]
		public void Merge_ShouldPerformLeftMerge_WhenNoMatchingRows()
		{
			//df1
			// col1, col2
			// 1,    A
			// 2,    B
			// 3,    C
			//df2
			// col1, col2
			// 2,    D
			// 3,    E
			// 4,    F
			//result
			// col1, col2, col1_right, col2_right
			// 1,       A, DataFrame.NAN, DataFrame.NAN
			// 2,       B, DataFrame.NAN, DataFrame.NAN
			// 3,       C, DataFrame.NAN, DataFrame.NAN


			// Arrange
			var leftDf = CreateLeftDataFrame();
			var rightDf = CreateRightDataFrame();

			// Act
			var result = leftDf.Merge(rightDf, new[] { "col1" }, new[] { "col2" }, JoinType.Left);

			// Assert
			Assert.Equal(new List<object> { 1, "A", DataFrame.NAN, DataFrame.NAN, 
                                            2, "B", DataFrame.NAN, DataFrame.NAN,
                                            3, "C", DataFrame.NAN, DataFrame.NAN }, result.Values);

			Assert.Equal(new List<object> { "row1", "row2", "row3" }, result.Index);
			Assert.Equal(new List<string> { "col1", "col2", "col1_right", "col2_right" }, result.Columns);
		}

		[Fact]
		public void Merge_ShouldPerformLeftMerge_WhenNoMatchingRows_INNER()
		{
			//df1
			// col1, col2
			// 1,    A
			// 2,    B
			// 3,    C
			//df2
			// col1, col2
			// 2,    D
			// 3,    E
			// 4,    F
			//result
			// col1, col2, col1_right, col2_right


			// Arrange
			var leftDf = CreateLeftDataFrame();
			var rightDf = CreateRightDataFrame();

			// Act
			var result = leftDf.Merge(rightDf, new[] { "col1" }, new[] { "col2" }, JoinType.Inner);

			// Assert
			Assert.Empty(result.Values);

			Assert.Empty(result.Index);
			Assert.Equal(new List<string> { "col1", "col2", "col1_right", "col2_right" }, result.Columns);
		}

		
		[Fact]
		public void Merge_ShouldThrowException_WhenJoinColumnsAreNull()
		{
			// Arrange
			var leftDf = CreateLeftDataFrame();
			var rightDf = CreateRightDataFrame();

			// Act & Assert
			Assert.Throws<ArgumentException>(() => leftDf.Merge(rightDf, null, new[] { "col1" }, JoinType.Inner));
		}

		[Fact]
		public void Merge_ShouldThrowException_WhenJoinColumnsExceedLimit()
		{
			// Arrange
			var leftDf = CreateLeftDataFrame();
			var rightDf = CreateRightDataFrame();

			// Act & Assert
			Assert.Throws<Exception>(() =>
				leftDf.Merge(rightDf, new[] { "col1", "col2", "col3", "col4" }, new[] { "col1", "col2", "col3", "col4" }, JoinType.Inner));
		}

		[Fact]
		public void Merge_ShouldThrowArgumentExcaptionWithEmptyDataFrame()
		{
			// Arrange
			var leftDf = new DataFrame(new List<object>(), new List<object>(), new List<string>(), new ColType[0]);
			var rightDf = CreateRightDataFrame();

			// Act & Assert
			Assert.Throws<ArgumentException>(() =>
			    leftDf.Merge(rightDf, new[] { "col1" }, new[] { "col1" }, JoinType.Left));
		}



		[Fact]
		public void Join_ShouldPerformInnerJoin()
		{
			// Arrange
			var leftDf = CreateOneColumnLeftDataFrame();
			var rightDf = CreateOneColumnRightDataFrame();

			// Act
			var result = leftDf.Join(rightDf, JoinType.Inner);

			// Assert
			Assert.Equal(new List<object> { 1, "A", 3, "B" }, result.Values);
			Assert.Equal(new List<object> { "row1", "row3" }, result.Index);
			Assert.Equal(new List<string> { "col1", "col2" }, result.Columns);
		}

		[Fact]
		public void Join_ShouldPerformLeftJoin()
		{
			// Arrange
			var leftDf = CreateOneColumnLeftDataFrame();
			var rightDf = CreateOneColumnRightDataFrame();

			// Act
			var result = leftDf.Join(rightDf, JoinType.Left);

			// Assert
			Assert.Equal(new List<object> { 1, "A", 2, DataFrame.NAN, 3, "B" }, result.Values);
			Assert.Equal(new List<object> { "row1", "row2", "row3" }, result.Index);
			Assert.Equal(new List<string> { "col1", "col2" }, result.Columns);
		}

		[Fact]
		public void Join_ShouldThrowException_WhenRightDataFrameIsNull()
		{
			// Arrange
			var leftDf = CreateOneColumnLeftDataFrame();

			// Act & Assert
			Assert.Throws<ArgumentException>(() => leftDf.Join(null, JoinType.Inner));
		}

		[Fact]
		public void Join_ShouldReturnEmptyDataFrame_WhenLeftDataFrameIsEmpty()
		{
			// Arrange
			var leftDf = new DataFrame(new List<object>(), new List<object>(), new List<string>(), new ColType[0]);
			var rightDf = CreateOneColumnRightDataFrame();

			// Act
			var result = leftDf.Join(rightDf, JoinType.Inner);

			// Assert
			Assert.Empty(result.Values);
			Assert.Empty(result.Index);
			Assert.Equal(new List<string> { "col2" }, result.Columns);
		}

		[Fact]
        public void JoinTwoDataFrameByIndex_Test1()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };
            var dict1 = new Dictionary<string, List<object>>
            {
                { "item2ID",new List<object>() {"foo", "bar", "baz" } },
                { "value2",new List<object>() { 5,6,7 } },
            };
            //
            var df1 = new DataFrame(dict);
            var df2 = new DataFrame(dict1);
            //
            var mergedDf = df1.Join(df2, JoinType.Inner);
            var e1 = new object[] { "foo", 1, "foo", 5, "bar", 2, "bar", 6, "baz", 3, "baz", 7 };
            var dd = mergedDf.ToStringBuilder();
            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);

            //
            mergedDf = df1.Join(df2, JoinType.Left);
            e1 = new object[] { "foo", 1, "foo", 5, "bar", 2, "bar", 6, "baz", 3, "baz", 7, "foo", 4, DataFrame.NAN, DataFrame.NAN };
            dd = mergedDf.ToStringBuilder();
            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);

        }


        [Fact]
        public void JoinByIndex_Test2()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };
            var dict1 = new Dictionary<string, List<object>>
            {
                { "item2ID",new List<object>() {"foo", "bar", "baz" } },
                { "value2",new List<object>() { 5,6,7 } },
            };
            //
            var df1 = new DataFrame(dict1);
            var df2 = new DataFrame(dict);
            //
            var mergedDf = df1.Join(df2, JoinType.Inner);
            var e1 = new object[] { "foo", 5, "foo", 1, "bar", 6, "bar", 2, "baz", 7, "baz", 3 };
            var dd = mergedDf.ToStringBuilder();
            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);

            //
            mergedDf = df1.Join(df2, JoinType.Left);
            e1 = new object[] { "foo", 5, "foo", 1, "bar", 6, "bar", 2, "baz", 7, "baz", 3 };
            dd = mergedDf.ToStringBuilder();
            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);

        }




        [Fact]
        public void MergeWithSingleColumn_Test011()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };
            var dict1 = new Dictionary<string, List<object>>
            {
                { "item2ID",new List<object>() {"foo", "bar", "baz" } },
                { "value2",new List<object>() { 5,6,7 } },
            };
            //
            var df1 = new DataFrame(dict);
            var df2 = new DataFrame(dict1);
            //
            var mergedDf = df1.Merge(df2, new string[] { "itemID" }, new string[] { "item2ID" }, JoinType.Inner);
            var e1 = new object[] { "foo", 1, "foo", 5, "bar", 2, "bar", 6, "baz", 3, "baz", 7, "foo", 4, "foo", 5, };
            var dd = mergedDf.ToStringBuilder();
            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);

            //
            mergedDf = df1.Merge(df2, new string[] { "itemID" }, new string[] { "item2ID" }, JoinType.Left);
            e1 = new object[] { "foo", 1, "foo", 5, "bar", 2, "bar", 6, "baz", 3, "baz", 7, "foo", 4, "foo", 5, };
            dd = mergedDf.ToStringBuilder();
            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);

            //
            mergedDf = df1.Merge(df2, new string[] { "value1" }, new string[] { "value2" }, JoinType.Inner);
            e1 = new object[] { "foo", 1, "foo", 5, "bar", 2, "bar", 6, "baz", 3, "baz", 7, "foo", 4, "foo", 5, };
            dd = mergedDf.ToStringBuilder();
            //row test
            Assert.Equal(0, mergedDf.RowCount());

        }

        [Fact]
        public void MergeWithSingleColumn_Test0111()
        {
            var dict1 = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };
            var dict = new Dictionary<string, List<object>>
            {
                { "item2ID",new List<object>() {"foo", "bar", "baz" } },
                { "value2",new List<object>() { 5,6,7 } },
            };
            //
            var df1 = new DataFrame(dict);
            var df2 = new DataFrame(dict1);
            //
            var mergedDf = df1.Merge(df2, new string[] { "item2ID" }, new string[] { "itemID" }, JoinType.Inner);
            var e1 = new object[] { "foo", 5, "foo", 1, "foo", 5, "foo", 4, "bar", 6, "bar", 2, "baz", 7, "baz", 3, };

            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);

            //
            mergedDf = df1.Merge(df2, new string[] { "item2ID" }, new string[] { "itemID" }, JoinType.Left);
            e1 = new object[] { "foo", 5, "foo", 1, "foo", 5, "foo", 4, "bar", 6, "bar", 2, "baz", 7, "baz", 3, };

            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);

            //
            mergedDf = df1.Merge(df2, new string[] { "value2" }, new string[] { "value1" }, JoinType.Left);
            e1 = new object[] { "foo", 5, DataFrame.NAN, DataFrame.NAN, "bar", 6, DataFrame.NAN, DataFrame.NAN, "baz", 7, DataFrame.NAN, DataFrame.NAN, };

            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);

        }



        [Fact]
        public void MergeBySingleColumn_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };
            var dict1 = new Dictionary<string, List<object>>
            {
                { "item2ID",new List<object>() {"foo", "bar", "baz","foo" } },
                { "value2",new List<object>() { 5,6,7,8 } },
            };
            //
            var df1 = new DataFrame(dict);
            var df2 = new DataFrame(dict1);
            //
            var mergedDf = df1.Merge(df2, new string[] { "itemID" }, new string[] { "item2ID" }, JoinType.Inner);
            var e1 = new object[] { "foo", 1, "foo", 5, "foo", 1, "foo", 8, "bar", 2, "bar", 6, "baz", 3, "baz", 7, "foo", 4, "foo", 5, "foo", 4, "foo", 8 };

            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);

        }

        [Fact]
        public void MergeByTwoColumns_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "catId",new List<object>() { "A", "A", "B", "B" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };
            var dict1 = new Dictionary<string, List<object>>
            {
                { "item2ID",new List<object>() {"foo", "bar", "baz","foo" } },
                { "cat2ID",new List<object>() { "A", "B", "A", "B" } },
                { "value2",new List<object>() { 5,6,7,8 } },
            };
            //
            var df1 = new DataFrame(dict);
            var df2 = new DataFrame(dict1);
            var mergedDf = df1.Merge(df2, new string[] { "itemID", "catId" }, new string[] { "item2ID", "cat2ID" }, JoinType.Inner);

            //row test
            var r1 = mergedDf[0].ToList();
            var r2 = mergedDf[1].ToList();

            var e1 = new object[] { "foo", "A", 1, "foo", "A", 5 };
            var e2 = new object[] { "foo", "B", 4, "foo", "B", 8 };

            for (int i = 0; i < r1.Count; i++)
                Assert.Equal(r1[i].ToString(), e1[i].ToString());
            for (int i = 0; i < r2.Count; i++)
                Assert.Equal(r2[i].ToString(), e2[i].ToString());

            //column test
            var c1 = new string[] { "foo", "foo" };
            var c2 = new string[] { "A", "B" };
            var c3 = new int[] { 1, 4 };
            var cc1 = mergedDf["itemID"].ToList();
            var cc2 = mergedDf["catId"].ToList();
            var cc3 = mergedDf["value1"].ToList();

            for (int i = 0; i < c1.Count(); i++)
                Assert.Equal(c1[i], cc1[i]);
            for (int i = 0; i < c2.Count(); i++)
                Assert.Equal(c2[i], cc2[i]);
            for (int i = 0; i < c3.Count(); i++)
                Assert.Equal((int)c3[i], cc3[i]);
        }

        [Fact]
        public void MergeByThreeColumns_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "catId",new List<object>() { "A", "A", "B", "B" } },
                { "value1",new List<object>() { 1,2,3,4 } },
                { "class1",new List<object>(){ 1, 1, 2, 5 } }
            };
            var dict1 = new Dictionary<string, List<object>>
            {
                { "item2ID",new List<object>() {"foo", "bar", "baz","foo" } },
                { "cat2ID",new List<object>() { "A", "B", "A", "B" } },
                { "value2",new List<object>() { 5,6,7,8 } },
                { "class2",new List<object>(){ 1, 3, 5, 5 } }
            };
            //
            var df1 = new DataFrame(dict);
            var df2 = new DataFrame(dict1);
            var mergedDf = df1.Merge(df2, new string[] { "itemID", "catId", "class1" }, new string[] { "item2ID", "cat2ID", "class2" }, JoinType.Inner);
            var e1 = new object[] { "foo", "A", 1, 1, "foo", "A", 5, 1, "foo", "B", 4, 5, "foo", "B", 8, 5 };

            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);
        }


        [Fact(Skip = "Run only if files exist on the machine")]

        public void MergeByTwoColumns_Test01()
        {
            //datetime,machineID,volt,rotate,pressure,vibration
            var telemetryPath = @"C:\sc\vs\Academic\PrM\Data\telemetry.csv";
            //machineID,model,age
            var errorMachinePath = @"C:\sc\vs\Academic\PrM\Data\errorfeat.csv";

            
            var telemetry = DataFrame.FromCsv(telemetryPath, sep: ',', parseDate: true);
            var errorMachine = DataFrame.FromCsv(errorMachinePath, sep: ',', parseDate: true);
           
            var mCols = new string[] { "datetime", "machineID" };
            
            var newDf = telemetry.Merge_old(errorMachine, mCols, mCols, JoinType.Left);
            var newDf1 = telemetry.Merge(errorMachine, mCols, mCols, JoinType.Left,"rDf");

            for (int i = 0; i < newDf1.Values.Count(); i++)
            {
                Assert.Equal(newDf.Values[i], newDf1.Values[i]);
            }
        }

        [Fact(Skip = "Run only if files exist on the machine")]
        public void MergeByOneColumns_Test02()
        {
            //datetime,machineID,volt,rotate,pressure,vibration
            var salesPath = @"C:\sc\vs\PredictFutureSales\Data\sales_train_v2.csv";
            //machineID,model,age
            var productIdsPath = @"C:\sc\vs\PredictFutureSales\Data\items.csv"; 


            var sales = DataFrame.FromCsv(salesPath, sep: ',', dformat:"dd.mm.yyyy");
            var products = DataFrame.FromCsv(productIdsPath, sep: ',', parseDate: true);

            var mCols = new string[] { "item_id" };

            var newDf = sales.Merge_old(products, mCols, mCols, JoinType.Left);
            var newDf1 = sales.Merge(products, mCols, mCols, JoinType.Left);

            //
            for (int i = 0; i < newDf1.Values.Count(); i++)
            {
                Assert.Equal(newDf.Values[i], newDf1.Values[i]);
            }
        }
    }
}
