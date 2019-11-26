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
using System.Buffers.Text;
using NReco.Csv;

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
        private static readonly string[] _missingCharacters = new string[] { "n/a", "?", "*", };
        #endregion


        #region Static members
        /// <summary>
        /// Saves data frame .NET object in a csv file.
        /// </summary>
        /// <param name="filePath">Full or relative file path.</param>
        /// <param name="dataFrame">Data frame to persist into file.</param>
        /// <returns>True if save successfully passed</returns>
        public static bool ToCsv(string filePath, DataFrame dataFrame)
        {
            if (dataFrame == null)
                throw new ArgumentNullException(nameof(dataFrame));

            var lst = new List<string>();
            var header = string.Join(",", dataFrame.Columns);
            lst.Add(header);

            for (int i = 0; i < dataFrame.Index.Count; i++)
            {
                var row = dataFrame[i];
                var strRow = string.Join(",", row.ToList());
                lst.Add(strRow);

            }

            File.WriteAllLines(filePath, lst);
            return true;
        }
        /// <summary>
        /// Saves data frame .NET object in a csv file.
        /// </summary>
        /// <param name="filePath">Full or relative file path.</param>
        /// <param name="dataFrame">Data frame to persist into file.</param>
        /// <returns>True if save successfully passed</returns>
        public static bool ToCsv(string filePath, DataFrame dataFrame, string dFormat)
        {
            if (dataFrame == null)
                throw new ArgumentNullException(nameof(dataFrame));

            using (var strWr = File.CreateText(filePath))
            {
                var csvWriter = new CsvWriter(strWr);
                //write header
                writeHeader(csvWriter, dataFrame.Columns);
                csvWriter.NextRecord();

                //write values
                int lstIndex = 0;
                for (int i = 0; i < dataFrame.RowCount(); i++)
                {
                    for (int j = 0; j < dataFrame.ColCount(); j++)
                    {
                        if(dataFrame._values[lstIndex]== DataFrame.NAN)
                        {
                            csvWriter.WriteField("");
                            lstIndex++;
                            continue;
                        }

                        switch (dataFrame.ColTypes[j])
                        {
                            case ColType.I2:
                                var bv = Convert.ToBoolean(dataFrame._values[lstIndex]);
                                csvWriter.WriteField(bv.ToString(CultureInfo.InvariantCulture));
                                break;
                            case ColType.IN:
                                csvWriter.WriteField(dataFrame._values[lstIndex].ToString());
                                break;
                            case ColType.I32:
                                var iv = Convert.ToInt32(dataFrame._values[lstIndex]);
                                csvWriter.WriteField(iv.ToString(CultureInfo.InvariantCulture));
                                break;
                            case ColType.I64:
                                var lv = Convert.ToInt64(dataFrame._values[lstIndex]);
                                csvWriter.WriteField(lv.ToString(CultureInfo.InvariantCulture));
                                break;
                            case ColType.F32:
                                var df = Convert.ToSingle(dataFrame._values[lstIndex]);
                                csvWriter.WriteField(df.ToString(CultureInfo.InvariantCulture));
                                break;
                            case ColType.DD:
                                var dv = Convert.ToDouble(dataFrame._values[lstIndex]);
                                csvWriter.WriteField(dv.ToString(CultureInfo.InvariantCulture));
                                break;
                            case ColType.STR:
                                csvWriter.WriteField(dataFrame._values[lstIndex].ToString());
                                break;
                            case ColType.DT:
                                var dt = Convert.ToDateTime(dataFrame._values[lstIndex]);
                                if (dFormat != null)
                                    csvWriter.WriteField(dt.ToString(dFormat));
                                else
                                    csvWriter.WriteField(dt.ToString());
                                break;
                        }

                        //
                        lstIndex++;
                    }
                    csvWriter.NextRecord();
                }
            }

            return true;
        }

        private static void writeHeader(CsvWriter csvWriter, IList<string> columns)
        {
            for (int i = 0; i < columns.Count; i++)
                csvWriter.WriteField(columns[i]);
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
            var strPath = $"web_csv_{DateTime.Now.Ticks}";
            using (System.Net.WebClient fileDownloader = new System.Net.WebClient())
            {
                fileDownloader.DownloadFile(urlPath, strPath);
            }
             
            var df =  FromCsv(strPath, sep, names, dformat, colTypes:colTypes);
            File.Delete(strPath);
            return df;
        }

        public static DataFrame FromText(string strText, char sep = ',', string[] names = null, string dformat = null, ColType[] colTypes = null, char[] missingValues=null, int nRows = -1)
        {
            if (string.IsNullOrEmpty(strText))
                throw new ArgumentNullException(nameof(strText), "Argument should not be null.");

            
            using (var csvStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(strText)))
            {
                using (var srdr = new StreamReader(csvStream))
                {
                    var csvReader = new CsvReader(srdr, sep.ToString());

                    var columns = names == null ? new List<string>() : names.ToList();

                    var retVal = parseReader(csvReader, columns: ref columns, colTypes: colTypes, dateFormats: null, nRows: nRows, parseDate: true, missingValue:missingValues);

                    var df = new DataFrame(retVal, columns);
                    if (colTypes != null)
                        df._colsType = colTypes;
                    return df;
                }
            }
        }

        public static DataFrame FromStrings(string[] strArray, char sep = ',', string[] names = null, string dformat = null, ColType[] colTypes = null, char[] missingValues = null, int nRows = -1)
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

                    var retVal = parseReader(csvReader, columns: ref columns, colTypes: colTypes, dateFormats: null, nRows: nRows, parseDate: true, missingValue: missingValues);

                    var df = new DataFrame(retVal, columns);
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
        /// <returns>Data Frame object.</returns>
        public static DataFrame FromCsv(string filePath, char sep = ',', string[] names = null, string dformat = null,  bool parseDate = true, ColType[] colTypes = null, char[] missingValues=null, int nRows = -1)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath), "Argument should not be null.");

            if (!File.Exists(filePath))
                throw new ArgumentException(nameof(filePath), "File name does not exist.");

            using (var srdr = new StreamReader(filePath))
            {
                var csvReader = new CsvReader(srdr, sep.ToString());

                var columns = names == null ? new List<string>() : names.ToList();

                var retVal = parseReader(csvReader, columns: ref columns, colTypes: colTypes, dateFormats: dformat, nRows: nRows, parseDate: parseDate, missingValue: missingValues);

                var df = new DataFrame(retVal, columns);
                if (colTypes != null)
                    df._colsType = colTypes;
                return df;
            }
                

        }

        #endregion
        private static List<object> parseReader(CsvReader csvReader, ref List<string> columns, ColType[] colTypes, string dateFormats, int nRows, bool parseDate, char[] missingValue)
        {
            //Define header
            int line = 0;

            //Initialize df
            var listValues = new List<object>();
            while (csvReader.Read())
            {
                if (nRows >= line)
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
                if (csvReader.FieldsCount == 1 && csvReader.AsSpan(0).IsEmpty)//skip empty line
                    continue;

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
                        //get value as Span
                        var span = csvReader.AsSpan(i);

                        if (colTypes != null)
                        {
                            var val = parseValue(span, missingValue, colTypes[i], dateFormats);
                            listValues.Add(val);
                        }
                        else
                        {
                            var val = parseValue(span, missingValue, parseDate, dateFormats);
                            listValues.Add(val);
                        }
                    }
                }
            }

            return listValues;
        }
        
        private static object parseValue(ReadOnlySpan<char> value, char[] missingValue, ColType colType, string dFormat = null)
        {
            //check the missing value
            if (IsMissingValue(value, missingChars: missingValue))
                return DataFrame.NAN;

#if NETSTANDARD2_1
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
#else
            var v = new Span<byte>(value.ToArray().Select(x => (byte)x).ToArray());
            switch (colType)
            {
                case ColType.I2:
                    {
                        Utf8Parser.TryParse(v,out bool bValue, out int p);
                        return bValue;
                    }
                case ColType.IN:
                    return new string(value.ToArray());
                case ColType.I32:
                    {
                        Utf8Parser.TryParse(v, out int bValue, out int p);
                        return bValue;
                    }
                case ColType.I64:
                    {
                        Utf8Parser.TryParse(v, out long bValue, out int p);
                        return bValue;
                    }
                case ColType.F32:
                    {
                        Utf8Parser.TryParse(v, out float bValue, out int p);
                        return bValue;
                    }
                case ColType.DD:
                    {
                        Utf8Parser.TryParse(v, out double bValue, out int p);
                        return bValue;
                    }
                case ColType.STR:
                    return new string(value.ToArray());
                case ColType.DT:
                    {
                        var vStr= new string(value.ToArray());
                        if (string.IsNullOrEmpty(dFormat))
                        {
                            DateTime.TryParse(vStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime bValue);
                            return bValue;
                        }      
                        else
                        {
                            DateTime.TryParseExact(vStr, dFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime bValue);
                            return bValue;
                        }
                    }
                default:
                    throw new Exception("column type is not known.");
            }
#endif
        }

        private static object parseValue(ReadOnlySpan<char> value, char[] missingValue, bool parseDate = false, string dFormat = null)
        {
            //check the missing value
            if (IsMissingValue(value, missingChars: missingValue))
                return DataFrame.NAN;

            var val = IsNumeric(value);
            #if NETSTANDARD2_1
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
#else
            var v = new Span<byte>(value.ToArray().Select(x => (byte)x).ToArray());
            if (val == ValueType.Int)
            {
                Utf8Parser.TryParse(v, out int bValue, out int p);
                return bValue;
            }
            else if (val == ValueType.Float)
            {
                Utf8Parser.TryParse(v, out float bValue, out int p);
                return bValue;
            }
            else // non numeric values
            {
                var vStr = new string(value.ToArray());
                if ((parseDate && dFormat != null) && DateTime.TryParse(vStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtValue))
                {
                    return dtValue;
                }
                else if(dFormat != null && DateTime.TryParseExact(vStr, dFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dteValue))
                {
                    return dteValue;
                }
                //else if (bool.TryParse(span, out bool bVal))
                //{
                //    llst.Add(bVal);
                //}
                else
                {
                    return vStr;
                }

            }
#endif
        }

        private static object parseValue(object value, string dformat)
        {
            if (value is string)
                return parseValue(value.ToString().AsSpan(),null, true, dformat);
            else
                return value;
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
    }
}
