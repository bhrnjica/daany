using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.stl;

namespace Unit.Test.DF
{
    public class LoessInterpolatorTest
    {

        
        [Fact]
        public void constantDataGivesConstantValuesAtNodes()
        {
            double[] data = this.createConstantDataArray();
            for (int degree = 0; (degree < 3); degree++)
            {
                var loessB = new InterpolatorBuilder();
                loessB.Width = 7;
                loessB.Degree = degree;
                var it = loessB.interpolate(data);
               //  
                this.checkFitToData(data, it, 5);
            }

        }

        [Fact]
        public void constantDataExtrapolatesConstantValues()
        {
            double[] data = this.createConstantDataArray();
            for (int degree = 0; (degree < 3); degree++)
            {
                //
                var loessB = new InterpolatorBuilder();
                loessB.Width = 7;
                loessB.Degree = degree;
                var it = loessB.interpolate(data);



                double y = it.smoothOnePoint(-100, 0, (data.Length - 1));
                Assert.True(y!=0);

                Assert.Equal(data[0], y, 5);

                y = it.smoothOnePoint(1000, 0, (data.Length - 1));
                Assert.True(y != 0);
                Assert.Equal(data[0], y, 5);
            }

        }

        [Fact]
        public void constantDataGivesConstantInterpolatedResults()
        {
            double[] data = this.createConstantDataArray();
            for (int degree = 0; (degree < 3); degree++)
            {
                var loessB = new InterpolatorBuilder();
                loessB.Width = 7;
                loessB.Degree = degree;
                var it = loessB.interpolate(data);

                for (int i = 0; (i < 99); i++)
                {
                    double x = (i + 0.5);
                    Double y = it.smoothOnePoint(x, 0, (data.Length - 1));
                    Assert.True(y!=0);
                    Assert.Equal(data[i], y, 5);
                }

            }

        }

        [Fact]
        public void linearDataReturnsDataOnLine()
        {
            double[] data = this.createLinearDataArray();
           // LoessInterpolator loess = (new LoessInterpolator.Builder() + setWidth(5).interpolate(data));
            var loessB = new InterpolatorBuilder();
            loessB.Width = 5;
            var it = loessB.interpolate(data);


            for (int i = 0; (i < data.Length); i++)
            {
                double y = it.smoothOnePoint(i, Math.Max(0, (i - 2)), Math.Min((i + 2), (data.Length - 1)));
                Assert.True(y != 0);
                Assert.Equal(data[i], y, 5);
            }

        }

        [Fact]
        public void linearDataReturnsDataOnLine2()
        {
            double[] data = this.createLinearDataArray();
            //LoessInterpolator.Builder builder = new LoessInterpolator.Builder();
            var loessB = new InterpolatorBuilder();
            
            for (int degree = 1; (degree < 3); degree++)
            {
                //.setWidth(5000).setDegree(degree)
                loessB.Width = 5000;
                loessB.Degree = degree;
                LoessInterpolator loess = loessB.interpolate(data);
                this.checkFitToData(data, loess, 5);
            }

        }

        [Fact]
        public void linearDataExtrapolatesLinearValues()
        {
            double[] data = new double[100];
            for (int i = 0; (i < data.Length); i++)
            {
                data[i] = ((0.25 * i) * -1);
            }

            var builder = new InterpolatorBuilder();

            for (int degree = 1; (degree < 3); degree++)
            {
                builder.Width = 7;
                builder.Degree = degree;
                LoessInterpolator loess = builder.interpolate(data);

                double y = loess.smoothOnePoint(-100, 0, (data.Length - 1));
                Assert.True(y != 0);
                Assert.Equal(((0.25 * -100) * -1), y, 5);

                
                y = loess.smoothOnePoint(1000, 0, (data.Length - 1));
                Assert.True(y != 0);
                Assert.Equal(((0.25 * 1000) * -1), y, 5);
               //
            }

        }

