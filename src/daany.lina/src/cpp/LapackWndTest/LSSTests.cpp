#include <iostream>
// includes, system
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <math.h>

#include <cassert>
#include "lblapack.h"

#include "mkl_lapacke.h"
#include "HelperTest.h"

using namespace LapackBinding;




void mv2sgels_cpu_test()
{
	/* Locals */
	int const M = 6, N = 4, NRHS = 2, LDA = M, LDB = M;
	int m = M, n = N, nrhs = NRHS, lda = LDA, ldb = LDB, info;
	
	/* Local arrays (Col major matrix) */
	float a[LDA * N] = {
		1.44f, -9.96f, -7.55f,  8.34f,  7.08f, -5.45f,
	   -7.84f, -0.28f,  3.24f,  8.09f,  2.52f, -5.70f,
	   -4.39f, -3.24f,  6.27f,  5.28f,  0.74f, -1.19f,
		4.53f,  3.83f, -6.64f,  2.06f, -2.47f,  4.70f
	};
	float b[LDB * NRHS] = {
		8.58f,  8.26f,  8.48f, -5.28f,  5.72f,  8.93f,
		9.35f, -4.43f, -0.70f, -0.26f, -7.36f, -2.52f
	};

	/* Executable statements */
	printf("mbv2sgels_cpu test \n");

	info = lbsgels_cpu(false, m, n, nrhs, a, lda, b, ldb);

	assert(0 == info);
	assert(-0.45f == round_up(b[0], 2));
	assert(-0.85f == round_up(b[1], 2));
	assert(0.71f == round_up(b[2], 2));
	assert(0.13f == round_up(b[3], 2));


	/* Print least squares solution */
	print_matrix((char*)"Least squares solution", n, nrhs, b, ldb);
	/* Print residual sum of squares for the solution */
	print_vector_norm((char*)"Residual sum of squares for the solution", m - n, nrhs,
		&b[n], ldb);
	/* Print details of QR factorization */

}




void mv2dgels_cpu_test()
{
	/* Locals */
	int const M = 6, N = 4, NRHS = 2, LDA = M, LDB = M;
	int m = M, n = N, nrhs = NRHS, lda = LDA, ldb = LDB, info;
	
	/* Local arrays (Col major matrix) */
	double a[LDA * N] = {
		1.44f, -9.96f, -7.55f,  8.34f,  7.08f, -5.45f,
	   -7.84f, -0.28f,  3.24f,  8.09f,  2.52f, -5.70f,
	   -4.39f, -3.24f,  6.27f,  5.28f,  0.74f, -1.19f,
		4.53f,  3.83f, -6.64f,  2.06f, -2.47f,  4.70f
	};
	double b[LDB * NRHS] = {
		8.58f,  8.26f,  8.48f, -5.28f,  5.72f,  8.93f,
		9.35f, -4.43f, -0.70f, -0.26f, -7.36f, -2.52f
	};

	/* Executable statements */
	printf("mbv2dgels_cpu test \n");

	info = lbdgels_cpu(false, m, n, nrhs, a, lda, b, ldb);

	assert(0 == info);
	assert(-0.45 == round_up(b[0], 2));
	assert(-0.85 == round_up(b[1], 2));
	assert(0.71 == round_up(b[2], 2));
	assert(0.13 == round_up(b[3], 2));


	/* Print least squares solution */
	print_matrix((char*)"Least squares solution", n, nrhs, b, ldb);
	/* Print residual sum of squares for the solution */
	print_vector_norm((char*)"Residual sum of squares for the solution", m - n, nrhs,
		&b[n], ldb);
	/* Print details of QR factorization */

}

