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
// bhrnjica at hotmail.com                                                              //random
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Daany.MathStuff.Random;

namespace Daany
{
    /// <summary>
    /// Class implementation for DataFrame. The DataFrame is going to be C# specific implementation
    /// to handle data loading from files, grouping, sorting, filtering, handling with columns and rows
    /// accessing data frame (df) elements etc.
    /// </summary>

    public partial class DataFrame : IDataFrame
    {
        #region Properties

        /// <summary>
        /// List of columns (names) in the data frame.
        /// </summary>
        /// 
        public List<string> Columns => _columns;


        /// <summary>
        /// Types of columns (names) in the data frame.
        /// </summary>
        /// 
        public IList<ColType> ColTypes => this._colsType == null || _colsType == Array.Empty<ColType>() ? columnsTypes() : this._colsType;
        

        /// <summary>
        /// Index for rows in the data frame.
        /// </summary>
        /// 
        public Daany.Index Index => _index;


        public (int rows, int cols) Shape => (RowCount(), ColCount());

        public IList<object> Values => _values;



        /// <summary>
        /// Representation of missing value.
        /// </summary>
        /// 
        public static object? NAN => null;

        #endregion

        #region Private fields

        /// <summary>
        /// Data type for each data frame column.
        /// </summary>
        /// 
        private ColType[]? _colsType;
        /// <summary>
        /// 1D element contains data frame values
        /// </summary>
        /// 
        private List<object> _values;
        private Daany.Index _index;
        private List<string> _columns;

        //Quick Sort algorithm. In case of false, the Merge Sort will be used.
        internal static bool qsAlgo = true;
		#endregion

		#region Enumerators

		/// <summary>
		/// Returns strongly typed row enumerator.
		/// </summary>
		/// <typeparam name="TRow">Result type for each row</typeparam>
		/// <param name="callBack">Function to transform row data</param>
		/// <returns>Enumerable sequence of processed rows</returns>
		public IEnumerable<TRow> GetEnumerator<TRow>(Func<IDictionary<string, object>, TRow> callBack)
		{
			// Reuse the same dictionary to avoid allocations
			var rowDictionary = new Dictionary<string, object>(Columns.Count);

			// Pre-calculate counts
			int columnCount = Columns.Count;
			int rowCount = RowCount();

			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
			{
				// Clear and reuse the dictionary
				rowDictionary.Clear();

				// Get values for current row
				for (int colIndex = 0; colIndex < columnCount; colIndex++)
				{
					// Calculate position in values array
					int valueIndex = rowIndex * columnCount + colIndex;
					rowDictionary[Columns[colIndex]] = _values[valueIndex];
				}

				yield return callBack(rowDictionary);
			}
		}

		/// <summary>
		/// Return row enumerators by returning row as dictionary 
		/// </summary>
		/// <returns>dictionary</returns>
		public IEnumerable<IDictionary<string, object>> GetEnumerator()
		{
			var rowDictionary = new Dictionary<string, object>(Columns.Count);

			// Pre-calculate counts
			int columnCount = Columns.Count;
			int valueCount = _values.Count; 

			// Calculate total rows based on values count
			int rowCount = valueCount / columnCount;

			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
			{
				rowDictionary.Clear();

				// Calculate starting index for this row
				int rowStart = rowIndex * columnCount;

				for (int colIndex = 0; colIndex < columnCount; colIndex++)
				{
					// Get value from internal storage
					object value = _values[rowStart + colIndex];
					rowDictionary[Columns[colIndex]] = value;
				}

				yield return rowDictionary;
			}
		}

		/// <summary>
		/// Return row enumerators by returning object array
		/// </summary>
		/// <returns>object array</returns>
		public IEnumerable<object[]> GetRowEnumerator()
		{
			int columnCount = Columns.Count;
			int rowCount = RowCount();

			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
			{
				var rowArray = new object[columnCount];
				int rowStart = rowIndex * columnCount;

				for (int colIndex = 0; colIndex < columnCount; colIndex++)
				{
					rowArray[colIndex] = _values[rowStart + colIndex];
				}

				yield return rowArray;
			}
		}
		#endregion

		#region Index Related Members
		private List<object> GenerateDefaultIndex(int rowCount)
		{
			// Generate a default index (0, 1, 2, ..., rowCount - 1)
			return Enumerable.Range(0, rowCount).Cast<object>().ToList();
		}

		public void SetIndex(List<object> ind, string name)
        {
            if (ind == null)
                throw new Exception("Index cannot be null.");

            if (ind.Count != _index.Count)
                throw new Exception("Wrong count of index list.");

            this._index = new Index(ind, name);
        }

        public DataFrame SetIndex(string colName)
        {
            if (!this._columns.Contains(colName))
                throw new Exception($"{colName} does not exist.");

            //all cols except colName
            var cols = this._columns.Where(x=>x!= colName).ToArray();
           
            //create new data frame
            var ind= this[colName].ToList();
            var df = this[cols];
            df._index = new Index(ind, colName);
            //
            return df;
            
        }

        public DataFrame ResetIndex(bool drop = false)
        {
            var colName = this.Index.Name;
            var colVal = this.Index.ToList();
            var newDf = this[this._columns.ToArray()];
            newDf.Index.Reset();
            //drop index if required
            if(!drop)
                newDf = newDf.InsertColumn(colName, colVal, 0);
            return newDf;
        }
        #endregion

        #region Constructors
        private DataFrame()
        {
            _values = new List<object>();
            _index = new Index(new List<object>());
            _columns = new List<string>();
			_colsType = Array.Empty<ColType>();
		}

		/// <summary>
		/// Initializes a new instance of the DataFrame class using a dictionary with 
		/// two keys to define columns and indexed rows.
		/// </summary>
		/// <param name="aggValues">
		/// A TwoKeysDictionary where:
		/// - The first key represents column names.
		/// - The second key represents row indices.
		/// - The value is the corresponding cell data.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when the dictionary 'aggValues' is null or empty.
		/// </exception>
		/// <remarks>
		/// - Extracts column names and unique row indices from the dictionary.
		/// - Populates internal data representation (_values) using the provided dictionary.
		/// - Missing data is filled with DataFrame.NAN.
		/// </remarks>
		/// <example>
		/// var aggValues = new TwoKeysDictionary<string, object, object>();
		/// aggValues.Add("Column1", "Row1", 1);
		/// aggValues.Add("Column1", "Row2", 2);
		/// aggValues.Add("Column2", "Row1", "A");
		/// aggValues.Add("Column2", "Row2", "B");
		/// var df = new DataFrame(aggValues);
		/// </example>

		internal DataFrame(TwoKeysDictionary<string, object, object> aggValues)
		{
			if (aggValues == null || aggValues.Count == 0)
				throw new ArgumentException("The dictionary 'aggValues' cannot be null or empty.", nameof(aggValues));

			// Initialize columns and index lists
			var indexSet = new HashSet<object>(); // Using HashSet for better performance
			this._columns = new List<string>(aggValues.Select(c => c.Key));

			// Extract unique index values
			foreach (var c in aggValues)
			{
				// Ensure 'c.Value' is enumerable
				if (c.Value is IEnumerable<KeyValuePair<object, object>> keyValuePairs)
				{
					foreach (var kvp in keyValuePairs)
					{
						indexSet.Add(kvp.Key); // Add unique keys to index set
					}
				}
				else
				{
					throw new InvalidOperationException($"The value associated with column '{c.Key}' is not enumerable.");
				}
			}

			// Convert HashSet index to list for ordering
			var orderedIndex = indexSet.ToList();
			this._index = new Index(orderedIndex);

			// Fill _values list
			this._values = orderedIndex
				.SelectMany(rowIndex => this._columns
					.Select(column => aggValues.ContainsKey(column, rowIndex)
						? aggValues[column, rowIndex]
						: DataFrame.NAN!))
				.ToList();
		}


		/// <summary>
		/// Initializes a new instance of the DataFrame class with specified data, 
		/// index, column names, and column types.
		/// </summary>
		/// <param name="data">
		/// A list of objects representing the flattened 2D data stored in row-major order.
		/// </param>
		/// <param name="index">
		/// An Index instance representing the row indices of the data.
		/// </param>
		/// <param name="cols">
		/// A list of strings representing the column names of the data.
		/// </param>
		/// <param name="colsType">
		/// An array of ColType enumerations representing the data types of the columns. 
		/// The length of this array must match the number of columns.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when:
		/// - The data list is null or empty.
		/// - The index is null or empty.
		/// - The columns list is null or empty.
		/// - The column types array is null or its length does not match the number of columns.
		/// </exception>
		/// <remarks>
		/// - Validates inputs to ensure consistency and correctness.
		/// - Clones all inputs to create immutable internal representations.
		/// - Protects the internal state of the DataFrame object from external modifications.
		/// </remarks>
		/// <example>
		/// var data = new List<object> { 1, "A", 2, "B", 3, "C" };
		/// var index = new Index(new List<object> { "Row1", "Row2", "Row3" });
		/// var cols = new List<string> { "Column1", "Column2" };
		/// var colsType = new ColType[] { ColType.Int, ColType.String };
		/// var df = new DataFrame(data, index, cols, colsType);
		/// </example>

		internal DataFrame(List<object> data, Index index, List<string> cols, ColType[] colsType)
		{
			// Validate inputs
            ValidateData(data, cols);

			if (index == null || index.Count == 0)
				throw new ArgumentException("Index cannot be null or empty.", nameof(index));

			// Clone columns to ensure immutability
			this._columns = new List<string>(cols);

			// Clone index to ensure immutability
			this._index = new Index(index.ToList());

			// Clone and assign data
			this._values = new List<object>(data);

            // Clone column types to ensure immutability
            if (colsType == null || colsType == Array.Empty<ColType>())
                this._colsType = columnsTypes(); 
            else
            {
                if (this._columns.Count != colsType.Length)
                    throw new ArgumentException("Inconsistant number of col tye with the number of columns.");
                else
                    this._colsType = colsType;
            }
		}



		/// <summary>
		/// Initializes a new instance of the DataFrame class by copying the internal state 
		/// of an existing DataFrame instance.
		/// </summary>
		/// <param name="dataFrame">
		/// The DataFrame instance to copy. Must not be null.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the provided DataFrame instance is null.
		/// </exception>
		/// <remarks>
		/// - Creates a deep copy of the provided DataFrame instance.
		/// - Ensures that the new DataFrame instance is independent of the original, 
		/// preserving immutability of both objects.
		/// </remarks>
		/// <example>
		/// var originalDataFrame = new DataFrame(existingDataFrame);
		/// var clonedDataFrame = new DataFrame(originalDataFrame);
		/// </example>
		public DataFrame(DataFrame dataFrame)
		{
			if (dataFrame == null)
				throw new ArgumentNullException(nameof(dataFrame), "Argument 'dataFrame' cannot be null.");

			// Clone values to protect internal state
			this._values = new List<object>(dataFrame._values);

			// Clone index to ensure immutability
			this._index = new Index(dataFrame.Index.ToList());

			// Clone columns to ensure immutability
			this._columns = new List<string>(dataFrame.Columns);

			// Clone column types (if applicable)
			this._colsType = dataFrame._colsType != null
				? (ColType[])dataFrame._colsType.Clone()
				: null;
		}

		/// <summary>
		/// Initializes a new instance of the DataFrame class, organizing tabular data 
		/// with named columns and automatically generated indexed rows.
		/// </summary>
		/// <param name="data">
		/// An array of objects representing the flattened 2D data stored in row-major order.
		/// </param>
		/// <param name="columns">
		/// A list of strings representing the column names of the data.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when:
		/// - The data array is null or empty.
		/// - The columns list is null or empty.
		/// - The number of columns does not evenly divide the length of the data array.
		/// </exception>
		/// <remarks>
		/// - Validates input data and columns to ensure consistency in dimensions.
		/// - Automatically generates a default index for rows.
		/// - Protects internal state by encapsulating data and columns in new List<T> objects.
		/// </remarks>
		/// <example>
		/// // Example usage of the constructor:
		/// var data = new object[] { 1, "A", 2, "B", 3, "C" };
		/// var columns = new List<string> { "Column1", "Column2" };
		/// var df = new DataFrame(data, columns);
		/// </example>


		public DataFrame(object[] data, IList<string> columns)
		{
			// Validate inputs using a dedicated validation method
			ValidateData(data, columns);

			// Calculate row count
			int rows = data.Length / columns.Count;

			// Initialize index
			this._index = new Index(GenerateDefaultIndex(rows));

			// Assign columns
			this._columns = new List<string>(columns);

			// Assign data
			this._values = new List<object>(data);
		}

