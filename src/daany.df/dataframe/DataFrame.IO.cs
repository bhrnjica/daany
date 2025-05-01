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
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Daany.Binding;

namespace Daany
{
	/// <summary>
	/// Represents a tabular data structure with strongly-typed columns, similar to pandas DataFrames.
	/// Provides functionality for data loading, transformation, filtering, and analysis.
	/// </summary>
	/// <remarks>
	/// The DataFrame is optimized for:
	/// - Efficient columnar storage
	/// - Type safety with explicit column types
	/// - Null/missing value handling
	/// - CSV/JSON/web data source integration
	/// - Common data operations (filter, sort, group, aggregate)
	/// </remarks>
	public partial class DataFrame : IDataFrame
    {

        #region Private fields
        private static readonly char[] _missingChars = new char[] { ' ', '?', '*', };
        private static readonly string[] _missingCharacters = new string[] { "n/a", "?", "*"," " };
        #endregion

        #region Static members
        public static bool ToCsvEx(string filePath, DataFrame dataFrame, char delimiter = ',', string? dateFormat = null, bool writHeader = true)
        {
            var columns = dataFrame.Columns;
            var data = dataFrame.Values;

			// Convert values to CellObject array
			CellObject[] cellObjects = new CellObject[dataFrame.Values.Count];
			
			for (int i = 0; i < dataFrame.Values.Count; i++)
			{
				cellObjects[i] = dataFrame.Values[i] switch
				{
					int n => new CellObject { value = new CellValue { intValue = n }, typeId = 0 },
					float n => new CellObject { value = new CellValue { floatValue = n }, typeId = 2 },
					double n => new CellObject { value = new CellValue { doubleValue = n }, typeId = 3 },
					long n => new CellObject { value = new CellValue { longValue = n }, typeId = 1 },
					string s => new CellObject { value = new CellValue { stringValue = DaanyRust.AllocateString(s) }, typeId = 4 },
					DateTime dt => new CellObject { value = new CellValue { datetimeValue = dt.ToUnixTimestampMilliseconds() }, typeId = 5 },
					null => new CellObject { value = new CellValue { stringValue = DaanyRust.AllocateString("*") }, typeId = 4 },
					_ => throw new Exception("Unsupported type")
				};
			}

			// Convert headers to IntPtr array
			IntPtr[] columnPointers = new IntPtr[dataFrame.Columns.Count];
			for (int i = 0; i < dataFrame.Columns.Count; i++)
				columnPointers[i] = DaanyRust.AllocateString(dataFrame.Columns[i]);

            //converts data to IntPtr
			IntPtr dataBuffer = Marshal.AllocHGlobal(Marshal.SizeOf<CellObject>() * dataFrame.Values.Count);
			for (int i = 0; i < dataFrame.Values.Count; i++)
				Marshal.StructureToPtr(cellObjects[i], dataBuffer + i * Marshal.SizeOf<CellObject>(), false);

			IntPtr columnBuffer = Marshal.AllocHGlobal(IntPtr.Size * dataFrame.Columns.Count);
			Marshal.Copy(columnPointers, 0, columnBuffer, dataFrame.Columns.Count);

			IntPtr filePathPtr = DaanyRust.AllocateString(filePath);
			IntPtr dateFormatPtr = DaanyRust.AllocateString("%Y-%m-%d");
			IntPtr missingValuePtr = DaanyRust.AllocateString("*");

			// Call Rust function
			DaanyRust.to_csv(filePathPtr, dataBuffer, dataFrame.Values.Count, columnBuffer, dataFrame.ColCount(), delimiter, true, dateFormatPtr);

			// Cleanup memory
			Marshal.FreeHGlobal(filePathPtr);
			Marshal.FreeHGlobal(dateFormatPtr);
			Marshal.FreeHGlobal(dataBuffer);
			Marshal.FreeHGlobal(columnBuffer);
            foreach (var ptr in cellObjects)
            {
                if (ptr.typeId == 4)
                    Marshal.FreeHGlobal(ptr.value.stringValue);
            };
			foreach (var ptr in columnPointers) Marshal.FreeHGlobal(ptr);

			Console.WriteLine($"CSV file '{filePath}' has been created successfully!");
            return true;
		}
		/// <summary>
		///  Saves data frame .NET object to csv file.
		/// </summary>
		/// <param name="filePath">Full or relative file path.</param>
		/// <param name="dataFrame">Data frame to persist.</param>
		/// <param name="delimiter">Use delimiter while writing.</param>
		/// <param name="dateFormat">Use data time  format while writing.</param>
		/// <param name="writHeader">Include heade in the file.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static bool ToCsv(string filePath, DataFrame dataFrame, char delimiter = ',', string? dateFormat = null, bool writHeader = true)
        {
            if (dataFrame == null)
                throw new ArgumentNullException(nameof(dataFrame));

            using (var strWr = File.CreateText(filePath))
            {
                var csvWriter = new CsvWriter(strWr);

                //write header
                if(writHeader)
                {
                    for (int i = 0; i < dataFrame.Columns.Count; i++)
                        csvWriter.WriteField(dataFrame.Columns[i]); 

                    csvWriter.NextRecord();
                }

                //write values
                int lstIndex = 0;
                for (int i = 0; i < dataFrame.RowCount(); i++)
                {
                    for (int j = 0; j < dataFrame.ColCount(); j++)
                    {
                        if (dataFrame._values[lstIndex] == DataFrame.NAN)
                        {
                            csvWriter.WriteField("");
                            lstIndex++;
                            continue;
                        }

                        else if(dataFrame.ColTypes[j]  == ColType.DT)
                        {
                            var dt = Convert.ToDateTime(dataFrame._values[lstIndex]);
                            if (!string.IsNullOrEmpty(dateFormat))
                                csvWriter.WriteField(dt.ToString(dateFormat));
                            else
                                csvWriter.WriteField(dt.ToString());
                        }
                        else
                        {
                            if(Convert.ToString(dataFrame._values[lstIndex], CultureInfo.InvariantCulture) is string strValue)
                                csvWriter.WriteField(strValue);
                        }
                        //
                        lstIndex++;
                    }
                    csvWriter.NextRecord();
                }
            }

            return true;

        }

