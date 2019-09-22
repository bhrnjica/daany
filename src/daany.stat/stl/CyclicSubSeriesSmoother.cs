using System;
//The code is converted from the java version found at https://github.com/ServiceNow/stl-decomp-4j
namespace Daany.stl
{
    public class CyclicSubSeriesSmootherBuilder
    {

        private int fWidth = 0;
        private int fDataLength = 0;
        private int fPeriodicity = 0;
        private int fNumPeriodsBackward = 0;
        private int fNumPeriodsForward = 0;
        private int fDegree = 1;
        private int fJump = 1;

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

        public int Jump
        {
            get
            {
                return this.fJump;
            }
            set
            {
                this.fJump = value;
            }
        }

        public int DataLength
        {
            get
            {
                return this.fDataLength;
            }
            set
            {
                this.fDataLength = value;
            }
        }

        public int Periodicity
        {
            get
            {
                return this.fPeriodicity;
            }
            set
            {
                this.fPeriodicity = value;
            }
        }


        public void extrapolateForwardOnly(int periods)
        {
            this.fNumPeriodsForward = periods;
            this.fNumPeriodsBackward = 0;
            return;
        }

        public void extrapolateForwardAndBack(int periods)
        {
            this.fNumPeriodsForward = periods;
            this.fNumPeriodsBackward = periods;
            return;
        }

        public int NumPeriodsForward
        {
            get
            {
                return this.fNumPeriodsForward;
            }
            set
            {
                this.fNumPeriodsForward = value;
            }
        }

        public int NumPeriodsBackward
        {
            get
            {
                return this.fNumPeriodsBackward;
            }
            set
            {
                this.fNumPeriodsBackward = value;
            }
        }


        public CyclicSubSeriesSmoother build()
        {
            this.checkSanity();
            return new CyclicSubSeriesSmoother(this.fWidth, this.fDegree,
                this.fJump, this.fDataLength, this.fPeriodicity,
                this.fNumPeriodsBackward, this.fNumPeriodsForward);
        }

        private void checkSanity()
        {
            if ((this.fWidth == 0))
            {
                throw new Exception("CyclicSubSeriesSmoother.Builder: setWidth must be called before building the smoother.");
            }

            if ((this.fPeriodicity == 0))
            {
                throw new Exception("CyclicSubSeriesSmoother.Builder: setPeriodicity must be called before building the smoother.");
            }

            if ((this.fDataLength == 0))
            {
                throw new Exception("CyclicSubSeriesSmoother.Builder: setDataLength must be called before building the smoother.");
            }

            if (((this.fNumPeriodsBackward == 0)
                        || (this.fNumPeriodsForward == 0)))
            {
                throw new Exception("CyclicSubSeriesSmoother.Builder: Extrapolation settings must be provided.");
            }

        }
    }

    public class CyclicSubSeriesSmoother
    {

        private double[][] fRawCyclicSubSeries;
        private double[][] fSmoothedCyclicSubSeries;
        private double[][] fSubSeriesWeights;
        private int fPeriodLength;
        private int fNumPeriods;
        private int fRemainder;
        private int fNumPeriodsToExtrapolateBackward;
        private int fNumPeriodsToExtrapolateForward;
        private int fWidth;
        private LoessBuilder fLoessSmootherFactory;

        public CyclicSubSeriesSmoother(int width, int degree, int jump, 
            int dataLength, int periodicity,
            int numPeriodsToExtrapolateBackward, int numPeriodsToExtrapolateForward)
        {
            this.fWidth = width;
            this.fLoessSmootherFactory = new LoessBuilder();
            this.fLoessSmootherFactory.Width = width;
            this.fLoessSmootherFactory.Jump = jump;
            this.fLoessSmootherFactory.Degree=degree;

            this.fPeriodLength = periodicity;
            this.fNumPeriods = (dataLength / periodicity);
            this.fRemainder = (dataLength % periodicity);

            this.fNumPeriodsToExtrapolateBackward = numPeriodsToExtrapolateBackward;
            this.fNumPeriodsToExtrapolateForward = numPeriodsToExtrapolateForward;


            this.fRawCyclicSubSeries = new double[periodicity][];
            this.fSmoothedCyclicSubSeries = new double[periodicity][];
            this.fSubSeriesWeights = new double[periodicity][];

            //  Bookkeeping: Write the data length as
            // 
            //  n = m * periodicity + r
            // 
            //  where r < periodicity. The first r sub-series will have length m + 1 and the remaining will have length m.
            //  Another way to look at this is that the cycle length is
            // 
            //  cycleLength = (n - p - 1) / periodicity + 1
            // 
            //  where p is the index of the cycle that we're currently in.
            for (int period = 0; (period < periodicity); period++)
            {
                int seriesLength = (period < fRemainder) ? (fNumPeriods + 1) : fNumPeriods;
                
                this.fRawCyclicSubSeries[period] = new double[seriesLength];
                this.fSmoothedCyclicSubSeries[period] = 
                    new double[(this.fNumPeriodsToExtrapolateBackward
                            + (seriesLength + this.fNumPeriodsToExtrapolateForward))];
                this.fSubSeriesWeights[period] = new double[seriesLength];
            }

        }

