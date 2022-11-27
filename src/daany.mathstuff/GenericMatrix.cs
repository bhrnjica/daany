//////////////////////////////////////////////////////////////////////////////////////////
// Daany - DAta ANalYtics Library                                                        //
// https://github.com/bhrnjica/daany                                                    //
//                                                                                      //
// Copyright 2006-2018 Bahrudin Hrnjica                                                 //
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
using System.Data.Common;
using System.Numerics;

namespace Daany.MathStuff;

#if NET7_0_OR_GREATER

/// <summary>
/// Matrix implementation based on 2D array type
/// </summary>
public static class Matrix 
{
    public static int Rows<T>(T[] vector) 
    {
        return vector.Length;
    }

    public static int Rows<T>( T[,] matrix)
    {
        return matrix.GetLength(0);
    }

    public static int Columns<T>( T[,] matrix)
    {
        return matrix.GetLength(1);
    }

    public static T[ , ] Rand<T>(int row, int col) where T : INumber<T>
    {
        var size = row * col;
        var obj = new T[row, col];

        for (int i = 0; i < size; i++)
            for(int j=0; j< col; j++)
                obj[i, j] = T.CreateChecked( Constant.rand.NextDouble() );

        return obj;
    }

    public static T[,] Rand<T>(int row, int col, T min, T max) where T : INumber<T>
    {
        var size = row * col;
        var obj = new T[row, col];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < col; j++)
                obj[i, j] = T.CreateChecked(Constant.rand.NextDouble(Convert.ToDouble(min), Convert.ToDouble(max)));

