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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Daany.Grouping;
using Daany.MathStuff.Random;
using Daany.Multikey;
using System.Diagnostics.Metrics;
using Microsoft.VisualBasic;
using Daany.Interfaces;

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
        public List<string> Columns => _columns ?? throw new ArgumentNullException(nameof(_columns));


		/// <summary>
		/// Types of columns (names) in the data frame.
		/// </summary>
		/// 
		public IList<ColType> ColTypes
		{
			get
			{

                EnsureColumnTypesInitialized();
                return _colTypes!;
			}
		}


		/// <summary>
		/// Index for rows in the data frame.
		/// </summary>
		/// 
		public Daany.Index Index => _index ?? throw new ArgumentNullException(nameof(_index));


		public (int rows, int cols) Shape => (RowCount(), ColCount());

        public IList<object?> Values => _values ?? throw new ArgumentNullException(nameof(_values));



        /// <summary>
        /// Representation of missing value.
        /// </summary>
        /// 
        public static object? NAN => null;

		#endregion

		#region Private fields
		private List<object?>? _values;
        private Daany.Index? _index;
        private List<string>? _columns;
		private ColType[]? _colTypes;

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
		public IEnumerable<TRow> GetEnumerator<TRow>(Func<IDictionary<string, object?>, TRow> callBack)
		{
			// Reuse the same dictionary to avoid allocations
			var rowDictionary = new Dictionary<string, object?>(Columns.Count);

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
					rowDictionary[Columns[colIndex]] = Values[valueIndex] ?? NAN;
				}

				yield return callBack(rowDictionary);
			}
		}

		/// <summary>
		/// Return row enumerators by returning row as dictionary 
		/// </summary>
		/// <returns>dictionary</returns>
		public IEnumerable<IDictionary<string, object?>> GetEnumerator()
		{
			var rowDictionary = new Dictionary<string, object?>(Columns.Count);

			// Pre-calculate counts
			int columnCount = Columns.Count;
			int valueCount = Values.Count; 

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
					object? value = Values[rowStart + colIndex];
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
					rowArray[colIndex] = Values[rowStart + colIndex]!;
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

            if (ind.Count != Index.Count)
                throw new Exception("Wrong count of index list.");

            this._index = new Index(ind, name);
        }

        public DataFrame SetIndex(string colName)
        {
            if (!Columns.Contains(colName))
                throw new Exception($"{colName} does not exist.");

            //all cols except colName
            var cols = Columns.Where(x=>x!= colName).ToArray();

			//create new index
			if (this[colName].Any(x => x == DataFrame.NAN))
			{
				throw new ArgumentException($"Column {colName} cannot be used as index. It contains missing values.");
			}

			var ind= this[colName].ToList();
            var df = this[cols];
            df._index = new Index(ind!, colName);

            return df;
            
        }

        public DataFrame ResetIndex(bool drop = false)
        {
            var colName = this.Index.Name;
            var colVal = this.Index.ToList();
            var newDf = this[Columns.ToArray()];
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
			_values = new List<object?>();
			_index = new Index(new List<object>());
			_columns = new List<string>();
			_colTypes = Array.Empty<ColType>();
		}

		public DataFrame(params (string columnName, object?[] values)[] data)
		{
			if (data == null || data.Length == 0)
				throw new ArgumentException("DataFrame must contain at least one column.");

			
			var columns = data.Select(c => c.columnName).ToList();
			int rowCount = data.SelectMany(x => x.values).Count() / columns.Count;
			var index = new Index(GenerateDefaultIndex(rowCount));

			var values = Enumerable.Range(0, rowCount)
								   .SelectMany(i => data.Select(column => column.values[i]))
								   .ToList();

			var colTypes = columnsTypes(values, rowCount, columns.Count);

			InitializeDataFrame(values, index, columns, colTypes);
		}

		/// <summary>
		/// Initializes a new instance of the DataFrame class using a dictionary with 
		/// two keys to define columns and indexed rows.
		/// </summary>
		/// <param name="data">
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

		internal DataFrame(TwoKeysDictionary<string, object, object> data)
		{
			if (data == null || data.Count == 0)
				throw new ArgumentException("The dictionary 'data' cannot be null or empty.", nameof(data));

			var indexSet = new HashSet<object>();
			var columns = new List<string>(data.Select(c => c.Key));

			foreach (var c in data)
			{
				if (c.Value is IEnumerable<KeyValuePair<object, object>> keyValuePairs)
				{
					foreach (var kvp in keyValuePairs)
					{
						indexSet.Add(kvp.Key);
					}
				}
				else
				{
					throw new InvalidOperationException($"The value associated with column '{c.Key}' is not enumerable.");
				}
			}

			var orderedIndex = indexSet.ToList();
			var values = orderedIndex
				.SelectMany(rowIndex => columns
					.Select(column => data.ContainsKey(column, rowIndex)
						? data[column, rowIndex]
						: DataFrame.NAN))
				.ToList();

			var colTypes = columnsTypes(values, orderedIndex.Count, columns.Count);
			
			InitializeDataFrame(values, new Index(orderedIndex), columns, colTypes);
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
		/// <param name="columns">
		/// A list of strings representing the column names of the data.
		/// </param>
		/// <param name="colTypes">
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

		public DataFrame(List<object?> data, Index index, List<string> columns, ColType[] colTypes)
		{
			InitializeDataFrame(data, index, columns, colTypes);
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
		public DataFrame(List<object?> data, List<object> index, List<string> columns, ColType[] colTypes)
		{
			InitializeDataFrame(data, new Index(index), columns, colTypes?? throw new ArgumentNullException(nameof(colTypes)));
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
		/// var df = new DataFrame(data, index, columns);
		/// </example>
		public DataFrame(List<object?> data, List<object> index, List<string> columns)
		{
			InitializeDataFrame(data, new Index(index), columns, columnsTypes(data, index.Count, columns.Count));
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

			InitializeDataFrame(
				new List<object?>(dataFrame.Values),
				new Index(dataFrame.Index.ToList()),
				new List<string>(dataFrame.Columns),
				dataFrame.ColTypes.ToArray()
			);
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
			ValidateData(data, columns);

			int rowCount = data.Length / columns.Count;
			
			var colTypes = columnsTypes(new List<object?>(data), rowCount, columns.Count);

			InitializeDataFrame(new List<object?>(data), new Index(GenerateDefaultIndex(rowCount)), new List<string>(columns), colTypes);
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
		/// var df = new DataFrame(data, columns);
		/// </example>
		public DataFrame(List<object?> data, List<string> columns)
		{
			ValidateData(data, columns);

			int rowCount = data.Count / columns.Count;

			var colTypes = columnsTypes(new List<object?>(data), rowCount, columns.Count);

			InitializeDataFrame(new List<object?>(data), new Index(GenerateDefaultIndex(rowCount)), new List<string>(columns), colTypes);
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
		/// An array of ColType enumerations representing the data types of the columns. 
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
		public DataFrame(List<object?> data, List<string> columns, ColType[]? colTypes)
		{
			ValidateData(data, columns);
			int rowCount = data.Count / columns.Count;
			
			if (colTypes != null && colTypes.Length != columns.Count)
				throw new ArgumentException("The number of column types must match the number of columns.", nameof(colTypes));

			InitializeDataFrame(data.Cast<object?>().ToList(), new Index(GenerateDefaultIndex(rowCount)), new List<string>(columns), colTypes);
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
		public DataFrame(IDictionary<string, List<object?>> data, IList<object>? index = null)
		{
			ValidateData(data);

			var firstColumnValues = data.Values.First();
			var computedIndex = index == null
				? new Index(Enumerable.Range(0, firstColumnValues.Count).Cast<object>().ToList())
				: new Index(index.ToList());

			var columns = data.Keys.ToList();
			var values = Enumerable.Range(0, firstColumnValues.Count)
				.SelectMany(row => data.Values.Select(column => ParseValue(column[row], null)))
				.ToList();

			var colTypes = columnsTypes(values, computedIndex.Count, columns.Count);

			InitializeDataFrame(values, computedIndex, columns, colTypes);
		}

		private void InitializeDataFrame(List<object?> data, Index index, List<string> columns, ColType[]? colTypes)
		{
			ValidateData(data, columns);

			if (index == null || index.Count != data.Count/columns.Count)
				throw new ArgumentException("Index cannot be null or empty.", nameof(index));

			_columns = new List<string>(columns);
			_index = new Index(index.ToList());
			_values = new List<object?>(data);

			if (colTypes == null || colTypes.Length != _columns.Count)
				throw new ArgumentException(nameof(colTypes));

			_colTypes = new ColType[colTypes.Length];
			Array.Copy(colTypes, _colTypes, colTypes.Length);
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
			EnsureColumnTypesInitialized();

			var dict = new Dictionary<string, List<object?>>();

			//define new name for columns
			for (int i = 0; i < colNames.Length; i++)
			{
				//get th ecolumn
				var colName = string.IsNullOrEmpty(colNames[i].newName) ? colNames[i].oldName : colNames[i].newName;
				dict.Add(colName, this[colNames[i].oldName].ToList());
			}
			var newDf = new DataFrame(dict);
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
			df._values = new List<object?>();
			df._index = new Index(new List<object>());
			df._columns = new List<string>(columns);
			return df;
		}

		private DataFrame CreateNewDataFrame(List<object?> clippedValues)
		{
			// Create a new DataFrame with the same structure as the current one
			var cols = Columns.ToList();
			var types = ColTypes.ToArray();

			//TODO: check if index should be clipped as well
			var ind = Index.ToList();

			return new DataFrame(clippedValues, ind, cols, types);
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

		private void ValidateData(List<object?> data, List<string> columns)
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
		private void ValidateData(IDictionary<string, List<object?>> data)
		{
			if (data == null || data.Count == 0)
				throw new ArgumentException("Dictionary cannot be null or empty.", nameof(data));

			int firstCount = data.Values.First().Count;
			if (!data.Values.All(list => list.Count == firstCount))
				throw new Exception("All lists within dictionary must be of the same length.");

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
            EnsureColumnTypesInitialized();

			// Set the column type
			_colTypes![index] = colType;
		}

		private void EnsureColumnTypesInitialized()
		{
			if (_colTypes == null || _colTypes == Array.Empty<ColType>())
				_colTypes = columnsTypes(_values?? throw new ArgumentNullException(nameof(_values)), Index.Count, Columns.Count);
		}

		#endregion

		#region Data Frame Operations

		#region add-append
		/// <summary>
		/// Adds new columns to the DataFrame while ensuring the row counts match and the column names are valid.
		/// </summary>
		/// <param name="columnsToAdd">
		/// A dictionary where keys are column names and values are lists representing column data.
		/// Each list must match the row count of the DataFrame.
		/// </param>
		/// <returns>
		/// A new DataFrame instance with the combined columns.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown when the row count of a column in <paramref name="columnsToAdd"/> does not match the DataFrame row count, 
		/// or when the input dictionary is null or empty.
		/// </exception>
		/// <example>
		/// Example usage:
		/// var dataFrame = new DataFrame(
		///     new List<object> { 1, "A", 2, "B" }, // Existing data.
		///     new List<object> { 0, 1 },          // Index.
		///     new List<string> { "Col1", "Col2" }, // Existing column names.
		///     null);
		///
		/// var newColumns = new Dictionary<string, List<object>>
		/// {
		///     { "Col3", new List<object> { 3, 4 } },
		///     { "Col4", new List<object> { "C", "D" } }
		/// };
		///
		/// var updatedDataFrame = dataFrame.AddColumns(newColumns);
		/// // Result: DataFrame with columns "Col1", "Col2", "Col3", and "Col4"
		/// </example>
		public DataFrame AddColumns(Dictionary<string, List<object?>> columnsToAdd)
		{
			// Check if the input dictionary is null or empty.
			if (columnsToAdd == null || !columnsToAdd.Any())
				throw new ArgumentException("The columns to add cannot be null or empty.");

			// Validate that all new columns have the same row count as the DataFrame.
			foreach (var column in columnsToAdd)
			{
				if (RowCount() != column.Value.Count)
					throw new ArgumentException($"Row count mismatch for column '{column.Key}'. Expected {RowCount()}, but got {column.Value.Count}.");
			}

			// Ensure no duplicate or invalid column names are introduced.
			checkColumnNames(Columns, columnsToAdd.Keys.ToArray());

			// Create a new list to hold the combined values for the updated DataFrame.
			var updatedValues = new List<object?>(Index.Count * (Columns.Count + columnsToAdd.Count));
			for (int i = 0; i < Index.Count; i++)
			{
				// Add the existing row values to the list.
				updatedValues.AddRange(this[i]);

				// Append the values from the new columns for the current row.
				foreach (var column in columnsToAdd)
					updatedValues.Add(column.Value[i]);
			}

			// Generate the updated column names by combining the existing names with the new ones.
			var newColumnNames = Columns.Union(columnsToAdd.Keys).ToList();

			//regenerate column types
			var colTypes = columnsTypes(updatedValues, Index.Count, newColumnNames.Count);

			// Return a new DataFrame with updated values, column names, and index.
			return new DataFrame(updatedValues, Index.ToList(), newColumnNames, colTypes);
		}


		/// <summary>
		/// Appends another DataFrame either vertically (row-wise) or horizontally (column-wise).
		/// </summary>
		/// <param name="df">The DataFrame to append.</param>
		/// <param name="verticaly">A boolean indicating whether to append vertically. If false, appends horizontally.</param>
		/// <returns>A new DataFrame containing the combined data.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown when the input DataFrame is incompatible with the current DataFrame for appending.
		/// </exception>
		/// <example>
		/// Example usage:
		/// var df1 = new DataFrame(
		///     new List<object> { 1, "A", 2, "B" },
		///     new List<object> { 0, 1 },
		///     new List<string> { "Col1", "Col2" },
		///     null);
		///
		/// var df2 = new DataFrame(
		///     new List<object> { 3, "C", 4, "D" },
		///     new List<object> { 2, 3 },
		///     new List<string> { "Col1", "Col2" },
		///     null);
		///
		/// var appendedDf = df1.Append(df2, true);
		/// // Result: Combined rows from df1 and df2.
		/// </example>
		public DataFrame Append(DataFrame df, bool verticaly = true)
		{
			if (df == null)
				throw new ArgumentException("The input DataFrame cannot be null.");

			if (verticaly) // Append rows
			{
				// Validate column compatibility for vertical append
				if (Columns.Count != df.Columns.Count)
					throw new ArgumentException("Data frames are not compatible for appending. Column counts must match.");

				// Combine values and index
				var valuesToAppend = new List<object?>(Values);
				valuesToAppend.AddRange(df.Values);

				var updatedIndex = new List<object>(Index);
				updatedIndex.AddRange(df.Index);

				// Use existing column names and types
				var columns = Columns.ToList();
				var columnTypes = this._colTypes != null && this._colTypes != Array.Empty<ColType>()
					? this._colTypes.ToArray()
					: this._colTypes;

				// Create and return the new DataFrame
				return new DataFrame(valuesToAppend, updatedIndex, columns, columnTypes!);
			}
			else // Append columns
			{
				// Validate row compatibility for horizontal append
				if (Index.Count != df.Index.Count)
					throw new ArgumentException("Data frames are not compatible for appending. Row counts must match.");

				// Prepare dictionary for new columns
				var newColumns = new Dictionary<string, List<object?>>();
				for (int i = 0; i < df.Columns.Count; i++)
				{
					newColumns.Add(df.Columns[i], df[df.Columns[i]].ToList());
				}

				// Add columns to the current DataFrame
				return this.AddColumns(newColumns);
			}
		}


		/// <summary>
		/// Adds a new row to the DataFrame.
		/// </summary>
		/// <param name="row">
		/// A list of values representing the row to be added.
		/// Must match the number of columns in the DataFrame.
		/// </param>
		/// <param name="index">
		/// The optional index value for the row.
		/// If not provided, a default index will be generated.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when the row is null or its length does not match the number of columns.
		/// </exception>
		/// <example>
		/// Example usage:
		/// var dataFrame = new DataFrame(
		///     new List<object> { 1, "A", 2 },
		///     new List<object> { 0, 1 },
		///     new List<string> { "Col1", "Col2", "Col3" },
		///     null);
		///
		/// dataFrame.AddRow(new List<object> { 3, "B", 4 });
		/// </example>
		public void AddRow(List<object?> row, object? index = null)
		{
			// Validate that the row is not null and matches the column count.
			if (row == null)
				throw new ArgumentException("The row cannot be null.");
			if (row.Count != Columns.Count)
				throw new ArgumentException($"Inconsistent row length. Expected {Columns.Count}, but got {row.Count}.");

			// Insert the row into the DataFrame at the end (-1 indicates append).
			InsertRow(-1, row, index);
		}

		#endregion

		#region Calculated Column
		/// <summary>
		/// Adds a calculated column to the DataFrame.
		/// The values of the new column are calculated using a provided callback function.
		/// </summary>
		/// <param name="colName">The name of the new column.</param>
		/// <param name="callBack">
		/// A callback function to calculate values for each row.
		/// The function takes a dictionary of existing column values (key-value pairs) and the row index,
		/// and returns the value for the new column.
		/// </param>
		/// <returns>True if the column was successfully added.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown when the column name is null, empty, or duplicate.
		/// </exception>
		/// <example>
		/// Example usage:
		/// var df = new DataFrame(
		///     new List<object> { 1, "A", 2, "B" },
		///     new List<object> { 0, 1 },
		///     new List<string> { "Col1", "Col2" },
		///     null);
		///
		/// df.AddCalculatedColumn("NewCol", (row, index) => row["Col1"].ToString() + row["Col2"].ToString());
		/// // Result: DataFrame now contains "NewCol" with values "1A" and "2B".
		/// </example>

		public bool AddCalculatedColumn(string colName, Func<IDictionary<string, object?>, int, object> callBack)
		{
			if (string.IsNullOrWhiteSpace(colName))
				throw new ArgumentException("Column name cannot be null or empty.");

            if (callBack == null)
                throw new ArgumentNullException(nameof(callBack));

			// Wrap single-column callback into multi-column logic
			var colNames = new string[] { colName };
			return AddCalculatedColumns(colNames, (row, index) => new[] { callBack(row, index) });
		}

		/// <summary>
		/// Adds a calculated column to the DataFrame using an array of existing row values.
		/// </summary>
		/// <param name="colName">The name of the new column.</param>
		/// <param name="callBack">
		/// A callback function to calculate values for each row.
		/// The function takes an array of existing column values and the row index,
		/// and returns the value for the new column.
		/// </param>
		/// <returns>True if the column was successfully added.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown when the column name is null, empty, or duplicate.
		/// </exception>
		/// <example>
		/// Example usage:
		/// var df = new DataFrame(
		///     new List<object> { 1, "A", 2, "B" },
		///     new List<object> { 0, 1 },
		///     new List<string> { "Col1", "Col2" },
		///     null);
		///
		/// df.AddCalculatedColumn("NewCol", (row, index) => row[0].ToString() + row[1].ToString());
		/// // Result: DataFrame now contains "NewCol" with values "1A" and "2B".
		/// </example>

		public bool AddCalculatedColumn(string colName, Func<object[], int, object> callBack)
		{
			if (string.IsNullOrWhiteSpace(colName))
				throw new ArgumentException("Column name cannot be null or empty.");

			if (callBack == null)
				throw new ArgumentNullException(nameof(callBack));

			// Wrap single-column callback into multi-column logic
			var colNames = new string[] { colName };
			return AddCalculatedColumns(colNames, (row, index) => new[] { callBack(row, index) });
		}

		/// <summary>
		/// Adds calculated columns to the DataFrame.
		/// The values of the columns are calculated for each row using a callback function.
		/// </summary>
		/// <param name="colNames">An array of names for the new columns.</param>
		/// <param name="callBack">
		/// A callback function to calculate values for each row.
		/// The function takes an existing row as a dictionary of column name-value pairs and the row index,
		/// and returns an array of values for the new columns.
		/// </param>
		/// <returns>True if the columns were successfully added.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown when column names are null, empty, or duplicate existing columns.
		/// </exception>
		/// <example>
		/// Example usage:
		/// var df = new DataFrame(
		///     new List<object> { 1, "A", 2, "B" },
		///     new List<object> { 0, 1 },
		///     new List<string> { "Col1", "Col2" },
		///     null);
		///
		/// df.AddCalculatedColumns(
		///     new[] { "Col3", "Col4" },
		///     (row, index) => new object[] { (int)row["Col1"] * 2, row["Col2"].ToString().ToLower() });
		/// // Result: DataFrame now contains "Col3" with values [2, 4] and "Col4" with values ["a", "b"].
		/// </example>
		/// <summary>
		/// Adds calculated columns to the DataFrame.
		/// The values of the new columns are calculated for each row using a callback function.
		/// </summary>
		/// <param name="colNames">An array of names for the new columns.</param>
		/// <param name="callBack">
		/// A callback function to calculate values for each row.
		/// The function takes an existing row as a dictionary of column name-value pairs and the row index,
		/// and returns an array of values for the new columns.
		/// </param>
		/// <returns>True if the columns were successfully added.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown when column names are null, empty, or duplicate existing columns.
		/// </exception>
		public bool AddCalculatedColumns(string[] colNames, Func<IDictionary<string, object?>, int, object[]> callBack)
		{
			if (colNames == null || colNames.Length == 0)
				throw new ArgumentException("Column names must not be null or empty.");

			if (callBack == null)
				throw new ArgumentNullException(nameof(callBack));

			// Check for duplicate column names
			checkColumnNames(this.Columns, colNames);

			// Initialize new values list
			var updatedValues = new List<object?>();

			// Create a template for processing each row
			var processingRow = new Dictionary<string, object?>();
			foreach (var column in this.Columns)
				processingRow[column] = null!;

			// Iterate through each row and calculate values for new columns
			for (int i = 0; i < Index.Count; i++)
			{
				int rowOffset = i * this.Columns.Count;

				// Populate existing row data
				for (int j = 0; j < this.Columns.Count; j++)
				{
					var value = Values[rowOffset + j];
					processingRow[this.Columns[j]] = value;
					updatedValues.Add(value); // Add existing values to updated list
				}

				// Calculate new column values for the current row
				var calculatedRow = callBack(processingRow, i);
				if (calculatedRow.Length != colNames.Length)
					throw new ArgumentException("Number of calculated values does not match the number of column names.");

				// Add calculated values for the current row
				updatedValues.AddRange(calculatedRow);
			}

			// Add new columns to the DataFrame
			this.Columns.AddRange(colNames);
			this._values = updatedValues;
            this._colTypes = columnsTypes(_values, Index.Count, Columns.Count);
			return true;
		}


		/// <summary>
		/// Adds calculated columns to the DataFrame using an array of existing row values.
		/// </summary>
		/// <param name="colNames">An array of names for the new columns.</param>
		/// <param name="callBack">
		/// A callback function to calculate values for each row.
		/// The function takes an existing row (as an array of column values) and the row index,
		/// and returns an array of calculated values for the new columns.
		/// </param>
		/// <returns>True if the columns were successfully added.</returns>
		/// <example>
		/// Example usage:
		/// var df = new DataFrame(
		///     new List<object> { 1, "A", 2, "B" },
		///     new List<object> { 0, 1 },
		///     new List<string> { "Col1", "Col2" },
		///     null);
		///
		/// df.AddCalculatedColumns(
		///     new[] { "Col3", "Col4" },
		///     (row, index) => new object[] { (int)row[0] * 2, row[1].ToString().ToLower() });
		/// // Result:
		/// // Col1 Col2 Col3 Col4
		/// //  1    A    2    a
		/// //  2    B    4    b
		/// </example>
		/// <exception cref="ArgumentException">
		/// Thrown when column names are null, empty, or duplicate existing columns.
		/// </exception>
		public bool AddCalculatedColumns(string[] colNames, Func<object[], int, object[]> callBack)
		{
			if (colNames == null || colNames.Length == 0)
				throw new ArgumentException("Column names must not be null or empty.");

			if (callBack == null)
				throw new ArgumentNullException(nameof(callBack));

			// Check for duplicate column names
			checkColumnNames(this.Columns, colNames);

			// Create a new list to store updated values
			var updatedValues = new List<object?>();

			// Create a template for processing each row
			var processingRow = new object[this.Columns.Count];

			// Iterate through each row and calculate values for new columns
			for (int i = 0; i < Index.Count; i++)
			{
				int rowOffset = i * this.Columns.Count;

				// Populate existing row values
				for (int j = 0; j < this.Columns.Count; j++)
				{
					var value = Values[rowOffset + j];
					processingRow[j] = value!;
					updatedValues.Add(value); // Add existing value to the updated list
				}

				// Calculate new column values for the current row
				var calculatedRow = callBack(processingRow, i);
				if (calculatedRow.Length != colNames.Length)
					throw new ArgumentException("Number of calculated values does not match the number of column names.");

				// Add the calculated values for the new columns
				updatedValues.AddRange(calculatedRow);
			}

			// Add new columns to the DataFrame
			this.Columns.AddRange(colNames);
			this._values = updatedValues;


			//reset existing column types and regenerate new 
			this._colTypes = columnsTypes(updatedValues, RowCount(), ColCount());


			return true;
		}

		#endregion

		#region Aggregation
		/// <summary>
		/// Aggregates values across specified columns, or optionally across all columns.
		/// </summary>
		/// <param name="aggs">Dictionary specifying columns and their aggregation type.</param>
		/// <param name="allColumns">If true, aggregates all columns not specified in <paramref name="aggs"/>.</param>
		/// <returns>List of aggregated values for the DataFrame.</returns>
		/// <example>
		/// Example usage:
		/// var df = new DataFrame(
		///     new List<object> { 1, 2, 3 },
		///     new List<object> { 0, 1, 2 },
		///     new List<string> { "Col1", "Col2" },
		///     null);
		///
		/// var aggs = new Dictionary<string, Aggregation>
		/// {
		///     { "Col1", Aggregation.Sum },
		///     { "Col2", Aggregation.Average }
		/// };
		///
		/// var result = df.Aggragate(aggs, true);
		/// // Output: [Sum of Col1, Average of Col2, Last value of Col3]
		/// </example>
		/// <exception cref="ArgumentException">Thrown if <paramref name="aggs"/> is null.</exception>
		public List<object?> Aggragate(IDictionary<string, Aggregation> aggs, bool allColumns = false)
		{
			if (aggs == null)
				throw new ArgumentException("The list of columns or aggregation types cannot be null.");

            // Initialize column types if necessary
            EnsureColumnTypesInitialized();

			var aggregatedValues = new List<object?>();

			for (int i = 0; i < Columns.Count; i++)
			{
				var columnName = Columns[i];
				if (aggs.ContainsKey(columnName))
				{
					var aggregation = aggs[columnName];
					var value = calculateAggregation(this[columnName]!, aggregation, _colTypes![i]);
					aggregatedValues.Add(value);
				}
				else if (allColumns)
				{
					aggregatedValues.Add(this[columnName].Last());
				}
			}

			return aggregatedValues;
		}

		/// <summary>
		/// Performs multiple aggregation operations on specified columns.
		/// Each column can have multiple aggregation operations.
		/// </summary>
		/// <param name="aggs">Dictionary specifying columns and their aggregation types.</param>
		/// <returns>DataFrame containing the aggregated values.</returns>
		/// /// <example>
		/// Example usage:
		/// var df = new DataFrame(
		///     new List<object> { 1, 2, 3 },
		///     new List<object> { 0, 1, 2 },
		///     new List<string> { "Col1", "Col2" },
		///     null);
		///
		/// var aggs = new Dictionary<string, Aggregation[]>
		/// {
		///     { "Col1", new[] { Aggregation.Sum, Aggregation.Count } },
		///     { "Col2", new[] { Aggregation.Average } }
		/// };
		///
		/// var resultDf = df.Aggragate(aggs);
		/// // Output:
		/// // Col1 - Sum: 6, Count: 3
		/// // Col2 - Average: 2
		/// </example>
		/// <exception cref="ArgumentException">Thrown if <paramref name="aggs"/> is null.</exception>
		public DataFrame Aggragate(IDictionary<string, Aggregation[]> aggs)
		{
			if (aggs == null)
				throw new ArgumentException("The list of columns or aggregation types cannot be null.");

			// Initialize column types if necessary
			EnsureColumnTypesInitialized();

			var aggregatedValues = new TwoKeysDictionary<string, object, object>();

			foreach (var columnName in Columns)
			{
				if (aggs.ContainsKey(columnName))
				{
					foreach (var aggregation in aggs[columnName])
					{
						var value = calculateAggregation(this[columnName], aggregation, _colTypes![Columns.IndexOf(columnName)]);
						aggregatedValues.Add(columnName, aggregation.GetEnumDescription(), value!);
					}
				}
			}

			return new DataFrame(aggregatedValues);
		}
		#endregion

		#region Clip
		/// <summary>
		/// Clips the values in the entire DataFrame to a specified range.
		/// Values below <paramref name="minValue"/> are replaced with <paramref name="minValue"/>,
		/// and values above <paramref name="maxValue"/> are replaced with <paramref name="maxValue"/>.
		/// Non-numeric values and NaN are left unchanged.
		/// </summary>
		/// <param name="minValue">The minimum value of the range.</param>
		/// <param name="maxValue">The maximum value of the range.</param>
		/// <returns>A new DataFrame with clipped values.</returns>
		/// <example>
		/// // Example usage:
		/// DataFrame df = new DataFrame(new List<object> { -10, 50, 200, "text", DataFrame.NAN }, 
		///                              new List<string> { "row1", "row2", "row3" }, 
		///                              new List<string> { "col1" }, 
		///                              new ColType[] { ColType.I32 });
		/// 
		/// DataFrame clippedDf = df.Clip(0, 100);
		/// // The clippedDf will contain values: { 0, 50, 100, "text", NAN }
		public DataFrame Clip(float minValue, float maxValue)
		{
			EnsureColumnTypesInitialized();

			var clippedValues = Values
				.Select((value, index) => ClipValue(value, ColTypes[index % Columns.Count], minValue, maxValue))
				.ToList();

			// Return a new DataFrame
			return CreateNewDataFrame(clippedValues);
		}

		/// <summary>
		/// Clips the values in the specified columns of the DataFrame to a given range.
		/// Values in specified columns below <paramref name="minValue"/> are replaced with <paramref name="minValue"/>,
		/// and values above <paramref name="maxValue"/> are replaced with <paramref name="maxValue"/>.
		/// Columns not specified, non-numeric values, and NaN are left unchanged.
		/// </summary>
		/// <param name="minValue">The minimum value of the range.</param>
		/// <param name="maxValue">The maximum value of the range.</param>
		/// <param name="columns">The names of the columns to clip. Other columns are not modified.</param>
		/// <returns>A new DataFrame with clipped values in the specified columns.</returns>
		/// <example>
		/// // Example usage:
		/// DataFrame df = new DataFrame(
		///     new List<object> { 50, 150, "text", -20, DataFrame.NAN },
		///     new List<string> { "row1", "row2" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.I32 });
		///
		/// DataFrame clippedDf = df.Clip(0, 100, "col1");
		/// // The clippedDf will contain:
		/// // col1: { 50, 100 }
		/// // col2: { -20, NAN }
		/// </example>
		public DataFrame Clip(float minValue, float maxValue, params string[] columns)
		{
			EnsureColumnTypesInitialized();
			var colIndices = getColumnIndex(columns);

			var clippedValues = Values
				.Select((value, index) =>
				{
					int colIndex = index % Columns.Count;
					return colIndices.Contains(colIndex)
						? ClipValue(value, ColTypes[colIndex], minValue, maxValue)
						: value;
				})
				.ToList();

			// Return a new DataFrame
			return CreateNewDataFrame(clippedValues);
		}

		/// <summary>
		/// Clips a single value to the specified range based on its column type.
		/// Non-numeric values and NaN are returned unchanged.
		/// </summary>
		/// <param name="value">The value to be clipped.</param>
		/// <param name="colType">The column type of the value.</param>
		/// <param name="minValue">The minimum value of the range (float).</param>
		/// <param name="maxValue">The maximum value of the range (float).</param>
		/// <returns>The clipped value, or the original value if not applicable.</returns>
		private object? ClipValue(object? value, ColType colType, float minValue, float maxValue)
		{
			// Convert minValue and maxValue to double for type compatibility
			double minDouble = Convert.ToDouble(minValue);
			double maxDouble = Convert.ToDouble(maxValue);

			if (value == DataFrame.NAN || colType == ColType.STR || colType == ColType.IN || colType == ColType.DT)
				return value;

			return colType switch
			{
				ColType.I32 => ClipToRange(Convert.ToInt32(value), minDouble, maxDouble),
				ColType.I64 => ClipToRange(Convert.ToInt64(value), minDouble, maxDouble),
				ColType.F32 => ClipToRange(Convert.ToSingle(value), minDouble, maxDouble),
				ColType.DD => ClipToRange(Convert.ToDouble(value), minDouble, maxDouble),
				_ => value
			};
		}

		/// <summary>
		/// Clips a numeric value to a specified range.
		/// </summary>
		/// <typeparam name="T">The type of the numeric value (e.g., int, float, double).</typeparam>
		/// <param name="value">The numeric value to be clipped.</param>
		/// <param name="minValue">The minimum value of the range (double).</param>
		/// <param name="maxValue">The maximum value of the range (double).</param>
		/// <returns>The clipped value.</returns>
		private T ClipToRange<T>(T value, double minValue, double maxValue) where T : IComparable
		{
			if (value.CompareTo((T)Convert.ChangeType(minValue, typeof(T))) < 0)
				return (T)Convert.ChangeType(minValue, typeof(T));
			if (value.CompareTo((T)Convert.ChangeType(maxValue, typeof(T))) > 0)
				return (T)Convert.ChangeType(maxValue, typeof(T));
			return value;
		}

        #endregion

        #region Describe
        
        /// <summary>
        /// Provides a summary of descriptive statistics for the columns in the DataFrame.
        /// By default, only numeric columns are included, but this behavior can be overridden
        /// with the <paramref name="numericOnly"/> parameter.
        /// Specific columns can be included using the <paramref name="inclColumns"/> parameter.
        /// </summary>
        /// <param name="numericOnly">If true, includes only numeric columns. Defaults to true.</param>
        /// <param name="inclColumns">An optional list of column names to include in the statistics.</param>
        /// <returns>
        /// A new DataFrame containing descriptive statistics, such as Count, Unique, Top, Frequency, 
        /// Avg (Mean), Std (Standard Deviation), Min, Quartiles, Median, and Max.
        /// </returns>
        /// <example>
        /// // Example 1: Describe only numeric columns in the DataFrame.
        /// DataFrame df = new DataFrame(
        ///     new List<object> { -10, 50, 200, "text", DataFrame.NAN },
        ///     new List<object> { "row1", "row2", "row3", "row4", "row5" },
        ///     new List<string> { "col1", "col2" },
        ///     new ColType[] { ColType.I32, ColType.STR });
        /// 
        /// DataFrame description = df.Describe(); // Defaults to numericOnly = true
        /// 
        /// // Example 2: Include all columns, numeric and non-numeric.
        /// DataFrame descriptionAll = df.Describe(numericOnly: false);
        /// 
        /// // Example 3: Include specific columns for description.
        /// DataFrame descriptionSpecific = df.Describe(numericOnly: false, "col1", "col2");
        /// </example>
        public DataFrame Describe(bool numericOnly = true, params string[] inclColumns)
		{
			// Initialize aggregation operations
			var aggOps = new Aggregation[]
			{
		        Aggregation.Count, Aggregation.Unique, Aggregation.Top, Aggregation.Frequency, Aggregation.Avg,
		        Aggregation.Std, Aggregation.Min, Aggregation.FirstQuartile, Aggregation.Median,
		        Aggregation.ThirdQuartile, Aggregation.Max
			};

			// Ensure column types are initialized
			EnsureColumnTypesInitialized();

			// Get relevant columns (filtered by inclColumns and numericOnly)
			var relevantColumns = GetRelevantColumns(inclColumns, numericOnly);

			// Prepare final column aggregation mapping
			var finalCols = relevantColumns.ToDictionary(col => col.cName, col => aggOps);

			// Perform aggregation
			return Aggragate(finalCols);
		}

        /// <summary>
        /// Filters and retrieves relevant columns based on input criteria.
        /// </summary>
        /// <param name="inclColumns">Columns to include (optional).</param>
        /// <param name="numericOnly">Whether to include only numeric columns.</param>
        /// <returns>A list of column name and type tuples.</returns>
        private List<(string cName, ColType cType)> GetRelevantColumns(string[] inclColumns, bool numericOnly)
		{
			var idxs = inclColumns.Length > 0 ? getColumnIndex(inclColumns) : Enumerable.Range(0, Columns.Count).ToArray();

			return idxs
				.Select(i => (Columns[i], ColTypes[i]))
				.Where(col => !numericOnly || IsNumeric(col.Item2))
				.ToList();
		}
		
        #endregion

		#region Missing Values 

		/// <summary>
		/// Counts the number of missing values (DataFrame.NAN) in each column of the DataFrame.
		/// Only includes columns with at least one missing value in the result.
		/// </summary>
		/// <returns>
		/// A dictionary where the keys are column names and the values are the count of missing values in each column.
		/// </returns>
		/// <example>
		/// // Example usage:
		/// var missingValues = df.MissingValues();
		/// // Result: { "col1": 3, "col3": 1 } (only columns with missing values are included)
		/// </example>
		public IDictionary<string, int> MissingValues()
		{
			return Columns
				.Select((col, index) =>
					(col, missingCount: this[col].Count(value => value == DataFrame.NAN)))
				.Where(columnData => columnData.missingCount > 0) // Filter columns with missing values
				.ToDictionary(columnData => columnData.col, columnData => columnData.missingCount);
		}

		/// <summary>
		/// Drops specified columns from the DataFrame.
		/// </summary>
		/// <param name="colName">The names of the columns to drop.</param>
		/// <returns>A new DataFrame without the specified columns.</returns>
		/// <example>
		/// // Example usage:
		/// DataFrame df = new DataFrame(
		///     new List<object> { 1, 2, 3, 4 },
		///     new List<string> { "row1", "row2", "row3", "row4" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.I32 });
		///
		/// DataFrame result = df.Drop("col1");
		/// // Result will exclude "col1", keeping only "col2".
		public DataFrame Drop(params string[] colName)
		{
			// Validate input
			if (colName == null || colName.Length == 0)
				throw new ArgumentException("No column names provided.", nameof(colName));
            if(!Columns.Any(c=> colName.Contains(c)))
				throw new ArgumentException("No column to drop.", nameof(colName));

			// Filter out columns to drop
			var remainingColumns = Columns
				.Where(c => !colName.Contains(c))
				.Select(c => (c, (string)null!))
				.ToArray();

			// Create and return a new DataFrame
			return Create(remainingColumns);
		}

		/// <summary>
		/// Removes rows containing missing values (DataFrame.NAN).
		/// Optionally checks for missing values in specified columns only.
		/// </summary>
		/// <param name="cols">The columns to check for missing values. If omitted, all columns are checked.</param>
		/// <returns>A new DataFrame without rows containing missing values.</returns>
		/// <example>
		/// // Example 1: Drop rows with missing values in any column.
		/// DataFrame df = new DataFrame(
		///     new List<object> { 1, DataFrame.NAN, 3, 4 },
		///     new List<string> { "row1", "row2", "row3", "row4" },
		///     new List<string> { "col1" },
		///     new ColType[] { ColType.I32 });
		///
		/// DataFrame result = df.DropNA();
		/// // Rows with missing values in "col1" will be removed.
		///
		/// // Example 2: Drop rows with missing values in specific columns.
		/// DataFrame resultSpecific = df.DropNA("col1");
		/// // Rows with missing values in "col1" will be removed.
		/// </example>
		public DataFrame DropNA(params string[] cols)
		{
			var colIndexes = cols.Length > 0 ? getColumnIndex(cols) : null;

			return RemoveRows((row, rowIndex) =>
			{
				if (colIndexes == null) // If no specific columns are provided
					return row.Any(value => value == DataFrame.NAN); // Check for any missing value in the row

				// Check for missing values only in specified columns
				return row.Select((value, colIndex) => new { value, colIndex })
						  .Any(x => x.value == DataFrame.NAN && colIndexes.Contains(x.colIndex));
			});
		}


		/// <summary>
		/// Fills all missing values (DataFrame.NAN) in the DataFrame with a specified value.
		/// </summary>
		/// <param name="value">The value to replace missing values with.</param>
		/// <example>
		/// // Example usage:
		/// DataFrame df = new DataFrame(
		///     new List<object> { DataFrame.NAN, 2, DataFrame.NAN, 4 },
		///     new List<string> { "row1", "row2", "row3", "row4" },
		///     new List<string> { "col1" },
		///     new ColType[] { ColType.I32 });
		///
		/// df.FillNA(0);
		/// // All missing values in "col1" will be replaced with 0.
		/// </example>
		public void FillNA(object value)
		{
			for (int i = 0; i < Values.Count; i++)
			{
				if (Values[i] == DataFrame.NAN)
					Values[i] = value;
			}
		}

		/// <summary>
		/// Fills missing values (DataFrame.NAN) in a specific column with a specified value.
		/// </summary>
		/// <param name="col">The column to modify.</param>
		/// <param name="replacedValue">The value to replace missing values with.</param>
		/// <example>
		/// // Example usage:
		/// DataFrame df = new DataFrame(
		///     new List<object> { DataFrame.NAN, 2, DataFrame.NAN, 4 },
		///     new List<string> { "row1", "row2", "row3", "row4" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.I32 });
		///
		/// df.FillNAMultiple("col1", 0);
		/// // All missing values in "col1" will be replaced with 0.
		/// </example>
		public void FillNAByValue(string col, object replacedValue)
		{
            if(replacedValue == null)
                throw new ArgumentNullException(nameof(replacedValue));

			var colIndex = getColumnIndex(col);

			for (int i = 0; i < Index.Count; i++)
			{
				int valueIndex = i * Columns.Count + colIndex;
				if (Values[valueIndex] == DataFrame.NAN)
                {
					// Ensure the aggregated value is converted to the appropriate column type
					var convertedValue = ConvertValueToColumnType(replacedValue, this.ColTypes[colIndex]);
					if (convertedValue == null)
						throw new ArgumentNullException(nameof(convertedValue));

					Values[valueIndex] = convertedValue;
                }
			}
		}

		/// <summary>
		/// Fills missing values (DataFrame.NAN) in a specific column with an aggregated value.
		/// </summary>
		/// <param name="col">The column to modify.</param>
		/// <param name="aggValue">The aggregation method to calculate the replacement value.</param>
		/// <example>
		/// // Example usage:
		/// DataFrame df = new DataFrame(
		///     new List<object> { DataFrame.NAN, 2, 4, 6 },
		///     new List<string> { "row1", "row2", "row3", "row4" },
		///     new List<string> { "col1" },
		///     new ColType[] { ColType.I32 });
		///
		/// df.FillNA("col1", Aggregation.Avg);
		/// // Missing values in "col1" will be replaced with the average of non-missing values.
		/// </example>
		public void FillNA(string col, Aggregation aggValue)
		{

			EnsureColumnTypesInitialized();

			var colIndex = getColumnIndex(col);

			// Get non-missing values from the column
			var nonMissingValues = this[col].Where(x => x != DataFrame.NAN);

			// Calculate the aggregated value
			var aggregatedValue = calculateAggregation(nonMissingValues, aggValue, this.ColTypes[colIndex]);

			// Ensure the aggregated value is converted to the appropriate column type
			var convertedValue = ConvertValueToColumnType(aggregatedValue, this.ColTypes[colIndex]);

			if (convertedValue == null)
				throw new ArgumentNullException(nameof(convertedValue));

			// Fill missing values with the converted value
			FillNAByValue(col, convertedValue);
		}

		/// <summary>
		/// Replaces missing values in the specified column using a delegate function.
		/// </summary>
		/// <param name="col">The name of the column where missing values should be replaced.</param>
		/// <param name="replDelg">
		/// A delegate function that takes the row index as an argument and returns the replacement value.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown if the column name is null or empty.
		/// </exception>
		/// <remarks>
		/// This method iterates through the column values and replaces missing entries (NaN) using the provided delegate.
		/// The delegate allows dynamic replacement logic based on row index.
		/// </remarks>
		/// <example>
		/// Example usage:
		/// <code>
		/// df.FillNA("Price", rowIdx => rowIdx % 2 == 0 ? 0.0 : 1.0);
		/// </code>
		/// This replaces missing values in the "Price" column with alternating values based on row index.
		/// </example>
		public void FillNA(string col, Func<int, object> replDelg)
		{
			if (string.IsNullOrEmpty(col))
				throw new ArgumentException(nameof(col));

			var colIndex = getColumnIndex(col);
			int index = 0;
			for (int i = 0; i < Index.Count; i++)
			{
				for (int j = 0; j < Columns.Count; j++)
				{
					if (j == colIndex)
					{
						if (Values[index] == DataFrame.NAN)
							Values[index] = replDelg(i);

					}
					index++;
				}
			}
		}

		/// <summary>
		/// Converts a value to the appropriate type for a specific column.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="colType">The data type of the column.</param>
		/// <returns>The value converted to the column's type.</returns>
		private object? ConvertValueToColumnType(object? value, ColType colType)
		{
            if(value == null)
                return DataFrame.NAN;

			return colType switch
			{
				ColType.I32 => Convert.ToInt32(value),
				ColType.I64 => Convert.ToInt64(value),
				ColType.F32 => Convert.ToSingle(value),
				ColType.DD => Convert.ToDouble(value),
				ColType.I2 => value.ToString(),
				ColType.IN => value.ToString(),
				ColType.DT => Convert.ToDateTime(value),
				ColType.STR => value.ToString(),
				_ => throw new InvalidOperationException($"Unsupported column type: {colType}")
			};
		}

		/// <summary>
		/// Fills missing values (DataFrame.NAN) in multiple specified columns with a given replacement value.
		/// </summary>
		/// <param name="cols">An array of column names to modify.</param>
		/// <param name="replacedValue">The value to replace missing values with.</param>
		/// <remarks>
		/// This method iterates through all specified columns and applies the replacement value to any missing values.
		/// If a column does not exist in the DataFrame, an exception will be thrown.
		/// </remarks>
		/// <example>
		/// // Example usage:
		/// DataFrame df = new DataFrame(
		///     new List<object> { DataFrame.NAN, 2, DataFrame.NAN, 4 },
		///     new List<object> { "row1", "row2", "row3", "row4" },
		///     new List<string> { "col1", "col2", "col3" },
		///     new ColType[] { ColType.I32, ColType.I32, ColType.I32 });
		///
		/// df.FillNA(new string[] { "col1", "col2" }, 0);
		/// // Missing values in "col1" and "col2" will be replaced with 0.
		/// </example>
		public void FillNA(string[] cols, object replacedValue)
		{
			var colIndexes = getColumnIndex(cols);

			foreach (var colIndex in colIndexes)
			{
				FillNAByValue(Columns[colIndex], replacedValue);
			}
		}


		#endregion

		#region Filter
		/// <summary>
		/// Filters rows in the DataFrame based on multiple column conditions.
		/// </summary>
		/// <param name="cols">An array of column names to filter.</param>
		/// <param name="filterValues">An array of values to filter against.</param>
		/// <param name="fOpers">An array of operators specifying how to filter values.</param>
		/// <returns>A new DataFrame containing rows that satisfy the conditions.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown when any of the input arrays (columns, filter values, or operators) is null, empty, or of inconsistent length.
		/// </exception>
		/// <example>
		/// // Example DataFrame initialization
		/// DataFrame df = new DataFrame(
		///     new List<object> { 1, "A", 3, "B", DataFrame.NAN, "C", 2, "D" },
		///     new List<object> { "row1", "row2", "row3", "row4" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.STR });
		///
		/// // Filter rows where col1 == 1 and col2 == "A"
		/// DataFrame filteredDf = df.Filter(
		///     new[] { "col1", "col2" },
		///     new object[] { 1, "A" },
		///     new[] { FilterOperator.Equal, FilterOperator.Equal });
		///
		/// // Result: Rows matching the conditions
		/// // Index: ["row1"]
		/// // Values: [1, "A"]
		/// </example>
		public DataFrame Filter(string[] cols, object[] filterValues, FilterOperator[] fOpers)
		{
			// Validate input arguments
			ValidateFilterArguments(cols, filterValues, fOpers);

			if (Index.Count == 0)
				return new DataFrame(Array.Empty<object>(), Columns.ToList());

			// Get column indices
			int[] columnIndices = getColumnIndex(cols);

			// Filter rows based on conditions
			var filteredRows = Enumerable.Range(0, Index.Count)
				.Where(rowIndex => FilterRow(rowIndex, columnIndices, filterValues, fOpers))
				.ToList();

			// Construct filtered DataFrame
			return CreateFilteredDataFrame(filteredRows);
		}

		/// <summary>
		/// Validates arguments for the Filter method.
		/// </summary>
		private void ValidateFilterArguments(string[] cols, object[] filterValues, FilterOperator[] fOpers)
		{
			if (cols == null || cols.Length == 0)
				throw new ArgumentException($"'{nameof(cols)}' cannot be null or an empty array.");
			if (filterValues == null || filterValues.Length == 0)
				throw new ArgumentException($"'{nameof(filterValues)}' cannot be null or an empty array.");
			if (fOpers == null || fOpers.Length == 0)
				throw new ArgumentException($"'{nameof(fOpers)}' cannot be null or an empty array.");
			if (cols.Length != filterValues.Length || cols.Length != fOpers.Length)
				throw new ArgumentException("Inconsistent number of columns, filter values, and operators.");
		}

		/// <summary>
		/// Determines if a row satisfies the filter conditions.
		/// </summary>
		private bool FilterRow(int rowIndex, int[] columnIndices, object[] filterValues, FilterOperator[] fOpers)
		{
			int baseIndex = rowIndex * Columns.Count;

			for (int i = 0; i < columnIndices.Length; i++)
			{
				int colIndex = columnIndices[i];
				object? value = Values[baseIndex + colIndex];

				// Skip rows with missing values
				if (value == DataFrame.NAN)
					return false;

				// Apply the filter operator
				if (!applyOperator(value!, filterValues[i], fOpers[i], ColTypes[colIndex]))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Constructs a new DataFrame using only the filtered rows.
		/// </summary>
		private DataFrame CreateFilteredDataFrame(List<int> filteredRows)
		{
			var filteredValues = filteredRows
				.SelectMany(rowIndex => Enumerable.Range(0, Columns.Count)
					.Select(colIndex => Values[rowIndex * Columns.Count + colIndex]))
				.ToList();

			var filteredIndex = filteredRows.Select(rowIndex => Index[rowIndex]).ToList();

			return new DataFrame(filteredValues, filteredIndex, Columns, _colTypes!);
		}

		/// <summary>
		/// Filters rows in the DataFrame based on a single column condition.
		/// </summary>
		/// <param name="col">The name of the column to filter.</param>
		/// <param name="value">The value to filter against.</param>
		/// <param name="fOper">The operator specifying how to filter the value.</param>
		/// <returns>A new DataFrame containing rows that satisfy the condition.</returns>
		/// <example>
		/// // Example DataFrame initialization
		/// DataFrame df = new DataFrame(
		///     new List<object> { 1, 2, 3, 4, 5 },
		///     new List<object> { "row1", "row2", "row3", "row4", "row5" },
		///     new List<string> { "col1" },
		///     new ColType[] { ColType.I32 });
		///
		/// // Filter rows where col1 > 2
		/// DataFrame result = df.Filter("col1", 2, FilterOperator.Greather);
		///
		/// // Result:
		/// // Index: ["row3", "row4", "row5"]
		/// // Values: [3, 4, 5]
		/// </example>
		public DataFrame Filter(string col, object value, FilterOperator fOper)
		{
			return Filter(new[] { col }, new[] { value }, new[] { fOper });
		}

		/// <summary>
		/// Filters rows in the DataFrame based on a user-defined condition.
		/// </summary>
		/// <param name="condition">A function that takes a dictionary representing a row and returns true if the row should be included.</param>
		/// <returns>A new DataFrame containing rows that satisfy the condition.</returns>
		/// <example>
		/// // Example DataFrame initialization
		/// DataFrame df = new DataFrame(
		///     new List<object> { 1, "A", 3, "B", 2, "C" },
		///     new List<object> { "row1", "row2", "row3" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.STR });
		///
		/// // Filter rows where col1 > 2 and col2 == "C"
		/// DataFrame filteredDf = df.Filter(row =>
		/// {
		///     return (int)row["col1"] > 2 && (string)row["col2"] == "C";
		/// });
		///
		/// // Result:
		/// // Index: ["row3"]
		/// // Values: [3, "C"]
		/// </example>
		public DataFrame Filter(Func<IDictionary<string, object>, bool> condition)
		{
			var filteredRows = Enumerable.Range(0, Index.Count)
				.Where(rowIndex =>
				{
					var rowDict = Columns.ToDictionary(
						col => col,
						col => Values[rowIndex * Columns.Count + getColumnIndex(col)]);
					return condition(rowDict!);
				})
				.ToList();

			return CreateFilteredDataFrame(filteredRows);
		}

		/// <summary>
		/// Applies a filter operation between two values based on their type.
		/// </summary>
		private bool applyOperator(object value, object filterValue, FilterOperator fOper, ColType colType)
		{
			return colType switch
			{
				ColType.I2 => Compare(Convert.ToBoolean(value), Convert.ToBoolean(filterValue), fOper),
				ColType.I32 or ColType.I64 or ColType.F32 or ColType.DD =>
					Compare(Convert.ToDouble(value), Convert.ToDouble(filterValue), fOper),
				ColType.STR or ColType.IN =>
					Compare(value.ToString()!, filterValue.ToString()!, fOper),
				ColType.DT =>
					Compare(Convert.ToDateTime(value), Convert.ToDateTime(filterValue), fOper),
				_ => throw new Exception("Unsupported column type!")
			};
		}

		/// <summary>
		/// Compares two values using a specified filter operator.
		/// </summary>
		private static bool Compare<T>(T val1, T val2, FilterOperator fOper) where T : IComparable
		{
			return fOper switch
			{
				FilterOperator.Equal => val1.CompareTo(val2) == 0,
				FilterOperator.Notequal => val1.CompareTo(val2) != 0,
				FilterOperator.Greather => val1.CompareTo(val2) > 0,
				FilterOperator.Less => val1.CompareTo(val2) < 0,
				FilterOperator.GreatherOrEqual => val1.CompareTo(val2) >= 0,
				FilterOperator.LessOrEqual => val1.CompareTo(val2) <= 0,
				FilterOperator.IsNUll => throw new Exception("Value cannot be null!"),
				FilterOperator.NonNull => val1 != null,
				_ => throw new Exception("Unknown operator!")
			};
		}

		#endregion

		#region Insert
		/// <summary>
		/// Inserts a column into the DataFrame at the specified position.
		/// If the position is -1, the column is appended to the end of the DataFrame.
		/// </summary>
		/// <param name="cName">The name of the column to insert.</param>
		/// <param name="value">The list of values representing the column to insert.</param>
		/// <param name="nPos">The position to insert the column. Use -1 to append the column to the end.</param>
		/// <returns>A new DataFrame with the column inserted.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown when the column name already exists or when the row counts do not match.
		/// </exception>
		/// <example>
		/// // Example: Append a column to the DataFrame
		/// DataFrame df = new DataFrame(
		///     new List<object> { 1, 2, 3, 4, 5, 6 },
		///     new List<object> { "row1", "row2" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.I32 });
		///
		/// DataFrame newDf = df.InsertColumn("col3", new List<object> { 7, 8 }, -1);
		///
		/// // Example: Insert a column at position 1
		/// DataFrame newDf = df.InsertColumn("colMiddle", new List<object> { 9, 10 }, 1);
		/// </example>
		public DataFrame InsertColumn(string cName, List<object> value, int nPos = -1)
		{
			// Validate inputs
			if (value == null)
				throw new ArgumentException("Value argument cannot be null.");
			if (RowCount() != value.Count)
				throw new ArgumentException("Row counts must be equal.");

			// Determine position for column insertion
			if (nPos == -1)
				nPos = this.Columns.Count;
			if (nPos < 0 || nPos > ColCount())
				throw new ArgumentException("Index position must be between 0 and ColCount.");

			// Ensure column name is unique
			checkColumnName(this.Columns, cName);

			// Create new values array with the column inserted
			var newValues = Enumerable.Range(0, Index.Count)
				.SelectMany(i => Enumerable.Range(0, Columns.Count + 1)
					.Select(j => j == nPos ? value[i] : Values[i * Columns.Count + (j < nPos ? j : j - 1)]))
				.ToList();

			// Create new column list with the column inserted
			var newColumns = this.Columns.ToList();
			if (nPos == this.Columns.Count)
				newColumns.Add(cName);
			else
				newColumns.Insert(nPos, cName);

			// Determine column types
			var newTypes = ColTypes.ToList();
			var colType = GetValueType(value.FirstOrDefault()!);
			if (nPos == newTypes.Count)
				newTypes.Add(colType);
			else
				newTypes.Insert(nPos, colType);

			// Return new DataFrame
			return new DataFrame(newValues, Index.ToList(), newColumns, newTypes.ToArray());
		}


		/// <summary>
		/// Inserts a row into the DataFrame at the specified position.
		/// If the position is -1, the row is appended to the end of the DataFrame.
		/// </summary>
		/// <param name="nPos">
		/// The position to insert the row. Use -1 to append the row to the end.
		/// </param>
		/// <param name="row">
		/// The list of values representing the row to insert.
		/// </param>
		/// <param name="index">
		/// The index of the row. If null, an auto-increment index is generated.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown if the row length does not match the number of columns.
		/// </exception>
		/// <example>
		/// // Example: Append a row to the DataFrame
		/// DataFrame df = new DataFrame(
		///     new List<object> { 1, 2, 3 },
		///     new List<object> { "row1" },
		///     new List<string> { "col1", "col2", "col3" },
		///     new ColType[] { ColType.I32, ColType.I32, ColType.I32 });
		///
		/// df.InsertRow(-1, new List<object> { 4, 5, 6 }, "row2");
		///
		/// // Example: Insert a row at position 0
		/// df.InsertRow(0, new List<object> { 0, 0, 0 }, "row0");
		/// </example>
		public void InsertRow(int nPos, List<object?> row, object? index = null)
		{
			// Validate the row size
			if (row.Count != this.ColCount())
				throw new ArgumentException("The row length must match the number of columns.");

			// Calculate index and add values
			if (nPos == -1)
			{
				// Append row
				_values?.AddRange(row);
				_index?.Add(index ?? _index.Count); // Add index (auto-increment if null)
			}
			else
			{
				// Insert row at the specified position
				int insertIndex = nPos * this.ColCount();
				_values?.InsertRange(insertIndex, row);
				_index?.Insert(nPos, index ?? this.RowCount()); // Add index
			}
		}

		#endregion

		#region Join and Merge

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
			EnsureColumnTypesInitialized();

			df2.EnsureColumnTypesInitialized();

            //get column indexes
            var leftInd = getColumnIndex(leftOn);

            //merge columns
            var tot = Columns.ToList();
            tot.AddRange(df2.Columns);

            //
            (var totalColumns, var totalTypes) = mergeColumns(this.Columns, _colTypes!, df2.Columns,df2._colTypes!, "rightDf");
            
            //create right lookup 
            var right = new List<ILookup<object, int>>();
            var ind = Enumerable.Range(0, df2.RowCount()).ToList();
            foreach (var l in rightOn)
            {
                var lo = df2[l].Zip(ind, (key, value) => (key, value)).ToLookup(x => x.key, x => x.value);
                right.Add(lo!);
            }

            var lst = new List<object?>();//new values
            var finIndex = new List<object>();//new index
            var leftRCount = RowCount();
            var leftCCount = ColCount();
            var rightRCount = df2.RowCount();
            var rightCCount = df2.ColCount();

            //left df enumeration          
            for (int i = 0; i < leftRCount; i++)
            {
                var leftKey = Index[i];
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
                            lst.Add(Values[r]);
                        //fill right table
                        int startR = j * rightCCount;
                        for (int r = startR; r < startR + rightCCount; r++)
                            lst.Add(df2.Values[r]);

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
                            lst.Add(Values[r]);

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
		/// Joins two DataFrames based on their indices.
		/// </summary>
		/// <param name="df2">The right DataFrame to join.</param>
		/// <param name="jType">The type of join (e.g., Left, Inner).</param>
		/// <returns>A new DataFrame resulting from the join operation.</returns>
		/// <exception cref="ArgumentException">Thrown if df2 is null.</exception>
		/// <example>
		/// // Example: Perform an inner join
		/// DataFrame df1 = new DataFrame(
		///     new List<object> { 1, 2, 3 },
		///     new List<object> { "row1", "row2", "row3" },
		///     new List<string> { "col1" },
		///     new ColType[] { ColType.I32 });
		///
		/// DataFrame df2 = new DataFrame(
		///     new List<object> { "A", "B", "C" },
		///     new List<object> { "row1", "row3", "row4" },
		///     new List<string> { "col2" },
		///     new ColType[] { ColType.STR });
		///
		/// DataFrame result = df1.Join(df2, JoinType.Inner);
		/// Result data:
		/// Index   | col1  | col2
		/// ----------------------
		/// row1    | 1     | "A"
		/// row3    | 3     | "B"
		/// </example>
		public DataFrame Join(DataFrame df2, JoinType jType)
		{
			if (df2 == null)
				throw new ArgumentException(nameof(df2));

			//if left dataframe is empty just return it.
			if (this.Values.Count == 0)
				return this;

			//if right dataframe is empty and joint type is left
			if (jType == JoinType.Left && df2.Values.Count == 0)
				return this;

			//if right dataframe is empty and joint type is inner
			if (jType == JoinType.Inner && df2.Values.Count == 0)
				return df2;


			// Initialize column types
			EnsureColumnTypesInitialized();
			df2.EnsureColumnTypesInitialized();

			// Merge column names and types
			var totalColumns = Columns.Concat(df2.Columns).ToList();
			var totalTypes = (_colTypes ?? Array.Empty<ColType>())
				.Concat(df2._colTypes ?? Array.Empty<ColType>()).ToList();

			// Create a lookup for the right DataFrame
			var rightIndexLookup = df2.Index
				.Select((key, value) => (key, value))
				.GroupBy(x => x.key)
				.ToDictionary(g => g.Key, g => g.Select(x => x.value).ToList());

			// Process left DataFrame
			var mergedValues = new List<object?>();
			var mergedIndex = new List<object>();
			for (int i = 0; i < RowCount(); i++)
			{
				var leftKey = Index[i];
				if (rightIndexLookup.TryGetValue(leftKey, out var rightRowIndices))
				{
					foreach (var j in rightRowIndices)
					{
						mergedIndex.Add(leftKey);
						mergedValues.AddRange(GetRowValues(i));
						mergedValues.AddRange(df2.GetRowValues(j));
					}
				}
				else if (jType == JoinType.Left)
				{
					// Handle Left join with missing data
					mergedIndex.Add(leftKey);
					mergedValues.AddRange(GetRowValues(i));
					mergedValues.AddRange(Enumerable.Repeat(DataFrame.NAN, df2.ColCount())!);
				}
			}

			// Construct resulting DataFrame
			return new DataFrame(mergedValues, mergedIndex, totalColumns, totalTypes.ToArray());
		}

		/// <summary>
		/// Retrieves the values of a row by its index.
		/// </summary>
		/// <param name="rowIndex">The index of the row.</param>
		/// <returns>A list of values in the row.</returns>
		private IEnumerable<object?> GetRowValues(int rowIndex)
		{
			int start = rowIndex * ColCount();
			return Values.Skip(start).Take(ColCount());
		}

		/// <summary>
		/// Merges two DataFrames based on specified column criteria.
		/// </summary>
		/// <param name="df2">The right DataFrame to merge.</param>
		/// <param name="leftOn">Columns in the left DataFrame used for the join.</param>
		/// <param name="rightOn">Columns in the right DataFrame used for the join.</param>
		/// <param name="jType">The type of join (e.g., Left, Inner).</param>
		/// <param name="suffix">The suffix to use for columns from the right DataFrame.</param>
		/// <returns>A new DataFrame resulting from the merge operation.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown if df2, leftOn, or rightOn are null, or if the number of join columns exceeds three.
		/// </exception>
		/// <example>
		/// // Example: Perform an inner merge
		/// DataFrame df1 = new DataFrame(
		///     new List<object> { 1, "A", 2, "B", 3, "C" },
		///     new List<object> { "row1", "row2", "row3" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.STR });
		///
		/// DataFrame df2 = new DataFrame(
		///     new List<object> { 2, "D", 3, "E", 4, "F" },
		///     new List<object> { "rowA", "rowB", "rowC" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.STR });
		///
		/// DataFrame result = df1.Merge(df2, new[] { "col1" }, new[] { "col1" }, JoinType.Inner, "right");
		///
		/// // Resulting DataFrame:
		/// // Index   | col1 | col2   | col2_right
		/// // -------------------------------
		/// // row1    | 2    | "B"    | "D"
		/// // row2    | 3    | "C"    | "E"
		/// </example>
		/// <summary>
		/// Merge two (left and right) data frames on specified leftOn and RightOn columns.
		/// </summary>
		/// <param name="df2">Second data frame.</param>
		/// <param name="leftOn">The list of column names for left data frames.</param>
		/// <param name="rightOn">The list of column names for right data frames.</param>
		/// <param name="jType">Join types. It can be Inner or Left. In case of right join call join from the second df.</param>
		/// <param name="suffix">For same column names, use suffix to make different names during merging.</param>
		/// <returns></returns>
		public DataFrame Merge(DataFrame df2, string[] leftOn, string[] rightOn, JoinType jType, string suffix = "right")
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
			EnsureColumnTypesInitialized();
			df2.EnsureColumnTypesInitialized();
			var typ1 = this._colTypes;
			var typ2 = df2._colTypes;

			//merge column names
			(List<string> totCols, List<ColType> totType) = mergeColumns(this.Columns, typ1!, df2.Columns, typ2!, suffix);

			//create lookup table
			(ILookup<object, int>? lookup1,
			 TwoKeyLookup<object, object, int>? lookup2,
			 ThreeKeyLookup<object, object, object, int>? lookup3) = createLookup(df2, rightOn);


			//mrging process
			var leftInd = getColumnIndex(leftOn);
			var lst = new List<object?>();//values
			var finIndex = new List<object>();//left df enumeration
			var leftRCount = this.RowCount();
			var leftCCount = this.ColCount();
			var rightRCount = df2.RowCount();
			var rightCCount = df2.ColCount();

			//
			for (int i = 0; i < leftRCount; i++)
			{
				var leftKey = Index[i];

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
							lst.Add(Values[r]);

						//fill right table
						int startR = j * rightCCount;
						//
						for (int r = startR; r < startR + rightCCount; r++)
							lst.Add(df2.Values[r]);
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
							lst.Add(Values[r]);

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

		#region Rename-column

		/// <summary>
		/// Rename one or more column names within the DataFrame.
		/// </summary>
		/// <param name="colNames">Array of tuples where each tuple contains the old column name and the new column name.</param>
		/// <returns>True if all columns are successfully renamed.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown when a column does not exist, the new name is invalid, or duplicates are introduced.
		/// </exception>
		/// <example>
		/// // Example: Rename columns in the DataFrame
		/// DataFrame df = new DataFrame(
		///     new List<object> { 1, 2, 3, 4 },
		///     new List<object> { "row1", "row2", "row3", "row4" },
		///     new List<string> { "col1", "col2", "col3" },
		///     new ColType[] { ColType.I32, ColType.I32, ColType.I32 });
		///
		/// // Rename col1 to newCol1 and col2 to newCol2
		/// bool result = df.Rename(("col1", "newCol1"), ("col2", "newCol2"));
		///
		/// // After renaming:
		/// // Columns: ["newCol1", "newCol2", "col3"]
		/// // Values: [1, 2, 3, 4]
		/// // Index: ["row1", "row2", "row3", "row4"]
		///
		/// // Example: Attempt to rename a column that does not exist
		/// try
		/// {
		///     df.Rename(("col4", "newCol4"));
		/// }
		/// catch (ArgumentException ex)
		/// {
		///     Console.WriteLine(ex.Message); 
		///     // Output: "The column name 'col4' does not exist in the DataFrame."
		/// }
		/// </example>
		public bool Rename(params (string oldName, string newName)[] colNames)
		{
			var renamedColumns = new HashSet<string>(Columns); // To check for duplicate names

			foreach (var (oldName, newName) in colNames)
			{
				// Ensure oldName exists
				var index = Columns.IndexOf(oldName);
				if (index == -1)
					throw new ArgumentException($"The column name '{oldName}' does not exist in the DataFrame.");

				// Ensure the new name is valid
				if (string.IsNullOrWhiteSpace(newName))
					throw new ArgumentException("New column name cannot be null or empty.");

				// Check for duplicate column names
				if (renamedColumns.Contains(newName) && newName != oldName)
					throw new ArgumentException($"The new column name '{newName}' would create a duplicate.");

				// Rename the column
				Columns[index] = newName;

				// Update the set for duplicate checks
				renamedColumns.Remove(oldName);
				renamedColumns.Add(newName);
			}

			return true;
		}

		#endregion

		#region Sorting
		/// <summary>
		/// Sorts the DataFrame by one or more specified columns.
		/// </summary>
		/// <param name="cols">
		/// The names of the columns to sort by. The sort is applied in the order the columns are specified.
		/// For example, specifying "col1" and "col2" will sort primarily by "col1" and use "col2" for tie-breaking.
		/// </param>
		/// <returns>
		/// A new DataFrame instance with rows sorted by the specified columns.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if:
		/// - No columns are provided.
		/// - Columns provided do not exist in the DataFrame.
		/// </exception>
		/// <remarks>
		/// This method determines the sorting algorithm to use based on the `qsAlgo` field:
		/// - If `qsAlgo` is true, QuickSort is used.
		/// - If `qsAlgo` is false, MergeSort is used.
		/// 
		/// The sorting respects the DataFrame's column types (`ColType`) for comparisons, including handling 
		/// strings, integers, floating-point numbers, and dates appropriately.
		/// </remarks>
		/// <example>
		/// // Example: Sort a DataFrame by a single column
		/// DataFrame df = new DataFrame(
		///     new List<object> { 3, "B", 1, "A", 2, "C" },
		///     new List<object> { "row1", "row2", "row3" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.STR });
		///
		/// // Sort by col1
		/// DataFrame sortedDf = df.SortBy("col1");
		///
		/// // Resulting sorted DataFrame:
		/// // Index   | col1 | col2
		/// // ---------------------
		/// // row2    | 1    | "A"
		/// // row3    | 2    | "C"
		/// // row1    | 3    | "B"
		///
		/// // Example: Sort a DataFrame by multiple columns
		/// sortedDf = df.SortBy("col1", "col2");
		/// 
		/// // Resulting sorted DataFrame (if tie-breaking by col2 applies):
		/// // Index   | col1 | col2
		/// // ---------------------
		/// // row2    | 1    | "A"
		/// // row3    | 2    | "C"
		/// // row1    | 3    | "B"
		/// </example>
		public DataFrame SortBy(params string[] cols)
		{
			// Validate input
			if (cols == null || cols.Length == 0)
				throw new ArgumentException("At least one column name must be provided for sorting.");

			// Ensure column types are initialized
			EnsureColumnTypesInitialized();

			// Get the column indices for the specified columns
			var colInd = getColumnIndex(cols);
			if (colInd.Length == 0)
				throw new ArgumentException("None of the specified columns exist in the DataFrame.");

			// Delegate to the SortDataFrame class
			var sorter = new SortDataFrame(_colTypes!);
			List<object?> sortedValues;
			List<object> sortedIndices;

			if (qsAlgo)
			{
				(sortedValues, sortedIndices) = sorter.QuickSort(Values, Index.ToList(), colInd);
			}
			else
			{
				(sortedValues, sortedIndices) = sorter.MergeSort(Values.ToArray()!, Index.ToArray(), colInd);
			}

			// Create a new sorted DataFrame
			return new DataFrame(sortedValues, sortedIndices, Columns.ToList(), _colTypes!);
		}


		/// <summary>
		/// Sorts the DataFrame by one or more specified columns in descending order.
		/// </summary>
		/// <param name="cols">
		/// The names of the columns to sort by. Sorting is applied in descending order for all specified columns.
		/// For example, specifying "col1" and "col2" will sort primarily by "col1" in descending order,
		/// and use "col2" in descending order for tie-breaking.
		/// </param>
		/// <returns>
		/// A new DataFrame instance with rows sorted by the specified columns in descending order.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if:
		/// - No columns are provided.
		/// - Columns provided do not exist in the DataFrame.
		/// </exception>
		/// <remarks>
		/// The method first sorts the DataFrame using the <see cref="SortBy"/> method in ascending order.
		/// It then reverses the order of rows in the DataFrame to produce a descending sort.
		/// 
		/// The descending sort respects the DataFrame's column types (`ColType`) for comparisons, including handling 
		/// strings, integers, floating-point numbers, and dates appropriately.
		/// </remarks>
		/// <example>
		/// // Example: Sort a DataFrame by "col1" in descending order
		/// DataFrame df = new DataFrame(
		///     new List<object> { 3, "B", 1, "A", 2, "C" },
		///     new List<object> { "row1", "row2", "row3" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.STR });

		/// // Sort by col1 in descending order
		/// DataFrame sortedDf = df.SortByDescending("col1");

		/// // Resulting sorted DataFrame:
		/// // Index   | col1 | col2
		/// // ---------------------
		/// // row1    | 3    | "B"
		/// // row3    | 2    | "C"
		/// // row2    | 1    | "A"
		/// </example>
		public DataFrame SortByDescending(params string[] cols)
		{
			// First sort the DataFrame in ascending order using SortBy
			var df = SortBy(cols);

			// Reverse the order of rows to achieve descending order
			DataFrame newDf = df.reverse();
			return newDf;
		}

		#endregion

		#region RemoveRows      
		/// <summary>
		/// Removes rows from the DataFrame that satisfy the provided callback condition.
		/// </summary>
		/// <param name="removeConditions">
		/// A callback function that takes two parameters:
		/// 1. A dictionary representing a row (with column names as keys and row values as values).
		/// 2. The row index.
		/// The function should return true if the row satisfies the condition for removal, and false otherwise.
		/// </param>
		/// <returns>
		/// A new DataFrame instance with rows that do not satisfy the removal condition.
		/// </returns>
		/// <example>
		/// // Example: Remove rows where "col1" is greater than 10
		/// DataFrame df = new DataFrame(
		///     new List<object> { 5, "A", 15, "B", 25, "C" },
		///     new List<object> { "row1", "row2", "row3" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.STR });
		/// // Define removal condition
		/// Func<IDictionary<string, object>, int, bool> condition = (row, index) => (int)row["col1"] > 10;
		/// // Apply removal
		/// DataFrame newDf = df.RemoveRows(condition);
		/// // Resulting DataFrame:
		/// // Index   | col1 | col2
		/// // ---------------------
		/// // row1    | 5    | "A"
		/// </example>
		public DataFrame RemoveRows(Func<IDictionary<string, object?>, int, bool> removeConditions)
		{
			if (removeConditions == null)
				throw new ArgumentException("The removal condition callback cannot be null.");

			// Define the dictionary to represent a row during processing
			var rowDict = new Dictionary<string, object?>();
			foreach (var column in Columns)
				rowDict[column] = null;

			// Lists to hold new DataFrame values and indices
			var newValues = new List<object?>();
			var newIndices = new List<object>();

			// Iterate through rows
			for (int i = 0; i < Index.Count; i++)
			{
				// Populate the row dictionary with values from the current row
				PopulateRowDictionary(rowDict, i);

				// Check the condition for removal
				if (!removeConditions(rowDict, i))
				{
					// Add the row's values to the new data
					int rowStart = calculateIndex(i, 0);
					for (int j = 0; j < Columns.Count; j++)
						newValues.Add(Values[rowStart + j]);

					// Add the row's index to the new indices
					newIndices.Add(Index[i]);
				}
			}

			// Construct the new DataFrame
			return new DataFrame(newValues, newIndices, Columns.ToList(), _colTypes!);
		}

		/// <summary>
		/// Populates the row dictionary with values from a specific row in the DataFrame.
		/// </summary>
		/// <param name="rowDict">The dictionary to populate.</param>
		/// <param name="rowIndex">The index of the row to process.</param>
		private void PopulateRowDictionary(Dictionary<string, object?> rowDict, int rowIndex)
		{
			int startIndex = calculateIndex(rowIndex, 0);
			for (int j = 0; j < Columns.Count; j++)
			{
				rowDict[Columns[j]] = Values[startIndex + j];
			}
		}


		/// <summary>
		/// Removes rows from the DataFrame that satisfy the provided callback condition.
		/// </summary>
		/// <param name="removeConditions">
		/// A callback function that takes two parameters:
		/// 1. An array representing a row (with column values in order).
		/// 2. The row index.
		/// The function should return true if the row satisfies the condition for removal, and false otherwise.
		/// </param>
		/// <returns>
		/// A new DataFrame instance with rows that do not satisfy the removal condition.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if the removal condition callback is null.
		/// </exception>
		/// <example>
		/// // Example: Remove rows where "col1" is greater than 10
		/// DataFrame df = new DataFrame(
		///     new List<object> { 5, "A", 15, "B", 25, "C" },
		///     new List<object> { "row1", "row2", "row3" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.STR });
		/// // Define removal condition
		/// Func<object[], int, bool> condition = (row, index) => (int)row[0] > 10;
		/// // Apply removal
		/// DataFrame newDf = df.RemoveRows(condition);
		/// // Resulting DataFrame:
		/// // Index   | col1 | col2
		/// // ---------------------
		/// // row1    | 5    | "A"
		/// </example>
		public DataFrame RemoveRows(Func<object[], int, bool> removeConditions)
		{
			// Validate the callback
			if (removeConditions == null)
				throw new ArgumentException("The removal condition callback cannot be null.");

			// Prepare row array for processing
			var rowArray = new object[ColCount()];

			// Prepare lists for values and indices in the resulting DataFrame
			var newValues = new List<object?>();
			var newIndices = new List<object>();

			// Iterate through rows
			for (int i = 0; i < Index.Count; i++)
			{
				// Populate the row array with values from the current row
				PopulateRowArray(rowArray, i);

				// Check the condition for removal
				if (!removeConditions(rowArray, i))
				{
					// Add the row's values to the new data
					int rowStart = calculateIndex(i, 0);
					for (int j = 0; j < Columns.Count; j++)
						newValues.Add(Values[rowStart + j]);

					// Add the row's index to the new indices
					newIndices.Add(Index[i]);
				}
			}

			// Construct the new DataFrame
			return new DataFrame(newValues, newIndices, Columns.ToList(), _colTypes!);
		}

		/// <summary>
		/// Populates the row array with values from a specific row in the DataFrame.
		/// </summary>
		/// <param name="rowArray">The array to populate.</param>
		/// <param name="rowIndex">The index of the row to process.</param>
		private void PopulateRowArray(object[] rowArray, int rowIndex)
		{
			int startIndex = calculateIndex(rowIndex, 0);
			for (int j = 0; j < Columns.Count; j++)
			{
				rowArray[j] = Values[startIndex + j]!;
			}
		}


		#endregion

		#region Rolling


		/// <summary>
		/// Groups the DataFrame by one, two, or three columns.
		/// </summary>
		/// <param name="groupCols">
		/// The names of columns to group by. A maximum of three columns is supported.
		/// </param>
		/// <returns>
		/// A <see cref="GroupDataFrame"/> containing grouped data.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if:
		/// - No columns are specified.
		/// - More than three columns are specified.
		/// - A specified column does not exist.
		/// </exception>
		/// <remarks>
		/// This method supports grouping by up to three columns. Each group contains its own DataFrame.
		/// </remarks>
		/// <example>
		/// // Example: Grouping by a single column ("col1")
		/// DataFrame df = new DataFrame(
		///     new List<object> { 1, "A", 2, "B", 1, "C", 2, "D" },
		///     new List<object> { "row1", "row2", "row3", "row4" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.STR });
		///
		/// GroupDataFrame groupedDf = df.GroupBy("col1");
		///
		/// // Example: Grouping by two columns ("col1" and "col2")
		/// GroupDataFrame groupedDf = df.GroupBy("col1", "col2");
		///
		/// // Example: Grouping by three columns ("col1", "col2", "col3")
		/// GroupDataFrame groupedDf = df.GroupBy("col1", "col2", "col3");
		/// </example>
		public GroupDataFrame GroupBy(params string[] groupCols)
		{
			if (groupCols == null || groupCols.Length == 0)
				throw new ArgumentException("At least one column name must be provided for grouping.");

			if (groupCols.Length > 3)
				throw new ArgumentException("Grouping by more than three columns is not supported.");

			// Ensure the columns exist
			foreach (var col in groupCols)
			{
				if (!Columns.Contains(col))
					throw new ArgumentException($"Column '{col}' does not exist in the DataFrame.");
			}

			if (groupCols.Length == 1)
			{
				var groupedData = groupDFBy(groupCols[0]);
				return new GroupDataFrame(groupCols[0], groupedData);
			}
			else if (groupCols.Length == 2)
			{
				var groupedData = new TwoKeysDictionary<object, object, DataFrame>();
				var firstLevel = groupDFBy(groupCols[0]);

				foreach (var firstKey in firstLevel.Keys)
				{
					var secondLevel = firstLevel[firstKey].groupDFBy(groupCols[1]);
					foreach (var secondKey in secondLevel.Keys)
					{
						groupedData.Add(firstKey, secondKey, secondLevel[secondKey]);
					}
				}

				return new GroupDataFrame(groupCols[0], groupCols[1], groupedData);
			}
			else
			{
				var groupedData = new ThreeKeysDictionary<object, object, object, DataFrame>();
				var firstTwoGroups = GroupBy(groupCols[0], groupCols[1]);

				foreach (var (firstKey, secondKey) in firstTwoGroups.Keys2)
				{
					var thirdLevelGroup = firstTwoGroups[firstKey, secondKey].GroupBy(groupCols[2]);
					foreach (var thirdKey in thirdLevelGroup.Keys)
					{
						groupedData.Add(firstKey, secondKey, thirdKey, thirdLevelGroup[thirdKey]);
					}
				}

				return new GroupDataFrame(groupCols[0], groupCols[1], groupCols[2], groupedData);
			}
		}


		/// <summary>
		/// Computes rolling aggregation over the DataFrame using the specified window size and a single aggregation operation.
		/// </summary>
		/// <param name="window">
		/// The rolling window size, determining how many previous rows are considered for aggregation.
		/// </param>
		/// <param name="agg">
		/// The aggregation operation to apply to all columns that support it.
		/// Columns that do not support the specified aggregation will be excluded from the resulting DataFrame.
		/// </param>
		/// <returns>
		/// A new DataFrame containing computed rolling values for all eligible columns.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if the window size is less than 1.
		/// </exception>
		/// <remarks>
		/// This method applies rolling aggregation across all columns that support the specified function.
		/// If the column does not support the given aggregation (e.g., trying to compute a mean on a string column),
		/// it will be automatically excluded.
		/// 
		/// The rolling computation considers a fixed-size window, aggregating values using the specified function.
		/// If the number of available rows is smaller than the window size, missing values are represented accordingly.
		/// </remarks>
		/// <example>
		/// // Example: Compute rolling mean over all numerical columns with a window of size 3.
		/// DataFrame df = new DataFrame(
		///     new List<object> { 1, 2, 3, 4, 5 },
		///     new List<object> { "row1", "row2", "row3", "row4", "row5" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.I32 });
		///
		/// DataFrame rollingDf = df.Rolling(3, Aggregation.Mean);
		///
		/// // Resulting DataFrame:
		/// // Index   | col1  | col2
		/// // -----------------------
		/// // row1    | NAN   | NAN
		/// // row2    | NAN   | NAN
		/// // row3    | 2.0   | 6
		/// // row4    | 3.0   | 9
		/// // row5    | 4.0   | 12
		/// </example>
		public DataFrame Rolling(int window, Aggregation agg)

		{
			if (window < 1)
				throw new ArgumentException("Window size must be at least 1.");

			var columnAggregations = Columns.ToDictionary(col => col, _ => agg);
			return Rolling(window, columnAggregations);
		}

		/// <summary>
		/// Computes rolling aggregation over the DataFrame using the specified window size and column-specific aggregation operations.
		/// </summary>
		/// <param name="window">
		/// The rolling window size, determining how many previous rows are considered for aggregation.
		/// </param>
		/// <param name="agg">
		/// A dictionary mapping column names to aggregation operations.
		/// Only columns explicitly included in this dictionary will undergo rolling computations.
		/// </param>
		/// <returns>
		/// A new DataFrame containing computed rolling values for the specified columns.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if:
		/// - The aggregation dictionary is null or empty.
		/// - The window size is less than 1.
		/// - None of the specified columns exist in the DataFrame.
		/// </exception>
		/// <remarks>
		/// This method applies rolling aggregation only to columns that are explicitly listed in the <paramref name="agg"/> dictionary.
		/// Unsupported columns will be ignored.
		/// 
		/// The rolling computation considers a fixed-size window, aggregating values using the specified function.
		/// If the number of available rows is smaller than the window size, the method fills missing values accordingly.
		/// </remarks>
		/// <example>
		/// // Example: Compute rolling mean for "col1" and rolling sum for "col2" over a window of size 3.
		/// DataFrame df = new DataFrame(
		///     new List<object> { 1, 2, 3, 4, 5 },
		///     new List<object> { "row1", "row2", "row3", "row4", "row5" },
		///     new List<string> { "col1", "col2" },
		///     new ColType[] { ColType.I32, ColType.I32 });
		/// var columnAggregations = new Dictionary<string, Aggregation>
		/// {
		///     { "col1", Aggregation.Mean },
		///     { "col2", Aggregation.Sum }
		/// };
		/// DataFrame rollingDf = df.Rolling(3, columnAggregations);
		///
		/// // Resulting DataFrame:
		/// // Index   | col1  | col2
		/// // -----------------------
		/// // row1    | NAN   | NAN
		/// // row2    | NAN   | NAN
		/// // row3    | 2.0   | 6
		/// // row4    | 3.0   | 9
		/// // row5    | 4.0   | 12
		/// </example>
		public DataFrame Rolling(int window, Dictionary<string, Aggregation> agg)

		{
			if (agg == null || agg.Count == 0)
				throw new ArgumentException("Aggregation dictionary must contain at least one column.");

			if (window < 1)
				throw new ArgumentException("Window size must be at least 1.");

			var rollingQueues = new Dictionary<string, Queue<object>>();
			var aggregatedValues = new Dictionary<string, List<object?>>();

			EnsureColumnTypesInitialized();

			for (int i = 0; i < Index.Count; i++)
			{
				for (int j = 0; j < ColCount(); j++)
				{
					var columnName = Columns[j];

					if (!agg.ContainsKey(columnName) || !isOperationSupported(ColTypes[j], agg[columnName]))
						continue;

					if (!rollingQueues.ContainsKey(columnName))
					{
						rollingQueues[columnName] = new Queue<object>(window);
						aggregatedValues[columnName] = new List<object?>();
					}

					rollingQueues[columnName].Enqueue(Values[calculateIndex(i, j)]!);

					if (rollingQueues[columnName].Count < window)
					{
						var val = agg[columnName] == Aggregation.Count ? rollingQueues[columnName].Count : NAN;
						aggregatedValues[columnName].Add(val!);
					}
					else
					{
						var val = calculateAggregation(rollingQueues[columnName], agg[columnName], ColTypes[j]);
						aggregatedValues[columnName].Add(val);
						rollingQueues[columnName].Dequeue();
					}
				}
			}

			return new DataFrame(aggregatedValues, Index.ToList());
		}


		#endregion

		#region Shift
		/// <summary>
		/// Shifts the values of a specified column by a given number of rows.
		/// </summary>
		/// <param name="steps">
		/// The number of rows to shift.
		/// - Positive values shift downward (insert missing values at the top).
		/// - Negative values shift upward (insert missing values at the bottom).
		/// </param>
		/// <param name="columnName">
		/// The name of the existing column to be shifted.
		/// </param>
		/// <param name="newColName">
		/// The name of the new shifted column.
		/// Must be different from existing column names.
		/// </param>
		/// <returns>
		/// A dictionary containing the new column with shifted values.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if:
		/// - `steps` is 0.
		/// - `columnName` does not exist.
		/// - `newColName` is already an existing column.
		/// </exception>
		/// <example>
		/// // Example: Shift "col1" down by 2 rows and create "col1_shifted"
		/// DataFrame df = new DataFrame(
		///     ("col1", new object[] {1, 2, 3, 4, 5}),
		///     ("col2", new object[] {10, 20, 30, 40, 50}));
		///
		/// var shiftedCol = df.Shift(2, "col1", "col1_shifted");
		///
		/// // Result:
		/// // Index   | col1 | col1_shifted
		/// // -------------------------------
		/// // 1       | 1    | NAN
		/// // 2       | 2    | NAN
		/// // 3       | 3    | 1
		/// // 4       | 4    | 2
		/// // 5       | 5    | 3
		/// </example>
		public Dictionary<string, List<object>> Shift(int steps, string columnName, string newColName)
		{
			if (steps == 0)
				throw new ArgumentException("'steps' must be nonzero.");

			if (!this.Columns.Contains(columnName))
				throw new ArgumentException($"Column '{columnName}' doesn't exist in the data frame.");

			if (this.Columns.Contains(newColName))
				throw new ArgumentException($"New column name '{newColName}' already exists.");

			var columnValues = this[columnName].ToList();
			var nanList = Enumerable.Repeat(DataFrame.NAN, Math.Abs(steps)).ToList();
			List<object> shiftedValues;

			if (steps > 0)
			{
				shiftedValues = nanList.Concat(columnValues).Take(this.RowCount()).ToList()!;
			}
			else
			{
				shiftedValues = columnValues.Concat(nanList).Skip(Math.Abs(steps)).ToList()!;
			}

			return new Dictionary<string, List<object>> { { newColName, shiftedValues } };
		}

		/// <summary>
		/// Shifts specified columns by given steps and adds them to the DataFrame.
		/// </summary>
		/// <param name="args">
		/// A tuple list of steps, columnName, and newColName.
		/// </param>
		/// <returns>
		/// A new DataFrame containing the newly shifted columns.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if:
		/// - No columns are provided.
		/// - Any new column name is duplicated.
		/// </exception>
		/// <example>
		/// // Example: Shift multiple columns
		/// DataFrame df = new DataFrame(
		///     ("col1", new object[] {1, 2, 3, 4, 5}),
		///     ("col2", new object[] {10, 20, 30, 40, 50}));
		///
		/// var newDf = df.Shift(("col1", "col1_shifted", 2), ("col2", "col2_shifted", -1));
		/// </example>
		public DataFrame Shift(params (string columnName, string newColName, int steps)[] args)
		{
			if (args == null || args.Length == 0)
				throw new ArgumentException("At least one column must be specified for shifting.");

			if (args.GroupBy(x => x.newColName).Any(g => g.Count() > 1))
				throw new ArgumentException("New column names must be unique.");

			var shiftedColumns = args.ToDictionary(c => c.newColName, c => Shift(c.steps, c.columnName, c.newColName)[c.newColName]);

			return this.AddColumns(shiftedColumns!);
		}


		/// <summary>
		/// Computes the difference between the current row and previous row.
		/// </summary>
		/// <param name="step">
		/// The number of rows to compare for the difference.
		/// </param>
		/// <param name="type">
		/// The type of difference calculation:
		/// - `DiffType.Seasonal` performs seasonal differencing.
		/// - `DiffType.Recursive` performs recursive differencing.
		/// </param>
		/// <returns>
		/// A new DataFrame with difference values.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if `step` is less than 1.
		/// </exception>
		/// <example>
		/// // Example: Compute seasonal difference with step 1
		/// DataFrame df = new DataFrame(
		///     ("col1", new object[] {10, 20, 30, 40, 50}));
		///
		/// DataFrame diffDf = df.Diff(1, DiffType.Seasonal);
		/// </example>
		public DataFrame Diff(int step, DiffType type = DiffType.Seasonal)
		{
			if (step < 1)
				throw new ArgumentException("'step' must be greater than zero.");

			return type == DiffType.Seasonal ? seasonalDiff(step) : recursiveDiff(step);
		}


		#endregion

		#region Selection


		/// <summary>
		/// Returns a DataFrame consisting of every nth row.
		/// </summary>
		/// <param name="nthRow">
		/// The interval of rows to select. Must be greater than zero.
		/// </param>
		/// <param name="includeLast">
		/// If true, includes the last row even if it does not align perfectly with nthRow.
		/// </param>
		/// <returns>
		/// A new DataFrame containing every nth row.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if `nthRow` is less than 1.
		/// </exception>
		/// <example>
		/// // Example: Select every 2nd row from a DataFrame
		/// DataFrame df = new DataFrame(
		///     ("col1", new object[] {1, 2, 3, 4, 5}),
		///     ("col2", new object[] {10, 20, 30, 40, 50}));
		///
		/// DataFrame resultDf = df.TakeEvery(2);
		///
		/// // Result:
		/// // Index   | col1 | col2
		/// // ---------------------
		/// // 2       | 2    | 20
		/// // 4       | 4    | 40
		/// </example>
		public DataFrame TakeEvery(int nthRow, bool includeLast = false)
		{
			if (nthRow < 1)
				throw new ArgumentException("'nthRow' must be greater than zero.");

			var selectedValues = new List<object?>();
			var selectedIndices = new List<object>();

			for (int i = 0; i < Index.Count; i++)
			{
				if ((i + 1) % nthRow == 0)
				{
					selectedValues.AddRange(this[i]);
					selectedIndices.Add(Index[i]);
				}
			}

			// Include last row if requested
			if (includeLast && Index.Count % nthRow != 0)
			{
				int lastIndex = Index.Count - 1;
				selectedValues.AddRange(this[lastIndex]);
				selectedIndices.Add(Index[lastIndex]);
			}

			// Ensure column types are initialized
			if (_colTypes == null || _colTypes.Length == 0)
				EnsureColumnTypesInitialized();

			return new DataFrame(selectedValues, selectedIndices, Columns.ToList(), _colTypes!);
		}

		/// <summary>
		/// Returns a DataFrame consisting of a randomly selected subset of rows.
		/// </summary>
		/// <param name="rows">
		/// The number of rows to randomly select.
		/// If `rows` is greater than or equal to the total number of rows, returns the original DataFrame.
		/// </param>
		/// <returns>
		/// A new DataFrame containing randomly selected rows.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if `rows` is less than 1.
		/// </exception>
		/// <example>
		/// // Example: Select 3 random rows from a DataFrame
		/// DataFrame df = new DataFrame(
		///     ("col1", new object[] {1, 2, 3, 4, 5}),
		///     ("col2", new object[] {10, 20, 30, 40, 50}));
		///
		/// DataFrame resultDf = df.TakeRandom(3);
		/// </example>
		public DataFrame TakeRandom(int rows)
		{
			if (rows <= 0)
				throw new ArgumentException(nameof(rows));

			if (rows >= this.RowCount() )
				return this;

			var selected = new List<int>();
			double needed = rows;
			double available = Index.Count;

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
		/// Returns a DataFrame containing rows from the main DataFrame that are **not** present in the second DataFrame.
		/// </summary>
		/// <param name="data2">
		/// The second DataFrame. Only rows whose index does not exist in `data2` will be included.
		/// The index in `data2` must be equal to or a subset of the main DataFrame.
		/// </param>
		/// <returns>
		/// A new DataFrame containing rows from the main DataFrame that are not in `data2`.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if `data2` contains index values that are greater than the main DataFrame’s index.
		/// </exception>
		/// <remarks>
		/// Resetting the index in both DataFrames before calling this method is recommended for correctness.
		/// </remarks>
		/// <example>
		/// // Example: Excluding rows present in another DataFrame
		/// DataFrame df1 = new DataFrame(
		///     ("col1", new object[] {1, 2, 3, 4, 5}),
		///     ("col2", new object[] {10, 20, 30, 40, 50}));
		///
		/// DataFrame df2 = new DataFrame(
		///     ("col1", new object[] {2, 3}),
		///     ("col2", new object[] {20, 30}));
		///
		/// DataFrame resultDf = df1.Except(df2);
		///
		/// // Result:
		/// // Index   | col1 | col2
		/// // ---------------------
		/// // 1       | 1    | 10
		/// // 4       | 4    | 40
		/// // 5       | 5    | 50
		/// </example>
		public DataFrame Except(DataFrame data2)
		{
			if (!this.Columns.SequenceEqual(data2.Columns))
				throw new ArgumentException("Both DataFrames must have the same column names.");

			var rowsToExclude = new HashSet<string>();

			// Convert all rows from data2 into string keys for fast lookup
			for (int i = 0; i < data2.Index.Count; i++)
			{
				var rowValues = data2[i].Select(v => v?.ToString() ?? "NULL").ToList();
				rowsToExclude.Add(string.Join(",", rowValues));
			}

			var filteredValues = new List<object?>();
			var filteredIndices = new List<object>();

			// Iterate through main DataFrame and exclude matching rows
			for (int i=0; i< this.Index.Count; i++)
			{
				var rowValues = this[i].Select(v => v?.ToString() ?? "NULL").ToList();
				var rowKey = string.Join(",", rowValues);

				if (!rowsToExclude.Contains(rowKey))
				{
					filteredValues.AddRange(this[i]);
					filteredIndices.Add(this.Index[i]);
				}
			}

			return new DataFrame(filteredValues, filteredIndices, this.Columns.ToList(), this._colTypes!);
		}

		/// <summary>
		/// Returns a DataFrame containing the first **N** rows.
		/// </summary>
		/// <param name="count">
		/// The number of rows to retrieve. Defaults to 5.
		/// If `count` exceeds the total row count, the entire DataFrame is returned.
		/// </param>
		/// <returns>
		/// A new DataFrame containing the top `count` rows.
		/// </returns>
		/// <example>
		/// // Example: Get first 3 rows from a DataFrame
		/// DataFrame df = new DataFrame(
		///     ("col1", new object[] {1, 2, 3, 4, 5}),
		///     ("col2", new object[] {10, 20, 30, 40, 50}));
		///
		/// DataFrame resultDf = df.Head(3);
		/// </example>
		public DataFrame Head(int count = 5)
		{
			int numRows = Math.Min(RowCount(), count);
			var selectedValues = new List<object?>();
			var selectedIndices = new List<object>();

			for (int i = 0; i < numRows; i++)
			{
				selectedValues.AddRange(this[i]);
				selectedIndices.Add(Index[i]);
			}

			return new DataFrame(selectedValues, selectedIndices, Columns, _colTypes!);
		}

		/// <summary>
		/// Returns a DataFrame containing the last **N** rows.
		/// </summary>
		/// <param name="count">
		/// The number of rows to retrieve. Defaults to 5.
		/// If `count` exceeds the total row count, the entire DataFrame is returned.
		/// </param>
		/// <returns>
		/// A new DataFrame containing the last `count` rows.
		/// </returns>
		/// <example>
		/// // Example: Get last 3 rows from a DataFrame
		/// DataFrame resultDf = df.Tail(3);
		/// </example>
		public DataFrame Tail(int count = 5)
		{
			int numRows = Math.Min(RowCount(), count);
			var selectedValues = new List<object?>();
			var selectedIndices = new List<object>();

			for (int i = RowCount() - numRows; i < RowCount(); i++)
			{
				selectedValues.AddRange(this[i]);
				selectedIndices.Add(Index[i]);
			}

			return new DataFrame(selectedValues, selectedIndices, Columns, _colTypes!);
		}

		/// <summary>
		/// Returns a DataFrame containing the first **N** rows.
		/// </summary>
		/// <param name="rows">
		/// The number of rows to take.
		/// </param>
		/// <returns>
		/// A new DataFrame with the specified number of rows.
		/// If `rows` exceeds the total row count, the entire DataFrame is returned.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if `rows` is less than 1.
		/// </exception>
		/// <example>
		/// // Example: Take the first 3 rows
		/// DataFrame resultDf = df.Take(3);
		/// </example>
		public DataFrame Take(int rows)
		{
			if (rows < 1)
				throw new ArgumentException("Rows count must be greater than zero.");

			var selectedValues = new List<object?>();
			var selectedIndices = new List<object>();

			int numRows = Math.Min(rows, RowCount());

			for (int i = 0; i < numRows; i++)
			{
				selectedIndices.Add(Index[i]);
				selectedValues.AddRange(this[i]);
			}

			return new DataFrame(selectedValues, selectedIndices, Columns, _colTypes!);
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
		/// Gets or sets a specific value from the DataFrame positioned at (rowIndex, colIndex).
		/// </summary>
		/// <param name="row">Zero-based row index.</param>
		/// <param name="col">Zero-based column index.</param>
		/// <returns>Object representing the cell value.</returns>
		/// <exception cref="IndexOutOfRangeException">
		/// Thrown if `row` or `col` exceeds valid bounds.
		/// </exception>
		/// <example>
		/// DataFrame df = new DataFrame(
		///     ("col1", new object[] { 1, 2, 3 }),
		///     ("col2", new object[] { "A", "B", "C" }));
		///
		/// object value = df[1, 0]; // Retrieves value at row index 1, column index 0.
		/// df[2, 1] = "Modified"; // Modifies value at row index 2, column index 1.
		/// </example>
		public object? this[int row, int col]
		{
			get
			{
				if (row < 0 || row >= RowCount() || col < 0 || col >= ColCount())
					throw new IndexOutOfRangeException($"Index ({row}, {col}) is out of bounds.");

				return Values[calculateIndex(row, col)];
			}
			set
			{
				if (row < 0 || row >= RowCount() || col < 0 || col >= ColCount())
					throw new IndexOutOfRangeException($"Index ({row}, {col}) is out of bounds.");

				Values[calculateIndex(row, col)] = value;
			}
		}


		/// <summary>
		/// Gets or sets a specific value from the DataFrame based on column name and row index.
		/// </summary>
		/// <param name="col">Column name.</param>
		/// <param name="row">Zero-based row index.</param>
		/// <returns>Object representing the cell value.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown if column does not exist.
		/// </exception>
		/// <example>
		/// object value = df["col1", 2]; // Retrieve value in "col1" at row index 2.
		/// df["col2", 1] = "Updated"; // Modify value in "col2" at row index 1.
		/// </example>
		public object? this[string col, int row]
		{
			get
			{
				if (!Columns.Contains(col))
					throw new ArgumentException($"Column '{col}' does not exist.");

				int ind = calculateIndex(col, row);
				return Values[ind];
			}
			set
			{
				if (!Columns.Contains(col))
					throw new ArgumentException($"Column '{col}' does not exist.");

				int ind = calculateIndex(col, row);
				Values[ind] = value;
			}
		}



		/// <summary>
		/// Returns a new DataFrame generated from a subset of columns.
		/// </summary>
		/// <param name="cols">Column names.</param>
		/// <returns>New DataFrame containing selected columns.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown if any column does not exist.
		/// </exception>
		/// <example>
		/// DataFrame subsetDf = df["col1", "col2"];
		/// </example>
		public DataFrame this[params string[] cols]
		{
			get
			{
				if (!cols.All(Columns.Contains))
					throw new ArgumentException("One or more specified columns do not exist.");

				EnsureColumnTypesInitialized();

				//get indexes of the columns
				var colInd = getColumnIndex(cols);

				//reserve space
				var lst = new object[cols.Length * Index.Count];
				var counter = 0;
				var newCounter = 0;
				for (int i = 0; i < Index.Count; i++)
				{
					for (int j = 0; j < colInd.Length; j++)
					{
						lst[newCounter + j] = Values[counter + colInd[j]]!;
					}
					//increase indexes
					newCounter += colInd.Length;
					counter += Columns.Count;
				}

				var newColTypes = new ColType[cols.Length];
				for (int i=0; i< cols.Count(); i++)
				{
					newColTypes[i] = ColTypes[colInd[i]];
				}

				return new DataFrame(lst.Select(x=>(object?)x).ToList(), Index.ToList(), cols.ToList(), newColTypes);
			}
		}


		/// <summary>
		/// Returns all values from a specified column.
		/// </summary>
		/// <param name="col">Column name.</param>
		/// <returns>Enumerable list of column values.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown if column does not exist.
		/// </exception>
		/// <example>
		/// IEnumerable<object> columnData = df["col1"];
		/// </example>
		public IEnumerable<object?> this[string col]
		{
			get
			{
				if (!Columns.Contains(col))
					throw new ArgumentException($"Column '{col}' does not exist.");

				int colIndex = getColumnIndex(col);
				int cols = ColCount();

				for (int i = colIndex; i < Values.Count; i += cols)
					yield return Values[i];
			}
		}


		/// <summary>
		/// Returns all values from a specific row.
		/// </summary>
		/// <param name="row">Zero-based row index.</param>
		/// <returns>Enumerable list of row values.</returns>
		/// <exception cref="IndexOutOfRangeException">
		/// Thrown if row index is out of range.
		/// </exception>
		/// <example>
		/// IEnumerable<object> rowData = df[1];
		/// </example>
		public IEnumerable<object?> this[int row]
		{
			get
			{
				if (row < 0 || row >= RowCount())
					throw new IndexOutOfRangeException($"Row index '{row}' is out of range.");

				int cols = ColCount();
				int startIndex = calculateIndex(row, 0);

				for (int i = startIndex; i < startIndex + cols; i++)
					yield return Values[i];
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
		/// Returns a concise string representation of the DataFrame dimensions.
		/// </summary>
		/// <returns>
		/// A formatted string in the form `(rowCount, columnCount)`, where rowCount is the number of rows
		/// and columnCount is the number of columns.
		/// </returns>
		/// <example>
		/// DataFrame df = new DataFrame(("col1", new object[] {1, 2, 3}));
		/// string result = df.ToString(); // Output: "(3,1)"
		/// </example>
		public override string ToString()
		{
			return $"({RowCount()},{ColCount()})";
		}


		/// <summary>
		/// Constructs a tabular string representation of the DataFrame.
		/// </summary>
		/// <param name="rowCount">
		/// The number of rows to display. Defaults to 15.
		/// </param>
		/// <returns>
		/// A formatted string representation of the DataFrame.
		/// </returns>
		/// <example>
		/// string tableView = df.ToStringBuilder(5); // Shows first 5 rows in tabular format.
		/// </example>
		public string ToStringBuilder(int rowCount = 15)
		{
			int rows = this.RowCount();
			int cols = this.ColCount();
			int longestColumnName = this.Columns.Max(c => c.Length);

			StringBuilder sb = new StringBuilder();

			// Header Row
			sb.Append("".PadRight(longestColumnName));
			foreach (var column in Columns)
			{
				sb.Append(column.PadRight(longestColumnName));
			}
			sb.AppendLine();

			// Data Rows
			int rr = Math.Min(rowCount, rows);
			for (int i = 0; i < rr; i++)
			{
				sb.Append(Index[i].ToString()!.PadRight(longestColumnName));
				foreach (var obj in this[i])
				{
					sb.Append((obj ?? "null").ToString()!.PadRight(longestColumnName));
				}
				sb.AppendLine();
			}

			return sb.ToString();
		}


		public Array To1DArray()
        {
            return Values.ToArray();
        }

		/// <summary>
		/// Generates a tabular view suitable for console output.
		/// </summary>
		/// <param name="rowCount">Number of rows to display. Defaults to 15.</param>
		/// <returns>
		/// A formatted console-friendly representation of the DataFrame.
		/// </returns>
		/// <example>
		/// string consoleView = df.ToConsole(10);
		/// </example>
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
                IList<object?> row = this[i].ToList();

                sb.Append((Index[i] ?? "null").ToString()!.PadRight(longestColumnName));
                foreach (object? obj in row)
                {
                    sb.Append((obj ?? "null").ToString()!.PadRight(longestColumnName));
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
            checkColumnName(this.Columns, ser.Name);
            //
            var vals = new List<object?>();
            for (int i = 0; i < Index.Count; i++)
            {
                vals.AddRange(this[i]);
                vals.Add(ser[i]);
            }

            //new column
            var newCols = Columns.ToList();
            newCols.Add(ser.Name);

            //new column type
            var newType = ser.ColType;
			EnsureColumnTypesInitialized();

            var newcolTypes = this._colTypes!.ToList();
            newcolTypes.Add(newType);
            //
            var index = Index.ToList();
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
            checkColumnNames(this.Columns, sers.Select(x => x.Name).ToArray());
            //
            var vals = new List<object?>();
            for (int i = 0; i < Index.Count; i++)
            {
                vals.AddRange(this[i]);
                vals.AddRange(sers.Select(x => x[i]));
            }
            //
            var newCols = Columns.Union(sers.Select(x => x.Name)).ToList();

            EnsureColumnTypesInitialized();

            var newTypes = ColTypes.Union(sers.Select(x=>x.ColType)).ToArray();

            //
            var index = Index.ToList();
            return new DataFrame(vals, index, newCols, newTypes);
        }

        /// <summary>
        /// Extract a column as series object including index
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public Series ToSeries(string colName)
        {
            if(!this.Columns.Contains(colName))
                throw new Exception($"Column {colName} do not exist in the dataframe.");

            var data = this[colName].ToList();
            var ind = Index.ToList();
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
                if (!this.Columns.Contains(colName))
                    throw new Exception($"Column {colName} do not exist in the dataframe.");

                var data = this[colName].ToList();
                var ind = Index.ToList();
                var s = new Series(data, ind, colName);
                retVal.Add(s);
            }

            return retVal.ToArray();
        }

        public Series ToSeries()
        {
            if (this.Columns.Count != 1)
                throw new Exception("DataFrame must have one column to be converted into Series.");

            var ser = new Series(_values!, Index, this.Columns.First());

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
                    newDf.AddRow(new object?[ColCount()].ToList());

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
                newDf.InsertRow(0, nanrow!);
            }
            //calculate differencing
            for (int i = period; i < RowCount(); i++ )
            {
                var prevrow = new Series(this[i - period].ToList());
                var row = new Series(this[i].ToList());

                var diffs = row - prevrow;
                newDf.InsertRow(i, diffs.ToList()!);
            }

            return newDf;
        }

        private (List<string>, List<ColType>) mergeColumns(
                IList<string> cols1, ColType[] typ1, IList<string> cols2, ColType[] typ2, string? sufix = null)
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


        private (ILookup<object, int>? lookup1, TwoKeyLookup<object, object, int>? lookup2, ThreeKeyLookup<object, object, object, int>? lookup3) createLookup(DataFrame df, string[] cols)
        {
            var rightInd = df.getColumnIndex(cols);
            var rIndex = Enumerable.Range(0, df.RowCount()).ToArray();

            //create right lookup 
            ILookup<object?, int>? lookup1 = null;
            TwoKeyLookup<object, object, int>? lookup2 = null;
            ThreeKeyLookup<object, object, object, int>? lookup3 = null;

            //construct lookup table
            if (rightInd.Length == 1)
                lookup1 = df[cols[0]].Zip(rIndex, (key, value) => (key, value)).ToLookup(x => x.key, x => x.value);
            else if (rightInd.Length == 2)
                lookup2 = new TwoKeyLookup<object, object, int>(rIndex, item => (df[item, rightInd[0]]!, df[item, rightInd[1]]!));
            else if (rightInd.Length == 3)
                lookup3 = new ThreeKeyLookup<object, object, object, int>(rIndex, item => (df[item, rightInd[0]]!, df[item, rightInd[1]]!, df[item, rightInd[2]]!));
            else
                throw new Exception("Unknown number of Key columns");

            return (lookup1, lookup2, lookup3)!;
        }

        private int[] findIndex(ILookup<object, int>? lookup1, TwoKeyLookup<object, object, int>? lookup2, ThreeKeyLookup<object, object, object, int>? lookup3, int[] colInd, int i)
        {
            if (lookup1 != null)
                return lookup1[this[i, colInd[0]]!].ToArray();
            else if (lookup2 != null)
                return lookup2[this[i, colInd[0]]!, this[i, colInd[1]]!].ToArray();
            else if (lookup3 != null)
                return lookup3[this[i, colInd[0]]!, this[i, colInd[1]]!, this[i, colInd[2]]!].ToArray();
            else
                throw new Exception("Unknown number of key columns.");
        }

        private int[]? containsKey(int i, int[] leftInd, List<ILookup<object, int>> right)
        {
            if (leftInd.Length != right.Count)
                throw new Exception("The number o elements must be equal.");

            var leftRow = this[i];

            //try the first column
            var lftValue = this[i, leftInd[0]];
            if (lftValue== null || !right[0].Contains(lftValue))
                return null;

            //in case one one joined column return
            var firstIndexes = right[0][lftValue].ToArray();
            if (leftInd.Length == 1)
                return firstIndexes;

            //check the second column
            var lftValue2 = this[i, leftInd[1]];
            if (lftValue2 == null|| !right[1].Contains(lftValue2))
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
            if (lftValue3 == null || !right[2].Contains(lftValue3))
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

		static private bool IsNumeric(ColType cType)
        {
            if (cType == ColType.I32 || cType == ColType.I64 || cType == ColType.F32 || cType == ColType.DD)
                return true;
            else
                return false;
        }

        private DataFrame reverse()
        {
            var cols = this.Columns;
            var types = this._colTypes;

            //
            var lst = new List<object?>();
            var lstInd = new List<object>();
            for(int i= this.Index.Count-1; i >=0 ; i--)
            {
                for (int j = 0; j < this.Columns.Count; j++)
                {
                    var v = this[i, j];
                    lst.Add(v);
                }
                //
                lstInd.Add(Index[i]);
            }
            //
            var dff = new DataFrame(lst, lstInd, cols, types!);
            return dff;
        }

        private DataFrame getDataFramesRows(List<int> selected)
        {
            var val = new List<object?>();
            var ind = new List<object>();
            //go through selected rows
            for (int i = 0; i < selected.Count; i++)
            {
                var row = this[selected[i]];
                val.AddRange(row);
                ind.Add(Index[selected[i]]);
            }
            //
            var df = new DataFrame(val, ind, this.Columns.ToList(), this._colTypes!);
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
                var row = new List<object?>();
                object? groupValue = null;
                var colCnt = ColCount();
                var grpColIndex = getColumnIndex(groupCol);

                //
                for (int j = 0; j < colCnt; j++)
                {
                    row.Add(Values[pos]);
                    
                   // if (Columns[j].Equals(groupCol, StringComparison.InvariantCultureIgnoreCase))
                   if(grpColIndex==j)
                        groupValue = Values[pos];
                    pos++;
                }
				if (groupValue == null)
					continue;

                //add to group
                if (!Group.ContainsKey(groupValue))
                    Group.Add(groupValue, new DataFrame(row, new List<object>(){Index[i]}, this.Columns, this._colTypes!));
                else
                    Group[groupValue].AddRow(row, Index[i]);
            }

            return Group;
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
            var sameCols = this.Columns.Intersect(colNames).ToArray();
            //check if column exists
            if (sameCols.Length > 0)
            {
                var str = string.Join(", ", sameCols);
                throw new ArgumentException($"Column(s) '{str}' already exist(s) in the data frame.");
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
                return null!;//TODO:
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

        internal void AddRows(DataFrame df)
        {
			if (_values == null || df._values == null)
				return;

            _values.AddRange(df._values);
            Index.AddRange(df.Index);
        }
        #endregion
    }
}
