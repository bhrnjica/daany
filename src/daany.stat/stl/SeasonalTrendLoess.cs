
using System;
using System.Linq;
using System.Collections.Generic;
using Daany.Stat;
using Daany.MathStuff;
//The code is converted from the java version found at https://github.com/ServiceNow/stl-decomp-4j

namespace Daany.stl
{

    public class SeasonalTrendLoess
    {
        private double[] fData;
        private Decomposition fDecomposition;
        private int fPeriodLength;
        private LoessSettings fSeasonalSettings;
        private LoessSettings fTrendSettings;
        private LoessSettings fLowpassSettings;
        private int fInnerIterations;
        private int fRobustIterations;
        private double[] fDetrend;
        private double[] fExtendedSeasonal;
        private double[] fDeSeasonalized;
        private CyclicSubSeriesSmoother fCyclicSubSeriesSmoother;
        private LoessBuilder fLoessSmootherFactory;
        private LoessBuilder fLowpassLoessFactory;

        public SeasonalTrendLoess(double[] data, int periodicity, int ni, int no, LoessSettings seasonalSettings,
            LoessSettings trendSettings, LoessSettings lowpassSettings)
        {
            this.fData = data;
            int size = data.Length;
            this.fPeriodLength = periodicity;
            this.fSeasonalSettings = seasonalSettings;
            this.fTrendSettings = trendSettings;
            this.fLowpassSettings = lowpassSettings;
            this.fInnerIterations = ni;
            this.fRobustIterations = no;

            this.fLoessSmootherFactory = new LoessBuilder();
            this.fLoessSmootherFactory.Width = this.fTrendSettings.Width;
            this.fLoessSmootherFactory.Degree = this.fTrendSettings.Degree;
            this.fLoessSmootherFactory.Jump=this.fTrendSettings.Jump;

            this.fLowpassLoessFactory = new LoessBuilder();
            this.fLowpassLoessFactory.Width=this.fLowpassSettings.Width;
            this.fLowpassLoessFactory.Degree=this.fLowpassSettings.Degree;
            this.fLowpassLoessFactory.Jump=this.fLowpassSettings.Jump;

            //
            var builder = new CyclicSubSeriesSmootherBuilder();
            builder.Width = seasonalSettings.Width;
            builder.Degree = seasonalSettings.Degree;
            builder.Jump = seasonalSettings.Jump;
            builder.DataLength = size;
            builder.extrapolateForwardAndBack(1);
            builder.Periodicity = periodicity;
            ///
            this.fCyclicSubSeriesSmoother  = builder.build();

            this.fDetrend = new double[size];
            this.fExtendedSeasonal = new double[(size + (2 * this.fPeriodLength))];
        }

        public static Decomposition performPeriodicDecomposition(double[] data, int periodicity)
        {
            //  The LOESS interpolator with degree 0 and a very long window (arbitrarily chosen to be 100 times the length of
            //  the array) will interpolate all points as the average value of the series. This particular setting is used
            //  for smoothing the seasonal sub-cycles, so the end result is that the seasonal component of the decomposition
            //  is exactly periodic.
            //  This fit is for diagnostic purposes, so we just do a single inner iteration.
            var stlBuilder = new SeasonalTrendLoessBuilder();
            stlBuilder.PeriodLength = periodicity;
            stlBuilder.SeasonalWidth = 100 * data.Length;
            stlBuilder.SeasonalDegree = 0;
            stlBuilder.InnerIterations = 1;
            stlBuilder.RobustIterations = 0;
            var stl = stlBuilder.buildSmoother(data);

            return stl.decompose();
        }

        public static Decomposition performRobustPeriodicDecomposition(double[] data, int periodicity)
        {
            //  The LOESS interpolator with degree 0 and a very long window (arbitrarily chosen to be 100 times the length of
            //  the array) will interpolate all points as the average value of the series. This particular setting is used
            //  for smoothing the seasonal sub-cycles, so the end result is that the seasonal component of the decomposition
            //  is exactly periodic.
            //  This fit is for diagnostic purposes, so we just do a single inner and outer iteration.
            var stlBuilder = new SeasonalTrendLoessBuilder();
            stlBuilder.PeriodLength = periodicity;
            stlBuilder.SeasonalWidth = 100 * data.Length;
            stlBuilder.SeasonalDegree = 0;
            stlBuilder.InnerIterations = 1;
            stlBuilder.RobustIterations = 1;
            var stl = stlBuilder.buildSmoother(data);

            return stl.decompose();
        }

     
        public Decomposition decompose()
        {
            this.fDecomposition = new Decomposition(this.fData);
            int outerIteration = 0;
            while (true)
            {
                bool useResidualWeights = outerIteration > 0;
                for (int iteration = 0; iteration < this.fInnerIterations; iteration++)
                {
                    this.smoothSeasonalSubCycles(useResidualWeights);
                    this.removeSeasonality();
                    this.updateSeasonalAndTrend(useResidualWeights);
                }

                if (++outerIteration > fRobustIterations)
                    break;

                this.fDecomposition.computeResidualWeights();
            }

            this.fDecomposition.updateResiduals();
            Decomposition result = this.fDecomposition;
            this.fDecomposition = null;
            return result;
        }