        return obj;
    }

    public static T[] Rand<T>(int count) where T : INumber<T>
    {
        var obj = new T[ count ];

        for (int i = 0; i < count; i++)

            obj[ i ] = T.CreateChecked(Constant.rand.NextDouble());

        return obj;
    }

    public static T[] Rand<T>(int length, T min, T max) where T: INumber<T>
    {
        var obj = new T[length];
        for (int i = 0; i < length; i++)
            obj[i] = T.CreateChecked( Constant.rand.NextDouble(Convert.ToDouble( min ), Convert.ToDouble( max )) );
        return obj;
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

    public static T[] GetColumn<T>( T[,] m, int index) where T : INumber<T>
    {
        T[] result = new T[m.Rows()];

        if (index < 0)
        {
            index = m.GetLength(1) + index;
        }

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = m[i,index];
        }

        return result;
    }

    public static T[] GetRow<T>( T[,] matrix, int index) where T : INumber<T>
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

    public static T[] GetDiagonal<T>( T[ , ] matrix )
    {
        var rows = Rows<T> (matrix );
        var cols = Columns<T>( matrix );

        int size = Math.Min(rows, cols);
            
        var result = new T[ size ];

        for (int i = 0; i < size; i++)
            result[ i ] = matrix[ i, i];

        return result;
    }

    public static T[,] MakeDiagonal<T>(T[] values)
    {
        var result = new T[values.Length, values.Length];
        for (int i = 0; i < values.Length; i++)
            result[i, i] = values[i];
        return result;
    }

    public static T[,] Copy<T>( T[,] matrix )
    {
        return (T[,]) matrix.Clone();
    }

    public static T[,] Reverse<T>(T[,] matrix, bool row)
    {
        int r = matrix.GetLength(0);
        int c = matrix.GetLength(1);
            
        T[,] m = new T[r,c];
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

                m[i,j] = matrix[ii,jj];
            }
        }
        return m;
    }

    public static T[,] ToMatrix<T>( T[][] array, bool transpose = false)
    {
        int rows = array.Length;
        if (rows == 0)
            return new T[0, rows];

        int cols = array[0].Length;

        T[,] retVal;

        if (transpose)
        {
            retVal = new T[cols, rows];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    retVal[j, i] = array[i][j];
        }
        else
        {
            retVal = new T[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    retVal[i, j] = array[i][j];
        }

        return retVal;
    }

    public static T[,] ToMatrix<T>( T[] array, bool asColumnVector = false)
    {
        if (asColumnVector)
        {
            T[,] retVal = new T[array.Length, 1];
            for (int i = 0; i < array.Length; i++)
                retVal[i, 0] = array[i];
            return retVal;
        }
        else
        {
            T[,] retVal = new T[1, array.Length];
            for (int i = 0; i < array.Length; i++)
                retVal[0, i] = array[i];
            return retVal;
        }
    }

    public static T[,] ToMatrix<T>( T[] array, int nRows)
    {
        int rows = nRows;
        int cols = array.Length / rows;
        int k = 0;
        T[,] retVal = new T[rows,cols];
                
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                retVal[i, j] = array[k];
                k++;
                    
            }
        }

        return retVal;
    }

    public static T[][] ToMatrix<T>( T[,] array)
    {
        int rowsFirstIndex = array.GetLowerBound(0);
        int rowsLastIndex = array.GetUpperBound(0);
        int numberOfRows = rowsLastIndex - rowsFirstIndex + 1;

        int columnsFirstIndex = array.GetLowerBound(1);
        int columnsLastIndex = array.GetUpperBound(1);
        int numberOfColumns = columnsLastIndex - columnsFirstIndex + 1;

        T[][] m = new T[numberOfRows][];
        for (int i = 0; i < numberOfRows; i++)
        {
            m[i] = new T[numberOfColumns];

            for (int j = 0; j < numberOfColumns; j++)
            {
                m[i][j] = array[i + rowsFirstIndex, j + columnsFirstIndex];
            }
        }
        return m;
    }


    public static T[,] Transpose<T>( T[,] m1)
    {
        var retVal = new T[m1.GetLength(1), m1.GetLength(0)];
        for (int i = 0; i < retVal.GetLength(0); i++)
            for (int j = 0; j < retVal.GetLength(1); j++)
                retVal[i, j] = m1[j, i];
        //
        return retVal;
    }


    public static T[,] Transpose< T >( T[] vector)
    {
        var retVal = new T[vector.Length, 1];

        for (int i = 0; i < vector.Length; i++)
            retVal[i, 0] = vector[i];

        return retVal;
    }


    public static T[ , ] Invert< T >( T[,] matrix) where T : IFloatingPointIeee754<T>
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
         
        (T[,] L, T[,] U) = MakeLU<T, T>( matrix );

        var invert = new T[rows, cols];


        for (int i = 0; i < rows; i++)
        {
            var Ei = Zeros<T>( rows );

            Ei[i] = T.CreateChecked( 1 );

            var col = Solve<T, T >(matrix, Ei);

            //inv.SetCol(col, i);
            for (int j = 0; j < invert.GetLength(1); i++)
            {
                invert[j, i] = col[ j ];
            }

        }

        return invert;
    }

    public static TResult[] Solve <T, TResult >( T[,] matrix, T[] vector) where T : IFloatingPointIeee754<T> where TResult : IFloatingPointIeee754<TResult>                        // Function solves Ax = v in confirmity with solution vector "v"
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        var elems = vector.Length;

        if (rows != cols) 
            throw new MException("The matrix is not square!");

        if (rows != elems) 
            throw new MException("Wrong number of results in solution vector!");

        (TResult[,] L, TResult[,] U ) =  MakeLU< T, TResult >( matrix );

        var b = new TResult[rows];

        for (int i = 0; i < vector.Length; i++)
            b[i] = TResult.CreateChecked(vector[i]);

        var z = SubsForth<TResult>(L, b);

        var x = SubsBack<TResult>(U, z);

        return x;
    }

    public static T[ ] SubsForth< T >( T[,] L, T[] b) where T : IFloatingPointIeee754<T>          // Function solves Ax = b for A as a lower triangular matrix
    {

        int n = L.Length;

        T[] x = new T[ n ];

        for (int i = 0; i < n; i++)
        {
            x[ i ] = b[i ];

            for (int j = 0; j < i; j++)
                x[i] -= L[i, j] * x[j];

            x[i] = x[i] / L[i, i];
        }

        return x;
    }

    public static T[ ] SubsBack< T >(T[,] U, T[] b) where T : IFloatingPointIeee754<T>         // Function solves Ax = b for A as an upper triangular matrix
    {
        int n = U.Length;
        T[] x = new T[n];

        for (int i = n - 1; i > -1; i--)
        {
            x[i] = b[i];

            for (int j = n - 1; j > i; j--)
                x[i] -= U[i, j] * x[j];

            x[i] = x[i] / U[i, i];
        }
        return x;
    }

    public static (TResult[ , ] L, TResult[ , ] U) MakeLU< T, TResult >( T[ , ] matrix ) where T : IFloatingPointIeee754< T > where TResult : IFloatingPointIeee754<TResult>
{
        if (matrix.GetLength(0) != matrix.GetLength(1))
        {
            throw new Exception("The matrix is not squared.");
        }

        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);

        var L = Identity<TResult>(rows, cols);

        var U = new TResult[rows,cols ];

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                U[i, j] = TResult.CreateChecked( matrix[i, j] );


        var vector = new int[ rows ];

        for (int i = 0; i < rows; i++)
            vector[i] = i;

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
                throw new MException("The matrix is singular!");

            pom1 = vector[k]; vector[k] = vector[k0]; vector[k0] = pom1;    // switch two rows in permutation matrix

            for (int i = 0; i < k; i++)
            {
                pom2 = L[k, i]; L[k, i] = L[k0, i]; L[k0, i] = pom2;
            }

            if (k != k0)
                detOfP *= -1;

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

        return ( L, U );
    }

    public static double[] Div( double[] a, double b)
    {
        double[] r = new double[a.Length];
        for (int i = 0; i < a.Length; i++)
            r[i] = (a[i] / b);
        return r;
    }

    public static double[] Dot( double[] a, double b)
    {
        double[] r = new double[a.Length];
        for (int i = 0; i < a.Length; i++)
            r[i] = (a[i] * b);
        return r;
    }

    public static double[] Dot( double[,] m, double[] v)
    {
        if (m.GetLength(1) != v.Length)
            throw new Exception("Wrong dimensions of matrix or vector!");

        double[] result = new double[m.GetLength(0)];

        for (int i = 0; i < m.GetLength(0); i++)
            for (int j = 0; j < v.Length; j++)
                result[i] += m[i, j] * v[j];
        return result;
    }

    public static double[,] Dot( double[,] m1, double[,] m2)
    {

        if (m1.GetLength(1) != m2.GetLength(0)) 
            throw new Exception("Wrong dimensions of matrices!");

        double[,] result = new double[m1.GetLength(0), m2.GetLength(1)];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                for (int k = 0; k < m1.GetLength(1); k++)
                    result[i, j] += m1[i, k] * m2[k, j];
            }
                   
        }
                
        return result;
    }

    public static double[] Dot( double[] v, double[,] m1)
    {
        var result = new double[m1.Columns()];
        int cols = m1.Columns();
        for (int j = 0; j < cols; j++)
        {
            double s = (double)0;
            for (int k = 0; k < v.Length; k++)
                s += (double)((double)v[k] * (double)m1[k, j]);
            result[j] = (double)s;
        }
        return result;
    }
    //https://mathworld.wolfram.com/HankelMatrix.html
    public static double [,] Hankel( double[] v, int colCount=-1)
    {
        int N = v.Length;
        int L = colCount == -1 ? N : colCount;
        int K = colCount == -1 ? N : N - L + 1;
        var result = new double[L, K];

        for(int i = 0; i < L; i++)
        {
            for (int j = 0; j < K; j++)
            {
                result[i, j] = (i + j) > N-1 ? 0: v[i + j];
            }
        }

        return result;
    }
    //https://mathworld.wolfram.com/ToeplitzMatrix.html
    public static double[,] Toeplitz( double[] v)
    {
        int N = v.Length;
        var result = new double[N, N];

        for (int i = 0; i < N; i++)
        {
            for (int j = i; j < N; j++)
            {
                result[i, j] = v[j - i];
                result[j, i] = result[i, j];
            }
        }

        return result;
    }


    #region Accord Matrix Implementation

    /// <summary>
    /// Accord.Math
    ///   Gets the transpose of a matrix.
    /// </summary>
    public static T[,] Transpose<T>( T[,] matrix, bool inPlace)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        if (inPlace)
        {
            if (rows != cols)
                throw new ArgumentException("Only square matrices can be transposed in place.", "matrix");

            for (int i = 0; i < rows; i++)
            {
                for (int j = i; j < cols; j++)
                {
                    T element = matrix[j, i];
                    matrix[j, i] = matrix[i, j];
                    matrix[i, j] = element;
                }
            }

            return matrix;
        }
        else
        {
            T[,] result = new T[cols, rows];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[j, i] = matrix[i, j];

            return result;
        }
    }
    public static double[,] DotWithDiagonal( double[,] a, double[] b)
    {
        return DotWithDiagonal(a, b, new double[a.Rows(), b.Length]);
    }
    public static double[,] DotWithTransposed( double[,] a, double[,] b)
    {
        return DotWithTransposed(a, b, new double[a.Rows(), b.Rows()]);
    }

    /// <summary>
    ///  Accord.NET
    ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="diagonal">The diagonal vector of right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[,] DotWithDiagonal( double[,] a, double[] diagonal, double[,] result)
    {
        int rows = a.Rows();

        unsafe
        {
            fixed (double* ptrA = a)
            fixed (double* ptrR = result)
            {
                double* A = ptrA;
                double* R = ptrR;
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < diagonal.Length; j++)
                        *R++ = (double)((double)(*A++) * (double)diagonal[j]);
            }
        }

        return result;
    }
    /// <summary>
    /// Accord.NET
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and transpose of <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The transposed right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B'</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[,] DotWithTransposed( double[,] a, double[,] b, double[,] result)
    {
        int n = a.Columns();
        int m = a.Rows();
        int p = b.Rows();

        unsafe
        {
            fixed (double* A = a)
            fixed (double* B = b)
            fixed (double* R = result)
            {
                double* pr = R;
                for (int i = 0; i < m; i++)
                {
                    double* pb = B;
                    for (int j = 0; j < p; j++, pr++)
                    {
                        double* pa = A + n * i;

                        double s = (double)0;
                        for (int k = 0; k < n; k++)
                            s += (double)((double)(*pa++) * (double)(*pb++));
                        *pr = (double)s;
                    }
                }
            }
        }
        return result;
    }
    /// <summary>
    ///   Determines whether two matrices contain the same values.
    /// </summary>
    ///

    public static bool IsEqual( Double[,] a, Double[,] b, Double atol = 0, Double rtol = 0)
    {
        if (a == b)
            return true;
        if (a == null && b == null)
            return true;
        if (a == null ^ b == null)
            return false;
        int[] la = new int[a.GetLength(1)];
        int[] lb = new int[b.GetLength(1)];
        if (la.Length != lb.Length)
            return false;
        for (int i = 0; i < la.Length; i++)
            if (la[i] != lb[i])
                return false;

        unsafe
        {
            fixed (Double* ptrA = a)
            fixed (Double* ptrB = b)
            {
                if (rtol > 0)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = ptrB[i];
                        if (A == B)
                            continue;
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                            continue;
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                            return false;
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                            return false;
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                            return false;
                        var C = A;
                        var D = B;
                        var delta = Math.Abs(C - D);
                        if (C == 0)
                        {
                            if (delta <= rtol)
                                continue;
                        }
                        else if (D == 0)
                        {
                            if (delta <= rtol)
                                continue;
                        }

                        if (delta <= Math.Abs(C) * rtol)
                            continue;
                        return false;
                    }

                }
                else if (atol > 0)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = ptrB[i];
                        if (A == B)
                            continue;
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                            continue;
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                            return false;
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                            return false;
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                            return false;
                        var C = A;
                        var D = B;
                        if (Math.Abs(C - D) <= atol)
                            continue;
                        return false;
                    }

                }
                else
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        var A = ptrA[i];
                        var B = ptrB[i];
                        if (Double.IsNaN(A) && Double.IsNaN(B))
                            continue;
                        if (Double.IsNaN(A) ^ Double.IsNaN(B))
                            return false;
                        if (Double.IsPositiveInfinity(A) ^ Double.IsPositiveInfinity(B))
                            return false;
                        if (Double.IsNegativeInfinity(A) ^ Double.IsNegativeInfinity(B))
                            return false;
                        if (A != B)
                            return false;
                    }

                }
            }
        }

        return true;
    }

    /// <summary>
    ///   Creates a memberwise copy of a multidimensional matrix. Matrix elements
    ///   themselves are copied only in a shallowed manner (i.e. not cloned).
    /// </summary>
    /// 
    public static T[,] MemberwiseClone<T>( T[,] a)
    {
        // TODO: Rename to Copy and implement shallow and deep copies
        return (T[,])a.Clone();
    }

    /// <summary>
    ///   Creates a memberwise copy of a vector matrix. Vector elements
    ///   themselves are copied only in a shallow manner (i.e. not cloned).
    /// </summary>
    /// 
    public static T[] MemberwiseClone<T>( T[] a)
    {
        // TODO: Rename to Copy and implement shallow and deep copies
        return (T[])a.Clone();
    }
    #endregion

    public static double[,] Add( double[,] m1, double[,] m2)
    {
        var retVal = new double[m1.GetLength(0), m2.GetLength(1)];
        for (int i = 0; i < m1.GetLength(0); i++)
            for (int j = 0; j < m2.GetLength(1); j++)
                retVal[i, j] = m1[i, j] + m2[i, j];
        //
        return retVal;
    }

    public static double[] Add( double[] v1, double[] v2)
    {
        var result = new double[v1.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = v1[i] + v2[i];
        return result;
    }

    public static double[] Add( double[] m, double s)
    {
        var result = new double[m.Length];
        for (int i = 0; i < result.GetLength(0); i++)
            result[i] = m[i] + s;
        return result;
    }

    public static double[,] Add( double[,] m, double s)
    {
        var result = new double[m.GetLength(0), m.GetLength(1)];
        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = m[i, j] + s;
        return result;
    }

    public static double[,] Substract( double[,] m1, double[,] m2)
    {
        var retVal = new double[m1.GetLength(0), m2.GetLength(1)];
        for (int i = 0; i < m1.GetLength(0); i++)
            for (int j = 0; j < m2.GetLength(1); j++)
                retVal[i, j] = m1[i, j] - m2[i, j];
        //
        return retVal;
    }

    public static double[,] Substract( double[,] m, double s)
    {
        var result = new double[m.GetLength(0), m.GetLength(1)];
        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = m[i, j] - s;
        return result;
    }

    public static double[] Substract( double[] m, double s)
    {
        var result = new double[m.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = m[i] - s;
        return result;
    }
    public static double[] Substruct( double[] v, double[] v1)
    {
        var res = new double[v.Length];
        for (int i = 0; i < v.Length; i++)
            res[i] = v[i] - v1[i];

        return res;
    }
      
    public static double[,] Multiply( double[,] m, double s)
    {
        var result = new double[m.GetLength(0),m.GetLength(1)];
        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = m[i, j]*s;
        return result;
    }


    public static double[] Multiply( double[] v, double s)
    {
        var result = new double[v.Length];
        for (int i = 0; i < result.GetLength(0); i++)
            result[i] = v[i] * s;
        return result;
    }

    public static double[] Multiply( double[] v1, double[] v2)
    {
        var result = new double[v1.Length];
        for (int i = 0; i < v1.Length; i++)
            result[i] = v1[i] * v2[i];
        return result;
    }

    public static double[,] Divide( double[,] m, double s)
    {
        var result = new double[m.GetLength(0), m.GetLength(1)];
        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = m[i, j] / s;
        return result;
    }

    public static double[] Divide( double[] m, double s)
    {
        var result = new double[m.Length];
        for (int i = 0; i < result.GetLength(0); i++)
            result[i] = m[i] / s;
        return result;
    }

    public static double[] Divide( double[] v1, double[] v2)
    {
        var result = new double[v1.Length];
        for (int i = 0; i < v1.Length; i++)
            result[i] = v1[i] / v2[i];
        return result;
    }

    public static double[,] Sqrt( double[,] m1)
    {

        double[,] result = new double[m1.GetLength(0), m1.GetLength(1)];
        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = Math.Sqrt(m1[i, j]);
        return result;
    }

    public static double[] Sqrt( double[] v)
    {

        double[] result = new double[v.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = Math.Sqrt(v[i]);
        return result;
    }

    public static double[,] Log( double[,] m1)
    {

        double[,] result = new double[m1.GetLength(0), m1.GetLength(1)];
        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = Math.Log(m1[i, j]);
        return result;
    }

    public static double[] Log( double[] v)
    {

        double[] result = new double[v.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = Math.Log(v[i]);
        return result;
    }

    public static double[,] Pow( double[,] m1, double y)
    {

        double[,] result = new double[m1.GetLength(0), m1.GetLength(1)];
        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = Math.Pow(m1[i, j], y);
        return result;
    }

    public static double[] Pow( double[] v, double y)
    {

        double[] result = new double[v.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = Math.Pow(v[i], y);
        return result;
    }

    /// <summary>
    /// Returns the vector of matrix columnIndex
    /// </summary>
    /// <param name="m1"></param>
    /// <param name="colIndex"></param>
    /// <returns></returns>
    public static double[] CVector( double[,] m1, int colIndex)
    {

        double[] result = new double[m1.GetLength(0)];

        for (int i = 0; i < result.GetLength(0); i++)
            result[i] = m1[i, colIndex];
        return result;
    }

    /// <summary>
    /// Returns the vector of matrix rowIndex
    /// </summary>
    /// <param name="m1"></param>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    public static double[] RVector( double[,] m1, int rowIndex)
    {

        double[] result = new double[m1.GetLength(1)];

        for (int i = 0; i < result.GetLength(1); i++)
            result[i] = m1[rowIndex, i];
        return result;
    }

        
    public static double Euclidean( double[,] a)
    {
        return Norm2(a);
    }

    public static double Norm2( double[] v)
    {
        var retVal = 0.0;
        for (int i = 0; i < v.Length; i++)
            retVal += v[i] * v[i];

        return Math.Sqrt(retVal);
    }

    public static double Norm2( double[,] m)
    {
        var retVal = 0.0;
        for (int i = 0; i < m.GetLength(0); i++)
        {
            for (int j = 0; j < m.GetLength(1); j++)
            {
                retVal += m[i, j] * m[i, j];
            }
        }
        return Math.Sqrt(retVal);
    }

    public static double[] CumulativeSum( double[] vector)
    {
        if (vector == null || vector.Length == 0)
            return new double[0];

        var result = new double[vector.Length];
        result[0] = vector[0];
        for (int i = 1; i < vector.Length; i++)
            result[i] = (double)(result[i - 1] + vector[i]);
        return result;
    }
}

#endif
