//////////////////////////////////////////////////////////////////////////////////////////
// Daany - DAta ANalYtics Library                                                        //
// https://github.com/bhrnjica/daany                                                    //
//                                                                                      //
// Copyright 2006-2021 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/daany/blob/master/LICENSE        //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica at hotmail.com                                                              //
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using static System.Math;

namespace Daany.MathStuff.Interpolation
{
    /*
 * This implementation is based on the book:
 * Numerical Recipes 3rd Edition: The Art of Scientific Computing 3rd Edition
    by William H. Press (Author).
    https://www.amazon.com/Numerical-Recipes-3rd-Scientific-Computing/dp/0521880688
 */
    public class Poly : InterpolationBase
    {
        double dy;
        public Poly(double[] xv, double[] yv, int m) : base(xv, yv, m)
        {
            
        }

        protected override double rawinterp(int jl, double x)
        {
            int i, m, ns = 0;
            double y, den, dif, dift, ho, hp, w;

            double[] c = new double[mm];
            double[] d = new double[mm];

            dif = Abs(x - xx[jl + 0]);

            for (i = 0; i < mm; i++)
            {
                if ((dift = Abs(x - xx[jl + i])) < dif)
                {
                    ns = i;
                    dif = dift;
                }

                c[i] = yy[jl + i];
                d[i] = yy[jl + i];
            }

            y = yy[jl + ns--];
            for (m = 1; m < mm; m++)
            {
                for (i = 0; i < mm - m; i++)
                {
                    ho = xx[jl + i] - x;
                    hp = xx[jl + i + m] - x;
                    w = c[i + 1] - d[i];

                    if ((den = ho - hp) == 0.0) 
                        throw new Exception("Poly_interp error");

                    den = w / den;
                    d[i] = hp * den;
                    c[i] = ho * den;
                }

                y += (dy = (2 * (ns + 1) < (mm - m) ? c[ns + 1] : d[ns--]));
            }
            return y;
        }
    }
}
