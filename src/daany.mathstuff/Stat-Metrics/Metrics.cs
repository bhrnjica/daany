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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Daany.MathStuff.Stats;

/// <summary>
/// Provides statistical metrics and calculations for data analysis.
/// </summary>
public static class Metrics
{
	/// <summary>
	/// Calculates the sum of all elements in the collection.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement INumber{TResult}.</typeparam>
	/// <param name="colData">The collection of data to sum.</param>
	/// <returns>The sum of all elements in the collection.</returns>
	/// <exception cref="ArgumentNullException">Thrown when colData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when colData has fewer than 2 elements.</exception>
	public static TResult Sum<T, TResult>(IList<T> colData)
		where T : INumber<T>
		where TResult : INumber<TResult>
	{
		if (colData is null)
		{
			throw new ArgumentNullException(nameof(colData), "Collection data cannot be null.");
		}

		if (colData.Count < 2)
		{
			throw new ArgumentException("Collection should contain at least 2 elements.", nameof(colData));
		}

		TResult sum = TResult.Zero;

		for (int i = 0; i < colData.Count; i++)
		{
			sum += TResult.CreateChecked(colData[i]);
		}

		return sum;
	}

	/// <summary>
	/// Calculates the arithmetic mean of the elements in the collection.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement INumber{TResult}.</typeparam>
	/// <param name="colData">The collection of data to calculate the mean.</param>
	/// <returns>The arithmetic mean of the elements.</returns>
	/// <exception cref="ArgumentNullException">Thrown when colData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when colData has fewer than 2 elements.</exception>
	public static TResult Mean<T, TResult>(IList<T> colData)
		where T : INumber<T>
		where TResult : INumber<TResult>
	{
		if (colData is null)
		{
			throw new ArgumentNullException(nameof(colData), "Collection data cannot be null.");
		}

		if (colData.Count < 2)
		{
			throw new ArgumentException("Collection should contain at least 2 elements.", nameof(colData));
		}

		TResult sum = TResult.Zero;

		for (int i = 0; i < colData.Count; i++)
		{
			sum += TResult.CreateChecked(colData[i]);
		}

		return sum / TResult.CreateChecked(colData.Count);
	}

	/// <summary>
	/// Finds the mode (most frequently occurring value) in the collection.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection, which must implement INumber{T}.</typeparam>
	/// <param name="colData">The collection of data to find the mode.</param>
	/// <returns>The mode of the collection.</returns>
	/// <exception cref="ArgumentNullException">Thrown when colData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when colData has fewer than 2 elements.</exception>
	public static T Mode<T>(IList<T> colData) where T : INumber<T>
	{
		if (colData is null)
		{
			throw new ArgumentNullException(nameof(colData), "Collection data cannot be null.");
		}

		if (colData.Count < 2)
		{
			throw new ArgumentException("Collection should contain at least 2 elements.", nameof(colData));
		}

		var counts = new Dictionary<T, int>();

		foreach (var item in colData)
		{
			counts.TryGetValue(item, out var currentCount);
			counts[item] = currentCount + 1;
		}

		var result = counts.MaxBy(kvp => kvp.Value).Key;
		return result;
	}

	/// <summary>
	/// Calculates the median of the elements in the collection.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement INumber{TResult}.</typeparam>
	/// <param name="colData">The collection of data to calculate the median.</param>
	/// <returns>The median value of the collection.</returns>
	/// <exception cref="ArgumentNullException">Thrown when colData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when colData has fewer than 2 elements.</exception>
	public static TResult Median<T, TResult>(IList<T> colData)
		where T : INumber<T>
		where TResult : INumber<TResult>
	{
		if (colData is null)
		{
			throw new ArgumentNullException(nameof(colData), "Collection data cannot be null.");
		}

		if (colData.Count < 2)
		{
			throw new ArgumentException("Collection should contain at least 2 elements.", nameof(colData));
		}

		var orderedData = colData.Order().ToList();
		int middleIndex = orderedData.Count / 2;

		if (orderedData.Count % 2 == 1)
		{
			return TResult.CreateChecked(orderedData[middleIndex]);
		}

		var lowerValue = TResult.CreateChecked(orderedData[middleIndex - 1]);
		var upperValue = TResult.CreateChecked(orderedData[middleIndex]);
		return (lowerValue + upperValue) / TResult.CreateChecked(2);
	}

