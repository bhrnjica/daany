//////////////////////////////////////////////////////////////////////////////
//   ____    _    _   _   _   __  __                                       //
//  |  _ \  / \  | \ | | | \ | |\ \/ /                                     //
//  | | | |/ _ \ |  \| | |  \| | \  /                                      //
//  | |_| / ___ \| |\  | | |\  | | |                                       //
//  |____/_/   \_\_| \_| |_| \_| |_|                                       //
//                                                                         //
//  DAata ANalYtics Library                                                //
//  Daany.DataFrame:Implementation of DataFrame.                           //
//  https://github.com/bhrnjica/daany                                      //
//                                                                         //
//  Copyright © 20019-2025 Bahrudin Hrnjica                                //
//                                                                         //
//  Free. Open Source. MIT Licensed.                                       //
//  https://github.com/bhrnjica/daany/blob/master/LICENSE                  //
//////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace Daany
{

	public class Series : IEnumerable<object?>, ICloneable
	{
		private List<object?> _data;
		private Index _index;
		private ColType _type;

		public string Name { get; set; }
		public int Count => _data.Count;
		public Index Index => _index;
		public ColType ColType => _type;

		public IList<object?> Data => _data ?? throw new ArgumentNullException(nameof(_data));

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Series class.
		/// </summary>
		/// <param name="data">The data values for the series</param>
		/// <param name="ind">Optional index values (must match data length)</param>
		/// <param name="name">Name of the series</param>
		/// <param name="type">Data type of the series (auto-detected if not specified)</param>
		/// <example>
		/// <code>
		/// // Create series with default index
		/// var series = new Series(new List<object> {1, 2, 3, 4, 5});
		/// 
		/// // Create series with custom index
		/// var series2 = new Series(
		///     new List<object> {"A", "B", "C"}, 
		///     new List<object> {10, 20, 30},
		///     "MySeries");
		/// </code>
		/// </example>
		public Series(List<object?> data, List<object>? ind = null, string name = "series", ColType type = ColType.STR)
		{
			_data = data ?? throw new ArgumentNullException("Data list cannot be null");
			Name = name;
			_type = type;

			if (ind != null)
			{
				if (ind.Count != data.Count)
					throw new ArgumentException("Index length must match data length");
				_index = new Index(ind);
			}
			else
			{
				_index = new Index(Enumerable.Range(0, _data.Count).Select(x => (object)x).ToList());
			}

			// Auto-detect type if not specified
			if (type == ColType.STR)
			{
				_type = DetectType();
			}
		}

		/// <summary>
		/// Initializes a new instance of the Series class.
		/// </summary>
		/// <param name="data">The data values for the series</param>
		/// <param name="ind">Optional index values (must match data length)</param>
		/// <param name="name">Name of the series</param>
		/// <param name="type">Data type of the series (auto-detected if not specified)</param>
		/// <example>
		/// <code>
		/// // Create series with default index
		/// var series = new Series(new List<object> {1, 2, 3, 4, 5});
		/// 
		/// // Create series with custom index
		/// var series2 = new Series(
		///     new List<object> {"A", "B", "C"}, 
		///     new List<object> {10, 20, 30},
		///     "MySeries");
		/// </code>
		/// </example>
		internal Series(List<object?> data, Index index, string name = "series", ColType type = ColType.STR)
			: this(data, index?.ToList(), name, type) { }

		/// <summary>
		/// Creates a deep copy of the Series.
		/// </summary>
		/// <returns>A new Series with copied data and index</returns>
		/// <example>
		/// <code>
		/// var original = new Series(new List<object> {1, 2, 3});
		/// var copy = new Series(original);
		/// </code>
		/// </example>
		public Series(Series ser) : this(
			new List<object?>(ser._data),
			new List<object>(ser._index),
			ser.Name,
			ser._type)
		{ }

		#endregion

		#region Core Functionality

		public IEnumerator<object?> GetEnumerator() => _data.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public object Clone() => new Series(this);

		private ColType DetectType()
		{
			if (_data.Count == 0) return ColType.STR;

			// Find first non-null value
			var firstValue = _data.FirstOrDefault(x => x != null && x != DataFrame.NAN);
			if (firstValue == null) return ColType.STR;

			return firstValue switch
			{
				bool _ => ColType.I2,
				int _ => ColType.I32,
				long _ => ColType.I64,
				float _ => ColType.F32,
				double _ => ColType.DD,
				DateTime _ => ColType.DT,
				string _ => ColType.STR,
				_ => throw new InvalidOperationException($"Unsupported type: {firstValue.GetType()}")
			};
		}

		#endregion

		#region Indexers

		/// <summary>
		/// Creates a new Series containing only the specified indices.
		/// </summary>
		/// <param name="indexes">Array of indices to select</param>
		/// <returns>New filtered Series</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {10, 20, 30, 40});
		/// var filtered = series[0, 2]; // Returns series with values 10 and 30
		/// </code>
		/// </example>
		public Series this[params int[] indexes]
		{
			get
			{
				var lst = new List<object?>(indexes.Length);
				var ind = new List<object>(indexes.Length);

				foreach (int i in indexes)
				{
					lst.Add(Data[i]);
					ind.Add(_index[i]);
				}

				return new Series(lst, ind, Name, _type);
			}
		}

		/// <summary>
		/// Gets or sets the value at the specified position.
		/// </summary>
		/// <param name="i">Zero-based index</param>
		/// <returns>The value at the specified index</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 2, 3});
		/// var value = series[1]; // Returns 2
		/// series[2] = 99; // Changes third value to 99
		/// </code>
		/// </example>

		public object? this[int i]
		{
			get => _data[i];
			set => _data[i] = value;
		}
		/// <summary>
		/// Gets or sets the value for the specified index label.
		/// </summary>
		/// <param name="indexLabel">The index label to look up</param>
		/// <returns>The value associated with the label</returns>
		/// <example>
		/// <code>
		/// var series = new Series(
		///     new List<object> {10, 20, 30},
		///     new List<object> {"A", "B", "C"});
		/// 
		/// var value = series["B"]; // Returns 20
		/// series["C"] = 99; // Changes value for index "C" to 99
		/// </code>
		/// </example>
		public object? this[object indexLabel]
		{
			get
			{
				int idx = _index.IndexOf(indexLabel);
				if (idx == -1)
					throw new KeyNotFoundException($"Index '{indexLabel}' not found.");
				return _data[idx];
			}
			set
			{
				int idx = _index.IndexOf(indexLabel);
				if (idx == -1)
					throw new KeyNotFoundException($"Index '{indexLabel}' not found.");
				_data[idx] = value;
			}
		}

		#endregion

		#region Data Operations

		/// <summary>
		/// Appends another series vertically (row-wise).
		/// </summary>
		/// <param name="ser">Series to append</param>
		/// <returns>New combined Series</returns>
		/// <example>
		/// <code>
		/// var s1 = new Series(new List<object> {1, 2});
		/// var s2 = new Series(new List<object> {3, 4});
		/// var combined = s1.AppendVertical(s2); // Contains 1, 2, 3, 4
		/// </code>
		/// </example>
		public Series AppendVertical(Series ser)
		{
			if (ser == null) throw new ArgumentNullException(nameof(ser));
			if (ser.ColType != this.ColType)
				throw new InvalidOperationException("Cannot append series of different types");

			var newData = new List<object?>(_data.Count + ser.Count);
			newData.AddRange(_data);
			newData.AddRange(ser._data);

			var newIndex = new List<object>(_index.Count + ser.Index.Count);
			newIndex.AddRange(_index);
			newIndex.AddRange(ser.Index);

			return new Series(newData, newIndex, Name, _type);
		}

		/// <summary>
		/// Appends another series horizontally (column-wise) by creating a DataFrame.
		/// </summary>
		/// <param name="ser">Series to append</param>
		/// <returns>New DataFrame with both series as columns</returns>
		/// <example>
		/// <code>
		/// var s1 = new Series(new List<object> {1, 2}, name: "A");
		/// var s2 = new Series(new List<object> {3, 4}, name: "B");
		/// var df = s1.AppendHorizontal(s2); // DataFrame with columns A and B
		/// </code>
		/// </example>
		public DataFrame AppendHorizontal(Series ser)
		{
			var df = new DataFrame(_data, _index.ToList(), new List<string> { Name }, new[] { _type });
			return df.AddColumn(ser);
		}

		/// <summary>
		/// Computes a rolling window calculation.
		/// </summary>
		/// <param name="window">Size of the moving window</param>
		/// <param name="agg">Aggregation function to apply</param>
		/// <returns>New Series with rolling values</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 2, 3, 4, 5});
		/// var rollingMean = series.Rolling(3, Aggregation.Mean); // Contains 2, 3, 4
		/// </code>
		/// </example>
		public Series Rolling(int window, Aggregation agg)
		{
			if (window <= 0 || window > Count)
				throw new ArgumentOutOfRangeException(nameof(window), "Window size must be between 1 and the length of the series.");

			var result = new List<object?>();
			var resultIndex = new List<object>();

			// Add `null` values at the beginning to match the window size
			for (int i = 0; i < window - 1; i++)
			{
				result.Add(null);
				resultIndex.Add(_index[i]);
			}

			// Compute rolling aggregation for remaining values
			for (int i = 0; i <= Count - window; i++)
			{
				var windowData = new List<object?>();

				for (int j = 0; j < window; j++)
				{
					var value = _data[i + j];

					//Be transparent and not allow null values in rolling calculation
					if (value == null || value == DataFrame.NAN)
						throw new FormatException("Tring to calculate rolling of null value");

					windowData.Add(value);
				}

				result.Add(DataFrame._calculateAggregation(windowData!, agg, _type));
				resultIndex.Add(_index[i + window - 1]);
			}

			return new Series(result, resultIndex, $"{Name}_Rolling{window}{agg}", _type);
		}


		#endregion

		#region Conversion Methods

		/// <summary>
		/// Converts the Series to a DataFrame with one column.
		/// </summary>
		/// <returns>New DataFrame containing this series</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 2, 3}, name: "Values");
		/// var df = series.ToDataFrame(); // DataFrame with one column "Values"
		/// </code>
		/// </example>
		public DataFrame ToDataFrame()
		{
			return new DataFrame(_data, _index, new List<string> { Name }, new[] { _type });
		}

		/// <summary>
		/// Creates a Series from a DataFrame column.
		/// </summary>
		/// <param name="df">Source DataFrame</param>
		/// <param name="colName">Name of column to extract</param>
		/// <returns>New Series containing the column data</returns>
		/// <example>
		/// <code>
		/// var df = new DataFrame(new List<object[]> {new object[] {1, "A"}, new object[] {2, "B"}},
		///     columns: new List<string> {"Num", "Text"});
		/// var series = Series.FromDataFrame(df, "Num"); // Contains 1, 2
		/// </code>
		/// </example>
		public static Series FromDataFrame(DataFrame df, string colName)
		{
			if (df == null) throw new ArgumentNullException(nameof(df));
			if (string.IsNullOrEmpty(colName)) throw new ArgumentNullException(nameof(colName));

			int colIndex = df.Columns.IndexOf(colName);
			if (colIndex == -1)
				throw new KeyNotFoundException($"Column '{colName}' not found in DataFrame");

			return new Series(
				df[colName].ToList(),
				df.Index,
				colName,
				df.ColTypes[colIndex]);
		}

		#endregion

		#region Data Manipulation

		/// <summary>
		/// Adds an item to the end of the series.
		/// </summary>
		/// <param name="item">Item to add</param>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 2});
		/// series.Add(3); // Series now contains 1, 2, 3
		/// </code>
		/// </example>
		public void Add(object item) => _data.Add(item);

		/// <summary>
		/// Inserts an item at the specified index.
		/// </summary>
		/// <param name="index">Zero-based index at which to insert</param>
		/// <param name="item">Item to insert</param>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 3});
		/// series.Insert(1, 2); // Series now contains 1, 2, 3
		/// </code>
		/// </example>
		public void Insert(int index, object item) => _data.Insert(index, item);

		/// <summary>
		/// Returns the series data as a List.
		/// </summary>
		/// <returns>New List containing series values</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 2, 3});
		/// var list = series.ToList(); // Returns List<object> {1, 2, 3}
		/// </code>
		/// </example>
		public List<object?> ToList() => _data;

		/// <summary>
		/// Counts the number of missing (NA) values in the series.
		/// </summary>
		/// <returns>Number of NA values</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, DataFrame.NAN, 3});
		/// var missing = series.MissingValues(); // Returns 1
		/// </code>
		/// </example>
		public int MissingValues() => _data.Count(x => x == null || x == DataFrame.NAN);

		/// <summary>
		/// Returns a new series with NA values removed.
		/// </summary>
		/// <returns>New Series without NA values</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, DataFrame.NAN, 3});
		/// var clean = series.DropNA(); // Contains 1, 3
		/// </code>
		/// </example>
		public Series DropNA()
		{
			var data = new List<object?>();
			var index = new List<object>();

			for (int i = 0; i < _data.Count; i++)
			{
				if (_data[i] != null && _data[i] != DataFrame.NAN)
				{
					data.Add(_data[i]);
					index.Add(_index[i]);
				}
			}

			return new Series(data, index, Name, _type);
		}

		/// <summary>
		/// Returns a new series with NA values replaced by the specified value.
		/// </summary>
		/// <param name="replacedValue">Value to use for replacement</param>
		/// <returns>New Series with filled values</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, DataFrame.NAN, 3});
		/// var filled = series.FillNA(0); // Contains 1, 0, 3
		/// </code>
		/// </example>
		public Series FillNA(object replacedValue)
		{
			var data = new List<object?>(_data);
			for (int i = 0; i < data.Count; i++)
			{
				if (data[i] == null || data[i] == DataFrame.NAN)
				{
					data[i] = replacedValue;
				}
			}

			return new Series(data, new List<object>(_index), Name, _type);
		}

		/// <summary>
		/// Returns a new series with NA values replaced by an aggregated value.
		/// </summary>
		/// <param name="aggValue">Aggregation method to calculate replacement</param>
		/// <returns>New Series with filled values</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, DataFrame.NAN, 3});
		/// var filled = series.FillNA(Aggregation.Mean); // Contains 1, 2, 3
		/// </code>
		/// </example>
		public Series FillNA(Aggregation aggValue)
		{
			var validValues = _data.Where(x => x != null && x != DataFrame.NAN).ToList();
			if (validValues.Count == 0)
				return new Series(this); // No valid values to calculate aggregation

			var replacedValue = DataFrame._calculateAggregation(validValues!, aggValue, _type);
			return FillNA(replacedValue!);
		}

		/// <summary>
		/// Returns a new series with NA values replaced by values from a delegate.
		/// </summary>
		/// <param name="replacedValue">Default replacement value</param>
		/// <param name="replacementDelegate">Function that returns replacement values based on index</param>
		/// <returns>New Series with filled values</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, DataFrame.NAN, DataFrame.NAN});
		/// var filled = series.FillNA(-1, i => i * 10); 
		/// // Contains 1, 10, 20 (first NA at index 1 replaced with 10, second with 20)
		/// </code>
		/// </example>
		public Series FillNA(object replacedValue, Func<int, object> replacementDelegate)
		{
			var data = new List<object?>(_data);
			for (int i = 0; i < data.Count; i++)
			{
				if (data[i] == null || data[i] == DataFrame.NAN)
				{
					data[i] = replacementDelegate(i);
				}
			}

			return new Series(data, new List<object>(_index), Name, _type);
		}

		#endregion

		#region Aggregations

		public object Aggregate(Aggregation agg)
		{
			object? result = DataFrame._calculateAggregation(_data!, agg, _type);
			if (result == null || result == DataFrame.NAN)
				return 0;
			return result;
		}
		/// <summary>
		/// Computes the sum of the series values.
		/// </summary>
		/// <returns>Sum of all numeric values</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 2, 3});
		/// var sum = series.Sum(); // Returns 6
		/// </code>
		/// </example>
		public double Sum()
		{
			if (_data == null || _data.Count == 0)
				return double.NaN;

			return _data.Where(IsNumeric).Sum(Convert.ToDouble);
		}

		/// <summary>
		/// Computes the arithmetic mean of the series values.
		/// </summary>
		/// <returns>Average of all numeric values</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 2, 3});
		/// var avg = series.Mean(); // Returns 2
		/// </code>
		/// </example>
		public double Mean()
		{
			if (_data == null || _data.Count == 0)
				return double.NaN;
			return _data.Where(IsNumeric).Average(Convert.ToDouble);
		}

		/// <summary>
		/// Computes the median of the series values.
		/// </summary>
		/// <returns>Median of all numeric values</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 2, 3, 4});
		/// var median = series.Median(); // Returns 2.5
		/// </code>
		/// </example>
		public double Median()
		{
			if (_data == null || _data.Count == 0)
				return double.NaN;

			var numericValues = _data.Where(IsNumeric).Select(Convert.ToDouble).OrderBy(x => x).ToList();
			if (numericValues.Count == 0) return double.NaN;

			int mid = numericValues.Count / 2;
			return numericValues.Count % 2 == 0 ?
				(numericValues[mid - 1] + numericValues[mid]) / 2.0 :
				numericValues[mid];
		}

		/// <summary>
		/// Filters the series using the specified predicate.
		/// </summary>
		/// <param name="predicate">Function to test each element</param>
		/// <returns>New Series with filtered values</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 2, 3, 4});
		/// var filtered = series.Filter(x => (int)x > 2); // Contains 3, 4
		/// </code>
		/// </example>
		public Series Filter(Func<object?, bool> predicate)
		{
			var filteredData = new List<object?>();
			var filteredIndex = new List<object>();

			for (int i = 0; i < _data.Count; i++)
			{
				if (predicate(_data[i]))
				{
					filteredData.Add(_data[i]);
					filteredIndex.Add(_index[i]);
				}
			}

			return new Series(filteredData, filteredIndex, Name, _type);
		}

		/// <summary>
		/// Determines if an object is a numeric type that can be used in arithmetic operations
		/// </summary>
		private static bool IsNumeric(object? value)
		{
			if (value == null || value == DataFrame.NAN)
				return false;

			return value is int || value is long || value is float || value is double
				   || value is short || value is decimal;
		}
		#endregion

		#region Operators
		/// <summary>
		/// Element-wise subtraction of two series
		/// </summary>
		/// <param name="ser1"></param>
		/// <param name="ser2"></param>
		/// <returns></returns>
		public static Series operator -(Series ser1, Series ser2)
		{
			var t = ser1.DetectType();
			var ser3 = new Series(ser1);
			switch (t)
			{
				case ColType.I2:
					throw new Exception("Series is of boolean type. Substraction cannot be applied.");
				case ColType.IN:
					throw new Exception("Series is of categorical type. Substraction cannot be applied.");
				case ColType.STR:
					throw new Exception("Series is of string type. Substraction cannot be applied.");

				case ColType.I32:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToInt32(ser1[i]) - Convert.ToInt32(ser2[i]);
						}
					}
					break;
				case ColType.I64:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToInt64(ser1[i]) - Convert.ToInt64(ser2[i]);
						}
					}
					break;
				case ColType.F32:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToSingle(ser1[i]) - Convert.ToSingle(ser2[i]);
						}
					}
					break;
				case ColType.DD:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToDouble(ser1[i]) - Convert.ToDouble(ser2[i]);
						}
					}
					break;
				case ColType.DT:
					throw new Exception("Series is of datetime type. Substraction cannot be applied.");
				default:
					throw new Exception("Series is of unknown type. Substraction cannot be applied.");
			}
			return ser3;
		}

		/// <summary>
		/// Element-wise addition of two series
		/// </summary>
		/// <param name="ser1"></param>
		/// <param name="ser2"></param>
		/// <returns></returns>
		public static Series operator +(Series ser1, Series ser2)
		{
			var t = ser1.DetectType();
			var ser3 = new Series(ser1);
			switch (t)
			{
				case ColType.I2:
					throw new Exception("Series is of boolean type. Addition cannot be applied.");
				case ColType.IN:
					throw new Exception("Series is of categorical type. Addition cannot be applied.");
				case ColType.STR:
					throw new Exception("Series is of string type. Addition cannot be applied.");

				case ColType.I32:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToInt32(ser1[i]) + Convert.ToInt32(ser2[i]);
						}
					}
					break;
				case ColType.I64:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToInt64(ser1[i]) + Convert.ToInt64(ser2[i]);
						}
					}
					break;
				case ColType.F32:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToSingle(ser1[i]) + Convert.ToSingle(ser2[i]);
						}
					}
					break;
				case ColType.DD:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToDouble(ser1[i]) + Convert.ToDouble(ser2[i]);
						}
					}
					break;
				case ColType.DT:
					throw new Exception("Series is of datetime type. Addition cannot be applied.");
				default:
					throw new Exception("Series is of unknown type. Addition cannot be applied.");
			}
			return ser3;
		}

		/// <summary>
		/// Addition series element with floating scalar value
		/// </summary>
		/// <param name="ser1"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Series operator +(Series ser1, float scalar)
		{
			var t = ser1.DetectType();
			var ser3 = new Series(ser1);
			switch (t)
			{
				case ColType.I2:
					throw new Exception("Series is of boolean type. Addition cannot be applied.");
				case ColType.IN:
					throw new Exception("Series is of categorical type. Addition cannot be applied.");
				case ColType.STR:
					throw new Exception("Series is of string type. Addition cannot be applied.");

				case ColType.I32:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToInt32(ser1[i]) + scalar;
						}
					}
					break;
				case ColType.I64:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToInt64(ser1[i]) + scalar;
						}
					}
					break;
				case ColType.F32:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToSingle(ser1[i]) + scalar;
						}
					}
					break;
				case ColType.DD:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToDouble(ser1[i]) + scalar;
						}
					}
					break;
				case ColType.DT:
					throw new Exception("Series is of datetime type. Addition cannot be applied.");
				default:
					throw new Exception("Series is of unknown type. Addition cannot be applied.");
			}
			return ser3;
		}

		/// <summary>
		/// Element-wise multiplication of two series
		/// </summary>
		/// <param name="ser1"></param>
		/// <param name="ser2"></param>
		/// <returns></returns>
		public static Series operator *(Series ser1, Series ser2)
		{
			var t = ser1.DetectType();
			var ser3 = new Series(ser1);
			switch (t)
			{
				case ColType.I2:
					throw new Exception("Series is of boolean type. Multiplication cannot be applied.");
				case ColType.IN:
					throw new Exception("Series is of categorical type. Multiplication cannot be applied.");
				case ColType.STR:
					throw new Exception("Series is of string type. Multiplication cannot be applied.");

				case ColType.I32:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToInt32(ser1[i]) * Convert.ToInt32(ser2[i]);
						}
					}
					break;
				case ColType.I64:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToInt64(ser1[i]) * Convert.ToInt64(ser2[i]);
						}
					}
					break;
				case ColType.F32:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToSingle(ser1[i]) * Convert.ToSingle(ser2[i]);
						}
					}
					break;
				case ColType.DD:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToDouble(ser1[i]) * Convert.ToDouble(ser2[i]);
						}
					}
					break;
				case ColType.DT:
					throw new Exception("Series is of datetime type. Multiplication cannot be applied.");
				default:
					throw new Exception("Series is of unknown type. Multiplication cannot be applied.");
			}
			return ser3;
		}

		/// <summary>
		/// Multiplication series element with floating scalar value
		/// </summary>
		/// <param name="ser1"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static Series operator *(Series ser1, float scalar)
		{
			var t = ser1.DetectType();

			var ser3 = new Series(ser1);
			switch (t)
			{
				case ColType.I2:
					throw new Exception("Series is of boolean type. Multiplication cannot be applied.");
				case ColType.IN:
					throw new Exception("Series is of categorical type. Multiplication cannot be applied.");
				case ColType.STR:
					throw new Exception("Series is of string type. Multiplication cannot be applied.");

				case ColType.I32:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToInt32(ser1[i]) * scalar;
						}
					}
					break;
				case ColType.I64:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToInt64(ser1[i]) * scalar;
						}
					}
					break;
				case ColType.F32:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToSingle(ser1[i]) + scalar;
						}
					}
					break;
				case ColType.DD:
					{
						for (int i = 0; i < ser1._data.Count; i++)
						{
							ser3[i] = Convert.ToDouble(ser1[i]) * scalar;
						}
					}
					break;
				case ColType.DT:
					throw new Exception("Series is of datetime type. Multiplication cannot be applied.");
				default:
					throw new Exception("Series is of unknown type. Multiplication cannot be applied.");
			}
			return ser3;
		}
		
		#endregion

		#region Time Series Specific
		/// <summary>
		/// Creates a time series regression dataset from the series.
		/// </summary>
		/// <param name="order">Number of lagged observations</param>
		/// <returns>Tuple containing X matrix (with intercept) and Y vector</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 2, 3, 4, 5});
		/// var (X, Y) = series.ToRegressors(2);
		/// // X will contain:
		/// // [1, 1, 2]
		/// // [1, 2, 3]
		/// // Y will contain:
		/// // [3]
		/// // [4]
		/// </code>
		/// </example>
		public (float[,] X, float[,] Y) ToRegressors(int order)
		{
			if (order <= 0 || order >= Count)
				throw new ArgumentOutOfRangeException(nameof(order), "Order must be between 1 and series length-1");

			int rows = Count - order;
			float[,] X = new float[rows, order + 1]; // +1 for intercept
			float[,] Y = new float[rows, 1];

			for (int i = 0; i < rows; i++)
			{
				X[i, 0] = 1.0f; // Intercept

				for (int j = 1; j <= order; j++)
				{
					X[i, j] = Convert.ToSingle(_data[i + j - 1]);
				}

				Y[i, 0] = Convert.ToSingle(_data[i + order]);
			}

			return (X, Y);
		}

		/// <summary>
		/// Creates a lagged time series DataFrame.
		/// </summary>
		/// <param name="lags">Number of lagged observations to create</param>
		/// <returns>DataFrame with lagged features and target</returns>
		/// <example>
		/// <code>
		/// var series = new Series(new List<object> {1, 2, 3, 4}, name: "TS");
		/// var df = series.TSToDataFrame(2);
		/// // DataFrame will have columns:
		/// // "TS-L2", "TS-L1", "TS"
		/// // And values:
		/// // 1, 2, 3
		/// // 2, 3, 4
		/// </code>
		/// </example>
		internal DataFrame TSToDataFrame(int lags)
		{
			if (lags <= 0 || lags >= Count)
				throw new ArgumentOutOfRangeException(nameof(lags), "Lags must be between 1 and series length-1");

			var columns = new List<string>();
			for (int i = 0; i < lags; i++)
				columns.Add($"{Name}-L{lags - i}");
			columns.Add(Name); // Target column

			int rows = Count - lags;
			var values = new List<object?>();

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j <= lags; j++)
				{
					values.Add(_data[i + j]);
				}
			}

			return new DataFrame(values, columns);
		}

		#endregion
	}
}
