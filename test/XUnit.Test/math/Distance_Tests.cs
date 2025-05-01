using System;
using Xunit;
using Daany.MathStuff.Norms;

namespace Daany.MathStuff.Tests;

public class DistanceTests
{
	[Fact]
	public void Euclidean_ShouldReturnCorrectDistance()
	{
		double result = Distance.Euclidean(0, 0, 3, 4);
		Assert.Equal(5, result, 5); // 3-4-5 triangle
	}

	[Fact]
	public void Manhattan_ShouldReturnCorrectDistance()
	{
		double result = Distance.Manhattan(1, 2, 4, 6);
		Assert.Equal(7, result, 5); // |4-1| + |6-2| = 3 + 4 = 7
	}

	[Fact]
	public void Chebyshev_ShouldReturnCorrectDistance()
	{
		double result = Distance.Chebyshev(1, 5, 8, 3);
		Assert.Equal(7, result);
	}

	[Fact]
	public void Minkowski_ShouldReturnCorrectDistance_ForP2()
	{
		double result = Distance.Minkowski(0, 0, 3, 4, 2);
		Assert.Equal(5, result, 5); // Equivalent to Euclidean for p=2
	}

	[Fact]
	public void Haversine_ShouldReturnCorrectDistance_Kilometers()
	{
		double result = Distance.Haversine(13.4050, 52.5200, 2.3522, 48.8566);

		// The approximate distance between Berlin and Paris is ~878 km
		Assert.Equal(877.5, result, 1);
	}


	[Fact]
	public void CosineSimilarity_ShouldReturnCorrectSimilarity()
	{
		double[] v1 = { 1, 2, 3 };
		double[] v2 = { 1, 2, 3 };
		double result = Distance.CosineSimilarity(v1, v2);
		Assert.Equal(1.0, result, 5); // Perfect match
	}

	[Fact]
	public void Mahalanobis_ShouldReturnCorrectDistance()
	{
		double[] vector = { 1.0, 2.0 };
		double[] mean = { 1.5, 2.5 };
		double[,] covMatrix = { { 1.0, 0.2 }, { 0.2, 1.0 } };

		double result = Distance.Mahalanobis(vector, mean, covMatrix);
		Assert.InRange(result, 0.5, 1.5); // Expected within a reasonable range
	}

	[Fact]
	public void CosineSimilarity_ShouldThrowException_ForUnequalLengthVectors()
	{
		double[] v1 = { 1, 2, 3 };
		double[] v2 = { 1, 2 };

		Assert.Throws<ArgumentException>(() => Distance.CosineSimilarity(v1, v2));
	}

	[Fact]
	public void Mahalanobis_ShouldThrowException_ForNullInputs()
	{
		Assert.Throws<ArgumentNullException>(() => Distance.Mahalanobis(null!, new double[] { 1, 2 }, new double[,] { { 1, 0 }, { 0, 1 } }));
		Assert.Throws<ArgumentNullException>(() => Distance.Mahalanobis(new double[] { 1, 2 }, null!, new double[,] { { 1, 0 }, { 0, 1 } }));
		Assert.Throws<ArgumentNullException>(() => Distance.Mahalanobis(new double[] { 1, 2 }, new double[] { 1, 2 }, null!));
	}
}
