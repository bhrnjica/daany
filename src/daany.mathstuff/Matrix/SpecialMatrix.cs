//////////////////////////////////////////////////////////////////////////////////////////
// Daany - DAta ANalYtics Library                                                        //
// https://github.com/bhrnjica/daany                                                    //
//                                                                                      //
// Copyright 2006-2023 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/daany/blob/master/LICENSE        //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica at hotmail.com                                                              //
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////

using Daany.MathStuff.MatrixExt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Numerics;

namespace Daany.MathStuff;

#if NET7_0_OR_GREATER


public static class SpecialMatrix 
{
    //https://mathworld.wolfram.com/HankelMatrix.html
    public static T [,] Hankel< T >( T[] vector, int colCount=-1) where T : INumber<T>
    {
        int N = vector.Length;
        int L = colCount == -1 ? N : colCount;
        int K = colCount == -1 ? N : N - L + 1;
        var result = new T[L, K];

        for(int i = 0; i < L; i++)
        {
            for (int j = 0; j < K; j++)
            {
                result[i, j] = (i + j) > N-1 ? default: vector[i + j];
            }
        }

        return result;
    }
    //https://mathworld.wolfram.com/ToeplitzMatrix.html
    public static T[,] Toeplitz <T> ( T[] vector) where T : INumber<T>
    {
        int N = vector.Length;
        var result = new T[N, N];

        for (int i = 0; i < N; i++)
        {
            for (int j = i; j < N; j++)
            {
                result[i, j] = vector[j - i];
                result[j, i] = result[i, j];
            }
        }

        return result;
    }

    public static TResult[,] Zeros<TResult>(int rows, int cols)
    {
        var result = new TResult[rows, cols];

        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = default;

        return result;
    }

    public static TResult[] Zeros<TResult>(int elements)
    {
        var result = new TResult[elements];
        for (int i = 0; i < result.GetLength(0); i++)
            result[i] = default;

        return result;
    }

    public static TResult[] Unit<TResult>(int elements) where TResult : INumber<TResult>
    {
        var result = new TResult[elements];
        for (int i = 0; i < result.GetLength(0); i++)
            result[i] = TResult.CreateChecked(1);

        return result;
    }

    public static TResult[,] Identity<TResult>(int rows, int cols) where TResult : INumber<TResult>
    {
        var retVal = new TResult[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (i != j)
                    retVal[i, j] = default;
                else
                    retVal[i, j] = TResult.CreateChecked(1);

            }
        }

        //
        return retVal;
    }

    /// <summary>
    /// Create vector of randomly generated double values stored in 1D array. 
    /// Vector.Length = row * col
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public static T[] Rand<T>(int row, int col) where T : IFloatingPointIeee754<T>
    {
        var size = row * col;
        var obj = new T[size];
        for (int i = 0; i < size; i++)
        {
            obj[i] = T.CreateChecked<double>(Constant.rand.NextDouble());
        }
        return obj;
    }

    public static T[] Rand<T>(int length, double min, double max) where T : IFloatingPointIeee754<T>
    {
        var obj = new T[length];
        for (int i = 0; i < length; i++)
            obj[i] = T.CreateChecked<double>(Constant.rand.NextDouble(min,max));
        return obj;
    }

  
    public static T[] Arange<T>(int stop) where T : IFloatingPointIeee754<T>
    {
        return Arange<T>(0, stop, 1);
    }

    public static T[] Arange<T>(int start, int stop, int step = 1) where T : IFloatingPointIeee754<T>
    {
        if (start > stop)
        {
            throw new Exception("parameters invalid, start is greater than stop.");
        }

        int length = (int)Math.Ceiling((stop - start + 0.0) / step);
        int index = 0;

        var vector = new T[length];
        for (int i = start; i < stop; i += step)
            vector[index++] = T.CreateChecked<int>(i);

        return vector;
    }

   
    public static T[] Generate<T>(int row, int col, T val) where T: INumber<T>
    {
        var size = row * col;
        var obj = new T[size];
        for (int i = 0; i < size; i++)
            obj[i] = val;
        return obj;
    }

    public static DateTime[] DateSeries(DateTime fromDate, DateTime toDate, TimeSpan span)
    {
        var lst = new List<DateTime>();
        for (DateTime i = fromDate; i < toDate; i += span)
        {
            lst.Add(i);
        }

        return lst.ToArray();
    }

    public static DateTime[] MonthlySeries(DateTime fromDate, int months, int count)
    {
        var lst = new List<DateTime>();
        var dt = fromDate;

        for (int i = 0; i < count; i++)
        {
            lst.Add(dt);

            dt = dt.AddMonths(months);
        }

        return lst.ToArray();
    }
    public static DateTime[] YearlySeries(DateTime fromDate, int years, int count)
    {
        var lst = new List<DateTime>();
        var dt = fromDate;
        for (int i = 0; i < count; i++)
        {
            lst.Add(dt);
            dt = dt.AddYears(years);
        }

        return lst.ToArray();
    }

    public static T[] SSeries<T>(T fromNumber, T toNumber, T step) where T : INumber<T>
    {
        var lst = new List<T>();
        for (T i = fromNumber; i < toNumber; i += step)
        {
            lst.Add(i);
        }

        return lst.ToArray();
    }


    

    public static T[] NSeries<T>(T fromNumber, T toNumber, T count) where T : INumber<T>
    {
        var lst = new List<T>();
        var step = (toNumber - fromNumber) / count;
        for (T i = fromNumber; i < toNumber; i += step)
        {
            lst.Add(i);
        }

        return lst.ToArray();
    }

    public static T[] ConstSeries<T>(T value, int count) where T : INumber<T>
    {
        var lst = new List<T>();
        for (double i = 0; i < count; i++)
        {
            lst.Add(value);
        }

        return lst.ToArray();
    }
}

#endif
