using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using System.Numerics;
using Daany.MathStuff.MatrixGeneric;

namespace Daany.MathStuff.Tests;

public class Generic_Matrix_Test
{
    double[] v1 = new double[3] { 1, 2, -4 };

    double[,] m1 = new double[3, 3]
                            { {  1,  2, -1 },
                              {  2, -2,  4 },
                              {  2 ,  9,-4 }
                            };
    decimal[,] m1d = new decimal[3, 3]
                           { {  1,  2, -1 },
                             {  2, -2,  4 },
                             {  2 ,  9,-4 }
                           };

    double[][] m2 = null;
    decimal[][] m2d = null;

    public Generic_Matrix_Test()
    {
        m2 = new double[3][];
        m2[0] = new double[3] { 1, 2, -1 };
        m2[1] = new double[3] { 2, -2, 4 };
        m2[2] = new double[3] { 2, 9, -4 };

        m2d = new decimal[3][];
        m2d[0] = new decimal[3] { 1, 2, -1 };
        m2d[1] = new decimal[3] { 2, -2, 4 };
        m2d[2] = new decimal[3] { 2, 9, -4 };

    }
    

    [Fact]
    public void Test_GetColumn()
    { 
        var result = m1.GetColumn< double > (0);
        var expected = new double[] { 1, 2, 2 };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_GetColumn1()
    {
        var result = m2.GetColumn<double>(0);
        var expected = new double[] { 1, 2, 2 };

        Assert.Equal(expected, result);
    }


    [Fact]
    public void Test_GetRow()
    {
        var result = m1.GetRow<double>( 0 );
        var expected = new double[] { 1, 2, -1 };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_GetRow1()
    {
        var result = m2.GetRow<double>(0);
        var expected = new double[] { 1, 2, -1 };

        Assert.Equal(expected, result);
    }


    [Fact]
    public void Test_MainDiagonal()
    {
        var result = m1.GetDiagonal();

        var expected = new double[] { 1, -2, -4 };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_MainDiagonal1()
    {
        var result = m2.GetDiagonal();

        var expected = new double[] { 1, -2, -4 };

        Assert.Equal(expected, result);
    }



    [Fact]
    public void Test_Reverse_ByRow()
    {
        var result = m2.Reverse<double>( true );

        var expected = new double[3][];
        expected[0] = new double[3] { 2, 9, -4 };
        expected[1] = new double[3] { 2, -2, 4 };
        expected[2] = new double[3] { 1, 2, -1 };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Reverse_ByRow1()
    {
        var result = m1.Reverse(true);

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
        var result = m1.Reverse(false);

        var expected = new double[3, 3]
                                { {  -1 , 2, 1 },
                                  {  4,  -2, 2 },
                                  {  -4,  9, 2 }
                               };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Reverse_ByColumns1()
    {
        var result = m2.Reverse(false);

        var expected = new double[3][];
        expected[0] = new double[3] { -1, 2, 1 };
        expected[1] = new double[3] { 4, -2, 2 };
        expected[2] = new double[3] { -4, 9, 2 };

        Assert.Equal(expected, result);
    }



    [Fact]
    public void Test_Transponse_Matrix()
    {
        var result = m1.Transpose( );

        var expected = new double[3, 3]
                                { {  1 , 2, 2 },
                                  {  2, -2, 9 },
                                  { -1,  4, -4 }
                               };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Transponse_Matrix1()
    {
        var result = m2.Transpose();

        var expected = new double[3][];
        expected[0] = new double[3] { 1, 2, 2 };
        expected[1] = new double[3] { 2, -2, 9 };
        expected[2] = new double[3] { -1, 4, -4 };


        Assert.Equal(expected, result);
    }



    [Fact]
    public void Test_Transponse_Vector()
    {
        var result = v1.Transpose( );

        var expected = new double[3, 1]
                                { {  1  },
                                  {  2  },
                                  { -4  }
                               };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_LU_Matrix()
    {

       var result =  m1d.MakeLU<decimal,decimal>();

        var L = new decimal[3, 3]
                                { {  1 , 0, 0 },
                                  {  1, 1, 0 },
                                  {0.5m,  0.2727272727272727272727272727M, 1 }
                               };

        var U = new decimal[3, 3]
                                { {  2 , -2, 4 },
                                  {  0, 11, -8},
                                  { 0,  0.0000000000000000000000000003M,-0.8181818181818181818181818184M }
                               };

        Assert.Equal(L, result.Item1);

        Assert.Equal(U, result.Item2);


    }

}