        private void smoothSeasonalSubCycles(bool useResidualWeights)
        {
            double[] data = this.fDecomposition.fData;
            double[] trend = this.fDecomposition.fTrend;
            double[] weights = this.fDecomposition.fWeights;

            for (int i = 0; i < data.Length; i++)
            {
                this.fDetrend[i] = data[i] - trend[i];
            }

            double[] residualWeights = useResidualWeights ? weights : null;
            //
            fCyclicSubSeriesSmoother.smoothSeasonal(this.fDetrend, this.fExtendedSeasonal,residualWeights);
        }

        private void removeSeasonality()
        {
            //
            double[] pass1 = this.fExtendedSeasonal.MA(this.fPeriodLength);
            //  data.length + periodLength + 1
            double[] pass2 = pass1.MA(this.fPeriodLength);
            //  data.length + 2
            double[] pass3 = pass2.MA(3);
            //
            this.fLowpassLoessFactory.Data = pass3;
            LoessSmoother lowPassLoess = this.fLowpassLoessFactory.build();
            this.fDeSeasonalized = lowPassLoess.smooth();
            //  dumpDebugData("lowpass", fDeSeasonalized);
        }

        private void updateSeasonalAndTrend(bool useResidualWeights)
        {
            double[] data = this.fDecomposition.fData;
            double[] trend = this.fDecomposition.fTrend;
            double[] weights = this.fDecomposition.fWeights;
            double[] seasonal = this.fDecomposition.fSeasonal;
            for (int i = 0; (i < data.Length); i++)
            {
                seasonal[i] = this.fExtendedSeasonal[this.fPeriodLength + i] - this.fDeSeasonalized[i];
                trend[i] = data[i] - seasonal[i];
            }

            //  dumpDebugData("seasonal", seasonal);
            //  dumpDebugData("trend0", trend);
            double[] residualWeights = useResidualWeights ? weights : null;

            this.fLoessSmootherFactory.Data = trend;
            this.fLoessSmootherFactory.ExternalWeights = residualWeights;
            LoessSmoother trendSmoother = this.fLoessSmootherFactory.build();

            //System.arraycopy(trendSmoother.smooth(), 0, trend, 0, trend.Length);
            //  dumpDebugData("trend", trend);

            var sss = trendSmoother.smooth();
            for (int i = 0; i < trend.Length; i++)
                trend[i] = sss[i];
        }


        public override String ToString()
        {
            return $"SeasonalTrendLoess: [\n" +
                         "inner iterations     = {this.fInnerIterations}\n" +
                         "outer iterations     = {this.fRobustIterations}\n" +
                         "periodicity          = {this.fPeriodLength}\n" +
                         "seasonality settings = {this.fSeasonalSettings}\n" +
                         "trend settings       = {this.fTrendSettings}\n" +
                         "lowpass settings     = {this.fLowpassSettings}\n]";
            //return base.ToString();
        }
    }

    public class SeasonalTrendLoessBuilder
    {

        private int fPeriodLength = 0;
        private int fSeasonalWidth = 0;
        private int fSeasonalJump = 0;
        private int fSeasonalDegree = 0;
        private int fTrendWidth = 0;
        private int fTrendJump = 0;
        private int fTrendDegree = 0;
        private int fLowpassWidth = 0;
        private int fLowpassJump = 0;
        private int fLowpassDegree = 1;
        //  Following the R interface, we default to "non-robust"
        private int fInnerIterations = 2;
        private int fRobustIterations = 0;
        //  Following the R interface, we implement a "periodic" flag that defaults to false.
        private bool fPeriodic = false;
        private bool fFlatTrend = false;
        private bool fLinearTrend = false;