        public static async Task<bool> ToCsvAsync(string filePath, DataFrame dataFrame, char delimiter = ',', string? dateFormat = null, bool writHeader = true)
        {
            if (dataFrame == null)
                throw new ArgumentNullException(nameof(dataFrame));

            using (var strWr = File.CreateText(filePath))
            {
                var csvWriter = new CsvWriter(strWr);
                //write header
                if (writHeader)
                {
                    for (int i = 0; i < dataFrame.Columns.Count; i++)
                        await csvWriter.WriteFieldAsync(dataFrame.Columns[i]);

                    csvWriter.NextRecord();
                }

                //write values
                int lstIndex = 0;
                for (int i = 0; i < dataFrame.RowCount(); i++)
                {
                    for (int j = 0; j < dataFrame.ColCount(); j++)
                    {
                        if (dataFrame._values[lstIndex] == DataFrame.NAN)
                        {
                            await csvWriter.WriteFieldAsync("");
                            lstIndex++;
                            continue;
                        }

                        else if (dataFrame.ColTypes[j] == ColType.DT)
                        {
                            var dt = Convert.ToDateTime(dataFrame._values[lstIndex]);
                            if (!string.IsNullOrEmpty(dateFormat))
                                await csvWriter.WriteFieldAsync(dt.ToString(dateFormat));
                            else
                                await csvWriter.WriteFieldAsync(dt.ToString());
                        }
                        else
                        {
                            if(Convert.ToString(dataFrame._values[lstIndex], CultureInfo.InvariantCulture) is string strValue)
                                await csvWriter.WriteFieldAsync(strValue);
                        }
                        //
                        lstIndex++;
                    }
                    csvWriter.NextRecord();
                }
            }

            return true;

        }

