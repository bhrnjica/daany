#include <iostream>
// includes, system
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <math.h>

#include <cassert>
#include "lblapack.h"

#include "HelperTest.h"

using namespace LapackBinding;




//void testSVD_Lapack()
//{
//	const int M = 6;//number of rows
//	const int N = 5;//number of columns
//	const int LDA = N;//Leadning dimension for A
//	const int LDU = M;//Leading dimensions for U
//	const int LDVT = N;//Leading dimensions for N
//
//	/* Locals */
//	MKL_INT m = M, n = N, lda = LDA, ldu = LDU, ldvt = LDVT, info;
//	double superb[min(M, N) - 1];
//	/* Local arrays */
//	double s[N], u[LDU * M], vt[LDVT * N];
//	double a[LDA * M] = {//(ROW major matrix)
//		8.79,  9.93,  9.83, 5.45,  3.16,
//		6.11,  6.91,  5.04, -0.27,  7.98,
//		-9.15, -7.93,  4.86, 4.85,  3.01,
//		9.57,  1.64,  8.83, 0.74,  5.80,
//		-3.49,  4.02,  9.80, 10.00,  4.27,
//		9.84,  0.15, -8.99, -6.02, -5.31
//	};
//	/* Executable statements */
//	printf("LAPACKE_dgesvd (row-major, high-level) Example Program Results\n");
//
//	/* Compute SVD */
//	info = LAPACKE_dgesvd(LAPACK_ROW_MAJOR, 'A', 'A', m, n, a, lda, s, u, ldu, vt, ldvt, superb);
//
//	/* Check for convergence */
//	if (info > 0) {
//		printf("The algorithm computing SVD failed to converge.\n");
//		return;
//	}
//	/* Print singular values */
//	print_matrix((char*)"Singular values", 1, n, s, 1);
//	/* Print left singular vectors */
//	print_matrix((char*)"Left singular vectors (stored columnwise)", m, n, u, ldu);
//	/* Print right singular vectors */
//	print_matrix((char*)"Right singular vectors (stored rowwise)", n, n, vt, ldvt);
//
//}