        private LoessSettings buildSettings(int width, int degree, int jump)

        {
            if ((jump == 0))
            {
                return new LoessSettings(width, degree);
            }
            else
            {
                return new LoessSettings(width, degree, jump);
            }

        }

        public int PeriodLength
        {
            get
            {
                return this.fPeriodLength;
            }
            set
            {
                if (value < 2)
                {
                    throw new Exception("periodicity must be at least 2");
                }

                this.fPeriodLength = value;
            }
        }

        public int SeasonalWidth
        {
            get
            {
                return this.fSeasonalWidth;
            }
            set
            {
                this.fSeasonalWidth = value;
            }
        }

        public int SeasonalDegree
        {
            get
            {
                return this.fSeasonalDegree;
            }
            set
            {
                this.fSeasonalDegree = value;
            }
        }

        public int SeasonalJump
        {
            get
            {
                return this.fSeasonalJump;
            }
            set
            {
                this.fSeasonalJump = value;
            }
        }

        public int TrendWidth
        {
            get
            {
                return this.fTrendWidth;
            }
            set
            {
                this.fTrendWidth = value;
            }
        }

        public int TrendDegree
        {
            get
            {
                return this.fTrendDegree;
            }
            set
            {
                this.fTrendDegree = value;
            }
        }

        public int TrendJump
        {
            get
            {
                return this.fTrendJump;
            }
            set
            {
                this.fTrendJump = value;
            }
        }

        public int LowpassWidth
        {
            get
            {
                return this.fLowpassWidth;
            }
            set
            {
                this.fLowpassWidth = value;
            }
        }

        public int LowpassDegree
        {
            get
            {
                return this.fLowpassDegree;
            }
            set
            {
                this.fLowpassDegree = value;
            }
        }

        public int LowpassJump
        {
            get
            {
                return this.fLowpassJump;
            }
            set
            {
                this.fLowpassJump = value;
            }
        }

        public int InnerIterations
        {
            get
            {
                return this.fInnerIterations;
            }
            set
            {
                this.fInnerIterations = value;
            }
        }

        public int RobustIterations
        {
            get
            {
                return this.fRobustIterations;
            }
            set
            {
                this.fRobustIterations = value;
            }
        }


        public bool Robust
        {
            
            set
            {
                if(value)
                {
                    this.fInnerIterations = 1;
                    this.fRobustIterations = 15;
                }
                else//NonRobust
                {
                    this.fInnerIterations = 2;
                    this.fRobustIterations = 0;
                }

            }
        }

        public bool Periodic
        {
            get
            {
                return this.fPeriodic;
            }
            set
            {
                this.fPeriodic = value;
            }
        }

        public bool FlatTrend
        {
            get
            {
                return this.fFlatTrend;
            }
            set
            {
                this.fFlatTrend = value;
                if (value)
                {
                    this.fLinearTrend = false;
                }
                else
                {
                    this.fLinearTrend = true;
                }

            }
        }

        public SeasonalTrendLoess buildSmoother(double[] data)
        {
            this.sanityCheck(data);

            if (this.fPeriodic)
            {
                this.fSeasonalWidth = (100 * data.Length);
                this.fSeasonalDegree = 0;
            }
            else if (this.fSeasonalDegree == 0)
            {
                this.fSeasonalDegree = 1;
            }

            var seasonalSettings = buildSettings(this.fSeasonalWidth, this.fSeasonalDegree, this.fSeasonalJump);

            if (this.fFlatTrend)
            {
                this.fTrendWidth = 100 * this.fPeriodLength * data.Length;
                this.fTrendDegree = 0;
            }
            else if (this.fLinearTrend)
            {
                this.fTrendWidth = 100 * this.fPeriodLength * data.Length;
                this.fTrendDegree = 1;
            }
            else if (this.fTrendDegree == 0)
            {
                this.fTrendDegree = 1;
            }

            if ((this.fTrendWidth == 0))
            {
                this.fTrendWidth = SeasonalTrendLoessBuilder.calcDefaultTrendWidth(this.fPeriodLength, this.fSeasonalWidth);
            }

            LoessSettings trendSettings = this.buildSettings(this.fTrendWidth, this.fTrendDegree, this.fTrendJump);

            if ((this.fLowpassWidth == 0))
            {
                this.fLowpassWidth = this.fPeriodLength;
            }

            LoessSettings lowpassSettings = this.buildSettings(this.fLowpassWidth, this.fLowpassDegree, this.fLowpassJump);

            var stl = new SeasonalTrendLoess(data, this.fPeriodLength, this.fInnerIterations, this.fRobustIterations, 
                seasonalSettings, trendSettings, lowpassSettings);

            return stl;
        }

