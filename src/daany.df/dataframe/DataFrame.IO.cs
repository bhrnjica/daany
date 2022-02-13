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
using System.Text;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;


using Daany.MathStuff;
using System.Net.Http;
using System.Threading.Tasks;

namespace Daany
{
    /// <summary>
    /// Class implementation for DataFrame. The DataFrame is going to be C# specific implementation
    /// to handle data loading from files, grouping, sorting, filtering, handling with columns and rows
    /// accessing data frame (df) elements etc.
    /// </summary>
  
    public partial class DataFrame : IDataFrame
    {

        #region Private fields
        private static readonly char[] _missingChars = new char[] { ' ', '?', '*', };
        private static readonly string[] _missingCharacters = new string[] { "n/a", "?", "*"," " };
        #endregion
        #region Static members
        /// <summary>
        ///  Saves data frame .NET object to  csv file.
        /// </summary>
        /// <param name="filePath">Full or relative file path.</param>
        /// <param name="dataFrame">Data frame to persist.</param>
        /// <param name="delimiter">Use delimiter while writing.</param>
        /// <param name="dateFormat">Use data time  format while writing.</param>
        /// <param name="writHeader">Include heade in the file.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool ToCsv(string filePath, DataFrame dataFrame, char delimiter = ',', string dateFormat = null, bool writHeader = true)
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
                            var strValue = Convert.ToString(dataFrame._values[lstIndex], CultureInfo.InvariantCulture);
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

        public static async Task<bool> ToCsvAsync(string filePath, DataFrame dataFrame, char delimiter = ',', string dateFormat = null, bool writHeader = true)
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
                            var strValue = Convert.ToString(dataFrame._values[lstIndex], CultureInfo.InvariantCulture);
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
        public static DataFrame FromWeb(string urlPath, char sep = ',', string[] names = null, string dformat = null, ColType[] colTypes = null, int nRows = -1)
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

