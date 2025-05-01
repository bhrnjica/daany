using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Stat.stl;
using Daany.Stat;
using Daany.Ext;
using XPlot.Plotly;
using Daany.MathStuff;
using global::Daany.MathStuff.MatrixGeneric;

namespace Daany.MathStuff.Tests;


public class SpecialMatrixTests
{
	#region Hankel Matrix Tests

	[Fact]
	public void Hankel_WithValidVector_ReturnsCorrectMatrix()
	{
		// Arrange
		var vector = new double[] { 1, 2, 3, 4 };
		var expected = new double[,]
		{
				{1, 2, 3},
				{2, 3, 4},
				
		};

		// Act
		var result = vector.Hankel(colCount: 3);

		// Assert
		Assert.Equal(expected, result);
	}


	[Fact]
	public void Hankel_WithDefaultColCount_of_4_ReturnsSquareMatrix()
	{
		// Arrange
		var vector = new double[] { 1, 2, 3, 4};
		var expected = new double[,]
		{
				{1, 2, 3, 4},
				{2, 3, 4, 0},
				{3, 4, 0, 0},
				{4, 0, 0, 0}
		};

		// Act
		var result = vector.Hankel();

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Hankel_WithDefaultColCount_ReturnsSquareMatrix()
	{
		// Arrange
		var vector = new double[] { 1, 2, 3 };
		var expected = new double[,]
		{
				{1, 2, 3},
				{2, 3, 0},
				{3, 0, 0}
		};

		// Act
		var result = vector.Hankel();

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Hankel_WithNullVector_ThrowsArgumentNullException()
	{
		// Arrange
		double[] vector = null;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => vector.Hankel());
	}

	#endregion

	#region Toeplitz Matrix Tests

	[Fact]
	public void Toeplitz_WithValidVector_ReturnsCorrectMatrix()
	{
		// Arrange
		var vector = new double[] { 1, 2, 3 };
		var expected = new double[,]
		{
				{1, 2, 3},
				{2, 1, 2},
				{3, 2, 1}
		};

		// Act
		var result = vector.Toeplitz();

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Toeplitz_WithEmptyVector_ReturnsEmptyMatrix()
	{
		// Arrange
		var vector = Array.Empty<double>();

		// Act
		var result = vector.Toeplitz();

		// Assert
		Assert.Empty(result);
	}

	#endregion

	#region Zeros Tests

	[Fact]
	public void Zeros_Matrix_ReturnsCorrectMatrix()
	{
		// Arrange
		int rows = 2;
		int cols = 3;
		var expected = new double[rows, cols];

		// Act
		var result = SpecialMatrix.Zeros<double>(rows, cols);

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Zeros_Vector_ReturnsCorrectVector()
	{
		// Arrange
		int length = 5;
		var expected = new double[length];

		// Act
		var result = SpecialMatrix.Zeros<double>(length);

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Zeros_WithZeroDimensions_ReturnsEmptyMatrix()
	{
		// Act
		var result = SpecialMatrix.Zeros<double>(0, 0);

		// Assert
		Assert.Empty(result);
	}

	#endregion

	#region Unit Vector Tests

	[Fact]
	public void Unit_ReturnsCorrectVector()
	{
		// Arrange
		int length = 3;
		var expected = new double[] { 1, 1, 1 };

		// Act
		var result = SpecialMatrix.Unit<double>(length);

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Unit_WithZeroLength_ReturnsEmptyVector()
	{
		// Act
		var result = SpecialMatrix.Unit<double>(0);

		// Assert
		Assert.Empty(result);
	}

	#endregion

	#region Identity Matrix Tests

	[Fact]
	public void Identity_ReturnsCorrectMatrix()
	{
		// Arrange
		int size = 3;
		var expected = new double[,]
		{
				{1, 0, 0},
				{0, 1, 0},
				{0, 0, 1}
		};

		// Act
		var result = SpecialMatrix.Identity<double>(size, size);

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Identity_NonSquare_ReturnsCorrectMatrix()
	{
		// Arrange
		int rows = 2;
		int cols = 3;
		var expected = new double[,]
		{
				{1, 0, 0},
				{0, 1, 0}
		};

		// Act
		var result = SpecialMatrix.Identity<double>(rows, cols);

		// Assert
		Assert.Equal(expected, result);
	}

	#endregion

	#region Random Matrix Tests

	[Fact]
	public void Rand_Matrix_ReturnsCorrectSize()
	{

		// Act
		var result = SpecialMatrix.Rand<double>(6);

		// Assert
		Assert.Equal(6, result.Length);
	}

	[Fact]
	public void Rand_VectorWithRange_ReturnsValuesInRange()
	{
		// Arrange
		int length = 100;
		double min = 5;
		double max = 10;

		// Act
		var result = SpecialMatrix.Rand<double>(length, min, max);

		// Assert
		foreach (var value in result)
		{
			Assert.InRange(double.CreateChecked(value), min, max);
		}
	}

	#endregion

	#region Arange Tests

	[Fact]
	public void Arange_WithStopOnly_ReturnsCorrectVector()
	{
		// Arrange
		int stop = 5;
		var expected = new double[] { 0, 1, 2, 3, 4 };

		// Act
		var result = SpecialMatrix.Arange<double>(stop);

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Arange_WithStartStopStep_ReturnsCorrectVector()
	{
		// Arrange
		int start = 2;
		int stop = 10;
		int step = 3;
		var expected = new double[] { 2, 5, 8 };

		// Act
		var result = SpecialMatrix.Arange<double>(start, stop, step);

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Arange_InvalidStartStop_ThrowsException()
	{
		// Arrange
		int start = 10;
		int stop = 5;

		// Act & Assert
		Assert.Throws<ArgumentException>(() => SpecialMatrix.Arange<double>(start, stop));
	}

	#endregion

	#region Generate Tests

	[Fact]
	public void Generate_ReturnsCorrectVector()
	{
		// Arrange
		int rows = 2;
		int cols = 3;
		double value = 5.5;
		var expected = new double[] { 5.5, 5.5, 5.5, 5.5, 5.5, 5.5 };

		// Act
		var result = SpecialMatrix.Generate(rows, cols, value);

		// Assert
		Assert.Equal(expected, result);
	}

	#endregion

	#region Date Series Tests

	[Fact]
	public void DateSeries_ReturnsCorrectDates()
	{
		// Arrange
		var fromDate = new DateTime(2023, 1, 1);
		var toDate = new DateTime(2023, 1, 4);
		var span = TimeSpan.FromDays(1);
		var expected = new DateTime[]
		{
				new DateTime(2023, 1, 1),
				new DateTime(2023, 1, 2),
				new DateTime(2023, 1, 3),
				new DateTime(2023, 1, 4)
		};

		// Act
		var result = SpecialMatrix.DateSeries(fromDate, toDate, span);

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void MonthlySeries_ReturnsCorrectDates()
	{
		// Arrange
		var fromDate = new DateTime(2023, 1, 1);
		int months = 2;
		int count = 3;
		var expected = new DateTime[]
		{
				new DateTime(2023, 1, 1),
				new DateTime(2023, 3, 1),
				new DateTime(2023, 5, 1)
		};

		// Act
		var result = SpecialMatrix.MonthlySeries(fromDate, months, count);

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void YearlySeries_ReturnsCorrectDates()
	{
		// Arrange
		var fromDate = new DateTime(2020, 1, 1);
		int years = 2;
		int count = 3;
		var expected = new DateTime[]
		{
				new DateTime(2020, 1, 1),
				new DateTime(2022, 1, 1),
				new DateTime(2024, 1, 1)
		};

		// Act
		var result = SpecialMatrix.YearlySeries(fromDate, years, count);

		// Assert
		Assert.Equal(expected, result);
	}

	#endregion

	#region Numeric Series Tests

	[Fact]
	public void SSeries_ReturnsCorrectValues()
	{
		// Arrange
		double from = 1;
		double to = 4;
		double step = 1;
		var expected = new double[] { 1, 2, 3 };

		// Act
		var result = SpecialMatrix.SSeries(from, to, step);

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void ConstSeries_ReturnsCorrectValues()
	{
		// Arrange
		double value = 3.14;
		int count = 4;
		var expected = new double[] { 3.14, 3.14, 3.14, 3.14 };

		// Act
		var result = SpecialMatrix.ConstSeries(value, count);

		// Assert
		Assert.Equal(expected, result);
	}

	#endregion


	[Fact]
	public void HankelMatrix_Test01()
	{
		var ts = new double[] { 1, 2, 3, 4, 5, 6 };
		var r = ts.Hankel();
		//
		Assert.Equal(1, r[0, 0]);
		Assert.Equal(2, r[0, 1]);
		Assert.Equal(3, r[0, 2]);
		Assert.Equal(4, r[0, 3]);
		Assert.Equal(5, r[0, 4]);
		Assert.Equal(6, r[0, 5]);

		Assert.Equal(2, r[1, 0]);
		Assert.Equal(3, r[1, 1]);
		Assert.Equal(4, r[1, 2]);
		Assert.Equal(5, r[1, 3]);
		Assert.Equal(6, r[1, 4]);
		Assert.Equal(0, r[1, 5]);

		Assert.Equal(3, r[2, 0]);
		Assert.Equal(4, r[2, 1]);
		Assert.Equal(5, r[2, 2]);
		Assert.Equal(6, r[2, 3]);
		Assert.Equal(0, r[2, 4]);
		Assert.Equal(0, r[2, 5]);

		Assert.Equal(4, r[3, 0]);
		Assert.Equal(5, r[3, 1]);
		Assert.Equal(6, r[3, 2]);
		Assert.Equal(0, r[3, 3]);
		Assert.Equal(0, r[3, 4]);
		Assert.Equal(0, r[3, 5]);

		Assert.Equal(5, r[4, 0]);
		Assert.Equal(6, r[4, 1]);
		Assert.Equal(0, r[4, 2]);
		Assert.Equal(0, r[4, 3]);
		Assert.Equal(0, r[4, 4]);
		Assert.Equal(0, r[4, 5]);

		Assert.Equal(6, r[5, 0]);
		Assert.Equal(0, r[5, 1]);
		Assert.Equal(0, r[5, 2]);
		Assert.Equal(0, r[5, 3]);
		Assert.Equal(0, r[5, 4]);
		Assert.Equal(0, r[5, 5]);

	}

	[Fact]
	public void HankelMatrix_Test02()
	{
		var ts = new double[] { 1, 2, 3, 4, 5 };
		var r = ts.Hankel(colCount:4);
		//
		Assert.Equal(1, r[0, 0]);
		Assert.Equal(2, r[0, 1]);
		Assert.Equal(3, r[0, 2]);
		Assert.Equal(4, r[0, 3]);

		Assert.Equal(2, r[1, 0]);
		Assert.Equal(3, r[1, 1]);
		Assert.Equal(4, r[1, 2]);
		Assert.Equal(5, r[1, 3]);


	}

	[Fact]
	public void ToeplitzMatrix_Test01()
	{
		var ts = new double[] { 1, 2, 3, 4, 5, 6 };
		var r = ts.Toeplitz();
		//
		Assert.Equal(1, r[0, 0]);
		Assert.Equal(2, r[0, 1]);
		Assert.Equal(3, r[0, 2]);
		Assert.Equal(4, r[0, 3]);
		Assert.Equal(5, r[0, 4]);
		Assert.Equal(6, r[0, 5]);

		Assert.Equal(2, r[1, 0]);
		Assert.Equal(1, r[1, 1]);
		Assert.Equal(2, r[1, 2]);
		Assert.Equal(3, r[1, 3]);
		Assert.Equal(4, r[1, 4]);
		Assert.Equal(5, r[1, 5]);

		Assert.Equal(3, r[2, 0]);
		Assert.Equal(2, r[2, 1]);
		Assert.Equal(1, r[2, 2]);
		Assert.Equal(2, r[2, 3]);
		Assert.Equal(3, r[2, 4]);
		Assert.Equal(4, r[2, 5]);

		Assert.Equal(4, r[3, 0]);
		Assert.Equal(3, r[3, 1]);
		Assert.Equal(2, r[3, 2]);
		Assert.Equal(1, r[3, 3]);
		Assert.Equal(2, r[3, 4]);
		Assert.Equal(3, r[3, 5]);

		Assert.Equal(5, r[4, 0]);
		Assert.Equal(4, r[4, 1]);
		Assert.Equal(3, r[4, 2]);
		Assert.Equal(2, r[4, 3]);
		Assert.Equal(1, r[4, 4]);
		Assert.Equal(2, r[4, 5]);

		Assert.Equal(6, r[5, 0]);
		Assert.Equal(5, r[5, 1]);
		Assert.Equal(4, r[5, 2]);
		Assert.Equal(3, r[5, 3]);
		Assert.Equal(2, r[5, 4]);
		Assert.Equal(1, r[5, 5]);

	}




	[Fact]
	public void SVD_Test01()
	{
		var matrix = new double[6, 5]
		{
				{ 8.79,  9.93,  9.83, 5.45,  3.16 },
				{ 6.11,  6.91,  5.04, -0.27,  7.98 },
			   { -9.15, -7.93,  4.86, 4.85,  3.01 },
				{ 9.57,  1.64,  8.83, 0.74,  5.80 },
			   { -3.49,  4.02,  9.80, 10.00,  4.27 },
				{ 9.84,  0.15, -8.99, -6.02, -5.31 }
		};
		//SVD
		var svd = Daany.LinA.LinA.Svd(matrix, false, false);
		//
		Assert.Equal(27.47, svd.s[0], 2);
		Assert.Equal(22.64, svd.s[1], 2);
		Assert.Equal(8.56, svd.s[2], 2);
		Assert.Equal(5.99, svd.s[3], 2);
		Assert.Equal(2.01, svd.s[4], 2);


	}
}


