using LapackSharp;
using System;
using Xunit;

namespace Unit.Test.LapackSharp
{
    public class SolveTests
    {
        [Fact]
        public void TestFloat()
        {
            const int n = 5;
            const int nrhs = 3;
            const int lda = n;
            int[] ipiv = new int[5];

            float[,] result = new float[n, nrhs]
            {
                 {-0.8f, -0.39f, 0.96f},
                 {-0.7f, -0.55f, 0.22f},
                 {0.59f,  0.84f, 1.90f},
                 {1.32f, -0.10f, 5.36f},
                 {0.57f,  0.11f, 4.04f},
            };

            {
                //
                float[,] A = new float[lda, n]
                {
                { 6.80f, -6.05f, -0.45f,  8.32f,-9.67f },
                {-2.11f, -3.30f,  2.58f,  2.71f,-5.14f },
                { 5.66f,  5.36f, -2.70f,  4.35f,-7.26f },
                { 5.97f, -4.44f,  0.27f, -7.17f, 6.08f },
                { 8.23f,  1.08f,  9.04f,  2.14f,-6.87f },

                };
                float[,] B = new float[n, nrhs]
                {
                {4.02f, -1.56f,  9.81f},
                {6.19f,  4.00f, -4.09f},
                {-8.22f,-8.67f, -4.57f},
                {-7.57f, 1.75f, -8.61f},
                {-3.03f, 2.86f,  8.99f}
                };

                var X = LinAlg.Solve(A, B);
                for (int i = 0; i < X.GetLength(0); i++)
                {
                    for (int j = 0; j < X.GetLength(1); j++)
                    {
                        Assert.Equal(X[i, j], result[i, j], 2);
                    }
                }
            }

            {
                //
                float[,] A = new float[lda, n]
                {
                { 6.80f, -6.05f, -0.45f,  8.32f,-9.67f },
                {-2.11f, -3.30f,  2.58f,  2.71f,-5.14f },
                { 5.66f,  5.36f, -2.70f,  4.35f,-7.26f },
                { 5.97f, -4.44f,  0.27f, -7.17f, 6.08f },
                { 8.23f,  1.08f,  9.04f,  2.14f,-6.87f },

                };
                float[,] B = new float[n, nrhs]
                {
                {4.02f, -1.56f,  9.81f},
                {6.19f,  4.00f, -4.09f},
                {-8.22f,-8.67f, -4.57f},
                {-7.57f, 1.75f, -8.61f},
                {-3.03f, 2.86f,  8.99f}
                };

                var X = LinAlg.Solve(A, B);
                for (int i = 0; i < X.GetLength(0); i++)
                {
                    for (int j = 0; j < X.GetLength(1); j++)
                    {
                        Assert.Equal(X[i, j], result[i, j], 2);
                    }
                }
            }

            {
                //
                float[,] A = new float[lda, n]
                {
                { 6.80f, -6.05f, -0.45f,  8.32f,-9.67f },
                {-2.11f, -3.30f,  2.58f,  2.71f,-5.14f },
                { 5.66f,  5.36f, -2.70f,  4.35f,-7.26f },
                { 5.97f, -4.44f,  0.27f, -7.17f, 6.08f },
                { 8.23f,  1.08f,  9.04f,  2.14f,-6.87f },

                };
                float[,] B = new float[n, nrhs]
                {
                {4.02f, -1.56f,  9.81f},
                {6.19f,  4.00f, -4.09f},
                {-8.22f,-8.67f, -4.57f},
                {-7.57f, 1.75f, -8.61f},
                {-3.03f, 2.86f,  8.99f}
                };

                var X = LinAlg.Solve(A, B);
                for (int i = 0; i < X.GetLength(0); i++)
                {
                    for (int j = 0; j < X.GetLength(1); j++)
                    {
                        Assert.Equal(X[i, j], result[i, j], 2);
                    }
                }
            }
        }

