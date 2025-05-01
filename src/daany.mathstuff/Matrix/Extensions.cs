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
using System.Linq;
using System;
using System.Numerics;

namespace Daany.MathStuff.MatrixGeneric;

/// <summary>
/// Provides extension methods for matrix operations on 2D arrays and jagged arrays.
/// </summary>
public static class MatrixExtensions
{
	/// <summary>
	/// Gets a column from a 2D matrix as a 1D array.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <param name="index">The zero-based column index (negative index counts from the end).</param>
	/// <returns>An array containing the column elements.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	/// <exception cref="IndexOutOfRangeException">Thrown when index is out of bounds.</exception>
	public static T[] GetColumn<T>(this T[,] matrix, int index)
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);

		if (index < 0)
		{
			index = cols + index;
		}

		if (index < 0 || index >= cols)
			throw new IndexOutOfRangeException("Column index is out of range.");

		T[] result = new T[rows];

		for (int i = 0; i < rows; i++)
		{
			result[i] = matrix[i, index];
		}

		return result;
	}

	/// <summary>
	/// Gets a column from a jagged matrix as a 1D array.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <param name="index">The zero-based column index (negative index counts from the end).</param>
	/// <returns>An array containing the column elements.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null or empty.</exception>
	/// <exception cref="IndexOutOfRangeException">Thrown when index is out of bounds.</exception>
	public static T[] GetColumn<T>(this T[][] matrix, int index)
	{
		if (matrix == null || matrix.Length == 0)
			throw new ArgumentNullException(nameof(matrix));

		int cols = matrix[0].Length;

		if (index < 0)
		{
			index = cols + index;
		}

		if (index < 0 || index >= cols)
			throw new IndexOutOfRangeException("Column index is out of range.");

		T[] result = new T[matrix.Length];

		for (int i = 0; i < result.Length; i++)
		{
			result[i] = matrix[i][index];
		}

		return result;
	}

	/// <summary>
	/// Gets a row from a 2D matrix as a 1D array.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <param name="index">The zero-based row index (negative index counts from the end).</param>
	/// <returns>An array containing the row elements.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	/// <exception cref="IndexOutOfRangeException">Thrown when index is out of bounds.</exception>
	public static T[] GetRow<T>(this T[,] matrix, int index)
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);

		if (index < 0)
		{
			index = rows + index;
		}

		if (index < 0 || index >= rows)
			throw new IndexOutOfRangeException("Row index is out of range.");

		T[] result = new T[cols];

		for (int i = 0; i < cols; i++)
		{
			result[i] = matrix[index, i];
		}

		return result;
	}

	/// <summary>
	/// Gets a row from a jagged matrix as a 1D array.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <param name="index">The zero-based row index (negative index counts from the end).</param>
	/// <returns>An array containing the row elements.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null or empty.</exception>
	/// <exception cref="IndexOutOfRangeException">Thrown when index is out of bounds.</exception>
	public static T[] GetRow<T>(this T[][] matrix, int index)
	{
		if (matrix == null || matrix.Length == 0)
			throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.Length;

		if (index < 0)
		{
			index = rows + index;
		}

		if (index < 0 || index >= rows)
			throw new IndexOutOfRangeException("Row index is out of range.");

		return matrix[index].ToArray(); // Return a copy to prevent modification of original
	}

	/// <summary>
	/// Gets the diagonal elements from a 2D matrix.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <returns>An array containing the diagonal elements.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[] GetDiagonal<T>(this T[,] matrix)
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));

		int size = Math.Min(matrix.GetLength(0), matrix.GetLength(1));
		T[] result = new T[size];

		for (int i = 0; i < size; i++)
		{
			result[i] = matrix[i, i];
		}

		return result;
	}

	/// <summary>
	/// Gets the diagonal elements from a jagged matrix.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <returns>An array containing the diagonal elements.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null or empty.</exception>
	public static T[] GetDiagonal<T>(this T[][] matrix)
	{
		if (matrix == null || matrix.Length == 0)
			throw new ArgumentNullException(nameof(matrix));

		int size = Math.Min(matrix.Length, matrix[0].Length);
		T[] result = new T[size];

		for (int i = 0; i < size; i++)
		{
			result[i] = matrix[i][i];
		}

		return result;
	}

	/// <summary>
	/// Creates a diagonal matrix from a 1D array.
	/// </summary>
	/// <typeparam name="T">The type of elements in the array.</typeparam>
	/// <param name="values">The diagonal values.</param>
	/// <returns>A square matrix with the given values on its diagonal.</returns>
	/// <exception cref="ArgumentNullException">Thrown when values is null.</exception>
	public static T[,] MakeDiagonal<T>(this T[] values)
	{
		if (values == null)
			throw new ArgumentNullException(nameof(values));

		int size = values.Length;
		T[,] result = new T[size, size];

		for (int i = 0; i < size; i++)
		{
			result[i, i] = values[i];
		}

		return result;
	}

	/// <summary>
	/// Creates a deep copy of a 2D matrix.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The matrix to copy.</param>
	/// <returns>A new matrix with the same elements.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[,] Copy<T>(this T[,] matrix)
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));

		return (T[,])matrix.Clone();
	}

	/// <summary>
	/// Reverses the order of rows or columns in a 2D matrix.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <param name="row">True to reverse rows, false to reverse columns.</param>
	/// <returns>A new matrix with reversed rows or columns.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[,] Reverse<T>(this T[,] matrix, bool row)
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);
		T[,] result = new T[rows, cols];

		for (int i = 0; i < rows; i++)
		{
			int sourceRow = row ? rows - i - 1 : i;

			for (int j = 0; j < cols; j++)
			{
				int sourceCol = row ? j : cols - j - 1;
				result[i, j] = matrix[sourceRow, sourceCol];
			}
		}

		return result;
	}

	/// <summary>
	/// Reverses the order of rows or columns in a jagged matrix.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <param name="row">True to reverse rows, false to reverse columns.</param>
	/// <returns>A new matrix with reversed rows or columns.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null or empty.</exception>
	public static T[][] Reverse<T>(this T[][] matrix, bool row)
	{
		if (matrix == null || matrix.Length == 0)
			throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.Length;
		int cols = matrix[0].Length;
		T[][] result = new T[rows][];

		for (int i = 0; i < rows; i++)
		{
			int sourceRow = row ? rows - i - 1 : i;
			result[i] = new T[cols];

			for (int j = 0; j < cols; j++)
			{
				int sourceCol = row ? j : cols - j - 1;
				result[i][j] = matrix[sourceRow][sourceCol];
			}
		}

		return result;
	}

	/// <summary>
	/// Converts a jagged array to a 2D matrix.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source jagged array.</param>
	/// <param name="transpose">True to transpose the matrix during conversion.</param>
	/// <returns>A 2D matrix.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[,] ToMatrix<T>(this T[][] matrix, bool transpose = false)
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));

		if (matrix.Length == 0)
		{
			return new T[0, 0];
		}

		int rows = matrix.Length;
		int cols = matrix[0].Length;
		T[,] result = new T[transpose ? cols : rows, transpose ? rows : cols];

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				if (transpose)
				{
					result[j, i] = matrix[i][j];
				}
				else
				{
					result[i, j] = matrix[i][j];
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Converts a 2D matrix to a jagged array.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source 2D matrix.</param>
	/// <returns>A jagged array.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[][] ToMatrix<T>(this T[,] matrix)
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);
		T[][] result = new T[rows][];

		for (int i = 0; i < rows; i++)
		{
			result[i] = new T[cols];
			for (int j = 0; j < cols; j++)
			{
				result[i][j] = matrix[i, j];
			}
		}

		return result;
	}

	/// <summary>
	/// Converts a vector to a 2D matrix with specified number of rows.
	/// </summary>
	/// <typeparam name="T">The type of elements in the vector.</typeparam>
	/// <param name="vector">The source vector.</param>
	/// <param name="nRows">The number of rows in the resulting matrix.</param>
	/// <returns>A 2D matrix with the vector elements arranged in rows.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	/// <exception cref="ArgumentException">Thrown when vector length is not divisible by nRows.</exception>
	public static T[,] ToMatrix<T>(this T[] vector, int nRows)
	{
		if (vector == null)
			throw new ArgumentNullException(nameof(vector));
		if (nRows <= 0)
			throw new ArgumentException("Number of rows must be positive.", nameof(nRows));
		if (vector.Length % nRows != 0)
			throw new ArgumentException("Vector length must be divisible by number of rows.", nameof(nRows));

		int cols = vector.Length / nRows;
		T[,] result = new T[nRows, cols];

		for (int i = 0; i < vector.Length; i++)
		{
			result[i / cols, i % cols] = vector[i];
		}

		return result;
	}

	/// <summary>
	/// Converts a vector to a 2D matrix as either a column or row vector.
	/// </summary>
	/// <typeparam name="T">The type of elements in the vector.</typeparam>
	/// <param name="vector">The source vector.</param>
	/// <param name="asColumnVector">True to create a column vector, false for row vector.</param>
	/// <returns>A 2D matrix representing the vector.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	public static T[,] ToMatrix<T>(this T[] vector, bool asColumnVector = false)
	{
		if (vector == null)
			throw new ArgumentNullException(nameof(vector));

		if (asColumnVector)
		{
			T[,] result = new T[vector.Length, 1];
			for (int i = 0; i < vector.Length; i++)
			{
				result[i, 0] = vector[i];
			}
			return result;
		}
		else
		{
			T[,] result = new T[1, vector.Length];
			for (int i = 0; i < vector.Length; i++)
			{
				result[0, i] = vector[i];
			}
			return result;
		}
	}

	/// <summary>
	/// Transposes a 2D matrix (rows become columns and vice versa).
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <returns>The transposed matrix.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[,] Transpose<T>(this T[,] matrix)
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);
		T[,] result = new T[cols, rows];

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				result[j, i] = matrix[i, j];
			}
		}

		return result;
	}

	/// <summary>
	/// Transposes a vector into a column matrix (2D array with single column).
	/// </summary>
	/// <typeparam name="T">The type of elements in the vector.</typeparam>
	/// <param name="vector">The source vector.</param>
	/// <returns>A 2D column matrix representation of the vector.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	public static T[,] Transpose<T>(this T[] vector)
	{
		if (vector == null)
			throw new ArgumentNullException(nameof(vector));

		T[,] result = new T[vector.Length, 1];

		for (int i = 0; i < vector.Length; i++)
		{
			result[i, 0] = vector[i];
		}

		return result;
	}

	/// <summary>
	/// Transposes a jagged array matrix.
	/// </summary>
	/// <typeparam name="T">The type of elements in the matrix.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <returns>The transposed matrix.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null or empty.</exception>
	/// <exception cref="ArgumentException">Thrown when matrix is not rectangular.</exception>
	public static T[][] Transpose<T>(this T[][] matrix)
	{
		if (matrix == null || matrix.Length == 0)
			throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.Length;
		int cols = matrix[0].Length;

		// Verify rectangular matrix
		for (int i = 1; i < rows; i++)
		{
			if (matrix[i] == null || matrix[i].Length != cols)
				throw new ArgumentException("Jagged array must be rectangular to transpose.", nameof(matrix));
		}

		T[][] result = new T[cols][];
		for (int i = 0; i < cols; i++)
		{
			result[i] = new T[rows];
			for (int j = 0; j < rows; j++)
			{
				result[i][j] = matrix[j][i];
			}
		}

		return result;
	}

	/// <summary>
	/// Calculates the determinant of a square matrix.
	/// </summary>
	/// <typeparam name="T">The floating-point type of matrix elements.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <returns>The determinant of the matrix.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	/// <exception cref="ArgumentException">Thrown when matrix is not square.</exception>
	public static T Determinant<T>(this T[,] matrix) where T : IFloatingPoint<T>
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));
		if (matrix.GetLength(0) != matrix.GetLength(1))
			throw new ArgumentException("Matrix must be square to calculate determinant.", nameof(matrix));

		var (_, U) = matrix.MakeLU<T, T>();
		T det = T.One;

		for (int i = 0; i < matrix.GetLength(0); i++)
		{
			det *= U[i, i];
		}

		return det;
	}

	/// <summary>
	/// Inverts a square matrix.
	/// </summary>
	/// <typeparam name="T">The floating-point type of matrix elements.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <returns>The inverted matrix.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	/// <exception cref="ArgumentException">Thrown when matrix is not square or is singular.</exception>
	public static T[,] Invert<T>(this T[,] matrix) where T : IFloatingPoint<T>
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));
		if (matrix.GetLength(0) != matrix.GetLength(1))
			throw new ArgumentException("Matrix must be square to invert.", nameof(matrix));

		int n = matrix.GetLength(0);
		var inv = new T[n, n];

		var (L, U) = matrix.MakeLU<T, T>();

		for (int col = 0; col < n; col++)
		{
			T[] e = new T[n];
			e[col] = T.One;

			T[] y = SubsForth(L, e);
			T[] x = SubsBack(U, y);

			for (int row = 0; row < n; row++)
			{
				inv[row, col] = x[row];
			}
		}

		return inv;
	}

	/// <summary>
	/// Solves a system of linear equations Ax = b.
	/// </summary>
	/// <typeparam name="T">The floating-point type of matrix elements.</typeparam>
	/// <typeparam name="TResult">The floating-point type of result elements.</typeparam>
	/// <param name="matrix">The coefficient matrix A.</param>
	/// <param name="vector">The right-hand side vector b.</param>
	/// <returns>The solution vector x.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix or vector is null.</exception>
	/// <exception cref="ArgumentException">Thrown when matrix is not square or dimensions don't match.</exception>
	public static TResult[] Solve<T, TResult>(this T[,] matrix, T[] vector)
		where T : IFloatingPoint<T>
		where TResult : IFloatingPoint<TResult>
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));
		if (vector == null)
			throw new ArgumentNullException(nameof(vector));
		if (matrix.GetLength(0) != matrix.GetLength(1))
			throw new ArgumentException("Matrix must be square.", nameof(matrix));
		if (matrix.GetLength(0) != vector.Length)
			throw new ArgumentException("Matrix dimension must match vector length.", nameof(vector));

		var (L, U) = matrix.MakeLU<T, TResult>();

		TResult[] b = new TResult[vector.Length];
		for (int i = 0; i < vector.Length; i++)
		{
			b[i] = TResult.CreateChecked(vector[i]);
		}

		var y = SubsForth(L, b);
		var x = SubsBack(U, y);

		return x;
	}

	/// <summary>
	/// Performs forward substitution on a lower triangular matrix.
	/// </summary>
	/// <typeparam name="T">The floating-point type of matrix elements.</typeparam>
	/// <param name="L">Lower triangular matrix.</param>
	/// <param name="b">Right-hand side vector.</param>
	/// <returns>The solution vector.</returns>
	/// <exception cref="ArgumentNullException">Thrown when L or b is null.</exception>
	/// <exception cref="ArgumentException">Thrown when dimensions don't match.</exception>
	private static T[] SubsForth<T>(this T[,] L, T[] b) where T : IFloatingPoint<T>
	{
		if (L == null)
			throw new ArgumentNullException(nameof(L));
		if (b == null)
			throw new ArgumentNullException(nameof(b));
		if (L.GetLength(0) != L.GetLength(1))
			throw new ArgumentException("Matrix must be square.", nameof(L));
		if (L.GetLength(0) != b.Length)
			throw new ArgumentException("Matrix dimension must match vector length.", nameof(b));

		int n = b.Length;
		T[] x = new T[n];

		for (int i = 0; i < n; i++)
		{
			x[i] = b[i];
			for (int j = 0; j < i; j++)
			{
				x[i] -= L[i, j] * x[j];
			}
			x[i] /= L[i, i];
		}

		return x;
	}

	/// <summary>
	/// Performs backward substitution on an upper triangular matrix.
	/// </summary>
	/// <typeparam name="T">The floating-point type of matrix elements.</typeparam>
	/// <param name="U">Upper triangular matrix.</param>
	/// <param name="b">Right-hand side vector.</param>
	/// <returns>The solution vector.</returns>
	/// <exception cref="ArgumentNullException">Thrown when U or b is null.</exception>
	/// <exception cref="ArgumentException">Thrown when dimensions don't match.</exception>
	private static T[] SubsBack<T>(this T[,] U, T[] b) where T : IFloatingPoint<T>
	{
		if (U == null)
			throw new ArgumentNullException(nameof(U));
		if (b == null)
			throw new ArgumentNullException(nameof(b));
		if (U.GetLength(0) != U.GetLength(1))
			throw new ArgumentException("Matrix must be square.", nameof(U));
		if (U.GetLength(0) != b.Length)
			throw new ArgumentException("Matrix dimension must match vector length.", nameof(b));

		int n = b.Length;
		T[] x = new T[n];

		for (int i = n - 1; i >= 0; i--)
		{
			x[i] = b[i];
			for (int j = n - 1; j > i; j--)
			{
				x[i] -= U[i, j] * x[j];
			}
			x[i] /= U[i, i];
		}

		return x;
	}

	/// <summary>
	/// Performs LU decomposition of a square matrix.
	/// </summary>
	/// <typeparam name="T">The floating-point type of source matrix elements.</typeparam>
	/// <typeparam name="TResult">The floating-point type of result matrix elements.</typeparam>
	/// <param name="matrix">The matrix to decompose.</param>
	/// <returns>Tuple containing L (lower triangular) and U (upper triangular) matrices.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	/// <exception cref="ArgumentException">Thrown when matrix is not square.</exception>
	public static (TResult[,] L, TResult[,] U) MakeLU<T, TResult>(this T[,] matrix)
		where T : IFloatingPoint<T>
		where TResult : IFloatingPoint<TResult>
	{
		if (matrix == null)
			throw new ArgumentNullException(nameof(matrix));
		if (matrix.GetLength(0) != matrix.GetLength(1))
			throw new ArgumentException("Matrix must be square for LU decomposition.", nameof(matrix));

		int n = matrix.GetLength(0);
		var L = new TResult[n, n];
		var U = new TResult[n, n];
		int[] perm = new int[n];
		TResult detOfP = TResult.One;

		// Initialize permutation vector
		for (int i = 0; i < n; i++)
		{
			perm[i] = i;
		}

		// Initialize U as copy of input matrix
		for (int i = 0; i < n; i++)
		{
			for (int j = 0; j < n; j++)
			{
				U[i, j] = TResult.CreateChecked(matrix[i, j]);
			}
		}

		// Initialize L as identity matrix
		for (int i = 0; i < n; i++)
		{
			L[i, i] = TResult.One;
		}

		for (int k = 0; k < n - 1; k++)
		{
			// Partial pivoting
			TResult max = TResult.Abs(U[k, k]);
			int pivot = k;

			for (int i = k + 1; i < n; i++)
			{
				if (TResult.Abs(U[i, k]) > max)
				{
					max = TResult.Abs(U[i, k]);
					pivot = i;
				}
			}

			if (pivot != k)
			{
				// Swap rows in U
				for (int j = k; j < n; j++)
				{
					(U[k, j], U[pivot, j]) = (U[pivot, j], U[k, j]);
				}

				// Swap rows in L (only up to k-1)
				for (int j = 0; j < k; j++)
				{
					(L[k, j], L[pivot, j]) = (L[pivot, j], L[k, j]);
				}

				// Swap permutation vector
				(perm[k], perm[pivot]) = (perm[pivot], perm[k]);
				detOfP = -detOfP;
			}

			if (U[k, k] == TResult.Zero)
			{
				throw new ArgumentException("Matrix is singular and cannot be decomposed.", nameof(matrix));
			}

			// Gaussian elimination
			for (int i = k + 1; i < n; i++)
			{
				L[i, k] = U[i, k] / U[k, k];
				for (int j = k; j < n; j++)
				{
					U[i, j] -= L[i, k] * U[k, j];
				}
			}
		}

		return (L, U);
	}
}