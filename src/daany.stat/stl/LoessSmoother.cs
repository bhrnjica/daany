using System;
//The code is converted from the java version found at https://github.com/ServiceNow/stl-decomp-4j
namespace Daany.stl
{

    public class LoessBuilder
    {

        private int fWidth = 0;
        private int fDegree = 1;
        private int fJump = 1;
        private double[] fExternalWeights = null;
        private double[] fData = null;

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

        public double[] Data
        {
            get
            {
                return this.fData;
            }
            set
            {
                this.fData = value;
            }
        }

        public LoessSmoother build()
        {
            if ((this.fWidth == 0))
            {
                throw new Exception("LoessSmoother.Builder: Width must be set before calling build");
            }

            if ((this.fData == null))
            {
                throw new Exception("LoessSmoother.Builder: Data must be set before calling build");
            }

            return new LoessSmoother(this.fWidth, this.fJump, this.fDegree, this.fData, this.fExternalWeights);
        }
    }

    public class LoessSmoother
    {
        private LoessInterpolator fInterpolator;
        private double[] fData;
        private int fWidth;
        private int fJump;
        private double[] fSmoothed;


        public LoessSmoother(int width, int jump, int degree, double[] data, double[] externalWeights)
        {
            InterpolatorBuilder b = new InterpolatorBuilder();
            b.Width = width;
            b.Degree = degree;
            b.ExternalWeights = externalWeights;
            this.fInterpolator = b.interpolate(data);

            this.fData = data;
            this.fJump = Math.Min(jump, (data.Length - 1));
            this.fWidth = width;
            this.fSmoothed = new double[data.Length];
        }

        public LoessInterpolator Interpolator
        {
            get
            {
                return this.fInterpolator;
            }

        }

        //Perform smooting and return smooted data
        public double[] smooth()

