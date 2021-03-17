#include <iostream>
// includes, system
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <math.h>
#include "lblapack.h"
#include "HelperTest.h"
#include <cassert>

using namespace LapackBinding;

void mv2dgesv_cpu_test()
{
	const int n = 5;//the number of rows of A matrix
	const int nrhs = 3;//the number of columns of B matrix
	const int lda = n;//leading dimension of A matrix is n (rows)
	const int ldb = n;//leading  dimensions of B matrix (same as A , rows)
	int ipiv[5];
	int info;

	//(Col major matrix)
	double A[lda * n] = {
		6.80, -2.11,  5.66,  5.97,  8.23,
	   -6.05, -3.30,  5.36, -4.44,  1.08,
	   -0.45,  2.58, -2.70,  0.27,  9.04,
		8.32,  2.71,  4.35, -7.17,  2.14,
	   -9.67, -5.14, -7.26,  6.08, -6.87
	};
	double B[ldb * nrhs] = {
		4.02,  6.19, -8.22, -7.57, -3.03,
	   -1.56,  4.00, -8.67,  1.75,  2.86,
		9.81, -4.09, -4.57, -8.61,  8.99
	};

	/* Executable statements */
	printf("mbv2dgesv_cpu test\n");

	/* Solve the equations A*X = B */
	info = lbdgesv_cpu(false, n, nrhs, A, lda, B, ldb);

	/* Check for the exact singularity */
	if (info > 0) {
		printf("The diagonal element of the triangular factor of A,\n");
		printf("U(%i,%i) is zero, so that A is singular;\n", info, info);
		printf("the solution could not be computed.\n");
		exit(1);
	}

	assert(0 == info);
	assert(-0.80 == round_up(B[0], 2));
	assert(-0.70 == round_up(B[1], 2));
	assert(0.59 == round_up(B[2], 2));
	assert(1.32 == round_up(B[3], 2));
	assert(0.57 == round_up(B[4], 2));

	/* Print solution */
	print_matrix((char*)"Solution", n, nrhs, B, ldb);

	/* Print details of LU factorization */
	print_matrix((char*)"Details of LU factorization", n, n, A, lda);

	/* Print pivot indices */
	print_int_vector((char*)"Pivot indices", n, ipiv);

	/*
	magma_dgesv(row - major, high - level) Example Program Results

	Solution
	- 0.80 - 0.39   0.96
	- 0.70 - 0.55   0.22
	0.59   0.84   1.90
	1.32 - 0.10   5.36
	0.57   0.11   4.04

	Details of LU factorization
	8.23   1.08   9.04   2.14 - 6.87
	0.83 - 6.94 - 7.92   6.55 - 3.99
	0.69 - 0.67 - 14.18   7.24 - 5.19
	0.73   0.75   0.02 - 13.82  14.19
	- 0.26   0.44 - 0.59 - 0.34 - 3.43

	Pivot indices
	5      5      3      4      5
	*/
}


void mv2sgesv_cpu_test()
{
	const int n = 5;//the number of rows of A matrix
	const int nrhs = 3;//the number of columns of B matrix
	const int lda = n;//leading dimension of A matrix is n (rows)
	const int ldb = n;//leading  dimensions of B matrix (same as A , rows)
	int ipiv[5];
	int info;

	//(Col major matrix)
	float A[lda * n] = {
		6.80f, -2.11f,  5.66f,  5.97f,  8.23f,
	   -6.05f, -3.30f,  5.36f, -4.44f,  1.08f,
	   -0.45f,  2.58f, -2.70f,  0.27f,  9.04f,
		8.32f,  2.71f,  4.35f, -7.17f,  2.14f,
	   -9.67f, -5.14f, -7.26f,  6.08f, -6.87f
	};
	float B[ldb * nrhs] = {
		4.02f,  6.19f, -8.22f, -7.57f, -3.03f,
	   -1.56f,  4.00f, -8.67f,  1.75f,  2.86f,
		9.81f, -4.09f, -4.57f, -8.61f,  8.99f
	};

	/* Executable statements */
	printf("mbv2sgesv_cpu test\n");

	/* Solve the equations A*X = B */
	info = lbsgesv_cpu(false, n, nrhs, A, lda,  B, ldb);

	/* Check for the exact singularity */
	if (info > 0) {
		printf("The diagonal element of the triangular factor of A,\n");
		printf("U(%i,%i) is zero, so that A is singular;\n", info, info);
		printf("the solution could not be computed.\n");
		exit(1);
	}

	assert(0 == info);
	assert(-0.80f == round_up(B[0], 2));
	assert(-0.70f == round_up(B[1], 2));
	assert(0.59f == round_up(B[2], 2));
	assert(1.32f == round_up(B[3], 2));
	assert(0.57f == round_up(B[4], 2));

	/* Print solution */
	print_matrix((char*)"Solution", n, nrhs, B, ldb);

	/* Print details of LU factorization */
	print_matrix((char*)"Details of LU factorization", n, n, A, lda);

	/* Print pivot indices */
	print_int_vector((char*)"Pivot indices", n, ipiv);

	/*
	magma_dgesv(row - major, high - level) Example Program Results

	Solution
	- 0.80 - 0.39   0.96
	- 0.70 - 0.55   0.22
	0.59   0.84   1.90
	1.32 - 0.10   5.36
	0.57   0.11   4.04

	Details of LU factorization
	8.23   1.08   9.04   2.14 - 6.87
	0.83 - 6.94 - 7.92   6.55 - 3.99
	0.69 - 0.67 - 14.18   7.24 - 5.19
	0.73   0.75   0.02 - 13.82  14.19
	- 0.26   0.44 - 0.59 - 0.34 - 3.43

	Pivot indices
	5      5      3      4      5
	*/
}




