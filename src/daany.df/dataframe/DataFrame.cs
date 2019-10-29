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


using Daany.MathExt;


namespace Daany
{
    /// <summary>
    /// Class implementation for DataFrame. The DataFrame is going to be C# specific implementation
    /// to handle data loading from files, grouping, sorting, filtering, handling with columns and rows
    /// accessing data frame (df) elements etc.
    /// </summary>
  
    public class DataFrame : IDataFrame
    {
        #region Properties

        /// <summary>
        /// List of columns (names) in the data frame.
        /// </summary>
        /// 
        public IList<string> Columns => _columns;


        /// <summary>
        /// Types of columns (names) in the data frame.
        /// </summary>
        /// 
        public IList<ColType> ColTypes => columnsTypes();

        /// <summary>
        /// Index for rows in the data frame.
        /// </summary>
        /// 
        public IList<object> Index => _index;

        internal IList<object> Values => _values;


        /// <summary>
        /// Representation of missing value.
        /// </summary>
        /// 
        public static object NAN => null;

        #endregion

        #region Private fields
        //private fields
        /// <summary>
        /// Data type for each data frame column.
        /// </summary>
        /// 
        private ColType[] _dfTypes;
        /// <summary>
        /// 1D element contains data frame values
        /// </summary>
        /// 
        private List<object> _values;
        private IList<object> _index;
        private IList<string> _columns;
        static readonly Regex _numFloatRegex = new Regex(@"^(((?!0)|[-+]|(?=0+\.))(\d*\.)?\d+(e\d+)?)$");
        static readonly Regex _numRegex = new Regex(@"^[0-9]+$");
        //Quick Sort algorithm. In case of false, the Merge Sort will be used.
        internal static bool qsAlgo = false;
        #endregion

        #region Enumerators
        /// <summary>
        /// Returns strongly typed row enumerator.
        /// </summary>
        /// <typeparam name="TRow"></typeparam>
        /// <param name="callBack">Dictionary of the current row.</param>
        /// <returns>Strongly type object representing the data frame row.</returns>
        public IEnumerable<TRow> GetEnumerator<TRow>(Func<IDictionary<string, object>, TRow> callBack)
        {
            var dic = new Dictionary<string, object>();
            for (int i = 0; i < Index.Count; i++)
            {
                dic.Clear();
                var row = this[i].ToList();
                for (int j=0; j< this.Columns.Count; j++)
                    dic.Add(this.Columns[j],row[j]);
  
                yield return callBack(dic);
            }
               
        }

        /// <summary>
        /// Return row enumerators by returning row as dictionary 
        /// </summary>
        /// <returns>dictionary</returns>
        public IEnumerable<IDictionary<string, object>> GetEnumerator()
        {
            var dic = new Dictionary<string, object>();
            for (int i = 0; i < Index.Count; i++)
            {
                dic.Clear();
                var row = this[i].ToList();
                for (int j = 0; j < this.Columns.Count; j++)
                    dic.Add(this.Columns[j], row[j]);

                yield return dic;
            }

        }
        /// <summary>
        /// Return row enumerators by returning object array
        /// </summary>
        /// <returns>object array</returns>
        public IEnumerable<object[]> GetRowEnumerator()
        {
            var r = new object[this._columns.Count];
            for (int i = 0; i < Index.Count; i++)
            {
                yield return this[i].ToArray();
            }

        }
        #endregion

