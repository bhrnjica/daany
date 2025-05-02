using System;
using System.Runtime.InteropServices;
namespace Daany.LinA
{
    enum CBLAS_LAYOUT { CblasRowMajor = 101, CblasColMajor = 102 };
    enum CBLAS_TRANSPOSE { CblasNoTrans = 111, CblasTrans = 112, CblasConjTrans = 113 };
    unsafe public class LinA
    {

		const string dllName = "mkl_rt";

		#region Solver- solver of system of linear equations
		// 
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_sgesv(int matrix_layout, int n, int nrhs, float* a, int lda, int* ipiv, float* b, int ldb);

        //double
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_dgesv(int matrix_layout, int n, int nrhs, double* a, int lda, int* ipiv, double* b, int ldb);
       
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
            int[] ipiv = new int[n]; 

            //define arrays
            fixed (float *pA = Ac, pB = Bc)
            {
                fixed(int* pipiv = ipiv)
                {
                    //pInvoke call
                    //#define LAPACK_ROW_MAJOR               101
                    //#define LAPACK_COL_MAJOR               102
                    info = LAPACKE_sgesv(101, n, nrhs, pA, n, pipiv, pB, nrhs);
                }
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
            int[] ipiv = new int[n];
            //define arrays
            fixed (float* pA = Ac, pB = Bc)
            {
                fixed (int* pipiv = ipiv)
                {
                    info = LAPACKE_sgesv(101/*by definition*/, n, nrhs, pA, n, pipiv, pB, nrhs);
                }
            }
            //
            if (info != 0)
                throw new Exception($"magma_sgesv failed due to invalid parameter {-info}.");

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
            int[] ipiv = new int[n]; //(int*)malloc(n * sizeof(int));

            //define arrays
            fixed (double* pA = Ac, pB = Bc)
            {
                fixed (int* pipiv = ipiv)
                {
                    //pInvoke call
                    info = LAPACKE_dgesv(101, n, nrhs, pA, n, pipiv, pB, nrhs);
                }
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
            int[] ipiv = new int[n]; 
            //define arrays
            fixed (double* pA = Ac, pB = Bc)
            {
                fixed (int* pipiv = ipiv)
                {
                    //pInvoke call
                    info = LAPACKE_dgesv(101, n, nrhs, pA, n, pipiv, pB, nrhs);
                }
            }
            //
            if (info != 0)
                throw new Exception($"magma_sgesv failed due to invalid parameter {-info}.");

            //
            return Bc;
        }
        #endregion

        #region LSS - least square solver
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_sgels(int matrix_layout, char trans, int m, int n, int nrhs, float* a, int lda, float* b, int ldb);
        

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_dgels(int matrix_layout, char trans, int m, int n, int nrhs, double* a, int lda, double* b, int ldb);

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
                info = LAPACKE_sgels(101,'N', m, n, nrhs, pA, n, pB, nrhs);

                //
                if (info != 0)
                    throw new Exception($"lapack_sgesv failed due to invalid parameter {-info}.");

                //X(n, nrhs) matrix is a submatrix of B(m, nrhs).
                var X = new float[n, nrhs];
                Array.Copy(Bc, X, n * nrhs);
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
                info = LAPACKE_dgels(101, 'N', m, n, nrhs, pA, n, pB, nrhs);

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
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_sgeev(int matrix_layout, char jobvl, char jobvr, int n, float* a, int lda, float* wr,
                          float* wi, float* vl, int ldvl, float* vr, int ldvr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_dgeev(int matrix_layout, char jobvl, char jobvr, int n, double* a, int lda, double* wr,
                          double* wi, double* vl, int ldvl, double* vr, int ldvr);
        /// <summary>
        /// Computes the eigenvalues and left and right eigenvectors of a general matrix.
        /// 
        /// The routine computes for an n-by-n real nonsymmetric matrix A, the eigenvalues and, optionally, 
        /// the left and/or right eigenvectors. The right eigenvector v of A satisfies A* v = λ * v where λ is its eigenvalue.
        /// 
        /// The left eigenvector u of A satisfies uH* A = λ * uH where uH denotes the conjugate transpose of u. 
        /// The computed eigenvectors are normalized to have Euclidean norm equal to 1 and largest component real.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="computeLeft"></param>
        /// <param name="computeRight"></param>
        /// <returns></returns>
        public static (float[] wr, float[] wi, float[,] VL, float[,] VR) Eigen(float[,] A, bool computeLeft = false, bool computeRight = false)
        {
            //define parameters
            int info = -1;
            int n = A.GetLength(0);
            if (n != A.GetLength(1))
                throw new Exception("Matrix A must be squared!");

            //define arrays
            var Ac = A.Clone() as float[,];
            var wr = new float[n];
            var wi = new float[n];
            var VL = new float[n, n];
            var VR = new float[n, n];

            char jjobvl = 'N', jjobvr = 'N';

            //left and right matrices
            if (computeLeft)
                jjobvl = 'V';
            else
                jjobvl = 'N';

            if (computeRight)
                jjobvr = 'V';
            else
                jjobvr = 'N';

            fixed (float* pA = Ac, pwr = wr, pwi = wi, pVL = VL, pVR = VR)
            {
                info = LAPACKE_sgeev(101, jjobvl, jjobvr, n, pA, n, pwr, pwi, pVL, n, pVR, n);
            }
            //
            return (wr, wi, VL, VR);
        }

        public static (double[] wr, double[] wi, double[,] VL, double[,] VR) Eigen(double[,] A, bool computeLeft, bool computeRight)
        {
            //define parameters
            int info = -1;
            int n = A.GetLength(0);
            if (n != A.GetLength(1))
                throw new Exception("Matrix A must be squared!");

            //define arrays
            var Ac = A.Clone() as double[,];
            var wr = new double[n];
            var wi = new double[n];
            var VL = new double[n, n];
            var VR = new double[n, n];

            char jjobvl = 'N', jjobvr = 'N';

            //left and right matrices
            if (computeLeft)
                jjobvl = 'V';
            else
                jjobvl = 'N';

            if (computeRight)
                jjobvr = 'V';
            else
                jjobvr = 'N';

            fixed (double* pA = Ac, pwr = wr, pwi = wi, pVL = VL, pVR = VR)
            {
                info = LAPACKE_dgeev(101, jjobvl, jjobvr, n, pA, n, pwr, pwi, pVL, n, pVR, n);
            }
            //
            return (wr, wi, VL, VR);
        }
        #endregion

        #region SVD singular value decomposition
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_sgesvd(int matrix_layout, char jobu, char jobvt,int m, int n, float* a, int lda,
                           float* s, float* u, int ldu, float* vt, int ldvt, float* superb);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_dgesvd(int matrix_layout, char jobu, char jobvt, int m, int n, double* a, int lda,
                           double* s, double* u, int ldu, double* vt, int ldvt, double* superb);


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
            float [] superb = new float[Math.Min(n,m)];
            char jobU= 'N';//no vector calculation
            char jobV = 'N';

            if (calcU)
                jobU = 'A';//all
            //
            if (calcVt)
                jobV = 'A';


            //Initialize unmanaged memory to hold the array.
            fixed (float* pA = Ac, ps = s, pU = U, pVT = VT, psuperb= superb)
            {

                //pInvoke call
                int info = -1;
                info = LAPACKE_sgesvd(101, jobU, jobV, m, n, pA, n, ps, pU, m, pVT, n, psuperb);

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
            double[] s = new double[n];
            double[,] U = new double[m, m];
            double[,] VT = new double[n, n];
            double [] superb = new double[Math.Min(n, m)];
            char jobU = 'N';//no vector calculation
            char jobV = 'N';

            if (calcU)
                jobU = 'A';//all
            //
            if (calcVt)
                jobV = 'A';

            //Initialize unmanaged memory to hold the array.
            fixed (double* pA = Ac, ps = s, pU = U, pVT = VT, psuperb = superb)
            {

                //pInvoke call
                int info = -1;
                info = LAPACKE_dgesvd(101, jobU, jobV, m, n, pA, n, ps, pU, m, pVT, n, psuperb);

                if (info != 0)
                    throw new Exception($"lapack_svd failed due to invalid parameter {-info}.");

                return (s, U, VT);
            }
        }
        #endregion

        #region Matrix operations

 
        // 
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void cblas_sgemm(CBLAS_LAYOUT Layout, CBLAS_TRANSPOSE TransA,CBLAS_TRANSPOSE TransB, int m, int n,int k, float alpha, float* A,
                 int lda, float* B, int ldb, float beta, float* C, int ldc);
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void cblas_dgemm(CBLAS_LAYOUT Layout, CBLAS_TRANSPOSE TransA, CBLAS_TRANSPOSE TransB, int m, int n, int k, double alpha, double* A,
                 int lda, double* B, int ldb, double beta, double* C, int ldc);

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
        public static float[,] MMult(float[,] A, float[,] B, float[,] C = null, float alpha = 1, float betta = 1)
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
            fixed (float* pA = Ac, pB = Bc, pC = Cc)
            {
                //pInvoke call
                cblas_sgemm(CBLAS_LAYOUT.CblasRowMajor, CBLAS_TRANSPOSE.CblasNoTrans, CBLAS_TRANSPOSE.CblasNoTrans, m, n, k, alpha, pA, k, pB, n, betta, pC, n);
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
                cblas_sgemm(CBLAS_LAYOUT.CblasRowMajor, CBLAS_TRANSPOSE.CblasNoTrans, CBLAS_TRANSPOSE.CblasNoTrans, m, n, k, alpha, pA, k, pB, n, betta, pC, n);
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
        public static double[,] MMult(double[,] A, double[,] B, double[,] C = null, double alpha = 1, double betta = 1)
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
            var Cc = C == null ? new double[m, n] : C.Clone() as double[,];

            //define arrays
            fixed (double* pA = Ac, pB = Bc, pC = Cc)
            {
                //pInvoke call
                cblas_dgemm(CBLAS_LAYOUT.CblasRowMajor, CBLAS_TRANSPOSE.CblasNoTrans, CBLAS_TRANSPOSE.CblasNoTrans, m, n, k, alpha, pA, k, pB, n, betta, pC, n);
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
                cblas_dgemm(CBLAS_LAYOUT.CblasRowMajor, CBLAS_TRANSPOSE.CblasNoTrans, CBLAS_TRANSPOSE.CblasNoTrans, m, n, k, alpha, pA, k, pB, n, betta, pC, n);
            }
            //
            return Cc;
        }


        #region Matrix Inverse
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_sgetrf(int matrix_layout, int m, int n, float* a, int lda, int* ipiv);
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_sgetri(int matrix_layout, int n, float* a, int lda, int* ipiv );
        ///
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_dgetrf(int matrix_layout, int m, int n, double* a, int lda, int* ipiv);
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LAPACKE_dgetri(int matrix_layout, int n, double* a, int lda, int* ipiv);

        /// <summary>
        /// Computes the inverse of an LU-factored general matrix.
        /// The routine computes the inverse inv(A) of a general matrix A. Before calling this routine, call ?getrf to factorize A.
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static float[,] MInverse(float[,] A)
        {
            //define parameters
            int info = -1;
            int n = A.GetLength(0);
            var Ac = A.Clone() as float[,];
            int[] ipiv = new int[n];
            //define arrays

            fixed (float* pA = Ac)
            {
                fixed (int* pipiv = ipiv)
                {
                    info = LAPACKE_sgetrf(101, n, n, pA, n, pipiv);
                    if (info > 0)
                        throw new Exception($"lapack_sgesv failed due to invalid parameter {-info}.");

                    info = LAPACKE_sgetri(101, n, pA, n, pipiv);
                }
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
            int[] ipiv = new int[n];
            //define arrays
            fixed (double* pA = Ac)
            {
                fixed (int* pipiv = ipiv)
                {
                    info = LAPACKE_dgetrf(101, n, n, pA, n, pipiv);
                    if (info > 0)
                        throw new Exception($"lapack_sgesv failed due to invalid parameter {-info}.");

                    info = LAPACKE_dgetri(101, n, pA, n, pipiv);
                }

            }
            //
            if (info != 0)
                throw new Exception($"lapack_sgesv failed due to invalid parameter {-info}.");

            //
            return Ac;
        }
        #endregion
        
        #endregion
    }
}
