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
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Daany.MathStuff.Random;
using Daany.MathStuff;

namespace Daany
{
    /// <summary>
    /// Class implementation for DataFrame. The DataFrame is going to be C# specific implementation
    /// to handle data loading from files, grouping, sorting, filtering, handling with columns and rows
    /// accessing data frame (df) elements etc.
    /// </summary>

    public partial class DataFrame
    {
        #region Private

        private static object? calculateAggregation(IEnumerable<object?> vals, Aggregation aggregation, ColType colType)
        {
            var value =  _calculateAggregation(vals, aggregation, colType);
            //
            if (value is double)
                value = Math.Round(Convert.ToDouble(value), 6);
            //
            else if (value is float)
                value = Math.Round(Convert.ToSingle(value), 6);

            return value;
        }
        internal static object? _calculateAggregation(IEnumerable<object?> vals, Aggregation aggregation, ColType colType)
        {
            switch (aggregation)
            {
                case Aggregation.None:
                    return null;
                case Aggregation.Unique:
                    return vals.Distinct().Count();

                case Aggregation.Top:
                    return vals.ToArray()!.FrequencyOf().First().Item1;

                case Aggregation.Random:
                    var ind = Constant.rand.Next(vals.Count());
                    return vals.ToArray().ElementAt(ind);

                case Aggregation.Frequency:
                    return vals.ToArray()!.FrequencyOf().First().Item2;

                case Aggregation.First:
                    return vals.First();

                case Aggregation.Last:
                    return vals.Last();

                case Aggregation.Count:
                    return vals.Count();

                case Aggregation.Sum:
                    {

                        if (colType == ColType.I2)//boolean
                            return DataFrame.NAN;

                        else if (colType == ColType.I32)//int
                            return vals.Select(x => Convert.ToInt32(x, CultureInfo.InvariantCulture)).Sum();

                        else if (colType == ColType.I64)//long
                            return vals.Select(x => Convert.ToInt64(x, CultureInfo.InvariantCulture)).Sum();

                        else if (colType == ColType.F32)//float
                            return vals.Select(x => Convert.ToSingle(x, CultureInfo.InvariantCulture)).Sum();

                        else if (colType == ColType.DD)//double
                            return vals.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).Sum();

                        else if (colType == ColType.DT)//datetime
                            return DataFrame.NAN;

                        else if (colType == ColType.IN)//Categorical
                            return DataFrame.NAN;

                        else if (colType == ColType.STR)//String
                            return DataFrame.NAN;

                        else
                            return DataFrame.NAN;
                    }

                case Aggregation.Avg:
                    {

                        if (colType == ColType.I2)//boolean
                            return DataFrame.NAN;

                        else if (colType == ColType.I32)//int
                            return vals.Select(x => Convert.ToInt32(x, CultureInfo.InvariantCulture)).Average();

                        else if (colType == ColType.I64)//long
                            return vals.Select(x => Convert.ToInt64(x, CultureInfo.InvariantCulture)).Average();

                        else if (colType == ColType.F32)//float
                            return vals.Select(x => Convert.ToSingle(x, CultureInfo.InvariantCulture)).Average();

                        else if (colType == ColType.DD)//double
                            return vals.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).Average();

                        else if (colType == ColType.DT)//datetime
                            return DataFrame.NAN;

                        else if (colType == ColType.IN)//Categorical
                            return DataFrame.NAN;

                        else if (colType == ColType.STR)//String
                            return DataFrame.NAN;

                        else
                            return DataFrame.NAN;
                    }

                case Aggregation.Min:
                    {

                        if (colType == ColType.I2)//boolean
                            return DataFrame.NAN;

                        else if (colType == ColType.I32)//int
                            return vals.Select(x => Convert.ToInt32(x, CultureInfo.InvariantCulture)).Min();

                        else if (colType == ColType.I64)//long
                            return vals.Select(x => Convert.ToInt64(x, CultureInfo.InvariantCulture)).Min();

                        else if (colType == ColType.F32)//float
                            return vals.Select(x => Convert.ToSingle(x, CultureInfo.InvariantCulture)).Min();

                        else if (colType == ColType.DD)//double
                            return vals.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).Min();

                        else if (colType == ColType.DT)//datetime
                            return vals.Select(x => Convert.ToDateTime(x, CultureInfo.InvariantCulture)).Min();

                        else if (colType == ColType.IN)//Categorical
                            return DataFrame.NAN;

                        else if (colType == ColType.STR)//String
                            return DataFrame.NAN;

                        else
                            return DataFrame.NAN;
                    }

                case Aggregation.Max:
                    {

                        if (colType == ColType.I2)//boolean
                            return DataFrame.NAN;

                        else if (colType == ColType.I32)//int
                            return vals.Select(x => Convert.ToInt32(x, CultureInfo.InvariantCulture)).Max();

                        else if (colType == ColType.I64)//long
                            return vals.Select(x => Convert.ToInt64(x, CultureInfo.InvariantCulture)).Max();

                        else if (colType == ColType.F32)//float
                            return vals.Select(x => Convert.ToSingle(x, CultureInfo.InvariantCulture)).Max();

                        else if (colType == ColType.DD)//double
                            return vals.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).Max();

