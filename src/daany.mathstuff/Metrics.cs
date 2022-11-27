//////////////////////////////////////////////////////////////////////////////////////////
// Daany - DAta ANalYtics Library                                                        //
// https://github.com/bhrnjica/daany                                                    //
//                                                                                      //
// Copyright 2006-2022 Bahrudin Hrnjica                                                 //
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
using System.Numerics;


#if NET7_0_OR_GREATER

namespace Daany.MathStuff;

    /// <summary>
    /// Implement statistics metrics.
    /// </summary>
    public class Metrics
    {
        public Metrics()
        {
        }

    public static TResult Sum<T, TResult>(IEnumerable<T> colData) where T : INumber<T> where TResult : INumber<TResult>
    {
        int count = colData.Count();

        if (colData == null || count < 2)
        {
            throw new Exception("'coldData' cannot be null or empty!");
        }

        TResult sum = default;

        for (int i = 0; i < count; i++)
        {
            sum +=  TResult.CreateChecked(colData.ElementAt(i));

        }

        return sum;
    }

    /// <summary>
    /// Calculate mean value of array of numbers.
    /// </summary>
    /// <typeparam name="T">1D column vector array</typeparam>
    /// <typeparam name="TResult">Type of the returned value.</typeparam>
    /// <param name="colData">1D column vector array.</param>
    /// <returns>Returned mean or average value.</returns>
    /// <exception cref="Exception"></exception>
    public static TResult Mean< T, TResult > (IEnumerable< T > colData) where T: INumber< T > where TResult: INumber < TResult >
    {
        int count =  colData.Count();

        if (colData == null || count < 2)
        {
            throw new Exception("'coldData' cannot be null or empty!");
        }

        TResult sum = default;

        for (int i = 0; i < count; i++)
        {
            sum = sum + TResult.CreateChecked( colData.ElementAt(i) );

        }

        TResult retVal = sum / TResult.CreateChecked( count );

        return retVal;
    }


    /// <summary>
    /// Calculate mode value of array of numbers. Mode represent the most frequent element in the array 
    /// </summary>
    /// <typeparam name="T">Type of the input vector.</typeparam>
    /// <param name="colData">1D column vector array.</param>
    /// <returns>Returned mode value.</returns>
    /// <exception cref="Exception"></exception>
    public static T Mode <T> ( IEnumerable<T> colData ) where T : INumber<T> 
    {
        if (colData == null || colData.Count() < 2)
        {
            throw new Exception( "'coldData' cannot be null or empty!" );
        }

        var count = colData.Count();

        var counts = new Dictionary<T, int>();

        for ( int i = 0; i < count; i++ )
        {
            var a = colData.ElementAt<T>( i );

            if (counts.ContainsKey(a))
            {
                counts[a] = counts[a] + 1;
            }
            else
            {
                counts[a] = 1;
            }
        }

        //
        var result = counts.Keys.First();
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
    /// Calculate median value of array of numbers. 
    /// If there is an odd number of data values 
    /// then the median will be the value in the middle. 
    /// If there is an even number of data values the median 
    /// is the mean of the two data values in the middle. 
    /// For the data set 1, 1, 2, 5, 6, 6, 9 the median is 5. 
    /// For the data set 1, 1, 2, 6, 6, 9 the median is 4.
    /// </summary>
    /// <typeparam name="T">Type of the input vector.</typeparam>
    /// <typeparam name="TResult">Type of the returned value.</typeparam>
    /// <param name="colData">1D column vector array.</param>
    /// <returns>Returned median value.</returns>
    /// <exception cref="Exception"></exception>
    public static TResult Median<T, TResult>(IEnumerable< T > colData) where T : INumber<T> where TResult : INumber< TResult>    
    {

        if (colData == null || colData.Count() < 2)
        {
            throw new Exception("'coldData' cannot be null or empty!");
        }

        var count = colData.Count();

        TResult median = default;
        int medianIndex = count / 2;

        var temp = colData.Order().Select(x => x).ToList();

        if (temp.Count % 2 == 1)
        {
 
            median = TResult.CreateChecked( temp.ElementAt( medianIndex ) );

        }
        else 
        {
            TResult val1 = TResult.CreateChecked( temp.ElementAt(medianIndex -1 ) );
            TResult val2 = TResult.CreateChecked( temp.ElementAt(medianIndex) );
            TResult val3 = TResult.CreateChecked( 2 );


            median = ( val1 + val2 ) / val3;
        }

        return median;
    }

    /// <summary>
    /// Calculate percentile of a sorted data set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="colData">Type of the input vector.</param>
    /// <param name="p">Percentage value.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static T Percentile< T >(IEnumerable< T > colData, double p) where T : INumber<T>
    {
        if (colData == null || colData.Count() < 2)
        {
            throw new Exception("'coldData' cannot be null or empty!");
        }

        var count = colData.Count();

        var sortedData = colData.Select(x => x).Order().ToList();

        if (p >= 100.0)
        {
            return sortedData.ElementAt( count - 1);
        }

        double position = (count + 1) * p / 100.0;

        T leftNumber =default, rightNumber = default;

        double n = p / 100.0d * (count - 1) + 1.0d;

        if (position >= 1)
        {
            leftNumber = sortedData.ElementAt( (int)Math.Floor(n) - 1 );

            rightNumber = sortedData.ElementAt( (int)Math.Floor(n) );
        }
        else
        {
            leftNumber = sortedData[0]; 

            rightNumber = sortedData[1];
        }

        if (leftNumber == rightNumber)
        {
            return leftNumber;
        }

        double part = n - Math.Floor(n);

        return leftNumber + T.CreateChecked( part ) * (rightNumber - leftNumber);
    }

    /// <summary>
    /// Calculate variance for the data. If the data is the data is sample data,
    /// it calculates the variance of the sample data, 
    /// othrewize it calculates the variance for the whole population.
    /// </summary>
    /// <typeparam name="T">Type of the input vector.</typeparam>
    /// <typeparam name="TResult">Type of the returned value.</typeparam>
    /// <param name="colData">1D column vector array.</param>
    /// <param name="isDataSample">if true the variance is calculated over the sample data, 
    /// otherwize the variance over the data population is calculated.</param>
    /// <returns>Variance value.</returns>
    /// <exception cref="Exception"></exception>
    public static TResult Variance<T, TResult >( IEnumerable<T> colData, bool isDataSample = true ) where T : INumber<T> where TResult : INumber<TResult>
    {
        if (colData == null || colData.Count() < 2)
        {
            throw new Exception("'coldData' cannot be null or empty!");
        }

        var count = colData.Count();

        //calculate the mean
        TResult mean = Metrics.Mean<T, TResult>( colData );

        //calculate sum of square 
        TResult parSum = default;

        for (int i = 0; i < count; i++)
        {
            TResult res = TResult.CreateChecked ( colData.ElementAt( i ) ) - mean;

            parSum += res * res;
        }

        if(isDataSample) 
        {
            return parSum / TResult.CreateChecked(count - 1);
        }
        else
        {
            return parSum / TResult.CreateChecked(count );
        }
    }

    /// <summary>
    /// Calculation covariance between two vectors.
    /// </summary>
    /// <typeparam name="T">Type of the input vector.</typeparam>
    /// <typeparam name="TResult">Type of the returned value.</typeparam>
    /// <param name="X">First vector</param>
    /// <param name="Y">Second vector</param>
    /// <returns>Covariance value.</returns>
    /// <exception cref="Exception"></exception>
    public static TResult Covariance< T, TResult >(IEnumerable< T > X, IEnumerable<T> Y) where T : INumber<T> where TResult : INumber<TResult>
    {
        if (X == null || X.Count() < 2 || Y == null || Y.Count() < 2)
        {
            throw new Exception("'data' cannot be null or less than 4 elements!");
        }

        var countX = X.Count();
        var countY = Y.Count();


        var mx = Metrics.Mean<T, TResult>( X );
        var my = Metrics.Mean<T, TResult>( Y );


        TResult Sxy = default;

        for ( int i = 0; i < countX; i++ )
        {
            var itemX = TResult.CreateChecked( X.ElementAt(i) );
            var itemY = TResult.CreateChecked( Y.ElementAt(i) );

            Sxy += ( itemX  - mx ) * ( itemY - my );
        }

        //divide by number of samples -1
        Sxy = Sxy / TResult.CreateChecked( countX - 1 );

        return Sxy;
    }

    /// <summary>
    /// Calculate the covariance matrix of the n dimensional data.
    /// </summary>
    /// <typeparam name="T">Type of the input vector.</typeparam>
    /// <typeparam name="TResult">Type of the returned value.</typeparam>
    /// <param name="Xi">Matrix data.</param>
    /// <returns>Returned the covariant matrix.</returns>
    /// <exception cref="Exception"></exception>
    public static TResult[,] CovMatrix< T, TResult >( IEnumerable<IEnumerable<T>> Xi) where T : INumber<T> where TResult : INumber<TResult>
    {

        if (Xi == null || Xi.Count() < 2)
        {
            throw new Exception("'data' cannot be null or less than 4 elements!");
        }

        var xiCount = Xi.Count();
        //
        TResult[,] matrix = new TResult[ Xi.Count(), Xi.Count() ];

        //
        for (int i = 0; i < xiCount; i++)
        {
            for (int j = 0; j < xiCount; j++)
            {
                if (i > j)
                    matrix[i, j] = matrix[j, i];
                else if (i == j)
                    matrix[i, j] = Metrics.Variance<T, TResult> ( Xi.ElementAt( i ) );
                else
                    matrix[i, j] = Metrics.Covariance<T, TResult>(Xi.ElementAt(i), Xi.ElementAt(j)); 
            }
        }

        //inverse matrix
        //try
        //{
        //    var covMat = matrix.Invert();
        //    return covMat;
        //}
        //catch
        //{

        //    return MatrixEx.Identity(xiCount, xiCount);
        //}
        return matrix;

    }

    /// <summary>
    /// Calculate Standard Deviation
    /// </summary>
    /// <typeparam name="T">Type of the input vector.</typeparam>
    /// <typeparam name="TResult">Type of the returned value.</typeparam>
    /// <param name="colData">1D column vector array.</param>
    /// <returns>Return standard deviation value.</returns>
    /// <exception cref="Exception"></exception>
    public static TResult Stdev< T, TResult >( IEnumerable<T> colData ) where T : INumber<T> where TResult : INumber<TResult>  
    {
        if (colData == null || colData.Count() < 2)
        {
            throw new Exception("'coldData' cannot be null or empty!");
        }

        var count = colData.Count();

        var vars = Metrics.Variance<T, TResult>( colData );

        //calculate sum of square 
        return TResult.CreateChecked( Sqrt( Convert.ToDouble( vars ) ) );
    }

    /// <summary>
    /// Select random element from the array.
    /// </summary>
    /// <typeparam name="T">Type of the input vector.</typeparam>
    /// <param name="colData">1D column vector array.</param>
    /// <param name="seed">Random seed.</param>
    /// <returns>Randon element from the array.</returns>
    /// <exception cref="Exception"></exception>
    public static T Random<T>( IEnumerable<T> colData, int seed = 0 )
    {
        if ( colData == null || colData.Count() < 2 )
        {
            throw new Exception("'coldData' cannot be null or empty!");
        }

        Random rand = new Random( seed );

        var randIndex = rand.Next(0, colData.Count() );
        
        return colData.ElementAt<T>( randIndex );
    }

    /// <summary>
    /// Calculate frequency of column vector.
    /// </summary>
    /// <typeparam name="T">Can be any numeric type</typeparam>
    /// <param name="colData">1D column vector array.</param>
    /// <returns>Tuple of value T and count of the value T</returns>
    /// <exception cref="Exception"></exception>
    public static List<(T, int)> Frequency< T >( IEnumerable< T > colData)
    {
        if ( colData == null || colData.Count() < 2 )
        {
            throw new Exception("'coldData' cannot be null or empty!");
        }

        var query = colData.GroupBy(g => g).Select(g => (g.Key, g.Count()));

        return query.OrderByDescending(x => x.Item2).ToList();
    }

    /// <summary>
    /// Calculate Classification Accuracy between two sets.
    /// </summary>
    /// <typeparam name="T">Type of the input sets.</typeparam>
    /// <typeparam name="TResult">Type of the retured value.</typeparam>
    /// <param name="obsData">Actual values.</param>
    /// <param name="preData">Predicted value.</param>
    /// <returns>Returned accuracy value (0-1).</returns>
    public static TResult CA <T, TResult>(IEnumerable<T> obsData, IEnumerable<T> preData) where T : INumber<T> where TResult : INumber<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();
        //
        TResult corected = default;
        for (int i = 0; i < count; i++)
        {
            if (obsData.ElementAt( i )  == preData.ElementAt(i))
                corected++;
        }

        return corected / TResult.CreateChecked(count);
    }

    /// <summary>
    /// Calculate Classification Error between two sets.
    /// </summary>
    /// <typeparam name="T">Type of the input sets.</typeparam>
    /// <typeparam name="TResult">Type of the retured value.</typeparam>
    /// <param name="obsData">Actual values.</param>
    /// <param name="preData">Predicted value.</param>
    /// <returns>Returned error value (0-1)</returns>
    public static TResult CE <T, TResult>(IEnumerable<T> obsData, IEnumerable<T> preData) where T : INumber<T> where TResult : INumber<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();
        //
        TResult corected = default;
        TResult one = TResult.CreateChecked(1);
        for (int i = 0; i < count; i++)
        {
            if (obsData.ElementAt(i) == preData.ElementAt(i))
                corected++;
        }

        return one - corected / TResult.CreateChecked(count);
    }

    /// <summary>
    /// Calculates squares of the two vectors
    /// </summary>
    /// <param name="obsData">Observer data</param>
    /// <param name="preData">Predicted data</param>
    /// <returns></returns>
    public static IEnumerable< TResult > SE< T, TResult >( IEnumerable< T >  obsData, IEnumerable<T> preData) where T : INumber<T> where TResult : INumber<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();

        //calculate sum of the square residuals
        TResult[] se = new TResult[ count ];

        for (int i = 0; i < count; i++)
        {
            var r = TResult.CreateChecked( obsData.ElementAt( i )) - TResult.CreateChecked( preData.ElementAt(i));

            se[i] = r * r;
        }

        return se;
    }


    public static TResult MSE< T, TResult >(IEnumerable< T > obsData, IEnumerable<T> preData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();

        var se = SE <T, TResult>(obsData, preData);

        TResult sss = Sum<T,TResult>(obsData);

        return sss / TResult.CreateChecked( count );
    }

    public static TResult RMSE< T, TResult >(IEnumerable<T> obsData, IEnumerable<T> preData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        return TResult.Sqrt( MSE<T, TResult>(obsData, preData) );
    }

    public static IEnumerable<TResult> AE<T, TResult>( IEnumerable<T> obsData, IEnumerable<T> preData ) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();    
        //
        TResult[] ae = new TResult[count];
        for (int i = 0; i < count; i++)
        {
            var r = TResult.Abs( TResult.CreateChecked( obsData.ElementAt(i)) - TResult.CreateChecked( preData.ElementAt(i)));
            
            ae[i] = r;
        }
        return ae;
    }

    public static TResult MAE<T, TResult>( IEnumerable<T> obsData, IEnumerable<T> preData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();

        var ae = AE<T, TResult>(obsData, preData);

        var sumare = Sum<TResult, TResult>(ae);
        
        return sumare / TResult.CreateChecked( count ) ;
    }

    public static IEnumerable<TResult> APE<T, TResult > (IEnumerable< T > obsData, IEnumerable<T> preData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();

        //
        TResult[] retVal = new TResult[count];

        for (int i = 0; i < count; i++)
        {
            var v1 = TResult.CreateChecked(obsData.ElementAt(i));
            var v2 = TResult.CreateChecked(preData.ElementAt(i));
            var r = TResult.Abs(v1 - v2) / v1;
            
            retVal[i] = r;
        }

        return retVal;
    }

    public static TResult MAPE<T, TResult>(IEnumerable<T> obsData, IEnumerable<T> preData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();

        var ae = APE<T, TResult>(obsData, preData);

        var sum = Sum<TResult, TResult>(ae);
        //c
        return sum / TResult.CreateChecked( count );

    }
    private static void checkDataSets< T >( T[] obsData, T[] preData)
    {
        if (obsData == null || obsData.Length < 2)
            throw new Exception("'observed Data' cannot be null or empty!");

        if (preData == null || preData.Length < 2)
            throw new Exception("'predicted data' cannot be null or empty!");

        if (obsData.Length != preData.Length)
            throw new Exception("Both datasets must be of the same size!");
    }

    private static void checkDataSets<T>(IEnumerable<T> obsData, IEnumerable<T> preData)
    {
        if (obsData == null || obsData.Count() < 2)
            throw new Exception("'observed Data' cannot be null or empty!");

        if (preData == null || preData.Count() < 2)
            throw new Exception("'predicted data' cannot be null or empty!");

        if (obsData.Count() != preData.Count())
            throw new Exception("Both datasets must be of the same size!");
    }
}

#endif


