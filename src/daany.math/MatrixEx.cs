using Daany.MathExt;
using System;
using System.Collections.Generic;
using System.Text;
using Daany.MathExt.MatrixExt;
namespace Daany.MathExt
{
    /// <summary>
    /// Matrix implementation based on 2D array type
    /// </summary>
    public static class MatrixEx
    {

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
            var retVal = new T[m1.GetLength(0), m1.GetLength(1)];
            for (int i = 0; i < m1.GetLength(0); i++)
                for (int j = 0; j < m1.GetLength(1); j++)
                    retVal[i, j] = m1[j, i];
            //
            return retVal;
        }

        public static double[,] Add(this double[,] m1, double[,] m2)
        {
            var retVal = new double[m1.GetLength(0), m2.GetLength(1)];
            for (int i = 0; i < m1.GetLength(0); i++)
                for (int j = 0; j < m2.GetLength(1); j++)
                    retVal[i,j]=m1[i,j]+m2[i,j];
            //
            return retVal;
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

        public static double[,] Sqrt(this double[,] m1)
        {

            double[,] result = new double[m1.GetLength(0), m1.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = Math.Sqrt(m1[i, j]);
            return result;
        }

        public static double[] Sqrt(this double[] m1)
        {

            double[] result = new double[m1.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Math.Sqrt(m1[i]);
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
    }
}