	/// <summary>
	/// Calculates the value at a specified percentile in the ordered collection.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection, which must implement INumber{T}.</typeparam>
	/// <param name="colData">The collection of data.</param>
	/// <param name="percentile">The percentile to calculate (0-100).</param>
	/// <returns>The value at the specified percentile.</returns>
	/// <exception cref="ArgumentNullException">Thrown when colData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when colData has fewer than 2 elements.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when percentile is not between 0 and 100.</exception>
	public static T Percentile<T>(IList<T> colData, double percentile) where T : INumber<T>
	{
		if (colData is null)
		{
			throw new ArgumentNullException(nameof(colData), "Collection data cannot be null.");
		}

		if (colData.Count < 2)
		{
			throw new ArgumentException("Collection should contain at least 2 elements.", nameof(colData));
		}

		if (percentile < 0 || percentile > 100)
		{
			throw new ArgumentOutOfRangeException(nameof(percentile), "Percentile must be between 0 and 100.");
		}

		var orderedData = colData.Order().ToList();

		if (percentile >= 100.0)
		{
			return orderedData[^1];
		}

		double position = (orderedData.Count + 1) * percentile / 100.0;
		double n = percentile / 100.0 * (orderedData.Count - 1) + 1.0;

		int lowerIndex = (int)Math.Floor(n) - 1;
		int upperIndex = (int)Math.Ceiling(n) - 1;

		if (lowerIndex < 0)
		{
			lowerIndex = 0;
			upperIndex = 1;
		}

		if (lowerIndex == upperIndex || orderedData[lowerIndex].Equals(orderedData[upperIndex]))
		{
			return orderedData[lowerIndex];
		}

		double fraction = n - Math.Floor(n);
		return orderedData[lowerIndex] + T.CreateChecked(fraction) * (orderedData[upperIndex] - orderedData[lowerIndex]);
	}

	/// <summary>
	/// Calculates the variance of the collection.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement INumber{TResult}.</typeparam>
	/// <param name="colData">The collection of data.</param>
	/// <param name="isSampleData">True if the data represents a sample; false for population.</param>
	/// <returns>The variance of the collection.</returns>
	/// <exception cref="ArgumentNullException">Thrown when colData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when colData has fewer than 2 elements.</exception>
	public static TResult Variance<T, TResult>(IList<T> colData, bool isSampleData = true)
		where T : INumber<T>
		where TResult : INumber<TResult>
	{
		if (colData is null)
		{
			throw new ArgumentNullException(nameof(colData), "Collection data cannot be null.");
		}

		if (colData.Count < 2)
		{
			throw new ArgumentException("Collection should contain at least 2 elements.", nameof(colData));
		}

		TResult mean = Mean<T, TResult>(colData);
		TResult sumOfSquares = TResult.Zero;

		foreach (var item in colData)
		{
			var deviation = TResult.CreateChecked(item) - mean;
			sumOfSquares += deviation * deviation;
		}

		var denominator = isSampleData ? colData.Count - 1 : colData.Count;
		return sumOfSquares / TResult.CreateChecked(denominator);
	}

