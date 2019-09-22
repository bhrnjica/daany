using System;
//The code is converted from the java version found at https://github.com/ServiceNow/stl-decomp-4j
namespace Daany.stl
{
    public enum State
    {
        WEIGHTS_FAILED,
        LINEAR_FAILED,
        LINEAR_OK,
    }

    public class InterpolatorBuilder
    {

        private int fWidth = 0;
        private int fDegree = 1;
        private double[] fExternalWeights = null;

        public int Width
        {
            get
            {
                return this.fWidth;
            }
          set
            {
                this.fWidth = value;
            }
        }

        public int Degree
        {
            get
            {
                return this.fDegree;
            }
            set
            {
                if (value < 0 || value > 2)
                {
                    throw new Exception("Degree must be 0, 1 or 2");
                }

                this.fDegree = value;
            }
        }

        public double[] ExternalWeights
        {
            get
            {
                return this.fExternalWeights;
            }
            set
            {
                this.fExternalWeights = value;
            }
        }

        public LoessInterpolator interpolate(double[] data)
        {
            if ((this.fWidth == 0))
            {
                throw new Exception("LoessInterpolator.Builder: Width must be set");
            }

            if ((data == null))
            {
                throw new Exception("LoessInterpolator.Builder: data must be non-null");
            }

            switch (this.fDegree)
            {
                case 0:
                    return new FlatLoessInterpolator(this.fWidth, data, this.fExternalWeights);
                case 1:
                    return new LinearLoessInterpolator(this.fWidth, data, this.fExternalWeights);
                case 2:
                    return new QuadraticLoessInterpolator(this.fWidth, data, this.fExternalWeights);
                default:
                    return null;
            }
        }
    }

    public abstract class LoessInterpolator
    {
        private int fWidth;
        private double[] fExternalWeights;
        protected double[] fData;
        protected double[] fWeights;


        public LoessInterpolator(int width, double[] data, double[] externalWeights)
        {
            this.fWidth = width;
            this.fData = data;
            this.fExternalWeights = externalWeights;
            this.fWeights = new double[data.Length];
        }

        public double smoothOnePoint(double x, int left, int right)
        {
            //  Ordinarily, one doesn't do linear regression one x-value at a time, but LOESS does since
            //  each x-value will typically have a different window. As a result, the weighted linear regression
            //  is recast as a linear operation on the input data, weighted by this.fWeights.
            State state = this.computeNeighborhoodWeights(x, left, right);
            if ((state == State.WEIGHTS_FAILED))
            {
                return 0;
            }

            if ((state == State.LINEAR_OK))
            {
                this.updateWeights(x, left, right);
            }

            double ys = 0;
            for (int i = left; (i <= right); i++)
            {
                ys = (ys  + (this.fWeights[i] * this.fData[i]));
            }

            return ys;
        }

        public abstract void updateWeights(double x, int left, int right);

        private State computeNeighborhoodWeights(double x, int left, int right)
        {
            double lambda = Math.Max((x - left), (right - x));
            //  Ordinarily, lambda ~ width / 2.
            // 
            //  If width > n, then we will only be computing with n points (i.e. left and right will always be in the
            //  domain of 1..n) and the above calculation will give lambda ~ n / 2. We want the shape of the neighborhood
            //  weight function to be driven by width, not by the size of the domain, so we adjust lambda to be ~ width / 2.
            //  (The paper does this by multiplying the above lambda by (width / n). Not sure why the code is different.)
            if ((this.fWidth > this.fData.Length))
            {
                lambda = (lambda + ((double)(((this.fWidth - this.fData.Length)/ 2))));
            }

            //  "Neighborhood" is computed somewhat fuzzily.
            double l999 = (0.999 * lambda);
            double l001 = (0.001 * lambda);
            //  Compute neighborhood weights, updating with external weights if supplied.
            double totalWeight = 0;
            for (int j = left; (j <= right); j++)
            {
                double delta = Math.Abs((x - j));
                //  Compute the tri-cube neighborhood weight
                double weight = 0;
                if ((delta <= l999))
                {
                    if ((delta <= l001))
                    {
                        weight = 1;
                    }
                    else
                    {
                        double fraction = (delta / lambda);
                        double trix = (1
                                    - (fraction
                                    * (fraction * fraction)));
                        weight = (trix * (trix * trix));
                    }

                    //  If external weights are provided, apply them.
                    if ((this.fExternalWeights != null))
                    {
                        weight = (weight * this.fExternalWeights[j]);
                    }

                    totalWeight = (totalWeight + weight);
                }

                this.fWeights[j] = weight;
            }

            //  If the total weight is 0, we can't proceed, so signal failure.
            if ((totalWeight <= 0))
            {
                return State.WEIGHTS_FAILED;
            }

            //  Normalize the weights
            for (int j = left; (j <= right); j++)
            {
                fWeights[j] = fWeights[j]/totalWeight;
            }

            //totalWeight;
            return (lambda > 0) ? State.LINEAR_OK : State.LINEAR_FAILED;
        }

      
    }

