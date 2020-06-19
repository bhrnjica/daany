using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.stl;
using Daany.MathStuff;
using Daany.Stat;
using Daany.Ext;
using XPlot.Plotly;

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

    }

}