	/// <summary>
	/// Calculates the covariance between two collections.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement INumber{TResult}.</typeparam>
	/// <param name="x">First collection.</param>
	/// <param name="y">Second collection.</param>
	/// <returns>The covariance between the two collections.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either x or y is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult Covariance<T, TResult>(IList<T> x, IList<T> y)
		where T : INumber<T>
		where TResult : INumber<TResult>
	{
		if (x is null)
		{
			throw new ArgumentNullException(nameof(x), "First collection cannot be null.");
		}

		if (y is null)
		{
			throw new ArgumentNullException(nameof(y), "Second collection cannot be null.");
		}

		if (x.Count != y.Count)
		{
			throw new ArgumentException("Collections must have the same length.");
		}

		if (x.Count < 2)
		{
			throw new ArgumentException("Collections should contain at least 2 elements.");
		}

		TResult meanX = Mean<T, TResult>(x);
		TResult meanY = Mean<T, TResult>(y);
		TResult sum = TResult.Zero;

		for (int i = 0; i < x.Count; i++)
		{
			sum += (TResult.CreateChecked(x[i]) - meanX) * (TResult.CreateChecked(y[i]) - meanY);
		}

		return sum / TResult.CreateChecked(x.Count - 1);
	}

	/// <summary>
	/// Calculates the covariance matrix for a set of collections.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement INumber{TResult}.</typeparam>
	/// <param name="data">The set of collections (matrix).</param>
	/// <returns>The covariance matrix.</returns>
	/// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
	/// <exception cref="ArgumentException">Thrown when data has fewer than 2 collections.</exception>
	public static TResult[,] CovMatrix<T, TResult>(IList<IList<T>> data)
		where T : INumber<T>
		where TResult : INumber<TResult>
	{
		if (data is null)
		{
			throw new ArgumentNullException(nameof(data), "Data cannot be null.");
		}

		if (data.Count < 2)
		{
			throw new ArgumentException("Data should contain at least 2 collections.", nameof(data));
		}

		int dimension = data.Count;
		var matrix = new TResult[dimension, dimension];

		for (int i = 0; i < dimension; i++)
		{
			for (int j = i; j < dimension; j++)
			{
				if (i == j)
				{
					matrix[i, j] = Variance<T, TResult>(data[i]);
				}
				else
				{
					var cov = Covariance<T, TResult>(data[i], data[j]);
					matrix[i, j] = cov;
					matrix[j, i] = cov;
				}
			}
		}

		return matrix;
	}

	/// <summary>
	/// Calculates the standard deviation of the collection.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement INumber{TResult}.</typeparam>
	/// <param name="colData">The collection of data.</param>
	/// <returns>The standard deviation of the collection.</returns>
	/// <exception cref="ArgumentNullException">Thrown when colData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when colData has fewer than 2 elements.</exception>
	public static TResult Stdev<T, TResult>(IList<T> colData)
		where T : INumber<T>
		where TResult : INumber<TResult>
	{
		if (colData is null)
		{
			throw new ArgumentNullException(nameof(colData), "Collection data cannot be null.");
		}

		if (colData.Count < 2)
		{
			throw new ArgumentException("Collection should contain at least 2 elements.", nameof(colData));
		}

		var variance = Variance<T, TResult>(colData);
		return TResult.CreateChecked(Sqrt(Convert.ToDouble(variance)));
	}

	/// <summary>
	/// Selects a random element from the collection.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	/// <param name="colData">The collection of data.</param>
	/// <param name="seed">Optional seed for the random number generator.</param>
	/// <returns>A randomly selected element from the collection.</returns>
	/// <exception cref="ArgumentNullException">Thrown when colData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when colData is empty.</exception>
	public static T Random<T>(IList<T> colData, int seed = 0)
	{
		if (colData is null)
		{
			throw new ArgumentNullException(nameof(colData), "Collection data cannot be null.");
		}

		if (colData.Count == 0)
		{
			throw new ArgumentException("Collection must not be empty.", nameof(colData));
		}

		var random = new System.Random(seed);
		return colData[random.Next(colData.Count)];
	}

