using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Daany.MathStuff.Stats;

namespace Daany.MathStuff.Tests
{
	public class MetricsTests
	{
		private readonly List<decimal> _testData = new List<decimal> { 1, 2, 3, 4, 5 };
		private readonly List<decimal> _testDataWithDuplicates = new List<decimal> { 1, 2, 2, 3, 4, 5, 5, 5 };
		private readonly List<decimal> _testDataEvenCount = new List<decimal> { 1, 2, 3, 4, 5, 6 };
		private readonly List<decimal> _testDataNegative = new List<decimal> { -3, -1, 0, 2, 4 };
		private readonly List<decimal> _testDataSingle = new List<decimal> { 5 };
		private readonly List<decimal> _testDataEmpty = new List<decimal>();

		decimal[] xActuald = new decimal[] { 0.06195m, 0.37760m, 0.83368m, 0.46979m, 0.90657m, 0.82522m, 0.83786m, 0.67581m, 0.80171m, 0.63398m };
		double[] xActual = new double[] { 0.06195, 0.37760, 0.83368, 0.46979, 0.90657, 0.82522, 0.83786, 0.67581, 0.80171, 0.63398, 0.87457, 0.83748, 0.85664, 0.48001, 0.12338, 0.45984, 0.75913, 0.41532, 0.18440, 0.72677, 0.76270, 0.12845, 0.02448, 0.32324, 0.22659, 0.76046, 0.14442, 0.59808, 0.99063, 0.73614, };
		double[] yPredicted = new double[] { 0.79672, 0.54635, 0.02614, 0.72299, 0.16721, 0.56652, 0.64241, 0.29990, 0.62151, 0.37354, 0.76671, 0.65242, 0.72815, 0.97263, 0.50080, 0.05452, 0.36795, 0.53565, 0.68247, 0.16595, 0.95008, 0.69064, 0.02562, 0.11362, 0.70848, 0.86734, 0.43400, 0.89580, 0.67682, 0.85494 };
		float[] array = new float[10] { 0.4f, 0.5f, 0.45f, 0.5f, 0.45f, 0.4f, 0.5f, 0.45f, 0.4f, 0.5f };


		#region Basic Statistics Tests

		[Fact]
		public void Sum_ValidData_ReturnsCorrectSum()
		{
			// Arrange
			var data = _testData;

			// Act
			var result = Metrics.Sum<decimal, decimal>(data);

			// Assert
			Assert.Equal(15, result);
		}

		[Fact]
		public void Sum_EmptyData_ThrowsArgumentException()
		{
			// Arrange
			var data = _testDataEmpty;

			// Act & Assert
			Assert.Throws<ArgumentException>(() => Metrics.Sum<decimal, decimal>(data));
		}

		[Fact]
		public void Mean_ValidData_ReturnsCorrectMean()
		{
			// Arrange
			var data = _testData;

			// Act
			var result = Metrics.Mean<decimal, decimal>(data);

			// Assert
			Assert.Equal(3, result);
		}

		[Fact]
		public void Mean_EmptyData_ThrowsArgumentException()
		{
			// Arrange
			var data = _testDataEmpty;

			// Act & Assert
			Assert.Throws<ArgumentException>(() => Metrics.Mean<decimal, decimal>(data));
		}

		[Fact]
		public void Mode_ValidData_ReturnsCorrectMode()
		{
			// Arrange
			var data = _testDataWithDuplicates;

			// Act
			var result = Metrics.Mode<decimal>(data);

			// Assert
			Assert.Equal(5, result);
		}

		[Fact]
		public void Median_OddCount_ReturnsCorrectMedian()
		{
			// Arrange
			var data = _testData;

			// Act
			var result = Metrics.Median<decimal, decimal>(data);

			// Assert
			Assert.Equal(3, result);
		}

		[Fact]
		public void Median_EvenCount_ReturnsCorrectMedian()
		{
			// Arrange
			var data = _testDataEvenCount;

			// Act
			var result = Metrics.Median<decimal, decimal>(data);

			// Assert
			Assert.Equal(3.5m, result);
		}

		[Fact]
		public void Percentile_ValidData_ReturnsCorrectPercentile()
		{
			// Arrange
			var data = _testData;

			// Act
			var result = Metrics.Percentile<decimal>(data, 50);

			// Assert
			Assert.Equal(3, result);
		}

