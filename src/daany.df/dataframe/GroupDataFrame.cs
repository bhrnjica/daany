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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Daany.Multikey;

namespace Daany.Grouping
{
	/// <summary>
	/// Represents a grouped collection of DataFrames with operations that can be performed on each group.
	/// Supports grouping by 1, 2, or 3 columns.
	/// </summary>
	public class GroupDataFrame
	{
		public string GroupedColumn { get; }
		public string SecondGroupedColumn { get; }
		public string ThirdGroupedColumn { get; }

		internal Dictionary<object, DataFrame> Groups { get; }
		internal TwoKeysDictionary<object, object, DataFrame> Groups2 { get; }
		internal ThreeKeysDictionary<object, object, object, DataFrame> Groups3 { get; }

		/// <summary>
		/// Initializes a new instance of GroupDataFrame grouped by a single column
		/// </summary>
		/// <param name="groupedColumn">Name of the column used for grouping</param>
		/// <param name="groups">Dictionary containing the grouped DataFrames</param>
		internal GroupDataFrame(string groupedColumn, Dictionary<object, DataFrame> groups)
		{
			GroupedColumn = groupedColumn;
			SecondGroupedColumn = null!;
			ThirdGroupedColumn = null!;

			Groups = groups!;
			Groups2 = null!;
			Groups3 = null!;
		}

		/// <summary>
		/// Initializes a new instance of GroupDataFrame grouped by two columns
		/// </summary>
		/// <param name="firstGroupedColumn">Name of the first column used for grouping</param>
		/// <param name="secondGroupedColumn">Name of the second column used for grouping</param>
		/// <param name="groups">Two-level dictionary containing the grouped DataFrames</param>
		internal GroupDataFrame(string firstGroupedColumn, string secondGroupedColumn, TwoKeysDictionary<object, object, DataFrame> groups)
		{
			GroupedColumn = firstGroupedColumn;
			SecondGroupedColumn = secondGroupedColumn;
			ThirdGroupedColumn = null!;

			Groups = null!;
			Groups2 = groups;
			Groups3 = null!;
		}

		/// <summary>
		/// Initializes a new instance of GroupDataFrame grouped by three columns
		/// </summary>
		/// <param name="firstGroupedColumn">Name of the first column used for grouping</param>
		/// <param name="secondGroupedColumn">Name of the second column used for grouping</param>
		/// <param name="thirdGroupedColumn">Name of the third column used for grouping</param>
		/// <param name="groups">Three-level dictionary containing the grouped DataFrames</param>
		internal GroupDataFrame(string firstGroupedColumn, string secondGroupedColumn, string thirdGroupedColumn, ThreeKeysDictionary<object, object, object, DataFrame> groups)
		{
			GroupedColumn = firstGroupedColumn;
			SecondGroupedColumn = secondGroupedColumn;
			ThirdGroupedColumn = thirdGroupedColumn;

			Groups = null!;
			Groups2 = null!;
			Groups3 = groups;
		}

		/// <summary>
		/// Gets the DataFrame associated with the specified single key
		/// </summary>
		/// <param name="key">The key of the DataFrame to get</param>
		/// <returns>The DataFrame associated with the specified key</returns>
		public DataFrame this[object key]
		{
			get => Groups[key];
		}

		/// <summary>
		/// Gets the DataFrame associated with the specified pair of keys
		/// </summary>
		/// <param name="key1">The first key of the DataFrame to get</param>
		/// <param name="key2">The second key of the DataFrame to get</param>
		/// <returns>The DataFrame associated with the specified keys</returns>
		public DataFrame this[object key1, object key2]
		{
			get => Groups2[key1, key2];
		}

		/// <summary>
		/// Gets the DataFrame associated with the specified triplet of keys
		/// </summary>
		/// <param name="key1">The first key of the DataFrame to get</param>
		/// <param name="key2">The second key of the DataFrame to get</param>
		/// <param name="key3">The third key of the DataFrame to get</param>
		/// <returns>The DataFrame associated with the specified keys</returns>
		public DataFrame this[object key1, object key2, object key3]
		{
			get => Groups3[key1, key2, key3];
		}

