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


/// <summary>
/// Represents a thread-safe, pseudo-random number generator.
/// </summary>
public class ThreadSafeRandom : System.Random, IDisposable
{
    public void Dispose()
    {
        _global.Dispose();
        _local?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>Seed provider.</summary>
    private static readonly RandomNumberGenerator _global = RandomNumberGenerator.Create(); //new RNGCryptoServiceProvider();
    public static bool FixedRandomSeed = false;




    /// <summary>The underlying provider of randomness, one instance per thread, initialized with _global.</summary>
    private ThreadLocal<System.Random>? _local = new ThreadLocal<System.Random>(() =>
    {
        byte[]? buffer = new byte[4];

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
        if (_local != null)
            if (_local.Value != null)
                return _local.Value.Next();

        throw new InvalidOperationException();
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
        if (_local != null)
            if (_local.Value != null)
                return _local.Value.Next(maxValue);

        throw new InvalidOperationException();
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
        if (_local != null)
            if (_local.Value != null)
                return _local.Value.Next(minValue, maxValue);

        throw new InvalidOperationException();
    }
    /// <summary>Fills the elements of a specified array of bytes with random numbers.</summary>
    /// <param name="buffer">An array of bytes to contain random numbers.</param>
    public override void NextBytes(byte[] buffer)
    {
        _local?.Value?.NextBytes(buffer);
    }
    /// <summary>Returns a random number between 0.0 and 1.0.</summary>
    /// <returns>A double-precision floating point number greater than or equal to 0.0, and less than 1.0.</returns>
    public override double NextDouble()
    {
        if (_local != null)
            if (_local.Value != null)
                return _local.Value.NextDouble();

        throw new InvalidOperationException();
    }
    public double NextDouble(double minValue, double maxValue)
    {
        double nextValue = 0;

        if (_local != null)
            if (_local.Value != null)
                nextValue = _local.Value.NextDouble();

        return minValue + nextValue * (maxValue - minValue);
    }

}