        private static int calcDefaultTrendWidth(int periodicity, int seasonalWidth)
        {
            //  This formula is based on a numerical stability analysis in the original paper.
            var retVal = (int)(1.5 * periodicity / (1 - 1.5 / (double)seasonalWidth) + 0.5);
            return retVal;
        }

        private void sanityCheck(double[] data)
        {
            if ((data == null))
            {
                throw new Exception("SeasonalTrendLoess.Builder: Data array must be non-null");
            }

            if ((this.fPeriodLength == 0))
            {
                throw new Exception("SeasonalTrendLoess.Builder: Period Length must be specified");
            }

            if ((data.Length < (2 * this.fPeriodLength)))
            {
                throw new Exception("SeasonalTrendLoess.Builder: Data series must be at least 2 * periodicity in length");
            }

            if (this.fPeriodic)
            {
                int massiveWidth = (100 * data.Length);
                bool periodicConsistent = ((this.fSeasonalDegree != 0)
                            && ((this.fSeasonalWidth != 0)
                            && ((this.fSeasonalWidth == massiveWidth)
                            && (this.fSeasonalDegree == 0))));
                if (((this.fSeasonalWidth != 0)
                            && !periodicConsistent))
                {
                    throw new Exception("SeasonalTrendLoess.Builder: setSeasonalWidth and setPeriodic cannot both be called.");
                }

                if (((this.fSeasonalDegree != 0)
                            && !periodicConsistent))
                {
                    throw new Exception("SeasonalTrendLoess.Builder: setSeasonalDegree and setPeriodic cannot both be called.");
                }

                if ((this.fSeasonalJump != 0))
                {
                    throw new Exception("SeasonalTrendLoess.Builder: setSeasonalJump and setPeriodic cannot both be called.");
                }

            }
            else if ((this.fSeasonalWidth == 0))
            {
                throw new Exception("SeasonalTrendLoess.Builder: setSeasonalWidth or setPeriodic must be called.");
            }

            if (this.fFlatTrend)
            {
                int massiveWidth = (100
                            * (this.fPeriodLength * data.Length));
                bool flatTrendConsistent = ((this.fTrendWidth != 0)
                            && ((this.fTrendDegree != 0)
                            && ((this.fTrendWidth == massiveWidth)
                            && (this.fTrendDegree == 0))));
                if (((this.fTrendWidth != 0)
                            && !flatTrendConsistent))
                {
                    throw new Exception("SeasonalTrendLoess.Builder: setTrendWidth incompatible with flat trend.");
                }

                if (((this.fTrendDegree != 0)
                            && !flatTrendConsistent))
                {
                    throw new Exception("SeasonalTrendLoess.Builder: setTrendDegree incompatible with flat trend.");
                }

                if ((this.fTrendJump != 0))
                {
                    throw new Exception("SeasonalTrendLoess.Builder: setTrendJump incompatible with flat trend.");
                }

            }

            if (this.fLinearTrend)
            {
                int massiveWidth = (100
                            * (this.fPeriodLength * data.Length));
                bool linearTrendConsistent = ((this.fTrendWidth != 0)
                            && ((this.fTrendDegree != 0)
                            && ((this.fTrendWidth == massiveWidth)
                            && (this.fTrendDegree == 1))));
                if (((this.fTrendWidth != 0)
                            && !linearTrendConsistent))
                {
                    throw new Exception("SeasonalTrendLoess.Builder: setTrendWidth incompatible with linear trend.");
                }

                if (((this.fTrendDegree != 0)
                            && !linearTrendConsistent))
                {
                    throw new Exception("SeasonalTrendLoess.Builder: setTrendDegree incompatible with linear trend.");
                }

                if ((this.fTrendJump != 0))
                {
                    throw new Exception("SeasonalTrendLoess.Builder: setTrendJump incompatible with linear trend.");
                }

            }

        }
    }

    public class Decomposition
    {

        public double[] fData;
        public double[] fTrend;
        public double[] fSeasonal;
        public double[] fResiduals;
        public double[] fWeights;

        public double[] Data { get { return this.fData; } }
        public double[] Trend { get { return this.fTrend; } }
        public double[] Seasonal { get { return this.fSeasonal; } }
        public double[] Residual { get { return this.fResiduals; } }
        public double[] Weights { get { return this.fWeights; } }

