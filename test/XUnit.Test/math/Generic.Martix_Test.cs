using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Stat.stl;
using Daany.MathStuff;
using Daany.Stat;
using Daany.Ext;
using XPlot.Plotly;

namespace Unit.Test.Math.Stats;


#if NET7_0_OR_GREATER

public class Generic_Matrix_Test
{
    double[] v1 = new double[3] { 1, 2, -4 };

    double[,] m1 = new double[3, 3]
                               { {  1,  2, -1 },
                                 {  2, -2,  4 },
                                 {  2 ,  9,-4 }
                               };
    double[,] simpleMatrix2 = new double[3, 3]
                               { {  1,  2, -1 },
                                 {  2, -2,  4 },
                                 {  2 ,  9,-4 }
                               };

    [Fact]
    public void Test_GetColumn()
    { 
        var result = Matrix.GetColumn< double > (m1, 0);
        var expected = new double[] { 1, 2, 2 };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_GetRow()
    {
        var result = Matrix.GetRow<double>(m1, 0);
        var expected = new double[] { 1, 2, -1 };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_MainDiagonal()
    {
        var result = Matrix.GetDiagonal<double>(m1);

        var expected = new double[] { 1, -2, -4 };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Reverse_ByRow()
    {
        var result = Matrix.Reverse<double>( m1, true );

        var expected = new double[3, 3]
                                { {  2 ,  9,-4 },
                                 {  2, -2,  4 },
                                  {  1,  2, -1 }
                               };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Reverse_ByColumns()
    {
        var result = Matrix.Reverse<double>(m1, false);

        var expected = new double[3, 3]
                                { {  -1 , 2, 1 },
                                  {  4,  -2, 2 },
                                  {  -4,  9, 2 }
                               };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Transponse_Matrix()
    {
        var result = Matrix.Transpose<double>(m1, false);

        var expected = new double[3, 3]
                                { {  1 , 2, 2 },
                                  {  2, -2, 9 },
                                  { -1,  4, -4 }
                               };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Transponse_Vector()
    {
        var result = Matrix.Transpose<double>( v1 );

        var expected = new double[3, 1]
                                { {  1  },
                                  {  2  },
                                  { -4  }
                               };

        Assert.Equal(expected, result);
    }


}

#endif
