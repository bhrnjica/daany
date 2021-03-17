
#include "pch.h"
#include "lblapack.h"
#include <stdio.h>
#include <crtdbg.h>
#include <mkl.h>
namespace LapackBinding
{
	extern void print_matrix(char* desc, int m, int n, float* a, const int lda);
	
	int lbdgesv_cpu(bool rowmajor, int n, int nrhs, double* A, int lda,  double* B, int ldb)
	{
		int info;
		int* ipiv = (int*)malloc(n * sizeof(int));
		if (rowmajor)
			info = LAPACKE_dgesv(LAPACK_ROW_MAJOR, n, nrhs, A, lda, ipiv, B, ldb);
		else
			info = LAPACKE_dgesv(LAPACK_COL_MAJOR, n, nrhs, A, lda, ipiv, B, ldb);

		free(ipiv);
		return info;

	}

	int lbsgesv_cpu(bool rowmajor, int n, int nrhs, float* A, int lda, float* B, int ldb)
	{
		int info;
		int* ipiv = (int*)malloc(n * sizeof(int));
		if(rowmajor)
			info = LAPACKE_sgesv(LAPACK_ROW_MAJOR, n, nrhs, A, lda, ipiv, B, ldb);
		else
			info = LAPACKE_sgesv(LAPACK_COL_MAJOR, n, nrhs, A, lda, ipiv, B, ldb);

		free(ipiv);
		return info;

	}

	//SVD
	int lbsgesvds_cpu(bool rowmajor, int m, int n, float* A, float* s, float* U, bool calcU, float* VT, bool calcV)
	{
		//U and V matrices
		lbvector jobV;
		lbvector jobU;

		if (calcU)
			jobU = lbvector::AllVector;
		else
			jobU = lbvector::NoVec;
		//
		if(calcV)
			jobV = lbvector::AllVector;
		else
			jobV = lbvector::NoVec;
		//
		int lda = rowmajor ? n : m;
		int ldu = m;
		int ldvt = n;
		return lbsgesvd_cpu(rowmajor, jobU, jobV, m, n, A, lda, s, U, ldu, VT, ldvt);
	}
	
	int lbsgesvd_cpu(bool rowmajor, lbvector jobu, lbvector jobv, int m, int n, float* A, int lda, float* s, float* U, int ldu, float* VT, int ldvt)
	{
		
		int info;
		//convert job
		char jobU = convertToChar(jobu);
		char jobV = convertToChar(jobv);

		//query for workspace size
		const int dim = min(m, n) - 1;
		float* superb = (float*)malloc(dim * sizeof(float));
		
		//print_matrix((char*)"Matrica ", m, n, A, lda);

		/* Compute SVD */
		if(rowmajor)
			info = LAPACKE_sgesvd(LAPACK_ROW_MAJOR, jobU, jobV, m, n, A, lda, s, U, ldu, VT, ldvt, superb);
		else
			info = LAPACKE_sgesvd(LAPACK_COL_MAJOR, jobU, jobV, m, n, A, lda, s, U, ldu, VT, ldvt, superb);

		/* Check for convergence */
		if (info > 0)
		{
			printf("The algorithm computing SVD failed to converge.\n");
		}

		free(superb); // free host memory
		return info;
	}

	int lbdgesvd_cpu(bool rowmajor, lbvector jobu, lbvector jobv, int m, int n, double* A, int lda, double* s, double* U, int ldu, double* VT, int ldvt)
	{
		int info;
		//convert job
		char jobU = convertToChar(jobu);
		char jobV = convertToChar(jobv);

		//query for workspace size
		int dim = min(m, n) - 1;
		double* superb = (double*)malloc(dim * sizeof(double));

		/* Compute SVD */
		if(rowmajor)
			info = LAPACKE_dgesvd(LAPACK_ROW_MAJOR, jobU, jobV, m, n, A, lda, s, U, ldu, VT, ldvt, superb);
		else
			info = LAPACKE_dgesvd(LAPACK_COL_MAJOR, jobU, jobV, m, n, A, lda, s, U, ldu, VT, ldvt, superb);

		/* Check for convergence */
		if (info > 0) {
			printf("The algorithm computing SVD failed to converge.\n");
		}

		free(superb); // free host memory
		return info;
	}
	
	int lbdgesvds_cpu(bool rowmajor, int m, int n, double* A, double* s, double* U, bool calcU, double* VT, bool calcV)
	{
		//U and V matrices
		lbvector jobV;
		lbvector jobU;

		if (calcU)
			jobU = lbvector::AllVector;
		else
			jobU = lbvector::NoVec;
		//
		if (calcV)
			jobV = lbvector::AllVector;
		else
			jobV = lbvector::NoVec;
		//
		int lda = rowmajor ? n : m;
		int ldu = m;
		int ldvt = n;
		return lbdgesvd_cpu(rowmajor, jobU, jobV, m, n, A, lda, s, U, ldu, VT, ldvt);
	}
	
