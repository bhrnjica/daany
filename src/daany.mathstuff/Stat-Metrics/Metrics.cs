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

namespace Daany.MathStuff.Stats;

/// <summary>
/// Implement statistics metrics.
/// </summary>
public class Metrics
{
    public Metrics()
    {
    }

   

    public static TResult Sum<T, TResult>(IList<T> colData) where T : INumber<T> where TResult : INumber<TResult>
    {
        int count = colData.Count();

        if (colData == null || count < 2)
        {
            throw new Exception("'coldData' should contains at least 2 elements!");
        }

        TResult sum = default;

        for (int i = 0; i < count; i++)
        {
            sum +=  TResult.CreateChecked(colData[i]);

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
    public static TResult Mean< T, TResult > (IList<T> colData) where T: INumber< T > where TResult: INumber < TResult >
    {
        int count =  colData.Count();

        if (colData == null || count < 2)
        {
            throw new Exception("'coldData' should contains at least 2 elements!");
        }

        TResult sum = default;

        for (int i = 0; i < count; i++)
        {
            sum = sum + TResult.CreateChecked( colData[i] );

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
    public static T Mode <T> ( IList<T> colData ) where T : INumber<T> 
    {
        if (colData == null || colData.Count() < 2)
        {
            throw new Exception("'coldData' should contains at least 2 elements!");
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
    public static TResult Median<T, TResult>(IList<T> colData) where T : INumber<T> where TResult : INumber< TResult>    
    {

        if (colData == null || colData.Count() < 2)
        {
            throw new Exception("'coldData' should contains at least 2 elements!");
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
    public static T Percentile< T >(IList<T> colData, double p) where T : INumber<T>
    {
        if (colData == null || colData.Count() < 2)
        {
            throw new Exception("'coldData' should contains at least 2 elements!");
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
    public static TResult Variance<T, TResult >( IList<T> colData, bool isDataSample = true ) where T : INumber<T> where TResult : INumber<TResult>
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
    public static TResult Covariance< T, TResult >(IList<T> X, IList<T> Y) where T : INumber<T> where TResult : INumber<TResult>
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
            var itemX = TResult.CreateChecked( X[i] );
            var itemY = TResult.CreateChecked( Y[i] );

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
    public static TResult[,] CovMatrix< T, TResult >( IList<IList<T>> Xi) where T : INumber<T> where TResult : INumber<TResult>
    {

        if (Xi == null || Xi.Count() < 2)
        {
            throw new Exception("'data' cannot be null or less than 2 elements!");
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
                    matrix[i, j] = Metrics.Variance<T, TResult>(Xi[i]);
                else
                    matrix[i, j] = Metrics.Covariance<T, TResult>(Xi[i], Xi[j]); 
            }
        }

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
    public static TResult Stdev< T, TResult >( IList<T> colData ) where T : INumber<T> where TResult : INumber<TResult>  
    {
        if (colData == null || colData.Count() < 2)
        {
            throw new Exception("'coldData' should contains at least 2 elements!");
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
    public static T Random<T>( IList<T> colData, int seed = 0 )
    {
        if ( colData == null || colData.Count() < 2 )
        {
            throw new Exception("'coldData' should contains at least 2 elements!");
        }

        var rand = new System.Random( seed );

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
    public static List<(T, int)> Frequency< T >( IList<T> colData)
    {
        if ( colData == null || colData.Count() < 2 )
        {
            throw new Exception("'coldData' should contains at least 2 elements!!");
        }

        var query = colData.GroupBy(g => g).Select(g => (g.Key, g.Count()));

        return query.OrderByDescending(x => x.Item2).ToList();
    }

    public static TResult RSquared<T, TResult>(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        //calculate average for each dataset
        var r = R<T, TResult>(preData, obsData);

        return r*r;
    }

    public static TResult R<T, TResult>(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        //calculate average for each dataset
        TResult aav = Mean<T, TResult>(obsData);
        TResult bav = Mean<T, TResult>(preData);

        var count = obsData.Count();

        TResult corr = default;
        TResult ab = default, aa = default, bb = default;
        for (int i = 0; i < count; i++)
        {
            var a = TResult.CreateChecked(obsData[i]) - aav;
            var b = TResult.CreateChecked(preData[i]) - bav;

            ab += a * b;
            aa += a * a;
            bb += b * b;
        }

        corr = ab / TResult.Sqrt(aa * bb);

        return corr;
    }


    /// <summary>
    /// Calculate Classification Accuracy between two sets.
    /// </summary>
    /// <typeparam name="T">Type of the input sets.</typeparam>
    /// <typeparam name="TResult">Type of the retured value.</typeparam>
    /// <param name="obsData">Actual values.</param>
    /// <param name="preData">Predicted value.</param>
    /// <returns>Returned accuracy value (0-1).</returns>
    public static TResult CA <T, TResult>(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : INumber<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();
        //
        TResult corected = default;
        for (int i = 0; i < count; i++)
        {
            if (obsData.ElementAt( i )  == preData[i])
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
    public static TResult CE <T, TResult>(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : INumber<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();
        //
        TResult corected = default;
        TResult one = TResult.CreateChecked(1);
        for (int i = 0; i < count; i++)
        {
            if (obsData[i] == preData[i])
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
    public static IList<TResult> SE< T, TResult >(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : INumber<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();

        //calculate sum of the square residuals
        TResult[] se = new TResult[ count ];

        for (int i = 0; i < count; i++)
        {
            var r = TResult.CreateChecked( obsData[i]) - TResult.CreateChecked( preData[i]);

            se[i] = r * r;
        }

        return se;
    }


    public static TResult MSE< T, TResult >(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();

        var se = SE <T, T>(obsData, preData);

        TResult sss = Sum<T,TResult>( se );

        return sss / TResult.CreateChecked( count );
    }

    public static TResult RMSE< T, TResult >(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        return TResult.Sqrt( MSE<T, TResult>(obsData, preData) );
    }

    public static IList<TResult> AE<T, TResult>(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();    
        //
        TResult[] ae = new TResult[count];
        for (int i = 0; i < count; i++)
        {
            var r = TResult.Abs( TResult.CreateChecked( obsData[i]) - TResult.CreateChecked( preData[i]));
            
            ae[i] = r;
        }
        return ae;
    }

    public static TResult MAE<T, TResult>(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();

        var ae = AE<T, TResult>(obsData, preData);

        var sumare = Sum<TResult, TResult>(ae);
        
        return sumare / TResult.CreateChecked( count ) ;
    }

    public static IList<TResult> APE<T, TResult > (IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();

        //
        TResult[] retVal = new TResult[count];

        for (int i = 0; i < count; i++)
        {
            var v1 = TResult.CreateChecked(obsData[i]);
            var v2 = TResult.CreateChecked(preData[i]);

            if (v1 == default)
            {
                retVal[i] = default;
                continue;
            }
            var r = TResult.Abs(v1 - v2) / v1;
            
            retVal[i] = r;
        }

        return retVal;
    }

    public static TResult MAPE<T, TResult>(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        checkDataSets(obsData, preData);

        var count = obsData.Count();

        var ae = APE<T, TResult>(obsData, preData);

        var sum = Sum<TResult, TResult>(ae);
        //c
        return sum / TResult.CreateChecked( count );

    }


    /// <summary>
    /// The Nash-Sutcliffe efficiency (NSE)  proposed by Nash and Sutcliffe (1970) 
    /// is defined as one minus the sum of the absolute squared differences between 
    /// the predicted and observed values normalized by the variance of the observed
    /// values during the period under investigation.
    /// 
    /// The Nash-Sutcliffe efficiency (NSE) is a normalized statistic
    /// that determines the relative magnitude of the residual variance (“noise”) compared to the 
    /// measured data variance (“information”) (Nash and Sutcliffe, 1970). NSE indicates how well 
    /// the plot of observed versus simulated data fits the 1:1 line. 
    /// </summary>
    /// <param name="obsData">Observer data</param>
    /// <param name="preData">Predicted data</param>
    /// <returns>NSE ranges between −∞ and 1.0 (1 inclusive), with NSE = 1 being the optimal value. 
    /// Values between 0.0 and 1.0 are generally viewed as acceptable levels of performance, 
    /// whereas values lower than 0.0 indicates that the mean observed value is a better predictor 
    /// than the simulated value, which indicates unacceptable performance.</returns>
    public static TResult NSE<T, TResult>(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        var se = SE<T, TResult>(obsData, preData);
        var sse = Sum<TResult, TResult>(se);
       
        var count = obsData.Count();


        //calculate the mean
        var mean = Mean<T, T>(obsData);
        //calculate sum of square 
        TResult ose = default;
        for (int i = 0; i < count; i++)
        {
            var res = obsData[i] - mean;

            ose += TResult.CreateChecked(res * res);
        }

        //calculate NSE
        var nse = TResult.CreateChecked(1.0) - sse / ose;
        return nse;
    }

    /// <summary>
    /// Normalized NSE: https://en.wikipedia.org/wiki/Nash%E2%80%93Sutcliffe_model_efficiency_coefficient
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="preData"></param>
    /// <param name="obsData"></param>
    /// <returns></returns>
    public static TResult NNSE<T, TResult>(IList<T> preData, IList<T> obsData) where T : INumber<T> where TResult : IFloatingPointIeee754<TResult>
    {
        var nse = NSE<T, TResult>(obsData, preData);
        //calculate NNSE
        return TResult.CreateChecked(1.0) / (TResult.CreateChecked(2.0) - nse);

    }


    private static void checkDataSets< T >( T[] preData, T[] obsData)
    {
        if (obsData == null || obsData.Length < 2)
            throw new Exception("'observed Data' should contains at least 2 elements!");

        if (preData == null || preData.Length < 2)
            throw new Exception("'predicted data' should contains at least 2 elements!");

        if (obsData.Length != preData.Length)
            throw new Exception("Both datasets must be of the same size!");
    }

    private static void checkDataSets<T>(IList<T> preData, IList<T> obsData)
    {
        if (obsData == null || obsData.Count() < 2)
            throw new Exception("'observed Data' should contains at least 2 elements!");

        if (preData == null || preData.Count() < 2)
            throw new Exception("'predicted data' should contains at least 2 elements!");

        if (obsData.Count() != preData.Count())
            throw new Exception("Both datasets must be of the same size!");
    }
}

#endif


