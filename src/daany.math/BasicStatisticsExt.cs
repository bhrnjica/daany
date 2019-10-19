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
namespace Daany.MathExt
{
    /// <summary>
    /// Implement extension methods for statistics calculation on one data set eg. mean, median, variance,... 
    /// Modul calculate mean value of array of numbers. 
    /// The mean is the average of the numbers.
    /// </summary>
    public static class BasicStatistics
    {
        /// <summary>
        /// Calculate mode value of array of numbers. Mode represent the most frequent element in the array 
        /// </summary>
        /// <param name="colData"> array of values </param>
        /// <returns>calculated mode</returns>
        //public static double ModeOf(this double[] colData)
        //{
        //    if (colData == null || colData.Length < 2)
        //        throw new Exception("'coldData' cannot be null or empty!");

        //    Dictionary<int, int> counts = new Dictionary<int, int>();
        //    foreach (int a in colData)
        //    {
        //        if (counts.ContainsKey(a))
        //            counts[a] = counts[a] + 1;
        //        else
        //            counts[a] = 1;
        //    }

        //    int result = int.MinValue;
        //    int max = int.MinValue;
        //    foreach (int key in counts.Keys)
        //    {
        //        if (counts[key] > max)
        //        {
        //            max = counts[key];
        //            result = key;
        //        }
        //    }

        //    return result;
        //}

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
            for(int i =0; i < colData.Length; i++)
            {
                var a = colData[i];

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

        ///// <summary>
        ///// Select random element from the array
        ///// </summary>
        ///// <param name="colData"> array of values </param>
        ///// <returns>random element</returns>
        //public static double RandomOf(this double[] colData)
        //{
        //    if (colData == null || colData.Length < 2)
        //        throw new Exception("'coldData' cannot be null or empty!");

        //    Random rand = new Random((int)DateTime.Now.Ticks);
        //    var randIndex= rand.Next(0,colData.Length);
        //    return colData[randIndex];
        //}

        /// <summary>
        /// Select random element from the array
        /// </summary>
        /// <param name="colData"> array of values </param>
        /// <returns>random element</returns>
        public static T RandomOf<T>(this T[] colData)
        {
            if (colData == null || colData.Length < 2)
                throw new Exception("'coldData' cannot be null or empty!");

            Random rand = new Random((int)DateTime.Now.Ticks);
            var randIndex = rand.Next(0, colData.Length);
            return colData[randIndex];
        }


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
            for(int i=0; i < colData.Length; i++)
                sum += colData[i];

            //calculate mean
            double retVal = sum / colData.Length;

            return retVal;
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
        public static double MedianOf(this double[] colData)
        {
            if (colData == null || colData.Length < 2)
                throw new Exception("'coldData' cannot be null or empty!");

            return Percentile(colData, 50);
        }

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
                leftNumber = sortedData[(int)Math.Floor(n) - 1];
                rightNumber = sortedData[(int)Math.Floor(n)];
            }
            else
            {
                leftNumber = sortedData[0]; // first data
                rightNumber = sortedData[1]; // first data
            }

            //if (leftNumber == rightNumber)
            if (Equals(leftNumber, rightNumber))
                return leftNumber;
            double part = n - Math.Floor(n);
            return leftNumber + part * (rightNumber - leftNumber);
        } 



        /// <summary>
        /// Calculate variance for the sample data .
        /// </summary>
        /// <param name="colData"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static double VarianceOfS(this double[] colData)
        {
            if (colData == null || colData.Length < 3)
                throw new Exception("'coldData' cannot be null or less than 4 elements!");
            
            //number of elements
            int count = colData.Length;

            //calculate the mean
            var mean = colData.MeanOf();

            //calculate summ of square 
            double parSum = 0;
            for (int i = 0; i < colData.Length; i++)
            {
                var res = (colData[i] - mean);

                parSum += res*res;
            }
                
            return parSum/(count-1);
        }

        /// <summary>
        /// Calculation covariance brtween two vectors
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
        public static double[][] Covariance( IList<double[]> Xi)
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
                        matrix[i, j] = VarianceOfS(Xi[i]);
                    else
                        matrix[i, j] = Covariance(Xi[i], Xi[j]);
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
        /// Calculate Standard Deviation
        /// </summary>
        /// <param name="colData"></param>
        /// <returns></returns>
        public static double Stdev(this double[] colData)
        {
            if (colData == null || colData.Length < 3)
                throw new Exception("'coldData' cannot be null or less than 4 elements!");

            //number of elements
            int count = colData.Length;

            //calculate the mean
            var vars = colData.VarianceOfS();

            //calculate summ of square 
            return Sqrt(vars);
        }

        /// <summary>
        /// Calculate variance for the whole population.
        /// </summary>
        /// <param name="colData"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static double VarianceOfP(this double[] colData)
        {
            if (colData == null || colData.Length < 4)
                throw new Exception("'coldData' cannot be null or less than 4 elements!");

            //number of elements
            int count = colData.Length;

            //calculate the mean
            var mean = colData.MeanOf();

            //calculate summ of square 
            double parSum = 0;
            for (int i = 0; i < colData.Length; i++)
            {
                var res = (colData[i] - mean);

                parSum += res * res;
            }

            return parSum / count;
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

            var colSet = dataset.ToColumnVector<double>();
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

            var colSet = dataset.ToColumnVector<double>();
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
}
