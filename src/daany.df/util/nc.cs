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
using System.Collections.Generic;
using System.Text;

namespace Daany
{
    /// <summary>
    /// Class implementation for generating various vector or matrix numbers and elements.
    /// nc - stands for 
    /// </summary>
    public class nc
    {
        private static readonly Random _rnd = new Random(1);

        /// <summary>
        /// Create vector of randomly generated double values stored in 1D array. 
        /// Vector.Length = row * col
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static object[] Rand(int row, int col)
        {
            var size = row * col;
            var obj = new object[size];
            for (int i = 0; i < size; i++)
                obj[i] = _rnd.NextDouble();
            return obj;
        }

        public static object[] ConsecutiveNum(int row, int col)
        {
            var size = row * col;
            var obj = new object[size];
            for (int i = 0; i < size; i++)
                obj[i] = i+1;
            return obj;
        }

        public static object[] Arange(int stop)
        {
            return Arange(0, stop, 1);
        }

        public static object[] Arange(int start, int stop, int step = 1)
        {
            if (start > stop)
            {
                throw new Exception("parameters invalid, start is greater than stop.");
            }

            int length = (int)Math.Ceiling((stop - start + 0.0) / step);
            int index = 0;

            var array = new object[length];
            for (int i = start; i < stop; i += step)
                array[index++] = i;

            return array;
        }

        public static object[] Zeros(int row, int col)
        {
            var size = row * col;
            var obj = new object[size];
            for (int i = 0; i < size; i++)
                obj[i] = 0;
            return obj;
        }

        public static object[] Generate(int row, int col, object val)
        {
            var size = row * col;
            var obj = new object[size];
            for (int i = 0; i < size; i++)
                obj[i] = val;
            return obj;
        }

        public static List<object> GenerateDateSeries(DateTime fromDate, DateTime toDate, TimeSpan span )
        {
            var lst = new List<object>();
            for(DateTime i = fromDate; i < toDate; i+=span )
            {
                lst.Add(i);
            }

            return lst;
        }

        public static List<object> GenerateMonthlySeries(DateTime fromDate, int months, int count)
        {
            var lst = new List<object>();
            var dt = fromDate;
            for (int i = 0; i < count; i ++)
            {
                lst.Add(dt);
                dt = dt.AddMonths(months);
            }

            return lst;
        }
        public static List<object> GenerateYearlySeries(DateTime fromDate, int years, int count)
        {
            var lst = new List<object>();
            var dt = fromDate;
            for (int i = 0; i < count; i++)
            {
                lst.Add(dt);
                dt = dt.AddYears(years);
            }

            return lst;
        }

        public static List<object> GenerateIntSeries(int fromNumber, int toNumber, int step)
        {
            var lst = new List<object>();
            for (int i = fromNumber; i < toNumber; i += step)
            {
                lst.Add(i);
            }

            return lst;
        }

        public static List<object> GenerateIntNSeries(int fromNumber, int step, int size)
        {
            var lst = new List<object>();
            int value = fromNumber;
            for (int i = 0; i < size; i++)
            {
                lst.Add(i);
                value = fromNumber + step;
            }

            return lst;
        }

        public static List<object> GenerateDoubleNSeries(double fromNumber, double step, int size)
        {
            var lst = new List<object>();
            double value = fromNumber;
            for (int i = 0; i < size; i++)
            {
                lst.Add(i);
                value = fromNumber + step;
            }

            return lst;
        }

        public static List<object> GenerateFloatNSeries(float fromNumber, float step, int size)
        {
            var lst = new List<object>();
            float value = fromNumber;
            for (int i = 0; i < size; i++)
            {
                lst.Add(i);
                value = fromNumber + step;
            }

            return lst;
        }

        public static List<object> GenerateDoubleSeries(double fromNumber, double toNumber, double step)
        {
            var lst = new List<object>();
            for (double i = fromNumber; i < toNumber; i += step)
            {
                lst.Add(i);
            }

            return lst;
        }

        public static List<object> GenerateDoubleSeries(double fromNumber, double toNumber, int count)
        {
            var lst = new List<object>();
            var step = (toNumber - fromNumber)/(double)count;
            for (double i = fromNumber; i < toNumber; i += step)
            {
                lst.Add(i);
            }

            return lst;
        }

        public static List<object> GenerateConstSeries(double number, int count)
        {
            var lst = new List<object>();
            for (double i = 0; i < count; i++)
            {
                lst.Add(number);
            }

            return lst;
        }
    }
}
