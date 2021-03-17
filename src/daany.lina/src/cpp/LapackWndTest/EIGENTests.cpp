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


void mv2sgeevs_cpu_test()
{

	/* Locals */
	const int N = 5;//number of rows
	const int LDA = N;//Leading dimension for A
	const int LDVL = N;//Leading dimensions for VL
	const int LDVR = N;//Leading dimensions for VR

	/* Locals */
	int n = N, lda = LDA, ldvl = LDVL, ldvr = LDVR, info;
	lbvector jjobvl = lbvector::NoVec;
	lbvector jjobvr = lbvector::NoVec;

	/* Local arrays */
	float wr[N], wi[N], vl[LDVL * N], vr[LDVR * N];
	float a[LDA * N] = {
		 -1.01f,  3.98f,  3.30f,  4.43f,  7.31f,
			0.86f,  0.53f,  8.26f,  4.96f, -6.43f,
		   -4.60f, -7.04f, -3.89f, -7.66f, -6.16f,
			3.31f,  5.29f,  8.20f, -7.33f,  2.47f,
		   -4.81f,  3.55f, -1.51f,  6.18f,  5.58f
	};
	/* Executable statements */
	printf("mv2sgeevs_cpu test\n");

	/* Solve eigenproblem */
	info = lbsgeevs_cpu(false, n, a, lda,wr, wi, vl, ldvl, vr, ldvr);

	/* Check for convergence */
	if (info > 0) {
		printf("The algorithm failed to compute eigenvalues.\n");
		exit(1);
	}

	assert(0 == info);
	assert(2.86f == round_up(wr[0], 2));
	assert(2.86f == round_up(wr[1], 2));
	assert(-0.69f == round_up(wr[2], 2));
	assert(-0.69f == round_up(wr[3], 2));
	assert(-10.46f == round_up(wr[4], 2));

	/*assert(0 == info);
	assert(2.86 == round_up(wr[0], 2));
	assert(2.86 == round_up(wr[1], 2));
	assert(-0.69 == round_up(wr[2], 2));
	assert(-0.69 == round_up(wr[3], 2));
	assert(-10.46 == round_up(wr[4], 2));*/


	/* Print eigenvalues */
	print_eigenvalues((char*)"Eigenvalues", n, wr, wi);

	//For left and righ eigen vector you have to provide the first two argument with MagmaVec
	///* Print left eigenvectors */
	//print_eigenvectors((char*)"Left eigenvectors", n, wi, vl, ldvl);
	///* Print right eigenvectors */
	//print_eigenvectors((char*)"Right eigenvectors", n, wi, vr, ldvr);
}

void mv2sgeev_cpu_test()
{

	/* Locals */
	const int N = 5;//number of rows
	const int LDA = N;//Leading dimension for A
	const int LDVL = N;//Leading dimensions for VL
	const int LDVR = N;//Leading dimensions for VR

	/* Locals */
	int n = N, lda = LDA, ldvl = LDVL, ldvr = LDVR, info;
	lbvector jjobvl = lbvector::NoVec;
	lbvector jjobvr = lbvector::NoVec;

	/* Local arrays */
	float wr[N], wi[N], vl[LDVL * N], vr[LDVR * N];
	float a[LDA * N] = {
		 -1.01f,  3.98f,  3.30f,  4.43f,  7.31f,
			0.86f,  0.53f,  8.26f,  4.96f, -6.43f,
		   -4.60f, -7.04f, -3.89f, -7.66f, -6.16f,
			3.31f,  5.29f,  8.20f, -7.33f,  2.47f,
		   -4.81f,  3.55f, -1.51f,  6.18f,  5.58f
	};
	/* Executable statements */
	printf("mv2sgeev_cpu test\n");

	/* Solve eigenproblem */
	info = lbsgeev_cpu(false, 'N', 'N', n, a, lda, wr, wi, vl, ldvl, vr, ldvr);

	/* Check for convergence */
	if (info > 0) {
		printf("The algorithm failed to compute eigenvalues.\n");
		exit(1);
	}

	assert(0 == info);
	assert(2.86f == round_up(wr[0], 2));
	assert(2.86f == round_up(wr[1], 2));
	assert(-0.69f == round_up(wr[2], 2));
	assert(-0.69f == round_up(wr[3], 2));
	assert(-10.46f == round_up(wr[4], 2));

	/*assert(0 == info);
	assert(2.86 == round_up(wr[0], 2));
	assert(2.86 == round_up(wr[1], 2));
	assert(-0.69 == round_up(wr[2], 2));
	assert(-0.69 == round_up(wr[3], 2));
	assert(-10.46 == round_up(wr[4], 2));*/


	/* Print eigenvalues */
	print_eigenvalues((char*)"Eigenvalues", n, wr, wi);

	//For left and righ eigen vector you have to provide the first two argument with MagmaVec
	///* Print left eigenvectors */
	//print_eigenvectors((char*)"Left eigenvectors", n, wi, vl, ldvl);
	///* Print right eigenvectors */
	//print_eigenvectors((char*)"Right eigenvectors", n, wi, vr, ldvr);
}

