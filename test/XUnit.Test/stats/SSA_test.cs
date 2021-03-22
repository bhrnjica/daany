using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.stl;
using Daany.MathStuff;
using Daany.Stat;
using Daany.Ext;
using XPlot.Plotly;

namespace Unit.Test.DF
{
    public class SSATest
    {
        static string root = "testdata";
        double[] _ts = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        private DataFrame getwCorrelationMatrix()
        {
            var strPath = $"{root}/wCorrelationResults.txt";
            var mlDF = DataFrame.FromCsv(strPath, sep: '\t', dformat: null);
            //
            return mlDF;

        }
        private double[] getAirPassengersData()
        {
            var strPath = $"{root}/AirPassengers.csv";
            var mlDF = DataFrame.FromCsv(strPath, sep: ',', dformat: null);
            //
            return mlDF["#Passengers"].Select(x => Convert.ToDouble(x)).ToArray();

        }

        private DataFrame getMonthlyDeatsData()
        {
            var strPath = $"{root}/monthly_acc_deaths_usa_1973_78.txt";
            var mlDF = DataFrame.FromCsv(strPath, sep: '\t', skipLines: 1);

            var monthlySeries = nc.GenerateMonthlySeries(new DateTime(1973, 1, 1), 1, (1979 - 1973) * 12);
            var mainDF = new DataFrame(monthlySeries, new List<string> { "Date" }, null);
            mainDF.AddCalculatedColumn("D", (object[] r, int i) =>
            {
                var date = Convert.ToDateTime(r[0]);
                //create index for accessing the temp and precipitation
                int year = date.Year - 1973+1;
                int month = date.Month-1;
                return mlDF[month, year];
            });
            //add index
            mainDF = mainDF.SetIndex("Date");
            //
            return mainDF;

        }


        private XPlot.Plotly.PlotlyChart PlotMatrix(double[,] matrix)
        {
            var m = matrix.Reverse(false);
            var s1 = new Graph.Heatmap()
            {
                name = "Heat-map of Matrix",
                colorscale = @"YIGnBu",
                z = m
            };

            var chart = XPlot.Plotly.Chart.Plot<Graph.Trace>(new Graph.Trace[] { s1 });
            return chart;
        }
        [Fact]
        public void SSA_TestHankelMatrix()
        {

           
            var ssa = new SSA(_ts);
            var X = ssa.Embedding(10);
            //

            Assert.Equal(1, X[0, 0]);
            Assert.Equal(new List<double> { 2,2 }, new List<double> { X[0, 1], X[1, 0] });
            Assert.Equal(new List<double> { 10,10,10,10,10,10,10,10,10,10},
                new List<double> { X[0, 9], X[1, 8], X[2, 7], X[3, 6], X[4, 5], X[5, 4], X[6, 3], X[7, 2], X[8, 1], X[9, 0]});
            Assert.Equal(new List<double> { 16, 16,16,16,16 }, new List<double> { X[5, 10], X[6, 9], X[7, 8], X[8, 7],X[9, 6] });


        }

        [Fact]
        public void SSA_TestDecomposition()
        {
            var ssa = new SSA(_ts);
            ssa.Embedding(10);
            //
            ssa.Decompose();

            Assert.Equal(2, ssa.EigenTriple.Count());
            Assert.Equal(118.59, ssa.EigenTriple[1].Li,2);
            Assert.Equal(8.42, ssa.EigenTriple[2].Li,2);
            
            //component 1 test
            Assert.Equal(3.54026,   ssa.EM[1][0,0],5);
            Assert.Equal(11.90644,  ssa.EM[1][2,10],5);
            Assert.Equal(13.24560,  ssa.EM[1][3,10],5);
            Assert.Equal(17.48321,  ssa.EM[1][8,8],5);
            Assert.Equal(19.94144,  ssa.EM[1][8,10],5);
            Assert.Equal(13.41069,  ssa.EM[1][9,4],5);
            Assert.Equal(21.28061,  ssa.EM[1][9,10],5);

            //component 2 test
            Assert.Equal(-2.54026,  ssa.EM[2][0, 0], 5);
            Assert.Equal(1.09356, ssa.EM[2][2, 10], 5);
            Assert.Equal(0.75440, ssa.EM[2][3, 10], 5);
            Assert.Equal(-0.48321, ssa.EM[2][8, 8], 5);
            Assert.Equal(-0.94144, ssa.EM[2][8, 10], 5);
            Assert.Equal(0.58931, ssa.EM[2][9, 4], 5);
            Assert.Equal(-1.28061, ssa.EM[2][9, 10], 5);
        }


