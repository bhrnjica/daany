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
using Accord;
using Daany.MathExt.MatrixExt;
using System;

namespace Daany.MathExt
{
    /// <summary>
    /// Matrix implementation based on 2D array type
    /// </summary>
    public static class MatrixEx
    {
        public static int Rows<T>(this T[] vector)
        {
            return vector.Length;
        }

        public static int Rows<T>(this T[,] matrix)
        {
            return matrix.GetLength(0);
        }

        public static int Columns<T>(this T[,] matrix)
        {
            return matrix.GetLength(1);
        }

        public static T[] GetColumn<T>(this T[,] m, int index)
        {
            T[] result = new T[m.Rows()];
            //in case we have negative index
            if (index < 0)
                index = m.GetLength(1) + index;

            for (int i = 0; i < result.Length; i++)
                result[i] = m[i,index];

            return result;
        }

        public static T[,] Diagonal<T>(int rows, int cols, T[] values)
        {
            var result =  new T[rows, cols];
            int size = Math.Min(rows, Math.Min(cols, values.Length));
            for (int i = 0; i < size; i++)
                result[i, i] = values[i];
            return result;
        }

        public static T[,] Copy<T>(this T[,] a)
        {
            return (T[,])a.Clone();
        }

        public static T[,] ToMatrix<T>(this T[][] array, bool transpose = false)
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

        public static T[,] ToMatrix<T>(this T[] array, bool asColumnVector = false)
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

        public static T[,] Transpose<T>(this T[,] m1)
        {
            var retVal = new T[m1.GetLength(1), m1.GetLength(0)];
            for (int i = 0; i < retVal.GetLength(0); i++)
                for (int j = 0; j < retVal.GetLength(1); j++)
                    retVal[i, j] = m1[j, i];
            //
            return retVal;
        }

        public static double[,] Zeros(int rows, int cols)
        {
            var result = new double[rows, cols];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = 0;
            return result;
        }

        public static double[] Zeros(int elements)
        {
            var result = new double[elements];
            for (int i = 0; i < result.GetLength(0); i++)
                result[i] = 0;
            return result;
        }

        public static T[,] Diagonal<T>(T[] values)
        {
            var result = new T[values.Length, values.Length];
            for (int i = 0; i < values.Length; i++)
                result[i, i] = values[i];
            return result;
        }

        public static double[,] Identity(int rows, int cols)
        {
            var retVal = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if(i!=j)
                        retVal[i, j] = 0;
                    else
                        retVal[i, j] = 0;

                }               
            }
               
