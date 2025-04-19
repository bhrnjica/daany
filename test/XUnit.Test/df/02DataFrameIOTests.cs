using Daany;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;


namespace Unit.Test.DF
{
	public class DataFrameIOTests
	{
		private const string TestCsvPath = "test.csv";
		private const string TestUrl = "http://example.com/data.csv";

		[Fact]
		public void ToCsv_WithNullDataFrame_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => DataFrame.ToCsv(TestCsvPath, null));
		}

		[Fact]
		public void ToCsv_WithEmptyDataFrame_CreatesEmptyFile()
		{
			//list of object
			var list = new object[] {};
			//defined columns
			var cols = new string[1] {"c" };
			//create empty data frame
			var df = new DataFrame(list, cols);

			DataFrame.ToCsv(TestCsvPath, df);

			Assert.True(File.Exists(TestCsvPath));
			var lines = File.ReadAllLines(TestCsvPath);
			Assert.Single(lines); // Just header
		}

		[Fact]
		public void ToCsv_WithData_WritesCorrectContent()
		{
			var values = new List<object> { 1, "test", 3.14, DateTime.ParseExact("2023-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture) };
			var columns = new List<string> { "Int", "String", "Double", "Date" };
			var colTypes = new ColType[] { ColType.I32, ColType.STR, ColType.DD, ColType.DT };
			var df = new DataFrame(values, columns, colTypes);

			DataFrame.ToCsv(TestCsvPath, df, dateFormat: "dd/MM/yyyy");

			var lines = File.ReadAllLines(TestCsvPath);
			Assert.Equal(2, lines.Length);
			Assert.Equal("Int,String,Double,Date", lines[0]);
			Assert.Contains("1,test,3.14,01/01/2023", lines[1]);
		}

		[Fact]
		public void ToCsv_WithData_WritesCorrectContentVer1()
		{
			var values = new List<object> 
			{
				1,
				"test",
				3.14,
				DateTime.ParseExact("2023-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture)
			};
			var columns = new List<string> { "Int", "String", "Double", "Date" };
			var colTypes = new ColType[] { ColType.I32, ColType.STR, ColType.DD, ColType.DT };
			var df = new DataFrame(values, columns, colTypes);

			DataFrame.ToCsv(TestCsvPath, df, dateFormat: "yyyy-MM-dd");

			var lines = File.ReadAllLines(TestCsvPath);
			Assert.Equal(2, lines.Length);
			Assert.Equal("Int,String,Double,Date", lines[0]);
			Assert.Equal("1,test,3.14,2023-01-01", lines[1]); // Now only contains date
		}

		[Fact]
		public void ToCsv_WithDateFormat_UsesFormat()
		{
			var values = new List<object> { DateTime.Parse("2023-01-01") };
			var columns = new List<string> { "Date" };
			var colTypes = new ColType[] { ColType.DT };
			var df = new DataFrame(values, columns, colTypes);

			DataFrame.ToCsv(TestCsvPath, df, dateFormat: "yyyy-MM-dd");

			var lines = File.ReadAllLines(TestCsvPath);
			Assert.Contains("2023-01-01", lines[1]);
		}

		[Fact]
		public async Task ToCsvAsync_WithData_WritesCorrectContent()
		{
			var values = new List<object> { 1, "test" };
			var columns = new List<string> { "Int", "String" };
			var df = new DataFrame(values, columns, null);

			await DataFrame.ToCsvAsync(TestCsvPath, df);

			var lines = await File.ReadAllLinesAsync(TestCsvPath);
			Assert.Equal(2, lines.Length);
			Assert.Equal("Int,String", lines[0]);
			Assert.Equal("1,test", lines[1]);
		}

		[Fact]
		public void FromCsv_WithInvalidPath_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => DataFrame.FromCsv("nonexistent.csv"));
		}

		[Fact]
		public void FromCsv_WithHeaderOnly_CreatesEmptyDataFrame()
		{
			File.WriteAllText(TestCsvPath, "Col1,Col2");

			var df = DataFrame.FromCsv(TestCsvPath);

			Assert.Equal(2, df.ColCount());
			Assert.Equal(0, df.RowCount());
		}

		[Fact]
		public void FromCsv_WithData_ParsesCorrectly()
		{
			File.WriteAllText(TestCsvPath, "Int,String\n1,test\n2,another");

			var df = DataFrame.FromCsv(TestCsvPath);

			Assert.Equal(2, df.RowCount());
			Assert.Equal(1, df["Int", 0]);
			Assert.Equal("test", df["String", 0]);
		}

		[Fact]
		public void FromCsv_WithCustomTypes_ParsesCorrectly()
		{
			File.WriteAllText(TestCsvPath, "Int,Float,Date\n1,3.14,2023-01-01");
			var colTypes = new ColType[] { ColType.I32, ColType.F32, ColType.DT };

			var df = DataFrame.FromCsv(TestCsvPath, colTypes: colTypes);

			Assert.IsType<int>(df["Int", 0]);
			Assert.IsType<float>(df["Float", 0]);
			Assert.IsType<DateTime>(df["Date", 0]);
		}

		[Fact]
		public void FromCsv_WithMissingValues_HandlesCorrectly()
		{
			File.WriteAllText(TestCsvPath, "Value\n1\n?\n3");

			var df = DataFrame.FromCsv(TestCsvPath, missingValues: new char[] { '?' });

			Assert.Equal(1, df["Value", 0]);
			Assert.Equal(DataFrame.NAN, df["Value", 1]);
			Assert.Equal(3, df["Value", 2]);
		}

		[Fact]
		public void FromText_WithValidData_ParsesCorrectly()
		{
			var text = "Int,String\n1,test\n2,another";

			var df = DataFrame.FromText(text);

			Assert.Equal(2, df.RowCount());
			Assert.Equal(1, df["Int", 0]);
			Assert.Equal("test", df["String", 0]);
		}

		[Fact]
		public void FromStrings_WithValidData_ParsesCorrectly()
		{
			var lines = new string[] { "Int,String", "1,test", "2,another" };

			var df = DataFrame.FromStrings(lines);

			Assert.Equal(2, df.RowCount());
			Assert.Equal(1, df["Int", 0]);
			Assert.Equal("test", df["String", 0]);
		}

		[Fact]
		public async Task FromWebAsync_WithMockData_ParsesCorrectly()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When(TestUrl)
				.Respond("text/csv", "Int,String\n1,test\n2,another");

			var httpClient = new HttpClient(mockHttp);
			var testDataFrame = new TestableDataFrame(httpClient);

			// Act
			var df = await testDataFrame.FromWebAsync(TestUrl);

			// Assert
			Assert.Equal(2, df.RowCount());
			Assert.Equal(1, df["Int", 0]);
			Assert.Equal("test", df["String", 0]);
			Assert.Equal(2, df["Int", 1]);
			Assert.Equal("another", df["String", 1]);
		}


		[Fact]
		public void ParseValue_WithInt_ReturnsInt()
		{
			var result = DataFrame.ParseValue("123".AsSpan(), null, ColType.I32);
			Assert.IsType<int>(result);
			Assert.Equal(123, result);
		}

		[Fact]
		public void ParseValue_WithFloat_ReturnsFloat()
		{
			var result = DataFrame.ParseValue("123.45".AsSpan(), null, ColType.F32);
			Assert.IsType<float>(result);
			Assert.Equal(123.45f, result);
		}

		[Fact]
		public void ParseValue_WithDate_ReturnsDateTime()
		{
			var result = DataFrame.ParseValue("2023-01-01".AsSpan(), null, ColType.DT);
			Assert.IsType<DateTime>(result);
			Assert.Equal(2023, ((DateTime)result).Year);
		}

		[Fact]
		public void ParseValue_WithMissingValue_ReturnsNAN()
		{
			var result = DataFrame.ParseValue("?".AsSpan(), new char[] { '?' }, ColType.I32);
			Assert.Equal(DataFrame.NAN, result);
		}

		[Fact]
		public void IsMissingValue_WithVariousInputs_ReturnsCorrectly()
		{
			Assert.True(DataFrame.IsMissingValue("".AsSpan(), null));
			Assert.True(DataFrame.IsMissingValue("?".AsSpan(), new char[] { '?' }));
			Assert.False(DataFrame.IsMissingValue("value".AsSpan(), null));
			Assert.False(DataFrame.IsMissingValue("123".AsSpan(), new char[] { '?' }));
		}

		[Fact]
		public void IsNumeric_WithVariousInputs_ReturnsCorrectly()
		{
			Assert.Equal(Daany.ValueType.Int, DataFrame.IsNumeric("123".AsSpan()));
			Assert.Equal(Daany.ValueType.Float, DataFrame.IsNumeric("123.45".AsSpan()));
			Assert.Equal(Daany.ValueType.None, DataFrame.IsNumeric("abc".AsSpan()));
			Assert.Equal(Daany.ValueType.Int, DataFrame.IsNumeric("-123".AsSpan()));
			Assert.Equal(Daany.ValueType.Float, DataFrame.IsNumeric("+123.45".AsSpan()));
		}

		public void Dispose()
		{
			if (File.Exists(TestCsvPath))
			{
				File.Delete(TestCsvPath);
			}
		}
	}
}