        [Fact]
        public void SSA_TestReconstruction()
        {
            var ssa = new SSA(_ts);
            ssa.Embedding(10);
            //
            ssa.Decompose();
            //diagonal averaginign
            var ts1 = ssa.Reconstruct(1,2);

            Assert.Equal(1, ts1[0],5);
            Assert.Equal(2, ts1[1], 5);
            Assert.Equal(3, ts1[2], 5);
            Assert.Equal(4, ts1[3], 5);
            Assert.Equal(5, ts1[4], 5);
            Assert.Equal(6, ts1[5], 5);
            Assert.Equal(7, ts1[6], 5);
            Assert.Equal(8, ts1[7], 5);
            Assert.Equal(9, ts1[8], 5);
            Assert.Equal(10, ts1[9], 5);
            Assert.Equal(11, ts1[10], 5);
            Assert.Equal(12, ts1[11], 5);
            Assert.Equal(13, ts1[12], 5);
            Assert.Equal(14, ts1[13], 5);
            Assert.Equal(15, ts1[14], 5);
            Assert.Equal(16, ts1[15], 5);
            Assert.Equal(17, ts1[16], 5);
            Assert.Equal(18, ts1[17], 5);
            Assert.Equal(19, ts1[18], 5);
            Assert.Equal(20, ts1[19], 5);
        }

