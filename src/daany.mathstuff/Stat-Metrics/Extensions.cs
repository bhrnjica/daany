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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Daany.MathStuff.Stats;

public static class Extensions
{
	public static decimal Sqrt(decimal x, decimal epsilon = 0.0M)
	{
		if (x < 0)
			throw new ArgumentException("Cannot calculate square root of a negative number.");

		decimal current = (decimal)Math.Sqrt((double)x); // Initial guess
		decimal previous;

		do
		{
			previous = current;
			current = (previous + x / previous) / 2;
		} while (Math.Abs(previous - current) > epsilon);

		return current;
	}
	public static IEnumerable<int[]> To2DList(this IList<int> flatList, int numCols)
    {
        var numRows = flatList.Count / numCols;

        if (flatList == null || flatList.Count % numCols != 0)
        {
            throw new Exception("Inconsistent number of rows and columns.");
        }

        for (int i = 0; i < numRows; i++)
        {
            var rowArray = new int[numCols];

            for (int j = 0; j < numCols; j++)
            {
                int index = i * numCols + j;
                rowArray[j] = flatList[index];
            }

            yield return rowArray;
        }
    }

    public static IEnumerable<float[]> To2DList(this IList<float> flatList, int numCols)
    {
        var numRows = flatList.Count / numCols;

        if (flatList == null || flatList.Count % numCols != 0)
        {
            throw new Exception("Inconsistent number of rows and columns.");
        }

        for (int i = 0; i < numRows; i++)
        {
            var rowArray = new float[numCols];

            for (int j = 0; j < numCols; j++)
            {
                int index = i * numCols + j;
                rowArray[j] = flatList[index];
            }

            yield return rowArray;
        }
    }

    public static IEnumerable<double[]> To2DList(this IList<double> flatList, int numCols)
    {
        var numRows = flatList.Count / numCols;

        if (flatList == null || flatList.Count % numCols != 0)
        {
            throw new Exception("Inconsistent number of rows and columns.");
        }

        for (int i = 0; i < numRows; i++)
        {
            var rowArray = new double[numCols];

            for (int j = 0; j < numCols; j++)
            {
                int index = i * numCols + j;
                rowArray[j] = flatList[index];
            }

            yield return rowArray;
        }
    }


    public static int MaxArg(this int[] oneHotVector) 
    {
        int maxIndex = 0;
        int count = oneHotVector.Count();

        for (int i = 1; i < count; i++)
        {
            if (oneHotVector[i-1] < oneHotVector[i])
            {
                maxIndex = i;
            }

        }

        return maxIndex;
    }

    public static int MaxArg(this float[] oneHotVector)
    {
        int maxIndex = 0;
        int count = oneHotVector.Count();

        for (int i = 1; i < count; i++)
        {
            if (oneHotVector[i - 1] < oneHotVector[i])
            {
                maxIndex = i;
            }

        }

        return maxIndex;
    }

    public static int MaxArg(this double[] oneHotVector)
    {
        int maxIndex = 0;
        int count = oneHotVector.Count();

        for (int i = 1; i < count; i++)
        {
            if (oneHotVector[i - 1] < oneHotVector[i])
            {
                maxIndex = i;
            }

        }

        return maxIndex;
    }
}
