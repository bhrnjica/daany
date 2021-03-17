#include <iostream>
// includes, system
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <math.h>
#include "HelperTest.h"

#include "GSVTests.h"
#include "SVDTests.h"
#include "LSSTests.h"
#include "EIGENTests.h"
#include "MatrixTest.h"


int main(int argc, char** argv)
{


	mv2sgemm_test_lapack_col_01();
	mv2sgemm_test_lapack_row_01();
	mv2sgemm_test_lapack_row();
	mv2sgemm_test_lapack_col();


	   
	mv2dgemm_test_lapack_col_01();
	mv2dgemm_test_lapack_row_01();
	mv2dgemm_test_lapack_row();
	mv2dgemm_test_lapack_col();



	
	//EIGEN
	mv2sgeevs_cpu_test();

	mv2sgeev_cpu_test();



	mv2dgeevs_test();
	mv2dgeev_cpu_test();


	//LSS
	mv2sgels_cpu_test();


	mv2dgels_cpu_test();

	
	//GSV tests
	mv2dgesv_cpu_test();

	mv2sgesv_cpu_test();


	//SVD




	return 0;

}
