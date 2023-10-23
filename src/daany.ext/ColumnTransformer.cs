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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Daany;
using Daany.Ext;

namespace Daany.Ext
{
    public static class DataFrameColumnTransformer
    {
        public static (DataFrame df, float[] scaledValues, string[] labels) TransformColumn(this DataFrame df, string colName, ColumnTransformer transformer, bool transformedColumnsOnly = false)
        {
            switch (transformer)
            {
                case ColumnTransformer.None:
                    return (df, null, null)!;
                case ColumnTransformer.Binary1:
                    var (edf, cValues) = BinaryEncoding(df, colName, transformedColumnsOnly);
                    return (edf, null, cValues)!;
                case ColumnTransformer.Binary2:
                    (edf, cValues) = BinaryEncoding2(df, colName, transformedColumnsOnly);
                    return (edf, null, cValues)!;
                case ColumnTransformer.Ordinal:
                    (edf, cValues) = OrdinalEncoding(df, colName, transformedColumnsOnly);
                    return (edf, null, cValues)!;
                case ColumnTransformer.OneHot:
                    (edf, cValues) = OneHotEncodeColumn(df, colName, transformedColumnsOnly);
                    return (edf, null, cValues)!;
                case ColumnTransformer.Dummy:
                    (edf, cValues) = DummyEncodeColumn(df, colName, transformedColumnsOnly);
                    return (edf, null, cValues)!;
                case ColumnTransformer.MinMax:
                case ColumnTransformer.Standardizer:
                    (var tdf, float[] fValues) = ScaleColumn(df, colName, transformer, transformedColumnsOnly);
                    return (tdf, fValues, null)!;                 
                default:
                    throw new NotSupportedException("Data normalization is not supported.");
            }

            throw new NotImplementedException();
        }

        private static (DataFrame edf, float[] fValues) ScaleColumn(DataFrame dff, string colName, ColumnTransformer transformer, bool transformedColumnsOnly)
        {
            
            var newColName = colName + "_scaled";
            var df = dff[dff.Columns.ToArray()];
            var s = Series.FromDataFrame(df, colName);
            float param1;
            float param2;
            if (transformer == ColumnTransformer.MinMax)
            {
               
                var minObj = s.Aggregate<float>(Aggregation.Min);
                param1 = Convert.ToSingle(minObj);
                var maxObj = s.Aggregate<float>(Aggregation.Max);
                param2 = Convert.ToSingle(maxObj);

                df.AddCalculatedColumn(newColName, (IDictionary<string, object> row, int i) =>
                {
                    var val = Convert.ToDouble(row[colName]);
                    return (val - param1) / (param2 - param1);
                });

            }
            else if (transformer == ColumnTransformer.MinMax || 
                    transformer == ColumnTransformer.Standardizer)
            {
                var avgObj = s.Aggregate<float>(Aggregation.Avg);
                param1 = Convert.ToSingle(avgObj);
                var stdObj = s.Aggregate<float>(Aggregation.Std);
                param2 = Convert.ToSingle(stdObj);

                df.AddCalculatedColumn(colName + "_scaled", (IDictionary<string, object> row, int i) =>
                {
                    var val = Convert.ToDouble(row[colName]);
                    return (val - param1) / (param2);
                });
            }
            else
                throw new NotSupportedException("Column transformation is not supported.");

            if (transformedColumnsOnly)
            {
                var ddf = df.Create((newColName, null)!);
                return (ddf, new float[] { param1, param2 });
            }
            else
                return (df, new float[] { param1, param2 });
        }

        private static (DataFrame, string[]) OneHotEncodeColumn(this DataFrame df, string colName, bool encodedOnly = false)
        {
            var colVector = df[colName];

            if (colVector == null)
            {
                throw new Exception("colVector is null");
            }

            var classValues = colVector.Where(x=> DataFrame.NAN != x).Select(x => x.ToString()).Distinct().ToArray();
            
            //define encoded columns
            var dict = new Dictionary<string, List<object>>();

            //add one-hot encoded columns
            foreach (var c in classValues)
            {
                if (c != null)
                {
                    dict.Add(c, new List<object>());
                }

            }
            //encode values
            foreach (var cValue in colVector)
            {
                for (int i = 0; i < classValues.Length; i++)
                {
                    if (cValue.ToString() == classValues[i])
                        dict[classValues[i]].Add((int)1);
                    else
                        dict[classValues[i]].Add((int)0);
                }

            }
            if (encodedOnly)
            {
                var newDf = new DataFrame(dict);
                return (newDf, classValues);
            }
            else
            {
                var newDf = df.AddColumns(dict);
                return (newDf, classValues);
            }

        }

