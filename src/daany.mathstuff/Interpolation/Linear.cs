//////////////////////////////////////////////////////////////////////////////
//   ____    _    _   _   _   __  __                                       //
//  |  _ \  / \  | \ | | | \ | |\ \/ /                                     //
//  | | | |/ _ \ |  \| | |  \| | \  /                                      //
//  | |_| / ___ \| |\  | | |\  | | |                                       //
//  |____/_/   \_\_| \_| |_| \_| |_|                                       //
//                                                                         //
//  DAata ANalYtics Library                                                //
//  MathStuff:Linear Algebra, Statistics, Optimization, Machine Learning.  //
//  https://github.com/bhrnjica/daany                                      //
//                                                                         //
//  Copyright © 2006-2025 Bahrudin Hrnjica                                 //
//                                                                         //
//  Free. Open Source. MIT Licensed.                                       //
//  https://github.com/bhrnjica/daany/blob/master/LICENSE                  //
//////////////////////////////////////////////////////////////////////////////
namespace Daany.MathStuff.Interpolation;

/// <summary>
/// This implementation is based on the book:
/// Numerical Recipes 3rd Edition: The Art of Scientific Computing 3rd Edition
///	by William H.Press(Author).
/// https://www.amazon.com/Numerical-Recipes-3rd-Scientific-Computing/dp/0521880688
/// </summary>
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