		/// <summary>
		/// Initializes a new instance of the DataFrame class, organizing tabular data 
		/// with indexed rows, named columns, and corresponding data values.
		/// </summary>
		/// <param name="data">
		/// A list of objects representing the flattened 2D data stored in row-major order.
		/// </param>
		/// <param name="index">
		/// A list of objects representing row indices. These indices are used to uniquely 
		/// identify each row in the data.
		/// </param>
		/// <param name="columns">
		/// A list of strings representing the column names of the data.
		/// </param>
		/// <param name="colTypes">
		/// An array of ColType enumerations representing the data types of the columns.
		/// </param>
		/// <remarks>
		/// - This constructor assumes that the data, index, columns, and column types 
		/// are provided in full and consistent dimensions.
		/// - Ensures that the internal representation of the DataFrame aligns with the 
		/// provided inputs.
		/// </remarks>
		/// <example>
		/// // Example usage of the constructor:
		/// var data = new List<object> { 1, "A", 2, "B", 3, "C" };
		/// var index = new List<object> { "Row1", "Row2", "Row3" };
		/// var columns = new List<string> { "Column1", "Column2" };
		/// var colTypes = new ColType[] { ColType.Int, ColType.String };
		/// var df = new DataFrame(data, index, columns, colTypes);
		/// </example>
		public DataFrame(List<object> data, List<object> index, List<string> columns, ColType[] colTypes)
        {
            this._index = new Index(index);
            this._columns = columns;
            this._values = data;
            this._colsType = colTypes;
        }



		/// <summary>
		/// Initializes a new instance of the DataFrame class, organizing tabular data 
		/// with named columns and indexed rows.
		/// </summary>
		/// <param name="data">
		/// A list of objects representing the flattened 2D data stored in row-major order.
		/// </param>
		/// <param name="columns">
		/// A list of strings representing the column names of the data.
		/// </param>
		/// <param name="colTypes">
		/// An optional array of ColType enumerations representing the data types of the columns. 
		/// If not provided, the column types will be null.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when:
		/// - The data list is null or empty.
		/// - The columns list is null or empty.
		/// - The number of columns does not evenly divide the length of the data list.
		/// </exception>
		/// <remarks>
		/// - Validates input data and columns to ensure consistency in dimensions and integrity.
		/// - Automatically generates a default index for rows if no custom index is provided.
		/// - Stores column names and data in internal structures to ensure immutability and maintainability.
		/// - Converts data from flattened format into an indexed DataFrame structure.
		/// </remarks>
		/// <example>
		/// // Example usage of the constructor:
		/// var data = new List<object> { 1, "A", 2, "B", 3, "C" };
		/// var columns = new List<string> { "Column1", "Column2" };
		/// var colTypes = new ColType[] { ColType.Int, ColType.String };
		/// var df = new DataFrame(data, columns, colTypes);
		/// </example>
		public DataFrame(List<object> data, List<string> columns, ColType[]? colTypes = null)
		{
			ValidateData(data, columns);

			// Calculate row count
			int rows = data.Count / columns.Count;

			// Initialize index
			this._index = new Index(GenerateDefaultIndex(rows));

			// Assign columns
			this._columns = new List<string>(columns);

			// Assign data
			this._values = new List<object>(data);

			// Assign column types (optional)
			this._colsType = colTypes;
		}


		/// <summary>
		/// Initializes a new instance of the DataFrame class, organizing tabular data 
		/// with named columns and indexed rows.
		/// </summary>
		/// <param name="data">
		/// A dictionary where the key represents a column name, and the value is a 
		/// list of objects corresponding to that column's data.
		/// </param>
		/// <param name="index">
		/// A list of objects representing row indices. If not provided, indices are 
		/// automatically generated as integers starting from 0.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when the provided dictionary is null or empty.
		/// </exception>
		/// <exception cref="Exception">
		/// Thrown when column data lists have differing lengths.
		/// </exception>
		/// <remarks>
		/// - Validates input data to ensure consistency in column dimensions.
		/// - Converts 2D column-major data into a flattened 1D list (_values) in row-major order.
		/// - Parses each data value using the ParseValue method.
		/// </remarks>
		/// <example>
		/// var data = new Dictionary<string, List<object>>
		/// {
		///     { "Column1", new List<object> { 1, 2, 3 } },
		///     { "Column2", new List<object> { "A", "B", "C" } },
		/// };
		/// var index = new List<object> { "Row1", "Row2", "Row3" };
		/// var df = new DataFrame(data, index);
		/// </example>
		public DataFrame(IDictionary<string, List<object>> data, IList<object>? index = null)
		{
			ValidateData(data);

			var firstColumnValues = data.Values.First();
			this._index = index == null
				? new Index(Enumerable.Range(0, firstColumnValues.Count).Cast<object>().ToList())
				: new Index(index.ToList());

			this._columns = data.Keys.ToList();

			this._values = Enumerable.Range(0, data.Values.First().Count) // Iterate through rows
	            .SelectMany(row => data.Values.Select(column => ParseValue(column[row], null)))
	            .ToList();

        }


		private void ValidateData(object[] data, IList<string> columns)
		{
			if (data == null)
				throw new ArgumentException("Data cannot be null.", nameof(data));

			if (columns == null || columns.Count == 0)
				throw new ArgumentException("Columns cannot be null or empty.", nameof(columns));

			if (data.Length % columns.Count != 0)
				throw new ArgumentException("The number of columns must evenly divide the length of the data.");
		}

		private void ValidateData(List<object> data, List<string> columns)
		{
			if (data == null)
				throw new ArgumentNullException("Data cannot be null.", nameof(data));

			if (columns == null || columns.Count == 0)
				throw new ArgumentException("Columns cannot be null or empty.", nameof(columns));

			if (data.Count % columns.Count != 0)
				throw new ArgumentException("The number of columns must evenly divide the data length.");

			// Additional validation for column data types
			if (columns.Distinct().Count() != columns.Count)
				throw new ArgumentException("Column names must be unique.", nameof(columns));
		}
		private void ValidateData(IDictionary<string, List<object>> data)
		{
			if (data == null || data.Count == 0)
				throw new ArgumentException("Dictionary cannot be null or empty.", nameof(data));

			int firstCount = data.Values.First().Count;
			if (!data.Values.All(list => list.Count == firstCount))
				throw new Exception("All lists within dictionary must be of the same length.");

		}

		/// <summary>
		/// Creates a new DataFrame from the specified columns, optionally renaming them.
		/// </summary>
		/// <param name="colNames">
		/// A parameter array of tuples, where each tuple contains:
		/// - <c>oldName</c>: The name of an existing column to include in the new DataFrame.
		/// - <c>newName</c>: The new name for the column (optional). If not provided or null/empty,
		/// the original name will be retained.
		/// </param>
		/// <returns>
		/// A new DataFrame containing the specified columns with updated column names.
		/// </returns>
		/// <remarks>
		/// - If <c>colNames</c> contains an <c>oldName</c> that does not exist in the current DataFrame,
		/// this method will throw an exception.
		/// - Columns in the new DataFrame are arranged in the same order as they appear in <c>colNames</c>.
		/// </remarks>
		/// <example>
		/// // Example usage:
		/// var newDf = originalDf.Create(("Column1", "NewColumn1"), ("Column2", ""));
		/// // Creates a DataFrame with "Column1" renamed to "NewColumn1" and "Column2" unchanged.
		/// </example>

		public DataFrame Create(params (string oldName, string newName)[] colNames)
        {
            var oldCols = colNames.Select(x => x.oldName).ToArray();
            var newDf = this[oldCols];
            for (int i = 0; i < colNames.Length; i++)
            {
                var newName = colNames[i].oldName;
                if (!string.IsNullOrEmpty(colNames[i].newName))
                    newName = colNames[i].newName;
               
                newDf._columns[i] = newName;
            }
           
            return newDf;
        }
		
        /// <summary>
		/// Creates an empty DataFrame with the specified column names.
		/// </summary>
		/// <param name="columns">
		/// A list of strings representing the names of the columns in the DataFrame.
		/// </param>
		/// <returns>
		/// A new empty DataFrame with the specified column names.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown when <c>columns</c> is null or empty.
		/// </exception>
		/// <example>
		/// // Example usage:
		/// var columns = new List<string> { "Column1", "Column2", "Column3" };
		/// var emptyDf = DataFrame.CreateEmpty(columns);
		/// </example>
		public static DataFrame CreateEmpty(List<string> columns)
		{
			// Validate input
			if (columns == null || columns.Count == 0)
				throw new ArgumentException("Columns cannot be null or empty.", nameof(columns));

			// Create and initialize an empty DataFrame
			var df = new DataFrame();
			df._values = new List<object>();
			df._index = new Index(new List<object>());
			df._columns = new List<string>(columns); // Protect internal state

			return df;
		}

		/// <summary>
		/// Sets the data type for a specified column in the DataFrame.
		/// </summary>
		/// <param name="columnName">
		/// The name of the column whose data type is to be set.
		/// </param>
		/// <param name="colType">
		/// The new data type to assign to the specified column.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when:
		/// - <c>columnName</c> is null, empty, or whitespace.
		/// - The specified <c>columnName</c> does not exist in the DataFrame.
		/// </exception>
		/// <remarks>
		/// Initializes the column types array (<c>_colsType</c>) if it is null.
		/// </remarks>
		/// <example>
		/// // Example usage:
		/// dataFrame.SetColumnType("Column1", ColType.Int);
		/// </example>

		public void SetColumnType(string columnName, ColType colType)
		{
			// Validate column existence
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentException("Column name cannot be null or empty.", nameof(columnName));

			if (!this.Columns.Contains(columnName))
				throw new ArgumentException($"The specified column name '{columnName}' does not exist.", nameof(columnName));

			// Get the column index
			int index = getColumnIndex(columnName);

			// Ensure _colsType is initialized
			if (_colsType == null)
				_colsType = columnsTypes();

			// Set the column type
			_colsType[index] = colType;
		}

		#endregion

		#region Data Frame Operations


		/// <summary>
		/// Add one or more column into current data frame and returns new data frame with added columns.
		/// </summary>
		/// <param name="cols">list of columns</param>
		/// <returns>New data frame with added columns.</returns>
		public DataFrame AddColumns(Dictionary<string, List<object>> cols)
        {
            foreach(var c in cols)
            {
                if(RowCount() != c.Value.Count)
                    throw new Exception("Row counts must be equal.");
            }

            //
            checkColumnNames(this._columns, cols.Keys.ToArray());

            //
            var vals = new List<object>();
            for (int i = 0; i < _index.Count; i++)
            {
                vals.AddRange(this[i]);

                foreach(var c in cols)
                    vals.Add(c.Value[i]);
            }
            //
            var newCols = Columns.Union(cols.Keys).ToList();
            var index = this._index.ToList();

            return new DataFrame(vals, index, newCols, null);
        }


        /// <summary>
        /// Append new data frame at the end of the data frame
        /// </summary>
        /// <param name="df">DataFrame to be appended. </param>
        /// <param name="verticaly"> if true DataFrame will be added row by row, otherwise DataFrame will be added column by column </param>
        public DataFrame Append(DataFrame df, bool verticaly = true)
        {
            if(verticaly)//add row by row including index
            {
                //for vertical append column count must be the same
                if (this._columns.Count != df.Columns.Count)
                    throw new Exception("Data frames are not compatible to be appended. Column count are not the same!");

                var lst = new List<object>();
                var ind = new List<object>();
                var cols = this._columns.ToList();
                var types = this._colsType != null && this._colsType != Array.Empty<ColType>() ? this._colsType.ToArray(): this._colsType;

                // add values
                lst.AddRange(this._values);
                lst.AddRange(df._values);
                ind.AddRange(this._index);
                ind.AddRange(df._index);

                //create new df
                var newDf = new DataFrame(lst, ind, cols, types );
                return newDf;
            }
            else //add columns
            {
                if(this._index.Count != df._index.Count)
                    throw new Exception("Data frames are not compatible to be appended. Row counts are not equal!");

                var dic = new Dictionary<string, List<object>>();
                for(int i=0; i< df._columns.Count; i++)
                {
                    dic.Add(df._columns[i], df[df._columns[i]].ToList()) ;
                }
                //create new df
                var newDf = this.AddColumns(dic);
                return newDf;
            }
            
        }