	/// <summary>
	/// Calculates the frequency distribution of elements in the collection.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	/// <param name="colData">The collection of data.</param>
	/// <returns>A list of tuples containing each unique value and its count, ordered by descending frequency.</returns>
	/// <exception cref="ArgumentNullException">Thrown when colData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when colData has fewer than 2 elements.</exception>
	public static List<(T Value, int Count)> Frequency<T>(IList<T> colData)
	{
		if (colData is null)
		{
			throw new ArgumentNullException(nameof(colData), "Collection data cannot be null.");
		}

		if (colData.Count < 2)
		{
			throw new ArgumentException("Collection should contain at least 2 elements.", nameof(colData));
		}

		return colData
			.GroupBy(x => x)
			.Select(g => (g.Key, g.Count()))
			.OrderByDescending(x => x.Item2)
			.ToList();
	}

	/// <summary>
	/// Calculates the coefficient of determination (R²) between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement IFloatingPointIeee754{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>The R² value.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult RSquared<T, TResult>(IList<T> predictedData, IList<T> observedData)
	where T : INumber<T>
	where TResult : INumber<TResult>
	{
		var r = R<T, TResult>(predictedData, observedData);
		return r * r;
	}

	/// <summary>
	/// Calculates the Pearson correlation coefficient (R) between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement IFloatingPointIeee754{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>The Pearson correlation coefficient.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult R<T, TResult>(IList<T> predictedData, IList<T> observedData)
	where T : INumber<T>
	where TResult : INumber<TResult>
	{
		ValidateDataSets(observedData, predictedData);

		TResult meanObserved = Mean<T, TResult>(observedData);
		TResult meanPredicted = Mean<T, TResult>(predictedData);

		TResult covariance = TResult.Zero;
		TResult varianceObserved = TResult.Zero;
		TResult variancePredicted = TResult.Zero;

		for (int i = 0; i < observedData.Count; i++)
		{
			var obsDiff = TResult.CreateChecked(observedData[i]) - meanObserved;
			var predDiff = TResult.CreateChecked(predictedData[i]) - meanPredicted;

			covariance += obsDiff * predDiff;
			varianceObserved += obsDiff * obsDiff;
			variancePredicted += predDiff * predDiff;
		}

		// Special handling for decimal type
		if (typeof(TResult) == typeof(decimal))
		{
			decimal cov = (decimal)(object)covariance;
			decimal varObs = (decimal)(object)varianceObserved;
			decimal varPred = (decimal)(object)variancePredicted;
			decimal sqrt = SqrtDecimal(varObs * varPred);
			decimal result = cov / sqrt;
			return (TResult)(object)result;
		}

		// Default handling for other numeric types
		return covariance / Sqrt(varianceObserved * variancePredicted);
	}

	private static decimal SqrtDecimal(decimal x)
	{
		if (x < 0) throw new ArgumentException("Negative value");
		if (x == 0) return 0;

		decimal current = (decimal)Math.Sqrt((double)x); // Initial approximation
		decimal previous;
		do
		{
			previous = current;
			current = (previous + x / previous) / 2;
		} while (Math.Abs(previous - current) > 0.0m);

		return current;
	}

	private static TResult Sqrt<TResult>(TResult value) where TResult : INumber<TResult>
	{
		// For types that support ISqrtFunctions (C# 11+)
		if (typeof(TResult).GetMethod("Sqrt") != null)
		{
			dynamic val = value;
			return (TResult)Math.Sqrt(val);
		}

		// Fallback for other types
		return TResult.CreateChecked(Math.Sqrt(Convert.ToDouble(value)));
	}

	/// <summary>
	/// Calculates classification accuracy between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement INumber{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>The classification accuracy (0-1).</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult CA<T, TResult>(IList<T> predictedData, IList<T> observedData)
		where T : INumber<T>
		where TResult : INumber<TResult>
	{
		ValidateDataSets(observedData, predictedData);

		TResult correctCount = TResult.Zero;

		for (int i = 0; i < observedData.Count; i++)
		{
			if (observedData[i].Equals(predictedData[i]))
			{
				correctCount++;
			}
		}

		return correctCount / TResult.CreateChecked(observedData.Count);
	}

