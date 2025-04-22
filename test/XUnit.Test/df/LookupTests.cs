using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Ext;
using Daany.Multikey;


namespace Unit.Test.DF
{
	public class LookupTests
	{
		public class Person
		{
			public string Name { get; set; }
			public int Age { get; set; }
			public string City { get; set; }
		}

		private readonly List<Person> _testData = new()
	{
		new Person { Name = "Alice", Age = 25, City = "New York" },
		new Person { Name = "Bob", Age = 30, City = "Chicago" },
		new Person { Name = "Charlie", Age = 25, City = "New York" },
		new Person { Name = "Diana", Age = 30, City = "Chicago" }
	};

		[Fact]
		public void TwoKeyLookup_ShouldGroupCorrectly()
		{
			// Arrange
			var lookup = new TwoKeyLookup<string, int, Person>(
				_testData,
				p => (p.City, p.Age)
			);

			// Act & Assert
			var ny25 = lookup["New York", 25].ToList();
			Assert.Equal(2, ny25.Count);
			Assert.Contains(ny25, p => p.Name == "Alice");
			Assert.Contains(ny25, p => p.Name == "Charlie");

			var chicago30 = lookup["Chicago", 30].ToList();
			Assert.Equal(2, chicago30.Count);
			Assert.Contains(chicago30, p => p.Name == "Bob");
			Assert.Contains(chicago30, p => p.Name == "Diana");

			Assert.Empty(lookup["Paris", 20]);
		}

		[Fact]
		public void ThreeKeyLookup_ShouldGroupCorrectly()
		{
			// Arrange
			var extendedData = _testData.Concat(new[]
			{
			new Person { Name = "Eve", Age = 25, City = "New York" }
		});

			var lookup = new ThreeKeyLookup<string, int, string, Person>(
				extendedData,
				p => (p.City, p.Age, p.Name.Substring(0, 1))
			);

			// Act & Assert
			var ny25A = lookup["New York", 25, "A"].ToList();
			Assert.Single(ny25A);
			Assert.Equal("Alice", ny25A[0].Name);

			var ny25C = lookup["New York", 25, "C"].ToList();
			Assert.Single(ny25C);
			Assert.Equal("Charlie", ny25C[0].Name);

			Assert.Empty(lookup["New York", 25, "X"]);
		}

		[Fact]
		public void Lookup_Contains_ShouldReturnCorrectResults()
		{
			// Arrange
			var lookup = new TwoKeyLookup<string, int, Person>(
				_testData,
				p => (p.City, p.Age)
			);

			// Act & Assert
			Assert.True(lookup.Contains("New York", 25));
			Assert.True(lookup.Contains("Chicago", 30));
			Assert.False(lookup.Contains("Paris", 20));
		}
	}

}