	//LSS
	int lbsgels_cpu(bool rowmajor, int m, int n, int nrhs, float* A, int lda, float* B, int ldb)
	{
		//declare helpers
		int info;
		
		/* Solve the equations A*X = B */
		if(rowmajor)
			info = LAPACKE_sgels(LAPACK_ROW_MAJOR,'N', m, n, nrhs, A, lda, B, ldb);
		else
			info = LAPACKE_sgels(LAPACK_COL_MAJOR, 'N', m, n, nrhs, A, lda, B, ldb);

		/* Check for the full rank */
		if (info > 0) {
			printf("The diagonal element %i of the triangular factor ", info);
			printf("of A is zero, so that A does not have full rank;\n");
			printf("the least squares solution could not be computed.\n");
		}

		return info;

	}

	int lbdgels_cpu(bool rowmajor, int m, int n, int nrhs, double* A, int lda, double* B, int ldb)
	{
		//declare helpers
		int info;

		/* Solve the equations A*X = B */
		if(rowmajor)
			info = LAPACKE_dgels(LAPACK_ROW_MAJOR, 'N', m, n, nrhs, A, lda, B, ldb);
		else
			info = LAPACKE_dgels(LAPACK_COL_MAJOR, 'N', m, n, nrhs, A, lda, B, ldb);


		/* Check for the full rank */
		if (info > 0) {
			printf("The diagonal element %i of the triangular factor ", info);
			printf("of A is zero, so that A does not have full rank;\n");
			printf("the least squares solution could not be computed.\n");
		}

		return info;

	}

	//EIGEN
	int lbsgeevs_cpu(bool rowmajor, int n, float* A, int lda, float* wr, float* wi, float* Vl, bool computeLeft, float* Vr, bool computeRight)
	{
		char jjobvl = 'N', jjobvr = 'N';
		//left and right matrices
		if (computeLeft)
			char jjobvl = 'V';
		else
			char jjobvl = 'N';

		if (computeRight)
			char jjobvr = 'V';
		else
			char jjobvr = 'N';

		return lbsgeev_cpu(rowmajor, jjobvl, jjobvr, n, A, lda, wr, wi, Vl, n, Vr, n);
	}
	
	int lbsgeev_cpu(bool rowmajor, char jobvl, char jobvr, int n, float* A, int lda, float* wr, float* wi, float* Vl, int ldvl, float* Vr, int ldvr)
	{
		int info;

		/* Compute SVD */
		if(rowmajor)
			info = LAPACKE_sgeev(LAPACK_ROW_MAJOR, jobvl, jobvr, n, A, lda, wr, wi, Vl, ldvr, Vr, ldvr);
		else 
			info = LAPACKE_sgeev(LAPACK_COL_MAJOR, jobvl, jobvr, n, A, lda, wr, wi, Vl, ldvr, Vr, ldvr);

		/* Check for convergence */
		if (info > 0) 
		{
			printf("The algorithm computing Eigen values failed to converge.\n");
		}

		//
		return info;
	}
	
	int lbdgeevs_cpu(bool rowmajor, int n, double* A, int lda, double* wr, double* wi, double* Vl, bool computeLeft, double* Vr, bool computeRight)
	{
		char jjobvl = 'N', jjobvr = 'N';

		//left and right matrices
		if(computeLeft)
			char jjobvl = 'V';
		else
			char jjobvl = 'N';

		if(computeRight)
			char jjobvr = 'V';
		else
			char jjobvr = 'N';

		//
		return lbdgeev_cpu(rowmajor, jjobvl, jjobvr, n, A, lda, wr, wi, Vl, n, Vr, n);
	}

	int lbdgeev_cpu(bool rowmajor, char jobvl, char jobvr, int n, double* A, int lda, double* wr, double* wi, double* Vl, int ldvl,double* Vr, int ldvr)
	{
		int info;

		/* Compute SVD */
		if(rowmajor)
			info = LAPACKE_dgeev(LAPACK_ROW_MAJOR, jobvl, jobvr, n, A, lda, wr, wi, Vl, ldvr, Vr, ldvr);
		else
			info = LAPACKE_dgeev(LAPACK_COL_MAJOR, jobvl, jobvr, n, A, lda, wr, wi, Vl, ldvr, Vr, ldvr);

		/* Check for convergence */
		if (info > 0) {
			printf("The algorithm computing Eigen values failed to converge.\n");
		}

		//
		return info;
	}

	//Matrix-Matrix operations
	void lbsgemm_cpu(bool rowmajor, int m, int n, int k, float alpha,const float* A, int lda, const float* B, int ldb, float beta, float* C, int ldc)
	{
		CBLAS_TRANSPOSE trA = CBLAS_TRANSPOSE::CblasNoTrans;
		CBLAS_TRANSPOSE trB = CBLAS_TRANSPOSE::CblasNoTrans;

		/*if (opA == mbv2trans::Trans)
			trA = CBLAS_TRANSPOSE::CblasTrans;
		else if (opA == mbv2trans::ConjTrans)
			throw WS_E_NOT_SUPPORTED;

		if (opB == mbv2trans::Trans)
			trB = CBLAS_TRANSPOSE::CblasTrans;
		else if (opB == mbv2trans::ConjTrans)
			throw WS_E_NOT_SUPPORTED;*/


		if (rowmajor)
		{
			
			cblas_sgemm(CBLAS_LAYOUT::CblasRowMajor, trA, trB, m, n, k, alpha, A, lda, B, ldb, beta, C, ldc);
		}
		else
		{
			cblas_sgemm(CBLAS_LAYOUT::CblasColMajor, trA, trB, m, n, k, alpha, A, lda, B, ldb, beta, C, ldc);
		}

		
	}