        public static bool ToCsv(string filePath, DataFrame dataFrame, string dFormat)
        {
            return DataFrame.ToCsv(filePath, dataFrame,',',dFormat);
        }


        /// <summary>
        /// Load data from the remote server.
        /// </summary>
        /// </summary>
        /// <param name="urlPath">Url of the file on remote server.</param>
        /// <param name="sep"> Separator string.</param>
        /// <param name="names">Column names in case the columns are provided separately from the file.</param>
        /// <param name="dformat">Date time format.</param>
        /// <param name="nRows">Number of loading rows. This is handy in case we need just few rows to load in order to see how df behaves.</param>
        /// <returns>Data Frame object.</returns>
        public static DataFrame FromWeb(string urlPath, char sep = ',', string[]? names = null, string? dformat = null, ColType[]? colTypes = null, int nRows = -1)
        {
            if (string.IsNullOrEmpty(urlPath))
                throw new ArgumentNullException(nameof(urlPath), "Argument should not be null.");

            var lines = new List<string>();
            //
            using (var fileDownloader = new HttpClient())
            {
                var streamTask = fileDownloader.GetStreamAsync(urlPath);
                streamTask.Wait();
                var stream = streamTask.Result;

                using (StreamReader reader = new StreamReader(stream))
                {
                    while(!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                            lines.Add(line);
                    }
                }
            }
             
            var df  = FromStrings(lines.ToArray(), sep,names, dformat, colTypes);
            return df;
        }

