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

#if NET7_0_OR_GREATER

using System;
using System.Numerics;

namespace Daany.MathStuff.MatrixGeneric;


public static class Operations
{
    public static T[,] Add<T  >(this T[,] matrix1, T[,] matrix2) where T : IFloatingPoint<T> 
    {
        var retVal = new T[matrix1.GetLength(0), matrix2.GetLength(1)];

        for (int i = 0; i < matrix1.GetLength(0); i++)
        {
            for (int j = 0; j < matrix2.GetLength(1); j++)
            { 
               retVal[i, j] = matrix1[i, j] + matrix2[i, j];
            }
        }
        //
        return retVal;
    }

    public static T[] Add<T>(T[] vector1, T[] vector2) where T : IFloatingPoint<T>
    {
        var result = new T[vector1.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = vector1[i] + vector2[i];
        }
        return result;
    }

    public static T[] Add<T>(T[] matrix, T scalar) where T : IFloatingPoint<T>
    {
        var result = new T[matrix.Length];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            result[i] = matrix[i] + scalar;
        }
        return result;
    }

    public static T[,] Add< T >( T[,] matrix, T scalar) where T : IFloatingPoint<T>
    {
        var result = new T[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                result[i, j] = matrix[i, j] + scalar;
            }
        }
        return result;
    }

    public static T[,] Substract<T>(T[,] matrix1, T[,] matrix2) where T : IFloatingPoint<T>
    {
        var retVal = new T[matrix1.GetLength(0), matrix2.GetLength(1)];

        for (int i = 0; i < matrix1.GetLength(0); i++)
        {
            for (int j = 0; j < matrix2.GetLength(1); j++)
            {
                retVal[i, j] = matrix1[i, j] - matrix2[i, j];
            }
        }
        //
        return retVal;
    }

    public static T[,] Substract<T>(T[,] matrix, T scalar) where T : IFloatingPoint<T>
    {
        var result = new T[matrix.GetLength(0), matrix.GetLength(1)];
        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
               result[i, j] = matrix[i, j] - scalar;
            }
        }
        return result;
    }

    public static T[] Substract<T>(T[] matrix, T scalar) where T : IFloatingPoint<T>
    {
        var result = new T[matrix.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = matrix[i] - scalar;
        }
        return result;
    }
    public static T[] Substruct<T>(T[] vector1, T[] vector2) where T : IFloatingPoint<T>
    {
        var res = new T[vector1.Length];

        for (int i = 0; i < vector1.Length; i++)
        {
            res[i] = vector1[i] - vector2[i];
        }

        return res;
    }

    public static T[,] Multiply<T>(T[,] matrix, T scalar) where T : IFloatingPoint<T>
    {
        var result = new T[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            { 
                result[i, j] = matrix[i, j] * scalar; 
            }
        }
        return result;
    }


    public static T[] Multiply<T>(T[] vector, T scalar) where T : IFloatingPoint<T>
    {
        var result = new T[vector.Length];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            result[i] = vector[i] * scalar;
        }

        return result;
    }

    public static T[] Multiply<T>(T[] vector1, T[] vector2) where T : IFloatingPoint<T>
    {
        var result = new T[vector1.Length];

        for (int i = 0; i < vector1.Length; i++)
        {
            result[i] = vector1[i] * vector2[i];
        }
        return result;
    }

    public static T[,] Divide<T>(T[,] matrix, T scalar) where T : IFloatingPoint<T>
    {
        var result = new T[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                result[i, j] = matrix[i, j] / scalar;
            }
        }
        return result;
    }

    public static T[] Divide< T >( T[] matrix, T scalar) where T : IFloatingPoint<T>
    {
        var result = new T[matrix.Length];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            result[i] = matrix[i] / scalar;
        }
        return result;
    }

    public static T[] Divide< T >(T[] vector1, T[] vector2) where T : IFloatingPoint<T>
    {
        var result = new T[vector1.Length];

        for (int i = 0; i < vector1.Length; i++)
        {
            result[i] = vector1[i] / vector2[i];
        }
        return result;
    }

    public static T[] Div<T>(T[] vector, T scalar) where T : IFloatingPoint<T>
    {
        T[] r = new T[vector.Length];

        for (int i = 0; i < vector.Length; i++)
        {
            r[i] = (vector[i] / scalar);
        }
        return r;
    }

    public static T[] Dot<T>(T[] vector, T scalar) where T : IFloatingPoint<T>
    {
        T[] r = new T[vector.Length];

        for (int i = 0; i < vector.Length; i++)
        {
            r[i] = (vector[i] * scalar);
        }
        return r;
    }

    public static T[] Dot<T>(T[,] matrix, T[] vector) where T : IFloatingPoint<T>
    {
        if (matrix.GetLength(1) != vector.Length)
            throw new Exception("Wrong dimensions of matrix or vector!");

        T[] result = new T[matrix.GetLength(0)];

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < vector.Length; j++)
            {
                result[i] += matrix[i, j] * vector[j];
            }
        }
        return result;
    }

    public static T[,] Dot<T>(T[,] matrix1, T[,] matrix2) where T : IFloatingPoint<T>
    {

        if (matrix1.GetLength(1) != matrix2.GetLength(0))
            throw new Exception("Wrong dimensions of matrices!");

        T[,] result = new T[matrix1.GetLength(0), matrix2.GetLength(1)];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                for (int k = 0; k < matrix1.GetLength(1); k++)
                {
                    result[i, j] += matrix1[i, k] * matrix2[k, j];
                }
            }

        }

        return result;
    }

    public static T[] Dot<T>(T[] vector, T[,] matrix) where T : IFloatingPoint<T>
    {
        var result = new T[matrix.Columns()];

        int cols = matrix.Columns<T>();

        for (int j = 0; j < cols; j++)
        {
            T s = default;
            for (int k = 0; k < vector.Length; k++)
            {
                s += vector[k] * matrix[k, j];
            }
            result[j] = s;
        }
        return result;
    }

    public static T[,] Sqrt<T>(T[,] matrix) where T : IFloatingPoint<T>, IRootFunctions<T>
    {

        T[,] result = new T[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                result[i, j] = T.Sqrt(matrix[i, j]);
            }
        }
        return result;
    }

    public static T[] Sqrt<T>( T[] vector) where T : IFloatingPoint<T>, IRootFunctions<T>
    {
        T[] result = new T[vector.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = T.Sqrt(vector[i]);
        }
        return result;
    }

    public static T[,] Log< T >( T[,] matrix) where T : IFloatingPoint<T>, IFloatingPointIeee754<T>, ITrigonometricFunctions<T>
    {

        T[,] result = new T[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
              result[i, j] = T.Log(matrix[i, j]);
            }
        }
        return result;
    }

    public static T[] Log< T > ( T[] vector) where T : IFloatingPoint<T>, IFloatingPointIeee754<T>
    {
        T[] result = new T[vector.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = T.Log(vector[i]);
        }

        return result;
    }

    public static T[,] Pow<T>(T[,] matrix, T scalar) where T : IFloatingPoint<T>, IFloatingPointIeee754<T>
    {
        T[,] result = new T[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                result[i, j] = T.Pow(matrix[i, j], scalar);
            }
        }
        return result;
    }

    public static T[] Pow<T>(T[] vector, T scalar) where T : IFloatingPoint<T>, IFloatingPointIeee754<T>
    {
        T[] result = new T[vector.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = T.Pow(vector[i], scalar);
        }
        return result;
    }

    /// <summary>
    /// Returns the vector of matrix columnIndex
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="colIndex"></param>
    /// <returns></returns>
    public static T[] CVector< T >(T[,] matrix, int colIndex) where T : IFloatingPoint<T>
    {

        T[] result = new T[matrix.GetLength(0)];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            result[i] = matrix[i, colIndex];
        }
        return result;
    }

    /// <summary>
    /// Returns the vector of matrix rowIndex
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    public static T[] RVector< T >(T[,] matrix, int rowIndex) where T : IFloatingPoint<T>
    {

        T[] result = new T[matrix.GetLength(1)];

        for (int i = 0; i < result.GetLength(1); i++)
            result[i] = matrix[rowIndex, i];
        return result;
    }


    public static T Euclidean<T>(T[,] matrix) where T : IFloatingPoint<T>, IFloatingPointIeee754<T>
    {
        return Norm2<T>(matrix);
    }

    public static T Norm2< T >(T[] matrix) where T : IFloatingPoint<T>, IFloatingPointIeee754<T>
    {
        T retVal = default;
        for (int i = 0; i < matrix.Length; i++)
            retVal += matrix[i] * matrix[i];

        return T.Sqrt(retVal);
    }

    public static T Norm2< T >(T[,] matrix) where T : IFloatingPoint<T>, IFloatingPointIeee754<T>
    {
        T retVal = default;
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                retVal += matrix[i, j] * matrix[i, j];
            }
        }
        return T.Sqrt(retVal);
    }

    public static T[] CumulativeSum< T >( T[] vector ) where T : IFloatingPoint<T>
    {
        if (vector == null || vector.Length == 0)
            return new T[0];

        var result = new T[vector.Length];
        result[0] = vector[0];

        for (int i = 1; i < vector.Length; i++)
        {
            result[i] = result[i - 1] + vector[i];
        }
        return result;
    }
}
#endif