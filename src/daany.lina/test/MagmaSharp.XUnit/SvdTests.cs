using System;
using Xunit;

namespace MagmaSharp.XUnit
{
    public class SvdTests
    {
        [Fact]
        public void TestFloat_RowMajor()
        {

            //result for s
            float[] result_s = new float[] {27.47f, 22.64f, 8.56f, 5.99f, 2.01f };

            //
            float[,] resultU = new float[,]
            {
                {-0.59f,  0.26f,   0.36f,   0.31f,   0.23f, 0.55F},
                {-0.40f,  0.24f,  -0.22f,  -0.75f,  -0.36f, 0.18F},
                {-0.03f, -0.60f,  -0.45f,   0.23f,  -0.31f, 0.54F},
                {-0.43f,  0.24f,  -0.69f,   0.33f,   0.16f, -0.39F},
                {-0.47f, -0.35f,   0.39f,   0.16f,  -0.52f, -0.46F},
                { 0.29f,  0.58f,  -0.02f,   0.38f,  -0.65f, 0.11F},
            };

            //
            float[,] resultvT = new float[,]
            {
                {-0.25f,  -0.40F,  -0.69F,  -0.37F,  -0.41F},
                { 0.81f,   0.36F,  -0.25F,  -0.37F,  -0.10F},
                {-0.26f,   0.70F,  -0.22F,   0.39F,  -0.49F},
                { 0.40f,  -0.45F,   0.25F,   0.43F,  -0.62F},
                {-0.22f,   0.14F,   0.59F,  -0.63F,  -0.44F},
            };


            {
                //
                float[,] A = new float[,]
                {
                    { 8.79f,  9.93f,   9.83f,   5.45f,   3.16f, },
                    {6.11f,   6.91f,   5.04f,  -0.27f,   7.98f, },
                    {-9.15f,  -7.93f,   4.86f,   4.85f,   3.01f, },
                    {9.57f,   1.64f,   8.83f,   0.74f,   5.80f, },
                    {-3.49f,   4.02f,   9.80f,  10.00f,   4.27f, },
                    { 9.84f,   0.15f,  -8.99f,  -6.02f,  -5.31f, },
                };
                float[,] A1 = new float[,]
                {
                    { 8.79f,  9.93f,   9.83f,   5.45f,   3.16f, },
                    {6.11f,   6.91f,   5.04f,  -0.27f,   7.98f, },
                    {-9.15f,  -7.93f,   4.86f,   4.85f,   3.01f, },
                    {9.57f,   1.64f,   8.83f,   0.74f,   5.80f, },
                    {-3.49f,   4.02f,   9.80f,  10.00f,   4.27f, },
                    { 9.84f,   0.15f,  -8.99f,  -6.02f,  -5.31f, },
                };

                (float[] s, float[,] U, float[,] vT) = MagmaSharp.LinAlg.Svd(A, true, true, device: Device.GPU);


                Assert.Equal(A, A1);

                for (int i = 0; i < result_s.Length; i++)
                    Assert.Equal(result_s[i], s[i], 2);

                
                for (int i = 0; i < U.GetLength(0); i++)
                {
                    for (int j = 0; j < U.GetLength(1); j++)
                    {
                        Assert.Equal(U[i, j], resultU[i, j], 2);
                    }
                }

                for (int i = 0; i < vT.GetLength(0); i++)
                {
                    for (int j = 0; j < vT.GetLength(1); j++)
                    {
                        Assert.Equal(vT[i, j], resultvT[i, j], 2);
                    }
                }
            }

            {
                //
                float[,] A = new float[,]
                {
                { 8.79f,  9.93f,   9.83f,   5.45f,   3.16f, },
                {6.11f,   6.91f,   5.04f,  -0.27f,   7.98f, },
                {-9.15f,  -7.93f,   4.86f,   4.85f,   3.01f, },
                {9.57f,   1.64f,   8.83f,   0.74f,   5.80f, },
                {-3.49f,   4.02f,   9.80f,  10.00f,   4.27f, },
                { 9.84f,   0.15f,  -8.99f,  -6.02f,  -5.31f, },
                };

                (float[] s, float[,] U, float[,] vT) = MagmaSharp.LinAlg.Svd(A, true, true, device: Device.CPU);

                for (int i = 0; i < result_s.Length; i++)
                    Assert.Equal(result_s[i], s[i], 2);


                for (int i = 0; i < U.GetLength(0); i++)
                {
                    for (int j = 0; j < U.GetLength(1); j++)
                    {
                        Assert.Equal(U[i, j], resultU[i, j], 2);
                    }
                }

                for (int i = 0; i < vT.GetLength(0); i++)
                {
                    for (int j = 0; j < vT.GetLength(1); j++)
                    {
                        Assert.Equal(vT[i, j], resultvT[i, j], 2);
                    }
                }
            }

            {
                //
                float[,] A = new float[,]
                {
                { 8.79f,  9.93f,   9.83f,   5.45f,   3.16f, },
                {6.11f,   6.91f,   5.04f,  -0.27f,   7.98f, },
                {-9.15f,  -7.93f,   4.86f,   4.85f,   3.01f, },
                {9.57f,   1.64f,   8.83f,   0.74f,   5.80f, },
                {-3.49f,   4.02f,   9.80f,  10.00f,   4.27f, },
                { 9.84f,   0.15f,  -8.99f,  -6.02f,  -5.31f, },
                };

                (float[] s, float[,] U, float[,] vT) = MagmaSharp.LinAlg.Svd(A, true, true, device: Device.CUSTOM);

                for (int i = 0; i < result_s.Length; i++)
                    Assert.Equal(result_s[i], s[i], 2);


                for (int i = 0; i < U.GetLength(0); i++)
                {
                    for (int j = 0; j < U.GetLength(1); j++)
                    {
                        Assert.Equal(U[i, j], resultU[i, j], 2);
                    }
                }

                for (int i = 0; i < vT.GetLength(0); i++)
                {
                    for (int j = 0; j < vT.GetLength(1); j++)
                    {
                        Assert.Equal(vT[i, j], resultvT[i, j], 2);
                    }
                }
            }

        }

   
        [Fact]
        public void TestDouble_RowMajor()
        {
            //result for s
            double[] result_s = new double[] { 27.47f, 22.64f, 8.56f, 5.99f, 2.01f };

            //
            double[,] resultU = new double[,]
            {
               {-0.59f,  0.26f,   0.36f,   0.31f,   0.23f, 0.55F},
                {-0.40f,  0.24f,  -0.22f,  -0.75f,  -0.36f, 0.18F},
                {-0.03f, -0.60f,  -0.45f,   0.23f,  -0.31f, 0.54F},
                {-0.43f,  0.24f,  -0.69f,   0.33f,   0.16f, -0.39F},
                {-0.47f, -0.35f,   0.39f,   0.16f,  -0.52f, -0.46F},
                { 0.29f,  0.58f,  -0.02f,   0.38f,  -0.65f, 0.11F},
            };

            // 
            double[,] resultvT = new double[,]
            {
                 {-0.25f,  -0.40F,  -0.69F,  -0.37F,  -0.41F},
                { 0.81f,   0.36F,  -0.25F,  -0.37F,  -0.10F},
                {-0.26f,   0.70F,  -0.22F,   0.39F,  -0.49F},
                { 0.40f,  -0.45F,   0.25F,   0.43F,  -0.62F},
                {-0.22f,   0.14F,   0.59F,  -0.63F,  -0.44F},
            };


            {
                //
                double[,] A = new double[,]
                {
                    { 8.79f,  9.93f,   9.83f,   5.45f,   3.16f, },
                    {6.11f,   6.91f,   5.04f,  -0.27f,   7.98f, },
                    {-9.15f,  -7.93f,   4.86f,   4.85f,   3.01f, },
                    {9.57f,   1.64f,   8.83f,   0.74f,   5.80f, },
                    {-3.49f,   4.02f,   9.80f,  10.00f,   4.27f, },
                    { 9.84f,   0.15f,  -8.99f,  -6.02f,  -5.31f, },
                };

                (double[] s, double[,] U, double[,] vT) = MagmaSharp.LinAlg.Svd(A, true, true, device: Device.GPU);

                for (int i = 0; i < result_s.Length; i++)
                    Assert.Equal(result_s[i], s[i], 2);


                for (int i = 0; i < U.GetLength(0); i++)
                {
                    for (int j = 0; j < U.GetLength(1); j++)
                    {
                        Assert.Equal(U[i, j], resultU[i, j], 2);
                    }
                }

                for (int i = 0; i < vT.GetLength(0); i++)
                {
                    for (int j = 0; j < vT.GetLength(1); j++)
                    {
                        Assert.Equal(vT[i, j], resultvT[i, j], 2);
                    }
                }
            }

            {
                //
                double[,] A = new double[,]
                {
                { 8.79f,  9.93f,   9.83f,   5.45f,   3.16f, },
                {6.11f,   6.91f,   5.04f,  -0.27f,   7.98f, },
                {-9.15f,  -7.93f,   4.86f,   4.85f,   3.01f, },
                {9.57f,   1.64f,   8.83f,   0.74f,   5.80f, },
                {-3.49f,   4.02f,   9.80f,  10.00f,   4.27f, },
                { 9.84f,   0.15f,  -8.99f,  -6.02f,  -5.31f, },
                };

                (double[] s, double[,] U, double[,] vT) = MagmaSharp.LinAlg.Svd(A, true, true, device: Device.CPU);

                for (int i = 0; i < result_s.Length; i++)
                    Assert.Equal(result_s[i], s[i], 2);


                for (int i = 0; i < U.GetLength(0); i++)
                {
                    for (int j = 0; j < U.GetLength(1); j++)
                    {
                        Assert.Equal(U[i, j], resultU[i, j], 2);
                    }
                }

                for (int i = 0; i < vT.GetLength(0); i++)
                {
                    for (int j = 0; j < vT.GetLength(1); j++)
                    {
                        Assert.Equal(vT[i, j], resultvT[i, j], 2);
                    }
                }
            }

            {
                //
                double[,] A = new double[,]
                {
                { 8.79f,  9.93f,   9.83f,   5.45f,   3.16f, },
                {6.11f,   6.91f,   5.04f,  -0.27f,   7.98f, },
                {-9.15f,  -7.93f,   4.86f,   4.85f,   3.01f, },
                {9.57f,   1.64f,   8.83f,   0.74f,   5.80f, },
                {-3.49f,   4.02f,   9.80f,  10.00f,   4.27f, },
                { 9.84f,   0.15f,  -8.99f,  -6.02f,  -5.31f, },
                };

                (double[] s, double[,] U, double[,] vT) = MagmaSharp.LinAlg.Svd(A, true, true, device: Device.CUSTOM);

                for (int i = 0; i < result_s.Length; i++)
                    Assert.Equal(result_s[i], s[i], 2);


                for (int i = 0; i < U.GetLength(0); i++)
                {
                    for (int j = 0; j < U.GetLength(1); j++)
                    {
                        Assert.Equal(U[i, j], resultU[i, j], 2);
                    }
                }

                for (int i = 0; i < vT.GetLength(0); i++)
                {
                    for (int j = 0; j < vT.GetLength(1); j++)
                    {
                        Assert.Equal(vT[i, j], resultvT[i, j], 2);
                    }
                }
            }

        }
    }
}
