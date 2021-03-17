using System;
using Xunit;

namespace MagmaSharp.XUnit
{
    public class Lssests
    {
        [Fact]
        public void TestFloat()
        {
            /* Local arrays */
            float[,] A = new float[,]
            {
                {  1.44f, -7.84f,  -4.39f,  4.53f},
                { -9.96f,-0.28f , -3.24f ,  3.83f},
                { -7.55f, 3.24f ,  6.27f , -6.64f},
                {  8.34f, 8.09f ,  5.28f ,  2.06f},
                {  7.08f, 2.52f ,  0.74f , -2.47f},
                { -5.45f,-5.70f , -1.19f ,  4.70f},
            };

            /*  */
            float[,] B = new float[,]
            {
                { 8.58f,   9.35f} ,
                { 8.26f,  -4.43f} ,
                { 8.48f,  -0.70f} ,
                {-5.28f,  -0.26f} ,
                { 5.72f,  -7.36f} ,
                { 8.93f,  -2.52f },
            };

            float[,] result = new float[,]
            {
                { -0.45f,   0.25f} ,
                { -0.85f, -0.90f} ,
                {  0.71f,  0.63f} ,
                {  0.13f,  0.14f} ,
            };

            var X = MagmaSharp.LinAlg.Lss(A, B, Device.CPU);

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Assert.Equal(X[i, j], result[i, j], 2);
                }
            }

            var X1 = MagmaSharp.LinAlg.Lss(A, B, Device.GPU);

            for (int i = 0; i < X1.GetLength(0); i++)
            {
                for (int j = 0; j < X1.GetLength(1); j++)
                {
                    Assert.Equal(X1[i, j], result[i, j], 2);
                }
            }

            var X2 = MagmaSharp.LinAlg.Lss(A, B, Device.CUSTOM);

            for (int i = 0; i < X2.GetLength(0); i++)
            {
                for (int j = 0; j < X2.GetLength(1); j++)
                {
                    Assert.Equal(X2[i, j], result[i, j], 2);
                }
            }

        }


        [Fact]
        public void TestDouble()
        {
            /* Local arrays */
            double[,] A = new double[,]
            {
                {  1.44, -7.84, -4.39,   4.53},
                { -9.96,-0.28 , -3.24 ,  3.83},
                { -7.55, 3.24 ,  6.27 , -6.64},
                {  8.34, 8.09 ,  5.28 ,  2.06},
                {  7.08, 2.52 ,  0.74 , -2.47},
                { -5.45,-5.70 , -1.19 ,  4.70},
            };

            /*  */
            double[,] B = new double[,]
            {
                { 8.58,   9.35} ,
                { 8.26,  -4.43} ,
                { 8.48,  -0.70} ,
                {-5.28,  -0.26} ,
                { 5.72,  -7.36} ,
                { 8.93,  -2.52 },
            };

            double[,] result = new double[,]
            {
                { -0.45,  0.25} ,
                { -0.85, -0.90} ,
                {  0.71,  0.63} ,
                {  0.13,  0.14} ,
            };

            var X = MagmaSharp.LinAlg.Lss(A, B, Device.CPU);

            for (int i = 0; i < X.GetLength(0); i++)
            {
                for (int j = 0; j < X.GetLength(1); j++)
                {
                    Assert.Equal(X[i, j], result[i, j], 2);
                }
            }

            var X1 = MagmaSharp.LinAlg.Lss(A, B, Device.GPU);

            for (int i = 0; i < X1.GetLength(0); i++)
            {
                for (int j = 0; j < X1.GetLength(1); j++)
                {
                    Assert.Equal(X1[i, j], result[i, j], 2);
                }
            }

            //var X2 = MagmaSharp.LinAlg.Lss(A, B, Device.CUSTOM);

            //for (int i = 0; i < X2.GetLength(0); i++)
            //{
            //    for (int j = 0; j < X2.GetLength(1); j++)
            //    {
            //        Assert.Equal(X2[i, j], result[i, j], 2);
            //    }
            //}

        }

        [Fact]
        public void SimpleRegression_Test()
        {
            //Solving simple linear regression od type
            // Yhat=b0+b1X
            /* Local arrays */
            double[,] A = new double[,]
            {   //b0    b1
                {1.0f, 1.0f},
                {1.0f, 2.0f},
                {1.0f, 3.0f},
                {1.0f, 4.0f},
            };

            /*  */
            double[,] B = new double[,]
            {   //Y
                { 6.0} ,
                { 5.0} ,
                { 7.0} ,
                {10.0} ,

            };

            var X = MagmaSharp.LinAlg.Lss(A, B, Device.CPU);

            Assert.Equal(3.5, Math.Round(X[0,0],2));
            Assert.Equal(1.4, Math.Round(X[1, 0],2));
        }
    }
}
