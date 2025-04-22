//////////////////////////////////////////////////////////////////////////////////////////
// Daany - DAta ANalYtics Library                                                        //
// https://github.com/bhrnjica/daany                                                    //
//                                                                                      //
// Copyright 2006-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/daany/blob/master/LICENSE        //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica at hotmail.com                                                              //
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;
using Daany.Multikey;


namespace Daany.Grouping
{
	public class GroupDataFrame
	{
		internal GroupDataFrame(string colName, Dictionary<object, DataFrame> grp)
		{
			GroupedColumn = colName;
			Group = grp;
		}

		internal GroupDataFrame(string firstGroupedColumn, string secondGroupedColumn, TwoKeysDictionary<object, object, DataFrame> grp)
		{
			GroupedColumn = firstGroupedColumn;
			SecondGroupedColumn = secondGroupedColumn;
			Group2 = grp;
		}

		internal GroupDataFrame(string firstGroupedColumn, string secondGroupedColumn, string thirdGroupedColumn, ThreeKeysDictionary<object, object, object, DataFrame> grp)
		{
			GroupedColumn = firstGroupedColumn;
			SecondGroupedColumn = secondGroupedColumn;
			ThirdGroupedColumn = thirdGroupedColumn;
			Group3 = grp;
		}


		public string GroupedColumn { get; set; }
		public string SecondGroupedColumn { get; set; }
		public string ThirdGroupedColumn { get; set; }

		internal Dictionary<object, DataFrame> Group { get; }
		internal TwoKeysDictionary<object, object, DataFrame> Group2 { get; }
		internal ThreeKeysDictionary<object, object, object, DataFrame> Group3 { get; }

		public DataFrame this[object Key]
		{
			get
			{
				return Group[Key];
			}
		}

		public DataFrame this[object Key1, object Key2]
		{
			get
			{
				return Group2[Key1, Key2];
			}
		}

		public DataFrame this[object Key1, object Key2, object Key3]
		{
			get
			{
				return Group3[Key1, Key2, Key3];
			}
		}

		public List<object> Keys
		{
			get
			{
				return Group.Keys.ToList();
			}
		}

		public List<(object key1, object key2)> Keys2
		{
			get
			{
				var lst = new List<(object key1, object key2)>();
				foreach (var kk in Group2)
				{
					var k1 = kk.Key;
					foreach (var kkk in kk.Value)
					{
						var k2 = kkk.Key;
						lst.Add((k1, k2));
					}
				}

				return lst;
			}
		}

		public List<(object key1, object key2, object key3)> Keys3
		{
			get
			{
				var lst = new List<(object key1, object key2, object key3)>();
				foreach (var k in Group3)
				{
					var k1 = k.Key;
					foreach (var kk in k.Value)
					{
						var k2 = kk.Key;
						foreach (var kkk in kk.Value)
						{
							var k3 = kkk.Key;
							lst.Add((k1, k2, k3));
						}
					}
				}

				return lst;
			}
		}
		public DataFrame Rolling(int rollingWindow, int window, Dictionary<string, Aggregation> agg)
		{
			//create columns and aggregation
			var ag = new Dictionary<string, Aggregation>();
			ag.Add(this.GroupedColumn, Daany.Aggregation.Last);
			if (Group2 != null && Group2.Count > 0)
				ag.Add(this.SecondGroupedColumn, Daany.Aggregation.Last);
			if (Group3 != null && Group3.Count > 0)
				ag.Add(this.ThirdGroupedColumn, Daany.Aggregation.Last);
			foreach (var d in agg)
				ag.Add(d.Key, d.Value);
			//
			DataFrame df = null;
			if (Group != null && Group.Count > 0)
			{
				foreach (var gr in Group)
				{
					var df1 = gr.Value.Rolling(rollingWindow, ag).TakeEvery(window);
					if (df == null)
						df = new DataFrame(df1);
					else
						df.addRows(df1);
				}
			}
			else if (Group2 != null && Group2.Count > 0)
			{
				foreach (var gr in Keys2)
				{
					var df1 = this.Group2[gr.key1][gr.key2].Rolling(rollingWindow, ag).TakeEvery(window);
					if (df == null)
						df = new DataFrame(df1);
					else
						df.addRows(df1);
				}
			}
			else if (Group3 != null && Group3.Count > 0)
			{
				foreach (var gr in Keys3)
				{
					var df1 = this.Group3[gr.key1][gr.key2][gr.key3].Rolling(rollingWindow, ag).TakeEvery(window);
					if (df == null)
						df = new DataFrame(df1);
					else
						df.addRows(df1);
				}
			}

			return df;
		}

