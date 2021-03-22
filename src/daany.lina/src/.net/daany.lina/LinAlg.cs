using System;
using System.Runtime.InteropServices;
namespace Daany.LinA
{
    unsafe public class LinA
    {

        static LinA()
        {
           
        }

        /// <summary>
        /// Dummy method to be called first 
        /// </summary>
        public static void init(){;}

        #region Solver- solver of system of linear equations
        // 
        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int lbsgesv_cpu(bool rowmajor, int n, int nrhs, float* A, int lda, float* B, int lbd);

        //double
        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int lbdgesv_cpu(bool rowmajor, int n, int nrhs, double* A, int ldda, double* B, int lddb);
       
        /// <summary>
        /// Sole system of Linear equation A X=B
        /// </summary>
        /// <param name="A">Matrix of the system</param>
        /// <param name="B">Right matrix</param>
        /// <returns></returns>
        public static float[,] Solve(float[,] A, float[,] B)
        {
            //define parameters
            int info = -1;
            int n = A.GetLength(0);
            int nrhs = B.GetLength(1);
            var Ac = A.Clone() as float[,];
            var Bc = B.Clone() as float[,];

            //define arrays
            fixed(float *pA = Ac, pB = Bc)
            {
                //pInvoke call
                info = lbsgesv_cpu(true, n, nrhs, pA, n, pB, nrhs);
            }
            //
            if (info != 0)
                throw new Exception($"lapack_sgesv failed due to invalid parameter {-info}.");

            //
            return Bc;
        }

        /// <summary>
        /// Solve linear system of equations A X = B.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns>X - solution matrix</returns>
        public static float[] Solve(float[,] A, float[] B)
        {
            //define parameters
            int info = -1;
            int n = A.GetLength(0);
            int nrhs = 1;
            var Ac = A.Clone() as float[,];
            var Bc = B.Clone() as float[];
            //define arrays
            int[] ipiv = new int[n];//permutation indices
            fixed (float* pA = Ac, pB = Bc)
            {
                //pInvoke call
                info = lbsgesv_cpu(true, n, nrhs, pA, n, pB, nrhs);
            }
            //
            if (info != 0)
                throw new Exception($"magma_sgesv failed due to invalid parameter {-info}.");

            //
            return Bc;
        }

        /// <summary>
        /// Solve linear system of equations A X = B.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns>X - solution matrix</returns>
        public static double[,] Solve(double[,] A, double[,] B)
        {
            //define parameters
            int info = -1;
            int n = A.GetLength(0);
            int nrhs = B.GetLength(1);
            var Ac = A.Clone() as double[,];
            var Bc = B.Clone() as double[,];
            //define arrays
            fixed (double* pA = Ac, pB = Bc)
            {
                //pInvoke call
                info = lbdgesv_cpu(true, n, nrhs, pA, n, pB, nrhs);
            }
            //
            if (info != 0)
                throw new Exception($"magma_sgesv failed due to invalid parameter {-info}.");

            //
            return Bc;
        }

        /// <summary>
        /// Solve linear system of equations A X = B.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns>X - solution matrix</returns>
        public static double[] Solve(double[,] A, double[] B)
        {
            //define parameters
            int info = -1;
            int n = A.GetLength(0);
            int nrhs = 1;
            var Ac = A.Clone() as double[,];
            var Bc = B.Clone() as double[];
            //define arrays
            fixed (double* pA = Ac, pB = Bc)
            {
                //pInvoke call
                info = lbdgesv_cpu(true, n, nrhs, pA, n, pB, nrhs);
            }
            //
            if (info != 0)
                throw new Exception($"magma_sgesv failed due to invalid parameter {-info}.");

            //
            return Bc;
        }
        #endregion

        #region LSS - least square solver
        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int lbsgels_cpu(bool rowmajor, int m, int n, int nrhs, float* A, int lda, float* B, int lbd);


        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int lbdgels_cpu(bool rowmajor, int m, int n, int nrhs, double* A, int lda, double* B, int lbd);
      
