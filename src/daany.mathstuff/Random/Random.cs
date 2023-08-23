//--------------------------------------------------------------------------
// 
//  Copyright (c) Microsoft Corporation.  All rights reserved. 
// 
//  File: ThreadSafeRandom.cs
//
//--------------------------------------------------------------------------

using System;
using System.Numerics;
using System.Security.Cryptography;

using System.Threading;


namespace Daany.MathStuff.Random;

#if NET7_0_OR_GREATER
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
    public static T[] Rand<T>(T[] array, int n, int seed= 1234) where T : INumber<T>
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
#endif
/// <summary>
/// Represents a thread-safe, pseudo-random number generator.
/// </summary>
[Obsolete("The class is obsolite. Use TSRandom instead.")]
public class ThreadSafeRandom : System.Random, IDisposable
{
    public void Dispose()
    {
        _global.Dispose();
        _local.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>Seed provider.</summary>
    private static readonly RandomNumberGenerator _global = RandomNumberGenerator.Create(); //new RNGCryptoServiceProvider();
    public static bool FixedRandomSeed = false;




    /// <summary>The underlying provider of randomness, one instance per thread, initialized with _global.</summary>
    private ThreadLocal<System.Random> _local = new ThreadLocal<System.Random>(() =>
    {
        byte[] buffer = new byte[4];

        _global.GetBytes(buffer); // RNGCryptoServiceProvider is thread-safe for use in this manner
        if (FixedRandomSeed)
            return new System.Random(8888);
        else
            return new System.Random(BitConverter.ToInt32(buffer, 0));
    });


    /// <summary>Returns a nonnegative random number.</summary>
    /// <returns>A 32-bit signed integer greater than or equal to zero and less than MaxValue.</returns>
    public override int Next()
    {
        return _local.Value.Next();
    }

    /// <summary>Returns a nonnegative random number less than the specified maximum.</summary>
    /// <param name="maxValue">
    /// The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero. 
    /// </param>
    /// <returns>
    /// A 32-bit signed integer greater than or equal to zero, and less than maxValue; 
    /// that is, the range of return values ordinarily includes zero but not maxValue. However, 
    /// if maxValue equals zero, maxValue is returned.
    /// </returns>
    public override int Next(int maxValue)
    {
        return _local.Value.Next(maxValue);
    }

    /// <summary>Returns a random number within a specified range.</summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
    /// <returns>
    /// A 32-bit signed integer greater than or equal to minValue and less than maxValue; 
    /// that is, the range of return values includes minValue but not maxValue. 
    /// If minValue equals maxValue, minValue is returned.
    /// </returns>
    public override int Next(int minValue, int maxValue)
    {
        return _local.Value.Next(minValue, maxValue);
    }
    /// <summary>Fills the elements of a specified array of bytes with random numbers.</summary>
    /// <param name="buffer">An array of bytes to contain random numbers.</param>
    public override void NextBytes(byte[] buffer)
    {
        _local.Value.NextBytes(buffer);
    }
    /// <summary>Returns a random number between 0.0 and 1.0.</summary>
    /// <returns>A double-precision floating point number greater than or equal to 0.0, and less than 1.0.</returns>
    public override double NextDouble()
    {
        return _local.Value.NextDouble();
    }
    public double NextDouble(double minValue, double maxValue)
    {
        return minValue + _local.Value.NextDouble() * (maxValue - minValue);
    }

}