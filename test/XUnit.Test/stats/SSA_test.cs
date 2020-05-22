using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.stl;
using Daany.MathStuff;
using Daany.Stat;

namespace Unit.Test.DF
{
    public class SSATest
    {
        static string root = "..\\..\\..\\testdata";

        private double[] getAirPassengersData()
        {
            var strPath = $"{root}/AirPassengers.csv";
            var mlDF = DataFrame.FromCsv(strPath, sep: ',', dformat: null);
            //
            return mlDF["#Passengers"].Select(x => Convert.ToDouble(x)).ToArray();

        }

        [Fact]
        public void SSA_Decomposition_Test01()
        {
            double[] ts = getAirPassengersData(); // Monthly time-series data

            //embed
            var ssa = new SSA(ts);
            //embed
            var x = ssa.Embedding(36);

            //decompose
            ssa.Decompose();

            var cc = ssa.Contributions;

            var c = ssa.SContributions(true, true);

            var c1 = ssa.SContributions(true, false);

            var c2 = ssa.SContributions(false, false);

            var c3 = ssa.SContributions(false, true);

            var sss = c.Add(c1).Add(c2).Add(c3);

            //Check some results
            Assert.Equal(11001.040457656189, sss[0]);
            Assert.Equal(10474.800198389861, sss[3]);
            Assert.Equal(9334.453037025567, sss[5]);
            Assert.Equal(7667.7872722835718, sss[7]);
            Assert.Equal(6001.1213064983767, sss[9]);
            Assert.Equal(1001.1218069488897, sss[11]);
            Assert.Equal(10858.260124510331, sss[2]);
            Assert.Equal(1001.1224072190787, sss[14]);

        }

    }

}
