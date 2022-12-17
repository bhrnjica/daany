using System;
//The code is converted from the java version found at https://github.com/ServiceNow/stl-decomp-4j
namespace Daany.Stat.stl
{
    /// <summary>
    /// Statistics of the STL decomposed time series components
    /// </summary>
    public class StlFitStats
    {
        //Private fields 
        private int fSampleSize;
        private double fDataMean;
        private double fDataVariance;
        private double fTrendMean;
        private double fTrendRange;
        private double fSeasonalMean;
        private double fSeasonalVariance;
        private double fResidualMean;
        private double fResidualVariance;
        private double fResidualLogLikelihood;
        private double fDeSeasonalMean;
        private double fDeSeasonalVariance;
        private double fDeTrendMean;
        private double fDeTrendVariance;
        private double fSeasonalRange;
        private double fResidualVarMLE;

        public StlFitStats(Decomposition stl)
        {
            int length = stl.Data.Length;
            //  Unnecessary since STL guarantees this, so it can't be tested:
            //  Preconditions.checkArgument(length >= 4, "STL Decomposition must have at least 4 data points");
            double[] data = stl.Data;
            double[] trend = stl.Trend;
            double[] seasonal = stl.Seasonal;
            double[] residuals = stl.Residual;
            double dataSum = 0;
            double dataSqSum = 0;
            double trendSum = 0;
            double trendMax = -1E+100;
            double trendMin = 1E+100;
            double seasonalSum = 0;
            double seasonalSqSum = 0;
            double seasonalMax = -1E+100;
            double seasonalMin = 1E+100;
            double residualSum = 0;
            double residualSqSum = 0;
            double deSeasonalSum = 0;
            double deSeasonalSqSum = 0;
            double deTrendSum = 0;
            double deTrendSqSum = 0;
            for (int i = 0; (i < length); i++)
            {
                double d = data[i];
                double t = trend[i];
                double s = seasonal[i];
                double r = residuals[i];
                double f = d - s;
                double dt = d - t;
                dataSum =  dataSum + d ;
                dataSqSum =  dataSqSum +  d * d ;
                trendSum =  trendSum + t ;
                if (t > trendMax)
                {
                    trendMax = t;
                }

                if (t < trendMin)
                {
                    trendMin = t;
                }

                seasonalSum = seasonalSum + s;
                seasonalSqSum = seasonalSqSum + s * s;

                if (s > seasonalMax)
                {
                    seasonalMax = s;
                }

                if (s < seasonalMin)
                {
                    seasonalMin = s;
                }

                residualSum = residualSum + r;
                residualSqSum = residualSqSum + r * r;
                deSeasonalSum = deSeasonalSum + f;
                deSeasonalSqSum = deSeasonalSqSum + f * f;
                deTrendSum = deTrendSum + dt;
                deTrendSqSum = deTrendSqSum + dt * dt;
            }

            double denom = 1.0 / length;

            fDataMean = dataSum * denom;
            fTrendMean = trendSum * denom;
            fSeasonalMean = seasonalSum * denom;
            fResidualMean = residualSum * denom;
            fDeSeasonalMean = deSeasonalSum * denom;
            fDeTrendMean = deTrendSum * denom;

            // The data is from a valid STL decomposition, so length = 4 at minimum.

            double corrBC = length / (length - 1.0); // Bessel's correction
            double denomBC = 1.0 / (length - 1.0);

            fDataVariance = dataSqSum * denomBC - fDataMean * fDataMean * corrBC;
            fTrendRange = trendMax - trendMin;
            fSeasonalVariance = seasonalSqSum * denomBC - fSeasonalMean * fSeasonalMean * corrBC;
            fSeasonalRange = seasonalMax - seasonalMin;
            fResidualVariance = residualSqSum * denomBC - fResidualMean * fResidualMean * corrBC;
            fDeSeasonalVariance = deSeasonalSqSum * denomBC - fDeSeasonalMean * fDeSeasonalMean * corrBC;
            fDeTrendVariance = deTrendSqSum * denomBC - fDeTrendMean * fDeTrendMean * corrBC;

            fResidualVarMLE = denom * residualSqSum;
            fResidualLogLikelihood = -0.5 * length * (1 + Math.Log(2 * Math.PI * fResidualVarMLE));

            fSampleSize = length;
        }