                        else if (colType == ColType.DT)//datetime
                            return vals.Select(x => Convert.ToDateTime(x, CultureInfo.InvariantCulture)).Max();

                        else if (colType == ColType.IN)//Categorical
                            return DataFrame.NAN;

                        else if (colType == ColType.STR)//String
                            return DataFrame.NAN;

                        else
                            return DataFrame.NAN;
                    }
                    
                //Standard deviation
                case Aggregation.Std:
                    {

                        if (colType == ColType.I2)//boolean
                            return DataFrame.NAN;

                        else if (colType == ColType.I32)//int
                            return vals.Select(x => Convert.ToInt32(x, CultureInfo.InvariantCulture)).ToArray().Stdev();

                        else if (colType == ColType.I64)//long
                            return vals.Select(x => Convert.ToInt64(x, CultureInfo.InvariantCulture)).ToArray().Stdev();

                        else if (colType == ColType.F32)//float
                            return vals.Select(x => Convert.ToSingle(x, CultureInfo.InvariantCulture)).ToArray().Stdev();

                        else if (colType == ColType.DD)//double
                            return vals.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToArray().Stdev();

                        else if (colType == ColType.DT)//datetime
                            return DataFrame.NAN;

                        else if (colType == ColType.IN)//Categorical
                            return DataFrame.NAN;

                        else if (colType == ColType.STR)//String
                            return DataFrame.NAN;

                        else
                            return DataFrame.NAN;
                    }

                case Aggregation.Mode:
                    return vals.ToArray()!.ModeOf<object>();

                case Aggregation.Median:
                    {

                        if (colType == ColType.I2)//boolean
                            return DataFrame.NAN;

                        else if (colType == ColType.I32)//int
                            return vals.Select(x => Convert.ToInt32(x, CultureInfo.InvariantCulture)).ToArray().MedianOf();

                        else if (colType == ColType.I64)//long
                            return vals.Select(x => Convert.ToInt64(x, CultureInfo.InvariantCulture)).ToArray().MedianOf();

                        else if (colType == ColType.F32)//float
                            return vals.Select(x => Convert.ToSingle(x, CultureInfo.InvariantCulture)).ToArray().MedianOf();

                        else if (colType == ColType.DD)//double
                            return vals.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToArray().MedianOf();

                        else if (colType == ColType.DT)//datetime
                            return DataFrame.NAN;

                        else if (colType == ColType.IN)//Categorical
                            return DataFrame.NAN;

                        else if (colType == ColType.STR)//String
                            return DataFrame.NAN;

                        else
                            return DataFrame.NAN;
                    }

                //25% percentage quantile
                case Aggregation.FirstQuartile:
                    {

                        if (colType == ColType.I2)//boolean
                            return DataFrame.NAN;

                        else if (colType == ColType.I32)//int
                            return vals.Select(x => Convert.ToInt32(x, CultureInfo.InvariantCulture)).ToArray().Percentile(25);

                        else if (colType == ColType.I64)//long
                            return vals.Select(x => Convert.ToInt64(x, CultureInfo.InvariantCulture)).ToArray().Percentile(25);

                        else if (colType == ColType.F32)//float
                            return vals.Select(x => Convert.ToSingle(x, CultureInfo.InvariantCulture)).ToArray().Percentile(25);

                        else if (colType == ColType.DD)//double
                            return vals.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToArray().Percentile(25);

                        else if (colType == ColType.DT)//datetime
                            return DataFrame.NAN;

                        else if (colType == ColType.IN)//Categorical
                            return DataFrame.NAN;

                        else if (colType == ColType.STR)//String
                            return DataFrame.NAN;

                        else
                            return DataFrame.NAN;
                    }

                //75% percentage quantile
                case Aggregation.ThirdQuartile:
                    {

                        if (colType == ColType.I2)//boolean
                            return DataFrame.NAN;

                        else if (colType == ColType.I32)//int
                            return vals.Select(x => Convert.ToInt32(x, CultureInfo.InvariantCulture)).ToArray().Percentile(75);

                        else if (colType == ColType.I64)//long
                            return vals.Select(x => Convert.ToInt64(x, CultureInfo.InvariantCulture)).ToArray().Percentile(75);

                        else if (colType == ColType.F32)//float
                            return vals.Select(x => Convert.ToSingle(x, CultureInfo.InvariantCulture)).ToArray().Percentile(75);

                        else if (colType == ColType.DD)//double
                            return vals.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToArray().Percentile(75);

                        else if (colType == ColType.DT)//datetime
                            return DataFrame.NAN;

                        else if (colType == ColType.IN)//Categorical
                            return DataFrame.NAN;

                        else if (colType == ColType.STR)//String
                            return DataFrame.NAN;

                        else
                            return DataFrame.NAN;
                    }
                default:
                    throw new Exception("DataType is not known.");
            }
        }


        #endregion
    }
}
