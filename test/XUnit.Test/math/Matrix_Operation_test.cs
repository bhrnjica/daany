using System;
using System.Numerics;
using Xunit;
using Daany.MathStuff.MatrixGeneric;

namespace Daany.MathStuff.Tests
{
	public class MatrixOperationsTests
	{
		#region Test Data

		private static readonly double[,] SampleMatrix = new double[,]
		{
			{1, 2, 3},
			{4, 5, 6},
			{7, 8, 9}
		};

		private static readonly double[] SampleVector = new double[] { 1, 2, 3 };

		#endregion

		#region Add Tests

		[Fact]
		public void Add_Matrices_ReturnsCorrectResult()
		{
			// Arrange
			var matrix1 = new double[,] { { 1, 2 }, { 3, 4 } };
			var matrix2 = new double[,] { { 5, 6 }, { 7, 8 } };
			var expected = new double[,] { { 6, 8 }, { 10, 12 } };

			// Act
			var result = matrix1.Add(matrix2);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Add_MatricesDifferentDimensions_ThrowsArgumentException()
		{
			// Arrange
			var matrix1 = new double[,] { { 1, 2 } };
			var matrix2 = new double[,] { { 1, 2, 3 } };

			// Act & Assert
			Assert.Throws<IndexOutOfRangeException>(() => matrix1.Add(matrix2));
		}

		[Fact]
		public void Add_MatrixWithScalar_ReturnsCorrectResult()
		{
			// Arrange
			var matrix = new double[,] { { 1, 2 }, { 3, 4 } };
			double scalar = 5;
			var expected = new double[,] { { 6, 7 }, { 8, 9 } };

			// Act
			var result = matrix.Add(scalar);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Add_Vectors_ReturnsCorrectResult()
		{
			// Arrange
			var vector1 = new double[] { 1, 2, 3 };
			var vector2 = new double[] { 4, 5, 6 };
			var expected = new double[] { 5, 7, 9 };

			// Act
			var result = vector1.Add(vector2);

			// Assert
			Assert.Equal(expected, result);
		}

		#endregion

		#region Subtract Tests

		[Fact]
		public void Subtract_Matrices_ReturnsCorrectResult()
		{
			// Arrange
			var matrix1 = new double[,] { { 5, 6 }, { 7, 8 } };
			var matrix2 = new double[,] { { 1, 2 }, { 3, 4 } };
			var expected = new double[,] { { 4, 4 }, { 4, 4 } };

			// Act
			var result = matrix1.Subtract(matrix2);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Subtract_MatrixWithScalar_ReturnsCorrectResult()
		{
			// Arrange
			var matrix = new double[,] { { 6, 7 }, { 8, 9 } };
			double scalar = 5;
			var expected = new double[,] { { 1, 2 }, { 3, 4 } };

			// Act
			var result = matrix.Subtract(scalar);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Subtract_Vectors_ReturnsCorrectResult()
		{
			// Arrange
			var vector1 = new double[] { 5, 7, 9 };
			var vector2 = new double[] { 4, 5, 6 };
			var expected = new double[] { 1, 2, 3 };

			// Act
			var result = vector1.Subtract(vector2);

			// Assert
			Assert.Equal(expected, result);
		}

		#endregion

		#region Multiply Tests

		[Fact]
		public void Multiply_MatrixWithScalar_ReturnsCorrectResult()
		{
			// Arrange
			var matrix = new double[,] { { 1, 2 }, { 3, 4 } };
			double scalar = 2;
			var expected = new double[,] { { 2, 4 }, { 6, 8 } };

			// Act
			var result = matrix.Multiply(scalar);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Multiply_VectorsElementWise_ReturnsCorrectResult()
		{
			// Arrange
			var vector1 = new double[] { 1, 2, 3 };
			var vector2 = new double[] { 4, 5, 6 };
			var expected = new double[] { 4, 10, 18 };

			// Act
			var result = vector1.Multiply(vector2);

			// Assert
			Assert.Equal(expected, result);
		}

		#endregion

		#region Divide Tests

		[Fact]
		public void Divide_MatrixByScalar_ReturnsCorrectResult()
		{
			// Arrange
			var matrix = new double[,] { { 2, 4 }, { 6, 8 } };
			double scalar = 2;
			var expected = new double[,] { { 1, 2 }, { 3, 4 } };

			// Act
			var result = matrix.Divide(scalar);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Divide_MatrixByZero_ThrowsDivideByZeroException()
		{
			// Arrange
			var matrix = new double[,] { { 1, 2 }, { 3, 4 } };
			double scalar = 0;

			// Act & Assert
			Assert.Throws<DivideByZeroException>(() => matrix.Divide(scalar));
		}

		[Fact]
		public void Divide_VectorsElementWise_ReturnsCorrectResult()
		{
			// Arrange
			var vector1 = new double[] { 4, 10, 18 };
			var vector2 = new double[] { 4, 5, 6 };
			var expected = new double[] { 1, 2, 3 };

			// Act
			var result = vector1.Divide(vector2);

			// Assert
			Assert.Equal(expected, result);
		}

		#endregion

		#region Dot Product Tests

		[Fact]
		public void Dot_MatrixAndVector_ReturnsCorrectResult()
		{
			// Arrange
			var matrix = new double[,] { { 1, 2 }, { 3, 4 } };
			var vector = new double[] { 5, 6 };
			var expected = new double[] { 17, 39 }; // 1*5 + 2*6 = 17, 3*5 + 4*6 = 39

			// Act
			var result = matrix.Dot(vector);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Dot_Matrices_ReturnsCorrectResult()
		{
			// Arrange
			var matrix1 = new double[,] { { 1, 2 }, { 3, 4 } };
			var matrix2 = new double[,] { { 5, 6 }, { 7, 8 } };
			var expected = new double[,] { { 19, 22 }, { 43, 50 } };

			// Act
			var result = matrix1.Dot(matrix2);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Dot_VectorAndMatrix_ReturnsCorrectResult()
		{
			// Arrange
			var vector = new double[] { 1, 2 };
			var matrix = new double[,] { { 3, 4 }, { 5, 6 } };
			var expected = new double[] { 13, 16 }; // 1*3 + 2*5 = 13, 1*4 + 2*6 = 16

			// Act
			var result = vector.Dot(matrix);

			// Assert
			Assert.Equal(expected, result);
		}

		#endregion

		#region Mathematical Functions Tests

		[Fact]
		public void Sqrt_Matrix_ReturnsCorrectResult()
		{
			// Arrange
			var matrix = new double[,] { { 4, 9 }, { 16, 25 } };
			var expected = new double[,] { { 2, 3 }, { 4, 5 } };

			// Act
			var result = matrix.Sqrt();

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Log_Matrix_ReturnsCorrectResult()
		{
			// Arrange
			var matrix = new double[,] { { Math.E, Math.E * Math.E }, { 1, Math.E } };
			var expected = new double[,] { { 1, 2 }, { 0, 1 } };

			// Act
			var result = matrix.Log();

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Pow_Matrix_ReturnsCorrectResult()
		{
			// Arrange
			var matrix = new double[,] { { 1, 2 }, { 3, 4 } };
			double exponent = 2;
			var expected = new double[,] { { 1, 4 }, { 9, 16 } };

			// Act
			var result = matrix.Pow(exponent);

			// Assert
			Assert.Equal(expected, result);
		}

		#endregion

		#region Vector Operations Tests

		[Fact]
		public void GetColumnVector_ReturnsCorrectColumn()
		{
			// Arrange
			var matrix = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } };
			int columnIndex = 1;
			var expected = new double[] { 2, 5 };

			// Act
			var result = matrix.GetColumnVector(columnIndex);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void GetRowVector_ReturnsCorrectRow()
		{
			// Arrange
			var matrix = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } };
			int rowIndex = 0;
			var expected = new double[] { 1, 2, 3 };

			// Act
			var result = matrix.GetRowVector(rowIndex);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void CumulativeSum_ReturnsCorrectResult()
		{
			// Arrange
			var vector = new double[] { 1, 2, 3, 4 };
			var expected = new double[] { 1, 3, 6, 10 };

			// Act
			var result = vector.CumulativeSum();

			// Assert
			Assert.Equal(expected, result);
		}

		#endregion

		#region Norm Tests

		[Fact]
		public void L2Norm_Vector_ReturnsCorrectResult()
		{
			// Arrange
			var vector = new double[] { 3, 4 };
			double expected = 5; // sqrt(3² + 4²) = 5

			// Act
			var result = vector.L2Norm();

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void L2Norm_Matrix_ReturnsCorrectResult()
		{
			// Arrange
			var matrix = new double[,] { { 3, 0 }, { 0, 4 } };
			double expected = 5; // sqrt(3² + 0² + 0² + 4²) = 5

			// Act
			var result = matrix.L2Norm();

			// Assert
			Assert.Equal(expected, result);
		}

		#endregion

		#region Null and Edge Case Tests

		[Fact]
		public void Add_NullMatrix_ThrowsArgumentNullException()
		{
			// Arrange
			double[,] matrix1 = null;
			var matrix2 = new double[,] { { 1, 2 }, { 3, 4 } };

			// Act & Assert
			Assert.Throws<NullReferenceException>(() => matrix1.Add(matrix2));
		}

		[Fact]
		public void Dot_IncompatibleDimensions_ThrowsArgumentException()
		{
			// Arrange
			var matrix = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } };
			var vector = new double[] { 1, 2 }; // Should be length 3

			// Act & Assert
			Assert.Throws<ArgumentException>(() => matrix.Dot(vector));
		}

		[Fact]
		public void GetColumnVector_InvalidIndex_ThrowsIndexOutOfRangeException()
		{
			// Arrange
			var matrix = new double[,] { { 1, 2 }, { 3, 4 } };
			int invalidIndex = 2; // Only columns 0 and 1 exist

			// Act & Assert
			Assert.Throws<IndexOutOfRangeException>(() => matrix.GetColumnVector(invalidIndex));
		}

		[Fact]
		public void Divide_VectorWithZeroElement_ThrowsDivideByZeroException()
		{
			// Arrange
			var vector1 = new double[] { 1, 2, 3 };
			var vector2 = new double[] { 1, 0, 1 }; // Contains zero

			// Act & Assert
			Assert.Throws<DivideByZeroException>(() => vector1.Divide(vector2));
		}

		#endregion
	}
}