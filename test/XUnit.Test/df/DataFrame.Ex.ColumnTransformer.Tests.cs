using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;
using Daany;
using Daany.Ext;

namespace Unit.Test.DF;
public class ColumnTransformationTests
{
	[Fact]
	public void TransformColumn_None_ShouldReturnOriginalDataFrame_NoEncoding()
	{
		var df = CreateMockDataFrame();
		var (resultDf, scaledValues, labels) = df.TransformColumn("Category", ColumnTransformer.None);

		Assert.Equal(df, resultDf);
		Assert.Null(scaledValues);
		Assert.Null(labels);
	}

	[Fact]
	public void TransformColumn_Binary1_ShouldReturnEncodedLabels_Binary1()
	{
		var df = CreateMockDataFrame();
		var (resultDf, scaledValues, labels) = df.TransformColumn("Category", ColumnTransformer.Binary1);
		int[] expected = [0,1,2,0,1];

		Assert.NotNull(resultDf);
		Assert.Null(scaledValues);
		Assert.NotNull(labels);
		Assert.True(labels.Length > 0);
		Assert.Equal(expected, resultDf["Category_cvalues"].Select(x=>(int)x));
	}

	[Fact]
	public void TransformColumn_Binary1_ShouldReturnEncodedLabels_Binary2()
	{
		var df = CreateMockDataFrame();
		var (resultDf, scaledValues, labels) = df.TransformColumn("Category", ColumnTransformer.Binary2);
		int[] expected = [-1, 1, 2, -1, 1];

		Assert.NotNull(resultDf);
		Assert.Null(scaledValues);
		Assert.NotNull(labels);
		Assert.True(labels.Length > 0);
		Assert.Equal(expected, resultDf["Category_cvalues"].Select(x => (int)x));
	}

	[Fact]
	public void TransformColumn_Binary1_ShouldReturnEncodedLabels_Dummy()
	{
		var df = CreateMockDataFrame();
		var (resultDf, scaledValues, labels) = df.TransformColumn("Category", ColumnTransformer.Dummy);
		int[] expectedA = [1, 0, 0, 1, 0];
		int[] expectedB = [0, 1, 0, 0, 1];
		//int[] expectedC = [0, 0, 1, 0, 0];-no C column in this encoding

		Assert.NotNull(resultDf);
		Assert.Null(scaledValues);
		Assert.NotNull(labels);
		Assert.True(labels.Length > 0);
		Assert.Equal(expectedA, resultDf["A"].Select(x => (int)x));
		Assert.Equal(expectedB, resultDf["B"].Select(x => (int)x));
	}

	[Fact]
	public void TransformColumn_Binary1_ShouldReturnEncodedLabels_OneHot()
	{
		var df = CreateMockDataFrame();
		var (resultDf, scaledValues, labels) = df.TransformColumn("Category", ColumnTransformer.OneHot);
		int[] expectedA = [1, 0, 0, 1, 0];
		int[] expectedB = [0, 1, 0, 0, 1];
		int[] expectedC = [0, 0, 1, 0, 0];

		Assert.NotNull(resultDf);
		Assert.Null(scaledValues);
		Assert.NotNull(labels);
		Assert.True(labels.Length > 0);
		Assert.Equal(expectedA, resultDf["A"].Select(x => (int)x));
		Assert.Equal(expectedB, resultDf["B"].Select(x => (int)x));
		Assert.Equal(expectedC, resultDf["C"].Select(x => (int)x));
	}


	[Fact]
	public void TransformColumn_MinMax_ShouldReturnMinMaxScaledValues()
	{
		var df = CreateMockDataFrame();
		var (resultDf, scaledValues, labels) = df.TransformColumn("Value", ColumnTransformer.MinMax);
		double[] expected = [0, 0.25, 0.5, 0.75, 1.0];
		Assert.NotNull(resultDf);
		Assert.NotNull(scaledValues);
		Assert.Null(labels);
		Assert.All(scaledValues, v => Assert.InRange(v, 1, 5));
		Assert.Equal(expected, resultDf["Value_scaled"].Select(x => (double)x));
	}

	[Fact]
	public void TransformColumn_MinMax_ShouldReturnStandardizedScaledValues()
	{
		var df = CreateMockDataFrame();
		var (resultDf, scaledValues, labels) = df.TransformColumn("Value", ColumnTransformer.Standardizer);
		double[] expected = [-1.264911, 0.632455, 0, 0.632455, 1.264911];
		Assert.NotNull(resultDf);
		Assert.NotNull(scaledValues);
		Assert.Null(labels);
		Assert.All(scaledValues, v => Assert.InRange(v, 1, 5));
		//Assert.Equal(expected, resultDf["Value_scaled"].Select(x => (double)x), 5);
	}


	[Fact]
	public void TransformColumn_Standardizer_ShouldReturnStandardizedScaledValues()
	{
		var df = CreateMockDataFrame();
		var (resultDf, scaledValues, labels) = df.TransformColumn("Value", ColumnTransformer.Standardizer);

		double[] expected = [-1.264911, -0.632455, 0, 0.632455, 1.264911];

		Assert.NotNull(resultDf);
		Assert.NotNull(scaledValues);
		Assert.Null(labels);

		// Fix range assertion: Standardized values should be roughly between -3 and 3
		Assert.All(scaledValues, v => Assert.InRange(v, -3.0f, 3.0f));

		// Fix equality assertion using precision tolerance
		Assert.Equal(expected.Length, resultDf["Value_scaled"].Count());
		var actual = resultDf["Value_scaled"].Select(x => (double)x).ToArray();
		for (int i = 0; i < expected.Length; i++)
		{
			Assert.InRange(Math.Abs(expected[i] - actual[i]), 0, 1E-5); // Allow small precision error
		}
	}


	[Fact]
	public void TransformColumn_InvalidTransformer_ShouldThrowException()
	{
		var df = CreateMockDataFrame();

		Assert.Throws<NotSupportedException>(() =>
			df.TransformColumn("Category", (ColumnTransformer)999) // Invalid transformer
		);
	}

	private DataFrame CreateMockDataFrame()
	{
		var data = new Dictionary<string, List<object>>
		{
			{ "Category", [ "A", "B", "C", "A", "B" ] },
			{ "Value", [ 1.0, 2.0, 3.0, 4.0, 5.0 ] }
		};
		return new DataFrame(data);
	}
}