        #region Static members
        /// <summary>
        /// Method for loading data from the file into data frame object.
        /// </summary>
        /// <param name="filePath">Full or relative path of the file.</param>
        /// <param name="sep"> Separator string.</param>
        /// <param name="names">Column names in case the columns are provided separately from the file.</param>
        /// <param name="dformat">Date time format.</param>
        /// <param name="nRows">Number of loading rows. This is handy in case we need just few rows to load in order to see how df behaves.</param>
        /// <returns>Data Frame object.</returns>
        public static DataFrame FromCsv(string filePath, char sep = ',', string[] names = null, char textQaualifier='"', string dformat = "dd/mm/yyyy", int nRows = -1)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath), "Argument should not be null.");

            if (!File.Exists(filePath))
                throw new ArgumentException(nameof(filePath), "File name does not exist.");


            var  rows = new List<string>();
            var line = ""; long lCounter = 0;
            //
            using (var sr = new StreamReader(filePath))
            {
                //
                while ((line = sr.ReadLine()) != null)
                {
                    rows.Add(line);
                    lCounter++;
                    if (nRows > -1 && lCounter >= nRows)
                        break;

                }
            }


            return FromStrArray(rows.ToArray(), sep, names, textQaualifier, dformat);
        }

        public static DataFrame FromStrArray(string[] rows, char sep = ',', string[] names = null, char textQaualifier = '"', string dformat = null)
        {
            if (rows==null)
                throw new ArgumentNullException(nameof(rows));

            //Define header
            var header = names;
            if (header == null)
                header = ParseText(rows[0], sep, textQaualifier).ToArray();

            //Initialize df
            var llst = new List<object>();
            int rowCount = 0;
            for (int i = 0; i < rows.Length; i++)
            {
                //
                if (i == 0 && names == null)
                    continue;
                var row = ParseText(rows[i], sep, textQaualifier).ToArray();

                //
                for (int j = 0; j < header.Length; j++)
                {
                    var v = "";
                    if (row.Length > j)
                        v = row[j];

                    var value = parseValue(v, dformat);
                    llst.Add(value);
                }
                rowCount++;
            }

            //create data frame
            var df = new DataFrame(llst.ToArray(), header.ToList());
            return df;
        }

        internal void SetIndex(List<object> ind)
        {
            if (ind == null)
                throw new Exception("Index cannot be null.");

            if (ind.Count != _index.Count)
                throw new Exception("Wrong count of index list.");

            this._index = ind;
        }


        /// <summary>
        /// Saves data frame .NET object in a csv file.
        /// </summary>
        /// <param name="filePath">Full or relative file path.</param>
        /// <param name="dataFrame">Data frame to persist into file.</param>
        /// <returns>True if save successfully passed</returns>
        public static bool SaveToCsv(string filePath, DataFrame dataFrame)
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
        /// Create empty data frame with specified column list.
        /// </summary>
        /// <param name="columns">Column name list.</param>
        /// <returns></returns>
        public static DataFrame CreateEmpty(IList<string> columns)
        {
            var val = Array.Empty<object>();
            var df = new DataFrame();
            df._values = new List<object>();
            df._index= new List<object>();
            df._columns = columns;
            return df;
        }
        #endregion

        #region Constructors
        private DataFrame()
        {

        }
        /// <summary>
        /// Create data frame from another data frame.
        /// </summary>
        /// <param name="dataFrame">Existing data frame.</param>
        public DataFrame(DataFrame dataFrame)
        {
            if (dataFrame == null)
                throw new Exception($"Argument '{nameof(dataFrame)}' cannot be null.");

            this._values = dataFrame._values;
            this._index = dataFrame.Index;
            this._columns = dataFrame.Columns.Select(x => (string)x).ToList();
        }


        /// <summary>
        /// Create data frame from the 1d array values, list of indexed rows and list of column names.
        /// </summary>
        /// <param name="data">1d object array of values.</param>
        /// <param name="index">Row index.</param>
        /// <param name="columns">List of column names.</param>
        [Obsolete("The constructor is obsolete and will be replaced in the next version.")]
        public DataFrame(object[] data, IList<int> index, IList<string> columns)
        {
            this._index = index.Select(x=>(object)x).ToList();
            this._columns = columns;
            this._values = data.ToList();
        }

        /// <summary>
        /// Create data frame from the 1d array values.
        /// </summary>
        /// <param name="data">1d object array of values</param>
        /// <param name="columns">List of column names.</param>
        public DataFrame(object[] data, IList<string> columns)
        {
            if (data==null)
                throw new ArgumentException(nameof(data));

            if (columns == null)
                throw new ArgumentException(nameof(columns));

            if (data.Length % columns.Count != 0)
                throw new Exception("The Columns count must be divisible by data length.");

            //calculate row count
            int rows = data.Length / columns.Count;

            this._index = Enumerable.Range(0, rows).Select(x=>(object)x).ToList();
            this._columns = columns;
            this._values = data.ToList();
        }

        /// <summary>
        /// Create data frame by list of values, row index and column names
        /// </summary>
        /// <param name="data">list of df values </param>
        /// <param name="index">row index</param>
        /// <param name="columns">column index</param>
        [Obsolete("The constructor is obsolete and will be replaced in the next version.")]
        public DataFrame(List<object> data, IList<int> index, IList<string> columns)
        {
            this._index = index.Select(x => (object)x).ToList();
            this._columns = columns;
            this._values = data;
        }


        /// <summary>
        /// Create data frame by list of values and column names.
        /// </summary>
        /// <param name="data">List of data frame values.</param>
        /// <param name="columns">List of column names.</param>
        public DataFrame(List<object> data, IList<string> columns)
        {
            if (data == null)
                throw new ArgumentException(nameof(data));

            if (columns == null)
                throw new ArgumentException(nameof(columns));

            if (data.Count % columns.Count != 0)
                throw new Exception("The Columns count must be divisible by data length.");
            //calculate row count
            int rows = data.Count / columns.Count;

            this._index = Enumerable.Range(0, rows).Select(x => (object)x).ToList();
            this._columns = columns;
            this._values = data;
        }


        /// <summary>
        /// Create data frame from dictionary.
        /// </summary>
        /// <param name="data">Data provided in dictionary collection.</param>
        public DataFrame(IDictionary<string, List<object>> data)
        {
            if (data == null)
                throw new ArgumentException(nameof(data));

            if (data == null || data.Count == 0)
                throw new Exception($"'{data}' dictionary cannot be empty!");

            //each list in directory musbe with the same count
            var counts = data.Select(x => x.Value.Count);
            var count = counts.First();
            if (!counts.All(x => x == count))
                throw new Exception("All lists within dictionary must be with same length.");

            //row column indexes preparation
            this._index = Enumerable.Range(0, data.Values.First().Count()).Select(x => (object)x).ToList();
            this._columns = data.Keys.ToList();

            //
            this._values = new List<object>();
            for (int i = 0; i < _index.Count; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    var value = data.ElementAt(j).Value[i];
                    var v = parseValue(value, null);
                    _values.Add(v);
                }

            }
        }

        #endregion

        #region Data Frame Operations
        /// <summary>
        /// Create new data frame from the existing by changing column names  
        /// </summary>
        /// <param name="colNames">List of old and new column names.</param>
        /// <returns>New data frame with renamed column names.</returns>
        public DataFrame Create(params (string oldName, string newName)[] colNames)
        {
            var oldCols = colNames.Select(x=>x.oldName).ToArray();
            var newDf = this[oldCols];
            for(int i=0; i < colNames.Length; i++)
            {
                var newName = colNames[i].oldName;
                if (!string.IsNullOrEmpty(colNames[i].newName))
                    newName = colNames[i].newName;
                //
                newDf._columns[i] = newName;
            }
            //
            return newDf;
        }

        /// <summary>
        /// Removes specified columns from the data frame.
        /// </summary>
        /// <param name="colName">List of column names to be removed.</param>
        /// <returns></returns>
        public DataFrame Remove(params string[] colName)
        {
            //
            var cols = new List<(string oldName, string newName)>();
            foreach(var c in this.Columns)
            {
                if (colName.Contains(c))
                    continue;
                cols.Add((c,null));
            }

            return Create(cols.ToArray());
        }

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

            //
            this._dfTypes = columnsTypes();
            int[] indCols = getColumnIndex(cols);
            //
            for (int i = 0; i < cols.Length; i++)
            {
                if (this._dfTypes[indCols[i]] == ColType.I2 && fOpers[indCols[i]] != FilterOperator.Equal)
                {
                    throw new Exception("Boolean column and filterValue  must be connected with only 'Equal' operator.");
                }
            }

            //temp row values
            int rowIndex = 0;
            object[] rowValues = new object[cols.Length];

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
                        for (int j = 0; j < Columns.Count; j++)
                            lst.Add(_values[rowIndex + j]);
                    }

                }
            }
            var df = new DataFrame(lst.ToArray(), Columns);
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
        /// Join two df with Inner or Left join type.
        /// </summary>
        /// <param name="df2">Right data frame</param>
        /// <param name="leftOn">Join columns from the left df</param>
        /// <param name="rightOn">Join columns from the right df.</param>
        /// <param name="jType">Join types. It can be Inner or Left. In case of right join call join from the second df.</param>
        /// <param name="sortedDataFrames">Optimization provides the right df starts from the last index of the previously joined row </param>
        /// <returns>New joined df.</returns>
        public DataFrame Join(DataFrame df2, string[] leftOn, string[] rightOn, JoinType jType, bool sortedDataFrames = false)
        {
            if (df2 == null)
                throw new ArgumentException(nameof(df2));

            if (leftOn == null)
                throw new ArgumentException(nameof(leftOn));


            if (rightOn == null)
                throw new ArgumentException(nameof(rightOn));

            if (leftOn.Length != rightOn.Length)
                throw new Exception("Join column numbers are different!");

            _dfTypes = columnsTypes();
            //get column indexes
            var leftInd = getColumnIndex(leftOn);
            var rightInd = df2.getColumnIndex(rightOn);

            //merge columns
            var tot = Columns.ToList();
            tot.AddRange(df2.Columns);

            var totalColumns = new List<string>();
            var totCount = tot.Count();
            for (int i = 0; i < totCount; i++)
            {
                var strVal = tot.ElementAt(i).ToString(CultureInfo.InvariantCulture);
                //
                addNewColumnName(totalColumns, strVal);
            }

            var lst = new List<object>();
            var leftRCount = _index.Count;
            var leftCCount =  ColCount();
            var rightRCount = df2.RowCount();
            var rightCCount = df2.ColCount();
            var lastIndex = 0;

            //left df enumeration
            for (int i = 0; i < leftRCount; i++)
            {
                if (jType == JoinType.Left)
                {
                    int startL = i * leftCCount;
                    for (int r = startL; r < startL + leftCCount; r++)
                        lst.Add(_values[r]);
                }

                //right df enumeration
                var notFound = true;
                for (int j = lastIndex; j < rightRCount; j++)
                {
                    //
                    if (isEqual(df2, leftInd, rightInd, i, j))
                    {
                        if (jType == JoinType.Inner)
                        {
                            int startL = i * leftCCount;
                            for (int r = startL; r < startL + leftCCount; r++)
                                lst.Add(_values[r]);
                        }
                        //
                        int startR = j * rightCCount;
                        for (int r = startR; r < startR + rightCCount; r++)
                            lst.Add(df2._values[r]);

                        //when dfs are sorted, next row of the right df should not start from zero
                        if (sortedDataFrames)
                            lastIndex = j + 1;
                        //
                        notFound = false;
                        break;
                    }
                }
                //in case of Left join and no right data found
                // fill with NAN numbers
                if (jType == JoinType.Left && notFound)
                {
                    for (int r = 0; r < rightCCount; r++)
                        lst.Add(DataFrame.NAN);
                }
            }
            //Now construct the Data frame
            var newDf = new DataFrame(lst, totalColumns);
            return newDf;

        }

        /// <summary>
        /// Join two df with Inner or Left join type.
        /// </summary>
        /// <param name="df2">Right data frame</param>
        /// <param name="leftOn">Join columns from the left df</param>
        /// <param name="rightOn">Join columns from the right df.</param>
        /// <param name="jType">Join types. It can be Inner or Left. In case of right join call join from the second df.</param>
        /// <param name="sortedDataFrames">Optimization provides the right df starts from the last index of the previously joined row </param>
        /// <returns>New joined df.</returns>
        internal DataFrame Join1(DataFrame df2, string[] leftOn, string[] rightOn, JoinType jType)
        {
            if (df2 == null)
                throw new ArgumentException(nameof(df2));

            if (leftOn == null)
                throw new ArgumentException(nameof(leftOn));


            if (rightOn == null)
                throw new ArgumentException(nameof(rightOn));

            if (leftOn.Length != rightOn.Length)
                throw new Exception("Join column numbers are different!");

            if (leftOn.Length > 3)
                throw new Exception("Join can be performed with less than 4 columns.");

            Dictionary<object, int> dic1 = null;
            TwoKeysDictionary<object, object, int> dic2 = null;
            ThreeKeysDictionary<object, object, object, int> dic3 = null;

            if(leftOn.Length==1)
                dic1 = df2.getIndex(rightOn[0]);
            else if(leftOn.Length==2)
                dic2 = df2.getIndex(rightOn[0], rightOn[1]);
            else if (leftOn.Length==3)
                dic3 = df2.getIndex(rightOn[0], rightOn[1], rightOn[2]);

           
            //get column indexes
            var leftInd = getColumnIndex(leftOn);
            var rightInd = df2.getColumnIndex(rightOn);

            //merge columns
            var tot = Columns.ToList();
            tot.AddRange(df2.Columns);

            var totalColumns = new List<string>();
            var totCount = tot.Count();
            for (int i = 0; i < totCount; i++)
            {
                var strVal = tot.ElementAt(i).ToString(CultureInfo.InvariantCulture);
                //
                addNewColumnName(totalColumns, strVal);
            }

            var lst = new List<object>();
            var leftRCount = _index.Count;
            var leftCCount = ColCount();
            var rightRCount = df2.RowCount();
            var rightCCount = df2.ColCount();

            //left df enumeration
            for (int i = 0; i < leftRCount; i++)
            {
                if (jType == JoinType.Left)
                {
                    int startL = i * leftCCount;
                    for (int r = startL; r < startL + leftCCount; r++)
                        lst.Add(_values[r]);
                }

                int j = containsKeys(dic1, dic2, dic3, i, leftInd);
                if (j >= 0)
                {
                    if (jType == JoinType.Inner)
                    {
                        int startL = i * leftCCount;
                        for (int r = startL; r < startL + leftCCount; r++)
                            lst.Add(_values[r]);
                    }
                    //
                    int startR = j * rightCCount;
                    for (int r = startR; r < startR + rightCCount; r++)
                        lst.Add(df2._values[r]);
                }
                else
                {
                    for (int r = 0; r < rightCCount; r++)
                        lst.Add(DataFrame.NAN);
                }
            }
            //Now construct the Data frame
            var newDf = new DataFrame(lst, totalColumns);
            return newDf;

        }

        private int containsKeys(IDictionary<object, int> dic1, TwoKeysDictionary<object,object, int> dic2, ThreeKeysDictionary<object,object, object, int> dic3,int i, int[] leftInd)
        {
            if (leftInd.Length == 1)
            {
                var val = this[i, leftInd[0]];
                if (dic1.ContainsKey(val))
                    return dic1[val];
            }
            else if (leftInd.Length == 2)
            {
                var val1 = this[i, leftInd[0]];
                var val2 = this[i, leftInd[1]];
                if (dic2.ContainsKey(val1, val2))
                    return dic2[val1][val2];
            }
            else if (leftInd.Length == 3)
            {
                var val1 = this[i, leftInd[0]];
                var val2 = this[i, leftInd[1]];
                var val3 = this[i, leftInd[2]];
                if (dic3.ContainsKey(val1, val2, val3))
                    return dic3[val1][val2][val3];
            }
            else
                throw new Exception("Not supported keys");

            return -1;
        }

        private Dictionary<object, int> getIndex(string colName)
        {
            var colInd = this.getColumnIndex(colName);
            //make  indexes for right df.
            var dict = new Dictionary<object, int>();
            for (int i = 0; i < this.RowCount(); i++)
            {
                var key = this[i, colInd];
                
                dict.Add(key,i);
            }

            return dict;
        }
        private TwoKeysDictionary<object, object, int> getIndex(string colName1, string colName2)
        {
            var colInd = this.getColumnIndex(new string[] { colName1, colName2 });
            //
            //make  indexes for right df.
            var dict = new TwoKeysDictionary<object,object, int>();
            for (int i = 0; i < this.RowCount(); i++)
            {
                var key1 = this[i, colInd[0]];
                var key2 = this[i, colInd[1]];

                dict.Add(key1,key2, i);
            }

            return dict;
        }
        private ThreeKeysDictionary<object, object, object, int> getIndex(string colName1, string colName2, string colName3)
        {
            var colInd = this.getColumnIndex(new string[] { colName1, colName2, colName3 });
            //
            //make  indexes for right df.
            var dict = new ThreeKeysDictionary<object, object, object, int>();
            for (int i = 0; i < this.RowCount(); i++)
            {
                var key1 = this[i, colInd[0]];
                var key2 = this[i, colInd[1]];
                var key3 = this[i, colInd[2]];

                dict.Add(key1, key2, key3, i);
            }

            return dict;
        }

        /// <summary>
        /// Sorts data-frame by specified column in ascending order
        /// </summary>
        /// <param name="cols">Sorting columns</param>
        /// <returns>New ordered df.</returns>
        public DataFrame SortBy(params string[] cols)
        {
            
            //determine column types
            _dfTypes = columnsTypes();
            var colInd = getColumnIndex(cols);
            //save
            var sdf = new SortDataFrame(colInd, _dfTypes);
            List<object> sortedList;
            if (qsAlgo)
                sortedList = sdf.QuickSort(_values, colInd);
            else
                sortedList = sdf.MergeSort(_values, colInd);

            //create a new df with sorted values 
            var df = new DataFrame(sortedList, Columns.ToList());
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
            return df;
        }

        
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
        public void FillNA(string col, Func<object> replDelg)
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
                            _values[index] = replDelg();

                    }
                    index++;
                }
            }
        }

        
        /// <summary>
        /// Add additional column into DataFrame. The values of the columns are 
        /// calculated by calling Func delegate for each row.
        /// </summary>
        /// <param name="colName">New calculated column name.</param>
        /// <param name="callBack">Func delegate for row value calculation.</param>
        /// <returns>True if calculated column is created successfully</returns>
        [Obsolete("This method is obsolute and will be replaced in the future. Use 'AddCalculatedColumnc' instead.")]
        public bool AddCalculatedColumn(string colName, Func<IDictionary<string,object>, int, object> callBack)
        {
            //
            if (this._columns.Contains(colName))
               throw new Exception($"Column '{colName}' already exists in the Data Frame.");

            //add new column
            addNewColumnName(this.Columns, colName);
            //
            var vals = new List<object>();
            int oldInd = 0;
            //
            for (int i = 0; i < _index.Count; i++)
            {
                //define processing row before adding column
                var processingRow = new Dictionary<string, object>();
                for (int j = 0; j < this.Columns.Count; j++)
                {

                    if (j + 1 >= Columns.Count)
                    {
                        var v = callBack(processingRow, i);
                        vals.Add(v);
                    }
                    else
                    {
                        var value = _values[oldInd++];
                        processingRow.Add(this.Columns[j],value);
                        vals.Add(value);
                    }
                }
            }
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
        public bool AddCalculatedColumns(string[] colNames, Func<IDictionary<string, object>, int, object[]> callBack)
        {
            if (colNames == null || colNames.Length == 0)
                throw new Exception("column names are not defined properly.");

            //check if column exists
            foreach(var col in colNames)
            {
                if (this._columns.Contains(col))
                    throw new Exception($"Column '{col}' already exists in the Data Frame.");

            }
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
                        foreach (var v in vs)
                            vals.Add(v);
                    }
                    else
                    {
                        var value = _values[oldInd++];
                        processingRow[this.Columns[j]]= value;
                        vals.Add(value);
                    }
                }

            }
            //add new columns
            foreach (var colName in colNames)
                addNewColumnName(this.Columns, colName);
            //apply new dataframe values
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
            foreach (var col in colNames)
            {
                if (this._columns.Contains(col))
                    throw new Exception($"Column '{col}' already exists in the Data Frame.");

            }
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
            foreach (var colName in colNames)
                addNewColumnName(this.Columns, colName);

            this._values = vals;
            return true;
        }

        /// <summary>
        /// Removes rows with missing values for specified set of columns. In case cols is null, removed values 
        /// will be applied to all columns.
        /// </summary>
        /// <param name="cols">List of columns</param>
        /// <returns>New df with fixed NAN</returns>
        public DataFrame DropNA(params string[] cols)
        {
            return RemoveRows((r, i) =>
            {
                for (int j = 0; j < r.Length; j++)
                {
                    if (r[j] == NAN)
                        return true;
                }
                return false;
            });

        }

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
                }
            }
            //create new df
            var df = new DataFrame(vals, this._columns.ToList());
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
                }
            }
            //create new df
            var df = new DataFrame(vals, this._columns.ToList());
            return df;
        }

        /// <summary>
        /// Apply set of operations on existing column in the DataFrame. The values of the column are 
        /// calculated by calling Func delegate for each row.
        /// </summary>
        /// <param name="colName">Existing column in the data frame.</param>
        /// <param name="callBack">Func delegate for wor value calculation.</param>
        /// <returns>True if calculated column is created/updated successfully</returns>
        public bool Apply(string colName, Func<IDictionary<string, object>, int, object> callBack)
        {
            if (!Columns.Contains(colName))
                throw new Exception($"'{colName}' does not exist in the data frame.");

            //define processing row before adding column
            var processingRow = new Dictionary<string, object>();
            for (int j = 0; j < this.Columns.Count; j++)
                processingRow.Add(this.Columns[j], null);
            //
            var colIndex = getColumnIndex(colName);

            //
            for (int i = 0; i < _index.Count; i++)
            {
                rowToDictionary(processingRow, i);
                //once the processing row is initialized perform apply 
                var v = callBack(processingRow, i);
                var applyIndex = calculateIndex(i, colIndex);// i * Columns.Count + colIndex;
                _values[applyIndex] = v;

            }

            return true;
        }


        /// <summary>
        /// Apply set of operations on existing column in the DataFrame. The values of the column are 
        /// calculated by calling Func delegate for each row.
        /// </summary>
        /// <param name="colName">Existing column in the data frame.</param>
        /// <param name="callBack">Func delegate for row value calculation.</param>
        /// <returns>True if calculated column is updated successfully</returns>
        public bool Apply(string colName, Func<object[], int, object> callBack)
        {
            //if column doesnt existi add new calculated column
            if (!Columns.Contains(colName))
                throw new Exception($"'{colName}' does not exist in the data frame.");

            //define processing row before adding column
            var processingRow = new object[ColCount()];
            //
            var colIndex = getColumnIndex(colName);

            //
            for (int i = 0; i < _index.Count; i++)
            {
                rowToArray(processingRow, i);
                //once the processing row is initialized perform apply 
                var v = callBack(processingRow, i);
                var applyIndex = calculateIndex(i, colIndex);
                _values[applyIndex] = v;

            }
            return true;
        }

        ///// <summary>
        ///// Rolling method for performing various operation.
        ///// </summary>
        ///// <param name="indexColumn"></param>
        ///// <param name="window">Rolling window size</param>
        ///// <param name="agg">Aggregated operation.</param>
        ///// <returns>New df with computed rolling operations</returns>
        //public DataFrame Rolling(string indexColumn, int window, Aggregation agg)
        //{
        //    var aggD = new Dictionary<string, Aggregation>();
        //    foreach (var col in Columns.Where(x => indexColumn != x))
        //        aggD.Add(col, agg);
        //    //
        //    return Rolling(new string[] { indexColumn}, window, aggD);
        //}

        /// <summary>
        /// Perform aggregate operation of list of columns, the rest of the column will be ignored and takes the last element
        /// </summary>
        /// <param name="indCols">indexes of the columns</param>
        /// <param name="agg"></param>
        /// <returns></returns>
        //public List<object> Aggregations(List<string> indCols, Aggregation agg)
        //{
        //    if (indCols == null || indCols.Count == 0)
        //        throw new Exception("'indCols' argument cannot be null");

        //    var aggValues = new List<object>();
        //    for (int i = 0; i < Columns.Count; i++)
        //    {
        //        if (indCols.Contains(Columns[i]))//grouped columns just skip and take the last one
        //            aggValues.Add(this[Columns[i]].Last());
        //        else
        //        {
        //            var ag = calculateAggregation(this[Columns[i]], agg);
        //            aggValues.Add(ag);
        //        }
        //    }
        //    return aggValues;
        //}

        //public List<object> Aggregations(string[] indCols, Aggregation agg)
        //{
        //    return Aggregations(indCols.ToList(), agg);
        //}
        /// <summary>
        /// Perform aggregate operation on the list of columns. For incomplete list, the rest of the column will be ignored and takes the last element
        /// </summary>
        /// <param name="indCols">indexes of the columns</param>
        /// <param name="agg"></param>
        /// <returns></returns>
        public List<object> Aggregations(IDictionary<string, Aggregation> aggs)
        {
            if (aggs == null)
                throw new Exception("List of columns or list of aggregation cannot be null.");

            var aggValues = new List<object>();
            for (int i = 0; i < Columns.Count; i++)
            {
                if (!aggs.ContainsKey(Columns[i])) 
                    aggValues.Add(this[Columns[i]].Last());
                else
                {
                    var ag = calculateAggregation(this[Columns[i]], aggs[Columns[i]]);
                    aggValues.Add(ag);
                }
            }
            return aggValues;
        }

        /// <summary>
        /// Create new dataFrame containing rolling values of specified columns of the data frame
        /// </summary>
        /// <param name="window">rolling width</param>
        /// <param name="agg">key value pair of column and its aggregate operation.</param>
        /// <returns></returns>
        public DataFrame Rolling(int window, Dictionary<string, Aggregation> agg)
        {
            string[] indexColumn = this._columns.Where(x => !agg.Keys.Contains(x)).Select(x => x).ToArray();
            if (agg == null || agg.Count == 0)
                throw new Exception($"Aggregation is empty.");

            int index = 0;
            int rolIndex = 1;
            var rRolls = new Dictionary<string, Queue<object>>();
            var aggrValues = new Dictionary<string, List<object>>();
            for (int i = 0; i < _index.Count; i++)
            {
                for (int j = 0; j < ColCount(); j++)
                {
                    var colValue = Columns[j];
                    if (agg.ContainsKey(colValue))
                    {
                        if (i == 0)
                        {
                            //add coll used for rolling operation
                            rRolls.Add(colValue, new Queue<object>(window));
                            //add calculated rolling column
                            aggrValues.Add(colValue, new List<object>());
                        }

                        //add value to rolling list
                        rRolls[colValue].Enqueue(_values[index]);

                        //rolling aggregation calculation
                        if (i + 1 < window)
                        {
                            aggrValues[colValue].Add(NAN);
                        }
                        else
                        {
                            var vals = rRolls[colValue].Select(x => x);
                            var value = calculateAggregation(vals, agg[colValue]);
                            aggrValues[colValue].Add(value);

                            //remove the last one, so the next item can be add to the first position
                            rRolls[colValue].Dequeue();
                        }
                    }

                    //reset rolling index when exceed the window value
                    if (rolIndex > window)
                        rolIndex = 1;
                    //
                    index++;
                }
            }

            //add index column to rolledValues
            var newColList = new List<string>();
            DataFrame dff=null;
            foreach (var colName in this._columns)
            {
                if (agg.Keys.Contains(colName))
                {
                    if (dff == null)
                        dff = new DataFrame(aggrValues[colName], new List<string> { colName });
                    else
                        dff.InsertColumn(colName, aggrValues[colName], -1);
                }
                else
                {
                    if (dff == null)
                        dff = new DataFrame(this[colName].ToList(), new List<string> { colName });
                    else
                        dff.InsertColumn(colName, this[colName].ToList(), -1);
                }

            }
            //
            return dff;
        }


        /// <summary>
        /// Returns data frame consisted of every nth row
        /// </summary>
        /// <param name="nthRow"></param>
        /// <returns></returns>
        public DataFrame TakeEvery(int nthRow)
        {
            var val = new List<object>();
            //go through all rows
            var index = 0;
            for (int i = 0; i < _index.Count; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    //take every nthRow
                    if ((i + 1) % nthRow == 0)
                        val.Add(_values[index]);

                    //increase index
                    index++;
                }

            }
            //
            var df = new DataFrame(val.ToArray(), Columns.ToArray());

            //
            return df;
        }

        /// <summary>
        /// Returns data frame consisted of randomly selected n rows.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public DataFrame TakeRandom(int rows)
        {
            var selected = new List<int>();
            double needed = rows;
            double available = _index.Count;
            var rand = new Random();
            while (selected.Count < rows)
            {
                if (rand.NextDouble() < needed / available)
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
        /// Returns data frame consisted of last n rows.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public DataFrame TakeLast(int rows)
        {
            var val = new List<object>();
            int counter = 1;
            for (int i = _index.Count-1; i >=0 ; i--)
            {
                val.InsertRange(0,this[i]);
                if (counter >= rows)
                    break;
                counter++;
            }
            //
            var df = new DataFrame(val.ToArray(), Columns.ToArray());

            //
            return df;
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
            var df = new DataFrame(val.ToArray(), Columns.ToArray());
            df._index = ind;
            //
            return df;
        }

        /// <summary>
        /// Create GroupedDataFrame
        /// </summary>
        /// <param name="groupCol"></param>
        /// <returns></returns>
        public GroupDataFrame GroupBy(string groupCol)
        {
            var Group = groupDFBy(groupCol);

            return new GroupDataFrame(groupCol, Group);
        }

        /// <summary>
        /// Grouping with two columns
        /// </summary>
        /// <param name="groupCols"></param>
        /// <returns></returns>
        public GroupDataFrame GroupBy(params string[] groupCols)
        {
            if (groupCols == null || groupCols.Length == 0)
                throw new Exception("Group columns cannot be null or empty.");
            if (groupCols.Length > 3)
                throw new Exception("Grouping with more than three group columns is not supported.");
            //grouping
            if (groupCols.Length == 1)
                return GroupBy(groupCols[0]);
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
                    var df2 = gp2.Groups[g.key1][g.key2];
                    var group3 = df2.groupDFBy(groupCols[2]);
                    foreach (var g1 in group3)
                        grp.Add(g.key1, g.key2, g1.Key, g1.Value);
                }

                return new GroupDataFrame(groupCols[0], groupCols[1], groupCols[2], grp);
            }

        }

        /// <summary>
        /// Add new columns int dataframe
        /// </summary>
        /// <param name="cols"></param>
        public void AddColumns(Dictionary<string, List<object>> cols)
        {
            if (RowCount() != cols.ElementAt(0).Value.Count)
                throw new Exception("Row counts must be equal.");
            //
            int index = 0;
            var vals = new List<object>();
            for (int i = 0; i < _index.Count; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    vals.Add(_values[index]);
                    //
                    index++;
                }
                for (int k = 0; k < cols.Keys.Count; k++)
                    vals.Add(cols.ElementAt(k).Value[i]);
            }
            //
            this._values = vals;
            this._columns = Columns.Union(cols.Keys).ToList();
        }

        /// <summary>
        /// Insert new columns at specific position
        /// </summary>
        /// <param name="cName">new Column name.</param>
        /// <param name="nPos">Zero based index position of the new column. -1 insert the column at last position.</param>
        /// <param name="value">column value</param>
        public void InsertColumn(string cName, List<object> value, int nPos =-1)
        {
            if (nPos == -1)
                nPos = this._columns.Count;

            if (this._columns.Contains(cName))
                throw new Exception($"{cName} already exists.");
            if(nPos < -1 && nPos >= ColCount())
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
                    if(j==nPos && nPos < this._columns.Count)
                    {
                        vals.Add(value[i]);

                    }
                    else if(j==nPos && (nPos==this._columns.Count))
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
            //
            this._values = vals;
            if(nPos== this._columns.Count)
                this._columns.Add(cName);
            else
                this._columns.Insert(nPos,cName);
        }

        /// <summary>
        /// Add new rows at the end of the df.
        /// </summary>
        /// <param name="df">Add df at the end. It must be the same shape and types as the first df.</param>
        public void AddRows(DataFrame df)
        {
            if (Columns.Count != df.Columns.Count)
                throw new Exception("Data frames are not consisted!");

            // 
            foreach (var v in df._values)
                _values.Add(v);
            this._index = Enumerable.Range(0, Index.Count + df.Index.Count).Select(x=>(object)x).ToList();
        }

        /// <summary>
        /// Add one row in the df.
        /// </summary>
        /// <param name="row">List of row values</param>
        public void AddRow(List<object> row)
        {
            if (row == null || row.Count != Columns.Count)
                throw new Exception("Inconsistent row, and cannot be inserted in the DataFrame");

            //add data to df
            foreach (var v in row)
                _values.Add(v);
            //add row index
            _index.Add(_index.Count);
        }
        #endregion

        #region Indexers
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
                var df = new DataFrame(lst.ToArray(),cols);
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
            long numR = Math.Min(rows, count);
            for (int i = 0; i < numR; i++)
            {
                IList<object> row = this[i].ToList();
                lst.AddRange(this[i].ToList());
            }
            return new DataFrame(lst, Columns.ToArray());
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
            int numR = Math.Min(rows, count);
            for (int i = rows - numR; i < rows; i++)
            {
                lst.AddRange(this[i].ToList());
            }
            //
            return new DataFrame(lst,Columns.ToArray());
        }

        public string ToStringBuilder(int rowCount=10)
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

        /// <summary>
        /// Prints basic descriptive statistics values of the data frame
        /// </summary>
        /// <param name="numericOnly">Include only numeric columns.</param>
        /// <param name="inclColumns">Specified columns</param>
        /// <returns></returns>
        public DataFrame Describe(bool numericOnly = true, params string[] inclColumns)
        {
            var types = this.columnsTypes();
            var lstCols = new List<(string cName, ColType cType)>();
            var idxs = getColumnIndex(inclColumns);

            //include columns
            if (inclColumns == null || inclColumns.Length==0)
            {
                for (int i = 0; i < this.Columns.Count(); i++)
                {
                    lstCols.Add((this.Columns[i], types[i]));
                }
            }
            else
            {
                for (int i = 0; i < idxs.Length; i++)
                {
                    var c = this.Columns[idxs[i]];
                    lstCols.Add((c, types[idxs[i]]));
                }
            }

            //only numeric columns
            var finalCols = new List<(string cName, ColType cType)>();
            for (int i = 0; i < lstCols.Count(); i++)
            {
                if(numericOnly)
                {
                    if (isNumeric(lstCols[i].cType))
                        finalCols.Add(lstCols[i]);
                }
                else //if(!isNumeric(lstCols[i].cType) && !numericOnly)
                    finalCols.Add(lstCols[i]);
            }

            DataFrame dfDescr = describeColumns(finalCols);
            //
            return dfDescr;
        }



        #endregion

        #region Private
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

        private DataFrame describeColumns(List<(string cName, ColType cType)> columns)
        {
            var dicDf = new Dictionary<string, List<object>>();
            // 
            var dicColumn = new Dictionary<string, object>();
            dicColumn.Add("count",    null); 
            dicColumn.Add("unique",   null);
            dicColumn.Add("top",      null);
            dicColumn.Add("mode",     null); 
            dicColumn.Add("mean",     null); 
            dicColumn.Add("std",      null);
            dicColumn.Add("min",      null); 
            dicColumn.Add("25%",      null); 
            dicColumn.Add("50%",      null); 
            dicColumn.Add("75%",      null);
            dicColumn.Add("max",      null);
            dicDf.Add("count", new List<object>());
            dicDf.Add("unique", new List<object>());
            dicDf.Add("top", new List<object>());
            dicDf.Add("mode", new List<object>());
            dicDf.Add("mean", new List<object>());
            dicDf.Add("std", new List<object>());
            dicDf.Add("min", new List<object>());
            dicDf.Add("25%", new List<object>());
            dicDf.Add("50%", new List<object>());
            dicDf.Add("75%", new List<object>());
            dicDf.Add("max", new List<object>());
            //
            for (int i=0; i < columns.Count(); i++)
            {
                if (isNumeric(columns[i].cType))
                {
                    //exclude null values
                    var val = this[columns[i].cName].Where(x=> x!=null).Select(x => Convert.ToDouble(x)).ToArray();
                    describeColumn(dicColumn, val);
                    dicColumn["count"] = this[columns[i].cName].Count();
                }
                else if(isCategorical(columns[i].cType))
                {
                    //exclude null values
                    var val = this[columns[i].cName].Where(x => x != null).Select(x => x).ToArray();
                    describeCatColumn(dicColumn, val);
                    dicColumn["count"] = this[columns[i].cName].Count();//count with missing values
                }
                else if (isDateTime(columns[i].cType))
                {
                    var val = this[columns[i].cName].Where(x => x != null).Select(x => Convert.ToDateTime(x)).ToArray();
                    describeDTColumn(dicColumn, val);
                    dicColumn["count"] = this[columns[i].cName].Count();//count with missing values
                }
                else if(isObject(columns[i].cType))
                {
                    var val = this[columns[i].cName].Where(x => x != null).Select(x => x).ToArray();
                    describeObjColumn(dicColumn, val);
                    dicColumn["count"] = this[columns[i].cName].Count();//count with missing values
                }
                else
                    throw new Exception("Unknown column type.");
                //add calculated values to df dictionary
                foreach (var d in dicColumn)
                    dicDf[d.Key].Add(d.Value);
            }

            //constuct the data frame
           var lst = new List<object>();
           var index = new List<object>();
           foreach(var v in dicDf)
            {
                var l = dicDf[v.Key];
                //check if some row is null
                if(l.Any(d=> d!=null))
                {
                    index.Add(v.Key);
                    for (int i = 0; i < l.Count; i++)
                    {
                        lst.Add(l[i]==null?float.NaN:l[i]);
                    }
                }
                
            }

            //
            var lstCols = new List<string>();
            lstCols.AddRange(columns.Select(c=>c.cName));

           //cretae data frame
           var df = new DataFrame();
           df._values = lst;
           df._index = index;
           df._columns = lstCols;
           return df;

        }
 
        private void describeColumn(Dictionary<string, object> dic, double[] colValue)
        {
            //
            dic["unique"]   = null;
            dic["top"]      = null;
            dic["mode"]     = null;
            dic["mean"]     = Math.Round(colValue.MeanOf(),6);
            dic["std"]      = Math.Round(colValue.Stdev(),6);
            dic["min"]      = Math.Round(colValue.Min(),6);
            dic["25%"]      = Math.Round(colValue.Percentile(25), 6);
            dic["50%"]      = Math.Round(colValue.MedianOf(),6);
            dic["75%"]      = Math.Round(colValue.Percentile(75), 6);
            dic["max"]      = Math.Round(colValue.Max(),6);
            return;
        }
        
        static private void describeCatColumn(Dictionary<string, object> dic, object[] colValue)
        {
            //
            //dic["count"]    = colValue.Length;
            dic["unique"]   = Math.Round((double)colValue.Distinct().Count(), 6); 
            dic["top"]      = colValue.First();
            dic["mode"]     = colValue.ModeOf();
            dic["mean"]     = null;
            dic["std"]  = null;
            dic["min"]  = null;
            dic["25%"]  = null;
            dic["50%"]  = null;
            dic["75%"]  = null;
            dic["max"]  = null;
            return;
        }

        static private void describeObjColumn(Dictionary<string, object> dic, object[] colValue)
        {
            //
            //dic["count"] = colValue.Length;
            dic["unique"] = Math.Round((double)colValue.Distinct().Count(), 6);
            dic["top"] = colValue.First();
            dic["mode"] = colValue.ModeOf();
            dic["mean"] = null;
            dic["std"] = null;
            dic["min"] = null;
            dic["25%"] = null;
            dic["50%"] = null;
            dic["75%"] = null;
            dic["max"] = null;
            return;
        }

        static private void describeDTColumn(Dictionary<string, object> dic, DateTime[] colValue)
        {
            //
            //dic["count"] = colValue.Length;
            dic["unique"] = Math.Round((double)colValue.Distinct().Count(), 6);
            dic["top"] = colValue.First();
            dic["mode"] = colValue.ModeOf();
            dic["mean"] = null;
            dic["std"] = null;
            dic["min"] = colValue.Min();
            dic["25%"] = null;
            dic["50%"] = null;
            dic["75%"] = null;
            dic["max"] = colValue.Max();
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
            var lst = new List<object>();
            for(int i= this.Index.Count-1; i >=0 ; i--)
            {
                for (int j = 0; j < this.Columns.Count; j++)
                {
                    var v = this[i, j];
                    lst.Add(v);
                }
            }
            var dff = new DataFrame(lst, cols);
            return dff;
        }

        private DataFrame getDataFramesRows(List<int> selected)
        {
            var val = new List<object>();
            //go through selected rows
            for (int i = 0; i < selected.Count; i++)
            {
                var row = this[selected[i]];
                val.AddRange(row);
            }
            //
            var df = new DataFrame(val.ToArray(), Columns.ToArray());
            return df;
        }

        //https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
        private static IEnumerable<string> ParseText(string line, char delimiter, char textQualifier)
        {

            if (line == null)
                yield break;

            else
            {
                char prevChar;
                char nextChar;
                char currentChar;

                bool inString = false;

                StringBuilder token = new StringBuilder();

                for (int i = 0; i < line.Length; i++)
                {
                    currentChar = line[i];

                    if (i > 0)
                        prevChar = line[i - 1];
                    else
                        prevChar = '\0';

                    if (i + 1 < line.Length)
                        nextChar = line[i + 1];
                    else
                        nextChar = '\0';

                    if (currentChar == textQualifier && (prevChar == '\0' || prevChar == delimiter) && !inString)
                    {
                        inString = true;
                        continue;
                    }

                    if (currentChar == textQualifier && (nextChar == '\0' || nextChar == delimiter) && inString)
                    {
                        inString = false;
                        continue;
                    }

                    if (currentChar == delimiter && !inString)
                    {
                        yield return token.ToString();
                        token = token.Remove(0, token.Length);
                        continue;
                    }

                    token = token.Append(currentChar);

                }

                yield return token.ToString();

            }
        }
        private Dictionary<object, DataFrame> groupDFBy(string groupCol)
        {
            var Group = new Dictionary<object, DataFrame>();
            
            //go through all data to group
            var index = 0;
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
                    row.Add(_values[index]);
                   // if (Columns[j].Equals(groupCol, StringComparison.InvariantCultureIgnoreCase))
                   if(grpColIndex==j)
                        groupValue = _values[index];
                    index++;
                }

                //add to group
                if (!Group.ContainsKey(groupValue))
                    Group.Add(groupValue, new DataFrame(row, Columns));
                else
                    Group[groupValue].AddRow(row);
            }

            return Group;
        }

        private static object parseValue(object value, string dformat)
        {
            if (value is string)
                return parseValue(value.ToString(), dformat);
            else
                return value;
        }

        private static object parseValue(string value, string dformat)
        {
            if (IsNumeric(value))
                return int.Parse(value, CultureInfo.InvariantCulture);
            else if (IsFloatNumeric(value))
                return float.Parse(value, CultureInfo.InvariantCulture);
            else if (IsMissingValue(value))
                return NAN;
            else //if (IsDateTime(value))
            {
                //
                if (!string.IsNullOrEmpty(dformat) && DateTime.TryParseExact(value, dformat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimee)) 
                {
                    return dateTimee;
                }
                else if (string.IsNullOrEmpty(dformat) && DateTime.TryParse(value, out DateTime dateTime))
                {
                    return dateTime;
                }
                if (bool.TryParse(value, out bool v))
                {
                    return v;
                }
                else
                {
                    return value;
                }

            }


        }

        private static readonly string[] _missingCharacters = new string[] { "n/a", "?", "*", };

        private static bool IsNumeric(string value)
        {
            return _numRegex.IsMatch(value);
        }

        private static bool IsFloatNumeric(string value)
        {
            return _numFloatRegex.IsMatch(value);
        }

        private bool applyOperator(int[] indCols, object[] rowValues, object[] filteValues, FilterOperator[] fOpers)
        {
            for (int colIndex = 0; colIndex < rowValues.Length; colIndex++)
            {
                var fOper = fOpers[colIndex];
                if (this._dfTypes[indCols[colIndex]] == ColType.I2)
                {
                    var val1 = Convert.ToBoolean(rowValues[colIndex]);
                    var val2 = Convert.ToBoolean(filteValues[colIndex]);
                    if (fOper != FilterOperator.Equal)
                        throw new Exception("Equal should be assign to boolean columns.");
                    if (val1 != val2)
                        return false;
                }
                if (this._dfTypes[indCols[colIndex]] == ColType.I32 || this._dfTypes[indCols[colIndex]] == ColType.I64
                    || this._dfTypes[indCols[colIndex]] == ColType.F32 || this._dfTypes[indCols[colIndex]] == ColType.DD)
                {
                    var val1 = Convert.ToDouble(rowValues[colIndex]);
                    var val2 = Convert.ToDouble(filteValues[colIndex]);

                    //
                    if (!applyOperator(val1, val2, fOper))
                        return false;
                }
                else if (this._dfTypes[indCols[colIndex]] == ColType.STR)
                {
                    var val1 = rowValues[colIndex].ToString();
                    var val2 = filteValues[colIndex].ToString();
                    //
                    if (!applyOperator(val1, val2, fOper))
                        return false;

                }
                else if (this._dfTypes[indCols[colIndex]] == ColType.DT)
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

        private static bool IsMissingValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;
            else if (string.IsNullOrWhiteSpace(value))
                return true;
            else if (_missingCharacters.Contains(value))
                return true;
            else
                return false;
        }

        private static List<object> deepCopyObject(IEnumerable<object> list)
        {
            var lstObj = new List<object>();
            foreach (var o in list)
            {
                var copy = o.DeepClone();
                lstObj.Add(copy);
            }

            return lstObj;
        }

        private static bool addNewColumnName(IList<string> columns, string colName)
        {
            while (columns.Contains(colName))
                colName += "0";
            //
            columns.Add(colName);

            return true;
        }

        private bool isEqual(DataFrame df2, int[] leftInd, int[] rightInd, int i, int j)
        {
            bool isEqual = true;
            int lInd = calculateIndex(i, 0);
            var numCols = df2.ColCount();
            var rInd = j * numCols;

            for (int k = 0; k < leftInd.Length; k++)
            {
                var li = lInd + leftInd[k];
                var ri = rInd + rightInd[k];

                if (df2._values[ri] == NAN || _values[li] == NAN)
                    return false;
                else if (this._dfTypes[leftInd[k]] == ColType.I2)
                {
                    //
                    if (Convert.ToBoolean(_values[li], CultureInfo.InvariantCulture) != Convert.ToBoolean(df2._values[ri], CultureInfo.InvariantCulture))
                    {
                        return false;
                    }
                }
                else if (this._dfTypes[leftInd[k]] == ColType.I32)
                {
                    //
                    if (Convert.ToInt32(_values[li], CultureInfo.InvariantCulture) != Convert.ToInt32(df2._values[ri], CultureInfo.InvariantCulture))
                    {
                        return false;
                    }
                }
                else if (this._dfTypes[leftInd[k]] == ColType.I64)
                {
                    //
                    if (Convert.ToInt64(_values[li], CultureInfo.InvariantCulture) != Convert.ToInt64(df2._values[ri], CultureInfo.InvariantCulture))
                    {
                        return false;
                    }
                }
                else if (this._dfTypes[leftInd[k]] == ColType.F32)
                {
                    //
                    if (Convert.ToSingle(_values[li], CultureInfo.InvariantCulture) != Convert.ToSingle(df2._values[ri], CultureInfo.InvariantCulture))
                    {
                        return false;
                    }
                }
                else if (this._dfTypes[leftInd[k]] == ColType.DD)
                {
                    //
                    if (Convert.ToDouble(_values[li], CultureInfo.InvariantCulture) != Convert.ToDouble(df2._values[ri], CultureInfo.InvariantCulture))
                    {
                        return false;
                    }
                }
                else if (this._dfTypes[leftInd[k]] == ColType.STR)
                {
                    //
                    if (Convert.ToString(_values[li], CultureInfo.InvariantCulture) != Convert.ToString(df2._values[ri], CultureInfo.InvariantCulture))
                    {
                        return false;
                    }
                }
                else if (this._dfTypes[leftInd[k]] == ColType.DT)
                {
                    //
                    if (Convert.ToDateTime(_values[li], CultureInfo.InvariantCulture) != Convert.ToDateTime(df2._values[ri], CultureInfo.InvariantCulture))
                    {
                        return false;
                    }
                }
                else
                    throw new Exception("Unknown column type");

            }

            return isEqual;
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
                if (_values[ind].GetType() == typeof(bool))
                    types[i] = ColType.I2;
                else if (_values[ind].GetType() == typeof(int))
                    types[i] = ColType.I32;
                else if (_values[ind].GetType() == typeof(long))
                    types[i] = ColType.I64;
                else if (_values[ind].GetType() == typeof(float))
                    types[i] = ColType.F32;
                else if (_values[ind].GetType() == typeof(double))
                    types[i] = ColType.DD;
                else if (_values[ind].GetType() == typeof(string))
                    types[i] = ColType.STR;
                else if (_values[ind].GetType() == typeof(DateTime))
                    types[i] = ColType.DT;
                else
                    throw new Exception("Unknown column type");
            }
            return types;
        }

        private static object calculateAggregation(IEnumerable<object> vals, Aggregation aggregation)
        {
            switch (aggregation)
            {
                case Aggregation.First:
                    return vals.First();
                case Aggregation.Last:
                    return vals.Last();
                case Aggregation.Count:
                    return vals.Count();
                case Aggregation.Sum:
                    return vals.Sum(x => Convert.ToDouble(x));
                case Aggregation.Avg:
                    return vals.Average(x => Convert.ToDouble(x));
                case Aggregation.Min:
                    return vals.Min(x => Convert.ToDouble(x));
                case Aggregation.Max:
                    return vals.Max(x => Convert.ToDouble(x));
                case Aggregation.Std:
                    return vals.Select(x => Convert.ToDouble(x)).ToArray().Stdev();
                default:
                    throw new Exception("Aggregation function is unknown.");
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

            throw new Exception($"Column '{col}' does not exist in the Data Frame. Column names are case sensitive.");
        }

        #endregion
    }
}