        public static async Task<DataFrame> FromWebAsync(string urlPath, char sep = ',', string[] names = null, string dformat = null, ColType[] colTypes = null, int nRows = -1)
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
                        string line = reader.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                            lines.Add(line);
                    }
                }
            }

            var df = FromStrings(lines.ToArray(), sep, names, dformat, colTypes);
            return df;
        }

        public static DataFrame FromText(string strText, char sep = ',', string[] names = null, string dformat = null, ColType[] colTypes = null, char[] missingValues=null, int nRows = -1, int skipLines = 0)
        {
            if (string.IsNullOrEmpty(strText))
                throw new ArgumentNullException(nameof(strText), "Argument should not be null.");

            
            using (var csvStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(strText)))
            {
                using (var srdr = new StreamReader(csvStream))
                {
                    var csvReader = new CsvReader(srdr, sep.ToString());

                    var columns = names == null ? new List<string>() : names.ToList();

                    var retVal = parseReader(csvReader, columns: ref columns, colTypes: colTypes, dateFormats: null, nRows: nRows, parseDate: true, missingValue:missingValues, skipLines: skipLines);

                    var df = new DataFrame(retVal, columns, colTypes);
                    if (colTypes != null)
                        df._colsType = colTypes;
                    return df;
                }
            }
        }

        public static DataFrame FromStrings(string[] strArray, char sep = ',', string[] names = null, string dformat = null, ColType[] colTypes = null, char[] missingValues = null, int nRows = -1, int skipLines = 0)
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

                    var retVal = parseReader(csvReader, columns: ref columns, colTypes: colTypes, dateFormats: null, nRows: nRows, parseDate: true, missingValue: missingValues, skipLines: skipLines);

                    var df = new DataFrame(retVal, columns, colTypes);
                    if (colTypes != null)
                        df._colsType = colTypes;
                    return df;
                }
            }
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
        public static DataFrame FromCsv(string filePath, char sep = ',', string[] names = null, string dformat = null, bool parseDate = true, ColType[] colTypes = null, char[] missingValues = null, int nRows = -1, int skipLines = 0)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath), "Argument should not be null.");

            if (!File.Exists(filePath))
                throw new ArgumentException(nameof(filePath), "File name does not exist.");

            using (var srdr = new StreamReader(filePath))
            {
                var csvReader = new CsvReader(srdr, sep.ToString());

                var columns = names == null ? new List<string>() : names.ToList();

                var retVal = parseReader(csvReader, columns: ref columns, colTypes: colTypes, dateFormats: dformat, nRows: nRows, parseDate: parseDate, missingValue: missingValues, skipLines: skipLines);

                var df = new DataFrame(retVal, columns, colTypes);
                if (colTypes != null)
                    df._colsType = colTypes;
                return df;
            }
                

        }

        #endregion
        private static List<object> parseReader(CsvReader csvReader, ref List<string> columns, ColType[] colTypes, string dateFormats, int nRows, bool parseDate, char[] missingValue, int skipLines)
        {
            //Define header
            int line = 0;
            int skipIndex = 0;

            //Initialize df
            var listValues = new List<object>();
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
#if NETSTANDARD2_0                           
                if (csvReader.FieldsCount == 1 && string.IsNullOrEmpty(csvReader[0]))//skip empty line
                    continue;
#else
                if (csvReader.FieldsCount == 1 && csvReader.AsSpan(0).IsEmpty)//skip empty line
                    continue;
#endif
                if (csvReader.FieldsCount != columns.Count)
                    throw new Exception($"The number of parsed elements at the line '{line}' is not equal to column count.");
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
#if NETSTANDARD2_0
                        var strValue = csvReader[i];
#else
                        //get value as Span
                        var span = csvReader.AsSpan(i);
#endif

                        if (colTypes != null)
                        {
#if NETSTANDARD2_0
                             var val = parseValue(strValue, missingValue, colTypes[i], dateFormats);
#else
                            var val = parseValue(span, missingValue, colTypes[i], dateFormats);
#endif

                            listValues.Add(val);
                        }
                        else
                        {
#if NETSTANDARD2_0
                            var val = parseValue(strValue, missingValue, parseDate, dateFormats);
#else
                            var val = parseValue(span, missingValue, parseDate, dateFormats);
#endif

                            listValues.Add(val);
                        }
                    }
                }
            }

            return listValues;
        }
#if NETSTANDARD2_0

        private static object parseValue(string strValue, char[] missingValue, ColType colType, string dFormat = null)
        {
            //check the missing value
            if (IsMissingValue(strValue, missingChars: missingValue))
                return DataFrame.NAN;

            switch (colType)
            {
                case ColType.I2:
                    return bool.Parse(strValue);
                case ColType.IN:
                    return new string(strValue.ToArray());
                case ColType.I32:
                    return int.Parse(strValue);
                case ColType.I64:
                    return long.Parse(strValue);
                case ColType.F32:
                    return float.Parse(strValue);
                case ColType.DD:
                    return double.Parse(strValue);
                case ColType.STR:
                    return new string(strValue.ToArray());
                case ColType.DT:
                    {
                        if (string.IsNullOrEmpty(dFormat))
                            return DateTime.Parse(strValue, CultureInfo.InvariantCulture, DateTimeStyles.None);
                        else
                            return DateTime.ParseExact(strValue, dFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);

                    }
                default:
                    throw new Exception("column type is not known.");
            }
        }

        private static object parseValue(string strValue, char[] missingValue, bool parseDate = false, string dFormat = null)
        {
            //check the missing value
            if (IsMissingValue(strValue, missingChars: missingValue))
                return DataFrame.NAN;

            var val = IsNumeric(strValue);

            if (val == ValueType.Int)
            {
                int v = int.Parse(strValue, provider: CultureInfo.InvariantCulture);
                return v;
            }
            else if (val == ValueType.Float)
            {
                float v = float.Parse(strValue, provider: CultureInfo.InvariantCulture);
                return v;
            }
            else // non numeric values
            {
                if (dFormat != null && DateTime.TryParseExact(strValue, dFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime pdtValue))
                {
                    return pdtValue;
                }
                else if (parseDate && DateTime.TryParse(strValue, out DateTime dtValue))
                {
                    return dtValue;
                }
                //else if (bool.TryParse(span, out bool bVal))
                //{
                //    llst.Add(bVal);
                //}
                else
                {
                    var strVal = new string(strValue.ToArray());
                    return strVal;
                }

            }

        }

        private static bool IsMissingValue(string spanValue, char[] missingChars)
        {
            if (spanValue.Length > 1)
                return false;
            else if (spanValue.Length < 1)
                return true;
            for (int i = 0; i < spanValue.Length; i++)
            {
                if (missingChars != null)
                {
                    if (missingChars.Contains(spanValue[i]))
                        return true;
                }
                else if (_missingChars.Contains(spanValue[i]))
                    return true;
            }
            return false;
        }

        private static ValueType IsNumeric(string spanValue)
        {
            int pointCounter = 0;
            for (int i = 0; i < spanValue.Length; i++)
            {
                if (i == 0 && (spanValue[i] == '-' || spanValue[i] == '+'))
                    continue;
                if (spanValue[i] == '.')
                {
                    pointCounter++;
                }
                else if (!char.IsNumber(spanValue[i]))
                {
                    return 0;
                }

            }

            if (pointCounter == 0)
                return ValueType.Int;
            else if (pointCounter == 1)
                return ValueType.Float;
            else
                return ValueType.None;
        }
  