        [Fact]
        public void smoothingWithLargeWidthGivesLinearRegressionFit()
        {
            //  100 point sample of linear data plus noise generated in Python with
            // 
            //  x = np.arange(0, 100)
            //  y = 10.0 * x + 100.0*np.random.randn(100)
            double[] scatter100 = new double[] {
                    45.0641826945,
                    69.6998783993,
                    9.81903951235,
                    -75.4079441854,
                    53.7430205615,
                    12.1359388898,
                    84.972441255,
                    194.467452805,
                    182.276035711,
                    128.161856616,
                    147.021732433,
                    -40.6773185264,
                    41.1575417261,
                    111.04115761,
                    75.0179056538,
                    278.946359666,
                    93.3453251262,
                    103.779785975,
                    252.750915429,
                    252.636103208,
                    457.859165335,
                    143.021758047,
                    79.343240193,
                    280.969547174,
                    35.650257308,
                    157.656673765,
                    29.6984404613,
                    141.980264706,
                    263.465758806,
                    346.309482972,
                    330.044915761,
                    135.019120067,
                    211.801092316,
                    198.186646037,
                    206.088498967,
                    510.89412974,
                    332.076915483,
                    530.524264511,
                    298.21175481,
                    234.317252809,
                    573.836352739,
                    382.708235416,
                    340.090947574,
                    452.475239395,
                    576.134135134,
                    536.703405146,
                    545.033194307,
                    479.525083559,
                    368.551750848,
                    588.429801268,
                    528.672000843,
                    507.301073925,
                    432.749370682,
                    600.239380863,
                    567.328853536,
                    481.544306962,
                    510.42118889,
                    456.519971302,
                    565.839651322,
                    510.505759788,
                    503.2514057,
                    491.279917041,
                    642.319449309,
                    573.019058995,
                    574.709858012,
                    597.316826688,
                    602.361341448,
                    622.312708681,
                    506.669245531,
                    640.120714982,
                    699.793133288,
                    672.969830555,
                    656.645808774,
                    901.375994679,
                    573.903581507,
                    906.472771298,
                    719.604429516,
                    759.262994619,
                    786.970584025,
                    717.422383977,
                    899.007418786,
                    745.516032607,
                    748.049043698,
                    876.99080793,
                    810.985707949,
                    888.762045358,
                    947.030030816,
                    1007.48402395,
                    830.251382179,
                    921.078927761,
                    810.212273661,
                    926.740829016,
                    787.965498372,
                    944.230542154,
                    808.215987256,
                    1044.74526488,
                    866.568085766,
                    1068.6479395,
                    776.566771785,
                    1190.32090194};
            //  Linear fit from Python
            double testSlope = 9.9564197212156671;
            double testIntercept = -12.894457726954045;

            //  Choose a loess width sufficiently large that tri-cube weights for all of the data will be 1.0.
            var builder = new InterpolatorBuilder();
            builder.Width = 1000000;
            var  loess = builder.interpolate(scatter100);
            double x = -5;
            while ((x < 105))
            {
                double y = loess.smoothOnePoint(x, 0, (scatter100.Length - 1));
                Assert.True(y != 0);
                Assert.Equal(((testSlope * x) + testIntercept), y, 5);
                 
                x += 0.5;
            }

        }

        [Fact]
        public void quadraticDataReturnsDataOnParabolaWithQuadraticInterpolation()
        {
            double[] data = this.createQuadraticDataArray();
            var builder = new InterpolatorBuilder();
            builder.Width = 500000;
            builder.Degree = 2;
            var loess = builder.interpolate(data);

            for (int i = -100; i < data.Length + 100; i++)
            {
                double y = loess.smoothOnePoint(i, 0, (data.Length - 1));
                Assert.True(y != 0);
                Assert.Equal(((3.7 - (0.25 * i)) + (0.7 * (i * i))), y, 5);
            }

        }