        /// <summary>
        /// Add one row in the data frame
        /// </summary>
        /// <param name="row">List of row values</param>
        public void AddRow(List<object> row, object index=null)
        {
            if (row == null || row.Count != Columns.Count)
                throw new Exception("Inconsistent row, and cannot be inserted in the DataFrame");
            InsertRow(-1, row, index);
        }

        #region Calculated Column
        /// <summary>
        /// Add additional column into DataFrame. The values of the additional column is calculated by calling Func delegate for each row.
        /// </summary>
        /// <param name="colName">New column names.</param>
        /// <param name="callBack">delegate for the calculation</param>
        /// <returns>True if column added successfully </returns>
        public bool AddCalculatedColumn(string colName, Func<IDictionary<string, object>, int, object> callBack)
        {
            var cols = new string[] { colName };
            var retVal = new object[1];
            object[] callBack2(IDictionary<string, object> row, int i)
            {
                var v = callBack(row,i);
                retVal[0] = v;
                return retVal;
            }
            return AddCalculatedColumns(cols,callBack2);
        }

        /// <summary>
        /// Add additional column into DataFrame. The values of the additional column is calculated by calling Func delegate for each row.
        /// </summary>
        /// <param name="colName">New column names.</param>
        /// <param name="callBack">delegate for the calculation</param>
        /// <returns>True if column added successfully </returns>
        public bool AddCalculatedColumn(string colName, Func<object[], int, object> callBack)
        {
            var cols = new string[] { colName };
            var retVal = new object[1];
            object[] callBack2(object[] row, int i)
            {
                var v = callBack(row, i);
                retVal[0] = v;
                return retVal;
            }
            return AddCalculatedColumns(cols, callBack2);
        }

