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

using Daany.MathStuff.Random;
using System;
using System.Collections.Generic;

namespace Daany
{
	/// <summary>
	/// Class implementation for generating various vector or matrix numbers and elements.
	/// </summary>
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS8981 // Naming Styles
	public class nc
#pragma warning restore CS8981 // Naming Styles
#pragma warning restore IDE1006 // Naming Styles
	{
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
                obj[i] = Constant.rand.NextDouble();
            return obj;
        }

        public static object[] Rand(int length, double min, double max)
        {
            var obj = new object[length];
            for (int i = 0; i < length; i++)
                obj[i] = Constant.rand.NextDouble(min,max);
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
