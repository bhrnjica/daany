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

using Daany.MathStuff.Random;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Daany.MathStuff.MatrixGeneric;

/// <summary>
/// Provides utility methods for constructing various types of special matrices.
/// </summary>
public static class SpecialMatrix
{
	/// <summary>
	/// Creates a Hankel matrix from a given vector.
	/// </summary>
	/// <typeparam name="T">Numeric type implementing IFloatingPoint.</typeparam>
	/// <param name="vector">Input vector.</param>
	/// <param name="colCount">Number of columns in the resulting matrix.</param>
	/// <returns>Hankel matrix.</returns>
	/// <exception cref="ArgumentNullException">Thrown if vector is null.</exception>
	public static T[,] Hankel<T>(this T[] vector, int colCount = -1) where T : IFloatingPoint<T>
	{
		if (vector == null) throw new ArgumentNullException(nameof(vector));

		int n = vector.Length;
		int cols = colCount == -1 ? n : colCount;
		int rows = colCount == -1 ? n : n - cols + 1;
		var result = new T[rows, cols];

		for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
				result[i, j] = (i + j) < n ? vector[i + j] : default!;

		return result;
	}

	/// <summary>
	/// Creates a Toeplitz matrix from a given vector.
	/// </summary>
	public static T[,] Toeplitz<T>(this T[] vector) where T : IFloatingPoint<T>
	{
		int n = vector.Length;
		var result = new T[n, n];

		for (int i = 0; i < n; i++)
			for (int j = i; j < n; j++)
				result[i, j] = result[j, i] = vector[j - i];

		return result;
	}

	/// <summary>
	/// Creates a Circulant matrix from a given vector.
	/// </summary>
	/// <typeparam name="T">Numeric type implementing IFloatingPoint.</typeparam>
	/// <param name="vector">Input vector.</param>
	/// <returns>Circulant matrix.</returns>
	public static T[,] Circulant<T>(this T[] vector) where T : IFloatingPoint<T>
	{
		int n = vector.Length;
		var result = new T[n, n];

		for (int i = 0; i < n; i++)
			for (int j = 0; j < n; j++)
				result[i, j] = vector[(j - i + n) % n];

		return result;
	}

	/// <summary>
	/// Generates a zero matrix.
	/// </summary>
	public static TResult[,] Zeros<TResult>(int rows, int cols)
	{
		return new TResult[rows, cols];
	}

	/// <summary>
	/// Generates a zero vector with specified length.
	/// </summary>
	/// <typeparam name="T">Numeric type.</typeparam>
	/// <param name="length">Number of elements.</param>
	/// <returns>A vector filled with zero values.</returns>
	public static T[] Zeros<T>(int elements)
	{
		return new T[elements]; 
	}
	/// <summary>
	/// Generates a unit vector.
	/// </summary>
	public static TResult[] Unit<TResult>(int elements) where TResult : IFloatingPoint<TResult>
	{
		var result = new TResult[elements];
		Array.Fill(result, TResult.One);
		return result;
	}

	/// <summary>
	/// Generates an identity matrix.
	/// </summary>
	public static TResult[,] Identity<TResult>(int size) where TResult : IFloatingPoint<TResult>
	{
		var result = new TResult[size, size];
		for (int i = 0; i < size; i++)
			result[i, i] = TResult.One;

		return result;
	}

	/// <summary>
	/// Generates an identity matrix of given dimensions.
	/// </summary>
	/// <typeparam name="T">Numeric type implementing IFloatingPoint.</typeparam>
	/// <param name="rows">Number of rows in the matrix.</param>
	/// <param name="cols">Number of columns in the matrix.</param>
	/// <returns>Identity matrix with ones on the diagonal and zeros elsewhere.</returns>
	public static T[,] Identity<T>(int rows, int cols) where T : IFloatingPoint<T>
	{
		if (rows <= 0 || cols <= 0)
			throw new ArgumentException("Rows and columns must be greater than zero.");

		var result = new T[rows, cols];

		for (int i = 0; i < Math.Min(rows, cols); i++)
			result[i, i] = T.One; // Assigning 1 to diagonal elements

		return result;
	}

	/// <summary>
	/// Generates a random vector of floating-point values.
	/// </summary>
	public static T[] Rand<T>(int length, double min = 0, double max= 1) where T : IFloatingPoint<T>
	{
		var rand = Constant.rand;
		var result = new T[length];

		for (int i = 0; i < length; i++)
			result[i] = T.CreateChecked(rand.NextDouble() * (max - min) + min);

		return result;
	}

	/// <summary>
	/// Generates a sequence from start to stop with a step of 1.
	/// </summary>
	/// <typeparam name="T">Numeric type implementing IFloatingPoint.</typeparam>
	/// <param name="start">Starting value of the sequence.</param>
	/// <param name="stop">Ending value of the sequence.</param>
	/// <returns>Array representing the range from start to stop.</returns>
	public static T[] Arange<T>(T start, T stop) where T : IFloatingPoint<T>
	{
		return Arange(start, stop, T.One);
	}

