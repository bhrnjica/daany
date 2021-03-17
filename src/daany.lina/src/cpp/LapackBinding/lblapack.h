
#pragma once

#include "pch.h"
/*
	Intel® Math Kernel Library LAPACK


	Driver routines
	Driver routines solve an entire problem.

	Name	Description
	------------------------------------------------------------------
	gesv	solve linear system, AX = B, A is general(non - symmetric)
	gels	least squares solve, AX = B, A is rectangular
	geev	non - symmetric eigenvalue solver, AX = X Lambda
	gesvd	singular value decomposition(SVD), A = U Sigma V^ H
	
*/

namespace LapackBinding
{
	
	enum class lbvector
	{
		NoVec = 301,  /* geev, syev, gesvd */
		Vector = 302,  /* geev, syev */
		IVector = 303,  /* stedc */
		AllVector = 304,  /* gesvd, trevc */
		SomeVector = 305,  /* gesvd, trevc */
		OverwriteVec = 306,  /* gesvd */
		BacktransVec = 307   /* trevc */
	};

	enum class lbtrans
	{
		NoTrans,//none
		Trans,//transpose the matrix before operations
		ConjTrans,//not supported yet
	};
	//
	char convertToChar(lbvector vec);
	void transpose(const float* src, float* dst, const int N, const int M);
	void transpose(const double* src, double* dst, const int N, const int M);

	//AX=B - solver
	extern "C" MKLBINDINGS_API int lbsgesv_cpu(bool rowmajor, int n, int nrhs, float* A, int lda, float* B, int ldb);
	//double
	extern "C" MKLBINDINGS_API int lbdgesv_cpu(bool rowmajor, int n, int nrhs, double* A, int lda,  double* B, int ldb);
	
	//SVD
	extern "C" MKLBINDINGS_API int lbsgesvd_cpu(bool rowmajor, lbvector jobu, lbvector jobv, int m, int n, float* A, int lda, float* s, float* U, int ldu, float* VT, int ldvt);
	extern "C" MKLBINDINGS_API int lbsgesvds_cpu(bool rowmajor, int m, int n, float* A, float* s, float* U, bool calcU, float* VT, bool calcV);
	
	//SVD
	extern "C" MKLBINDINGS_API int lbdgesvd_cpu(bool rowmajor, lbvector jobu, lbvector jobv, int m, int n, double* A, int lda, double* s, double* U, int ldu, double* VT, int ldvt);
	extern "C" MKLBINDINGS_API int lbdgesvds_cpu(bool rowmajor, int m, int n, double* A, double* s, double* U, bool calcU, double* VT, bool calcV);
	
	

	//LSS - least squares solver
	extern "C" MKLBINDINGS_API int lbsgels_cpu(bool rowmajor, int m, int n, int nrhs, float* A, int lda, float* B, int lbd);
	extern "C" MKLBINDINGS_API int lbdgels_cpu(bool rowmajor, int m, int n, int nrhs, double* A, int lda, double* B, int lbd);
	
	//EIGEN
	extern "C" MKLBINDINGS_API int lbsgeevs_cpu(bool rowmajor, int n, float* A, int lda, float* wr, float* wi, float* VL, bool computeLeft, float* Vr, bool computeRight);
	extern "C" MKLBINDINGS_API int lbsgeev_cpu(bool rowmajor, char jobvl, char jobvr, int n, float* A, int lda, float* wr, float* wi, float* Vl, int ldvl, float* Vr, int ldvr);
	
	extern "C" MKLBINDINGS_API int lbdgeevs_cpu(bool rowmajor, int n, double* A, int lda, double* wr, double* wi, double* VL, bool computeLeft, double* VR, bool computeRight);
	extern "C" MKLBINDINGS_API int lbdgeev_cpu(bool rowmajor, char jobvl, char jobvr, int n, double* A, int lda, double* wr, double* wi, double* Vl, int ldvl, double* Vr, int ldvr);

	//Matrix-Matrix multiplication
	extern "C" MKLBINDINGS_API void lbsgemm_cpu(bool rowmajor, int m, int n, int k, float alpha, const float* A, int lda, const float* B, int ldb, float beta, float* C, int ldc);
	extern "C" MKLBINDINGS_API void lbdgemm_cpu(bool rowmajor, int m, int n, int k, double alpha, const double* A, int lda, const double* B, int ldb, double beta, double* C, int ldc);

	//Transpose 
	extern "C" MKLBINDINGS_API void lbstranspose_cpu(bool rowmajor, int m, int n, const float* A, int lda, float* At, int ldat);
	extern "C" MKLBINDINGS_API void lbdtranspose_cpu(bool rowmajor, int m, int n, const double* A, int lda, double* At, int ldat);

	//Inverse matrix
	extern "C" MKLBINDINGS_API int lbdinverse_cpu(bool rowmajor, int n, double* A, int lda);
	extern "C" MKLBINDINGS_API int lbsinverse_cpu(bool rowmajor, int n, float* A, int lda);
}
