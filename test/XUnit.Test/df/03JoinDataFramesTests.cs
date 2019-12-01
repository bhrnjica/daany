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


        [Fact]
        
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

        [Fact]
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