		[Fact]
		public void Percentile_100Percent_ReturnsMaxValue()
		{
			// Arrange
			var data = _testData;

			// Act
			var result = Metrics.Percentile<decimal>(data, 100);

			// Assert
			Assert.Equal(5, result);
		}

		[Fact]
		public void Variance_SampleData_ReturnsCorrectVariance()
		{
			// Arrange
			var data = _testData;

			// Act
			var result = Metrics.Variance<decimal, decimal>(data, isSampleData: true);

			// Assert
			Assert.Equal(2.5m, result);
		}

		[Fact]
		public void Variance_PopulationData_ReturnsCorrectVariance()
		{
			// Arrange
			var data = _testData;

			// Act
			var result = Metrics.Variance<decimal, decimal>(data, isSampleData: false);

			// Assert
			Assert.Equal(2m, result);
		}

		[Fact]
		public void Covariance_ValidData_ReturnsCorrectCovariance()
		{
			// Arrange
			var x = new List<decimal> { 1, 2, 3, 4, 5 };
			var y = new List<decimal> { 2, 3, 4, 5, 6 };

			// Act
			var result = Metrics.Covariance<decimal, decimal>(x, y);

			// Assert
			Assert.Equal(2.5m, result);
		}

		[Fact]
		public void Covariance_DifferentLengths_ThrowsArgumentException()
		{
			// Arrange
			var x = new List<decimal> { 1, 2, 3 };
			var y = new List<decimal> { 1, 2 };

			// Act & Assert
			Assert.Throws<ArgumentException>(() => Metrics.Covariance<decimal, decimal>(x, y));
		}

		[Fact]
		public void Stdev_ValidData_ReturnsCorrectStandardDeviation()
		{
			// Arrange
			var data = _testData;

			// Act
			var result = Metrics.Stdev<decimal, decimal>(data);

			// Assert
			Assert.Equal((decimal)Math.Sqrt(2.5), result);
		}

		[Fact]
		public void Random_ValidData_ReturnsElementFromCollection()
		{
			// Arrange
			var data = _testData;
			var seed = 42;

			// Act
			var result = Metrics.Random<decimal>(data, seed);

			// Assert
			Assert.Contains(result, data);
		}

		[Fact]
		public void Frequency_ValidData_ReturnsCorrectFrequencies()
		{
			// Arrange
			var data = _testDataWithDuplicates;

			// Act
			var result = Metrics.Frequency<decimal>(data);

			// Assert
			Assert.Equal(5, result.Count);
			Assert.Equal(5, result[0].Value);
			Assert.Equal(3, result[0].Count);
			Assert.Equal(2, result[1].Value);
			Assert.Equal(2, result[1].Count);
		}

		#endregion

		#region Error Metric Tests

		[Fact]
		public void RSquared_ValidData_ReturnsCorrectValue()
		{
			// Arrange
			var observed = new List<decimal> { 1, 2, 3, 4, 5 };
			var predicted = new List<decimal> { 1.1m, 1.9m, 3.2m, 3.8m, 5.2m };

			// Act
			var result = Metrics.RSquared<decimal, decimal>(predicted, observed);

			// Assert
			Assert.Equal(result, 0.98732094463801m, 5);
		}

		[Fact]
		public void R_ValidData_ReturnsCorrectValue()
		{
			// Arrange
			var observed = new List<double> { 1, 2, 3, 4, 5 };
			var predicted = new List<double> { 1.1, 1.9, 3.2, 3.8, 5.2 };

			// Act
			var result = Metrics.R<double, double>(predicted, observed);

			// Assert
			Assert.InRange(result, 0.99, 1.0);
		}

		[Fact]
		public void CA_ValidData_ReturnsCorrectAccuracy()
		{
			// Arrange
			var observed = new List<double> { 1, 2, 3, 4, 5 };
			var predicted = new List<double> { 1, 2, 3, 5, 5 };

			// Act
			var result = Metrics.CA<double, double>(predicted, observed);

			// Assert
			Assert.Equal(0.8, result);
		}