		/// <summary>
		/// Gets a list of all keys in a single-column grouping
		/// </summary>
		public List<object> Keys
		{
			get => new List<object>(Groups.Keys);
		}

		/// <summary>
		/// Gets a list of all key pairs in a two-column grouping
		/// </summary>
		public List<(object key1, object key2)> Keys2
		{
			get
			{
				// Pre-allocate with estimated capacity
				var list = new List<(object, object)>(Groups2.Count * 2); 

				foreach (var outerPair in Groups2)
				{
					foreach (var innerPair in outerPair.Value)
					{
						list.Add((outerPair.Key, innerPair.Key));
					}
				}

				return list;
			}
		}

		/// <summary>
		/// Gets a list of all key triplets in a three-column grouping
		/// </summary>
		public List<(object key1, object key2, object key3)> Keys3
		{
			get
			{
				// Pre-allocate with estimated capacity
				var list = new List<(object, object, object)>(Groups3.Count * 3); 

				foreach (var level1 in Groups3)
				{
					foreach (var level2 in level1.Value)
					{
						foreach (var level3 in level2.Value)
						{
							list.Add((level1.Key, level2.Key, level3.Key));
						}
					}
				}

				return list;
			}
		}

		/// <summary>
		/// Applies a rolling window operation to each grouped DataFrame, with optional downsampling
		/// </summary>
		/// <param name="rollingWindow">Size of the rolling window</param>
		/// <param name="window">Take every Nth row (downsampling). Use 1 for no downsampling.</param>
		/// <param name="aggregations">Dictionary specifying aggregations to apply to each column</param>
		/// <returns>A new DataFrame containing the results of the rolling operation</returns>
		public DataFrame Rolling(int rollingWindow, int window, Dictionary<string, Aggregation> aggregations)
		{

			if (rollingWindow <= 0)
				throw new ArgumentException("Rolling window must be positive", nameof(rollingWindow));
			if (window <= 0)
				throw new ArgumentException("Window must be positive", nameof(window));

			// Initialize aggregations with grouping columns
			var combinedAggregations = new Dictionary<string, Aggregation>(aggregations.Count + 3)
			{
				[GroupedColumn] = Aggregation.Last
			};


			if (Groups2 != null && Groups2.Count > 0)
				combinedAggregations[SecondGroupedColumn] = Aggregation.Last;
			if (Groups3 != null && Groups3.Count > 0)
				combinedAggregations[ThirdGroupedColumn] = Aggregation.Last;

			foreach (var agg in aggregations)
				combinedAggregations[agg.Key] = agg.Value;

			DataFrame result = null!;

			if (Groups != null && Groups.Count > 0)
			{
				foreach (var group in Groups.Values)
				{
					var rolled = group.Rolling(rollingWindow, combinedAggregations).TakeEvery(window);
					if (rolled == null)
						continue;

					if (result == null)
						result = new DataFrame(rolled);
					else
						result.AddRows(rolled);
				}
			}
			else if (Groups2 != null && Groups2.Count > 0)
			{
				foreach (var (key1, key2) in Keys2)
				{
					var rolled = Groups2[key1][key2].Rolling(rollingWindow, combinedAggregations).TakeEvery(window);

					if (result == null)
						result = new DataFrame(rolled);
					else
						result.AddRows(rolled);
				}
			}
			else if (Groups3 != null && Groups3.Count > 0)
			{
				foreach (var (key1, key2, key3) in Keys3)
				{
					var rolled = Groups3[key1][key2][key3].Rolling(rollingWindow, combinedAggregations).TakeEvery(window);

					if (result == null)
						result = new DataFrame(rolled);
					else
						result.AddRows(rolled);
				}
			}

			return result;
		}

		/// <summary>
		/// Applies a rolling window operation to each grouped DataFrame
		/// </summary>
		/// <param name="rollingWindow">Size of the rolling window</param>
		/// <param name="aggregations">Dictionary specifying aggregations to apply to each column</param>
		/// <returns>A new DataFrame containing the results of the rolling operation</returns>
		public DataFrame Rolling(int rollingWindow, Dictionary<string, Aggregation> aggregations)
		{
			return Rolling(rollingWindow, 1, aggregations);
		}

