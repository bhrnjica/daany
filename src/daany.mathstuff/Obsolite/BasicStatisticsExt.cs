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
using System.Linq;
using System.Threading.Tasks;
using static System.Math;
namespace Daany.MathStuff;


/// <summary>
/// Implement extension methods for statistics calculation on one data set eg. mean, median, variance,... 
/// Modul calculate mean value of array of numbers. 
/// The mean is the average of the numbers.
/// </summary>
[Obsolete("The class is obsolite. Use classes from Daany.MathStuff.Matrix namespace instead.")]
public static class BasicStatistics
{

    /// <summary>
    /// Calculate mode value of array of numbers. Mode represent the most frequent element in the array 
    /// </summary>
    /// <param name="colData"> array of values </param>
    /// <returns>calculated mode</returns>
    public static T ModeOf<T>(this T[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or empty!");

        var counts = new Dictionary<T, int>();
        foreach (var a in colData)
        {
            if (counts.ContainsKey(a))
                counts[a] = counts[a] + 1;
            else
                counts[a] = 1;
        }

        //
        T result = counts.Keys.First();
        int max = counts[result];
        foreach (var key in counts.Keys)
        {
            if (counts[key] > max)
            {
                max = counts[key];
                result = key;
            }
        }

        return result;
    }


    /// <summary>
    /// Select random element from the array
    /// </summary>
    /// <param name="colData"> array of values </param>
    /// <returns>random element</returns>
    public static T RandomOf<T>(this T[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or empty!");

        var rand = new System.Random((int)DateTime.Now.Ticks);
        var randIndex = rand.Next(0, colData.Length);
        return colData[randIndex];
    }

    /// <summary>
    /// Calculate frequency of the set
    /// </summary>
    /// <param name="colData"></param>
    /// <returns></returns>
    public static List<(object, int)> FrequencyOf(this object[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or empty!");

        var query = from r in colData
                    group r by r into g
                    select (g.Key, g.Count());

        return query.OrderByDescending(x => x.Item2).ToList();
    }

    #region Mean-Average
    /// <summary>
    /// Calculate mean value of array of numbers. 
    /// </summary>
    /// <param name="colData"> array of values </param>
    /// <returns>calculated mean</returns>
    public static double MeanOf(this double[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or empty!");

        //calculate summ of the values
        double sum = 0;
        for (int i = 0; i < colData.Length; i++)
            sum += colData[i];

        //calculate mean
        double retVal = sum / colData.Length;

        return retVal;
    }
    /// <summary>
    /// Calculate mean value of array of numbers. 
    /// </summary>
    /// <param name="colData"> array of values </param>
    /// <returns>calculated mean</returns>
    public static float MeanOf(this float[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or empty!");

        //calculate summ of the values
        float sum = 0;
        for (int i = 0; i < colData.Length; i++)
            sum += colData[i];

        //calculate mean
        float retVal = sum / colData.Length;

        return retVal;
    }
    /// <summary>
    /// Calculate mean value of array of numbers. 
    /// </summary>
    /// <param name="colData"> array of values </param>
    /// <returns>calculated mean</returns>
    public static float MeanOf(this int[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or empty!");

        //calculate summ of the values
        float sum = 0;
        for (int i = 0; i < colData.Length; i++)
            sum += colData[i];

        //calculate mean
        float retVal = sum / colData.Length;

        return retVal;
    }
    /// <summary>
    /// Calculate mean value of array of numbers. 
    /// </summary>
    /// <param name="colData"> array of values </param>
    /// <returns>calculated mean</returns>
    public static float MeanOf(this long[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or empty!");

        //calculate summ of the values
        float sum = 0;
        for (int i = 0; i < colData.Length; i++)
            sum += colData[i];

        //calculate mean
        float retVal = sum / colData.Length;

        return retVal;
    }
    #endregion 

    #region Median
    /// <summary>
    /// Calculate median value of array of numbers. 
    /// If there is an odd number of data values 
    /// then the median will be the value in the middle. 
    /// If there is an even number of data values the median 
    /// is the mean of the two data values in the middle. 
    /// For the data set 1, 1, 2, 5, 6, 6, 9 the median is 5. 
    /// For the data set 1, 1, 2, 6, 6, 9 the median is 4.
    /// </summary>
    /// <param name="colData"></param>
    /// <returns></returns>
    public static double MedianOf(this double[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or empty!");

        //initial mean value
        double median = 0;
        int medianIndex = colData.Length / 2;

        //make a deep copy of the data
        var temp = new double[colData.Length];
        Array.Copy(colData, temp, colData.Length);
        //sort the values

        Array.Sort(temp);

        //in case we have odd number of elements in data set
        // median is just in the middle of the dataset
        if (temp.Length % 2 == 1)
        {
            // 
            median = temp[medianIndex];
        }
        else//otherwise calculate average between two element in the middle
        {
            var val1 = temp[medianIndex - 1];
            var val2 = temp[medianIndex];

            //calculate the median
            median = (val1 + val2) / 2;
        }

        return median;
    }

    /// <summary>
    /// Calculate median value of array of numbers. 
    /// If there is an odd number of data values 
    /// then the median will be the value in the middle. 
    /// If there is an even number of data values the median 
    /// is the mean of the two data values in the middle. 
    /// For the data set 1, 1, 2, 5, 6, 6, 9 the median is 5. 
    /// For the data set 1, 1, 2, 6, 6, 9 the median is 4.
    /// </summary>
    /// <param name="colData"></param>
    /// <returns></returns>
    public static float MedianOf(this float[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or empty!");

        //initial mean value
        float median = 0;
        int medianIndex = colData.Length / 2;

        //make a deep copy of the data
        var temp = new float[colData.Length];
        Array.Copy(colData, temp, colData.Length);
        //sort the values

        Array.Sort(temp);

        //in case we have odd number of elements in data set
        // median is just in the middle of the dataset
        if (temp.Length % 2 == 1)
        {
            // 
            median = temp[medianIndex];
        }
        else//otherwise calculate average between two element in the middle
        {
            var val1 = temp[medianIndex - 1];
            var val2 = temp[medianIndex];

            //calculate the median
            median = (val1 + val2) / 2;
        }

        return median;
    }

    /// <summary>
    /// Calculate median value of array of numbers. 
    /// If there is an odd number of data values 
    /// then the median will be the value in the middle. 
    /// If there is an even number of data values the median 
    /// is the mean of the two data values in the middle. 
    /// For the data set 1, 1, 2, 5, 6, 6, 9 the median is 5. 
    /// For the data set 1, 1, 2, 6, 6, 9 the median is 4.
    /// </summary>
    /// <param name="colData"></param>
    /// <returns></returns>
    public static float MedianOf(this int[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or empty!");

        //initial mean value
        float median = 0;
        int medianIndex = colData.Length / 2;

        //make a deep copy of the data
        var temp = new int[colData.Length];
        Array.Copy(colData, temp, colData.Length);
        //sort the values

        Array.Sort(temp);

        //in case we have odd number of elements in data set
        // median is just in the middle of the dataset
        if (temp.Length % 2 == 1)
        {
            // 
            median = temp[medianIndex];
        }
        else//otherwise calculate average between two element in the middle
        {
            var val1 = temp[medianIndex - 1];
            var val2 = temp[medianIndex];

            //calculate the median
            median = (val1 + val2) / 2f;
        }

        return median;
    }

    /// <summary>
    /// Calculate median value of array of numbers. 
    /// If there is an odd number of data values 
    /// then the median will be the value in the middle. 
    /// If there is an even number of data values the median 
    /// is the mean of the two data values in the middle. 
    /// For the data set 1, 1, 2, 5, 6, 6, 9 the median is 5. 
    /// For the data set 1, 1, 2, 6, 6, 9 the median is 4.
    /// </summary>
    /// <param name="colData"></param>
    /// <returns></returns>
    public static float MedianOf(this long[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or empty!");

        //initial mean value
        float median = 0;
        int medianIndex = colData.Length / 2;

        //make a deep copy of the data
        var temp = new long[colData.Length];
        Array.Copy(colData, temp, colData.Length);
        //sort the values

        Array.Sort(temp);

        //in case we have odd number of elements in data set
        // median is just in the middle of the dataset
        if (temp.Length % 2 == 1)
        {
            // 
            median = temp[medianIndex];
        }
        else//otherwise calculate average between two element in the middle
        {
            var val1 = temp[medianIndex - 1];
            var val2 = temp[medianIndex];

            //calculate the median
            median = (val1 + val2) / 2f;
        }

        return median;
    }


    #endregion

    #region Precentile
    /// <summary>
    /// Calculate percentile of a sorted data set
    /// </summary>
    /// <param name="sortedData"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static double Percentile(this double[] data, double p)
    {

        //make a deep copy of the data
        var sortedData = new double[data.Length];

        Array.Copy(data, sortedData, data.Length);

        //sort the values
        Array.Sort(sortedData);

        //
        if (p >= 100.0d)
            return sortedData[sortedData.Length - 1];

        double position = (sortedData.Length + 1) * p / 100.0;
        double leftNumber = 0.0d, rightNumber = 0.0d;

        double n = p / 100.0d * (sortedData.Length - 1) + 1.0d;

        if (position >= 1)
        {
            leftNumber = sortedData[(int)Floor(n) - 1];
            rightNumber = sortedData[(int)Floor(n)];
        }
        else
        {
            leftNumber = sortedData[0]; // first data
            rightNumber = sortedData[1]; // first data
        }

        //if (leftNumber == rightNumber)
        if (Equals(leftNumber, rightNumber))
            return leftNumber;
        double part = n - Floor(n);
        return leftNumber + part * (rightNumber - leftNumber);
    }

    /// <summary>
    /// Calculate percentile of a sorted data set
    /// </summary>
    /// <param name="sortedData"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static double Percentile(this float[] data, double p)
    {

        //make a deep copy of the data
        var sortedData = new double[data.Length];

        Array.Copy(data, sortedData, data.Length);

        //sort the values
        Array.Sort(sortedData);

        //
        if (p >= 100.0d)
            return sortedData[sortedData.Length - 1];

        double position = (sortedData.Length + 1) * p / 100.0;
        double leftNumber = 0.0d, rightNumber = 0.0d;

        double n = p / 100.0d * (sortedData.Length - 1) + 1.0d;

        if (position >= 1)
        {
            leftNumber = sortedData[(int)Floor(n) - 1];
            rightNumber = sortedData[(int)Floor(n)];
        }
        else
        {
            leftNumber = sortedData[0]; // first data
            rightNumber = sortedData[1]; // first data
        }

        //if (leftNumber == rightNumber)
        if (Equals(leftNumber, rightNumber))
            return leftNumber;
        double part = n - Floor(n);
        return leftNumber + part * (rightNumber - leftNumber);
    }

    /// <summary>
    /// Calculate percentile of a sorted data set
    /// </summary>
    /// <param name="sortedData"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static double Percentile(this long[] data, double p)
    {

        //make a deep copy of the data
        var sortedData = new double[data.Length];

        Array.Copy(data, sortedData, data.Length);

        //sort the values
        Array.Sort(sortedData);

        //
        if (p >= 100.0d)
            return sortedData[sortedData.Length - 1];

        double position = (sortedData.Length + 1) * p / 100.0;
        double leftNumber = 0.0d, rightNumber = 0.0d;

        double n = p / 100.0d * (sortedData.Length - 1) + 1.0d;

        if (position >= 1)
        {
            leftNumber = sortedData[(int)Floor(n) - 1];
            rightNumber = sortedData[(int)Floor(n)];
        }
        else
        {
            leftNumber = sortedData[0]; // first data
            rightNumber = sortedData[1]; // first data
        }

        //if (leftNumber == rightNumber)
        if (Equals(leftNumber, rightNumber))
            return leftNumber;
        double part = n - Floor(n);
        return leftNumber + part * (rightNumber - leftNumber);
    }

    /// <summary>
    /// Calculate percentile of a sorted data set
    /// </summary>
    /// <param name="sortedData"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static double Percentile(this int[] data, double p)
    {

        //make a deep copy of the data
        var sortedData = new double[data.Length];

        Array.Copy(data, sortedData, data.Length);

        //sort the values
        Array.Sort(sortedData);

        //
        if (p >= 100.0d)
            return sortedData[sortedData.Length - 1];

        double position = (sortedData.Length + 1) * p / 100.0;
        double leftNumber = 0.0d, rightNumber = 0.0d;

        double n = p / 100.0d * (sortedData.Length - 1) + 1.0d;

        if (position >= 1)
        {
            leftNumber = sortedData[(int)Floor(n) - 1];
            rightNumber = sortedData[(int)Floor(n)];
        }
        else
        {
            leftNumber = sortedData[0]; // first data
            rightNumber = sortedData[1]; // first data
        }

        //if (leftNumber == rightNumber)
        if (Equals(leftNumber, rightNumber))
            return leftNumber;
        double part = n - Floor(n);
        return leftNumber + part * (rightNumber - leftNumber);
    }
    #endregion

    #region STD
    /// <summary>
    /// Calculate Standard Deviation
    /// </summary>
    /// <param name="colData"></param>
    /// <returns></returns>
    public static double Stdev(this double[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or less than 2 elements!");

        //number of elements
        int count = colData.Length;

        //calculate the mean
        var vars = colData.VarianceOfS();

        //calculate sum of square 
        return Sqrt(vars);
    }
    /// <summary>
    /// Calculate Standard Deviation
    /// </summary>
    /// <param name="colData"></param>
    /// <returns></returns>
    public static double Stdev(this int[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or less than 2 elements!");

        //number of elements
        int count = colData.Length;

        //calculate the mean
        var vars = colData.VarianceOfS();

        //calculate sum of square 
        return Sqrt(vars);
    }
    /// <summary>
    /// Calculate Standard Deviation
    /// </summary>
    /// <param name="colData"></param>
    /// <returns></returns>
    public static double Stdev(this long[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or less than 2 elements!");

        //number of elements
        int count = colData.Length;

        //calculate the mean
        var vars = colData.VarianceOfS();

        //calculate sum of square 
        return Sqrt(vars);
    }
    /// <summary>
    /// Calculate Standard Deviation
    /// </summary>
    /// <param name="colData"></param>
    /// <returns></returns>
    public static double Stdev(this float[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or less than 2 elements!");

        //number of elements
        int count = colData.Length;

        //calculate the mean
        var vars = colData.VarianceOfS();

        //calculate sum of square 
        return Sqrt(vars);
    }
    #endregion 

    #region Variance
    /// <summary>
    /// Calculate variance for the whole population.
    /// </summary>
    /// <param name="colData"></param>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public static double VarianceOfP(this double[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or less than 2 elements!");

        //number of elements
        int count = colData.Length;

        //calculate the mean
        var mean = colData.MeanOf();

        //calculate summ of square 
        double parSum = 0;
        for (int i = 0; i < colData.Length; i++)
        {
            var res = colData[i] - mean;

            parSum += res * res;
        }

        return parSum / count;
    }

    /// <summary>
    /// Calculate variance for the sample data .
    /// </summary>
    /// <param name="colData"></param>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public static double VarianceOfS(this double[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or less than 2 elements!");

        //number of elements
        int count = colData.Length;

        //calculate the mean
        var mean = colData.Average();

        //calculate sum of square 
        double parSum = 0;
        for (int i = 0; i < colData.Length; i++)
        {
            var res = colData[i] - mean;

            parSum += res * res;
        }

        return parSum / (count - 1);
    }
    /// <summary>
    /// Calculate variance for the sample data .
    /// </summary>
    /// <param name="colData"></param>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public static double VarianceOfS(this int[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or less than 2 elements!");

        //number of elements
        int count = colData.Length;

        //calculate the mean
        var mean = colData.Average();

        //calculate sum of square 
        double parSum = 0;
        for (int i = 0; i < colData.Length; i++)
        {
            var res = colData[i] - mean;

            parSum += res * res;
        }

        return parSum / (count - 1);
    }
    /// <summary>
    /// Calculate variance for the sample data .
    /// </summary>
    /// <param name="colData"></param>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public static double VarianceOfS(this long[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or less than 2 elements!");

        //number of elements
        int count = colData.Length;

        //calculate the mean
        var mean = colData.Average();

        //calculate sum of square 
        double parSum = 0;
        for (int i = 0; i < colData.Length; i++)
        {
            var res = colData[i] - mean;

            parSum += res * res;
        }

        return parSum / (count - 1);
    }
    /// <summary>
    /// Calculate variance for the sample data .
    /// </summary>
    /// <param name="colData"></param>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public static double VarianceOfS(this float[] colData)
    {
        if (colData == null || colData.Length < 2)
            throw new Exception("'coldData' cannot be null or less than 2 elements!");

        //number of elements
        int count = colData.Length;

        //calculate the mean
        var mean = colData.Average();

        //calculate sum of square 
        double parSum = 0;
        for (int i = 0; i < colData.Length; i++)
        {
            var res = colData[i] - mean;

            parSum += res * res;
        }

        return parSum / (count - 1);
    }
    #endregion

    /// <summary>
    /// Calculation covariance between two vectors
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <returns></returns>
    public static double Covariance(this double[] X, double[] Y)
    {
        if (X == null || X.Length < 2 || Y == null || Y.Length < 2)
            throw new Exception("'data' cannot be null or less than 4 elements!");

        var mx = X.MeanOf();
        var my = Y.MeanOf();
        double Sxy = 0;
        for (int i = 0; i < X.Length; i++)
        {
            Sxy += (X[i] - mx) * (Y[i] - my);
        }
        //divide by number of samples -1
        Sxy = Sxy / (X.Length - 1);
        //

        return Sxy;
    }

    /// <summary>
    /// Calculation of covariance matrix and return as 2D array
    /// </summary>
    /// <param name="Xi">arbitrary number of vectors </param>
    /// <returns></returns>
    public static double[][] Covariance(IList<double[]> Xi)
    {
        //
        var covMat = CovMatrix(Xi);

        var mat = new double[Xi.Count][];
        for (int i = 0; i < Xi.Count; i++)
        {
            mat[i] = new double[Xi.Count];
            for (int j = 0; j < Xi.Count; j++)
                mat[i][j] = covMat[i, j];
        }

        return mat;
    }

    /// <summary>
    /// Calculation of covariance matrix and return Matrix type
    /// </summary>
    /// <param name="Xi">arbitrary number of vectors </param>
    /// <returns></returns>
    public static double[,] CovMatrix(IList<double[]> Xi)
    {

        if (Xi == null || Xi.Count < 2)
            throw new Exception("'data' cannot be null or less than 4 elements!");
        //
        double[,] matrix = new double[Xi.Count, Xi.Count];
        //
        for (int i = 0; i < Xi.Count; i++)
        {
            for (int j = 0; j < Xi.Count; j++)
            {
                if (i > j)
                    matrix[i, j] = matrix[j, i];
                else if (i == j)
                    matrix[i, j] = Xi[i].VarianceOfS();
                else
                    matrix[i, j] = Xi[i].Covariance(Xi[j]);
            }
        }

        //inverse matrix
        try
        {
            var covMat = matrix.Invert();
            return covMat;
        }
        catch
        {

            return MatrixEx.Identity(Xi.Count, Xi.Count);
        }


    }


    /// <summary>
    /// Calculates the minimum and maximum value for each column in dataset
    /// </summary>
    /// <param name="dataset">row based 2D array</param>
    /// <returns>tuple where the first value is MIN, and second value is MAX</returns>
    public static (double[] min, double[] max) calculateMinMax(this double[][] dataset)
    {
        //
        if (dataset == null || dataset.Length == 0)
            throw new Exception("data cannot be null or empty!");

        var colSet = dataset.ToColumnVector();
        var minMax = (new double[dataset[0].Length], new double[dataset[0].Length]);


        for (int i = 0; i < colSet.Length; i++)
        {
            //initialize first values
            minMax.Item1[i] = colSet[i].Min();
            minMax.Item2[i] = colSet[i].Max();
        }

        return minMax;
    }

    /// <summary>
    /// Calculate Mean and Standard deviation
    /// </summary>
    /// <param name="dataset"></param>
    /// <returns>return tuple of mean and StDev</returns>
    public static (double[] means, double[] stdevs) calculateMeanStDev(this double[][] dataset)
    {
        //
        if (dataset == null || dataset.Length <= 1)
            throw new Exception("data cannot be null or empty!");

        var colSet = dataset.ToColumnVector();
        double[] means = new double[dataset[0].Length];
        double[] stdevs = new double[dataset[0].Length];

        //
        for (int i = 0; i < colSet.Length; i++)
        {
            //initialize first values
            means[i] = colSet[i].MeanOf();
            stdevs[i] = colSet[i].Stdev();
        }
        return (means, stdevs);
    }


}
