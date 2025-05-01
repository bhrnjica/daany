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
using System.Numerics;

namespace Daany.MathStuff.MatrixGeneric;

/// <summary>
/// Provides generic mathematical operations for matrices and vectors.
/// </summary>
public static class Operations
{
	#region Basic Arithmetic Operations

	/// <summary>
	/// Adds two matrices element-wise.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix1">First matrix operand.</param>
	/// <param name="matrix2">Second matrix operand.</param>
	/// <returns>The resulting matrix after addition.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either matrix is null.</exception>
	/// <exception cref="ArgumentException">Thrown when matrices have different dimensions.</exception>
	public static T[,] Add<T>(this T[,] matrix1, T[,] matrix2) where T : IFloatingPoint<T>
	{
		if (matrix1 == null) throw new ArgumentNullException(nameof(matrix1));
		if (matrix2 == null) throw new ArgumentNullException(nameof(matrix2));

		int rows = matrix1.GetLength(0);
		int cols = matrix1.GetLength(1);

		if (rows != matrix2.GetLength(0) || cols != matrix2.GetLength(1))
			throw new ArgumentException("Matrices must have the same dimensions.");

		var result = new T[rows, cols];

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				result[i, j] = matrix1[i, j] + matrix2[i, j];
			}
		}

		return result;
	}

	/// <summary>
	/// Adds two vectors element-wise.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector1">First vector operand.</param>
	/// <param name="vector2">Second vector operand.</param>
	/// <returns>The resulting vector after addition.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either vector is null.</exception>
	/// <exception cref="ArgumentException">Thrown when vectors have different lengths.</exception>
	public static T[] Add<T>(this T[] vector1, T[] vector2) where T : IFloatingPoint<T>
	{
		if (vector1 == null) throw new ArgumentNullException(nameof(vector1));
		if (vector2 == null) throw new ArgumentNullException(nameof(vector2));
		if (vector1.Length != vector2.Length)
			throw new ArgumentException("Vectors must have the same length.");

		var result = new T[vector1.Length];

		for (int i = 0; i < result.Length; i++)
		{
			result[i] = vector1[i] + vector2[i];
		}

		return result;
	}

	/// <summary>
	/// Adds a scalar value to each element of a matrix.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix">The matrix operand.</param>
	/// <param name="scalar">The scalar value to add.</param>
	/// <returns>The resulting matrix after scalar addition.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[,] Add<T>(this T[,] matrix, T scalar) where T : IFloatingPoint<T>
	{
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);
		var result = new T[rows, cols];

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				result[i, j] = matrix[i, j] + scalar;
			}
		}

		return result;
	}

	/// <summary>
	/// Adds a scalar value to each element of a vector.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector">The vector operand.</param>
	/// <param name="scalar">The scalar value to add.</param>
	/// <returns>The resulting vector after scalar addition.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	public static T[] Add<T>(this T[] vector, T scalar) where T : IFloatingPoint<T>
	{
		if (vector == null) throw new ArgumentNullException(nameof(vector));

		var result = new T[vector.Length];

		for (int i = 0; i < result.Length; i++)
		{
			result[i] = vector[i] + scalar;
		}

		return result;
	}

	#endregion

	#region Subtraction Operations

	/// <summary>
	/// Subtracts one matrix from another element-wise.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix1">The matrix to subtract from.</param>
	/// <param name="matrix2">The matrix to subtract.</param>
	/// <returns>The resulting matrix after subtraction.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either matrix is null.</exception>
	/// <exception cref="ArgumentException">Thrown when matrices have different dimensions.</exception>
	public static T[,] Subtract<T>(this T[,] matrix1, T[,] matrix2) where T : IFloatingPoint<T>
	{
		if (matrix1 == null) throw new ArgumentNullException(nameof(matrix1));
		if (matrix2 == null) throw new ArgumentNullException(nameof(matrix2));

		int rows = matrix1.GetLength(0);
		int cols = matrix1.GetLength(1);

		if (rows != matrix2.GetLength(0) || cols != matrix2.GetLength(1))
			throw new ArgumentException("Matrices must have the same dimensions.");

		var result = new T[rows, cols];

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				result[i, j] = matrix1[i, j] - matrix2[i, j];
			}
		}

		return result;
	}

	/// <summary>
	/// Subtracts a scalar value from each element of a matrix.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix">The matrix operand.</param>
	/// <param name="scalar">The scalar value to subtract.</param>
	/// <returns>The resulting matrix after scalar subtraction.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[,] Subtract<T>(this T[,] matrix, T scalar) where T : IFloatingPoint<T>
	{
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);
		var result = new T[rows, cols];

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				result[i, j] = matrix[i, j] - scalar;
			}
		}

		return result;
	}

	/// <summary>
	/// Subtracts a scalar value from each element of a vector.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector">The vector operand.</param>
	/// <param name="scalar">The scalar value to subtract.</param>
	/// <returns>The resulting vector after scalar subtraction.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	public static T[] Subtract<T>(this T[] vector, T scalar) where T : IFloatingPoint<T>
	{
		if (vector == null) throw new ArgumentNullException(nameof(vector));

		var result = new T[vector.Length];

		for (int i = 0; i < result.Length; i++)
		{
			result[i] = vector[i] - scalar;
		}

		return result;
	}

	/// <summary>
	/// Subtracts one vector from another element-wise.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector1">The vector to subtract from.</param>
	/// <param name="vector2">The vector to subtract.</param>
	/// <returns>The resulting vector after subtraction.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either vector is null.</exception>
	/// <exception cref="ArgumentException">Thrown when vectors have different lengths.</exception>
	public static T[] Subtract<T>(this T[] vector1, T[] vector2) where T : IFloatingPoint<T>
	{
		if (vector1 == null) throw new ArgumentNullException(nameof(vector1));
		if (vector2 == null) throw new ArgumentNullException(nameof(vector2));
		if (vector1.Length != vector2.Length)
			throw new ArgumentException("Vectors must have the same length.");

		var result = new T[vector1.Length];

		for (int i = 0; i < vector1.Length; i++)
		{
			result[i] = vector1[i] - vector2[i];
		}

		return result;
	}

	#endregion

	#region Multiplication Operations

	/// <summary>
	/// Multiplies a matrix by a scalar value.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix">The matrix operand.</param>
	/// <param name="scalar">The scalar value to multiply by.</param>
	/// <returns>The resulting matrix after scalar multiplication.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[,] Multiply<T>(this T[,] matrix, T scalar) where T : IFloatingPoint<T>
	{
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);
		var result = new T[rows, cols];

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				result[i, j] = matrix[i, j] * scalar;
			}
		}

		return result;
	}

	/// <summary>
	/// Multiplies a vector by a scalar value.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector">The vector operand.</param>
	/// <param name="scalar">The scalar value to multiply by.</param>
	/// <returns>The resulting vector after scalar multiplication.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	public static T[] Multiply<T>(this T[] vector, T scalar) where T : IFloatingPoint<T>
	{
		if (vector == null) throw new ArgumentNullException(nameof(vector));

		var result = new T[vector.Length];

		for (int i = 0; i < vector.Length; i++)
		{
			result[i] = vector[i] * scalar;
		}

		return result;
	}

	/// <summary>
	/// Multiplies two vectors element-wise.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector1">First vector operand.</param>
	/// <param name="vector2">Second vector operand.</param>
	/// <returns>The resulting vector after element-wise multiplication.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either vector is null.</exception>
	/// <exception cref="ArgumentException">Thrown when vectors have different lengths.</exception>
	public static T[] Multiply<T>(this T[] vector1, T[] vector2) where T : IFloatingPoint<T>
	{
		if (vector1 == null) throw new ArgumentNullException(nameof(vector1));
		if (vector2 == null) throw new ArgumentNullException(nameof(vector2));
		if (vector1.Length != vector2.Length)
			throw new ArgumentException("Vectors must have the same length.");

		var result = new T[vector1.Length];

		for (int i = 0; i < vector1.Length; i++)
		{
			result[i] = vector1[i] * vector2[i];
		}

		return result;
	}

	#endregion

	#region Division Operations

	/// <summary>
	/// Divides each element of a matrix by a scalar value.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix">The matrix operand.</param>
	/// <param name="scalar">The scalar value to divide by.</param>
	/// <returns>The resulting matrix after scalar division.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	/// <exception cref="DivideByZeroException">Thrown when scalar is zero.</exception>
	public static T[,] Divide<T>(this T[,] matrix, T scalar) where T : IFloatingPoint<T>
	{
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));
		if (scalar == T.Zero)
			throw new DivideByZeroException("Scalar value cannot be zero.");

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);
		var result = new T[rows, cols];

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				result[i, j] = matrix[i, j] / scalar;
			}
		}

		return result;
	}

	/// <summary>
	/// Divides each element of a vector by a scalar value.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector">The vector operand.</param>
	/// <param name="scalar">The scalar value to divide by.</param>
	/// <returns>The resulting vector after scalar division.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	/// <exception cref="DivideByZeroException">Thrown when scalar is zero.</exception>
	public static T[] Divide<T>(this T[] vector, T scalar) where T : IFloatingPoint<T>
	{
		if (vector == null) throw new ArgumentNullException(nameof(vector));
		if (scalar == T.Zero)
			throw new DivideByZeroException("Scalar value cannot be zero.");

		var result = new T[vector.Length];

		for (int i = 0; i < vector.Length; i++)
		{
			result[i] = vector[i] / scalar;
		}

		return result;
	}

	/// <summary>
	/// Divides two vectors element-wise.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector1">The dividend vector.</param>
	/// <param name="vector2">The divisor vector.</param>
	/// <returns>The resulting vector after element-wise division.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either vector is null.</exception>
	/// <exception cref="ArgumentException">Thrown when vectors have different lengths.</exception>
	/// <exception cref="DivideByZeroException">Thrown when divisor vector contains zero.</exception>
	public static T[] Divide<T>(this T[] vector1, T[] vector2) where T : IFloatingPoint<T>
	{
		if (vector1 == null) throw new ArgumentNullException(nameof(vector1));
		if (vector2 == null) throw new ArgumentNullException(nameof(vector2));
		if (vector1.Length != vector2.Length)
			throw new ArgumentException("Vectors must have the same length.");

		var result = new T[vector1.Length];

		for (int i = 0; i < vector1.Length; i++)
		{
			if (vector2[i] == T.Zero)
				throw new DivideByZeroException($"Division by zero at index {i}.");

			result[i] = vector1[i] / vector2[i];
		}

		return result;
	}

	#endregion

	#region Dot Product and Matrix Multiplication

	/// <summary>
	/// Computes the dot product of a matrix and a vector.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix and vector elements.</typeparam>
	/// <param name="matrix">The matrix operand.</param>
	/// <param name="vector">The vector operand.</param>
	/// <returns>The resulting vector.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either matrix or vector is null.</exception>
	/// <exception cref="ArgumentException">Thrown when matrix columns don't match vector length.</exception>
	public static T[] Dot<T>(this T[,] matrix, T[] vector) where T : IFloatingPoint<T>
	{
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));
		if (vector == null) throw new ArgumentNullException(nameof(vector));
		if (matrix.GetLength(1) != vector.Length)
			throw new ArgumentException("Matrix columns must match vector length.");

		var result = new T[matrix.GetLength(0)];

		for (int i = 0; i < matrix.GetLength(0); i++)
		{
			T sum = T.Zero;
			for (int j = 0; j < vector.Length; j++)
			{
				sum += matrix[i, j] * vector[j];
			}
			result[i] = sum;
		}

		return result;
	}

	/// <summary>
	/// Computes the matrix multiplication of two matrices.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix1">First matrix operand.</param>
	/// <param name="matrix2">Second matrix operand.</param>
	/// <returns>The resulting matrix product.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either matrix is null.</exception>
	/// <exception cref="ArgumentException">Thrown when matrix dimensions are incompatible.</exception>
	public static T[,] Dot<T>(this T[,] matrix1, T[,] matrix2) where T : IFloatingPoint<T>
	{
		if (matrix1 == null) throw new ArgumentNullException(nameof(matrix1));
		if (matrix2 == null) throw new ArgumentNullException(nameof(matrix2));
		if (matrix1.GetLength(1) != matrix2.GetLength(0))
			throw new ArgumentException("Matrix1 columns must match matrix2 rows.");

		int m = matrix1.GetLength(0);
		int n = matrix2.GetLength(1);
		int p = matrix1.GetLength(1);
		var result = new T[m, n];

		for (int i = 0; i < m; i++)
		{
			for (int j = 0; j < n; j++)
			{
				T sum = T.Zero;
				for (int k = 0; k < p; k++)
				{
					sum += matrix1[i, k] * matrix2[k, j];
				}
				result[i, j] = sum;
			}
		}

		return result;
	}

	/// <summary>
	/// Computes the dot product of a vector and a matrix.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector and matrix elements.</typeparam>
	/// <param name="vector">The vector operand.</param>
	/// <param name="matrix">The matrix operand.</param>
	/// <returns>The resulting vector.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either vector or matrix is null.</exception>
	/// <exception cref="ArgumentException">Thrown when vector length doesn't match matrix rows.</exception>
	public static T[] Dot<T>(this T[] vector, T[,] matrix) where T : IFloatingPoint<T>
	{
		if (vector == null) throw new ArgumentNullException(nameof(vector));
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));
		if (vector.Length != matrix.GetLength(0))
			throw new ArgumentException("Vector length must match matrix rows.");

		var result = new T[matrix.GetLength(1)];

		for (int j = 0; j < matrix.GetLength(1); j++)
		{
			T sum = T.Zero;
			for (int k = 0; k < vector.Length; k++)
			{
				sum += vector[k] * matrix[k, j];
			}
			result[j] = sum;
		}

		return result;
	}

	#endregion

	#region Mathematical Functions

	/// <summary>
	/// Computes the square root of each element in a matrix.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix">The matrix operand.</param>
	/// <returns>A matrix with square roots of each element.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[,] Sqrt<T>(this T[,] matrix) where T : IFloatingPoint<T>, IRootFunctions<T>
	{
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);
		var result = new T[rows, cols];

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				result[i, j] = T.Sqrt(matrix[i, j]);
			}
		}

		return result;
	}

	/// <summary>
	/// Computes the square root of each element in a vector.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector">The vector operand.</param>
	/// <returns>A vector with square roots of each element.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	public static T[] Sqrt<T>(this T[] vector) where T : IFloatingPoint<T>, IRootFunctions<T>
	{
		if (vector == null) throw new ArgumentNullException(nameof(vector));

		var result = new T[vector.Length];

		for (int i = 0; i < vector.Length; i++)
		{
			result[i] = T.Sqrt(vector[i]);
		}

		return result;
	}

	/// <summary>
	/// Computes the natural logarithm of each element in a matrix.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix">The matrix operand.</param>
	/// <returns>A matrix with natural logarithms of each element.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[,] Log<T>(this T[,] matrix) where T : IFloatingPoint<T>, ILogarithmicFunctions<T>
	{
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);
		var result = new T[rows, cols];

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				result[i, j] = T.Log(matrix[i, j]);
			}
		}

		return result;
	}

	/// <summary>
	/// Computes the natural logarithm of each element in a vector.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector">The vector operand.</param>
	/// <returns>A vector with natural logarithms of each element.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	public static T[] Log<T>(this T[] vector) where T : IFloatingPoint<T>, ILogarithmicFunctions<T>
	{
		if (vector == null) throw new ArgumentNullException(nameof(vector));

		var result = new T[vector.Length];

		for (int i = 0; i < vector.Length; i++)
		{
			result[i] = T.Log(vector[i]);
		}

		return result;
	}

	/// <summary>
	/// Raises each element of a matrix to a specified power.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix">The matrix operand.</param>
	/// <param name="exponent">The exponent to raise each element to.</param>
	/// <returns>A matrix with each element raised to the specified power.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T[,] Pow<T>(this T[,] matrix, T exponent) where T : IFloatingPoint<T>, IPowerFunctions<T>
	{
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));

		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);
		var result = new T[rows, cols];

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				result[i, j] = T.Pow(matrix[i, j], exponent);
			}
		}

		return result;
	}

	/// <summary>
	/// Raises each element of a vector to a specified power.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector">The vector operand.</param>
	/// <param name="exponent">The exponent to raise each element to.</param>
	/// <returns>A vector with each element raised to the specified power.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	public static T[] Pow<T>(this T[] vector, T exponent) where T : IFloatingPoint<T>, IPowerFunctions<T>
	{
		if (vector == null) throw new ArgumentNullException(nameof(vector));

		var result = new T[vector.Length];

		for (int i = 0; i < vector.Length; i++)
		{
			result[i] = T.Pow(vector[i], exponent);
		}

		return result;
	}

	#endregion

	#region Vector Operations

	/// <summary>
	/// Gets a column from a matrix as a vector.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <param name="columnIndex">The zero-based index of the column to extract.</param>
	/// <returns>A vector containing the column elements.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	/// <exception cref="IndexOutOfRangeException">Thrown when columnIndex is out of range.</exception>
	public static T[] GetColumnVector<T>(this T[,] matrix, int columnIndex) where T : IFloatingPoint<T>
	{
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));
		if (columnIndex < 0 || columnIndex >= matrix.GetLength(1))
			throw new IndexOutOfRangeException("Column index is out of range.");

		var result = new T[matrix.GetLength(0)];

		for (int i = 0; i < result.Length; i++)
		{
			result[i] = matrix[i, columnIndex];
		}

		return result;
	}

	/// <summary>
	/// Gets a row from a matrix as a vector.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix">The source matrix.</param>
	/// <param name="rowIndex">The zero-based index of the row to extract.</param>
	/// <returns>A vector containing the row elements.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	/// <exception cref="IndexOutOfRangeException">Thrown when rowIndex is out of range.</exception>
	public static T[] GetRowVector<T>(this T[,] matrix, int rowIndex) where T : IFloatingPoint<T>
	{
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));
		if (rowIndex < 0 || rowIndex >= matrix.GetLength(0))
			throw new IndexOutOfRangeException("Row index is out of range.");

		var result = new T[matrix.GetLength(1)];

		for (int i = 0; i < result.Length; i++)
		{
			result[i] = matrix[rowIndex, i];
		}

		return result;
	}

	/// <summary>
	/// Computes the cumulative sum of a vector.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector">The vector operand.</param>
	/// <returns>A vector containing the cumulative sums.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	public static T[] CumulativeSum<T>(this T[] vector) where T : IFloatingPoint<T>
	{
		if (vector == null) throw new ArgumentNullException(nameof(vector));
		if (vector.Length == 0) return Array.Empty<T>();

		var result = new T[vector.Length];
		result[0] = vector[0];

		for (int i = 1; i < vector.Length; i++)
		{
			result[i] = result[i - 1] + vector[i];
		}

		return result;
	}

	#endregion

	#region Norm and Distance Operations

	/// <summary>
	/// Computes the Euclidean norm (L2 norm) of a vector.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector">The vector operand.</param>
	/// <returns>The Euclidean norm of the vector.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	public static T EuclideanNorm<T>(this T[] vector) where T : IFloatingPoint<T>, IRootFunctions<T>
	{
		return L2Norm(vector);
	}

	/// <summary>
	/// Computes the L2 norm of a vector.
	/// </summary>
	/// <typeparam name="T">The numeric type of the vector elements.</typeparam>
	/// <param name="vector">The vector operand.</param>
	/// <returns>The L2 norm of the vector.</returns>
	/// <exception cref="ArgumentNullException">Thrown when vector is null.</exception>
	public static T L2Norm<T>(this T[] vector) where T : IFloatingPoint<T>, IRootFunctions<T>
	{
		if (vector == null) throw new ArgumentNullException(nameof(vector));

		T sum = T.Zero;
		for (int i = 0; i < vector.Length; i++)
		{
			sum += vector[i] * vector[i];
		}

		return T.Sqrt(sum);
	}

	/// <summary>
	/// Computes the Frobenius norm (Euclidean norm for matrices) of a matrix.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix">The matrix operand.</param>
	/// <returns>The Frobenius norm of the matrix.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T FrobeniusNorm<T>(this T[,] matrix) where T : IFloatingPoint<T>, IRootFunctions<T>
	{
		return L2Norm(matrix);
	}

	/// <summary>
	/// Computes the L2 norm (Frobenius norm) of a matrix.
	/// </summary>
	/// <typeparam name="T">The numeric type of the matrix elements.</typeparam>
	/// <param name="matrix">The matrix operand.</param>
	/// <returns>The L2 norm of the matrix.</returns>
	/// <exception cref="ArgumentNullException">Thrown when matrix is null.</exception>
	public static T L2Norm<T>(this T[,] matrix) where T : IFloatingPoint<T>, IRootFunctions<T>
	{
		if (matrix == null) throw new ArgumentNullException(nameof(matrix));

		T sum = T.Zero;
		for (int i = 0; i < matrix.GetLength(0); i++)
		{
			for (int j = 0; j < matrix.GetLength(1); j++)
			{
				sum += matrix[i, j] * matrix[i, j];
			}
		}

		return T.Sqrt(sum);
	}

	#endregion
}