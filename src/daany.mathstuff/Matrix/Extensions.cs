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

using System;
using System.Numerics;

#if NET7_0_OR_GREATER

namespace Daany.MathStuff.MatrixGeneric;

/// <summary>
/// Matrix generic math implementation based on 2D array type
/// </summary>
public static class Extensions
{

    public static T[] GetColumn<T>(this T[,] m, int index)
    {
        T[] result = new T[m.Rows()];

        if (index < 0)
        {
            index = m.GetLength(1) + index;
        }

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = m[i, index];
        }

        return result;
    }

    public static T[] GetColumn<T>(this T[][] m, int index)
    {
        T[] result = new T[m.Length];

        if (index < 0)
        {
            index = m[0].Length + index;
        }

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = m[i][index];
        }

        return result;
    }


    public static T[] GetRow<T>(this T[,] matrix, int index)
    {
        T[] result = new T[matrix.Columns()];

        if (index < 0)
            index = matrix.GetLength(0) + index;

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = matrix[index, i];
        }

        return result;
    }

    public static T[] GetRow<T>(this T[][] matrix, int index)
    {
        T[] result = new T[matrix[0].Length];

        if (index < 0)
            index = matrix.Length + index;

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = matrix[index][i];
        }

        return result;
    }


    public static T[] GetDiagonal<T>(this T[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);

        int size = Math.Min(rows, cols);

        var result = new T[size];

        for (int i = 0; i < size; i++)
        {
            result[i] = matrix[i, i];
        }

        return result;
    }

    public static T[] GetDiagonal<T>(this T[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;

        int size = Math.Min(rows, cols);

        var result = new T[size];

        for (int i = 0; i < size; i++)
        {
            result[i] = matrix[i][i];
        }

        return result;
    }


    public static T[,] MakeDiagonal<T>(this T[] values)
    {
        var result = new T[values.Length, values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            result[i, i] = values[i];
        }
        return result;
    }

    public static T[,] Copy<T>(this T[,] matrix)
    {
        return (T[,])matrix.Clone();
    }

    public static T[,] Reverse<T>(this T[,] matrix, bool row)
    {
        int r = matrix.GetLength(0);
        int c = matrix.GetLength(1);

        T[,] m = new T[r, c];

        for (int i = 0; i < r; i++)
        {
            int ii = i;

            for (int j = 0; j < c; j++)
            {
                int jj = j;

                if (row)
                    ii = r - i - 1;
                else
                    jj = c - j - 1;

                m[i, j] = matrix[ii, jj];
            }
        }
        return m;
    }

    public static T[][] Reverse<T>(this T[][] matrix, bool row)
    {
        int r = matrix.Length;
        int c = matrix[0].Length;

        T[][] m = new T[r][];

        for (int i = 0; i < r; i++)
        {
            int ii = i;

            m[i] = new T[c];

            for (int j = 0; j < c; j++)
            {
                int jj = j;

                if (row)
                    ii = r - i - 1;
                else
                    jj = c - j - 1;

                m[i][j] = matrix[ii][jj];
            }
        }
        return m;
    }


    public static T[,] ToMatrix<T>(this T[][] matrix, bool transpose = false)
    {
        int rows = matrix.Length;

        if (rows == 0)
        {
            return new T[0, rows];
        }

        int cols = matrix[0].Length;

        T[,] retVal;

        if (transpose)
        {
            retVal = new T[cols, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    retVal[j, i] = matrix[i][j];
                }
            }
        }
        else
        {
            retVal = new T[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    retVal[i, j] = matrix[i][j];
                }
            }
        }

        return retVal;
    }

    public static T[,] ToMatrix<T>(this T[] matrix, bool asColumnVector = false)
    {
        if (asColumnVector)
        {
            T[,] retVal = new T[matrix.Length, 1];

            for (int i = 0; i < matrix.Length; i++)
            {
                retVal[i, 0] = matrix[i];
            }
            return retVal;
        }
        else
        {
            T[,] retVal = new T[1, matrix.Length];

            for (int i = 0; i < matrix.Length; i++)
            {
                retVal[0, i] = matrix[i];
            }
            return retVal;
        }
    }

    public static T[,] ToMatrix<T>(this T[] matrix, int nRows)
    {
        int rows = nRows;
        int cols = matrix.Length / rows;
        int k = 0;
        T[,] retVal = new T[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                retVal[i, j] = matrix[k];
                k++;

            }
        }

        return retVal;
    }

    public static T[][] ToMatrix<T>(this T[,] matrix)
    {
        int rowsFirstIndex = matrix.GetLowerBound(0);
        int rowsLastIndex = matrix.GetUpperBound(0);
        int numberOfRows = rowsLastIndex - rowsFirstIndex + 1;

        int columnsFirstIndex = matrix.GetLowerBound(1);
        int columnsLastIndex = matrix.GetUpperBound(1);
        int numberOfColumns = columnsLastIndex - columnsFirstIndex + 1;

        T[][] m = new T[numberOfRows][];
        for (int i = 0; i < numberOfRows; i++)
        {
            m[i] = new T[numberOfColumns];

            for (int j = 0; j < numberOfColumns; j++)
            {
                m[i][j] = matrix[i + rowsFirstIndex, j + columnsFirstIndex];
            }
        }
        return m;
    }


    public static T[,] Transpose<T>(this T[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        var retVal = new T[cols, rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                retVal[i, j] = matrix[j, i];
            }
        }
        //
        return retVal;
    }

     public static T[][] Transpose<T>(this T[][] matrix)
    {
        int rows = matrix.Length;
        int cols = matrix[0].Length;

        T[][] retVal = new T[cols][];

        for (int i = 0; i < rows; i++)
        {
            retVal[i] = new T[cols];

            for (int j = 0; j < cols; j++)
            {
                retVal[i][j] = matrix[j][i];
            }
        }
        //
        return retVal;
    }


    public static T[,] Transpose<T>(this T[] vector)
    {
        var retVal = new T[vector.Length, 1];

        for (int i = 0; i < vector.Length; i++)
        {
            retVal[i, 0] = vector[i];
        }

        return retVal;
    }

    public static T Det<T>(this T[,] matrix) where T : IFloatingPoint<T>
    {
        var rows = matrix.GetLength(0);

        var lu = matrix.MakeLU<T, T>();

        T det = T.CreateChecked( 1.0 );

        for (int i = 0; i < rows; i++)
        {
            det *= lu.U[i, i];
        }
        return det;
    }

    public static T[,] Invert<T>(this T[,] matrix) where T : IFloatingPoint<T>
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);

        (T[,] L, T[,] U) = MakeLU<T, T>(matrix);

        var invert = new T[rows, cols];


        for (int i = 0; i < rows; i++)
        {
            var Ei = SpecialMatrix.Zeros<T>(rows);

            Ei[i] = T.CreateChecked(1);

            var col = Solve<T, T>(matrix, Ei);

            for (int j = 0; j < invert.GetLength(1); i++)
            {
                invert[j, i] = col[j];
            }

        }

        return invert;
    }

    // Function solves Ax = v 
    public static TResult[] Solve<T, TResult>( this T[,] matrix, T[] vector)
                                                where T : IFloatingPoint<T>
                                                where TResult : IFloatingPoint<TResult>
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        var elems = vector.Length;

        if (rows != cols)
            throw new Exception("The matrix is not square!");

        if (rows != elems)
            throw new Exception("Wrong number of results in solution vector!");

        (TResult[,] L, TResult[,] U) = MakeLU<T, TResult>(matrix);

        var b = new TResult[rows];

        for (int i = 0; i < vector.Length; i++)
        {
            b[i] = TResult.CreateChecked(vector[i]);
        }

        var z = SubsForth<TResult>(L, b);

        var x = SubsBack<TResult>(U, z);

        return x;
    }

    // Function solves Ax = b for A as a lower triangular matrix
    public static T[] SubsForth<T>(this T[,] L, T[] b) where T : IFloatingPoint<T>
    {

        int n = L.Length;

        T[] x = new T[n];

        for (int i = 0; i < n; i++)
        {
            x[i] = b[i];

            for (int j = 0; j < i; j++)
            {
                x[i] -= L[i, j] * x[j];
            }

            x[i] = x[i] / L[i, i];
        }

        return x;
    }

    // Function solves Ax = b for A as an upper triangular matrix
    public static T[] SubsBack<T>(this T[,] U, T[] b) where T : IFloatingPoint<T>
    {
        int n = U.Length;
        T[] x = new T[n];

        for (int i = n - 1; i > -1; i--)
        {
            x[i] = b[i];

            for (int j = n - 1; j > i; j--)
            {
                x[i] -= U[i, j] * x[j];
            }

            x[i] = x[i] / U[i, i];
        }
        return x;
    }

   public static (TResult[,] L, TResult[,] U) MakeLU<T, TResult>(this T[,] matrix)
                                                                        where T : IFloatingPoint<T>
                                                                        where TResult : IFloatingPoint<TResult>
                                                                       
    {
        if (matrix.GetLength(0) != matrix.GetLength(1))
        {
            throw new Exception("The matrix is not squared.");
        }

        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);

        var L = SpecialMatrix.Identity<TResult>(rows, cols);

        var U = new TResult[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                U[i, j] = TResult.CreateChecked(matrix[i, j]);
            }
        }


        var vector = new int[rows];

        for (int i = 0; i < rows; i++)
        {
            vector[i] = i;
        }


        TResult p = default;
        TResult pom2;
        int k0 = 0;
        int pom1 = 0;
        int detOfP = 1;

        for (int k = 0; k < cols - 1; k++)
        {
            p = default;
            for (int i = k; i < rows; i++)      // find the row with the biggest pivot
            {
                if (TResult.Abs(U[i, k]) > p)
                {
                    p = TResult.Abs(U[i, k]);

                    k0 = i;
                }
            }
            if (p == default)
                throw new Exception("The matrix is singular!");

            pom1 = vector[k]; vector[k] = vector[k0]; vector[k0] = pom1;    // switch two rows in permutation matrix

            for (int i = 0; i < k; i++)
            {
                pom2 = L[k, i]; L[k, i] = L[k0, i]; L[k0, i] = pom2;
            }

            if (k != k0)
            {
                detOfP *= -1;
            }

            for (int i = 0; i < cols; i++)
            {
                pom2 = U[k, i]; U[k, i] = U[k0, i]; U[k0, i] = pom2;
            }

            for (int i = k + 1; i < rows; i++)
            {
                L[i, k] = U[i, k] / U[k, k];

                for (int j = k; j < cols; j++)

                    U[i, j] = U[i, j] - L[i, k] * U[k, j];
            }
        }

        return (L, U);
    }
}

#endif
