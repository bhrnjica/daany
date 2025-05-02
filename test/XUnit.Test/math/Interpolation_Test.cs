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
using Daany.MathStuff.Interpolation;

namespace Daany.MathStuff.Tests
{
    public class InterpolationTest
    {
        
        [Fact(Skip = "Use this tes only if you want to play with plotting capabilities.")]
        public void Spline_Test01()
        {
            var x = new double[] { 1, 2, 3};
            var y = new double[] { 2,5,1 };
            var fun1 = new Linear(x,y);//linear iterpolation
            var fun2 = new Poly(x, y,3);//quadratic interpolation
            var fun3 = new Spline(x, y);//SPline interpolation


            var retVal1  = fun1.interp(1.5);
            var retVal2  = fun2.interp(1.5);
            var retVal3  = fun3.interp(1.5);

            var xVal = nc.GenerateDoubleSeries(0.8, 3.3, 0.1).Select(x=>(double)x).ToList();
            var yVal1 = xVal.Select(x => fun1.interp(x)).ToList();
            var yVal2 = xVal.Select(x => fun2.interp(x)).ToList();
            var yVal3 = xVal.Select(x => fun3.interp(x)).ToList();


            var s1 = new Scatter() { name = "Linear", x = xVal, y =yVal1 , mode = "line", };
            var s2 = new Scatter() { name = "Poly", x = xVal, y = yVal2, mode = "line", };
            var s3 = new Scatter() { name = "Spline", x = xVal, y = yVal3, mode = "line", };
            var s4 = new Scatter() { name = "Actual", x = x, y = y, mode = "point", fillcolor = "Blue" };

            var chart = XPlot.Plotly.Chart.Plot<Trace>(new Trace[] { s1, s2, s3, s4});
            chart.Show();
        }       
    }
}
