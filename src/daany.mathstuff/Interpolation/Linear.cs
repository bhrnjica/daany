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
/*
 * This implementation is based on the book:
 * Numerical Recipes 3rd Edition: The Art of Scientific Computing 3rd Edition
    by William H. Press (Author).
    https://www.amazon.com/Numerical-Recipes-3rd-Scientific-Computing/dp/0521880688
 */
namespace Daany.MathStuff.Interpolation
{
    public class Linear : InterpolationBase
    {
        public Linear(double[] xv, double[] yv) :  base(xv, yv, 2)
        {
        }

        protected override double rawinterp(int j, double x)
        {
            if (xx[j] == xx[j + 1])
            {
                return yy[j];
            }
            else
            {
                return yy[j] + ((x - xx[j]) / (xx[j + 1] - xx[j])) * (yy[j + 1] - yy[j]);
            }
        }
    }
}
