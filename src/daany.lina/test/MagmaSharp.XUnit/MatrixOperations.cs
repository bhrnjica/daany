using System;
using Xunit;

namespace MagmaSharp.XUnit
{
    public class MatrixOperations
    {
        [Fact]
        public void MInverseTest_01()
        {
            //Matrix A
            var A =new double[4, 4]// { { 3, 3.2 }, { 3.5, 3.6 } };

                {
                { 1, 2, 3, 4},
                { 2, 2, 5, 4},
                { 3, 2, 6, 4},
                { 5, 5, 3, 4} };

            var result = new double[4, 4]
            {
               { 1.00000,-3.00000,  2.00000, 0.00000 },
               { -1.66667,    4.00000, -2.66667 ,   0.33333},
               { -1.00000,    2.00000, -1.00000,    0.00000},
               { 1.58333, -2.75000,    1.58333, -0.16667}

            };
            var C = LapackSharp.LinAlg.MInverse(A);

            for (int i = 0; i < C.GetLength(0); i++)
            {
                for (int j = 0; j < C.GetLength(1); j++)
                {
                    Assert.Equal(Math.Round(C[i, j],5), result[i, j], 2);
                }
            }

            //Matrix A
            var AA = new float[4, 4]// { { 3, 3.2 }, { 3.5, 3.6 } };

                {
                { 1, 2, 3, 4},
                { 2, 2, 5, 4},
                { 3, 2, 6, 4},
                { 5, 5, 3, 4} };

            var result1 = new float[4, 4]
            {
               { 1.00000f, -3.00000f,  2.00000f, 0.00000f },
               { -1.66667f,    4.00000f, -2.66667f,   0.33333f},
               { -1.00000f,    2.00000f, -1.00000f,    0.00000f},
               { 1.58333f, -2.75000f,    1.58333f, -0.16667f}

            };
            var CC = LapackSharp.LinAlg.MInverse(A);

            for (int i = 0; i < CC.GetLength(0); i++)
            {
                for (int j = 0; j < CC.GetLength(1); j++)
                {
                    Assert.Equal(Convert.ToSingle(Math.Round(CC[i, j], 5)), result1[i, j], 2);
                }
            }

        }

        [Fact]
        public void MultiplicationTest_01()
        {
            var A = new double[3, 5]
                {
                {1,   2,   3,   4,   5 },
                {6,   7,   8,   9,   10 },
                { 11,  12,  13,  14,  15 }
                 };
            var B = new double[5, 3]
                {
                    {1,2,5 },
                    {3,4,7 },
                    {5,6,4 },
                    {7,8,1 },
                    {9,10,15}
                };

            var C = LapackSharp.LinAlg.MMult(A, B);
            var result = new double[3, 3]
                {
                {95,  110, 110},
                {220, 260, 270},
                { 345, 410, 430}
                };

            for (int i = 0; i < C.GetLength(0); i++)
            {
                for (int j = 0; j < C.GetLength(1); j++)
                {
                    Assert.Equal(Convert.ToSingle(Math.Round(C[i, j], 5)), result[i, j], 2);
                }
            }
        }


        [Fact]
        public void MultiplicationTest_04()
        {
            var A = new double[3, 5]
                {
                {1,   2,   3,   4,   5 },
                {6,   7,   8,   9,   10 },
                { 11,  12,  13,  14,  15 }
                 };
            var B = new double[5] {1,3,5,7,9 };

            var C = LapackSharp.LinAlg.MMult(A, B);
            var result = new double[3]{95,220,345 };

            for (int i = 0; i < C.GetLength(0); i++)
            {
                Assert.Equal(C[i], result[i], 2);
            }
        }
        [Fact]
        public void MultiplicationTest_064()
        {
            var alpha = 2.0;
            var betta = 3.0;

            var A = new double[3, 5]
                {
                {1,   2,   3,   4,   5 },
                {6,   7,   8,   9,   10 },
                { 11,  12,  13,  14,  15 }
                 };
            var B = new double[5] { 1, 3, 5, 7, 9 };
            var C = new double[3] { 12, 13, 14 };
            var D = LapackSharp.LinAlg.MMult(A, B, C, alpha,betta);
            var result = new double[3] { 226, 479, 732 };

            for (int i = 0; i < D.GetLength(0); i++)
            {
                Assert.Equal(D[i], result[i], 2);
            }
        }

        [Fact]
        public void MultiplicationTest_05()
        {
            var A = new float[3, 5]
                {
                {1,   2,   3,   4,   5 },
                {6,   7,   8,   9,   10 },
                { 11,  12,  13,  14,  15 }
                 };
            var B = new float[5] { 1, 3, 5, 7, 9 };

            var C = LapackSharp.LinAlg.MMult(A, B);
            var result = new float[3] { 95, 220, 345 };

            for (int i = 0; i < C.GetLength(0); i++)
            {
                Assert.Equal(C[i], result[i], 2);
            }
        }

        [Fact]
        public void MultiplicationTest_02()
        {
            double alpha = 2.0;
            double betta = 3.0;
            var A = new double[3, 5]
                {
                {1,   2,   3,   4,   5 },
                {6,   7,   8,   9,   10 },
                { 11,  12,  13,  14,  15 }
                 };
            var B = new double[5, 2]
                {
                    {1,2 },
                    {3,4 },
                    {5,6 },
                    {7,8 },
                    {9,10}
                };
            var C = new double[3, 2] { {2,4 },{6,8 },{10,12 } };
            var D = LapackSharp.LinAlg.MMult(A, B, C, alpha,betta);
            var result = new double[3, 2]
                {
                { 196, 232 },
                {458,544},
                {720,856 }};

            for (int i = 0; i < C.GetLength(0); i++)
            {
                for (int j = 0; j < C.GetLength(1); j++)
                {
                    Assert.Equal(Convert.ToSingle(Math.Round(D[i, j], 5)), result[i, j], 2);
                }
            }
        }

    }
}