void mv2dgeevs_test()
{
	/* Locals */
	const int N = 5;//number of rows
	const int LDA = N;//Leading dimension for A
	const int LDVL = N;//Leading dimensions for VL
	const int LDVR = N;//Leading dimensions for VR

	/* Locals */
	int n = N, lda = LDA, ldvl = LDVL, ldvr = LDVR, info;
	lbvector jjobvl = lbvector::NoVec;
	lbvector jjobvr = lbvector::NoVec;

	/* Local arrays */
	double wr[N], wi[N], vl[LDVL * N], vr[LDVR * N];
	double a[LDA * N] = {
		 -1.01f,  3.98f,  3.30f,  4.43f,  7.31f,
			0.86f,  0.53f,  8.26f,  4.96f, -6.43f,
		   -4.60f, -7.04f, -3.89f, -7.66f, -6.16f,
			3.31f,  5.29f,  8.20f, -7.33f,  2.47f,
		   -4.81f,  3.55f, -1.51f,  6.18f,  5.58f
	};
	/* Executable statements */
	printf("mv2dgeevs test\n");

	/* Solve eigenproblem */
	info = lbdgeevs_cpu(false,  n, a, lda, wr, wi, vl, ldvl, vr, ldvr);

	/* Check for convergence */
	if (info > 0) {
		printf("The algorithm failed to compute eigenvalues.\n");
		exit(1);
	}

	assert(0 == info);
	assert(2.86 == round_up(wr[0], 2));
	assert(2.86 == round_up(wr[1], 2));
	assert(-0.69 == round_up(wr[2], 2));
	assert(-0.69 == round_up(wr[3], 2));
	assert(-10.46 == round_up(wr[4], 2));

	/* Print eigenvalues */
	print_eigenvalues((char*)"Eigenvalues", n, wr, wi);

	//For left and righ eigen vector you have to provide the first two argument with MagmaVec
	///* Print left eigenvectors */
	//print_eigenvectors((char*)"Left eigenvectors", n, wi, vl, ldvl);
	///* Print right eigenvectors */
	//print_eigenvectors((char*)"Right eigenvectors", n, wi, vr, ldvr);
}
void mv2dgeev_cpu_test()
{
	/* Locals */
	const int N = 5;//number of rows
	const int LDA = N;//Leading dimension for A
	const int LDVL = N;//Leading dimensions for VL
	const int LDVR = N;//Leading dimensions for VR

	/* Locals */
	int n = N, lda = LDA, ldvl = LDVL, ldvr = LDVR, info;
	lbvector jjobvl = lbvector::NoVec;
	lbvector jjobvr = lbvector::NoVec;

	/* Local arrays */
	double wr[N], wi[N], vl[LDVL * N], vr[LDVR * N];
	double a[LDA * N] = {
		 -1.01f,  3.98f,  3.30f,  4.43f,  7.31f,
			0.86f,  0.53f,  8.26f,  4.96f, -6.43f,
		   -4.60f, -7.04f, -3.89f, -7.66f, -6.16f,
			3.31f,  5.29f,  8.20f, -7.33f,  2.47f,
		   -4.81f,  3.55f, -1.51f,  6.18f,  5.58f
	};
	/* Executable statements */
	printf("mv2dgeev_cpu test\n");

	/* Solve eigenproblem */
	info = lbdgeev_cpu(false, 'N', 'N',n, a, lda, wr, wi, vl, ldvl, vr, ldvr);

	/* Check for convergence */
	if (info > 0) {
		printf("The algorithm failed to compute eigenvalues.\n");
		exit(1);
	}

	assert(0 == info);
	assert(2.86 == round_up(wr[0], 2));
	assert(2.86 == round_up(wr[1], 2));
	assert(-0.69 == round_up(wr[2], 2));
	assert(-0.69 == round_up(wr[3], 2));
	assert(-10.46 == round_up(wr[4], 2));

	/* Print eigenvalues */
	print_eigenvalues((char*)"Eigenvalues", n, wr, wi);

	//For left and righ eigen vector you have to provide the first two argument with MagmaVec
	///* Print left eigenvectors */
	//print_eigenvectors((char*)"Left eigenvectors", n, wi, vl, ldvl);
	///* Print right eigenvectors */
	//print_eigenvectors((char*)"Right eigenvectors", n, wi, vr, ldvr);
}



void testSGEEV_float()
{

	/* Locals */
	const int N = 5;//number of rows
	const int LDA = N;//Leadning dimension for A
	const int LDVL = N;//Leading dimensions for VL
	const int LDVR = N;//Leading dimensions for VR

	/* Locals */
	int n = N, lda = LDA, ldvl = LDVL, ldvr = LDVR, info;
	lbvector jjobvl = lbvector::NoVec;
	lbvector jjobvr = lbvector::NoVec;

	/* Local arrays */
	float wr[N], wi[N], vl[LDVL * N], vr[LDVR * N];
	float a[LDA * N] = {
		 -1.01f,  3.98f,  3.30f,  4.43f,  7.31f,
			0.86f,  0.53f,  8.26f,  4.96f, -6.43f,
		   -4.60f, -7.04f, -3.89f, -7.66f, -6.16f,
			3.31f,  5.29f,  8.20f, -7.33f,  2.47f,
		   -4.81f,  3.55f, -1.51f,  6.18f,  5.58f
	};
	/* Executable statements */
	printf(" SGEEV Example Program Results\n");

	/* Solve eigenproblem */
	info = lbsgeev_cpu(false, 'N', 'N', n, a, lda, wr, wi, vl, ldvl, vr, ldvr);

	/* Check for convergence */
	if (info > 0) {
		printf("The algorithm failed to compute eigenvalues.\n");
		exit(1);
	}

	/* Print eigenvalues */
	print_eigenvalues((char*)"Eigenvalues", n, wr, wi);

	//For left and righ eigen vector you have to provide the first two argument with MagmaVec
	///* Print left eigenvectors */
	//print_eigenvectors((char*)"Left eigenvectors", n, wi, vl, ldvl);
	///* Print right eigenvectors */
	//print_eigenvectors((char*)"Right eigenvectors", n, wi, vr, ldvr);
}