    public class FlatLoessInterpolator : LoessInterpolator
    {

        public FlatLoessInterpolator(int width, double[] data, double[] externalWeights) :
                base(width, data, externalWeights)
        {
            
        }

        public override void updateWeights(double x, int left, int right)
        {
             
        }

        
    }

    public class LinearLoessInterpolator : LoessInterpolator
    {

        public LinearLoessInterpolator(int width, double[] data, double[] externalWeights) :
                base(width, data, externalWeights)
        {
             
        }

        public override void updateWeights(double x, int left, int right)
        {
            double xMean = 0;
            for (int i = left; i <= right; ++i)
                xMean += i * fWeights[i];

            double x2Mean = 0.0;
            for (int i = left; i <= right; ++i)
            {
                double delta = i - xMean;
                x2Mean += fWeights[i] * delta * delta;
            }

            //  Finding y(x) from the least-squares fit can be cast as a linear operation on the input data.
            //  This is implemented by updating the weights to include the least-squares weighting of the points.
            //  Note that this is only done if the points are spread out enough (variance > (0.001 * range)^2)
            //  to compute a slope. If not, we leave the weights alone and essentially fall back to a moving
            //  average of the data based on the neighborhood and external weights.
            double range = fData.Length - 1;
            if (x2Mean > 0.000001 * range * range)
            {
                double beta = (x - xMean) / x2Mean;

                for (int i = left; i <= right; ++i)
                    fWeights[i] = fWeights[i] * (1.0 + beta * (i - xMean));
            }

        }
    }

    public class QuadraticLoessInterpolator : LoessInterpolator
    {

       public QuadraticLoessInterpolator(int width, double[] data, double[] externalWeights) :
                base(width, data, externalWeights)
        {
            
        }

        public override void updateWeights(double x, int left, int right)
        {
            double x1Mean = 0;
            double x2Mean = 0;
            double x3Mean = 0;
            double x4Mean = 0;
            for (int i = left; (i <= right); i++)
            {
                double w = fWeights[i];
                double x1w = i * w;
                double x2w = i * x1w;
                double x3w = i * x2w;
                double x4w = i * x3w;
                x1Mean += x1w;
                x2Mean += x2w;
                x3Mean += x3w;
                x4Mean += x4w;
            }

            double m2 = x2Mean - x1Mean * x1Mean;
            double m3 = x3Mean - x2Mean * x1Mean;
            double m4 = x4Mean - x2Mean * x2Mean;

            double denominator = m2 * m4 - m3 * m3;
            double range = fData.Length - 1;

            if (denominator > (1E-06 * range * range))
            {
                //  TODO: Are there cases where denominator is too small but m2 is not too small?
                //  In that case, it would make sense to fall back to linear regression instead of falling back to just the
                //  weighted average.
                double beta2 = m4 / denominator;
                double beta3 = m3 / denominator;
                double beta4 = m2 / denominator;

                double x1 = x - x1Mean;
                double x2 = x * x - x2Mean;

                double a1 = beta2 * x1 - beta3 * x2;
                double a2 = beta4 * x2 - beta3 * x1;

                for (int i = left; i <= right; ++i)
                    fWeights[i] = fWeights[i] * (1 + a1 * (i - x1Mean) + a2 * (i * i - x2Mean));

            }

        }
    }
}