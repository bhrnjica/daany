using System;
using System.Collections.Generic;
using System.Text;

namespace Daany.MathExt
{
    /// <summary>
    /// Implementation of various distance
    /// </summary>
    public static class Distance
    {
        /// <summary>
        /// Calculate the great circle distance between two points on the earth
        /// </summary>
        /// <param name="loStart"></param>
        /// <param name="laStart"></param>
        /// <param name="loEnd"></param>
        /// <param name="laEnd"></param>
        public static void Haversine(double loStart, double laStart, double loEnd, double laEnd)
        {
            var dlo = laEnd - loStart;
            var dla = loEnd - laStart;

            var a = Math.Pow(Math.Sin(dla / 2.0), 2) + Math.Cos(laStart) * Math.Cos(laEnd) * Math.Pow(Math.Sin(dlo / 2.0), 2);

            var c = 2 * Math.Asin(Math.Sqrt(a));
            var km = 6371 * c;//# 6371 is Radius of earth in kilometers. Use 3956 for miles
        }

        public static double Euclidian(double x1, double y1, double x2, double y2)
        {
           
            //return value
            return Math.Sqrt((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));

        }

        public static double Manhattan(double x1, double y1, double x2, double y2)
        {

            //return value
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

        }

        /// <summary>
        /// Calculate Mahanalobis distance of vector using mean and covariance matrix
        /// </summary>
        /// <param name="vector"> vector distance is calculating</param>
        /// <param name="mean">mean value </param>
        /// <param name="covMatrix">cov matrix</param>
        /// <returns></returns>
        public static double Mahanalobis(double[] vector, double[] mean, double[][] covMatrix)
        {
            //create matrix from arrays
            var covM = new double[covMatrix.Length, covMatrix.Length];// Matrix(covMatrix.Length, covMatrix.Length);
            var m = new double[1, mean.Length]; //new Matrix(1,mean.Length);
            var v = new double[1, vector.Length]; //new Matrix(1, vector.Length);
            //init matrices
            for (int i = 0; i < vector.Length; i++)
            {
                v[0, i] = vector[i];
                m[0, i] = mean[i];
                for (int j = 0; j < vector.Length; j++)
                    covM[i, j] = covMatrix[i][j];
            }


            //perform calculation
            var vm = v.Substract(m);// v - m;
            var trm = vm.Transpose();//Matrix.Transpose(vm);
            var tmp = vm.Dot(covM);//(vm * covM);
            var retVal = Math.Sqrt(tmp.Dot(trm)[0, 0]);// //Sqrt((tmp * trm)[0, 0]);
            //return value
            return retVal;

        }
    }
}