        [Fact]
        public void SSA_TestRForecast()
        {
            int stepsAhead = 3;
            
            var ssa = new SSA(_ts);
            ssa.Embedding(10);
            ssa.Decompose();
            //r forecasting
            var ts1 = ssa.Forecast(new int[2] { 1, 2 },stepsAhead, method: Forecasting.Rforecasing);

            Assert.Equal(1, ts1[0], 5);
            Assert.Equal(2, ts1[1], 5);
            Assert.Equal(3, ts1[2], 5);
            Assert.Equal(4, ts1[3], 5);
            Assert.Equal(5, ts1[4], 5);
            Assert.Equal(6, ts1[5], 5);
            Assert.Equal(7, ts1[6], 5);
            Assert.Equal(8, ts1[7], 5);
            Assert.Equal(9, ts1[8], 5);
            Assert.Equal(10, ts1[9], 5);
            Assert.Equal(11, ts1[10], 5);
            Assert.Equal(12, ts1[11], 5);
            Assert.Equal(13, ts1[12], 5);
            Assert.Equal(14, ts1[13], 5);
            Assert.Equal(15, ts1[14], 5);
            Assert.Equal(16, ts1[15], 5);
            Assert.Equal(17, ts1[16], 5);
            Assert.Equal(18, ts1[17], 5);
            Assert.Equal(19, ts1[18], 5);
            Assert.Equal(20, ts1[19], 5);
            //forecast
            Assert.Equal(21, ts1[20], 5);
            Assert.Equal(22, ts1[21], 5);
            Assert.Equal(23, ts1[22], 5);
        }
        [Fact]
        public void SSA_TestVForecast()
        {
            int stepsAhead = 11;

            var ssa = new SSA(_ts);
            ssa.Embedding(10);
            ssa.Decompose();
            //r forecasting
            var ts1 = ssa.Forecast(new int[2] { 1, 2 }, stepsAhead, method: Forecasting.Vforecasting);

            Assert.Equal(1, ts1[0], 5);
            Assert.Equal(2, ts1[1], 5);
            Assert.Equal(3, ts1[2], 5);
            Assert.Equal(4, ts1[3], 5);
            Assert.Equal(5, ts1[4], 5);
            Assert.Equal(6, ts1[5], 5);
            Assert.Equal(7, ts1[6], 5);
            Assert.Equal(8, ts1[7], 5);
            Assert.Equal(9, ts1[8], 5);
            Assert.Equal(10, ts1[9], 5);
            Assert.Equal(11, ts1[10], 5);
            Assert.Equal(12, ts1[11], 5);
            Assert.Equal(13, ts1[12], 5);
            Assert.Equal(14, ts1[13], 5);
            Assert.Equal(15, ts1[14], 5);
            Assert.Equal(16, ts1[15], 5);
            Assert.Equal(17, ts1[16], 5);
            Assert.Equal(18, ts1[17], 5);
            Assert.Equal(19, ts1[18], 5);
            Assert.Equal(20, ts1[19], 5);

            //forecast
            Assert.Equal(21, ts1[20], 5);
            Assert.Equal(22, ts1[21], 5);
            Assert.Equal(23, ts1[22], 5);
            Assert.Equal(24, ts1[23], 5);
            Assert.Equal(25, ts1[24], 5);
            Assert.Equal(26, ts1[25], 5);
            Assert.Equal(27, ts1[26], 5);
            Assert.Equal(28, ts1[27], 5);
            Assert.Equal(29, ts1[28], 5);
            Assert.Equal(30, ts1[29], 5);
            Assert.Equal(31, ts1[30], 5);
        }
       
        
        [Fact]
        //Test is based on Kaggle notebook and python implementation of SSA
        public void SSATest_ToyTimeSeries01()
        {
            uint L = 70;

            var n = 200;
            var t = nc.GenerateIntSeries(0,n, 1).Select(x=>Convert.ToDouble(x)).ToArray();
            var temp1 = t.Substract(100.0).Pow(2);
            var trend = temp1.Multiply(0.001);
            var p1 = 20.0;
            var p2 = 30.0;
            var periodic1 = t.Multiply(2.0 * Math.PI / p1).Select(x => 2 * Math.Sin(x)).ToArray();
            var periodic2 = t.Multiply(2.0 * Math.PI / p2).Select(x => 0.75 * Math.Sin(x)).ToArray();

            var r = Daany.MathStuff.Constant.FixedRandomSeed = true;
            //Since we cannot replicate random we take it from the source;
            var noise = new double[] {
             0.39293837, -0.42772133, -0.54629709,  0.10262954,  0.43893794,
               -0.15378708,  0.9615284 ,  0.36965948, -0.0381362 , -0.21576496,
               -0.31364397,  0.45809941, -0.12285551, -0.88064421, -0.20391149,
                0.47599081, -0.63501654, -0.64909649,  0.06310275,  0.06365517,
                0.26880192,  0.69886359,  0.44891065,  0.22204702,  0.44488677,
               -0.35408217, -0.27642269, -0.54347354, -0.41257191,  0.26195225,
               -0.81579012, -0.13259765, -0.13827447, -0.0126298 , -0.14833942,
               -0.37547755, -0.14729739,  0.78677833,  0.88832004,  0.00367335,
                0.2479059 , -0.76876321, -0.36542904, -0.17034758,  0.73261832,
               -0.49908927, -0.03393147,  0.97111957,  0.03897024,  0.22578905,
               -0.75874267,  0.6526816 ,  0.20612026,  0.09013601, -0.31447233,
               -0.39175842, -0.16595558,  0.36260153,  0.75091368,  0.02084467,
                0.33862757,  0.17187311,  0.249807  ,  0.3493781 ,  0.68468488,
               -0.83361002,  0.52736568, -0.51266725, -0.61155408,  0.14491391,
               -0.80857497,  0.77065365,  0.25449794,  0.44683272, -0.96774159,
                0.18886376,  0.11357038, -0.68208071, -0.69385897,  0.39105906,
               -0.36246715,  0.38394059,  0.1087665 , -0.22209885,  0.85026498,
                0.68333999, -0.28520487, -0.91281707, -0.39046385, -0.20362864,
                0.40991766,  0.99071696, -0.28817027,  0.52509563,  0.18635383,
                0.3834036 , -0.6977451 , -0.20224741, -0.5182882 , -0.31308797,
                0.02625631,  0.3332491 , -0.78818303, -0.7382101 , -0.35603879,
                0.32312867,  0.69301245,  0.10651469,  0.70890498, -0.23032438,
               -0.36642421, -0.29147065, -0.65783634,  0.65822527, -0.32265831,
                0.10474015,  0.15710294,  0.04306612, -0.99462387,  0.97669084,
                0.81068315, -0.58472828, -0.41502117,  0.04002031,  0.80382275,
                0.96726177, -0.48491587,  0.12871809,  0.61393737, -0.21125989,
                0.46214607, -0.67786197,  0.20139714,  0.73172892,  0.96704322,
               -0.84126842, -0.14330545, -0.59091428, -0.09872702,  0.09552715,
               -0.81334658, -0.40627845,  0.85516848,  0.13800746, -0.085176  ,
                0.50705198,  0.4837243 , -0.90284193,  0.41739479,  0.6784867 ,
               -0.66812423,  0.56199588, -0.42692677, -0.38706049,  0.33052293,
               -0.77721566,  0.3297449 ,  0.77571359,  0.39262254, -0.11934425,
               -0.12357123,  0.53019219,  0.131284  , -0.83019167,  0.16534218,
                0.62968741, -0.32586723,  0.85515316,  0.501434  ,  0.14812765,
                0.50328798, -0.84170208,  0.71877815,  0.64300823,  0.81974332,
               -0.7427376 , -0.83643983, -0.72316885, -0.20124258, -0.15138628,
                0.12443676, -0.7555129 , -0.597201  ,  0.6232887 , -0.06402485,
                0.61587642, -0.98514724,  0.10318545,  0.8638643 ,  0.16435092,
               -0.58780855,  0.43551512, -0.2420283 ,  0.33676789, -0.94136055,
                0.27180072, -0.93560413,  0.48956131, -0.054174  , -0.75649129};//nc.Rand(200, 0d,1d).Select(x=>(double)x).ToArray().Substract(0.5).Multiply(2);

            //toy function
            var f = trend.Add(periodic1.Add(periodic2).Add(noise));

            //construct and decompose the time series.
            var ssa = new SSA(f);
            var em = ssa.Embedding(L);
             ssa.Decompose();

            //
            var rTrend = ssa.Reconstruct(new int[] { 1, 2, 7 });
            var rPer1 = ssa.Reconstruct(new int[] { 3, 4 });
            var rPer2 = ssa.Reconstruct(new int[] { 5, 6 });

            //Test reconstructed components
            var testTrend = new double[] 
            {
                1.12794763e+01,  1.11222948e+01,  1.10169536e+01,  1.08863655e+01,
                1.06853902e+01,  1.03931489e+01,  1.00688871e+01,  9.68813466e+00,
                9.28032006e+00,  8.87127467e+00,  8.47568239e+00,  8.10084450e+00,
                7.74515661e+00,  7.42221287e+00,  7.14337328e+00,  6.90399929e+00,
                6.69367292e+00,  6.52195814e+00,  6.38280912e+00,  6.26390036e+00,
                6.15777257e+00,  6.05979873e+00,  5.95877019e+00,  5.84764883e+00,
                5.72693409e+00,  5.59038886e+00,  5.44691196e+00,  5.29435361e+00,
                5.13710987e+00,  4.97537350e+00,  4.80894410e+00,  4.65007745e+00,
                4.49642526e+00,  4.34888363e+00,  4.21004949e+00,  4.08456981e+00,
                3.97474947e+00,  3.87778441e+00,  3.78439997e+00,  3.69259752e+00,
                3.60301152e+00,  3.51188069e+00,  3.42230365e+00,  3.32920568e+00,
                3.23153049e+00,  3.12161348e+00,  3.00660092e+00,  2.88435775e+00,
                2.75172388e+00,  2.61500546e+00,  2.47898457e+00,  2.35062530e+00,
                2.22490094e+00,  2.10649533e+00,  1.99896709e+00,  1.90696915e+00,
                1.82970455e+00,  1.76354502e+00,  1.70611509e+00,  1.65440231e+00,
                1.60749250e+00,  1.56233040e+00,  1.51545707e+00,  1.46496011e+00,
                1.40862476e+00,  1.34441016e+00,  1.27480842e+00,  1.19600873e+00,
                1.11222764e+00,  1.02613571e+00,  9.44345682e-01,  8.66692292e-01,
                7.87494873e-01,  7.11183116e-01,  6.38442347e-01,  5.75885062e-01,
                5.19243744e-01,  4.69368629e-01,  4.28402880e-01,  3.96665950e-01,
                3.68825607e-01,  3.44908911e-01,  3.21862795e-01,  2.98967748e-01,
                2.76413872e-01,  2.49845487e-01,  2.18670761e-01,  1.86623538e-01,
                1.57237382e-01,  1.29348700e-01,  1.02360562e-01,  7.45365654e-02,
                4.58600349e-02,  2.27543772e-02,  2.55500371e-03, -1.12901539e-02,
                -2.00612450e-02, -2.05302337e-02, -1.43955736e-02, -1.96362344e-03,
                1.43138815e-02,  3.02983258e-02,  4.34823994e-02,  5.90085959e-02,
                7.54443450e-02,  8.95641407e-02,  9.67971875e-02,  9.70562193e-02,
                9.55311566e-02,  9.15592170e-02,  9.01994445e-02,  9.41480445e-02,
                1.04052993e-01,  1.23400059e-01,  1.48492716e-01,  1.83582911e-01,
                2.26294682e-01,  2.75626392e-01,  3.32998578e-01,  4.00528271e-01,
                4.67934973e-01,  5.32690412e-01,  6.00663214e-01,  6.69237915e-01,
                7.35044063e-01,  7.92103549e-01,  8.40125444e-01,  8.85118228e-01,
                9.26710071e-01,  9.63585313e-01,  9.99416886e-01,  1.03370618e+00,
                1.07058016e+00,  1.10912625e+00,  1.14910105e+00,  1.19208798e+00,
                1.24438037e+00,  1.30576440e+00,  1.37735963e+00,  1.45664249e+00,
                1.54193024e+00,  1.63454464e+00,  1.73351863e+00,  1.83294504e+00,
                1.93370136e+00,  2.03402182e+00,  2.13233740e+00,  2.22839848e+00,
                2.32598392e+00,  2.41995397e+00,  2.51115761e+00,  2.60621539e+00,
                2.70227033e+00,  2.80455557e+00,  2.91302734e+00,  3.02734125e+00,
                3.15428983e+00,  3.28767069e+00,  3.42238855e+00,  3.55859521e+00,
                3.69743476e+00,  3.83768499e+00,  3.97463035e+00,  4.10736914e+00,
                4.24048865e+00,  4.36720598e+00,  4.48434912e+00,  4.59589214e+00,
                4.69648125e+00,  4.78858713e+00,  4.87648490e+00,  4.96283704e+00,
                5.06076331e+00,  5.15916588e+00,  5.26102971e+00,  5.36878962e+00,
                5.49923056e+00,  5.65359704e+00,  5.83209320e+00,  6.03534268e+00,
                6.25949176e+00,  6.49756945e+00,  6.75355801e+00,  7.02179657e+00,
                7.28684141e+00,  7.54783554e+00,  7.78890776e+00,  8.02756007e+00,
                8.24485944e+00,  8.41762883e+00,  8.56386667e+00,  8.70045401e+00,
                8.79791358e+00,  8.86760620e+00,  8.89660240e+00,  8.93821625e+00,
                8.96249649e+00,  9.01373384e+00,  9.03171593e+00,  9.02370461e+00
            };
            var testPer1 = new double[] 
            {
                -0.80020077, -0.20799692,  0.34058598,  0.79812753,  1.11234431,
                1.26627552,  1.24296279,  1.04435011,  0.700797  ,  0.25219845,
                -0.25319069, -0.75880308, -1.21529466, -1.56925367, -1.77808467,
                -1.82163868, -1.69270393, -1.39533391, -0.95647582, -0.42056558,
                0.16110082,  0.73001779,  1.22991918,  1.61448026,  1.84535913,
                1.90093538,  1.77635869,  1.48465622,  1.05407977,  0.5267259 ,
                -0.04708132, -0.61084437, -1.11063844, -1.49620291, -1.73032528,
                -1.79054622, -1.67088411, -1.38391355, -0.95775676, -0.4339794 ,
                0.13813546,  0.70329124,  1.20664338,  1.59845943,  1.83860363,
                1.90318627,  1.78592608,  1.49700388,  1.06300559,  0.52709454,
                -0.06013703, -0.64052324, -1.15758081, -1.56061871, -1.80985932,
                -1.88067834, -1.76510211, -1.47340338, -1.03424322, -0.49027391,
                0.1069    ,  0.69860768,  1.22807982,  1.64300383,  1.90256571,
                1.98059464,  1.8708777 ,  1.58215114,  1.1426878 ,  0.59451356,
                -0.01776822, -0.62619539, -1.17329314, -1.60635986, -1.88378628,
                -1.97765929, -1.88005299, -1.60091012, -1.16577305, -0.61724179,
                -0.00962269,  0.59931731,  1.14897868,  1.58687717,  1.87105722,
                1.97233196,  1.8811974 ,  1.60741854,  1.17791583,  0.6336449 ,
                0.02733875, -0.58286743, -1.13868809, -1.58457863, -1.87713153,
                -1.98777608, -1.90540126, -1.63633736, -1.20731554, -0.65961451,
                -0.04683626,  0.57122652,  1.13417009,  1.58723187,  1.88576591,
                1.99983319,  1.91797844,  1.64736758,  1.21411434,  0.65951476,
                0.03881991, -0.58738637, -1.15717097, -1.61474627, -1.91712043,
                -2.03383836, -1.95332229, -1.68267916, -1.24865504, -0.69220392,
                -0.06875556,  0.56213332,  1.13957463,  1.60784606,  1.9208251 ,
                2.0480416 ,  1.97649791,  1.71478315,  1.28696975,  0.73427712,
                0.11180259, -0.52213658, -1.10131413, -1.56989044, -1.88275321,
                -2.0099503 , -1.93685931, -1.67181406, -1.24014776, -0.68456918,
                -0.06007609,  0.57333983,  1.15222245,  1.61836307,  1.9266784 ,
                2.04795906,  1.96883273,  1.69699325,  1.26139978,  0.70321934,
                0.07603428, -0.55656503, -1.13432999, -1.59903982, -1.90448993,
                -2.02179409, -1.93727912, -1.65913972, -1.21424054, -0.64542751,
                -0.00742886,  0.63697835,  1.2237692 ,  1.69683144,  2.01074899,
                2.1331573 ,  2.05076264,  1.77374564,  1.32586246,  0.75070573,
                0.10436375, -0.55123658, -1.15012341, -1.63600488, -1.96236899,
                -2.09900554, -2.03060716, -1.76373843, -1.32481644, -0.76003157,
                -0.12338318,  0.5217479 ,  1.11522784,  1.59784859,  1.91573826,
                2.0415897 ,  1.96001876,  1.686063  ,  1.24281843,  0.67253564,
                0.03025695, -0.6129921 , -1.19289926, -1.64345407, -1.92236339,
                -1.98832954, -1.84458753, -1.46644984, -0.91565827, -0.24748056
            };
            var testPer2 = new double[] 
            {
                -0.29921287, -0.19345375, -0.09264466,  0.00538367,  0.0907433 ,
                0.16907797,  0.23281512,  0.27667018,  0.30375439,  0.31264929,
                0.30318964,  0.27683397,  0.23051852,  0.1691338 ,  0.09786924,
                0.01567301, -0.07382499, -0.16253747, -0.24723623, -0.32657007,
                -0.39565374, -0.45213328, -0.49290679, -0.51314143, -0.51150822,
                -0.48620446, -0.43678634, -0.3637922 , -0.27010747, -0.15922197,
                -0.03680229,  0.09234732,  0.22141066,  0.34493897,  0.45608176,
                0.54878997,  0.61836517,  0.66011225,  0.67133841,  0.65086931,
                0.60048771,  0.52207301,  0.41941025,  0.2969615 ,  0.15963949,
                0.01425226, -0.13160233, -0.27130424, -0.39893947, -0.50709348,
                -0.591491  , -0.64698753, -0.67141509, -0.66373644, -0.62440791,
                -0.55546672, -0.45986244, -0.34231455, -0.20917714, -0.06712453,
                0.07799069,  0.2187835 ,  0.3494885 ,  0.46369719,  0.55647288,
                0.62355095,  0.66384701,  0.67455501,  0.65630649,  0.61023269,
                0.53794124,  0.44180212,  0.32482084,  0.19242304,  0.05034762,
                -0.0936652 , -0.2344481 , -0.3660088 , -0.4812901 , -0.57572488,
                -0.64618439, -0.68844124, -0.70207956, -0.68591189, -0.64000472,
                -0.5679019 , -0.47237943, -0.35672993, -0.22535624, -0.0844686 ,
                0.05999953,  0.2009793 ,  0.33150171,  0.44722197,  0.5425109 ,
                0.61305958,  0.65569418,  0.66973158,  0.65376309,  0.60885867,
                0.53658775,  0.43994741,  0.3228424 ,  0.19075384,  0.04938327,
                -0.09548885, -0.23760632, -0.37145314, -0.49108451, -0.59206643,
                -0.66878423, -0.71801811, -0.73697631, -0.7245778 , -0.68268287,
                -0.61202535, -0.51581889, -0.39788717, -0.26378834, -0.11818053,
                0.03113469,  0.17851163,  0.31805561,  0.44402114,  0.55053608,
                0.63279649,  0.68684265,  0.71205093,  0.70628458,  0.66945202,
                0.6042364 ,  0.51449515,  0.40259484,  0.27271194,  0.12985762,
                -0.0203204 , -0.16917514, -0.31115496, -0.43933442, -0.5478243 ,
                -0.63234952, -0.68745967, -0.71128844, -0.70371168, -0.66444627,
                -0.59399224, -0.49673506, -0.37714367, -0.2387792 , -0.08907026,
                0.06422394,  0.21577591,  0.35679647,  0.48165979,  0.58521375,
                0.66142591,  0.70818757,  0.72285868,  0.7052971 ,  0.65734491,
                0.58244398,  0.48432965,  0.36716518,  0.23761659,  0.10275575,
                -0.03205726, -0.16190241, -0.27887718, -0.38049791, -0.46243047,
                -0.52139587, -0.55627585, -0.56520695, -0.55042255, -0.51355427,
                -0.45825986, -0.3854348 , -0.29895221, -0.20322302, -0.10530744,
                -0.00819016,  0.08336356,  0.16881468,  0.24426875,  0.30256322,
                0.3450472 ,  0.36713917,  0.37428971,  0.36234736,  0.32987369,
                0.27673663,  0.20708934,  0.11892907,  0.01975251, -0.09113284,
                -0.2043234 , -0.33148174, -0.44418749, -0.56488906, -0.69471798
            };
            //test reconstructed components
            for (int i = 0; i < rTrend.Length; i++)
            {
                Assert.Equal(testTrend[i], rTrend[i],5);
                Assert.Equal(testPer1[i], rPer1[i], 5);
                Assert.Equal(testPer2[i], rPer2[i], 5);
            }


                //PlotMatrix(em).Show();
                ////
                //var plt = new List<XPlot.Plotly.PlotlyChart>();
                //foreach (var m in ssa.EM)
                //    plt.Add(PlotMatrix(m.Value));
                //Chart.ShowAll(plt);


            ///test correlations
            var w = ssa.WCorrelation();
            var df = getwCorrelationMatrix();
            for(int i=0; i < w.GetLength(0); i++)
            {
                for (int j = 0; j < w.GetLength(0); j++)
                {
                    Assert.Equal(df[i, j], Convert.ToSingle(w[i, j]));
                }
            }
        }
        
        
        [Fact(Skip = "Not a unit test")]
        public void SSA_Correlation_Test02()
        {
            //
            var df= getMonthlyDeatsData(); // Monthly time-series data
            //DataFrame.ToCsv("usa_deaths.csv",df);
            var ts = df.ToSeries().Select(x => Convert.ToDouble(x));
            //embed
            var ssa = new SSA(ts);
            //embed
            var x = ssa.Embedding(24);

            PlotMatrix(x).Show();

            //decompose
            ssa.Decompose();
            var plt = new List<XPlot.Plotly.PlotlyChart > ();
            foreach (var tm in ssa.EM)
                plt.Add(PlotMatrix(tm.Value));
            XPlot.Plotly.Chart.ShowAll(plt);

            var f1 = ssa.Reconstruct(1);
            var sct1 = new Graph.Scatter() { name = "Actual", x = Enumerable.Range(1, f1.Length), y = f1, mode = "line", };
            var chart1 = XPlot.Plotly.Chart.Plot<Graph.Trace>(new Graph.Trace[] { sct1 });
            chart1.Show();


            var ff = ssa.Forecast(new int[] {1,2,3,4,5,6,7,8,9,10,11,12,13},6, method:Forecasting.Rforecasing);
            var scatters1 = new Graph.Scatter() { name = "Predicted", x = Enumerable.Range(1, ff.Length), y = ff, mode = "line", };
            var scatters2 = new Graph.Scatter() { name = "Actual", x = Enumerable.Range(1, ts.Count()), y = ts, mode = "line", fillcolor="Blue" };

            var chart = XPlot.Plotly.Chart.Plot<Graph.Trace>(new Graph.Trace[] { scatters1, scatters2 });
           // chart.Show();
           // int i= 0;
           // ssa.PlotComponents(12);

            //
            ssa.PlotSingularValues().Show();

            //
            var charts=ssa.PlotEigenPairs();
            XPlot.Plotly.Chart.ShowAll(charts);

            var w = ssa.WCorrelation(new int[] { 1, 2, 3, 4, 5, 6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24 });
            ssa.PlotwCorrelation(w).Show();

        }