        /// <summary>
        /// Add additional columns into DataFrame. The values of the columns are 
        /// calculated by calling Func delegate for each row.
        /// </summary>
        /// <param name="colNames">New column names</param>
        /// <param name="callBack">Callback for calculation new columns.</param>
        /// <returns></returns>
        public bool AddCalculatedColumns(string[] colNames, Func<IDictionary<string, object>, int, object[]> callBack)
        {
            if (colNames == null || colNames.Length == 0)
                throw new Exception("column names are not defined properly.");

            //chekc for duplicate column names
            checkColumnNames(this._columns, colNames);
            if (_colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = columnsTypes();
            
            //
            var vals = new List<object>();

            //define processing row before adding columns
            var processingRow = new Dictionary<string, object>();
            for (int j = 0; j < this.Columns.Count; j++)
                processingRow.Add(this.Columns[j], null);

            int oldInd = 0;
            //
            for (int i = 0; i < _index.Count; i++)
            {

                for (int j = 0; j <= this.Columns.Count; j++)
                {

                    if (j >= Columns.Count)
                    {
                        var vs = callBack(processingRow, i);

                        if (vs.Length != colNames.Length)
                            throw new Exception("Defined and calculated columns are not consistent.");

                        foreach (var v in vs)
                            vals.Add(v);
                    }
                    else
                    {
                        var value = _values[oldInd++];
                        processingRow[this.Columns[j]] = value;
                        vals.Add(value);
                    }
                }

            }
            //add new columns
            this._colsType = Array.Empty<ColType>();
            this._columns.AddRange(colNames);
           

            //apply new data frame values
            this._values = vals;
            return true;
        }
        

        /// <summary>
        /// Add additional columns into DataFrame. The values of the columns are 
        /// calculated by calling Func delegate for each row.
        /// </summary>
        /// <param name="colNames">New column names</param>
        /// <param name="callBack">Callback for calculation new columns.</param>
        /// <returns></returns>
        public bool AddCalculatedColumns(string[] colNames, Func<object[], int, object[]> callBack)
        {
            if (colNames == null || colNames.Length == 0)
                throw new Exception("column names are not defined properly.");

            //check if column exists
            checkColumnNames(this._columns, colNames);
            //
            var vals = new List<object>();

            //define processing row before adding column
            var processingRow = new object[this.Columns.Count];
            int oldInd = 0;
            //
            for (int i = 0; i < _index.Count; i++)
            {

                for (int j = 0; j <= this.Columns.Count; j++)
                {

                    if (j >= Columns.Count)
                    {
                        var vs = callBack(processingRow, i);
                        foreach (var v in vs)
                            vals.Add(v);
                    }
                    else
                    {
                        var value = _values[oldInd++];
                        processingRow[j] = value;
                        vals.Add(value);
                    }
                }

            }
            //add new column
            this._colsType = Array.Empty<ColType>();
			this._columns.AddRange(colNames);

            this._values = vals;
            return true;
        }
        #endregion

        #region Aggregation
        /// <summary>
        /// Perform aggregate operation on the list of columns. For incomplete list, the rest of the column will be ommited
        /// </summary>
        /// <param name="indCols">indexes of the columns</param>
        /// <param name="agg"></param>
        /// <returns>List of aggregated values</returns>
        public List<object> Aggragate(IDictionary<string, Aggregation> aggs, bool allColumns = false)
        {
            if (aggs == null)
                throw new Exception("List of columns or list of aggregation cannot be null.");

            //initialize column types
            if(this._colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = columnsTypes();
            //
            var aggValues = new List<object>();
            for (int i = 0; i < Columns.Count; i++)
            {
                if (!aggs.ContainsKey(Columns[i]) && allColumns)
                    aggValues.Add(this[Columns[i]].Last());
                else if(aggs.ContainsKey(Columns[i]))
                {
                    var ag = calculateAggregation(this[Columns[i]], aggs[Columns[i]], this._colsType[i]);
                    aggValues.Add(ag);
                }
            }
            return aggValues;
        }

        /// <summary>
        /// Perform aggregate operation on the list of columns. 
        /// For one column it can be setup more than one aggregate operations
        /// </summary>
        /// <param name="indCols">indexes of the columns</param>
        /// <param name="agg"></param>
        /// <returns></returns>
        public DataFrame Aggragate(IDictionary<string, Aggregation[]> aggs)
        {
            if (aggs == null)
                throw new Exception("List of columns or list of aggregation cannot be null.");

            //initialize column types
            if (this._colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = columnsTypes();
            
            //
            var aggValues = new TwoKeysDictionary<string, object, object>();
            for (int i = 0; i < Columns.Count; i++)
            {
                if (aggs.ContainsKey(Columns[i]))
                {
                    for (int j = 0; j < aggs[Columns[i]].Length; j++)
                    {
                        var column = Columns[i];
                        var key = aggs[Columns[i]][j];
                        var value = calculateAggregation(this[Columns[i]], key, this._colsType[i]);
                        aggValues.Add(column, key.GetEnumDescription(), value);
                    }
                }
            }

            //create dataframe
            var df = new DataFrame(aggValues);
            return df;
        }
        #endregion

        #region Clip
        /// <summary>
        /// Clip all data frame values between the bounds
        /// </summary>
        /// <param name="minValue">min value</param>
        /// <param name="maxValue">max value</param>
        /// <returns></returns>
        public DataFrame Clip(float minValue, float maxValue)
        {
            //initialize column types
            if (this._colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = columnsTypes();

            var lst = new List<object>();
            int index = 0;
            for (int j = 0; j < _index.Count; j++)
            {
                for (int i = 0; i < _columns.Count; i++)
                {
                    if (_values[index] == DataFrame.NAN)
                    {
                        lst.Add(_values[index]);
                       
                    }  
                    else if (this._colsType[i] == ColType.STR || this._colsType[i] == ColType.IN || this._colsType[i] == ColType.F32 || this._colsType[i] == ColType.DT)
                    {
                        lst.Add(_values[index]);
                       
                    }

                    else if(this._colsType[i] == ColType.I32)
                    {
                        var v = Convert.ToInt32(_values[index]);

                        if (v < minValue)
                            v = Convert.ToInt32(minValue);
                        else if(v > maxValue)
                            v = Convert.ToInt32(maxValue);

                        lst.Add(v);
                    }
                    else if (this._colsType[i] == ColType.I64)
                    {
                        var v = Convert.ToInt64(_values[index]);

                        if (v < minValue)
                            v = Convert.ToInt64(minValue);
                        else if (v > maxValue)
                            v = Convert.ToInt64(maxValue);

                        lst.Add(v);
                    }
                    else if (this._colsType[i] == ColType.F32)
                    {
                        var v = Convert.ToSingle(_values[index]);

                        if (v < minValue)
                            v = Convert.ToSingle(minValue);
                        else if (v > maxValue)
                            v = Convert.ToSingle(maxValue);

                        lst.Add(v);
                    }
                    else if (this._colsType[i] == ColType.DD)
                    {
                        var v = Convert.ToDouble(_values[index]);

                        if (v < minValue)
                            v = Convert.ToDouble(minValue);
                        else if (v > maxValue)
                            v = Convert.ToDouble(maxValue);

                        lst.Add(v);
                    }
                    index++;
                }
            }
            //return new data frame
            var ind = this._index.ToList();
            var cols = this.Columns.ToList();
            var types = this._colsType;
            var df = new DataFrame(lst, ind, cols, types);
            return df;
        }

        /// <summary>
        /// Clip all values for specified columns between the bounds in the DataFrame
        /// </summary>
        /// <param name="minValue">min values</param>
        /// <param name="maxValue">max values</param>
        /// <param name="columns">list of columns</param>
        /// <returns></returns>
        public DataFrame Clip(float minValue, float maxValue, params string[] columns)
        {
            //initialize column types
            if (this._colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = columnsTypes();
            var colInd = getColumnIndex(columns);
            var lst = new List<object>();
            int index = 0;
            for (int j = 0; j < _index.Count; j++)
            {
                for (int i = 0; i < _columns.Count; i++)
                {
                    if (!colInd.Contains(i))
                    {
                        lst.Add(_values[index]);
                       
                    }

                    else if (_values[index] == DataFrame.NAN)
                    {
                        lst.Add(_values[index]);
                       
                    }
                    else if (this._colsType[i] == ColType.STR || this._colsType[i] == ColType.IN || this._colsType[i] == ColType.F32 || this._colsType[i] == ColType.DT)
                    {
                        lst.Add(_values[index]);
                        
                    }

                    else if (this._colsType[i] == ColType.I32)
                    {
                        var v = Convert.ToInt32(_values[index]);

                        if (v < minValue)
                            v = Convert.ToInt32(minValue);
                        else if (v > maxValue)
                            v = Convert.ToInt32(maxValue);

                        lst.Add(v);
                    }
                    else if (this._colsType[i] == ColType.I64)
                    {
                        var v = Convert.ToInt64(_values[index]);

                        if (v < minValue)
                            v = Convert.ToInt64(minValue);
                        else if (v > maxValue)
                            v = Convert.ToInt64(maxValue);

                        lst.Add(v);
                    }
                    else if (this._colsType[i] == ColType.F32)
                    {
                        var v = Convert.ToSingle(_values[index]);

                        if (v < minValue)
                            v = Convert.ToSingle(minValue);
                        else if (v > maxValue)
                            v = Convert.ToSingle(maxValue);

                        lst.Add(v);
                    }
                    else if (this._colsType[i] == ColType.DD)
                    {
                        var v = Convert.ToDouble(_values[index]);

                        if (v < minValue)
                            v = Convert.ToDouble(minValue);
                        else if (v > maxValue)
                            v = Convert.ToDouble(maxValue);

                        lst.Add(v);
                    }
                    index++;
                }
            }
            //return new data frame
            var ind = this._index.ToList();
            var cols = this.Columns.ToList();
            var types = this._colsType;
            var df = new DataFrame(lst, ind, cols, types);
            return df;
        }
        #endregion

        /// <summary>
        /// Creates new data frame of basic descriptive statistics values of the data frame
        /// </summary>
        /// <param name="numericOnly"></param>
        /// <param name="inclColumns"></param>
        /// <returns></returns>
        public DataFrame Describe(bool numericOnly = true, params string[] inclColumns)
        {
            var aggOp = new Aggregation[]
            {
                Aggregation.Count,Aggregation.Unique,Aggregation.Top, Aggregation.Frequency, Aggregation.Avg,
                Aggregation.Std,Aggregation.Min, Aggregation.FirstQuartile,Aggregation.Median, Aggregation.ThirdQuartile, Aggregation.Max
            };
            
            //initialize column types
            if (_colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = columnsTypes();

            var lstCols = new List<(string cName, ColType cType)>();
            var idxs = getColumnIndex(inclColumns);

            //include columns
            if (inclColumns.Length == 0)
            {
                for (int i = 0; i < this.Columns.Count(); i++)
                {
                    lstCols.Add((this.Columns[i], this._colsType[i]));
                }
            }
            else
            {
                foreach (var t in idxs)
                {
                    var c = this.Columns[t];
                    lstCols.Add((c, this._colsType[t]));
                }
            }

            //only numeric columns
            var finalCols = new Dictionary<string, Aggregation[]>(); // new List<(string cName, ColType cType)>();
            for (var i = 0; i < lstCols.Count(); i++)
            {
                if (numericOnly)
                {
                    if (isNumeric(lstCols[i].cType))
                        finalCols.Add(lstCols[i].cName, aggOp);
                }
                else //if(!isNumeric(lstCols[i].cType) && !numericOnly)
                    finalCols.Add(lstCols[i].cName, aggOp);
            }

            DataFrame dfDescr = Aggragate(finalCols);
            //
            return dfDescr;
        }


        #region Missing Values 

        /// <summary>
        /// Returns the dictionary containing missing values
        /// </summary>
        /// <returns>Dictionary with specified column and number of missing value in it.</returns>
        public IDictionary<string, int> MissingValues()
        {
            var dc = new Dictionary<string, int>();
            foreach (var col in Columns)
            {
                var mCount = this[col].Where(x => x == NAN).Count();
                dc.Add(col, mCount);
            }

            return dc.Where(x => x.Value > 0).ToDictionary(x => x.Key, y => y.Value);
        }



        /// <summary>
        /// Removes specified columns from the data frame.
        /// </summary>
        /// <param name="colName">List of column names to be removed.</param>
        /// <returns></returns>
        public DataFrame Drop(params string[] colName)
        {
            //
            var cols = new List<(string oldName, string newName)>();
            foreach (var c in this.Columns)
            {
                if (colName.Contains(c))
                    continue;
                cols.Add((c, null));
            }

            return Create(cols.ToArray());
        }

        /// <summary>
        /// Removes rows with missing values for specified set of columns. In case cols is null, removed values 
        /// will be applied to all columns.
        /// </summary>
        /// <param name="cols">List of columns</param>
        /// <returns>New df with fixed NAN</returns>
        public DataFrame DropNA(params string[] cols)
        {
            var colIndex = getColumnIndex(cols);
            return RemoveRows((r, i) =>
            {
                for (int j = 0; j < r.Length; j++)
                {
                    if (colIndex == null && r[j] == NAN)
                        return true;

                    else if(colIndex != null && r[j] == NAN && colIndex.Contains(j))
                        return true;
                }
                return false;
            });

        }

        /// <summary>
        /// Replace NAN values with specified value.
        /// </summary>
        /// <param name="value"></param>
        public void FillNA(object value)
        {
            int index = 0;
            for (int i = 0; i < _index.Count; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    if (_values[index] == DataFrame.NAN)
                        _values[index] = value;
                    index++;
                }
            }
        }

        /// <summary>
        /// Replaces the missing values from specified column with 'replacedValue'. 
        /// </summary>
        /// <param name="col">Column to replace the missing value</param>
        /// <param name="replacedValue">Replaced value</param>
        public void FillNA(string col, object replacedValue)
        {
            var colIndex = getColumnIndex(col);
            int index = 0;
            for (int i = 0; i < _index.Count; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    if (j == colIndex)
                    {
                        if (_values[index] == DataFrame.NAN)
                            _values[index] = replacedValue;

                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// Replaces the missing values from specified column with 'replacedValue'. 
        /// </summary>
        /// <param name="col">Column to replace the missing value</param>
        /// <param name="aggValue">Aggregated Value of the column</param>
        public void FillNA(string col, Aggregation aggValue)
        {
            var colIndex = getColumnIndex(col);
            var vals = this[col].Where(x=>x!=DataFrame.NAN);
            var value = calculateAggregation(vals, aggValue, this.ColTypes[colIndex]);
            FillNA(col, value);
        }

        /// <summary>
        /// Replaces the missing values from specified columns with 'replacedValue'. 
        /// </summary>
        /// <param name="col">Column to replace the missing value</param>
        /// <param name="replacedValue">Replaced value</param>
        public void FillNA(string[] cols, object replacedValue)
        {
            var colIndexes = getColumnIndex(cols);
            int index = 0;
            for (int i = 0; i < _index.Count; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    if (colIndexes.Contains(j))
                    {
                        if (_values[index] == DataFrame.NAN)
                            _values[index] = replacedValue;
                    }

                    index++;
                }
            }
        }

        /// <summary>
        /// Replaces the missing values from specified column with replacedDelegate. 
        /// </summary>
        /// <param name="col">Column to replace the missing value</param>
        /// <param name="replacedValue">Delegate for replaced value</param>
        public void FillNA(string col, Func<int, object> replDelg)
        {
            if (string.IsNullOrEmpty(col))
                throw new ArgumentException(nameof(col));

            var colIndex = getColumnIndex(col);
            int index = 0;
            for (int i = 0; i < _index.Count; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    if (j == colIndex)
                    {
                        if (_values[index] == DataFrame.NAN)
                            _values[index] = replDelg(i);

                    }
                    index++;
                }
            }
        }

        #endregion

        #region Filter
        /// <summary>
        /// Filter data frame based on selected columns and coresponded values and operators.
        /// </summary>
        /// <param name="cols">selected columns</param>
        /// <param name="filteValues">filter values.</param>
        /// <param name="fOpers">filter operators</param>
        /// <returns>returns filtered df</returns>
        public DataFrame Filter(string[] cols, object[] filteValues, FilterOperator[] fOpers)
        {
            if (_index.Count == 0)
                return new DataFrame(Array.Empty<object>(), Columns.ToList());

            //check for non-null arguments
            if (cols == null || cols.Length == 0)
                throw new ArgumentException($"'{nameof(cols)}' cannot be null or empty array.");

            if (filteValues == null || filteValues.Length == 0)
                throw new ArgumentException($"'{nameof(filteValues)}' cannot be null or empty array.");

            if (fOpers == null || fOpers.Length == 0)
                throw new ArgumentException($"'{nameof(fOpers)}' cannot be null or empty array.");

            //check for the same length of the arguments
            if (!(cols.Length == filteValues.Length && cols.Length == fOpers.Length))
                throw new Exception("Inconsistent number of columns, filter values an doperators.");

            //initialize column types
            if (this._colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = columnsTypes();

            int[] indCols = getColumnIndex(cols);
            //
            for (int i = 0; i < cols.Length; i++)
            {
                if (this._colsType[indCols[i]] == ColType.I2 && fOpers[indCols[i]] != FilterOperator.Equal)
                {
                    throw new Exception("Boolean column and filterValue  must be connected with only 'Equal' operator.");
                }
            }

            //temp row values
            int rowIndex = 0;
            object[] rowValues = new object[cols.Length];
            var dfIndex = new List<object>();
            //filtered values
            var lst = new List<object>();
            for (int i = 0; i < _index.Count; i++)
            {
                rowIndex = i * Columns.Count;

                //fill current row
                for (int ix = 0; ix < indCols.Length; ix++)
                    rowValues[ix] = _values[rowIndex + indCols[ix]];

                //perform  filtering
                if (rowValues.Any(x => x == DataFrame.NAN))
                {
                    continue;
                }
                else
                {
                    if (applyOperator(indCols, rowValues, filteValues, fOpers))
                    {
                        //fill index
                        dfIndex.Add(this._index[i]);
                        //fill row
                        for (int j = 0; j < Columns.Count; j++)
                            lst.Add(_values[rowIndex + j]);
                    }

                }
            }
            var df = new DataFrame(lst, dfIndex ,this.Columns, this._colsType);
            return df;
        }

        /// <summary>
        /// Perform data frame filtering 
        /// </summary>
        /// <param name="col"> column name to filter.</param>
        /// <param name="value">filter values</param>
        /// <param name="fOper">filter operator</param>
        /// <returns></returns>
        public DataFrame Filter(string col, object value, FilterOperator fOper)
        {
            return Filter(new string[] { col }, new object[] { value }, new FilterOperator[] { fOper });
        }

        /// <summary>
        /// Return DataFrame where each row satisfied the condition delegate 
        /// </summary>
        /// <param name="condition">The condition delegate</param>
        /// <returns></returns>
        public DataFrame Filter(Func<IDictionary<string, object>, bool> condition)
        {
            bool cnd(IDictionary<string, object> row, int i)
            {
                return !condition(row);
            }

            return RemoveRows(cnd);
        }

        #endregion


        #region Insert
        /// <summary>
        /// Insert new columns at specific position
        /// </summary>
        /// <param name="cName">new Column name.</param>
        /// <param name="nPos">Zero based index position of the new column. -1 insert the column at last position.</param>
        /// <param name="value">column value</param>
        public DataFrame InsertColumn(string cName, List<object> value, int nPos = -1)
        {
            if (value==null)
                throw new Exception("value argument cannot be null");
            if (nPos == -1)
                nPos = this._columns.Count;

            //
            checkColumnName(this._columns, cName);

            if (nPos < -1 && nPos >= ColCount())
                throw new Exception("Index position must be between 0 and ColCount.");

            if (RowCount() != value.Count)
                throw new Exception("Row counts must be equal.");
            //
            int index = 0;
            var vals = new List<object>();
            for (int i = 0; i < _index.Count; i++)
            {
                for (int j = 0; j <= this._columns.Count; j++)
                {
                    if (j == nPos && nPos < this._columns.Count)
                    {
                        vals.Add(value[i]);

                    }
                    else if (j == nPos && (nPos == this._columns.Count))
                    {
                        vals.Add(value[i]);
                    }
                    else
                    {
                        vals.Add(_values[index]);
                        index++;
                    }
                }
            }
            //Create new Data Frame
            var cols = this._columns.ToList();

            ColType[] types = Array.Empty<ColType>(); 
            if(this._colsType != null && _colsType != Array.Empty<ColType>())
            {
                var colType = GetValueType(value.FirstOrDefault()!);
                var t = this._colsType.ToList();

                if (t.Count==0 || nPos == t.Count)
                    t.Add(colType);
                else
                    t.Insert(nPos, colType);
                types = t.ToArray();
            }
           
            if (nPos == this._columns.Count)
                cols.Add(cName);
            else
                cols.Insert(nPos, cName);
            //index
            var ind = this._index.ToList();

            //new data frame
            var newDf = new DataFrame(vals, ind, cols, types);
            return newDf;
        }

        /// <summary>
        /// Inserts the row at specified position in the DataFrame
        /// </summary>
        /// <param name="nPos"></param>
        /// <param name="row"></param>
        public void InsertRow(int nPos, List<object> row, object index=null)
        {
            if(nPos== -1 )
            {
                _values.AddRange(row);
                //add row index
                if (index == null)
                    _index.Add(_index.Count);
                else
                    _index.Add(index);
            }
            else
            {
                int ind = nPos * this.ColCount(); 
                _values.InsertRange(ind, row);

                if (index == null)
                    _index.Insert(nPos, this.RowCount());
                else
                    _index.Insert(nPos, index);
            }            
        }
        #endregion


        #region Join and Merge
        /// <summary>
        /// Join two data frames with Inner or Left join type,based on their index.
        /// </summary>
        /// <param name="df2">Right data frame</param>
        /// <param name="jType">Join types. It can be Inner or Left. In case of right join call join from the second df.</param>
        /// <returns>New joined df.</returns>
        public DataFrame Join(DataFrame df2, JoinType jType)
        {
            if (df2 == null)
                throw new ArgumentException(nameof(df2));

            //initialize column types
            if (this._colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = this.columnsTypes();

            if (df2._colsType == null || df2._colsType == Array.Empty<ColType>())
                df2._colsType = df2.columnsTypes();

            //merge columns
            var tot = Columns.ToList();
            tot.AddRange(df2.Columns);

            //
            (var totalColumns, var totTypes) = mergeColumns(this._columns, this._colsType, df2._columns, df2._colsType,"rightDf");

            var lst = new List<object>();
            var leftRCount = RowCount();
            var leftCCount = ColCount();
            var rightRCount = df2.RowCount();
            var rightCCount = df2.ColCount();

            //create right lookup 
            var ind = Enumerable.Range(0, df2.RowCount()).ToList();
            var rightIndex = df2.Index.Zip(ind, (key, value) => (key, value)).ToLookup(x => x.key, x => x.value);

            //left df enumeration
            var finIndex = new List<object>();
            for (int i = 0; i < leftRCount; i++)
            {
                var leftKey = this._index[i];
                if (rightIndex.Contains(leftKey))
                {
                    var rPos = rightIndex[leftKey].ToList();
                    for (int k = 0; k < rPos.Count; k++)
                    {
                        int j = rPos[k];

                        //fill the index
                        finIndex.Add(leftKey);
                        //fill left table
                        int startL = i * leftCCount;
                        for (int r = startL; r < startL + leftCCount; r++)
                            lst.Add(_values[r]);
                        //fill right table
                        int startR = j * rightCCount;
                        for (int r = startR; r < startR + rightCCount; r++)
                            lst.Add(df2._values[r]);

                    }
                }
                else
                {
                    //in case of Left join and no right data found
                    // fill with NAN numbers
                    if (jType == JoinType.Left)
                    {
                        //fill the index
                        finIndex.Add(leftKey);

                        int startL = i * leftCCount;
                        for (int r = startL; r < startL + leftCCount; r++)
                            lst.Add(_values[r]);

                        for (int r = 0; r < rightCCount; r++)
                            lst.Add(DataFrame.NAN);
                    }
                }
            }
            //Now construct the Data frame with index
            var newDf = new DataFrame(lst, finIndex, totalColumns, totTypes.ToArray());
            return newDf;
        }


        /// <summary>
        /// Join two df with Inner or Left join type.
        /// </summary>
        /// <param name="df2">Right data frame</param>
        /// <param name="leftOn">Join columns from the left df</param>
        /// <param name="rightOn">Join columns from the right df.</param>
        /// <param name="jType">Join types. It can be Inner or Left. In case of right join call join from the second df.</param>
        /// <returns>New joined df.</returns>
        [Obsolete("This method will be deprecated. Please use 'Merge' with the same argument list.")]
        public DataFrame Join(DataFrame df2, string[] leftOn, string[] rightOn, JoinType jType)
        {
            return Merge(df2, leftOn, rightOn, jType, "rightDf");
        }

        /// <summary>
        /// Merge two dfs with Inner or Left join type, by specified leftOn and RightOn columns.
        /// </summary>
        /// <param name="df2">Right data frame</param>
        /// <param name="leftOn">Join columns from the left df</param>
        /// <param name="rightOn">Join columns from the right df.</param>
        /// <param name="jType">Join types. It can be Inner or Left. In case of right join call join from the second df.</param>
        /// <returns>New joined df.</returns>
        internal DataFrame Merge_old(DataFrame df2, string[] leftOn, string[] rightOn, JoinType jType)
        {
            if (df2 == null)
                throw new ArgumentException(nameof(df2));

            if (leftOn == null)
                throw new ArgumentException(nameof(leftOn));


            if (rightOn == null)
                throw new ArgumentException(nameof(rightOn));

            if (leftOn.Length != rightOn.Length)
                throw new Exception("Join column numbers are different!");

            //we allow three column maximum to be criterion for join
            if (leftOn.Length > 3)
                throw new Exception("Three columns for join is exceeded.");

            //initialize column types
            if (this._colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = this.columnsTypes();

            if (df2._colsType == null || df2._colsType == Array.Empty<ColType>())
                df2._colsType = df2.columnsTypes();

            //get column indexes
            var leftInd = getColumnIndex(leftOn);

            //merge columns
            var tot = Columns.ToList();
            tot.AddRange(df2.Columns);

            //
            (var totalColumns, var totalTypes) = mergeColumns(this._columns,this._colsType, df2._columns,df2._colsType, "rightDf");
            
            //create right lookup 
            var right = new List<ILookup<object, int>>();
            var ind = Enumerable.Range(0, df2.RowCount()).ToList();
            foreach (var l in rightOn)
            {
                var lo = df2[l].Zip(ind, (key, value) => (key, value)).ToLookup(x => x.key, x => x.value);
                right.Add(lo);
            }

            var lst = new List<object>();//new values
            var finIndex = new List<object>();//new index
            var leftRCount = RowCount();
            var leftCCount = ColCount();
            var rightRCount = df2.RowCount();
            var rightCCount = df2.ColCount();

            //left df enumeration          
            for (int i = 0; i < leftRCount; i++)
            {
                var leftKey = this._index[i];
                var rPos = containsKey(i, leftInd, right);
                if (rPos != null)
                {
                    for (int k = 0; k < rPos.Length; k++)
                    {
                        int j = rPos[k];

                        //fill the index
                        finIndex.Add(leftKey);

                        //fill left table
                        int startL = i * leftCCount;
                        for (int r = startL; r < startL + leftCCount; r++)
                            lst.Add(_values[r]);
                        //fill right table
                        int startR = j * rightCCount;
                        for (int r = startR; r < startR + rightCCount; r++)
                            lst.Add(df2._values[r]);

                    }

                }
                else
                {
                    //in case of Left join and no right data found
                    // fill with NAN numbers
                    if (jType == JoinType.Left)
                    {
                        //fill the index
                        finIndex.Add(leftKey);

                        int startL = i * leftCCount;
                        for (int r = startL; r < startL + leftCCount; r++)
                            lst.Add(_values[r]);

                        for (int r = 0; r < rightCCount; r++)
                            lst.Add(DataFrame.NAN);
                    }
                }
            }
            //Now construct the Data frame
            var newDf = new DataFrame(lst, finIndex, totalColumns,totalTypes.ToArray() );
            return newDf;

        }

         
        /// <summary>
        /// Merge two (left and right) data frames on specified leftOn and RightOn columns.
        /// </summary>
        /// <param name="df2">Second data frame.</param>
        /// <param name="leftOn">The list of column names for left data frames.</param>
        /// <param name="rightOn">The list of column names for right data frames.</param>
        /// <param name="jType">Join types. It can be Inner or Left. In case of right join call join from the second df.</param>
        /// <param name="suffix">For same column names, use suffix to make different names during merging.</param>
        /// <returns></returns>
        public DataFrame Merge(DataFrame df2, string[] leftOn, string[] rightOn, JoinType jType, string suffix="right")
        {
            if (df2 == null)
                throw new ArgumentException(nameof(df2));

            if (leftOn == null)
                throw new ArgumentException(nameof(leftOn));


            if (rightOn == null)
                throw new ArgumentException(nameof(rightOn));

            if (leftOn.Length != rightOn.Length)
                throw new Exception("Join column numbers are different!");

            //we allow three column maximum to be criterion for join
            if (leftOn.Length > 3)
                throw new Exception("Three columns for merge is exceeded.");

            //check type of columns
            var typ1 = this._colsType == null ? this.columnsTypes():this._colsType;
            var typ2 = df2._colsType == null ? df2.columnsTypes() : df2._colsType;

            //merge column names
            (List<string> totCols, List<ColType> totType) = mergeColumns(this._columns, typ1, df2._columns, typ2, suffix);

            //create lookup table
            (ILookup<object, int> lookup1, 
             TwoKeyLookup<object, object, int> lookup2, 
             ThreeKeyLookup<object, object, object, int> lookup3) = createLookup(df2, rightOn);


            //mrging process
            var leftInd = getColumnIndex(leftOn);
            var lst = new List<object>();//values
            var finIndex = new List<object>();//left df enumeration
            var leftRCount = this.RowCount();
            var leftCCount = this.ColCount();
            var rightRCount = df2.RowCount();
            var rightCCount = df2.ColCount();

            //
            for (int i = 0; i <leftRCount ; i++)
            {
                var leftKey = this._index[i];

                //search for match
                int[] rPos = findIndex(lookup1, lookup2, lookup3, leftInd, i); 

                if (rPos.Length > 0)
                {
                    for (int k = 0; k < rPos.Length; k++)
                    {
                        int j = rPos[k];

                        //fill the index
                        finIndex.Add(leftKey);

                        //fill left table
                        int startL = i * leftCCount;
                        for (int r = startL; r < startL + leftCCount; r++)
                            lst.Add(this._values[r]);

                        //fill right table
                        int startR = j * rightCCount;
                        //
                        for (int r = startR; r < startR + rightCCount; r++)
                            lst.Add(df2._values[r]);
                    }

                }
                else
                {
                    //in case of Left join and no right data found
                    // fill with NAN numbers
                    if (jType == JoinType.Left)
                    {
                        //fill the index
                        finIndex.Add(leftKey);

                        int startL = i * leftCCount;
                        for (int r = startL; r < startL + leftCCount; r++)
                            lst.Add(_values[r]);

                        for (int r = 0; r < rightCCount; r++)
                            lst.Add(DataFrame.NAN);
                    }
                }
            }
            //Now construct the Data frame
            var newDf = new DataFrame(lst, finIndex, totCols, totType.ToArray());
            return newDf;

        }

        #endregion

        /// <summary>
        /// Rename column name within the data frame.
        /// </summary>
        /// <param name="colNames">Tuple of old and new name</param>
        /// <returns></returns>
        public bool Rename(params (string oldName, string newName)[] colNames)
        {
            foreach (var (oldName, newName) in colNames)
            {
                var index = Columns.IndexOf(oldName);
                if (index == -1)
                    throw new Exception($"The column name '{oldName}' does not exist!");
                Columns[index] = newName;
            }
            
            //
            return true;
        }

        #region Sorting

        /// <summary>
        /// Sorts data-frame by specified column in ascending order
        /// </summary>
        /// <param name="cols">Sorting columns</param>
        /// <returns>New ordered df.</returns>
        public DataFrame SortBy(params string[] cols)
        {

            //initialize column types
            if (this._colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = columnsTypes();

            var colInd = getColumnIndex(cols);
            //save
            var sdf = new SortDataFrame(colInd, _colsType);
            List<object> val;
            List<object> ind;
            //
            if (qsAlgo)
              (val, ind)  = sdf.QuickSort(this._values, this._index.ToList(), colInd);
            else
              (val, ind) = sdf.MergeSort(this._values.ToArray(), this._index.ToArray(), colInd);

            //
            var df = new DataFrame(val, ind, Columns.ToList(), this._colsType.ToArray());
            return df;
        }

        /// <summary>
        /// Sorts data-frame by specified column in descending order
        /// </summary>
        /// <param name="cols">Sorting columns.</param>
        /// <returns></returns>
        public DataFrame SortByDescending(params string[] cols)
        {
            var df = SortBy(cols);
            DataFrame newDf = df.reverse();
            return newDf;
        }
        #endregion

        #region RemoveRows      
        /// <summary>
        /// Removes rows satisfying the callback condition.
        /// </summary>
        /// <param name="removeConditions"></param>
        /// <returns></returns>
        public DataFrame RemoveRows(Func<IDictionary<string, object>, int, bool> removeConditions)
        {
            //define processing row before apply condition
            //define processing row before adding column
            var processingRow = new Dictionary<string, object>();
            for (int j = 0; j < this.Columns.Count; j++)
                processingRow.Add(this.Columns[j], null);

            //values in case of new data frame to be generated
            var vals = new List<object>();
            var indValues = new List<object>();
            //
            for (int i = 0; i < _index.Count; i++)
            {              
                rowToDictionary(processingRow, i);
                var isRemoved = removeConditions(processingRow, i);
                if (!isRemoved)
                {
                    int iRow = calculateIndex(i, 0);
                    for (int j = 0; j < this.Columns.Count; j++)
                        vals.Add(_values[iRow + j]);

                    //add index
                    indValues.Add(_index[i]);
                }
            }
            //create new df
            var df = new DataFrame(vals, indValues, this._columns.ToList(), this._colsType);
            return df;
        }

        /// <summary>
        /// Removes rows satisfying the callback condition.
        /// </summary>
        /// <param name="removeConditions"></param>
        /// <returns></returns>
        public DataFrame RemoveRows(Func<object[], int, bool> removeConditions)
        {
            //define processing row before apply condition
            var processingRow = new object[ColCount()];
            
            //values in case of new data frame to be generated
            var vals = new List<object>();
            var indValues = new List<object>();
            var removedRows = new List<int>();
            //
            for (int i = 0; i < _index.Count; i++)
            {
                rowToArray(processingRow, i);
                var isRemoved = removeConditions(processingRow, i);
                if (!isRemoved)
                {
                    int iRow = calculateIndex(i, 0);
                    for (int j = 0; j < this.Columns.Count; j++)
                        vals.Add(_values[iRow + j]);

                    //add index
                    indValues.Add(_index[i]);
                }
            }
            //create new df
            var df = new DataFrame(vals, indValues, this._columns.ToList(), this._colsType);
            return df;
        }

        #endregion 

        #region Rolling

        
        /// <summary>
        /// Grouping with one, two or three columns
        /// </summary>
        /// <param name="groupCols">List of grouped column names. If the list is bigger than three the exception will throw</param>
        /// <returns>GroupedDataFrame</returns>
        public GroupDataFrame GroupBy(params string[] groupCols)
        {
            if (groupCols == null || groupCols.Length == 0)
                throw new Exception("Group columns cannot be null or empty.");
            if (groupCols.Length > 3)
                throw new Exception("Grouping with more than three group columns is not supported.");
            //grouping
            if (groupCols.Length == 1)
            {
                var Group = groupDFBy(groupCols[0]);
                return new GroupDataFrame(groupCols[0], Group);
            }
            else if (groupCols.Length == 2)
            {
                var grp = new TwoKeysDictionary<object, object, DataFrame>();
                //first group
                var group1 = groupDFBy(groupCols[0]);
                foreach (var g in group1)
                {
                    var group2 = group1[g.Key].groupDFBy(groupCols[1]);
                    foreach (var g1 in group2)
                        grp.Add(g.Key, g1.Key, g1.Value);
                }

                return new GroupDataFrame(groupCols[0], groupCols[1], grp);
            }
            else //if (groupCols.Length == 3)
            {
                var grp = new ThreeKeysDictionary<object, object, object, DataFrame>();
                //two columns grouping 
                var gp2 = GroupBy(groupCols[0], groupCols[1]);
                foreach (var g in gp2.Keys2)
                {
                    var df2 = gp2.Group2[g.key1][g.key2];
                    var group3 = df2.groupDFBy(groupCols[2]);
                    foreach (var g1 in group3)
                        grp.Add(g.key1, g.key2, g1.Key, g1.Value);
                }

                return new GroupDataFrame(groupCols[0], groupCols[1], groupCols[2], grp);
            }

        }


        /// <summary>
        /// Create new DataFrame containing rolling values of all column supported the aggregate operation. 
        /// In case the aggregation is not support for certain columns the column is dropped
        /// </summary>
        /// <param name="window">Rolling size.</param>
        /// <param name="agg">Aggregate operation.</param>
        /// <returns></returns>
        public DataFrame Rolling(int window, Aggregation agg)
        {
            //
            var dic = new Dictionary<string, Aggregation>();
            foreach (var c in this._columns)
                dic.Add(c, agg);

            return Rolling(window,dic);
        }

        /// <summary>
        /// Create new dataFrame containing rolling values of specified columns of the data frame
        /// </summary>
        /// <param name="window">rolling width</param>
        /// <param name="agg">key value pair of column and its aggregate operation.</param>
        /// <returns></returns>
        public DataFrame Rolling(int window, Dictionary<string, Aggregation> agg)
        {
            //
            if (agg == null || agg.Count == 0)
                throw new Exception($"Aggregation is empty.");

            int index = 0;
            int rolIndex = 1;
            var rRolls = new Dictionary<string, Queue<object>>();
            var aggrValues = new Dictionary<string, List<object>>();

            //initialize column types
            if (this._colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = columnsTypes();

            //
            for (int i = 0; i < this._index.Count; i++)
            {
                for (int j = 0; j < ColCount(); j++)
                {
                    var colName = this._columns[j];
                    if (agg.ContainsKey(colName) && isOperationSupported(this._colsType[j], agg[colName]))
                    {
                        if (i == 0)
                        {
                            //add coll used for rolling operation
                            rRolls.Add(colName, new Queue<object>(window));
                            //add calculated rolling column
                            aggrValues.Add(colName, new List<object>());
                        }

                        //add value to rolling list
                        rRolls[colName].Enqueue(_values[index]);

                        //rolling aggregation calculation
                        if (i + 1 < window)
                        {
                            if (agg[colName] == Aggregation.Count)
                                aggrValues[colName].Add(i + 1);
                            else
                                aggrValues[colName].Add(NAN);
                        }
                        else
                        {
                            var vals = rRolls[colName].Select(x => x);
                            var value = calculateAggregation(vals, agg[colName], this._colsType[j]);
                            aggrValues[colName].Add(value);

                            //remove the last one, so the next item can be add to the first position
                            rRolls[colName].Dequeue();
                        }
                    }

                    //reset rolling index when exceed the window value
                    if (rolIndex > window)
                        rolIndex = 1;
                    //
                    index++;
                }
            }

            return new DataFrame(aggrValues, this._index.ToList());
        }

        #endregion

        #region Shift
        /// <summary>
        /// Shifts the values of the column by the number of 'steps' rows. 
        /// </summary>
        /// <param name="columnName">existing column to be shifted</param>
        /// <param name="newColName">new shifted column</param>
        /// <param name="step"></param>
        /// <returns></returns>
        public Dictionary<string, List<object>> Shift(int steps, string columnName, string newColName)
        {

            if (steps == 0 )
                throw new Exception("'steps' must be nonzero and between ± row count number");

            if (!this.Columns.Contains(columnName))
                throw new Exception("'columnName' doesn't exist in the data frame.");

            if (this.Columns.Contains(newColName))
                throw new Exception("'newColName' cannot be the same of the existing column namess.");

            var newValues = new List<object>();
            var shitedCol = this[columnName].ToList();
            var NANList = Enumerable.Range(0, (int)Math.Abs(steps)).Select(x=>DataFrame.NAN);
            if (steps > 0)
            {
                shitedCol.InsertRange(0, NANList);
                newValues = shitedCol.Take(this.RowCount()).ToList();
            }
            else
            {
                shitedCol.AddRange(NANList);
                newValues = shitedCol.Skip((int)Math.Abs(steps)).ToList();
            }

            //create column and add to the df
            var dir = new Dictionary<string, List<object>>() { { newColName, newValues } };
            return dir;
        }



        /// <summary>
        /// Shift specified columns and create new columns in data frame
        /// </summary>
        /// <param name="arg">tuple list of steps, columnName and newColName.</param>
        /// <returns></returns>
        public DataFrame Shift(params (string columnName, string newColName, int steps)[] arg)
        {

            if(arg==null || arg.Length==0)
                throw new Exception("Method argument cannot be null.");

            if (arg.GroupBy(x => x.newColName).Any(g => g.Count() > 1))
                throw new Exception("newColumnName must be all with different name.");

            var dir = new Dictionary<string, List<object>>();

            foreach (var c in arg)
            {
                var d = Shift(c.steps, c.columnName, c.newColName);
                dir.Add(c.newColName , d[c.newColName]);
            }

            //
            var df = this.AddColumns(dir);
            return df;
        }


        /// <summary>
        /// Calculates the difference of a Dataframe row compared with previous row.
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public DataFrame Diff(int step, DiffType type = DiffType.Seasonal)
        {
            if (type == DiffType.Seasonal)
                return seasonalDiff(step);
            else
                return recursiveDiff(step);
        }

        #endregion

        #region Selection


        /// <summary>
        /// Returns data frame consisted of every nth row
        /// </summary>
        /// <param name="nthRow"></param>
        /// <param name="includeLast">For incomplete nthRow, select the last one</param>
        /// <returns></returns>
        public DataFrame TakeEvery(int nthRow, bool includeLast = false)
        {
            var val = new List<object>();
            var ind = new List<object>();

            //go through all rows
            int counter = 0;
            for (int i = 0; i < _index.Count; i++)
            {
                if ((i + 1) % nthRow == 0)
                {
                    val.AddRange(this[i]);
                    ind.Add(this._index[i]);
                    counter = 0;
                }
                if(i + 1 == _index.Count && includeLast)
                {
                    if(counter > 0)
                    {
                        val.AddRange(this[i]);
                        ind.Add(this._index[i]);
                    }
                }
                //
                counter++;
            }
            //
            var df = new DataFrame(val, ind, this._columns.ToList(), this._colsType);
            return df;
        }

        /// <summary>
        /// Returns data frame consisted of randomly selected n rows.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public DataFrame TakeRandom(int rows)
        {
            if (rows >= this.RowCount())
                return this;

            var selected = new List<int>();
            double needed = rows;
            double available = _index.Count;

            while (selected.Count < rows)
            {
                if (Constant.rand.NextDouble() < needed / available)
                {
                    selected.Add((int)available - 1);
                   needed--;
                }
                available--;
            }

            //
            var df = getDataFramesRows(selected);

            //
            return df;
        }

        /// <summary>
        /// Returns data frame with index element not containing in the index of the second data frame.
        /// Reset Index before call this method is recommended. 
        /// Index of the second data frame must be less of equal than the main data frame
        /// </summary>
        /// <param name="data2">Second data frame</param>
        /// <returns></returns>
        public DataFrame Except(DataFrame data2)
        {
            //rest of data should be define training dataset
            var resIndex = this._index.Select(x => Convert.ToInt32(x)).Except(data2.Index.Select(x => Convert.ToInt32(x))).ToList();
            var finalDf = DataFrame.CreateEmpty(this._columns);
            foreach (var i in resIndex)
                finalDf.AddRow(this[i].ToList());

            //reset index
            return finalDf;
        }

        /// <summary>
        /// Returns the formated string of the first  'count' rows of the data frame
        /// Suitable for the Jupyter notebooks
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataFrame Head(int count = 5)
        {
            //
            int rows = this.RowCount();
            int cols = this.ColCount();
            //
            var lst = new List<object>();
            var ind = new List<object>();
            long numR = Math.Min(rows, count);
            for (int i = 0; i < numR; i++)
            {
                lst.AddRange(this[i]);
                ind.Add(i);
            }
            return new DataFrame(lst, ind, this._columns, this._colsType);
        }

        /// <summary>
        /// Returns the formated string of the last  'count' rows of the data frame
        /// Suitable for the Jupyter notebooks
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataFrame Tail(int count = 5)
        {
            int rows = this.RowCount();
            int cols = this.ColCount();
            //
            var lst = new List<object>();
            var ind = new List<object>();

            int numR = Math.Min(rows, count);
            for (int i = rows - numR; i < rows; i++)
            {
                lst.AddRange(this[i]);
                ind.Add(i);
            }
            //
            return new DataFrame(lst, ind, this._columns, this._colsType);
        }

        /// <summary>
        /// Returns data frame consisted of first n rows.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public DataFrame Take(int rows)
        {
            var val = new List<object>();
            var ind = new List<object>();
            int counter = 1;
            for (int i = 0; i < _index.Count; i++)
            {
                ind.Add(_index[i]);
                val.AddRange(this[i]);
                if (counter >= rows)
                    break;

                counter++;
            }
            //
            var df = new DataFrame(val, ind, this._columns.ToList(), this._colsType);
            return df;
        }
        #endregion

        #endregion

        #region Indexers

        /// <summary>
        /// Returns zero based column index from the column name
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public int ColIndex(string colName)
        {
            return getColumnIndex(colName);
        }



        /// <summary>
        /// Returns specific value from Data Frame positioned at (rowIndex,colIndex )
        /// </summary>
        /// <param name="row">Zero based row index </param>
        /// <param name="col">Zero based col index</param>
        /// <returns>Object cell value</returns>
        public object this[int row, int col]
        {
            get
            {
                int ind = calculateIndex(row, col);
                return _values[ind];
            }
            set
            {
                int ind = calculateIndex(row, col);
                _values[ind] = value;
            }
        }

        /// <summary>
        /// Return specific value from Data Frame positioned at (rowIndex,colIndex )
        /// </summary>
        /// <param name="col">Column name</param>
        /// <param name="row">Zero based row index</param>
        /// <returns>Object cell value</returns>
        public object this[string col, int row]
        {
            get
            {
                int ind = calculateIndex(col, row);
                return _values[ind];
            }
            set
            {
                int ind = calculateIndex(col, row);
                _values[ind] = value;
            }
        }

        
        /// <summary>
        /// Return DataFrame generated from list of columns.
        /// </summary>
        /// <param name="col"></param>
        /// <returns>New Data Frame</returns>
        public DataFrame this[params string[] cols]
        {
            get
            {   //get indexes of the columns
                var idxs = getColumnIndex(cols);
                //reserve for space
                var lst = new object[cols.Length * _index.Count];
                var counter = 0;
                var newCounter = 0;
                for (int i = 0; i < _index.Count; i++)
                {
                    for (int j = 0; j < idxs.Length; j++)
                    {
                        lst[newCounter + j] = this._values[counter + idxs[j]];                   
                    }
                    //increase indexes
                    newCounter += idxs.Length;
                    counter +=_columns.Count;
                }

                ColType[] colTypes= Array.Empty<ColType>();
                if (_colsType != null && _colsType != Array.Empty<ColType>())
                    colTypes = idxs.Select(j=>_colsType[j]).ToArray();

                var df = new DataFrame(lst.ToList(),this._index.ToList(), cols.ToList(), colTypes);
                return df;
            }
        }

        /// <summary>
        /// Return Specific Column  from Data Frame
        /// </summary>
        /// <param name="col">COlumn name</param>
        /// <returns>Enumerated object list</returns>
        public IEnumerable<object> this[string col]
        {
            get
            {
                var cols = ColCount();
                var colIndex = getColumnIndex(col);
                for (int i = colIndex; i < _values.Count; i += cols)
                    yield return _values[i];
            }
        }

        /// <summary>
        /// Return specific row from Data Frame
        /// </summary>
        /// <param name="row">Zero based row index</param>
        /// <returns>Enumerated object list.</returns>
        public IEnumerable<object> this[int row]
        {
            get
            {
                var cols = ColCount();
                var start = calculateIndex(row, 0);// row * cols;
                for (int i = start; i < start + cols; i++)
                    yield return _values[i];
            }
        }
        #endregion

        #region Interface Implementation
        /// <summary>
        /// Returns the number of rows in the data frame
        /// </summary>
        /// <returns>Integer value</returns>
        public int RowCount()
        {
            return Index.Count;
        }
        /// <summary>
        /// Returns the number of columns in the data frame
        /// </summary>
        /// <returns></returns>
        public int ColCount()
        {
            return Columns.Count;
        }

        #endregion

        #region Print Helper
        /// <summary>
        /// Customization of the standard ToString method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var str = $"({Index.Count},{Columns.Count})";
            return str;
        }

        public string ToStringBuilder(int rowCount=15)
        {
            StringBuilder sb = new StringBuilder();
            int rows = this.RowCount();
            int cols = this.ColCount();
            int longestColumnName = 0;
            for (int i = 0; i < cols; i++)
            {
                longestColumnName = Math.Max(longestColumnName, this.Columns[i].Length);
            }
            //add space for idnex
            sb.Append(string.Format("".PadRight(longestColumnName)));
            for (int i = 0; i < cols; i++)
            {
                // Left align by 10
                sb.Append(string.Format(this.Columns[i].PadRight(longestColumnName)));
            }
            sb.AppendLine();
            //
            var rr = Math.Min(rowCount, rows);
            for (int i = 0; i < rr; i++)
            {
                sb.Append((_index[i]).ToString().PadRight(longestColumnName));
                IList<object> row = this[i].ToList();
                foreach (object obj in row)
                {
                    sb.Append((obj ?? "null").ToString().PadRight(longestColumnName));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public Array To1DArray()
        {
            return this._values.ToArray();
        }

        public string ToConsole(int rowCount = 15)
        {
            StringBuilder sb = new StringBuilder();
            int rows = this.RowCount();
            int cols = this.ColCount();
            int longestColumnName = 0;
            for (int i = 0; i < cols; i++)
            {
                longestColumnName = Math.Max(longestColumnName, this.Columns[i].Length);
            }
            sb.AppendLine();
            sb.Append("index".PadRight(longestColumnName));
            for (int i = 0; i < cols; i++)
            {
                // Left align by 10
                sb.Append(string.Format(this.Columns[i].PadRight(longestColumnName)));
            }
            sb.AppendLine();
            int lenCols = sb.Length;
            for (int i=0; i< lenCols; i++)
                sb.Append("-");
            for (int i = 0; i < lenCols; i++)
                sb.Insert(0,"-");
            
            sb.AppendLine();
            //
            var rr = Math.Min(rowCount, rows);
            for (int i = 0; i < rr; i++)
            {
                IList<object> row = this[i].ToList();

                sb.Append((Index[i] ?? "null").ToString().PadRight(longestColumnName));
                foreach (object obj in row)
                {
                    sb.Append((obj ?? "null").ToString().PadRight(longestColumnName));
                }
                sb.AppendLine();
            }
            for (int i = 0; i < lenCols; i++)
                sb.Append("_");
            return sb.ToString();
        }

        #endregion

        #region Series Related Operations

        

        /// <summary>
        /// Add series as DataFrame column 
        /// </summary>
        /// <param name="ser">series </param>
        /// <returns></returns>
        public DataFrame AddColumn(Series ser)
        {
            //
            checkColumnName(this._columns, ser.Name);
            //
            var vals = new List<object>();
            for (int i = 0; i < this._index.Count; i++)
            {
                vals.AddRange(this[i]);
                vals.Add(ser[i]);
            }

            //new column
            var newCols = Columns.ToList();
            newCols.Add(ser.Name);

            //new column type
            var newType = ser.ColType;
            if (this._colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = this.columnsTypes();

            var newcolTypes = this._colsType.ToList();
            newcolTypes.Add(newType);
            //
            var index = this._index.ToList();
            return new DataFrame(vals, index, newCols, newcolTypes.ToArray());
        }

        /// <summary>
        /// Add series as DataFrame columns 
        /// </summary>
        /// <param name="ser">series </param>
        /// <returns></returns>
        public DataFrame AddColumns(params Series[] sers)
        {
            //
            checkColumnNames(this._columns, sers.Select(x => x.Name).ToArray());
            //
            var vals = new List<object>();
            for (int i = 0; i < this._index.Count; i++)
            {
                vals.AddRange(this[i]);
                vals.AddRange(sers.Select(x => x[i]));
            }
            //
            var newCols = Columns.Union(sers.Select(x => x.Name)).ToList();

            if (this._colsType == null || _colsType == Array.Empty<ColType>())
                this._colsType = this.columnsTypes();

            var newTypes = this._colsType.Union(sers.Select(x=>x.ColType)).ToArray();

            //
            var index = this._index.ToList();
            return new DataFrame(vals, index, newCols, newTypes);
        }

        /// <summary>
        /// Extract a column as series object including index
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public Series ToSeries(string colName)
        {
            if(!this._columns.Contains(colName))
                throw new Exception($"Column {colName} do not exist in the dataframe.");

            var data = this[colName].ToList();
            var ind = this._index.ToList();
            var s = new Series(data, ind, colName);
            return s;
        }

        /// <summary>
        /// Extract a columns as array series object including index
        /// </summary>
        /// <param name="colNames"></param>
        /// <returns></returns>
        public Series[] ToSeries(params string[] colNames)
        {
            var retVal = new List<Series>();
            foreach(var colName in colNames)
            {
                if (!this._columns.Contains(colName))
                    throw new Exception($"Column {colName} do not exist in the dataframe.");

                var data = this[colName].ToList();
                var ind = this._index.ToList();
                var s = new Series(data, ind, colName);
                retVal.Add(s);
            }

            return retVal.ToArray();
        }

        public Series ToSeries()
        {
            if (this.Columns.Count != 1)
                throw new Exception("DataFrame must have one column to be converted into Series.");
            var ser = new Series(this._values, this._index, this._columns.First());

            return ser;
        }
        #endregion

        #region Private
        private DataFrame recursiveDiff(int order)
        {
            
            var oldDf = this;
            for (int i = 0; i < order; i++)
            {
                DataFrame newDf = DataFrame.CreateEmpty(Columns);
                for(int d=0; d <= i; d++)
                    newDf.AddRow(new object[ColCount()].ToList());
                //calculate differencing
                for (int j = i+1; j < oldDf.RowCount(); j++)
                {
                    var prevrow = new Series(oldDf[j - 1].ToList());
                    var row = new Series(oldDf[j].ToList());
                    var diffs = row - prevrow;
                    newDf.AddRow(diffs.ToList());
                }

                oldDf = newDf;
            }
            return oldDf;
        }

        private DataFrame seasonalDiff(int period)
        {
            var newDf = DataFrame.CreateEmpty(Columns);
            //
            for (int i = 0; i < period; i++)
            {
                var nanrow = new object[ColCount()].ToList();
                newDf.InsertRow(0, nanrow);
            }
            //calculate differencing
            for (int i = period; i < RowCount(); i++ )
            {
                var prevrow = new Series(this[i - period].ToList());
                var row = new Series(this[i].ToList());
                var diffs = row - prevrow;
                newDf.InsertRow(i, diffs.ToList());
            }

            return newDf;
        }

        private List<string> mergeColumnNames(IList<string> cols1, IList<string> cols2, string sufix = null)
        {
            //merge columns
            var totColumns = cols1.ToList();
            //
            for (int i = 0; i < cols2.Count; i++)
            {
                var colName = cols2[i];
                //if the first list already contains the columnName
                if (totColumns.Contains(colName) && !string.IsNullOrEmpty(sufix))
                {
                    var newColName = $"{colName}_{sufix}";
                    //if the new column Names already occupied throw exception
                    if (totColumns.Contains(newColName))
                        throw new Exception($"Column suffix {sufix} produces duplicated column. Please change the column suffix for columns merge.");

                    totColumns.Add(newColName);
                }
                else if (totColumns.Contains(colName) && string.IsNullOrEmpty(sufix))
                    continue;
                else
                    totColumns.Add(colName);
            }

            return totColumns;
        }
        private (List<string>, List<ColType>) mergeColumns(
                IList<string> cols1, ColType[] typ1, IList<string> cols2, ColType[] typ2, string sufix = null)
        {
            //merge columns
            var totColumns = cols1.ToList();
            var totType = typ1.ToList();

            //
            for (int i = 0; i < cols2.Count; i++)
            {
                var colName = cols2[i];
                var colType = typ2[i];
                //if the first list already contains the columnName
                if (totColumns.Contains(colName) && !string.IsNullOrEmpty(sufix))
                {
                    var newColName = $"{colName}_{sufix}";
                    //if the new column Names already occupied throw exception
                    if (totColumns.Contains(newColName))
                        throw new Exception($"Column suffix {sufix} produces duplicated column. Please change the column suffix for columns merge.");

                    totColumns.Add(newColName);
                    totType.Add(colType);
                }
                else if (totColumns.Contains(colName) && string.IsNullOrEmpty(sufix))
                    continue;
                else
                {
                    totColumns.Add(colName);
                    totType.Add(colType);
                }
            }

            return (totColumns, totType);
        }


        private (ILookup<object, int> lookup1, TwoKeyLookup<object, object, int> lookup2, ThreeKeyLookup<object, object, object, int> lookup3) createLookup(DataFrame df, string[] cols)
        {
            var rightInd = df.getColumnIndex(cols);
            var rIndex = Enumerable.Range(0, df.RowCount()).ToArray();

            //create right lookup 
            ILookup<object, int> lookup1 = null;
            TwoKeyLookup<object, object, int> lookup2 = null;
            ThreeKeyLookup<object, object, object, int> lookup3 = null;

            //construct lookup table
            if (rightInd.Length == 1)
                lookup1 = df[cols[0]].Zip(rIndex, (key, value) => (key, value)).ToLookup(x => x.key, x => x.value);
            else if (rightInd.Length == 2)
                lookup2 = new TwoKeyLookup<object, object, int>(rIndex, item => (df[item, rightInd[0]], df[item, rightInd[1]]));
            else if (rightInd.Length == 3)
                lookup3 = new ThreeKeyLookup<object, object, object, int>(rIndex, item => (df[item, rightInd[0]], df[item, rightInd[1]], df[item, rightInd[2]]));
            else
                throw new Exception("Unknown number of Key columns");

            return (lookup1, lookup2, lookup3);
        }

        private int[] findIndex(ILookup<object, int> lookup1, TwoKeyLookup<object, object, int> lookup2, ThreeKeyLookup<object, object, object, int> lookup3, int[] colInd, int i)
        {
            if (lookup1 != null)
                return lookup1[this[i, colInd[0]]].ToArray();
            else if (lookup2 != null)
                return lookup2[this[i, colInd[0]], this[i, colInd[1]]].ToArray();
            else if (lookup3 != null)
                return lookup3[this[i, colInd[0]], this[i, colInd[1]], this[i, colInd[2]]].ToArray();
            else
                throw new Exception("Unknown number of key columns.");
        }

        private int[] containsKey(int i, int[] leftInd, List<ILookup<object, int>> right)
        {
            if (leftInd.Length != right.Count)
                throw new Exception("The number o elements must be equal.");

            var leftRow = this[i];

            //try the first column
            var lftValue = this[i, leftInd[0]];
            if (!right[0].Contains(lftValue))
                return null;

            //in case one one joined column return
            var firstIndexes = right[0][lftValue].ToArray();
            if (leftInd.Length == 1)
                return firstIndexes;

            //check the second column
            var lftValue2 = this[i, leftInd[1]];
            if (!right[1].Contains(lftValue2))
                return null;
            var secondIndexes = right[1][lftValue2].ToArray();
            //intersect two arrays
            var firstSecondIndexes = firstIndexes.Intersect(secondIndexes).ToArray();

            //no intersected elements
            if (firstSecondIndexes.Length <= 0)
                return null;

            if (leftInd.Length == 2)
                return firstSecondIndexes;

            //check the third column
            var lftValue3 = this[i, leftInd[2]];
            if (!right[2].Contains(lftValue3))
                return null;
            var thirdIndexes = right[2][lftValue3].ToArray();

            var finalIndexes = firstSecondIndexes.Intersect(thirdIndexes).ToArray();

            return finalIndexes;
        }

        private bool isOperationSupported(ColType colType, Aggregation aggOperation)
        {
            switch (colType)
            {
                case ColType.I2://boolean
                case ColType.IN://categorical
                case ColType.STR://string
                    {
                        if (aggOperation == Aggregation.Count
                            || aggOperation == Aggregation.First
                            || aggOperation == Aggregation.Last
                            )
                            return true;
                        else
                            return false;
                    }
                case ColType.I32:
                case ColType.I64:
                case ColType.F32:
                case ColType.DD:
                    return true;
                case ColType.DT:
                    {
                        if (aggOperation == Aggregation.Avg
                           || aggOperation == Aggregation.Std
                           || aggOperation == Aggregation.Sum
                           )
                            return false;
                        else
                            return true;
                    }
                default:
                    throw new Exception("The column type is not supported.");
            }
        }

        private void rowToArray(object[] processingRow, int rowIndex)
        {
            if (processingRow == null && processingRow.Length != ColCount())
                throw new ArgumentException($"'{nameof(processingRow)}' cannot be null or with different length than columns count.");

            //
            var i = calculateIndex(rowIndex, 0);
            //
            for (int j = 0; j < this._columns.Count; j++)
            {
                var value = _values[i + j];
                processingRow[j] = value;
            }

            return;
        }

        private void rowToDictionary(Dictionary<string, object> processingRow, int rowIndex)
        {
            if (processingRow == null)
                throw new ArgumentException($"'{nameof(processingRow)}' cannot be null");

            //
            var i = calculateIndex(rowIndex, 0);
            //
            for (int j = 0; j < this._columns.Count; j++)
            {
                var value = _values[i + j];
                processingRow[this._columns[j]] = value;
            }

            return;
        }
      
        static private bool isNumeric(ColType cType)
        {
            if (cType == ColType.I32 || cType == ColType.I64 || cType == ColType.F32 || cType == ColType.DD)
                return true;
            else
                return false;
        }

        static private bool isCategorical(ColType cType)
        {
            if (cType == ColType.I2 || cType == ColType.IN)
                return true;
            else
                return false;
        }

        static private bool isObject(ColType cType)
        {
            if (cType == ColType.STR)
                return true;
            else
                return false;
        }

        static private bool isDateTime(ColType cType)
        {
            if (cType == ColType.DT)
                return true;
            else
                return false;
        }

        private DataFrame reverse()
        {
            var cols = this.Columns;
            var types = this._colsType;

            //
            var lst = new List<object>();
            var lstInd = new List<object>();
            for(int i= this.Index.Count-1; i >=0 ; i--)
            {
                for (int j = 0; j < this.Columns.Count; j++)
                {
                    var v = this[i, j];
                    lst.Add(v);
                }
                //
                lstInd.Add(this._index[i]);
            }
            //
            var dff = new DataFrame(lst, lstInd, cols, types);
            return dff;
        }

        private DataFrame getDataFramesRows(List<int> selected)
        {
            var val = new List<object>();
            var ind = new List<object>();
            //go through selected rows
            for (int i = 0; i < selected.Count; i++)
            {
                var row = this[selected[i]];
                val.AddRange(row);
                ind.Add(this._index[selected[i]]);
            }
            //
            var df = new DataFrame(val, ind, this._columns.ToList(), this._colsType);
            return df;
        }


        private Dictionary<object, DataFrame> groupDFBy(string groupCol)
        {
            var Group = new Dictionary<object, DataFrame>();
            
            //go through all data to group
            var pos = 0;
            var rows = Index.Count;
            //
            for (int i = 0; i < rows; i++)
            {
                var row = new List<object>();
                object groupValue = null;
                var colCnt = ColCount();
                var grpColIndex = getColumnIndex(groupCol);

                //
                for (int j = 0; j < colCnt; j++)
                {
                    row.Add(_values[pos]);
                    
                   // if (Columns[j].Equals(groupCol, StringComparison.InvariantCultureIgnoreCase))
                   if(grpColIndex==j)
                        groupValue = _values[pos];
                    pos++;
                }

                //add to group
                if (!Group.ContainsKey(groupValue))
                    Group.Add(groupValue, new DataFrame(row, new List<object>(){_index[i]}, this.Columns, this._colsType));
                else
                    Group[groupValue].AddRow(row, _index[i]);
            }

            return Group;
        }


        private bool applyOperator(int[] indCols, object[] rowValues, object[] filteValues, FilterOperator[] fOpers)
        {
            for (int colIndex = 0; colIndex < rowValues.Length; colIndex++)
            {
                if (this._colsType == null || _colsType == Array.Empty<ColType>())
                    this._colsType = this.columnsTypes();

                var fOper = fOpers[colIndex];
                if (this._colsType[indCols[colIndex]] == ColType.I2)
                {
                    var val1 = Convert.ToBoolean(rowValues[colIndex]);
                    var val2 = Convert.ToBoolean(filteValues[colIndex]);
                    if (fOper != FilterOperator.Equal)
                        throw new Exception("Equal should be assign to boolean columns.");
                    if (val1 != val2)
                        return false;
                }
                if (this._colsType[indCols[colIndex]] == ColType.I32 || this._colsType[indCols[colIndex]] == ColType.I64
                    || this._colsType[indCols[colIndex]] == ColType.F32 || this._colsType[indCols[colIndex]] == ColType.DD)
                {
                    var val1 = Convert.ToDouble(rowValues[colIndex]);
                    var val2 = Convert.ToDouble(filteValues[colIndex]);

                    //
                    if (!applyOperator(val1, val2, fOper))
                        return false;
                }
                else if (this._colsType[indCols[colIndex]] == ColType.STR || this._colsType[indCols[colIndex]] == ColType.IN)
                {
                    var val1 = rowValues[colIndex].ToString();
                    var val2 = filteValues[colIndex].ToString();
                    //
                    if (!applyOperator(val1, val2, fOper))
                        return false;

                }
                else if (this._colsType[indCols[colIndex]] == ColType.DT)
                {
                    var val1 = Convert.ToDateTime(rowValues[colIndex]);
                    var val2 = Convert.ToDateTime(filteValues[colIndex]);
                    //
                    if (!applyOperator(val1, val2, fOper))
                        return false;
                }
                else
                    throw new Exception("Unknown column type");
            }

            return true;
        }

        private static bool applyOperator(DateTime val1, DateTime val2, FilterOperator fOper)
        {
            switch (fOper)
            {
                case FilterOperator.Equal:
                    return val1 == val2;
                case FilterOperator.Notequal:
                    return val1 != val2;
                case FilterOperator.Greather:
                    return val1 > val2;
                case FilterOperator.Less:
                    return val1 < val2;
                case FilterOperator.GreatherOrEqual:
                    return val1 >= val2;
                case FilterOperator.LessOrEqual:
                    return val1 <= val2;
                case FilterOperator.IsNUll:
                    throw new Exception("value cannot be null");
                case FilterOperator.NonNull:
                    return true;
                default:
                    throw new Exception("Unknown operator!");
            }
        }

        private static bool applyOperator(string val1, string val2, FilterOperator fOper)
        {
            switch (fOper)
            {
                case FilterOperator.Equal:
                    return val1 == val2;
                case FilterOperator.Notequal:
                    return val1 != val2;
                case FilterOperator.Greather:
                    throw new Exception("Operator for string is not supported!");
                case FilterOperator.Less:
                    throw new Exception("Operator for string is not supported!");
                case FilterOperator.GreatherOrEqual:
                    throw new Exception("Operator for string is not supported!");
                case FilterOperator.LessOrEqual:
                    throw new Exception("Operator for string is not supported!");
                case FilterOperator.IsNUll:
                    throw new Exception("value cannot be null");
                case FilterOperator.NonNull:
                    return true;
                default:
                    throw new Exception("Unknown operator!");
            }
        }

        private static bool applyOperator(double val1, double val2, FilterOperator fOper)
        {
            switch (fOper)
            {
                case FilterOperator.Equal:
                    return val1 == val2;
                case FilterOperator.Notequal:
                    return val1 != val2;
                case FilterOperator.Greather:
                    return val1 > val2;
                case FilterOperator.Less:
                    return val1 < val2;
                case FilterOperator.GreatherOrEqual:
                    return val1 >= val2;
                case FilterOperator.LessOrEqual:
                    return val1 <= val2;
                case FilterOperator.IsNUll:
                    throw new Exception("value cannot be null");
                case FilterOperator.NonNull:
                    return true;
                default:
                    throw new Exception("Unknown operator!");
            }
        }

        private void checkColumnNames(List<string> columns, string[] colNames)
        {
            var sameCols = this._columns.Intersect(colNames).ToArray();
            //check if column exists
            if (sameCols.Length > 0)
            {
                var str = string.Join(", ", sameCols);
                throw new Exception($"Column(s) '{str}' already exist(s) in the data frame.");
            }
        }

        private void checkColumnName(List<string> columns, string colName)
        {
            //check if column exists
            if (columns.Contains(colName))
            {
                throw new Exception($"Column '{colName}' already exists in the data frame.");
            }
        }

        private ColType[] columnsTypes()
        {
            int cc = ColCount();
            var types = new ColType[cc];
            var k = 0;
            for (int i = 0; i < Columns.Count; i++)
            {

                while (_values[(i + k * Columns.Count)] == NAN)
                {
                    k++;
                    //if the type is not found put default type to the column
                    if (_values.Count < i + k * Columns.Count)
                    {
                        types[i] = ColType.STR;
                        k = 0;
                        i++;
                        break;
                    }
                    continue;
                }
                var ind = i + k * Columns.Count;

                types[i] = GetValueType(_values[ind]);
            }
            return types;
        }

        internal static ColType GetValueType(object value)
        {
            if(value == null)
                throw new ArgumentNullException("The value cannot be null.");

            if (value.GetType() == typeof(bool))
                return ColType.I2;
            else if (value.GetType() == typeof(int))
                return ColType.I32;
            else if (value.GetType() == typeof(long))
                return ColType.I64;
            else if (value.GetType() == typeof(float))
                return ColType.F32;
            else if (value.GetType() == typeof(double))
                return ColType.DD;
            else if (value.GetType() == typeof(string))
                return ColType.STR;
            else if (value.GetType() == typeof(DateTime))
                return ColType.DT;
            else
                throw new Exception("Unknown column type");
        }

        private int calculateIndex(string col, int row)
        {
            int iind;
            int cIndex = getColumnIndex(col);
            iind = calculateIndex(row, cIndex);
            return iind;
        }

        private int calculateIndex(int row, int col)
        {
            var numCols = ColCount();
            var iind = row * numCols + col;
            return iind;
        }

        private int[] getColumnIndex(params string[] cols)
        {
            var inds = new List<int>();
            if (cols == null || cols.Length == 0)
                return null;
            else
            {
                foreach (var ii in cols)
                {
                    var i = getColumnIndex(ii);
                    inds.Add(i);
                }
                return inds.ToArray();
            }
        }

        private int getColumnIndex(string col)
        {
            //
            for (int i = 0; i < Columns.Count; i++)
            {
                var colStr = Columns[i];
                if(colStr==col)
                {
                    return i;
                }
            }

            throw new ArgumentException($"Column '{col}' does not exist in the Data Frame. Column names are case sensitive.");
        }

        internal void addRows(DataFrame df)
        {
            this._values.AddRange(df._values);
            this._index.AddRange(df._index);
        }
        #endregion
    }
}