		/// <summary>
		/// Each group data frame performed rolling operation, with aggregate operation on each column
		/// </summary>
		/// <param name="rollingWindow">size of rolling</param>
		/// <param name="agg"></param>
		/// <returns></returns>
		public DataFrame Rolling(int rollingWindow, Dictionary<string, Aggregation> agg)
		{
			//create columns and aggregation
			var ag = new Dictionary<string, Aggregation>();
			ag.Add(this.GroupedColumn, Daany.Aggregation.Last);

			if (Group2 != null && Group2.Count > 0)
				ag.Add(this.SecondGroupedColumn, Daany.Aggregation.Last);

			if (Group3 != null && Group3.Count > 0)
				ag.Add(this.ThirdGroupedColumn, Daany.Aggregation.Last);

			foreach (var d in agg)
				ag.Add(d.Key, d.Value);

			//
			DataFrame df = null;
			if (Group != null && Group.Count > 0)
			{
				foreach (var gr in Group)
				{
					var df1 = gr.Value.Rolling(rollingWindow, ag);
					if (df == null)
						df = new DataFrame(df1);
					else
						df.addRows(df1);
				}
			}
			else if (Group2 != null && Group2.Count > 0)
			{
				foreach (var gr in Keys2)
				{
					var df1 = this.Group2[gr.key1][gr.key2].Rolling(rollingWindow, ag);
					if (df == null)
						df = new DataFrame(df1);
					else
						df.addRows(df1);
				}
			}
			else if (Group3 != null && Group3.Count > 0)
			{
				foreach (var gr in Keys3)
				{
					var df1 = this.Group3[gr.key1][gr.key2][gr.key3].Rolling(rollingWindow, ag);
					if (df == null)
						df = new DataFrame(df1);
					else
						df.addRows(df1);
				}
			}

			return df;
		}
		/// <summary>
		/// Shifts the values of the columns by the number of 'steps' rows in every grouped data frame. 
		/// </summary>
		/// <param name="columnName">existing column to be shifted</param>
		/// <param name="newColName">new shifted column</param>
		/// <param name="step"></param>
		/// <returns></returns>
		public DataFrame Shift(params (string columnName, string newColName, int steps)[] arg)
		{
			//
			DataFrame df = null;
			if (Group != null && Group.Count > 0)
			{
				foreach (var gr in Group)
				{
					var df1 = gr.Value.Shift(arg);
					if (df == null)
						df = new DataFrame(df1);
					else
						df.addRows(df1);
				}
			}
			else if (Group2 != null && Group2.Count > 0)
			{
				foreach (var gr in Keys2)
				{
					var df1 = this.Group2[gr.key1][gr.key2].Shift(arg);
					if (df == null)
						df = new DataFrame(df1);
					else
						df.addRows(df1);
				}
			}
			else if (Group3 != null && Group3.Count > 0)
			{
				foreach (var gr in Keys3)
				{
					var df1 = this.Group3[gr.key1][gr.key2][gr.key3].Shift(arg);
					if (df == null)
						df = new DataFrame(df1);
					else
						df.addRows(df1);
				}
			}

			return df;
		}

