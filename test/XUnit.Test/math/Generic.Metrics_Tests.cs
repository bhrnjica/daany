using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Stat.stl;
using Daany.MathStuff.Stats;
using Daany.MathStuff;

namespace Unit.Test.Math.Stats;


#if NET7_0_OR_GREATER

public class Metrics_Test
{
    decimal[] xActuald = new decimal[] { 0.06195m, 0.37760m, 0.83368m, 0.46979m, 0.90657m, 0.82522m, 0.83786m, 0.67581m, 0.80171m, 0.63398m };
    double[] xActual = new double[] { 0.06195, 0.37760, 0.83368, 0.46979, 0.90657, 0.82522, 0.83786, 0.67581, 0.80171, 0.63398, 0.87457, 0.83748, 0.85664, 0.48001, 0.12338, 0.45984, 0.75913, 0.41532, 0.18440, 0.72677, 0.76270, 0.12845, 0.02448, 0.32324, 0.22659, 0.76046, 0.14442, 0.59808, 0.99063, 0.73614, };
    double[] yPredicted = new double[] { 0.79672, 0.54635, 0.02614, 0.72299, 0.16721, 0.56652, 0.64241, 0.29990, 0.62151, 0.37354, 0.76671, 0.65242, 0.72815, 0.97263, 0.50080, 0.05452, 0.36795, 0.53565, 0.68247, 0.16595, 0.95008, 0.69064, 0.02562, 0.11362, 0.70848, 0.86734, 0.43400, 0.89580, 0.67682, 0.85494 };
    float[] array = new float[10] { 0.4f, 0.5f, 0.45f, 0.5f, 0.45f, 0.4f, 0.5f, 0.45f, 0.4f, 0.5f };


    [Fact]
    public void Statistics_Mode_Test()
    {

        var retVal = Metrics.Mode<double>(xActual.ToList());

        Assert.Equal<double>(0.06195, retVal);  
    }

    [Fact]
    public void Statistics_Random_Test()
    {

        var retVal = Metrics.Random<float>(array.ToList());

        Assert.Equal<float>(0.45f, retVal);
    }

    [Fact]
    public void Statistics_Frequency_Test()
    {

        var retVal = Metrics.Frequency <float> (array.ToList());

        var expected = new List<(float, int )> () { (0.5f, 4), (0.4f, 3), (0.45f,3) };

        Assert.Equal<(float value, int count)>(expected, retVal );
    }

    [Fact]
    public void Statistics_Median_Test()
    {

        var retVal = Metrics.Median< double, double >(xActual.ToList());

        var expected = 0.654895;

        Assert.Equal< double >(expected, retVal);
    }

    //double[] xActual = new double[] { 0.06195, 0.37760, 0.83368, 0.46979, 0.90657, 0.82522, 0.83786, 0.67581, 0.80171, 0.63398, 0.87457, 0.83748, 0.85664, 0.48001, 0.12338, 0.45984, 0.75913, 0.41532, 0.18440, 0.72677, 0.76270, 0.12845, 0.02448, 0.32324, 0.22659, 0.76046, 0.14442, 0.59808, 0.99063, 0.73614, };
    //double[] yPredicted = new double[] { 0.79672, 0.54635, 0.02614, 0.72299, 0.16721, 0.56652, 0.64241, 0.29990, 0.62151, 0.37354, 0.76671, 0.65242, 0.72815, 0.97263, 0.50080, 0.05452, 0.36795, 0.53565, 0.68247, 0.16595, 0.95008, 0.69064, 0.02562, 0.11362, 0.70848, 0.86734, 0.43400, 0.89580, 0.67682, 0.85494 };

