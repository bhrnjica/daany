using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Daany.MathStuff.Stats;

public static class Extensions
{
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