	/// <summary>
	/// Calculates classification error between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement INumber{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>The classification error (0-1).</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult CE<T, TResult>(IList<T> predictedData, IList<T> observedData)
		where T : INumber<T>
		where TResult : INumber<TResult>
	{
		return TResult.One - CA<T, TResult>(predictedData, observedData);
	}

	/// <summary>
	/// Calculates squared errors between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement INumber{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>An array of squared errors.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult[] SE<T, TResult>(IList<T> predictedData, IList<T> observedData)
		where T : INumber<T>
		where TResult : INumber<TResult>
	{
		ValidateDataSets(observedData, predictedData);

		var errors = new TResult[observedData.Count];

		for (int i = 0; i < observedData.Count; i++)
		{
			var residual = TResult.CreateChecked(observedData[i]) - TResult.CreateChecked(predictedData[i]);
			errors[i] = residual * residual;
		}

		return errors;
	}

	/// <summary>
	/// Calculates mean squared error between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement IFloatingPointIeee754{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>The mean squared error.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult MSE<T, TResult>(IList<T> predictedData, IList<T> observedData)
		where T : INumber<T>
		where TResult : IFloatingPointIeee754<TResult>
	{
		ValidateDataSets(observedData, predictedData);

		var squaredErrors = SE<T, TResult>(observedData, predictedData);
		var sum = Sum<TResult, TResult>(squaredErrors);
		return sum / TResult.CreateChecked(observedData.Count);
	}

	/// <summary>
	/// Calculates root mean squared error between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement IFloatingPointIeee754{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>The root mean squared error.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult RMSE<T, TResult>(IList<T> predictedData, IList<T> observedData)
		where T : INumber<T>
		where TResult : IFloatingPointIeee754<TResult>
	{
		return TResult.Sqrt(MSE<T, TResult>(observedData, predictedData));
	}

	/// <summary>
	/// Calculates absolute errors between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement IFloatingPointIeee754{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>An array of absolute errors.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult[] AE<T, TResult>(IList<T> predictedData, IList<T> observedData)
		where T : INumber<T>
		where TResult : IFloatingPointIeee754<TResult>
	{
		ValidateDataSets(observedData, predictedData);

		var errors = new TResult[observedData.Count];

		for (int i = 0; i < observedData.Count; i++)
		{
			errors[i] = TResult.Abs(TResult.CreateChecked(observedData[i]) - TResult.CreateChecked(predictedData[i]));
		}

		return errors;
	}

	/// <summary>
	/// Calculates mean absolute error between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement IFloatingPointIeee754{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>The mean absolute error.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult MAE<T, TResult>(IList<T> predictedData, IList<T> observedData)
		where T : INumber<T>
		where TResult : IFloatingPointIeee754<TResult>
	{
		ValidateDataSets(observedData, predictedData);

		var absoluteErrors = AE<T, TResult>(observedData, predictedData);
		var sum = Sum<TResult, TResult>(absoluteErrors);
		return sum / TResult.CreateChecked(observedData.Count);
	}

	/// <summary>
	/// Calculates absolute percentage errors between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement IFloatingPointIeee754{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>An array of absolute percentage errors.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult[] APE<T, TResult>(IList<T> predictedData, IList<T> observedData)
		where T : INumber<T>
		where TResult : IFloatingPointIeee754<TResult>
	{
		ValidateDataSets(observedData, predictedData);

		var errors = new TResult[observedData.Count];

		for (int i = 0; i < observedData.Count; i++)
		{
			var observedValue = TResult.CreateChecked(observedData[i]);
			if (observedValue == TResult.Zero)
			{
				errors[i] = TResult.Zero;
				continue;
			}

			var error = TResult.Abs((observedValue - TResult.CreateChecked(predictedData[i])) / observedValue);
			errors[i] = error;
		}

		return errors;
	}

