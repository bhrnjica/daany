using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Ext;
using Daany.Multikey;

namespace Unit.Test.DF
{
	public class MultiKeyDictionaryTests
	{
		[Fact]
		public void TwoKeysDictionary_ShouldWorkCorrectly()
		{
			// Arrange
			var dict = new TwoKeysDictionary<string, int, string>();

			// Act - Add items
			dict.Add("Group1", 1, "Value1");
			dict.Add("Group1", 2, "Value2");
			dict.Add("Group2", 1, "Value3");

			// Assert
			Assert.Equal("Value1", dict["Group1", 1]);
			Assert.Equal("Value2", dict["Group1", 2]);
			Assert.Equal("Value3", dict["Group2", 1]);

			// Test TryGetValue
			Assert.True(dict.TryGetValue("Group1", 1, out var value));
			Assert.Equal("Value1", value);
			Assert.False(dict.TryGetValue("Group3", 1, out _));

			// Test ContainsKey
			Assert.True(dict.ContainsKey("Group1", 2));
			Assert.False(dict.ContainsKey("Group1", 3));

			// Test Remove
			Assert.True(dict.Remove("Group1", 1));
			Assert.False(dict.ContainsKey("Group1", 1));
			Assert.Equal(2, dict.Count);

			// Test indexer setter
			dict["Group2", 2] = "Value4";
			Assert.Equal("Value4", dict["Group2", 2]);
		}

		[Fact]
		public void ThreeKeysDictionary_ShouldWorkCorrectly()
		{
			// Arrange
			var dict = new ThreeKeysDictionary<string, int, DateTime, string>();

			var date1 = DateTime.Today;
			var date2 = DateTime.Today.AddDays(1);

			// Act - Add items
			dict.Add("Group1", 1, date1, "Value1");
			dict.Add("Group1", 2, date1, "Value2");
			dict.Add("Group1", 1, date2, "Value3");

			// Assert
			Assert.Equal("Value1", dict["Group1", 1, date1]);
			Assert.Equal("Value3", dict["Group1", 1, date2]);

			// Test TryGetValue
			Assert.True(dict.TryGetValue("Group1", 2, date1, out var value));
			Assert.Equal("Value2", value);

			// Test ContainsKey
			Assert.True(dict.ContainsKey("Group1", 1, date2));
			Assert.False(dict.ContainsKey("Group1", 3, date1));

			// Test Remove
			Assert.True(dict.Remove("Group1", 1, date1));
			Assert.False(dict.ContainsKey("Group1", 1, date1));
			Assert.Equal(1, dict.Count);

			// Test indexer setter
			dict["Group1", 3, date1] = "Value4";
			Assert.Equal("Value4", dict["Group1", 3, date1]);
		}

		[Fact]
		public void MultiKeyDictionary_ShouldHandleEdgeCases()
		{
			// Arrange
			var dict = new TwoKeysDictionary<string, int, string>();
			dict.Add("Group1", 1, "Value1");

			// Act & Assert - Duplicate key
			Assert.Throws<ArgumentException>(() => dict.Add("Group1", 1, "Value2"));

			// Nonexistent key access should return null
			Assert.Null(dict["Nonexistent", 1]);

			// Safe access via TryGetValue for nonexistent key
			Assert.False(dict.TryGetValue("Nonexistent", 1, out _));

			// Empty dictionary
			var emptyDict = new TwoKeysDictionary<string, int, string>();
			Assert.False(emptyDict.ContainsKey("A", 1));
			Assert.Equal(0, emptyDict.Count);
		}

		[Fact]
		public void GetAllKeys_ShouldReturnAllKeyCombinations()
		{
			// Arrange
			var dict = new ThreeKeysDictionary<string, int, bool, string>();
			dict.Add("A", 1, true, "Val1");
			dict.Add("A", 2, false, "Val2");
			dict.Add("B", 1, true, "Val3");

			// Act
			var keys = dict.GetAllKeys().ToList();

			// Assert
			Assert.Equal(3, keys.Count);
			Assert.Contains(new object[] { "A", 1, true }, keys);
			Assert.Contains(new object[] { "A", 2, false }, keys);
			Assert.Contains(new object[] { "B", 1, true }, keys);
		}
	}

}