            //
            return retVal;
        }

        public static double[,] Invert(this double[,] m1)
        {
            var retVal = new double[m1.GetLength(0), m1.GetLength(1)];
            //Init matrix
            var mat = new Matrix(m1.GetLength(0), m1.GetLength(1));
            for (int i = 0; i < m1.GetLength(0); i++)
                for (int j = 0; j < m1.GetLength(1); j++)
                    mat[i, j] = m1[j, i];
            //calculate invert matrix
            var inMat= mat.Invert();

            for (int i = 0; i < m1.GetLength(0); i++)
                for (int j = 0; j < m1.GetLength(1); j++)
                    retVal[i, j] = inMat[j, i];
            //
            return retVal;
        }

        public static double[] Div(this double[] a, double b)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
                r[i] = (a[i] / b);
            return r;
        }

        public static double[] Dot(this double[] a, double b)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
                r[i] = (a[i] * b);
            return r;
        }

        public static double[] Dot(this double[,] m, double[] v)
        {
            if (m.GetLength(1) != v.Length)
                throw new Exception("Wrong dimensions of matrix or vector!");

            double[] result = new double[m.GetLength(0)];

            for (int i = 0; i < m.GetLength(0); i++)
                for (int j = 0; j < v.Length; j++)
                    result[i] += m[i, j] * v[j];
            return result;
        }

        public static double[,] Dot(this double[,] m1, double[,] m2)
        {

            if (m1.GetLength(1) != m2.GetLength(0)) throw new Exception("Wrong dimensions of matrix!");

            double[,] result = new double[m1.GetLength(0), m2.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    for (int k = 0; k < m1.GetLength(1); k++)
                        result[i, j] += m1[i, k] * m2[k, j];
            return result;
        }

        public static double[] Dot(this double[] v, double[,] m1)
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

        #region Accord Matrix Implementation

        /// <summary>
        /// Accord.Math
        ///   Gets the transpose of a matrix.
        /// </summary>
        public static T[,] Transpose<T>(this T[,] matrix, bool inPlace)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (inPlace)
            {
                if (rows != cols)
                    throw new ArgumentException("Only square matrices can be transposed in place.", "matrix");

#if DEBUG
                T[,] expected = matrix.Transpose();
#endif

                for (int i = 0; i < rows; i++)
                {
                    for (int j = i; j < cols; j++)
                    {
                        T element = matrix[j, i];
                        matrix[j, i] = matrix[i, j];
                        matrix[i, j] = element;
                    }
                }

//#if DEBUG
//                if (!expected.IsEqual(matrix))
//                    throw new Exception();
//#endif

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
        public static double[,] DotWithDiagonal(this double[,] a, double[] b)
        {
            return DotWithDiagonal(a, b, new double[a.Rows(), b.Length]);
        }
        public static double[,] DotWithTransposed(this double[,] a, double[,] b)
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
        public static double[,] DotWithDiagonal(this double[,] a, double[] diagonal, double[,] result)
        {
#if DEBUG
            var C = Dot(a.To<double[,]>(), MatrixEx.Diagonal(diagonal.To<double[]>()));
#endif
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
#if DEBUG
            if (!MatrixEx.IsEqual(C, result.To<double[,]>(), 1e-4))
                throw new Exception();
#endif
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
        public static double[,] DotWithTransposed(this double[,] a, double[,] b, double[,] result)
        {
#if DEBUG
            if (a.Columns() != b.Columns() || result.Rows() > a.Rows() || result.Columns() > b.Rows())
                throw new Exception("Dimension mismatch!");
            //
           var C = MatrixEx.Dot(a.To<double[,]>(), b.Transpose().To<double[,]>());
#endif
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
#if DEBUG
            if (!MatrixEx.IsEqual(C, result.To<double[,]>(), 1e-4))
                throw new Exception();
#endif
            return result;
        }
        /// <summary>
        ///   Determines whether two matrices contain the same values.
        /// </summary>
        ///

        public static bool IsEqual(this Double[,] a, Double[,] b, Double atol = 0, Double rtol = 0)
        {
            if (a == b)
                return true;
            if (a == null && b == null)
                return true;
            if (a == null ^ b == null)
                return false;
            int[] la = a.GetLength(true);
            int[] lb = b.GetLength(true);
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
        public static T[,] MemberwiseClone<T>(this T[,] a)
        {
            // TODO: Rename to Copy and implement shallow and deep copies
            return (T[,])a.Clone();
        }

        /// <summary>
        ///   Creates a memberwise copy of a vector matrix. Vector elements
        ///   themselves are copied only in a shallow manner (i.e. not cloned).
        /// </summary>
        /// 
        public static T[] MemberwiseClone<T>(this T[] a)
        {
            // TODO: Rename to Copy and implement shallow and deep copies
            return (T[])a.Clone();
        }
        #endregion

        public static double[,] Add(this double[,] m1, double[,] m2)
        {
            var retVal = new double[m1.GetLength(0), m2.GetLength(1)];
            for (int i = 0; i < m1.GetLength(0); i++)
                for (int j = 0; j < m2.GetLength(1); j++)
                    retVal[i, j] = m1[i, j] + m2[i, j];
            //
            return retVal;
        }
        public static double[] Add(this double[] v1, double[] v2)
        {
            var result = new double[v1.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = v1[i] + v2[i];
            return result;
        }
        public static double[] Add(this double[] m, double s)
        {
            var result = new double[m.Length];
            for (int i = 0; i < result.GetLength(0); i++)
                result[i] = m[i] + s;
            return result;
        }

        public static double[,] Add(this double[,] m, double s)
        {
            var result = new double[m.GetLength(0), m.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = m[i, j] + s;
            return result;
        }

        public static double[,] Substract(this double[,] m1, double[,] m2)
        {
            var retVal = new double[m1.GetLength(0), m2.GetLength(1)];
            for (int i = 0; i < m1.GetLength(0); i++)
                for (int j = 0; j < m2.GetLength(1); j++)
                    retVal[i, j] = m1[i, j] - m2[i, j];
            //
            return retVal;
        }

        public static double[,] Substract(this double[,] m, double s)
        {
            var result = new double[m.GetLength(0), m.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = m[i, j] - s;
            return result;
        }

        public static double[] Substract(this double[] m, double s)
        {
            var result = new double[m.Length];
            for (int i = 0; i < result.GetLength(0); i++)
               result[i] = m[i] - s;
            return result;
        }

       
        public static double[,] Multiply(this double[,] m, double s)
        {
            var result = new double[m.GetLength(0),m.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = m[i, j]*s;
            return result;
        }
        public static double[] Multiply(this double[] v, double s)
        {
            var result = new double[v.Length];
            for (int i = 0; i < result.GetLength(0); i++)
                result[i] = v[i] * s;
            return result;
        }

        public static double[] Multiply(this double[] v1, double[] v2)
        {
            var result = new double[v1.Length];
            for (int i = 0; i < v1.Length; i++)
                result[i] = v1[i] * v2[i];
            return result;
        }
        public static double[,] Divide(this double[,] m, double s)
        {
            var result = new double[m.GetLength(0), m.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = m[i, j] / s;
            return result;
        }
        public static double[] Divide(this double[] m, double s)
        {
            var result = new double[m.Length];
            for (int i = 0; i < result.GetLength(0); i++)
                result[i] = m[i] / s;
            return result;
        }

        public static double[] Divide(this double[] v1, double[] v2)
        {
            var result = new double[v1.Length];
            for (int i = 0; i < v1.Length; i++)
                result[i] = v1[i] / v2[i];
            return result;
        }
        public static double[,] Sqrt(this double[,] m1)
        {

            double[,] result = new double[m1.GetLength(0), m1.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = Math.Sqrt(m1[i, j]);
            return result;
        }

        public static double[] Sqrt(this double[] v)
        {

            double[] result = new double[v.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Math.Sqrt(v[i]);
            return result;
        }

        public static double[,] Pow(this double[,] m1, double y)
        {

            double[,] result = new double[m1.GetLength(0), m1.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = Math.Pow(m1[i, j], y);
            return result;
        }

        public static double[] Pow(this double[] v, double y)
        {

            double[] result = new double[v.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Math.Pow(v[i], y);
            return result;
        }
        public static double[] CVector(this double[,] m1, int colIndex)
        {

            double[] result = new double[m1.GetLength(0)];

            for (int i = 0; i < result.GetLength(0); i++)
                result[i] = m1[i, colIndex];
            return result;
        }

        public static double[] RVector(this double[,] m1, int rowIndex)
        {

            double[] result = new double[m1.GetLength(1)];

            for (int i = 0; i < result.GetLength(1); i++)
                result[i] = m1[rowIndex, i];
            return result;
        }

        public static double SquareEuclidean(this double[,] a)
        {
            double sum = 0;
            foreach (var v in a)
                sum += v * v;
            return sum;
        }

        public static double Euclidean(this double[,] a)
        {
            return (double)Math.Sqrt(SquareEuclidean(a));
        }

        public static double[] CumulativeSum(this double[] vector)
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
}