        public static float[,] Lss(float[,] A, float[,] B)
        {
            //define parameters
            int info = -1;
            int m = A.GetLength(0);
            int n = A.GetLength(1);
            int nrhs = B.GetLength(1);

            //define arrays
            var Ac = A.Clone() as float[,];
            var Bc = B.Clone() as float[,];

            fixed (float* pA = Ac, pB = Bc)
            {
                //pInvoke call
                info = lbsgels_cpu(true, m, n, nrhs, pA, n, pB, nrhs);

                //
                if (info != 0)
                    throw new Exception($"lapack_sgesv failed due to invalid parameter {-info}.");

                //X(n, nrhs) matrix is a submatrix of B(m, nrhs).
                var X = new float[n,nrhs];
                Array.Copy(Bc,X,n*nrhs);
                return X;
            }            
        }

        public static double[,] Lss(double[,] A, double[,] B)
        {
            //define parameters
            int info = -1;
            int m = A.GetLength(0);
            int n = A.GetLength(1);
            int nrhs = B.GetLength(1);

            //define arrays
            var Ac = A.Clone() as double[,];
            var Bc = B.Clone() as double[,];

            fixed (double* pA = Ac, pB = Bc)
            {
                //pInvoke call
                info = lbdgels_cpu(true, m, n, nrhs, pA, n, pB, nrhs);

                //
                if (info != 0)
                    throw new Exception($"magma_dgesv failed due to invalid parameter {-info}.");

                //X(n, nrhs) matrix is a submatrix of B(m, nrhs).
                var X = new double[n, nrhs];
                Array.Copy(Bc, X, n * nrhs);
                return X;
            }

        }
        #endregion

        #region Eigenvalues 
        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int lbsgeevs_cpu(bool rowmajor, int n, float* A, int lda, float* wr, float* wi, float* VL, bool computeLeft, float* VR, bool computerRight);
       
        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int lbdgeevs_cpu(bool rowmajor, int n, double* A, int lda, double* wr, double* wi, double* VL, bool computeLeft, double* VR, bool computerRight);
              
        public static (float[] wr, float[] wi, float[,] VL, float[,] VR ) Eigen(float[,] A, bool computeLeft= false, bool computeRight= false)
        {
            //define parameters
            int info = -1;
            int m = A.GetLength(0);
            if (m != A.GetLength(1))
                throw new Exception("Matrix A must be squared!");

            //define arrays
            var Ac = A.Clone() as float[,];
            var wr = new float[m];
            var wi = new float[m];
            var VL = new float[m, m];
            var VR = new float[m, m];

            fixed (float* pA = Ac, pwr = wr, pwi = wi, pVL = VL, pVR = VR)
            {
                info = lbsgeevs_cpu(true, m, pA, m, pwr, pwi, pVL, computeLeft, pVR, computeRight);
            }
            //
            return (wr, wi, VL, VR);
        }

        public static (double[] wr, double[] wi, double[,] VL, double[,] VR) Eigen(double[,] A, bool computeLeft, bool computeRight)
        {
            //define parameters
            int info = -1;
            int m = A.GetLength(0);
            if (m != A.GetLength(1))
                throw new Exception("Matrix A must be squared!");

            //define arrays
            var Ac = A.Clone() as double[,];
            var wr = new double[m];
            var wi = new double[m];
            var VL = new double[m, m];
            var VR = new double[m, m];

            fixed (double* pA = Ac, pwr = wr, pwi = wi, pVL = VL, pVR = VR)
            {
                info = lbdgeevs_cpu(true, m, pA, m, pwr, pwi, pVL, computeLeft, pVR, computeRight);
            }
            //
            return (wr, wi, VL, VR);
        }
        #endregion

        #region SVD singular value decomposition
        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int lbsgesvds_cpu(bool rowmajor, int m, int n, float* A, float* s, float* U,bool calcU, float* VT, bool calcV);

        
        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int lbdgesvds_cpu(bool rowmajor, int m, int n, double* A, double* s, double* U, bool calcU, double* VT, bool calcV);
      

