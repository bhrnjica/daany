using System;
using System.Collections.Generic;
using System.Text;

namespace Daany.Optimizers
{
    public class Optimization
    {
        public static double[] GradientDescent(List<double[]> X, List<double> Y, double lr, int it)
        {
            var colCount = X[0].Length;
            var initW = new double[colCount];
            for (int i = 0; i < it; i++)
                initW = StepG(initW, X, Y, lr);
            return initW;

        }
        private static double[] StepG(double[] W, List<double[]> X, List<double> Y, double lr)
        {
            int numFeature = X[0].Length;
            int rowCount = Y.Count;
            var deltaW = new double[numFeature];

            for (int j = 0; j < rowCount; j++)
            {
                var x = X[j];
                var y = Y[j];
                var val = 0.0;

                for (int i = 0; i < numFeature; i++)
                    val += W[i] * x[i];
                //
                for (int i = 0; i < numFeature; i++)
                {
                    deltaW[i] += (val - y) * x[i];
                }
            }

            for (int i = 0; i < numFeature; i++)
            {
                W[i] = W[i] - lr * (2.0 / (double)rowCount) * deltaW[i];
            }

            return W;
        }

    }
}