		[Fact]
		public void CE_ValidData_ReturnsCorrectError()
		{
			// Arrange
			var observed = new List<double> { 1, 2, 3, 4, 5 };
			var predicted = new List<double> { 1, 2, 3, 5, 5 };

			// Act
			var result = Metrics.CE<double, double>(predicted, observed);

			// Assert
			Assert.True(Math.Abs(0.2 - result) < 0.00001);
		}

		[Fact]
		public void SE_ValidData_ReturnsSquaredErrors()
		{
			// Arrange
			var observed = new List<double> { 1, 2, 3 };
			var predicted = new List<double> { 1.1, 1.9, 3.2 };

			// Act
			var result = Metrics.SE<double, double>(predicted, observed);

			// Assert
			Assert.Equal(3, result.Length);
			Assert.Equal(0.01, result[0], 5);
			Assert.Equal(0.01, result[1], 5);
			Assert.Equal(0.04, result[2], 5);
		}

		[Fact]
		public void MSE_ValidData_ReturnsMeanSquaredError()
		{
			// Arrange
			var observed = new List<double> { 1, 2, 3 };
			var predicted = new List<double> { 1.1, 1.9, 3.2 };

			// Act
			var result = Metrics.MSE<double, double>(predicted, observed);

			// Assert
			Assert.Equal(0.02, result, 5);
		}

		[Fact]
		public void RMSE_ValidData_ReturnsRootMeanSquaredError()
		{
			// Arrange
			var observed = new List<double> { 1, 2, 3 };
			var predicted = new List<double> { 1.1, 1.9, 3.2 };

			// Act
			var result = Metrics.RMSE<double, double>(predicted, observed);

			// Assert
			Assert.Equal(Math.Sqrt(0.02), result, 5);
		}

		[Fact]
		public void AE_ValidData_ReturnsAbsoluteErrors()
		{
			// Arrange
			var observed = new List<double> { 1, 2, 3 };
			var predicted = new List<double> { 1.1, 1.9, 3.2 };

			// Act
			var result = Metrics.AE<double, double>(predicted, observed);

			// Assert
			Assert.Equal(3, result.Length);
			Assert.Equal(0.1, result[0], 5);
			Assert.Equal(0.1, result[1], 5);
			Assert.Equal(0.2, result[2], 5);
		}

		[Fact]
		public void MAE_ValidData_ReturnsMeanAbsoluteError()
		{
			// Arrange
			var observed = new List<double> { 1, 2, 3 };
			var predicted = new List<double> { 1.1, 1.9, 3.2 };

			// Act
			var result = Metrics.MAE<double, double>(predicted, observed);

			// Assert
			Assert.Equal((0.1 + 0.1 + 0.2) / 3, result, 5);
		}

		[Fact]
		public void APE_ValidData_ReturnsAbsolutePercentageErrors()
		{
			// Arrange
			var observed = new List<double> { 1, 2, 4 };
			var predicted = new List<double> { 1.1, 1.9, 4.4 };

			// Act
			var result = Metrics.APE<double, double>(predicted, observed);

			// Assert
			Assert.Equal(3, result.Length);
			Assert.Equal(0.1, result[0], 5);
			Assert.Equal(0.05, result[1], 5);
			Assert.Equal(0.1, result[2], 5);
		}

		[Fact]
		public void MAPE_ValidData_ReturnsMeanAbsolutePercentageError()
		{
			// Arrange
			decimal[] observed = [30, 33.3m, 38, 42, 31, 29.5m, 43.5m, 35.9m, 37, 40];
			decimal[] predicted = [34, 31, 43, 41.4m, 35.6m, 33, 40, 38, 34, 44.5m];
			

			// Act
			var result = Metrics.MAPE<decimal, decimal>(predicted, observed);

			// Assert
			Assert.Equal(0.09478349m, result, 5); // Allowing 5 decimal places precision
		}

		[Fact]
		public void NSE_ValidData_ReturnsNashSutcliffeEfficiency()
		{
			// Arrange
			var observed = new List<double> { 1, 2, 3, 4, 5 };
			var predicted = new List<double> { 1.1, 1.9, 3.2, 3.8, 5.2 };

			// Act
			var result = Metrics.NSE<double, double>(predicted, observed);

			// Assert
			Assert.InRange(result, 0.9, 1.0);
		}