        public Decomposition(double[] data)
        {

            this.fData = data;
            int size = this.fData.Length;
            this.fTrend = new double[size];
            this.fSeasonal = new double[size];
            this.fResiduals = new double[size];
            this.fWeights = new double[size];

            for (int i = 0; i < size; i++)
                fWeights[i] = 1;
        }

        public void updateResiduals()
        {
            for (int i = 0; (i < this.fData.Length); i++)
            {
                this.fResiduals[i] = this.fData[i] - this.fSeasonal[i] - this.fTrend[i];
            }

        }

        public void computeResidualWeights()
        {
            //  TODO: There can be problems if "robust" iterations are done but MAD ~= 0. May want to put a floor on c001.
            //  The residual-based weights are a "bisquare" weight based on the residual deviation compared to 6 times the
            //  median absolute deviation (MAD). First compute 6 * MAD. (The sort could be a selection but this is
            //  not critical as the rest of the algorithm is higher complexity.)
            for (int i = 0; (i < this.fData.Length); i++)
            {
                this.fWeights[i] = Math.Abs(fData[i] - fSeasonal[i] - fTrend[i]);
            }

            //Arrays.sort(this.fWeights);
            this.fWeights = this.fWeights.OrderBy(x => x).ToArray();

            //  For an even number of elements, the median is the average of the middle two.
            //  With proper indexing this formula works either way at the cost of some
            //  superfluous work when the number is odd.
            int mi0 = (fData.Length + 1) / 2 - 1; //  n = 5, mi0 = 2; n = 4, mi0 = 1
            int mi1 = fData.Length - mi0 - 1;     //  n = 5, mi1 = 2; n = 4, mi1 = 2

            double sixMad = 3 * (this.fWeights[mi0] + this.fWeights[mi1]);
            double c999 = 0.999 * sixMad;
            double c001 = 0.001 * sixMad;

            //
            for (int i = 0; i < this.fData.Length; i++)
            {
                double r = Math.Abs(fData[i] - fSeasonal[i] - fTrend[i]);

                if (r <= c001)
                {
                    this.fWeights[i] = 1;
                }
                else if (r <= c999)
                {
                    double h = r / sixMad;
                    double w = 1.0 - h * h;
                    this.fWeights[i] = w * w;
                }
                else
                {
                    this.fWeights[i] = 0;
                }

            }

        }

        public void smoothSeasonal(int width)
        {
            this.smoothSeasonal(width, true);
        }

        public void smoothSeasonal(int width, bool restoreEndPoints)
        {
            //  Ensure that LOESS smoother width is odd and >= 3.
            width = Math.Max(3, width);
            if (width % 2 == 0)
            {
                width++;
            }

            //  Quadratic smoothing of the seasonal component.
            //  Do NOT perform linear interpolation between smoothed points - the quadratic spline can accommodate
            //  sharp changes and linear interpolation would cut off peaks/valleys.
            LoessBuilder builder = new LoessBuilder();
            builder.Width = width;
            builder.Degree = 2;
            builder.Jump=1;
            builder.Data = this.fSeasonal;

            LoessSmoother seasonalSmoother = builder.build();
            double[] smoothedSeasonal = seasonalSmoother.smooth();

            //  TODO: Calculate the variance reduction in smoothing the seasonal.
            //  Update the seasonal with the smoothed values.
            //  TODO: This is not very good - it causes discontinuities a the endpoints.
            //        Better to transition to linear in the last half-smoother width.
            //  Restore the end-point values as the smoother will tend to over-modify these.
            double s0 = this.fSeasonal[0];
            double sN = this.fSeasonal[(this.fSeasonal.Length - 1)];

            //System.arraycopy(smoothedSeasonal, 0, this.fSeasonal, 0, smoothedSeasonal.Length)
            this.fSeasonal = new double[smoothedSeasonal.Length];
            for (int i = 0; i < smoothedSeasonal.Length; i++)
                this.fSeasonal[i] = smoothedSeasonal[i];


            if (restoreEndPoints)
            {
                this.fSeasonal[0] = s0;
                this.fSeasonal[this.fSeasonal.Length - 1] = sN;
            }

            for (int i = 0; (i < smoothedSeasonal.Length); i++)
            {
                fResiduals[i] = fData[i] - fTrend[i] - fSeasonal[i];
            }

        }
    }
}