        [Fact]
        public void TestDouble()
        {
            const int n = 5;
            const int nrhs = 3;
            int[] ipiv = new int[5];

            double[,] result = new double[n, nrhs]
            {
                 {-0.8, -0.39,0.96},
                 {-0.7, -0.55,0.22},
                 {0.59, 0.84 ,1.9 },
                 {1.32, -0.1 ,5.36},
                 {0.57, 0.11 ,4.04},
            };

            {
                //
                double[,] A = new double[n, n]
                {
                { 6.80, -6.05, -0.45,  8.32,-9.67 },
                {-2.11, -3.30,  2.58,  2.71,-5.14 },
                { 5.66,  5.36, -2.70,  4.35,-7.26 },
                { 5.97, -4.44,  0.27, -7.17, 6.08 },
                { 8.23,  1.08,  9.04,  2.14,-6.87 },

                };
                double[,] B = new double[n, nrhs]
                {
                {4.02f, -1.56f,  9.81f},
                {6.19f,  4.00f, -4.09f},
                {-8.22f,-8.67f, -4.57f},
                {-7.57f, 1.75f, -8.61f},
                {-3.03f, 2.86f,  8.99f}
                };

                var X = LinAlg.Solve(A, B);
                for (int i = 0; i < X.GetLength(0); i++)
                {
                    for (int j = 0; j < X.GetLength(1); j++)
                    {
                        Assert.Equal(X[i, j], result[i, j], 2);
                    }
                }
            }

            {
                //
                double[,] A = new double[n, n]
                {
                { 6.80, -6.05, -0.45,  8.32,-9.67 },
                {-2.11, -3.30,  2.58,  2.71,-5.14 },
                { 5.66,  5.36, -2.70,  4.35,-7.26 },
                { 5.97, -4.44,  0.27, -7.17, 6.08 },
                { 8.23,  1.08,  9.04,  2.14,-6.87 },

                };
                double[,] B = new double[n, nrhs]
                {
                {4.02f, -1.56f,  9.81f},
                {6.19f,  4.00f, -4.09f},
                {-8.22f,-8.67f, -4.57f},
                {-7.57f, 1.75f, -8.61f},
                {-3.03f, 2.86f,  8.99f}
                };

                var X = LinAlg.Solve(A, B);
                for (int i = 0; i < X.GetLength(0); i++)
                {
                    for (int j = 0; j < X.GetLength(1); j++)
                    {
                        Assert.Equal(X[i, j], result[i, j], 2);
                    }
                }
            }

            {
                //
                double[,] A = new double[n, n]
                {
                { 6.80, -6.05, -0.45,  8.32,-9.67 },
                {-2.11, -3.30,  2.58,  2.71,-5.14 },
                { 5.66,  5.36, -2.70,  4.35,-7.26 },
                { 5.97, -4.44,  0.27, -7.17, 6.08 },
                { 8.23,  1.08,  9.04,  2.14,-6.87 },

                };
                double[,] B = new double[n, nrhs]
                {
                {4.02f, -1.56f,  9.81f},
                {6.19f,  4.00f, -4.09f},
                {-8.22f,-8.67f, -4.57f},
                {-7.57f, 1.75f, -8.61f},
                {-3.03f, 2.86f,  8.99f}
                };

                var X = LinAlg.Solve(A, B);
                for (int i = 0; i < X.GetLength(0); i++)
                {
                    for (int j = 0; j < X.GetLength(1); j++)
                    {
                        Assert.Equal(X[i, j], result[i, j], 2);
                    }
                }
            }


        }


        [Fact]
        public void Test1Double()
        {
            
            double[,] A = new double[3, 3]
                {
                {2.0, 3.0, 5.0},
                {3.0, 5.0, 2.0},
                {5.0, 4.0, 4.0},
                };

            ///
            double[] B = new double[3]{ 21.0,22.0,25.0};

            var result = LinAlg.Solve(A,B);
            var expected = new double[3] { 1f, 3f, 2f };

            for (int j = 0; j < result.Length; j++)
            {
                Assert.Equal(expected[j], result[j], 2);
            }
        }
        [Fact]
        public void Test1fLOAT()
        {

            float[,] A = new float[3, 3]
                {
                {2.0F, 3.0F, 5.0F},
                {3.0F, 5.0F, 2.0F},
                {5.0F, 4.0F, 4.0F},
                };

            ///
            float[] B = new float[3] { 21.0F, 22.0F, 25.0F };

            var result = LinAlg.Solve(A, B);
            var expected = new float[3] { 1f, 3f, 2f };

            for (int j = 0; j < result.Length; j++)
            {
                Assert.Equal(expected[j], result[j], 2);
            }
        }
    }
}