        [Fact]
        public void SSA_Correlation_Test01()
        {
            double[] ts = getAirPassengersData(); // Monthly time-series data

            //embed
            var ssa = new SSA(ts);
            //embed
            var x = ssa.Embedding(12);

            //decompose
            ssa.Decompose();


            var w = ssa.WCorrelation(new int[] {1,2,3,4,5,6,7,8,9,10,11,12 });
           // ssa.PlotwCorrelation(w).Show();
            for (int i = 0; i < w.GetLength(0); i++)
                Assert.True(Math.Round(w[i, i], 5) == 1.0000);

        }

        [Fact]
        public void SSAForecast_Test()
        {
            var _ts = getAirPassengersData(); // Monthly time-series data
            //DataFrame.ToCsv("usa_deaths.csv",df);
            //var ts = df.ToSeries().Select(x => Convert.ToDouble(x));
            //embed
            var ssa = new SSA(_ts);
            //embed
            var x = ssa.Embedding(12);

            //decompose
            ssa.Decompose();

            //
            var charts = ssa.PlotEigenPairs();
            XPlot.Plotly.Chart.ShowAll(charts);


            var ff = ssa.Forecast(new int[] { 1,2, 3, 5, 7 }, 6, method: Forecasting.Rforecasing);
            var scatters1 = new Graph.Scatter() { name = "Predicted", x = Enumerable.Range(1, ff.Length), y = ff, mode = "line", };
            var scatters2 = new Graph.Scatter() { name = "Actual", x = Enumerable.Range(1, _ts.Count()), y = _ts, mode = "line", fillcolor = "Blue" };

            var chart = XPlot.Plotly.Chart.Plot<Graph.Trace>(new Graph.Trace[] { scatters1, scatters2 });
            chart.Show();
        }
    }

}