        public static async Task<DataFrame> FromWebAsync(string urlPath, char sep = ',', string[]? names = null, string? dformat = null, ColType[]? colTypes = null, int nRows = -1)
        {
            if (string.IsNullOrEmpty(urlPath))
                throw new ArgumentNullException(nameof(urlPath), "Argument should not be null.");

            var lines = new List<string>();
            //
            using (var fileDownloader = new HttpClient())
            {
                var stream = await fileDownloader.GetStreamAsync(urlPath);

                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string? line = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(line))
                            lines.Add(line);
                    }
                }
            }

            var df = FromStrings(lines.ToArray(), sep, names, dformat, colTypes);
            return df;
        }

        public static DataFrame FromText(string strText, char sep = ',', string[]? names = null, string? dformat = null, ColType[]? colTypes = null, char[]? missingValues=null, int nRows = -1, int skipLines = 0)
        {
            if (string.IsNullOrEmpty(strText))
                throw new ArgumentNullException(nameof(strText), "Argument should not be null.");

            
            using (var csvStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(strText)))
            {
                using (var srdr = new StreamReader(csvStream))
                {
                    var csvReader = new CsvReader(srdr, sep.ToString());

                    var columns = names == null ? new List<string>() : names.ToList();

                    var parsedate = dformat != null && dformat.Length > 0 ? true : false;

					var retVal = ParseReader(csvReader, columns: ref columns, colTypes: ref colTypes, parseDateTime:parsedate, dateFormats: dformat, nRows: nRows, parseDate: true, missingValue:missingValues, skipLines: skipLines);

                    var df = new DataFrame(retVal, columns, colTypes);

                    return df;
                }
            }
        }

        public static DataFrame FromStrings(string[] strArray, char sep = ',', string[]? names = null, string? dformat = null, ColType[]? colTypes = null, char[]? missingValues = null, int nRows = -1, int skipLines = 0)
        {
            if (strArray==null || strArray.Length ==0)
                throw new ArgumentNullException(nameof(strArray), "Argument should not be null or empty.");
            //prepare for stream
            var strText = string.Join(Environment.NewLine, strArray);

            using (var csvStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(strText)))
            {
                using (var srdr = new StreamReader(csvStream))
                {
                    var csvReader = new CsvReader(srdr, sep.ToString());

                    var columns = names == null ? new List<string>() : names.ToList();
                    var parsedate = dformat != null && dformat.Length > 0 ? true : false;

					var retVal = ParseReader(csvReader, columns: ref columns, colTypes:ref colTypes, parseDateTime: parsedate, dateFormats: dformat, nRows: nRows, parseDate: true, missingValue: missingValues, skipLines: skipLines);

                    var df = new DataFrame(retVal, columns, colTypes);
                    if (colTypes != null)
                        df._colTypes = colTypes;
                    return df;
                }
            }
        }
		public static DataFrame FromCsvEx(string filePath, char sep = ',', string[]? names = null, string? dformat = null, 
                                            bool parseDate = true, ColType[]? colTypes = null, char[]? missingValues = null, 
                                            int nRows = -1, int skipLines = 0)
        {
			IntPtr filePathPtr = DaanyRust.AllocateString(filePath);
			string missingValue = "*";
            bool hasHeader = true;
			IntPtr columnsPtr, dataPtr;
			ulong colCount, rowCount;
			//// Call Rust function
			DaanyRust.from_csv(filePath, sep, dformat!, missingValue, hasHeader, out columnsPtr, out colCount, out dataPtr, out rowCount);

			string[] columns = DaanyRust.exctractColumns(columnsPtr, colCount);
			object[] data = DaanyRust.exctractData(dataPtr, rowCount, colCount);

            return new DataFrame(data,columns);

		}
	/// <summary>
	/// Method for loading data from the file into data frame object.
	/// </summary>
	/// <param name="filePath">Full or relative path of the file.</param>
	/// <param name="sep"> Separator character.</param>
	/// <param name="names">Column names in case the columns are provided separately from the file.</param>
	/// <param name="dformat">Date time format.</param>
	/// <param name="nRows">Number of loading rows. This is handy in case we need just few rows to load in order to see how df behaves.</param>
	/// <param name="skipLines">Number of first lines to skip with parsing. This is handy in case we need to put description to data before actual data.</param>
	/// <returns>Data Frame object.</returns>
	public static DataFrame FromCsv(string filePath, char sep = ',', string[]? names = null, string? dformat = null, bool parseDate = true, ColType[]? colTypes = null, char[]? missingValues = null, int nRows = -1, int skipLines = 0)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath), "Argument should not be null.");

            if (!File.Exists(filePath))
                throw new ArgumentException(nameof(filePath), "File name does not exist.");

            using (var srdr = new StreamReader(filePath))
            {
                var csvReader = new CsvReader(srdr, sep.ToString());

                var columns = names == null ? new List<string>() : names.ToList();

                var retVal = ParseReader(csvReader, columns: ref columns, colTypes: ref colTypes,
										parseDateTime: parseDate, dateFormats: dformat, nRows: nRows, parseDate: parseDate, 
                                        missingValue: missingValues, skipLines: skipLines);

                var df = new DataFrame(retVal, columns, colTypes);

                return df;
            }
       

        }

        #endregion
        private static List<object?> ParseReader(CsvReader csvReader, ref List<string> columns, ref ColType[]? colTypes, bool parseDateTime, string? dateFormats, int nRows, bool parseDate, char[]? missingValue, int skipLines)
        {
            //Define header
            int line = 0;
            int skipIndex = 0;

            //Initialize df
            var listValues = new List<object?>();
            while (csvReader.Read())
            {
                //skip lines from parsing
                if (skipLines > skipIndex)
                {
                    skipIndex++;
                    continue;
                }

                if (nRows !=-1 && nRows < line)
                    break;

                line++;

                //add columns from file header if exists
                if (line == 1 && columns.Count == 0)
                {
                    for (int i = 0; i < csvReader.FieldsCount; i++)
                        columns.Add(csvReader[i]);
                    continue;
                }

				//check consistency of the current line in the file
				if (csvReader.FieldsCount == 1 && string.IsNullOrEmpty(csvReader[0]))//skip empty line
					continue;

				if (csvReader.FieldsCount != columns.Count)
                    throw new Exception($"The number of parsed elements at the line '{line}' is not equal to column count.");

                //resolve column types if they dont exists
                if (colTypes == null)
                {
					colTypes = new ColType[csvReader.FieldsCount];
					for (int i = 0; i < columns.Count; i++)
                    {
                        colTypes[i] = DetectType(csvReader[i], dateFormats, parseDate);
                    }
				}

                //
                for (int i = 0; i < columns.Count; i++)
                {
                    if (csvReader.FieldsCount <= i)
                    {
                        //missing values
                        listValues.Add(DataFrame.NAN);
                    }
                    else
                    {

						if (colTypes == null)
                            throw new ArgumentNullException(nameof(colTypes));

						object? val = ParseValue(csvReader[i], missingValue, colTypes[i], dateFormats);

						listValues.Add(val!);
					}
                }
            }
            //when csv is empty and hear is valid
            if (listValues.Count == 0 && columns.Count > 0)
                colTypes = columns.Select(x => ColType.STR).ToArray();

            return listValues;
        }

		internal static object ParseValue(ReadOnlySpan<char> value, char[]? missingValue, ColType colType, string? dFormat = null)
		{
			// Check if the value is a missing value
			if (IsMissingValue(value, missingChars: missingValue))
				return DataFrame.NAN;

			// Use a switch expression with fallback to original string
			return colType switch
			{
				ColType.I2 => bool.TryParse(value, out var boolResult) ? boolResult : (object)value.ToString(),
				ColType.IN or ColType.STR => value.ToString(),
				ColType.I32 => int.TryParse(value, out var intResult) ? intResult : (object)value.ToString(),
				ColType.I64 => long.TryParse(value, out var longResult) ? longResult : (object)value.ToString(),
				ColType.F32 => float.TryParse(value, out var floatResult) ? floatResult : (object)value.ToString(),
				ColType.DD => double.TryParse(value, out var doubleResult) ? doubleResult : (object)value.ToString(),
				ColType.DT => TryParseDateTime(value, dFormat, out var dateResult) ? dateResult : (object)value.ToString(),
				_ => value.ToString()
			};
		}

		private static bool TryParseDateTime(ReadOnlySpan<char> value, string? dFormat, out DateTime result)
		{
			if (string.IsNullOrEmpty(dFormat))
			{
				return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
			}

			return DateTime.TryParseExact(value, dFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
		}

		private static object ParseValue(ReadOnlySpan<char> value, char[]? missingValue, bool parseDate = false, string? dFormat = null)
        {
            //check the missing value
            if (IsMissingValue(value, missingChars: missingValue))
                return DataFrame.NAN;

            var val = IsNumeric(value);

            if (val == ValueType.Int)
            {
                int v = int.Parse(value, provider: CultureInfo.InvariantCulture);
                return v;
            }
            else if (val == ValueType.Float)
            {
                float v = float.Parse(value, provider: CultureInfo.InvariantCulture);
                return v;
            }
            else // non numeric values
            {
                if (dFormat != null && DateTime.TryParseExact(value, dFormat.AsSpan(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime pdtValue))
                {
                    return pdtValue;
                }
                else if (parseDate && DateTime.TryParse(value, out DateTime dtValue))
                {
                    return dtValue;
                }
                //else if (bool.TryParse(span, out bool bVal))
                //{
                //    llst.Add(bVal);
                //}
                else
                {
                    var strVal = new string(value.ToArray());
                    return strVal;
                }

            }

        }

		internal static bool IsMissingValue(ReadOnlySpan<char> spanValue, char[]? missingChars)
		{
			if (spanValue.Length == 0)
				return true;

			if (spanValue.Length > 1)
				return false;

			char character = spanValue[0];
			return (missingChars?.Contains(character) ?? _missingChars.Contains(character));
		}

		internal static ValueType IsNumeric(ReadOnlySpan<char> spanValue)
		{
			int pointCounter = 0;

			for (int i = 0; i < spanValue.Length; i++)
			{
				char currentChar = spanValue[i];

				// Handle sign at the beginning
				if (i == 0 && (currentChar == '-' || currentChar == '+'))
					continue;

				// Count decimal points
				if (currentChar == '.')
				{
					pointCounter++;
					continue;
				}

				// Check if the character is not numeric
				if (!char.IsDigit(currentChar))
					return ValueType.None; // Explicitly returning 'None' for invalid input
			}

			// Determine numeric type based on decimal point count
			return pointCounter switch
			{
				0 => ValueType.Int,
				1 => ValueType.Float,
				_ => ValueType.None, // More than one point is invalid
			};
		}

		private static object? ParseValue(object value, string? dformat)
		{
			if (value is string stringValue)
			{
				return ParseValue(stringValue.AsSpan(), null, true, dformat);
			}
			return value;
		}

		/// <summary>
		/// Determines the data type (ColType) for each column in the DataFrame by inspecting the first non-missing value.
		/// If no non-missing value exists, assigns a default type of ColType.STR.
		/// </summary>
		/// <returns>An array of ColType representing the type of each column.</returns>
		private ColType[] columnsTypes(List<object?> data, int rowCount, int columnCount)
		{
			var types = new ColType[columnCount];

			// Iterate through columns to determine their types
			for (int colIndex = 0; colIndex < columnCount; colIndex++)
			{
				// Find the first non-missing value in the current column
				object firstNonMissingValue = null!;
				for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
				{
					int valueIndex = rowIndex * columnCount + colIndex; // Calculate the index of the value in the flat array
					if (data[valueIndex] != DataFrame.NAN) // Check for non-missing value
					{
						firstNonMissingValue = data[valueIndex];
						break; // Stop once the first non-missing value is found
					}
				}

				// Determine the column type or assign default type
				types[colIndex] = firstNonMissingValue != null
					? GetValueType(firstNonMissingValue) // Resolve type
					: ColType.STR; // Default type
			}

			return types;
		}


		internal static ColType GetValueType(object value)
		{
			if (value == null)
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ColType DetectType(ReadOnlySpan<char> value, string? dateFormat = null, bool parseDate = true)
		{
			if (value.IsEmpty || value.IsWhiteSpace())
				return ColType.STR;

			// Only attempt date parsing if explicitly requested
			if (parseDate)
			{
				// First try exact date format if provided
				if (!string.IsNullOrEmpty(dateFormat) &&
					DateTime.TryParseExact(value, dateFormat, CultureInfo.InvariantCulture,
						DateTimeStyles.None, out _))
				{
					return ColType.DT;
				}

				// Fallback to standard date parsing if string looks date-like
				if (LooksLikeDateTime(value) &&
					DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
				{
					return ColType.DT;
				}
			}

			// Rest of the type detection (numeric types, boolean, etc.)
			return DetectNumericOrOtherType(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool LooksLikeDateTime(ReadOnlySpan<char> value)
		{
			// Fast check for common date/time separators
			bool hasDateSeparators = value.Contains('/') || value.Contains('-');
			bool hasTimeSeparators = value.Contains(':');

			return hasDateSeparators || hasTimeSeparators;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ColType DetectNumericOrOtherType(ReadOnlySpan<char> value)
		{
			// Try most specific numeric types first
			if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
				return ColType.I32;

			if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
				return ColType.I64;

			if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
				return ColType.DD;

			if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
				return ColType.DD;

			if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
				return ColType.F32;

			if (bool.TryParse(value, out _))
				return ColType.I2;

			return ColType.STR;
		}

	}
}
