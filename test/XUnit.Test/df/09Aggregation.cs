using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using System.Globalization;
using Daany.MathStuff;

namespace Unit.Test.DF
{
    public class DataFrameAggregationTests
    {
        private DataFrame createDataFrame()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>()  { 1,2,3,4,5,6,7,8,9,10} },
                { "A",new List<object>()  { -2.385977,-1.004295,0.735167, -0.702657,-0.246845,2.463718, -1.142255,1.396598, -0.543425,-0.64050} },
                { "B",new List<object>()  { -0.102758,0.905829, -0.165272,-1.340923,0.211596, 3.157577, 2.340594, -1.647453,1.761277, 0.289374} },
                { "C",new List<object>()  { 0.438822, -0.954544,-1.619346,-0.706334,-0.901819,-1.380906,-0.039875,1.677227, -0.220481,-1.55067} },
                { "D",new List<object>()  { "chair", "label", "item", "window", "computer", "label", "chair", "item", "abaqus", "window" } },
                {"E", new List<object>() { DateTime.ParseExact("12/20/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("6/13/2016" , "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("8/25/2016",  "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("11/4/2016" , "MM/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("6/18/2016",  "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("3/8/2016" ,  "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("9/3/2016" ,  "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("11/24/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("6/16/2016",  "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("1/31/2016",  "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)}
                }
            };
            
            return new DataFrame(dict);
        }

        [Fact]
        public void Aggregate_Test01()
        {
            //
            var agg = new Dictionary<string, Aggregation[]>();
            agg.Add("A", new Aggregation[] { Aggregation.Min, Aggregation.Max });
            agg.Add("B", new Aggregation[] { Aggregation.Min, Aggregation.Avg, Aggregation.Max });
            agg.Add("C", new Aggregation[] { Aggregation.Count });
            agg.Add("E", new Aggregation[] { Aggregation.Min, Aggregation.Max });

            //
            var df = createDataFrame();
            var rollingdf = df.Aggragate(agg);
            var val = new List<object>()
                //A                 B           C               E
            { -2.385977,        -1.647453,      DataFrame.NAN,  DateTime.ParseExact("1/31/2016", "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
               2.463718,        3.157577,       DataFrame.NAN,  DateTime.ParseExact("12/20/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
               DataFrame.NAN,   0.540984,      DataFrame.NAN,  DataFrame.NAN,
               DataFrame.NAN,   DataFrame.NAN,  10,             DataFrame.NAN
            };



            //
            for (int i = 0; i < rollingdf.Values.Count; i++)
            {
                Assert.Equal(rollingdf.Values[i], val[i]);
            }
        }

        [Fact(Skip ="The data is not defined corectly.")]
        public void Aggregate_Test02()
        {
            //
            var agg = new Dictionary<string, Aggregation[]>();
            agg.Add("A", new Aggregation[] { Aggregation.Min, Aggregation.Max });
            agg.Add("B", new Aggregation[] { Aggregation.Min, Aggregation.Avg, Aggregation.Max });
            agg.Add("C", new Aggregation[] { Aggregation.Count });
            agg.Add("E", new Aggregation[] { Aggregation.Min, Aggregation.Max });

            //set random seed
            Daany.MathStuff.ThreadSafeRandom.FixedRandomSeed = true;
            //
            var df = createDataFrame();
            var rollingdf = df.Aggragate(agg);
            var val = new List<object>()
                //A                 B           C               E
            { -2.385977,        -1.647453,      DataFrame.NAN,  DateTime.ParseExact("1/31/2016", "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
               2.463718,        3.157577,       DataFrame.NAN,  DateTime.ParseExact("12/20/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
               DataFrame.NAN,   0.540984,       DataFrame.NAN,  DataFrame.NAN,
               DataFrame.NAN,   DataFrame.NAN,  DataFrame.NAN,  DataFrame.NAN,
               DataFrame.NAN,   -0.102758,      10,             DataFrame.NAN
            };

          
            //
            for (int i = 0; i < rollingdf.Values.Count; i++)
            {
                Assert.Equal(rollingdf.Values[i], val[i]);
            }
        }

    }

}
