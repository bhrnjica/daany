using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class JoinDataFramesTests
    {
    
        [Fact]
        public void JoinBySingleColumn_Test01()
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
            var mergedDf = df1.Join(df2, new string[] { "itemID" }, new string[] { "item2ID" }, JoinType.Inner);
            var e1 = new object[] { "foo", 1, "foo", 5, "foo", 1, "foo", 8, "bar", 2, "bar", 6, "baz", 3, "baz", 7, "foo", 4, "foo", 5, "foo", 4, "foo", 8 };

            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);
          
        }

        [Fact]
        public void JoinByTwoColumns_Test()
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
            var mergedDf = df1.Join(df2, new string[] { "itemID", "catId" }, new string[] { "item2ID", "cat2ID" }, JoinType.Inner);

            //row test
            var r1 = mergedDf[0].ToList();
            var r2 = mergedDf[1].ToList();
            
            var e1 = new object[] { "foo","A", 1, "foo","A", 5 };
            var e2 = new object[] { "foo","B", 4, "foo","B", 8 };
           
            for (int i = 0; i < r1.Count; i++)
                Assert.Equal(r1[i].ToString(), e1[i].ToString());
            for (int i = 0; i < r2.Count; i++)
                Assert.Equal(r2[i].ToString(), e2[i].ToString());
           
            //column test
            var c1 = new string[] { "foo", "foo" };
            var c2 = new string[] {"A","B"};
            var c3 = new int[] { 1,4 };
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
        public void JoinByThreeColumns_Test()
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
            var mergedDf = df1.Join(df2, new string[] { "itemID", "catId","class1" }, new string[] { "item2ID", "cat2ID", "class2" }, JoinType.Inner);
            var e1 = new object[] { "foo","A",1 ,1, "foo", "A",5, 1, "foo","B",4, 5, "foo", "B",8,5 };

            //row test
            for (int i = 0; i < mergedDf.Values.Count; i++)
                Assert.Equal(mergedDf.Values[i], e1[i]);
        }
    }
}
