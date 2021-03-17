using System;
using Xunit;

namespace MagmaSharp.XUnit
{
    public class EvpTests
    {
        [Fact]
        public void TestFloat()
        {
            float[,] A = new float[,]
            {
             {-1.01f, 0.86f,-4.60f, 3.31f,-4.81f},
             { 3.98f, 0.53f,-7.04f, 5.29f, 3.55f},
             { 3.30f, 8.26f,-3.89f, 8.20f,-1.51f},
             { 4.43f, 4.96f,-7.66f,-7.33f, 6.18f},
             { 7.31f,-6.43f,-6.16f, 2.47f, 5.58f},

            };
           
            float[] result_wr = new float[] {2.86f,2.86f,-0.69f, -0.69f, -10.46f };
            float[] result_wi = new float[] {10.76f, -10.76f, 4.70f, -4.70f, 0 };

            {
                (float[] wr, float[] wi, float[,] VL, float[,] VR) = MagmaSharp.LinAlg.Eigen(A,true, true, Device.CPU);

                for (int i = 0; i < result_wr.Length; i++)
                    Assert.Equal(result_wr[i], wr[i], 2);

                for (int i = 0; i < result_wi.Length; i++)
                    Assert.Equal(result_wi[i], wi[i], 2);

            }


            {
                (float[] wr, float[] wi, float[,] VL, float [,] VR) = MagmaSharp.LinAlg.Eigen(A, true, true, Device.GPU);

                for (int i = 0; i < result_wr.Length; i++)
                    Assert.Equal(result_wr[i], wr[i], 2);

                for (int i = 0; i < result_wi.Length; i++)
                    Assert.Equal(result_wi[i], wi[i], 2);

            }

            {
                (float[] wr, float[] wi, float[,] VL, float[,] VR) = MagmaSharp.LinAlg.Eigen(A,true, true, Device.CUSTOM);

                for (int i = 0; i < result_wr.Length; i++)
                    Assert.Equal(result_wr[i], wr[i], 2);

                for (int i = 0; i < result_wi.Length; i++)
                    Assert.Equal(result_wi[i], wi[i], 2);

            }
        }

        [Fact]
        public void TestDouble()
        {
            double[,] A = new double[,]
            {
             {-1.01f, 0.86f,-4.60f, 3.31f,-4.81f},
             { 3.98f, 0.53f,-7.04f, 5.29f, 3.55f},
             { 3.30f, 8.26f,-3.89f, 8.20f,-1.51f},
             { 4.43f, 4.96f,-7.66f,-7.33f, 6.18f},
             { 7.31f,-6.43f,-6.16f, 2.47f, 5.58f},

            };

            double[] result_wr = new double[] { 2.86f, 2.86f, -0.69f, -0.69f, -10.46f };
            double[] result_wi = new double[] { 10.76f, -10.76f, 4.70f, -4.70f, 0 };

            {
                (double[] wr, double[] wi, double[,] VL, double[,] VR) = MagmaSharp.LinAlg.Eigen(A,true, true, Device.CPU);

                for (int i = 0; i < result_wr.Length; i++)
                    Assert.Equal(result_wr[i], wr[i], 2);

                for (int i = 0; i < result_wi.Length; i++)
                    Assert.Equal(result_wi[i], wi[i], 2);

            }


            {
                (double[] wr, double[] wi, double[,] VL, double[,] VR) = MagmaSharp.LinAlg.Eigen(A, true, true, Device.GPU);

                for (int i = 0; i < result_wr.Length; i++)
                    Assert.Equal(result_wr[i], wr[i], 2);

                for (int i = 0; i < result_wi.Length; i++)
                    Assert.Equal(result_wi[i], wi[i], 2);

            }

            {
                (double[] wr, double[] wi, double[,] VL, double[,] VR) = MagmaSharp.LinAlg.Eigen(A, true, true, Device.CUSTOM);

                for (int i = 0; i < result_wr.Length; i++)
                    Assert.Equal(result_wr[i], wr[i], 2);

                for (int i = 0; i < result_wi.Length; i++)
                    Assert.Equal(result_wi[i], wi[i], 2);

            }
        }
    }
}
