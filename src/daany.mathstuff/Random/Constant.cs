//////////////////////////////////////////////////////////////////////////////
//   ____    _    _   _   _   __  __                                       //
//  |  _ \  / \  | \ | | | \ | |\ \/ /                                     //
//  | | | |/ _ \ |  \| | |  \| | \  /                                      //
//  | |_| / ___ \| |\  | | |\  | | |                                       //
//  |____/_/   \_\_| \_| |_| \_| |_|                                       //
//                                                                         //
//  DAata ANalYtics Library                                                //
//  MathStuff:Linear Algebra, Statistics, Optimization, Machine Learning.  //
//  https://github.com/bhrnjica/daany                                      //
//                                                                         //
//  Copyright © 2006-2025 Bahrudin Hrnjica                                 //
//                                                                         //
//  Free. Open Source. MIT Licensed.                                       //
//  https://github.com/bhrnjica/daany/blob/master/LICENSE                  //
//////////////////////////////////////////////////////////////////////////////

using System.Numerics;
using System;

namespace Daany.MathStuff.Random
{
    public static class Constant
    {
        public static ThreadSafeRandom rand = new ThreadSafeRandom();
        static bool _fixedRandom = false;
        public static bool FixedRandomSeed
        {
            get
            {
                return _fixedRandom;
            }
            set
            {
                _fixedRandom = value;
                ThreadSafeRandom.FixedRandomSeed = value;
                rand = new ThreadSafeRandom();
            }
        }
    }

	public sealed class TSRandom
	{
		/// <summary>
		/// Select row x col random elements from interval (0, 1) values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public static T[,] Rand<T>(int row, int col) where T : INumber<T>
		{
			var size = row * col;
			var obj = new T[row, col];

			for (int i = 0; i < size; i++)
				for (int j = 0; j < col; j++)
					obj[i, j] = T.CreateChecked(Constant.rand.NextDouble());

			return obj;
		}

		/// <summary>
		/// Select row x col random elements from interval (min, max) values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static T[,] Rand<T>(int row, int col, T min, T max) where T : INumber<T>
		{
			var size = row * col;
			var obj = new T[row, col];

			for (int i = 0; i < size; i++)
				for (int j = 0; j < col; j++)
					obj[i, j] = T.CreateChecked(Constant.rand.NextDouble(Convert.ToDouble(min), Convert.ToDouble(max)));

			return obj;
		}

		/// <summary>
		/// Select n numbers from interval (0, 1) values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="count"></param>
		/// <returns></returns>
		public static T[] Rand<T>(int count) where T : INumber<T>
		{
			var obj = new T[count];

			for (int i = 0; i < count; i++)
			{
				obj[i] = T.CreateChecked(Constant.rand.NextDouble());
			}

			return obj;
		}

		/// <summary>
		/// Select n random elements from interval (min, max) values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="n"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static T[] Rand<T>(int n, T min, T max) where T : INumber<T>
		{
			var obj = new T[n];
			for (int i = 0; i < n; i++)
				obj[i] = T.CreateChecked(Constant.rand.NextDouble(Convert.ToDouble(min), Convert.ToDouble(max)));
			return obj;
		}

		/// <summary>
		/// Reservoir Sampling to select n random elements from the array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array">input array of elements</param>
		/// <param name="n">number of random elements to select</param>
		/// <returns>array of n random elements</returns>
		public static T[] Rand<T>(T[] array, int n, int seed = 1234) where T : INumber<T>
		{
			var result = new T[n];
			Array.Copy(array, result, n);
			var rand = new System.Random(seed);

			for (int i = n; i < array.Length; i++)
			{
				int j = rand.Next(0, i + 1);
				if (j < n)
				{
					result[j] = array[i];
				}
			}

			return result;
		}


	}

}