		/// <summary>
		/// Aggregate columns of each data fame in grups
		/// </summary>
		/// <param name="agg">List of columns to aggregate. Grouped columns are excluded from aggregation.</param>
		/// <returns></returns>
		public DataFrame Aggregate(IDictionary<string, Aggregation> agg)
		{
			DataFrame df = null;
			if (Group == null && Group2 == null && Group3 == null)
				throw new Exception("Group is  empty.");

			//grouping with one column
			if (Group != null && Group.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Group.ElementAt(0).Value.Columns);
				foreach (var gr in Group)
				{
					var row = gr.Value.Aggragate(agg, true);
					df1.AddRow(row);
				}

				return df1;
			}
			//grouping with two columns
			else if (Group2 != null && Group2.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Group2.ElementAt(0).Value.ElementAt(0).Value.Columns);
				foreach (var gr in Group2)
				{
					foreach (var g2 in gr.Value)
					{
						var row = g2.Value.Aggragate(agg, true);
						df1.AddRow(row);
					}

				}
				return df1;
			}
			//grouping with three columns
			else if (Group3 != null && Group3.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Group3.ElementAt(0).Value.ElementAt(0).Value.ElementAt(0).Value.Columns);
				foreach (var gr in Group3)
				{
					foreach (var g2 in gr.Value)
					{
						foreach (var g3 in g2.Value)
						{
							var row = g3.Value.Aggragate(agg, true);
							df1.AddRow(row);
						}
					}

				}
				return df1;
			}
			return df;
		}

		/// <summary>
		/// Returns data frame of number of rows for each group
		/// </summary>
		/// <returns></returns>
		public DataFrame GCount()
		{
			var cols = new string[] { GroupedColumn, "count" };
			var lst = new List<object>();
			foreach (var gr in Group.OrderByDescending(x => x.Value.RowCount()))
			{
				var cnt = gr.Value.RowCount();
				lst.Add(gr.Key);
				lst.Add(cnt);
			}

			return new DataFrame(lst, cols.ToList(), null);
		}

		/// <summary>
		/// Perform transformation on each grouped data frame.
		/// </summary>
		/// <param name="callBack"></param>
		/// <returns></returns>
		public DataFrame Transform(Func<DataFrame, DataFrame> callBack)
		{
			DataFrame df = null;
			if (Group == null && Group2 == null && Group3 == null)
				throw new Exception("Group is  empty.");

			//grouping with one column
			if (Group != null && Group.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Group.ElementAt(0).Value.Columns);
				foreach (var gr in Group)
				{
					var rows = callBack(gr.Value);
					if (rows != null && rows.RowCount() > 0)
						df1.addRows(rows);
				}

				return df1;
			}
			//grouping with two columns
			else if (Group2 != null && Group2.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Group2.ElementAt(0).Value.ElementAt(0).Value.Columns);
				foreach (var gr in Group2)
				{
					foreach (var g2 in gr.Value)
					{
						var rows = callBack(g2.Value);
						if (rows != null && rows.RowCount() > 0)
							df1.addRows(rows);
					}

				}
				return df1;
			}
			//grouping with three columns
			else if (Group3 != null && Group3.Count > 0)
			{
				var df1 = DataFrame.CreateEmpty(Group3.ElementAt(0).Value.ElementAt(0).Value.ElementAt(0).Value.Columns);
				foreach (var gr in Group3)
				{
					foreach (var g2 in gr.Value)
					{
						foreach (var g3 in g2.Value)
						{
							var rows = callBack(g3.Value);
							if (rows != null && rows.RowCount() > 0)
								df1.addRows(rows);
						}
					}

				}
				return df1;
			}
			return df;
		}
		public string ToStringBuilder(int rowCount = 15)
		{
			StringBuilder sb = new StringBuilder();
			int rows = this.Group != null ? this.Group.Count() :
					   this.Group2 != null ? this.Group2.Count :
					   this.Group3.Count;

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
				if (this.Group != null)
				{
					var grp = this.Group.ElementAt(i);
					sb.Append((grp.Key).ToString().PadRight(longestColumnName));
					sb.AppendLine();
					sb.Append(grp.Value.ToStringBuilder());
					sb.AppendLine();
				}
				else if (this.Group2 != null)
				{
					var grp = this.Group2.ElementAt(i);
					foreach (var k2 in grp.Value)
					{
						sb.Append((grp.Key).ToString().PadRight(longestColumnName));
						sb.Append((k2.Key).ToString().PadRight(longestColumnName));
						sb.AppendLine();
						sb.Append(k2.Value.ToStringBuilder());
						sb.AppendLine();
					}

				}
				else if (this.Group3 != null)
				{
					var grp = this.Group3.ElementAt(i);
					foreach (var k2 in grp.Value)
					{
						foreach (var k3 in k2.Value)
						{
							sb.Append((grp.Key).ToString().PadRight(longestColumnName));
							sb.Append((k2.Key).ToString().PadRight(longestColumnName));
							sb.Append((k3.Key).ToString().PadRight(longestColumnName));
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