		/// <summary>
		/// Shifts column values in each grouped DataFrame by specified steps
		/// </summary>
		/// <param name="shifts">Array of tuples specifying:
		///   - columnName: Name of column to shift
		///   - newColName: Name for new shifted column
		///   - steps: Number of rows to shift (positive or negative)</param>
		/// <returns>A new DataFrame with shifted columns</returns>
		public DataFrame Shift(params (string columnName, string newColName, int steps)[] arg)
		{
			//
			DataFrame result = null!;
			if (Groups != null && Groups.Count > 0)
			{
				foreach (var gr in Groups)
				{
					var df1 = gr.Value.Shift(arg);
					if (result == null)
						result = new DataFrame(df1);
					else
						result.AddRows(df1);
				}
			}
			else if (Groups2 != null && Groups2.Count > 0)
			{
				foreach (var gr in Keys2)
				{
					var df1 = this.Groups2[gr.key1][gr.key2].Shift(arg);
					if (result == null)
						result = new DataFrame(df1);
					else
						result.AddRows(df1);
				}
			}
			else if (Groups3 != null && Groups3.Count > 0)
			{
				foreach (var gr in Keys3)
				{
					var df1 = this.Groups3[gr.key1][gr.key2][gr.key3].Shift(arg);
					if (result == null)
						result = new DataFrame(df1);
					else
						result.AddRows(df1);
				}
			}

			return result;
		}