        [Fact]
        public void quadraticSmoothingWithLargeWidthGivesQuadraticFit()
        {
            //  Half-period of sine plus noise, generated in Python with
            // 
            //  >>> x = np.arange(0, 100)
            //  >>> y = 100 * np.sin(x * np.pi / 100.0)
            //  >>> y = y + 20*np.random.randn(100)
            // 
            //  Quadratic fit:
            // 
            //  >>> np.polyfit(x, y, 2)
            //  array([-0.042576513162, 4.318963328925, -9.80856523083 ])
            double[] data = new double[] {
                    -10.073853166025,
                    -47.578434834077,
                    9.969567309914,
                    13.607475640614,
                    26.336724862687,
                    20.24315196619,
                    8.522203731921,
                    40.879813612701,
                    20.348936031958,
                    34.851420490978,
                    23.004883874872,
                    54.308938782219,
                    15.829781536312,
                    48.719668671254,
                    8.119311766507,
                    1.318458454996,
                    47.063368648646,
                    53.312795063592,
                    83.823883969792,
                    59.110160316898,
                    77.957952679217,
                    27.187112586324,
                    58.265304568637,
                    58.51100724642,
                    66.008865742665,
                    72.672400306629,
                    81.552532336694,
                    49.790263630259,
                    97.490016206155,
                    100.088531750104,
                    67.022085750862,
                    101.72944638112,
                    76.523955444828,
                    109.879122870237,
                    103.156426935471,
                    97.440990018768,
                    96.326853943821,
                    100.002052764625,
                    97.901908920881,
                    81.907764661345,
                    104.608286357414,
                    70.096952411082,
                    87.900737922771,
                    123.466069349253,
                    86.36343272932,
                    96.898061547722,
                    105.2409423246,
                    84.473529980995,
                    87.589406762096,
                    107.145948743204,
                    103.924243272493,
                    86.327435697654,
                    122.078243981121,
                    82.664603304996,
                    90.610134349843,
                    94.333055790992,
                    130.280210790056,
                    106.70486524105,
                    76.506903917192,
                    81.412062643472,
                    93.910953769154,
                    106.832729589699,
                    115.642049987031,
                    84.975670522389,
                    97.761576968675,
                    111.855362368863,
                    72.717525044868,
                    81.957250239574,
                    61.808571079313,
                    70.85792217601,
                    40.898527454521,
                    97.782149960766,
                    97.913155063949,
                    101.714088071105,
                    86.227528826015,
                    67.255531559075,
                    80.13052355131,
                    74.988502831106,
                    96.560985475347,
                    65.285104731415,
                    62.127365337288,
                    28.616465130641,
                    82.768020843782,
                    52.291991098773,
                    64.194294668567,
                    38.225290216514,
                    20.662635351816,
                    26.091102513734,
                    24.5632772509,
                    23.281240785751,
                    23.800117109909,
                    52.816749904647,
                    33.332347686135,
                    28.2914005902,
                    14.683404049683,
                    53.212854193497,
                    1.829566520138,
                    18.404833513506,
                    -9.019769796879,
                    9.006983482915};

            var builder = new InterpolatorBuilder();
            builder.Width = 500000;
            builder.Degree = 2;
            var loess = builder.interpolate(data);

            for (int i = 0; (i < data.Length); i++)
            {
                Double y = loess.smoothOnePoint(i, 0, (data.Length - 1));
                Assert.True(y != 0);
                double y0 = (((0.042576513162
                            * (i * i))
                            * -1)
                            + ((4.318963328925 * i)
                            - 9.80856523083));
                Assert.Equal(y0, y, 5);
            }

        }
        //exception degree
        [Fact]
        
        public void degreeCheck1()
        {
            var builder = new InterpolatorBuilder();
            builder.Width = 37;
            builder.Degree = -1;
             
            var exception = Assert.Throws<Exception>(() => builder.interpolate(this.createLinearDataArray()));
            Assert.Equal("Degree must be 0, 1 or 2", exception.Message);

        }
        //exception degree
        [Fact]
        public void degreeCheck2()
        {
            var builder = new InterpolatorBuilder();
            builder.Width = 37;
            builder.Degree = 3;
           // var loess = builder.interpolate(this.createLinearDataArray());

            var exception = Assert.Throws<Exception>(() => builder.interpolate(this.createLinearDataArray()));
            Assert.Equal("Degree must be 0, 1 or 2", exception.Message);

        }
        //exception width
        [Fact]
        public void widthMustBeSet()
        {
            var builder = new InterpolatorBuilder();
            var exception = Assert.Throws<Exception>(() => builder.interpolate(new double[2000]));
            Assert.Equal("LoessInterpolator.Builder: Width must be set", exception.Message);

        }

        [Fact]
        public void dataMustBeNonNull()
        {
            var builder = new InterpolatorBuilder();
            
           // var loess = builder.interpolate(null);
            var exception = Assert.Throws<Exception>(() => builder.interpolate(null));
            Assert.Equal("LoessInterpolator.Builder: Width must be set", exception.Message);

        }

        //  Utility functions...
        private void checkFitToData(double[] data, LoessInterpolator loess, int precision)
        {
            for (int i = 0; (i < data.Length); i++)
            {
                Double y = loess.smoothOnePoint(i, 0, (data.Length - 1));
                Assert.True(y!=0);
                Assert.Equal(data[i], y, precision);
                //ssertEquals(String.format("Bad value at %d", i), data[i], y, eps);
            }

        }

        private double[] createConstantDataArray()
        {
            double[] data = new double[100];
            for (int i = 0; (i < 100); i++)
            {
                data[i] = 2016;
            }

            return data;
        }

        private double[] createLinearDataArray()
        {
            double[] data = new double[100];
            for (int i = 0; (i < data.Length); i++)
            {
                data[i] = (3.7 - (0.25 * i));
            }

            return data;
        }

        private double[] createQuadraticDataArray()
        {
            double[] data = new double[100];
            for (int i = 0; (i < data.Length); i++)
            {
                data[i] = ((3.7 - (0.25 * i)) + (0.7
                            * (i * i)));
            }

            return data;
        }
    }

}