        /// <summary>
        ///  Decompose rectangular matrix A on A = U * s * Vt
        /// </summary>
        /// <param name="A"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static (float[] s, float[,] U, float[,] vt) Svd(float[,] A, bool calcU, bool calcVt)
        {
            //define parameters
            int m = A.GetLength(0);//the number of rows
            int n = A.GetLength(1);//the number of columns
            var Ac = A.Clone() as float[,];
            //
            float[] s = new float[n];
            float[,] U = new float[m, m];
            float[,] VT = new float[n, n];
            
            //Initialize unmanaged memory to hold the array.
            fixed (float* pA = Ac, ps=s, pU=U, pVT=VT)
            {
                
                //pInvoke call
                int info = -1;
                info = lbsgesvds_cpu(true, m, n, pA, ps, pU, calcU, pVT, calcVt);

                //
                if (info != 0)
                    throw new Exception($"lapack_svd failed due to invalid parameter {-info}.");

                return (s, U, VT);
            }
        }

        /// <summary>
        /// Decompose rectangular matrix A on A = U * s * Vt
        /// </summary>
        /// <param name="A"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static (double[] s, double[,] U, double[,] vt) Svd(double[,] A, bool calcU, bool calcVt)
        {
            //define parameters
            int m = A.GetLength(0);//the number of rows
            int n = A.GetLength(1);//the number of columns
            var Ac = A.Clone() as double[,];
            double[]   s = new double[n];
            double[,]  U = new double[m, m];
            double[,] VT = new double[n, n];

            //Initialize unmanaged memory to hold the array.
            fixed (double* pA = Ac, ps = s, pU = U, pVT = VT)
            {

                //pInvoke call
                int info = -1;
                info = lbdgesvds_cpu(true, m, n, pA, ps, pU, calcU, pVT, calcVt);

                if (info != 0)
                    throw new Exception($"lapack_svd failed due to invalid parameter {-info}.");

                return (s, U, VT);
            }
        }
        #endregion

        #region Matrix operations
        // 
        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void  lbsgemm_cpu(bool rowmajor, int m, int n, int k, float alpha, float* A, int lda, float* B, int ldb, float beta, float* C, int ldc);
        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void lbdgemm_cpu(bool rowmajor, int m, int n, int k, double alpha, double* A, int lda, double* B, int ldb, double beta, double* C, int ldc);

        /// <summary>
        /// Multiplies A * B and multiplies the resulting matrix by alpha. 
        /// It then multiplies matrix C by beta.
        /// It stores the sum of these two products in matrix C.
        /// Thus, it calculates either
        ///
        /// C←αAB + βC
        ///
        /// or
        ///
        /// C←αBA + βC
        /// 
        /// with optional use of transposed forms of A, B, or both.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="alpha"></param>
        /// <param name="betta"></param>
        /// <returns></returns>
        public static float[,] MMult(float[,] A, float[,] B,float[,] C=null, float alpha=1, float betta= 1)
        {
            //define parameters
            int m = A.GetLength(0);
            int k = A.GetLength(1);
            int n = B.GetLength(1);
            if (C != null && (C.GetLength(0) != m || C.GetLength(1) != n))
                throw new Exception($"C matrix has wrong format. The format should be ({m},{n})");

            //const int lda = k, ldb = n, ldc = n;

            var Ac = A.Clone() as float[,];
            var Bc = B.Clone() as float[,];
            var Cc = C == null ? new float[m, n] : C.Clone() as float[,];
            //define arrays
            fixed (float* pA = Ac, pB = Bc, pC=Cc)
            {
                //pInvoke call
                lbsgemm_cpu(true, m, n, k, alpha, pA, k, pB, n, betta, pC, n);
            }
            //
            return Cc;
        }