        //Public properties for Statistics 
        public double TrendMean { get { return this.fTrendMean; } }
        public double TrendRange { get { return this.fTrendRange; } }
        public double DataMean { get { return this.fDataMean; } }
        public double DataVariance { get { return this.fDataVariance; } }
        public double DataStdDev { get { return Math.Sqrt(this.fDataVariance); } }
        public double SeasonalMean { get { return this.fSeasonalMean; } }
        public double SeasonalVariance { get { return this.fSeasonalVariance; } }
        public double SeasonalStdDev { get { return Math.Sqrt(this.fSeasonalVariance); } }
        public double SeasonalRange { get { return this.fSeasonalRange; } }
        public double ResidualMean { get { return this.fResidualMean; } }
        public double ResidualVariance { get { return this.fResidualVariance; } }
        public double ResidualStdDev { get { return this.fResidualVariance; } }
        public double DeSeasonalMean { get { return this.fDeSeasonalMean; } }
        public double DeSeasonalVariance { get { return this.fDeSeasonalVariance; } }
        public double DeTrendMean { get { return this.fDeTrendMean; } }
        public double DeTrendVariance { get { return this.fDeTrendVariance; } }
        public double EstimatedVarianceOfResidualSampleVariance
        {
            get {
                double v = this.ResidualVariance;
                return 2 * v * v / (fSampleSize - 1);
            }
        }
        public double TrendinessZScore
        { get {
                double resVarVar = EstimatedVarianceOfResidualSampleVariance;
                return (this.fDeSeasonalVariance - this.fResidualVariance)
                            / Math.Sqrt(Math.Max(1E-12, resVarVar));
            }
        }

        public double SeasonalZScore
        {
            get
            {
                double resVarVar = EstimatedVarianceOfResidualSampleVariance;
                return (this.fDeTrendVariance - this.fResidualVariance)
                            / Math.Sqrt(Math.Max(1E-12, resVarVar));
            }
        }

        public double ResidualLogLikelihood { get { return this.fResidualLogLikelihood; } }
       

        public double getResidualLogLikelihood(double sigma)
        {
            double var = sigma * sigma;
            var retVal =  -0.5 * fSampleSize * (fResidualVarMLE / var + Math.Log(2 * Math.PI * var));
            return retVal;
        }

        
        public override string ToString()
        {
            return
                $"Data Mean           = {DataMean}\n" +
                "Data Variance        = {DataVariance}\n" +
                "Trend Mean           = {TrendMean}\n" +
                "Trend Range          = {TrendRange}\n" +
                "Seasonal Mean        = {SeasonalMean}\n" +
                "Seasonal Variance    = {SeasonalVariance}\n" +
                "Seasonal Range       = {SeasonalRange}\n" +
                "De-Seasonal Mean     = {DeSeasonalMean}\n" +
                "De-Seasonal Variance = {DeSeasonalVariance}\n" +
                "De-Trend Mean        = {DeTrendMean}\n" +
                "De-Trend Variance    = {DeTrendVariance}\n" +
                "Residual Mean        = {ResidualMean}\n" +
                "Residual Variance    = {ResidualVariance}\n" +
                "Var(ResSampleVar)    = {EstimatedVarianceOfResidualSampleVariance}\n" +
                "Trend Test ZScore    = {TrendinessZScore}\n" +
                "Seasonal Test ZScore = {SeasonalZScore}\n" +
                "SeasonalVar/ResidVar = {SeasonalVariance / ResidualVariance}";

        }
    }
}