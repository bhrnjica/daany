using System;
//The code is converted from the java version found at https://github.com/ServiceNow/stl-decomp-4j
namespace Daany.Stat.stl
{
    /// <summary>
    /// Class contains settings for one time series component
    /// </summary>
    public class LoessSettings
    {

        private int fWidth;
        private int fDegree;
        private int fJump;

        public int Width { get { return this.fWidth; } }
        public int Degree { get { return this.fDegree; } }
        public int Jump { get { return this.fJump; } }


        public LoessSettings(int width, int degree, int jump)
        {

            width = Math.Max(3, width);
            if (width % 2 == 0)
                width++;

            this.fWidth = width;
            this.fJump = Math.Max(1, jump);
            degree = Math.Max(0, Math.Min(2, degree));
            this.fDegree = degree;
        }

        public LoessSettings(int width, int degree)
        {
            //  NOTE: calling this(width, degree, Math.max(1, (int) (0.1 * width + 0.9))) is wrong here since width hasn't
            //  been adjusted yet. Simpler to just copy the code and test.
            width = Math.Max(3, width);
            if (width % 2 == 0)
                width++;

            this.fWidth = width;
            this.fJump = Math.Max(1, (int)(0.1 * width + 0.9));
            degree = Math.Max(0, Math.Min(2, degree));
            this.fDegree = degree;
        }

        public LoessSettings(int width)
        {
            width = Math.Max(3, width);
            if (width % 2 == 0)
                width++;
            
            this.fWidth = width;
            this.fJump = Math.Max(1, (int)(0.1 * width + 0.9));
            this.fDegree = 1;
        }
 
        public override String ToString()
        {
            return $"[width = {this.fWidth}, degree = {this.fDegree}, jump = {this.fJump}]";
        }
    }
}