		/// <summary>
		/// Aggregates each grouped DataFrame using the specified operations
		/// </summary>
		/// <param name="aggregations">Dictionary mapping column names to aggregation operations</param>
		/// <returns>A new DataFrame with aggregated results</returns>
		public DataFrame Aggregate(IDictionary<string, Aggregation> aggregations)
		{
			if (Groups == null && Groups2 == null && Groups3 == null)
				throw new InvalidOperationException("No groups available for aggregation");

			DataFrame result = null!;
			//grouping with one column
			if (Groups != null && Groups.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Groups.ElementAt(0).Value.Columns);
				foreach (var gr in Groups)
				{
					var row = gr.Value.Aggragate(aggregations, true);
					df1.AddRow(row);
				}

				return df1;
			}
			//grouping with two columns
			else if (Groups2 != null && Groups2.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Groups2.ElementAt(0).Value.ElementAt(0).Value.Columns);
				foreach (var gr in Groups2)
				{
					foreach (var g2 in gr.Value)
					{
						var row = g2.Value.Aggragate(aggregations, true);
						df1.AddRow(row);
					}

				}
				return df1;
			}
			//grouping with three columns
			else if (Groups3 != null && Groups3.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Groups3.ElementAt(0).Value.ElementAt(0).Value.ElementAt(0).Value.Columns);
				foreach (var gr in Groups3)
				{
					foreach (var g2 in gr.Value)
					{
						foreach (var g3 in g2.Value)
						{
							var row = g3.Value.Aggragate(aggregations, true);
							df1.AddRow(row);
						}
					}

				}
				return df1;
			}
			return result;
		}

		/// <summary>
		/// Returns a DataFrame with the count of rows in each group
		/// </summary>
		/// <returns>DataFrame with columns: [grouped column(s), "count"]</returns>
		public DataFrame GCount()
		{
			if (Groups == null)
				throw new InvalidOperationException("Count is only supported for single-column grouping");

			var values = new List<object?>(Groups.Count * 2);
			foreach (var group in Groups.OrderByDescending(x => x.Value.RowCount()))
			{
				values.Add(group.Key);
				values.Add(group.Value.RowCount());
			}

			return new DataFrame(values, new List<string> { GroupedColumn, "count" }, null);
		}

		/// <summary>
		/// Applies a transformation function to each grouped DataFrame
		/// </summary>
		/// <param name="transform">Function that takes a DataFrame and returns a transformed DataFrame</param>
		/// <returns>A new DataFrame containing all transformed results</returns>
		public DataFrame Transform(Func<DataFrame, DataFrame> trasnform)
		{
			DataFrame result = null!;

			if (Groups == null && Groups2 == null && Groups3 == null)
				throw new Exception("Group is  empty.");

			//grouping with one column
			if (Groups != null && Groups.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Groups.ElementAt(0).Value.Columns);
				foreach (var gr in Groups)
				{
					var rows = trasnform(gr.Value);
					if (rows != null && rows.RowCount() > 0)
						df1.AddRows(rows);
				}

				return df1;
			}
			//grouping with two columns
			else if (Groups2 != null && Groups2.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Groups2.ElementAt(0).Value.ElementAt(0).Value.Columns);
				foreach (var gr in Groups2)
				{
					foreach (var g2 in gr.Value)
					{
						var rows = trasnform(g2.Value);
						if (rows != null && rows.RowCount() > 0)
							df1.AddRows(rows);
					}

				}
				return df1;
			}
			//grouping with three columns
			else if (Groups3 != null && Groups3.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Groups3.ElementAt(0).Value.ElementAt(0).Value.ElementAt(0).Value.Columns);
				foreach (var gr in Groups3)
				{
					foreach (var g2 in gr.Value)
					{
						foreach (var g3 in g2.Value)
						{
							var rows = trasnform(g3.Value);
							if (rows != null && rows.RowCount() > 0)
								df1.AddRows(rows);
						}
					}

				}
				return df1;
			}
			return result;
		}

		/// <summary>
		/// Returns a string representation of the grouped DataFrames
		/// </summary>
		/// <param name="maxRows">Maximum number of rows to display per group</param>
		/// <returns>Formatted string representation</returns>
		public string ToStringBuilder(int rowCount = 15)
		{
			StringBuilder sb = new StringBuilder();
			int rows = this.Groups != null ? this.Groups.Count() :
					   this.Groups2 != null ? this.Groups2.Count :
					   this.Groups3.Count;

			int longestColumnName = 20;

			//add space for group
			sb.Append(string.Format($"Group By Column: {this.GroupedColumn}".PadRight(longestColumnName)));

			if (!string.IsNullOrEmpty(this.SecondGroupedColumn))
				sb.Append(string.Format(", " + this.SecondGroupedColumn.PadRight(longestColumnName)));

			if (!string.IsNullOrEmpty(this.ThirdGroupedColumn))
				sb.Append(string.Format(", " + this.ThirdGroupedColumn.PadRight(longestColumnName)));

			sb.AppendLine();
			//
			var rr = Math.Min(rowCount, rows);
			for (int i = 0; i < rr; i++)
			{
				if (this.Groups != null)
				{
					var grp = this.Groups.ElementAt(i);
					sb.Append((grp.Key).ToString()!.PadRight(longestColumnName));
					sb.AppendLine();
					sb.Append(grp.Value.ToStringBuilder());
					sb.AppendLine();
				}
				else if (this.Groups2 != null)
				{
					var grp = this.Groups2.ElementAt(i);
					foreach (var k2 in grp.Value)
					{
						sb.Append((grp.Key).ToString()!.PadRight(longestColumnName));
						sb.Append((k2.Key).ToString()!.PadRight(longestColumnName));
						sb.AppendLine();
						sb.Append(k2.Value.ToStringBuilder());
						sb.AppendLine();
					}

				}
				else if (this.Groups3 != null)
				{
					var grp = this.Groups3.ElementAt(i);
					foreach (var k2 in grp.Value)
					{
						foreach (var k3 in k2.Value)
						{
							sb.Append((grp.Key).ToString()!.PadRight(longestColumnName));
							sb.Append((k2.Key).ToString()!.PadRight(longestColumnName));
							sb.Append((k3.Key).ToString()!.PadRight(longestColumnName));
							sb.AppendLine();
							sb.Append(k3.Value.ToStringBuilder());
							sb.AppendLine();
						}
					}

				}

			}
			return sb.ToString();
		}
	}
}