		[Fact]
		public void NNSE_ValidData_ReturnsNormalizedNashSutcliffeEfficiency()
		{
			// Arrange
			var observed = new List<double> { 1, 2, 3, 4, 5 };
			var predicted = new List<double> { 1.1, 1.9, 3.2, 3.8, 5.2 };

			// Act
			var result = Metrics.NNSE<double, double>(predicted, observed);

			// Assert
			Assert.InRange(result, 0.9, 1.0);
		}

		#endregion

		#region Error Cases

		[Fact]
		public void AllMethods_NullData_ThrowsArgumentNullException()
		{
			// Arrange
			List<double> nullData = null;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => Metrics.Sum<double, double>(nullData));
			Assert.Throws<ArgumentNullException>(() => Metrics.Mean<double, double>(nullData));
			Assert.Throws<ArgumentNullException>(() => Metrics.Mode<double>(nullData));
			Assert.Throws<ArgumentNullException>(() => Metrics.Median<double, double>(nullData));
			Assert.Throws<ArgumentNullException>(() => Metrics.Percentile<double>(nullData, 50));
			Assert.Throws<ArgumentNullException>(() => Metrics.Variance<double, double>(nullData));
			Assert.Throws<ArgumentNullException>(() => Metrics.Stdev<double, double>(nullData));
			Assert.Throws<ArgumentNullException>(() => Metrics.Random<double>(nullData));
			Assert.Throws<ArgumentNullException>(() => Metrics.Frequency<double>(nullData));
		}

		[Fact]
		public void AllMethods_EmptyData_ThrowsArgumentException()
		{
			// Arrange
			var emptyData = _testDataEmpty;

			// Act & Assert
			Assert.Throws<ArgumentException>(() => Metrics.Sum<decimal, decimal>(emptyData));
			Assert.Throws<ArgumentException>(() => Metrics.Mean<decimal, decimal>(emptyData));
			Assert.Throws<ArgumentException>(() => Metrics.Mode<decimal>(emptyData));
			Assert.Throws<ArgumentException>(() => Metrics.Median<decimal, decimal>(emptyData));
			Assert.Throws<ArgumentException>(() => Metrics.Percentile<decimal>(emptyData, 50));
			Assert.Throws<ArgumentException>(() => Metrics.Variance<decimal, decimal>(emptyData));
			Assert.Throws<ArgumentException>(() => Metrics.Stdev<decimal, decimal>(emptyData));
			Assert.Throws<ArgumentException>(() => Metrics.Frequency<decimal>(emptyData));
		}

		[Fact]
		public void ErrorMetrics_DifferentLengths_ThrowsArgumentException()
		{
			// Arrange
			var obs = new List<double> { 1, 2, 3 };
			var pred = new List<double> { 1, 2 };

			// Act & Assert
			Assert.Throws<ArgumentException>(() => Metrics.RSquared<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.R<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.CA<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.CE<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.SE<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.MSE<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.RMSE<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.AE<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.MAE<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.APE<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.MAPE<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.NSE<double, double>(pred, obs));
			Assert.Throws<ArgumentException>(() => Metrics.NNSE<double, double>(pred, obs));
		}

		#endregion

		#region Common Metrics Tests
		[Fact]
		public void Statistics_Mode_Test()
		{

			var retVal = Metrics.Mode<double>(xActual.ToList());

			Assert.Equal<double>(0.06195, retVal);
		}

		[Fact]
		public void Statistics_Random_Test()
		{

			var retVal = Metrics.Random<float>(array.ToList());

			Assert.Equal<float>(0.45f, retVal);
		}

		[Fact]
		public void Statistics_Frequency_Test()
		{

			var retVal = Metrics.Frequency<float>(array.ToList());

			var expected = new List<(float, int)>() { (0.5f, 4), (0.4f, 3), (0.45f, 3) };

			Assert.Equal<(float value, int count)>(expected, retVal);
		}

		[Fact]
		public void Statistics_Median_Test()
		{

			var retVal = Metrics.Median<double, double>(xActual.ToList());

			var expected = 0.654895;

			Assert.Equal<double>(expected, retVal);
		}
		#endregion
	}
}  