	void lbdgemm_cpu(bool rowmajor, int m, int n, int k, double alpha, const double* A, int lda, const double* B, int ldb, double beta, double* C, int ldc)
	{
		CBLAS_TRANSPOSE trA = CBLAS_TRANSPOSE::CblasNoTrans;
		CBLAS_TRANSPOSE trB = CBLAS_TRANSPOSE::CblasNoTrans;

		/*if (opA == mbv2trans::Trans)
			trA = CBLAS_TRANSPOSE::CblasTrans;
		else if (opA == mbv2trans::ConjTrans)
			throw WS_E_NOT_SUPPORTED;

		if (opB == mbv2trans::Trans)
			trB = CBLAS_TRANSPOSE::CblasTrans;
		else if (opB == mbv2trans::ConjTrans)
			throw WS_E_NOT_SUPPORTED;*/


		if (rowmajor)
		{

			cblas_dgemm(CBLAS_LAYOUT::CblasRowMajor, trA, trB, m, n, k, alpha, A, lda, B, ldb, beta, C, ldc);
		}
		else
		{
			cblas_dgemm(CBLAS_LAYOUT::CblasColMajor, trA, trB, m, n, k, alpha, A, lda, B, ldb, beta, C, ldc);

		}


	}

	void lbstranspose_cpu(bool rowmajor, int m, int n, const float* A, int lda, float* At, int ldat)
	{

		if (rowmajor)
		{
			transpose(A, At, m, n);
		}
		else
		{
			transpose(A, At, n, m);
		}
	}

	void lbdtranspose_cpu(bool rowmajor, int m, int n, const double* A, int lda, double* At, int ldat)
	{

		if (rowmajor)
		{
			transpose(A, At, m, n);
		}
		else
		{
			transpose(A, At, n, m);
		}
	}

	//Inverse matrix
	int lbsinverse_cpu(bool rowmajor, int n, float* A, int lda)
	{
		int info;
		int* ipiv = (int*)malloc(n * sizeof(int));
		
		if (rowmajor)
		{
			LAPACKE_sgetrf(LAPACK_ROW_MAJOR,n, n, A, n,ipiv);
			info = LAPACKE_sgetri(LAPACK_ROW_MAJOR, n, A, lda, ipiv);
			// Error checking
			if (info > 0)
			{
				return info;
			}
		}
		else
		{
			info = LAPACKE_sgetrf(LAPACK_COL_MAJOR, n, n, A, n, ipiv);
			if (info > 0)
			{
				return info;
			}
			info = LAPACKE_sgetri(LAPACK_COL_MAJOR, n, A, lda, ipiv);
		}
		free(ipiv);
		return info;
	}

	int lbdinverse_cpu(bool rowmajor, int n, double* A, int lda)
	{
		int info;
		int* ipiv = (int*)malloc(n * sizeof(int));
		if (rowmajor)
		{
			info = LAPACKE_dgetrf(LAPACK_ROW_MAJOR, n, n, A, n, ipiv);
			if (info > 0)
			{
				return info;
			}
			info = LAPACKE_dgetri(LAPACK_ROW_MAJOR, n, A, lda, ipiv);
		}
		else
		{
			info = LAPACKE_dgetrf(LAPACK_COL_MAJOR, n, n, A, n, ipiv);
			if (info > 0)
			{
				return info;
			}
			info = LAPACKE_dgetri(LAPACK_COL_MAJOR, n, A, lda, ipiv);
		}
		free(ipiv);
		return info;
	}

	//Util

	void transpose(const float* src, float* dst, const int N, const int M)
	{
#pragma omp parallel for
		for (int n = 0; n < N * M; n++)
		{
			int i = n / N;
			int j = n % N;
			dst[n] = src[M * j + i];
		}
	}

	void transpose(const double* src, double* dst, const int N, const int M)
	{
#pragma omp parallel for
		for (int n = 0; n < N * M; n++)
		{
			int i = n / N;
			int j = n % N;
			dst[n] = src[M * j + i];
		}
	}

	char convertToChar(lbvector vec)
	{
		switch (vec)
		{
			case lbvector::NoVec:
				return 'N';
				break;
			case lbvector::AllVector:
				return 'A';
			case lbvector::SomeVector:
				return 'S';
			case lbvector::OverwriteVec:
				return 'O';
			default:
				return 'N';
		}
		return 'N';
	}

	void print_matrix(char* desc, int m, int n, float* a, const int lda) {
		int i, j;
		printf("\n %s\n", desc);
		for (i = 0; i < m; i++)
		{
			for (j = 0; j < n; j++)
				printf(" %6.2f", a[i * lda + j]);

			printf("\n");
		}
	}


}