        public static float[] MMult(float[,] A, float[] B, float[] C = null, float alpha = 1, float betta = 1)
        {
            //define parameters
            int m = A.GetLength(0);
            int k = A.GetLength(1);
            int n = 1;
            if (C != null && C.Length != m)
                throw new Exception($"C matrix has wrong format. The format should be ({m})");
            //const int lda = k, ldb = n, ldc = n;

            var Ac = A.Clone() as float[,];
            var Bc = B.Clone() as float[];
            var Cc = C == null ? new float[m] : C.Clone() as float[];

            //define arrays
            fixed (float* pA = Ac, pB = Bc, pC = Cc)
            {
                //pInvoke call
                lbsgemm_cpu(true, m, n, k, alpha, pA, k, pB, n, betta, pC, n);
            }
            //
            return Cc;
        }

        /// <summary>
        /// Multiplies A * B and multiplies the resulting matrix by alpha. 
        /// It then multiplies matrix C by beta.
        /// It stores the sum of these two products in matrix C.
        /// Thus, it calculates either
        ///
        /// C←αAB + βC
        ///
        /// or
        ///
        /// C←αBA + βC
        /// 
        /// with optional use of transposed forms of A, B, or both.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="alpha"></param>
        /// <param name="betta"></param>
        /// <returns></returns>
        public static double[,] MMult(double[,] A, double[,] B, double[,] C=null, double alpha = 1, double betta = 1)
        {
            //define parameters
            int m = A.GetLength(0);
            int k = A.GetLength(1);
            int n = B.GetLength(1);
            if (C != null && (C.GetLength(0) != m || C.GetLength(1) != n))
                throw new Exception($"C matrix has wrong format. The format should be ({m},{n})");
            //const int lda = k, ldb = n, ldc = n;

            var Ac = A.Clone() as double[,];
            var Bc = B.Clone() as double[,];
            var Cc = C==null? new double[m,n]: C.Clone() as double[,];
            
            //define arrays
            fixed (double* pA = Ac, pB = Bc, pC = Cc)
            {
                //pInvoke call
                lbdgemm_cpu(true, m, n, k, alpha, pA, k, pB, n, betta, pC, n);
            }
            //
            return Cc;
        }

        public static double[] MMult(double[,] A, double[] B, double[] C = null, double alpha = 1, double betta = 1)
        {
            //define parameters
            int m = A.GetLength(0);
            int k = A.GetLength(1);
            int n = 1;
            if (C != null && C.Length != m)
                throw new Exception($"C matrix has wrong format. The format should be ({m})");
            //const int lda = k, ldb = n, ldc = n;

            var Ac = A.Clone() as double[,];
            var Bc = B.Clone() as double[];
            var Cc = C == null ? new double[m] : C.Clone() as double[];

            //define arrays
            fixed (double* pA = Ac, pB = Bc, pC = Cc)
            {
                //pInvoke call
                lbdgemm_cpu(true, m, n, k, alpha, pA, k, pB, n, betta, pC, n);
            }
            //
            return Cc;
        }

        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int lbdinverse_cpu(bool rowmajor, int n, double* A, int lda);

        [DllImport("LapackBinding.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int lbsinverse_cpu(bool rowmajor, int n, float* A, int lda);

        public static float[,] MInverse(float[,] A)
        {
            //define parameters
            int info = -1;
            int n = A.GetLength(0);
            var Ac = A.Clone() as float[,];

            //define arrays
 
            fixed (float* pA = Ac)
            {
                //pInvoke call
                info = lbsinverse_cpu(true, n, pA, n);
            }
            //
            if (info != 0)
                throw new Exception($"lapack_sgesv failed due to invalid parameter {-info}.");

            //
            return Ac;
        }

        public static double[,] MInverse(double[,] A)
        {
            //define parameters
            int info = -1;
            int n = A.GetLength(0);
            var Ac = A.Clone() as double[,];

            //define arrays
            fixed (double* pA = Ac)
            {
                //pInvoke call
                info = lbdinverse_cpu(true, n, pA, n);

            }
            //
            if (info != 0)
                throw new Exception($"lapack_sgesv failed due to invalid parameter {-info}.");

            //
            return Ac;
        }

        #endregion
    }
}
