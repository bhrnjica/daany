using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.stl;
using Daany.MathExt;

namespace Unit.Test.DF
{
    public class Math_Metrics_Tests
    {
        double[] xActual = new double[] {0.06195,0.37760,0.83368,0.46979,0.90657,0.82522,0.83786,0.67581,0.80171,0.63398,0.87457,0.83748,0.85664,0.48001,0.12338,0.45984,0.75913,0.41532,0.18440,0.72677,0.76270,0.12845,0.02448,0.32324,0.22659,0.76046,0.14442,0.59808,0.99063,0.73614,};
        double[] yPredicted = new double[] {0.79672,0.54635,0.02614,0.72299,0.16721,0.56652,0.64241,0.29990,0.62151,0.37354,0.76671,0.65242,0.72815,0.97263,0.50080,0.05452,0.36795,0.53565,0.68247,0.16595,0.95008,0.69064,0.02562,0.11362,0.70848,0.86734,0.43400,0.89580,0.67682,0.85494};

        [Fact]
        public void Statistics_Metrics_Test()
        {
           var values = AdvancedStatistics.SE(xActual, yPredicted);
            Assert.Equal(4.443698, values.Sum(), 4);

            var value = AdvancedStatistics.MSE(xActual, yPredicted);
            Assert.Equal(0.1481236, value, 4);

            values = AdvancedStatistics.AE(xActual, yPredicted);
            Assert.Equal(9.810468245, values.Sum(), 4);

            value = AdvancedStatistics.MAE(xActual, yPredicted);
            Assert.Equal(0.327015608, value, 4);

            values = AdvancedStatistics.APE(xActual, yPredicted);
            Assert.Equal(36.67386, values.Sum(), 4);

            value = AdvancedStatistics.MAPE(xActual, yPredicted);
            Assert.Equal(1.222462, value, 4);

            value = AdvancedStatistics.SMAPE(xActual, yPredicted);
            Assert.Equal(0.6992009, value, 4);

            // 
            values = AdvancedStatistics.SLE(xActual, yPredicted);
            Assert.Equal(2.104749, values.Sum(), 4);

            value = AdvancedStatistics.MSLE(xActual, yPredicted);
            Assert.Equal(0.0701583, value, 4);

            value = AdvancedStatistics.RMSLE(xActual, yPredicted);
            Assert.Equal(0.2648741, value, 4);

            value = AdvancedStatistics.RSE(xActual, yPredicted);
            Assert.Equal(1.762048, value, 4);

            value = AdvancedStatistics.RRSE(xActual, yPredicted);
            Assert.Equal(1.327422, value, 4);

            value = AdvancedStatistics.RAE(xActual, yPredicted);
            Assert.Equal(1.265375, value, 4);


            value = AdvancedStatistics.MASE(xActual, yPredicted, 1);
            Assert.Equal(1.139131, value, 4);

            value = AdvancedStatistics.MASE(xActual, yPredicted, 3);
            Assert.Equal(1.055599, value, 4);

            value = AdvancedStatistics.MASE(xActual, yPredicted, 5);
            Assert.Equal(0.9490496, value, 4);

            value = AdvancedStatistics.MASE(xActual, yPredicted, 7);
            Assert.Equal(0.8738611, value, 4);

            value = AdvancedStatistics.MASE(xActual, yPredicted, 9);
            Assert.Equal(1.024809, value, 4);


        }

        
    }

}