#else
        private static object parseValue(ReadOnlySpan<char> value, char[] missingValue, ColType colType, string dFormat = null)
        {
            //check the missing value
            if (IsMissingValue(value, missingChars: missingValue))
                return DataFrame.NAN;

            switch (colType)
            {
                case ColType.I2:
                    return bool.Parse(value);
                case ColType.IN:
                    return new string(value.ToArray());
                case ColType.I32:
                    return int.Parse(value);
                case ColType.I64:
                    return long.Parse(value);
                case ColType.F32:
                    return float.Parse(value);
                case ColType.DD:
                    return double.Parse(value);
                case ColType.STR:
                    return new string(value.ToArray());
                case ColType.DT:
                    {
                        if (string.IsNullOrEmpty(dFormat))
                            return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.None);
                        else
                            return DateTime.ParseExact(value, dFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);

                    }
                default:
                    throw new Exception("column type is not known.");
            }
        }

        private static object parseValue(ReadOnlySpan<char> value, char[] missingValue, bool parseDate = false, string dFormat = null)
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

        private static bool IsMissingValue(ReadOnlySpan<char> spanValue, char[] missingChars)
        {
            if (spanValue.Length > 1)
                return false;
            else if (spanValue.Length < 1)
                return true;
            for (int i = 0; i < spanValue.Length; i++)
            {
                if (missingChars != null)
                {
                    if (missingChars.Contains(spanValue[i]))
                        return true;
                }
                else if (_missingChars.Contains(spanValue[i]))
                    return true;
            }
            return false;
        }

        private static ValueType IsNumeric(ReadOnlySpan<char> spanValue)
        {
            int pointCounter = 0;
            for (int i = 0; i < spanValue.Length; i++)
            {
                if (i == 0 && (spanValue[i] == '-' || spanValue[i] == '+'))
                    continue;
                if (spanValue[i] == '.')
                {
                    pointCounter++;
                }
                else if (!char.IsNumber(spanValue[i]))
                {
                    return 0;
                }

            }

            if (pointCounter == 0)
                return ValueType.Int;
            else if (pointCounter == 1)
                return ValueType.Float;
            else
                return ValueType.None;
        }

#endif

        private static object parseValue(object value, string dformat)
        {
            if (value is string)
#if NETSTANDARD2_0
                return parseValue(value.ToString(), null, true, dformat);
#else
                return parseValue(value.ToString().AsSpan(),null, true, dformat);
#endif
            else
                return value;
        }




       
    }
}
