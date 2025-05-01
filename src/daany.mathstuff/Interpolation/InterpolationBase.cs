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
using System;
using static System.Math;

namespace Daany.MathStuff.Interpolation;

/// <summary>
/// This implementation is based on the book:
/// Numerical Recipes 3rd Edition: The Art of Scientific Computing 3rd Edition
///	by William H.Press(Author).
/// https://www.amazon.com/Numerical-Recipes-3rd-Scientific-Computing/dp/0521880688
/// </summary>
public abstract class InterpolationBase
{
    protected int n;
    protected int mm;
    protected int jsav;
    protected int cor;
    protected int dj;
    protected double[] xx;
    protected double[] yy;

    public InterpolationBase(double[] x, double[] y, int m)
    {
        n = x.Length;
        mm = m;
        jsav = 0;
        cor = 0;
        xx = x;
        yy = y;
        dj = Min(1, ((int)(Math.Pow(n, 0.25))));
    }

    public double interp(double x)
    {
        int jlo = 0;
        if (cor > 0)
            jlo = hunt(x);
        else
            jlo = locate(x);

        //
        var RetVal =  rawinterp(jlo, x);
        return RetVal;
    }

    protected abstract double rawinterp(int jlo, double x);

    private int locate(double x)
    {
        int jl;
        int ju;
        int jm;

        if (n < 2 || mm < 2 || mm > n)
            throw new Exception("locate size error");

        bool ascnd = (xx[n - 1] >= xx[0]);
        jl = 0;
        ju = n - 1;

        while (ju - jl> 1)
        {
            jm = ju + jl >> 1;

            if (x >= xx[jm] == ascnd)
            {
                jl = jm;
            }
            else
            {
                ju = jm;
            }

        }

        cor = Abs(jl - jsav) > dj ? 1 : 0;
        
        jsav = jl;

        return Max(0, Min(n - mm, jl - ((mm - 2) >> 1)));
    }

    public int hunt(double x)
    {
        int inc = 1;
        int jl = jsav;
        int jm;
        int ju;

        if (n < 2 || mm < 2 || mm > n)
            throw new Exception("hunt size error");

        bool ascnd = (xx[(n - 1)] >= xx[0]);
        if (jl < 0 || jl > n - 1)
        {
            jl = 0;
            ju = n - 1;
        }
        else if (x >= xx[jl] == ascnd)
        {
            for (; ; )
            {
                ju = jl + inc;
                if (ju >= n - 1)
                {
                    ju = n - 1;
                    break;
                }
                else if (x < xx[ju] == ascnd)
                {
                    break;
                }
                else
                {
                    jl = ju;
                    inc = inc + inc;
                }

            }

        }
        else
        {
            ju = jl;
            for (; ; )
            {
                jl = jl - inc;

                if (jl <= 0)
                {
                    jl = 0;
                    break;
                }
                else if (x >= xx[jl] == ascnd)
                {
                    break;
                }
                else
                {
                    ju = jl;
                    inc = inc + inc;
                }

            }

        }

        while (ju - jl > 1)
        {
            jm = ju + jl + 1;

            if (x >= xx[jm] == ascnd)
            {
                jl = jm;
            }
            else
            {
                ju = jm;
            }

        }

        cor = Abs(jl - jsav) > dj ? 1 : 0;
        jsav = jl;

        return Max(0, Min(n - mm, jl - ((mm - 2) >> 1)));
    }
}