        public void smoothSeasonal(double[] rawData, double[] smoothedData, double[] weights)
        {
            this.extractRawSubSeriesAndWeights(rawData, weights);
            this.computeSmoothedSubSeries(weights != null);
            this.reconstructExtendedDataFromSubSeries(smoothedData);
            //  SeasonalTrendLoess.dumpDebugData("extended seasonal", smoothedData);
        }

        private void computeSmoothedSubSeries(bool useResidualWeights)
        {
            for (int period = 0; (period < this.fPeriodLength); period++)
            {
                double[] weights = useResidualWeights ? fSubSeriesWeights[period] : null;
                double[] rawData = fRawCyclicSubSeries[period];
                double[] smoothedData = fSmoothedCyclicSubSeries[period];

                smoothOneSubSeries(weights, rawData, smoothedData);

                // dumpCyclicSubseriesDebugData(period, rawData.length, smoothedData, rawData);
            }

        }

        private void extractRawSubSeriesAndWeights(double[] data, double[] weights)
        {
            for (int period = 0; (period < this.fPeriodLength); period++)
            {
                int cycleLength = (period < fRemainder) ? (fNumPeriods + 1) : fNumPeriods; 
                
                for (int i = 0; (i < cycleLength); i++)
                {
                    fRawCyclicSubSeries[period][i] = data[i * fPeriodLength + period];
                    if (weights != null)
                    {
                        fSubSeriesWeights[period][i] = weights[i * fPeriodLength + period];
                    }

                }

            }

        }

        private void reconstructExtendedDataFromSubSeries(double[] data)
        {
            //  Copy this smoothed cyclic sub-series to the extendedSeasonal work array.
            for (int period = 0; (period < this.fPeriodLength); period++)
            {
                int cycleLength = (period < fRemainder) ? (fNumPeriods + 1) : fNumPeriods;
                for (int i = 0; i < 
                    fNumPeriodsToExtrapolateBackward + cycleLength + 
                    fNumPeriodsToExtrapolateForward; ++i)
                {
                    data[i * fPeriodLength + period] = fSmoothedCyclicSubSeries[period][i];
                }
            }

        }

        private void smoothOneSubSeries(double[] weights, double[] rawData, double[] smoothedData)
        {
            int cycleLength = rawData.Length;

            //  Smooth the cyclic sub-series with LOESS and then extrapolate one place beyond each end.
            this.fLoessSmootherFactory.Data = rawData;
            this.fLoessSmootherFactory.ExternalWeights = weights;
            LoessSmoother smoother = this.fLoessSmootherFactory.build();

            //Copy, shifting by 1 to leave room for the extrapolated point at the beginning.
            // System.arraycopy(smoother.smooth(), 0, smoothedData, this.fNumPeriodsToExtrapolateBackward, cycleLength);

            var ss = smoother.smooth();
            for(int i=0; i< cycleLength; i++)
            {
                smoothedData[this.fNumPeriodsToExtrapolateBackward+i] =  ss[i];
            }


            LoessInterpolator interpolator = smoother.Interpolator;
            
            //  Extrapolate from the leftmost "width" points to the "-1" position
            int left = 0;
            int right = (left + (this.fWidth - 1));
            right = Math.Min(right, (cycleLength - 1));

            int leftValue = this.fNumPeriodsToExtrapolateBackward;

            for (int i = 1; (i <= this.fNumPeriodsToExtrapolateBackward); i++)
            {
                double ys = interpolator.smoothOnePoint((i * -1), left, right);
                //
                smoothedData[(leftValue - i)] = ys == 0 ? smoothedData[leftValue] : ys;
            }

            //  Extrapolate from the rightmost "width" points to the "length" position (one past the array end).
            right = (cycleLength - 1);
            left = ((right - this.fWidth)+ 1);
            left = Math.Max(0, left);

            int rightValue = (this.fNumPeriodsToExtrapolateBackward + right);

            for (int i = 1; (i <= this.fNumPeriodsToExtrapolateForward); i++)
            {
                Double ys = interpolator.smoothOnePoint((right + i), left, right);
                //smoothedData[(rightValue + i)] = (ys == null);
                smoothedData[rightValue + i] = ys == 0 ? smoothedData[rightValue] : ys;
            }

        }
    }
}