        {
            if ((this.fData.Length == 1))
            {
                this.fSmoothed[0] = this.fData[0];
                return this.fSmoothed;
            }

            int right = -1;
            int left = -1;
            if ((this.fWidth >= this.fData.Length))
            {
                left = 0;
                right = (this.fData.Length - 1);
                for (int i = 0; (i < this.fData.Length); i = (i + this.fJump))
                {
                    Double y = this.fInterpolator.smoothOnePoint(i, left, right);
                    //this.fSmoothed[i] = (y == 0);
                    this.fSmoothed[i] = (y == 0) ? fData[i] : y;
                }

            }
            else if ((this.fJump == 1))
            {
                int halfWidth = (int)((this.fWidth + 1) / 2.0);
                left = 0;
                right = (this.fWidth - 1);
                for (int i = 0; (i < this.fData.Length); i++)
                {
                    if (((i >= halfWidth)
                                && (right
                                != (this.fData.Length - 1))))
                    {
                        left++;
                        right++;
                    }

                    double y = this.fInterpolator.smoothOnePoint(i, left, right);

                    this.fSmoothed[i] = (y == 0) ? fData[i] : y;
                }

            }
            else
            {
                //  For reference, the original RATFOR:
                //  else { # newnj greater than one, len less than n
                //  nsh = (len+1)/2
                //  do i = 1,n,newnj { # fitted value at i
                //  if(i<nsh) {              // i     = [1, 2, 3, 4, 5, 6, 7, 8, 9]; 9 points
                //  nleft = 1                // left  = [1, 1, 1, 1, 1, 1, 1, 1, 1];
                //  nright = len             // right = [19, 19, 19, 19, 19, 19, 19, 19, 19]; right - left = 18
                //  }
                //  else if(i>=n-nsh+1) {    // i     = [135, 136, 137, 138, 139, 140, 141, 142, 143, 144]; 10 points
                //  nleft = n-len+1          // left  = [126, 126, 126, 126, 126, 126, 126, 126, 126, 126];
                //  nright = n               // right = [144, 144, 144, 144, 144, 144, 144, 144, 144, 144]; right - left = 18
                //  }
                //  else {                   // i     = [10, 11, 12, ..., 132, 133, 134]; 125 points
                //  nleft = i-nsh+1          // left  = [1, 2, 3, ..., 123, 124, 125]
                //  nright = len+i-nsh       // right = [19, 20, 21, ..., 141, 142, 143]; right - left = 18
                //  }
                //  call est(y,n,len,ideg,float(i),ys(i),nleft,nright,res,userw,rw,ok)
                //  if(!ok) ys(i) = y(i)
                //  }
                //  }
                //  Note that RATFOR/Fortran are indexed from 1
                // 
                //  test: data.length == 144, fWidth = 19
                //    --> halfWidth = 10
                //  Ignoring jumps...
                //  First branch for  i = [0, 1, 2, 3, 4, 5, 6, 7, 8]; 9 points
                //                 left = [0, 0, 0, 0, 0, 0, 0, 0, 0]
                //                right = [18, 18, 18, 18, 18, 18, 18, 18, 18]; right - left = 18
                //  Second branch for i = [134, 135, 136, 137, 138, 139, 140, 141, 142, 143]; 10 points
                //                 left = [125, 125, 125, 125, 125, 125, 125, 125, 125, 125];
                //                right = [143, 143, 143, 143, 143, 143, 143, 143, 143, 143]; right - left = 18
                //  Third branch for  i = [ 9, 10, 11, ..., 131, 132, 133]; 125 points
                //                 left = [ 0,  1,  2, ..., 122, 123, 124]
                //                right = [18, 19, 20, ..., 140, 141, 142]; right - left = 18
                int halfWidth = (int)((this.fWidth + 1) / 2.0);
                for (int i = 0; (i < this.fData.Length); i = (i + this.fJump))
                {
                    if ((i < (halfWidth - 1)))
                    {
                        left = 0;
                    }
                    else if ((i >= (this.fData.Length - halfWidth)))
                    {
                        left = (this.fData.Length - this.fWidth);
                    }
                    else
                    {
                        left = ((i - halfWidth) + 1);
                    }

                    right = (left + (this.fWidth - 1));
                    double y = this.fInterpolator.smoothOnePoint(i, left, right);
                    //
                    this.fSmoothed[i] = (y == 0) ? fData[i] : y;
                    //  logSmoothedPoint(i, smooth[i]);
                }

            }

            if ((this.fJump != 1))
            {
                for (int i = 0; (i
                            < (this.fData.Length - this.fJump)); i = (i + this.fJump))
                {
                    double slope = ((this.fSmoothed[(i + this.fJump)] - this.fSmoothed[i])
                                / ((double)(this.fJump)));
                    for (int j = (i + 1); (j
                                < (i + this.fJump)); j++)
                    {
                        this.fSmoothed[j] = (this.fSmoothed[i]
                                    + (slope
                                    * (j - i)));
                        //  logInterpolatedPoint(j, smooth[j]);
                    }

                }

                int last = (this.fData.Length - 1);
                int lastSmoothedPos = ((last / this.fJump)
                            * this.fJump);
                if ((lastSmoothedPos != last))
                {
                    double y = this.fInterpolator.smoothOnePoint(last, left, right);
                    this.fSmoothed[last] = (y == 0) ? fData[last] : y;
                    //  logSmoothedPoint(last, smooth[last]);

                    if ((lastSmoothedPos != (last - 1)))
                    {
                        double slope = ((this.fSmoothed[last] - this.fSmoothed[lastSmoothedPos])
                                    / (last - lastSmoothedPos));
                        for (int j = (lastSmoothedPos + 1); (j < last); j++)
                        {
                            this.fSmoothed[j] = (this.fSmoothed[lastSmoothedPos]
                                        + (slope
                                        * (j - lastSmoothedPos)));
                            //  logInterpolatedPoint(j, smooth[j]);
                        }

                    }

                }

            }

            return this.fSmoothed;
        }
    }
}