        private static (DataFrame, string[]) DummyEncodeColumn(this DataFrame df, string colName, bool encodedOnly = false)
        {
            var colVector = df[colName];
            var classValues = colVector.Where(x => DataFrame.NAN != x).Select(x => x.ToString()).Distinct().ToArray();

            //define encoded columns
            var dict = new Dictionary<string, List<object>>();
            var dummyClasses = classValues.SkipLast(1);

            //add dummy encoded columns
            foreach (var c in dummyClasses)
            {
                dict.Add(c, new List<object>());
            }

            //encode values
            foreach (var cValue in colVector)
            {
                for (int i = 0; i < dummyClasses.Count(); i++)
                {
                    if (cValue.ToString() == classValues[i])
                        dict[classValues[i]].Add((int)1);
                    else
                        dict[classValues[i]].Add((int)0);
                }

            }
            if (encodedOnly)
            {
                var newDf = new DataFrame(dict);
                return (newDf, classValues);
            }
            else
            {
                var newDf = df.AddColumns(dict);
                return (newDf, classValues);
            }

        }

        /// <summary>
        /// Ordinal encoding of classification data. First value start from 0. So in case of example:
        /// var colors = new string[] { "red", "green", "blue", "green", "red" }; endoded values are:
        /// var colors = new int { 0, 1, 2, 1, 0 };
        /// </summary>
        /// <param name="df"></param>
        /// <param name="colName"></param>
        /// <param name="encodedOnly"></param>
        /// <returns></returns>
        private static (DataFrame, string[]) OrdinalEncoding(this DataFrame df, string colName, bool encodedOnly = false)
        {
            var colVector = df[colName];

            var classValues = colVector.Where(x => DataFrame.NAN != x).Select(x => x.ToString()).Distinct().ToList();

            //define encoded columns
            var dict = new Dictionary<string, List<object>>();
            var encodedValues = new List<object>();
            foreach(var value in colVector)
            {
                int ordinalValue = classValues.IndexOf(value.ToString());
                encodedValues.Add(ordinalValue);
            }
            
            //
            dict.Add(colName + "_cvalues", encodedValues);
            var newDf = df.AddColumns(dict);

            if (encodedOnly)
                return (newDf[new string[] { colName + "_cvalues" }],classValues.ToArray());
            else
                return (newDf, classValues.ToArray());

        }

        private static (DataFrame, string[]) BinaryEncoding(this DataFrame df, string colName, bool encodedOnly = false)
        {
            var colVector = df[colName];

            var classValues = colVector.Where(x => DataFrame.NAN != x).Select(x => x.ToString()).Distinct().ToList();

            //define encoded columns
            var dict = new Dictionary<string, List<object>>();
            var encodedValues = new List<object>();
            foreach (var value in colVector)
            {
                if (value is bool)
                {
                    int ordinalValue = Convert.ToInt16(value);
                    encodedValues.Add(ordinalValue);
                }
                else
                {
                    int ordinalValue = classValues.IndexOf(value.ToString());
                    encodedValues.Add(ordinalValue);
                }
            }

            //
            dict.Add(colName + "_cvalues", encodedValues);
            var newDf = df.AddColumns(dict);

            if (encodedOnly)
                return (newDf[new string[] { colName + "_cvalues" }], classValues.Select(x=>x.ToString()).ToArray());
            else
                return (newDf, classValues.Select(x => x.ToString()).ToArray());

        }

        private static (DataFrame, string[]) BinaryEncoding2(this DataFrame df, string colName, bool encodedOnly = false)
        {
            var colVector = df[colName];

            var classValues = colVector.Where(x => DataFrame.NAN != x).Select(x => x.ToString()).Distinct().ToList();

            //define encoded columns
            var dict = new Dictionary<string, List<object>>();
            var encodedValues = new List<object>();
            foreach (var value in colVector)
            {
                if (value is bool)
                {
                    int ordinalValue = Convert.ToInt16(value);
                    if (ordinalValue == 0)
                        ordinalValue = -1;
                    encodedValues.Add(ordinalValue);
                }
                else
                {
                    int ordinalValue = classValues.IndexOf(value.ToString());
                    if (ordinalValue == 0)
                        ordinalValue = -1;
                    encodedValues.Add(ordinalValue);
                }
            }

            //
            dict.Add(colName + "_cvalues", encodedValues);
            var newDf = df.AddColumns(dict);

            if (encodedOnly)
                return (newDf[new string[] { colName + "_cvalues" }], classValues.Select(x => x.ToString()).ToArray());
            else
                return (newDf, classValues.Select(x => x.ToString()).ToArray());

        }

    }
}
