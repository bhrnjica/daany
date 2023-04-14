using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Stat.stl;
using Daany.Stat;
using Daany.Ext;
using XPlot.Plotly;
using Daany.MathStuff;

namespace Unit.Test.DF
{
    public class MatrixTest
    {
        
        [Fact]
        public void HankelMatrix_Test01()
        {
            var ts = new double[] { 1, 2, 3, 4, 5,6};
            var r = ts.Hankel();
            //
            Assert.Equal(1, r[0, 0]);
            Assert.Equal(2, r[0, 1]);
            Assert.Equal(3, r[0, 2]);
            Assert.Equal(4, r[0, 3]);
            Assert.Equal(5, r[0, 4]);
            Assert.Equal(6, r[0, 5]);

            Assert.Equal(2, r[1, 0]);
            Assert.Equal(3, r[1, 1]);
            Assert.Equal(4, r[1, 2]);
            Assert.Equal(5, r[1, 3]);
            Assert.Equal(6, r[1, 4]);
            Assert.Equal(0, r[1, 5]);

            Assert.Equal(3, r[2, 0]);
            Assert.Equal(4, r[2, 1]);
            Assert.Equal(5, r[2, 2]);
            Assert.Equal(6, r[2, 3]);
            Assert.Equal(0, r[2, 4]);
            Assert.Equal(0, r[2, 5]);

            Assert.Equal(4, r[3, 0]);
            Assert.Equal(5, r[3, 1]);
            Assert.Equal(6, r[3, 2]);
            Assert.Equal(0, r[3, 3]);
            Assert.Equal(0, r[3, 4]);
            Assert.Equal(0, r[3, 5]);

            Assert.Equal(5, r[4, 0]);
            Assert.Equal(6, r[4, 1]);
            Assert.Equal(0, r[4, 2]);
            Assert.Equal(0, r[4, 3]);
            Assert.Equal(0, r[4, 4]);
            Assert.Equal(0, r[4, 5]);

            Assert.Equal(6, r[5, 0]);
            Assert.Equal(0, r[5, 1]);
            Assert.Equal(0, r[5, 2]);
            Assert.Equal(0, r[5, 3]);
            Assert.Equal(0, r[5, 4]);
            Assert.Equal(0, r[5, 5]);

        }

        [Fact]
        public void HankelMatrix_Test02()
        {
            var ts = new double[] { 1, 2, 3, 4, 5 };
            var r = ts.Hankel(2);
            //
            Assert.Equal(1, r[0, 0]);
            Assert.Equal(2, r[0, 1]);
            Assert.Equal(3, r[0, 2]);
            Assert.Equal(4, r[0, 3]);

            Assert.Equal(2, r[1, 0]);
            Assert.Equal(3, r[1, 1]);
            Assert.Equal(4, r[1, 2]);
            Assert.Equal(5, r[1, 3]);


        }

        [Fact]
        public void ToeplitzMatrix_Test01()
        {
            var ts = new double[] { 1, 2, 3, 4, 5, 6 };
            var r = ts.Toeplitz();
            //
            Assert.Equal(1, r[0, 0]);
            Assert.Equal(2, r[0, 1]);
            Assert.Equal(3, r[0, 2]);
            Assert.Equal(4, r[0, 3]);
            Assert.Equal(5, r[0, 4]);
            Assert.Equal(6, r[0, 5]);

            Assert.Equal(2, r[1, 0]);
            Assert.Equal(1, r[1, 1]);
            Assert.Equal(2, r[1, 2]);
            Assert.Equal(3, r[1, 3]);
            Assert.Equal(4, r[1, 4]);
            Assert.Equal(5, r[1, 5]);

            Assert.Equal(3, r[2, 0]);
            Assert.Equal(2, r[2, 1]);
            Assert.Equal(1, r[2, 2]);
            Assert.Equal(2, r[2, 3]);
            Assert.Equal(3, r[2, 4]);
            Assert.Equal(4, r[2, 5]);

            Assert.Equal(4, r[3, 0]);
            Assert.Equal(3, r[3, 1]);
            Assert.Equal(2, r[3, 2]);
            Assert.Equal(1, r[3, 3]);
            Assert.Equal(2, r[3, 4]);
            Assert.Equal(3, r[3, 5]);

            Assert.Equal(5, r[4, 0]);
            Assert.Equal(4, r[4, 1]);
            Assert.Equal(3, r[4, 2]);
            Assert.Equal(2, r[4, 3]);
            Assert.Equal(1, r[4, 4]);
            Assert.Equal(2, r[4, 5]);

            Assert.Equal(6, r[5, 0]);
            Assert.Equal(5, r[5, 1]);
            Assert.Equal(4, r[5, 2]);
            Assert.Equal(3, r[5, 3]);
            Assert.Equal(2, r[5, 4]);
            Assert.Equal(1, r[5, 5]);

        }




        [Fact]
        public void SVD_Test01()
        {
            var matrix = new double[6,5]
            {
            { 8.79,  9.93,  9.83, 5.45,  3.16 },
            { 6.11,  6.91,  5.04, -0.27,  7.98 },
           { -9.15, -7.93,  4.86, 4.85,  3.01 },
            { 9.57,  1.64,  8.83, 0.74,  5.80 },
           { -3.49,  4.02,  9.80, 10.00,  4.27 },
            { 9.84,  0.15, -8.99, -6.02, -5.31 }
            };
             //SVD
            var svd = Daany.LinA.LinA.Svd(matrix, false,false);
            //
            Assert.Equal(27.47, svd.s[0], 2);
            Assert.Equal(22.64, svd.s[1], 2);
            Assert.Equal(8.56, svd.s[2], 2);
            Assert.Equal(5.99, svd.s[3], 2);
            Assert.Equal(2.01, svd.s[4], 2);


        }
    }

}