	/// <summary>
	/// Generates a sequence from start to stop using a specified step value.
	/// </summary>
	/// <typeparam name="T">Numeric type implementing IFloatingPoint.</typeparam>
	/// <param name="start">Starting value of the sequence.</param>
	/// <param name="stop">Ending value of the sequence.</param>
	/// <param name="step">Increment value.</param>
	/// <returns>Array representing the range from start to stop.</returns>
	/// <exception cref="ArgumentException">Thrown if step is zero or start is greater than stop.</exception>
	public static T[] Arange<T>(T start, T stop, T step) where T : IFloatingPoint<T>
	{
		if (step <= T.Zero)
			throw new ArgumentException("Step value must be greater than zero.");

		if (start > stop)
			throw new ArgumentException("Start value must be less than or equal to stop value.");

		var list = new List<T>();
		for (T i = start; i < stop; i += step)
			list.Add(i);

		return list.ToArray();
	}

	/// <summary>
	/// Generates a sequence from 0 to stop with a step of 1.
	/// </summary>
	/// <typeparam name="T">Numeric type implementing IFloatingPoint.</typeparam>
	/// <param name="stop">Ending value of the sequence.</param>
	/// <returns>Array representing the range from 0 to stop.</returns>
	public static T[] Arange<T>(T stop) where T : IFloatingPoint<T>
	{
		return Arange(T.Zero, stop, T.One);
	}


	/// <summary>
	/// Generates a sequence with a fixed number of elements distributed evenly between two numbers.
	/// </summary>
	/// <typeparam name="T">Numeric type implementing IFloatingPoint.</typeparam>
	/// <param name="fromNumber">Starting value.</param>
	/// <param name="toNumber">Ending value.</param>
	/// <param name="count">Number of elements in the sequence.</param>
	/// <returns>Equally spaced numeric sequence.</returns>
	public static T[] NSeries<T>(T fromNumber, T toNumber, int count) where T : IFloatingPoint<T>
	{
		var result = new T[count];
		T step = (toNumber - fromNumber) / T.CreateChecked(count - 1);

		for (int i = 0; i < count; i++)
			result[i] = fromNumber + step * T.CreateChecked(i);

		return result;
	}

	/// <summary>
	/// Generates a sequence with constant values.
	/// </summary>
	/// <typeparam name="T">Numeric type implementing IFloatingPoint.</typeparam>
	/// <param name="value">Constant value.</param>
	/// <param name="count">Number of elements.</param>
	/// <returns>Array filled with a constant value.</returns>
	public static T[] ConstSeries<T>(T value, int count) where T : IFloatingPoint<T>
	{
		var result = new T[count];
		Array.Fill(result, value);
		return result;
	}

	/// <summary>
	/// Generates a date series by a fixed time interval.
	/// </summary>
	/// <param name="fromDate">Starting date.</param>
	/// <param name="toDate">Ending date.</param>
	/// <param name="span">Time interval between values.</param>
	/// <returns>Array of dates.</returns>
	public static DateTime[] DateSeries(DateTime fromDate, DateTime toDate, TimeSpan span)
	{
		var list = new List<DateTime>();
		for (DateTime dt = fromDate; dt <= toDate; dt += span)
			list.Add(dt);

		return list.ToArray();
	}

	/// <summary>
	/// Generates a monthly date series.
	/// </summary>
	/// <param name="fromDate">Starting date.</param>
	/// <param name="months">Number of months to increment.</param>
	/// <param name="count">Total number of dates.</param>
	/// <returns>Array of dates with monthly intervals.</returns>
	public static DateTime[] MonthlySeries(DateTime fromDate, int months, int count)
	{
		var result = new DateTime[count];

		for (int i = 0; i < count; i++)
			result[i] = fromDate.AddMonths(i * months);

		return result;
	}

	/// <summary>
	/// Generates a yearly date series.
	/// </summary>
	/// <param name="fromDate">Starting date.</param>
	/// <param name="years">Number of years to increment.</param>
	/// <param name="count">Total number of dates.</param>
	/// <returns>Array of dates with yearly intervals.</returns>
	public static DateTime[] YearlySeries(DateTime fromDate, int years, int count)
	{
		var result = new DateTime[count];

		for (int i = 0; i < count; i++)
			result[i] = fromDate.AddYears(i * years);

		return result;
	}

	/// <summary>
	/// Generates a sequence with fixed increments using a step value.
	/// </summary>
	/// <typeparam name="T">Numeric type implementing IFloatingPoint.</typeparam>
	/// <param name="fromNumber">Starting number.</param>
	/// <param name="toNumber">Ending number.</param>
	/// <param name="step">Increment value.</param>
	/// <returns>Array with stepped values.</returns>
	public static T[] SSeries<T>(T fromNumber, T toNumber, T step) where T : IFloatingPoint<T>
	{
		var list = new List<T>();
		for (T i = fromNumber; i < toNumber; i += step)
			list.Add(i);

		return list.ToArray();
	}

	public static T[] Generate<T>(int row, int col, T val) where T : IFloatingPoint<T> 
	{ 
		var size = row * col; 
		var obj = new T[size]; 
		for (int i = 0; i < size; i++) 
			obj[i] = val; return obj; 
	}
}

