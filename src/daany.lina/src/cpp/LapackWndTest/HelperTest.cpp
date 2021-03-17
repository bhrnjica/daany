#include "HelperTest.h"


void print_eigenvalues(char* desc, int n, float* wr, float* wi) {
	int j;
	printf("\n %s\n", desc);
	for (j = 0; j < n; j++) {
		if (wi[j] == (float)0.0) {
			printf(" %6.2f", wr[j]);
		}
		else {
			printf(" (%6.2f,%6.2f)", wr[j], wi[j]);
		}
	}
	printf("\n");
}

void print_eigenvalues(char* desc, int n, double* wr, double* wi) {
	int j;
	printf("\n %s\n", desc);
	for (j = 0; j < n; j++) {
		if (wi[j] == (float)0.0) {
			printf(" %6.2f", wr[j]);
		}
		else {
			printf(" (%6.2f,%6.2f)", wr[j], wi[j]);
		}
	}
	printf("\n");
}

void print_eigenvectors(char* desc, int n, float* wi, float* v, int ldv) {
	int i, j;
	printf("\n %s\n", desc);
	for (i = 0; i < n; i++) {
		j = 0;
		while (j < n) {
			if (wi[j] == (float)0.0) {
				printf(" %6.2f", v[i + j * ldv]);
				j++;
			}
			else {
				printf(" (%6.2f,%6.2f)", v[i + j * ldv], v[i + (j + 1) * ldv]);
				printf(" (%6.2f,%6.2f)", v[i + j * ldv], -v[i + (j + 1) * ldv]);
				j += 2;
			}
		}
		printf("\n");
	}
}

void print_eigenvectors(char* desc, int n, double* wi, double* v, int ldv) {
	int i, j;
	printf("\n %s\n", desc);
	for (i = 0; i < n; i++) {
		j = 0;
		while (j < n) {
			if (wi[j] == (double)0.0) {
				printf(" %6.2f", v[i + j * ldv]);
				j++;
			}
			else {
				printf(" (%6.2f,%6.2f)", v[i + j * ldv], v[i + (j + 1) * ldv]);
				printf(" (%6.2f,%6.2f)", v[i + j * ldv], -v[i + (j + 1) * ldv]);
				j += 2;
			}
		}
		printf("\n");
	}
}



void print_vector_norm(char* desc, int m, int n, float* a, int lda) {
	int i, j;
	float norm;
	printf("\n %s\n", desc);
	for (j = 0; j < n; j++) {
		norm = 0.0;
		for (i = 0; i < m; i++) norm += a[i + j * lda] * a[i + j * lda];
		printf(" %6.2f", norm);
	}
	printf("\n");
}
void print_vector_norm(char* desc, int m, int n, double* a, int lda) {
	int i, j;
	double norm;
	printf("\n %s\n", desc);
	for (j = 0; j < n; j++) {
		norm = 0.0;
		for (i = 0; i < m; i++)
			norm += a[i + j * lda] * a[i + j * lda];

		printf(" %6.2f", norm);
	}
	printf("\n");
}


void print_int_vector(char* desc, int n, int* a) {
	int i;
	printf("\n %s\n", desc);
	for (i = 0; i < n; i++) {
		printf("%d\t", a[i]);
	}
	printf("\n");
}


void print_matrix(char* desc, int m, int n, double* a, const int lda, bool colMajor ) {
	int i, j;
	printf("\n %s\n", desc);
	for (i = 0; i < m; i++)
	{
		for (j = 0; j < n; j++)
			colMajor ? printf(" %6.2f", a[i + j * lda]) : printf(" %6.2f", a[i * lda + j]);

		printf("\n");
	}
}

void print_matrix(char* desc, int m, int n, float* a, const int lda, bool colMajor ) {
	int i, j;
	printf("\n %s\n", desc);
	for (i = 0; i < m; i++)
	{
		for (j = 0; j < n; j++)
			colMajor ? printf(" %6.2f", a[i + j * lda]): printf(" %6.2f", a[i*lda + j]);

		printf("\n");
	}
}

double round_up(double value, int decimal_places) {
	const double multiplier = std::pow(10.0, decimal_places);
	return std::floor(value * multiplier+0.5) / multiplier;
}

float round_up(float value, int decimal_places) {
	const float multiplier = std::pow(10.0f, decimal_places);
	return std::floor(value * multiplier+0.5f) / multiplier;
}
