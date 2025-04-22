using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using System.Globalization;
using XPlot.Plotly;
using Daany.Stat.SSA;
using System.Threading.Tasks;

namespace Unit.Test.DF
{
	using Xunit;

	public class DataFrameToStringTests
	{
		[Fact]
		public void ToString_ShouldReturnCorrectSize()
		{
			var df = new DataFrame(
				("col1", new object[] { 1, 2, 3 }),
				("col2", new object[] { "A", "B", "C" }));

			Assert.Equal("(3,2)", df.ToString());
		}

		[Fact]
		public void ToStringBuilder_ShouldFormatCorrectly()
		{
			var df = new DataFrame(
				("col1", new object[] { 1, 2, 3 }),
				("col2", new object[] { "X", "Y", "Z" }));

			string output = df.ToStringBuilder(3);

			Assert.Contains("col1", output);
			Assert.Contains("col2", output);
			Assert.Contains("1", output);
			Assert.Contains("X", output);
		}

		[Fact]
		public void ToConsole_ShouldReturnConsoleFormattedString()
		{
			var df = new DataFrame(
				("col1", new object[] { 1, 2 }),
				("col2", new object[] { "A", "B" }));

			string output = df.ToConsole(2);

			Assert.Contains("index", output);
			Assert.Contains("col1", output);
			Assert.Contains("col2", output);
			Assert.Contains("1", output);
			Assert.Contains("A", output);
		}
	}

}
