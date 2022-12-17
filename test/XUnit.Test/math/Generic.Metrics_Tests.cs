using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Stat.stl;
using Daany.MathStuff;

namespace Unit.Test.Math.Stats;


#if NET7_0_OR_GREATER

public class Metrics_Test
{
    double[] xActual = new double[] { 0.06195, 0.37760, 0.83368, 0.46979, 0.90657, 0.82522, 0.83786, 0.67581, 0.80171, 0.63398, 0.87457, 0.83748, 0.85664, 0.48001, 0.12338, 0.45984, 0.75913, 0.41532, 0.18440, 0.72677, 0.76270, 0.12845, 0.02448, 0.32324, 0.22659, 0.76046, 0.14442, 0.59808, 0.99063, 0.73614, };
    double[] yPredicted = new double[] { 0.79672, 0.54635, 0.02614, 0.72299, 0.16721, 0.56652, 0.64241, 0.29990, 0.62151, 0.37354, 0.76671, 0.65242, 0.72815, 0.97263, 0.50080, 0.05452, 0.36795, 0.53565, 0.68247, 0.16595, 0.95008, 0.69064, 0.02562, 0.11362, 0.70848, 0.86734, 0.43400, 0.89580, 0.67682, 0.85494 };
    float[] array = new float[10] { 0.4f, 0.5f, 0.45f, 0.5f, 0.45f, 0.4f, 0.5f, 0.45f, 0.4f, 0.5f };


    [Fact]
    public void Statistics_Mode_Test()
    {

        var retVal = Metrics.Mode<float>(array.ToList());

        Assert.Equal<float>(0.5f, retVal);  
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

}
#endif