	/// <summary>
	/// Calculates mean absolute percentage error between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement IFloatingPointIeee754{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>The mean absolute percentage error.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult MAPE<T, TResult>(IList<T> predictedData, IList<T> observedData)
		where T : INumber<T>
		where TResult : INumber<TResult>
	{
		ValidateDataSets(observedData, predictedData);

		TResult sum = TResult.Zero;
		int count = observedData.Count;

		for (int i = 0; i < count; i++)
		{
			T observed = observedData[i];
			if (observed == T.Zero)
			{
				throw new ArgumentException("Observed values cannot contain zero for MAPE calculation.");
			}

			// Convert to TResult before calculations to maintain precision
			TResult obs = TResult.CreateChecked(observed);
			TResult pred = TResult.CreateChecked(predictedData[i]);

			TResult absoluteError = TResult.Abs(obs - pred);
			sum += absoluteError / obs;
		}

		return sum / TResult.CreateChecked(count);
	}

	/// <summary>
	/// Calculates Nash-Sutcliffe efficiency coefficient between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement IFloatingPointIeee754{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>The Nash-Sutcliffe efficiency coefficient.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult NSE<T, TResult>(IList<T> predictedData, IList<T> observedData)
		where T : INumber<T>
		where TResult : IFloatingPointIeee754<TResult>
	{
		ValidateDataSets(observedData, predictedData);

		var squaredErrors = SE<T, TResult>(observedData, predictedData);
		var sumSquaredErrors = Sum<TResult, TResult>(squaredErrors);

		var meanObserved = Mean<T, TResult>(observedData);
		TResult sumSquaredDeviations = TResult.Zero;

		foreach (var item in observedData)
		{
			var deviation = TResult.CreateChecked(item) - meanObserved;
			sumSquaredDeviations += deviation * deviation;
		}

		return TResult.One - sumSquaredErrors / sumSquaredDeviations;
	}

	/// <summary>
	/// Calculates normalized Nash-Sutcliffe efficiency coefficient between predicted and observed values.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections, which must implement INumber{T}.</typeparam>
	/// <typeparam name="TResult">The type of the result, which must implement IFloatingPointIeee754{TResult}.</typeparam>
	/// <param name="predictedData">The predicted values.</param>
	/// <param name="observedData">The observed values.</param>
	/// <returns>The normalized Nash-Sutcliffe efficiency coefficient.</returns>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	public static TResult NNSE<T, TResult>(IList<T> predictedData, IList<T> observedData)
		where T : INumber<T>
		where TResult : IFloatingPointIeee754<TResult>
	{
		var nse = NSE<T, TResult>(observedData, predictedData);
		return TResult.One / (TResult.CreateChecked(2) - nse);
	}

	/// <summary>
	/// Validates that two datasets are compatible for comparison operations.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collections.</typeparam>
	/// <param name="observedData">The observed values.</param>
	/// <param name="predictedData">The predicted values.</param>
	/// <exception cref="ArgumentNullException">Thrown when either predictedData or observedData is null.</exception>
	/// <exception cref="ArgumentException">Thrown when collections have different lengths or fewer than 2 elements.</exception>
	private static void ValidateDataSets<T>(IList<T> observedData, IList<T> predictedData)
	{
		if (observedData is null)
		{
			throw new ArgumentNullException(nameof(observedData), "Observed data cannot be null.");
		}

		if (predictedData is null)
		{
			throw new ArgumentNullException(nameof(predictedData), "Predicted data cannot be null.");
		}

		if (observedData.Count != predictedData.Count)
		{
			throw new ArgumentException("Datasets must have the same length.");
		}

		if (observedData.Count < 2)
		{
			throw new ArgumentException("Datasets should contain at least 2 elements.");
		}
	}
}