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


namespace Daany.MathStuff.Norms;

/// <summary>
/// Provides implementations for various distance metrics.
/// </summary>
public static class Distance
{
	private const double EarthRadiusKm = 6371.0; // Earth's radius in kilometers
	private const double EarthRadiusMiles = 3956.0; // Earth's radius in miles

	/// <summary>
	/// Computes the Haversine distance between two geographical points in kilometers or miles.
	/// </summary>
	/// <param name="loStart">Longitude of the start point (degrees).</param>
	/// <param name="laStart">Latitude of the start point (degrees).</param>
	/// <param name="loEnd">Longitude of the end point (degrees).</param>
	/// <param name="laEnd">Latitude of the end point (degrees).</param>
	/// <param name="useKilometers">If true, distance is in kilometers, otherwise in miles.</param>
	/// <returns>Distance between points in the specified unit.</returns>
	public static double Haversine(double loStart, double laStart, double loEnd, double laEnd, bool useKilometers = true)
	{
		const double EarthRadiusKm = 6371.0;  // Earth's radius in kilometers
		const double EarthRadiusMiles = 3958.8; // Earth's radius in miles

		// Convert latitude and longitude from degrees to radians
		double lat1 = DegreeToRadian(laStart);
		double lon1 = DegreeToRadian(loStart);
		double lat2 = DegreeToRadian(laEnd);
		double lon2 = DegreeToRadian(loEnd);

		// Compute differences
		double dLat = lat2 - lat1;
		double dLon = lon2 - lon1;

		// Apply Haversine formula
		double a = Math.Pow(Math.Sin(dLat / 2), 2) +
				   Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2);
		double c = 2 * Math.Asin(Math.Sqrt(a));

		return (useKilometers ? EarthRadiusKm : EarthRadiusMiles) * c;
	}

	/// <summary>
	/// Converts degrees to radians.
	/// </summary>
	private static double DegreeToRadian(double degrees) => degrees * (Math.PI / 180.0);


	/// <summary>
	/// Computes the Euclidean distance between two points.
	/// </summary>
	public static double Euclidean(double x1, double y1, double x2, double y2)
		=> Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

	/// <summary>
	/// Computes the Manhattan distance between two points.
	/// </summary>
	public static double Manhattan(double x1, double y1, double x2, double y2)
		=> Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

	/// <summary>
	/// Computes the Minkowski distance between two points.
	/// </summary>
	/// <param name="x1">X-coordinate of the first point.</param>
	/// <param name="y1">Y-coordinate of the first point.</param>
	/// <param name="x2">X-coordinate of the second point.</param>
	/// <param name="y2">Y-coordinate of the second point.</param>
	/// <param name="p">The order of the Minkowski distance (p=1 is Manhattan, p=2 is Euclidean).</param>
	/// <returns>Minkowski distance.</returns>
	public static double Minkowski(double x1, double y1, double x2, double y2, double p)
		=> Math.Pow(Math.Pow(Math.Abs(x1 - x2), p) + Math.Pow(Math.Abs(y1 - y2), p), 1 / p);

	/// <summary>
	/// Computes the Chebyshev distance between two points.
	/// </summary>
	public static double Chebyshev(double x1, double y1, double x2, double y2)
		=> Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2));

	/// <summary>
	/// Computes the Cosine similarity between two vectors.
	/// </summary>
	public static double CosineSimilarity(double[] vectorA, double[] vectorB)
	{
		if (vectorA is null || vectorB is null) throw new ArgumentNullException("Vectors must not be null.");
		if (vectorA.Length != vectorB.Length) throw new ArgumentException("Vectors must have the same length.");

		double dotProduct = 0, magnitudeA = 0, magnitudeB = 0;

		for (int i = 0; i < vectorA.Length; i++)
		{
			dotProduct += vectorA[i] * vectorB[i];
			magnitudeA += Math.Pow(vectorA[i], 2);
			magnitudeB += Math.Pow(vectorB[i], 2);
		}

		return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
	}

	/// <summary>
	/// Computes the Mahalanobis distance between a vector and a distribution.
	/// </summary>
	public static double Mahalanobis(double[] vector, double[] mean, double[,] covMatrix)
	{
		if (vector is null || mean is null || covMatrix is null) throw new ArgumentNullException("Inputs must not be null.");
		if (vector.Length != mean.Length || vector.Length != covMatrix.GetLength(0))
			throw new ArgumentException("Dimensions of vector, mean, and covariance matrix must match.");

		double[] diff = new double[vector.Length];
		for (int i = 0; i < vector.Length; i++)
			diff[i] = vector[i] - mean[i];

		double[] temp = MatrixVectorMultiply(covMatrix, diff);
		double result = 0;
		for (int i = 0; i < vector.Length; i++)
			result += diff[i] * temp[i];

		return Math.Sqrt(result);
	}

	/// <summary>
	/// Performs matrix-vector multiplication.
	/// </summary>
	private static double[] MatrixVectorMultiply(double[,] matrix, double[] vector)
	{
		int size = vector.Length;
		double[] result = new double[size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				result[i] += matrix[i, j] * vector[j];
			}
		}
		return result;
	}
}