    [Fact]
    public void Statistics_Metrics_Test()
    {
        var values = Metrics.SE<double, double>(xActual, yPredicted);
        Assert.Equal(4.443698, values.Sum(), 4);

        var value = Metrics.MSE<double, double>(xActual, yPredicted);
        Assert.Equal(0.1481236, value, 4);

        values = Metrics.AE<double, double>(xActual, yPredicted);
        Assert.Equal(9.810468245, values.Sum(), 4);

        value = Metrics.MAE<double, double>(xActual, yPredicted);
        Assert.Equal(0.327015608, value, 4);

        values = Metrics.APE<double, double>(xActual, yPredicted);
        Assert.Equal(/*36.67386*/59.896, values.Sum(), 4);

        value = Metrics.MAPE<double, double>(xActual, yPredicted);
        Assert.Equal(1.222462, value, 4);

        //value = Metrics.SMAPE<double, double>(xActual, yPredicted);
        //Assert.Equal(0.6992009, value, 4);

        // 
        //values = Metrics.SLE<double, double>(xActual, yPredicted);
        //Assert.Equal(2.104749, values.Sum(), 4);

        //value = Metrics.MSLE<double, double>(xActual, yPredicted);
        //Assert.Equal(0.0701583, value, 4);

        //value = Metrics.RMSLE<double, double>(xActual, yPredicted);
        //Assert.Equal(0.2648741, value, 4);

        //value = Metrics.RSE<double, double>(xActual, yPredicted);
        //Assert.Equal(1.762048, value, 4);

        //value = Metrics.RRSE<double, double>(xActual, yPredicted);
        //Assert.Equal(1.327422, value, 4);

        //value = Metrics.RAE<double, double>(xActual, yPredicted);
        //Assert.Equal(1.265375, value, 4);


        //value = Metrics.MASE<double, double>(xActual, yPredicted, 1);
        //Assert.Equal(1.139131, value, 4);

        //value = Metrics.MASE<double, double>(xActual, yPredicted, 3);
        //Assert.Equal(1.055599, value, 4);

        //value = Metrics.MASE<double, double>(xActual, yPredicted, 5);
        //Assert.Equal(0.9490496, value, 4);

        //value = Metrics.MASE<double, double>(xActual, yPredicted, 7);
        //Assert.Equal(0.8738611, value, 4);

        //value = Metrics.MASE<double, double>(xActual, yPredicted, 9);
        //Assert.Equal(1.024809, value, 4);


    }

    [Fact]
    public void Statistics_MinMax_Test()
    {
        var data = new double[10][];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new double[3];
            data[i][0] = i + 5;
            data[i][1] = System.Math.Sin(i * 2.14);
            data[i][2] = System.Math.Sin(2 * i + i * i - 5);
        }
        var minMax = data.calculateMinMax();//BasicStatistics.calculateMinMax(data);

        Assert.Equal(5, minMax.min[0]);
        Assert.Equal(-0.98742414298941528, minMax.min[1]);
        Assert.Equal(-0.98803162409286183, minMax.min[2]);

        Assert.Equal(14, minMax.max[0]);
        Assert.Equal(0.84233043163664567, minMax.max[1]);
        Assert.Equal(0.99287264808453712, minMax.max[2]);
    }
    [Fact]
    public void Statistics_MeanStDev_Test()
    {
        var data = new double[10][];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new double[3];
            data[i][0] = i + 5;
            data[i][1] = System.Math.Sin(i * 2.14);
            data[i][2] = System.Math.Sin(2 * i + i * i - 5);
        }

        var minMax = data.calculateMeanStDev();//BasicStatistics.calculateMeanStDev(data);

        Assert.Equal(9.5, minMax.means[0]);
        Assert.Equal(0.02222381660881375, minMax.means[1]);
        Assert.Equal(-0.16633643848431015, minMax.means[2]);

        Assert.Equal(3.0276503540974917, minMax.stdevs[0]);
        Assert.Equal(0.72201262859409132, minMax.stdevs[1]);
        Assert.Equal(0.72052383815693033, minMax.stdevs[2]);
    }


    [Fact]
    public void Statistics_Percentiles_Test()
    {

        var data = new double[] { 2, 3, 4, 5, 6, 8, 7, 54, 3, 23, 23, 5, 6, 8, 8, 99, 5, 5, 5 };
        var val1 = data.Percentile(0);
        var val2 = data.Percentile(25);
        var val3 = data.Percentile(50);
        var val4 = data.Percentile(75);
        var val5 = data.Percentile(100);
        var median = data.MedianOf();
        var min = data.Min();
        var max = data.Max();
        Assert.Equal(min, val1);
        Assert.Equal(5, val2);
        Assert.Equal(median, val3);
        Assert.Equal(8, val4);
        Assert.Equal(max, val5);
    }


}
#endif
