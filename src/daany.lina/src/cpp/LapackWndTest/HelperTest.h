#pragma once
#include <iostream>



//#define min(a,b) (((a)<(b ))?( a):(b))
//#define max(a,b) (((a)<(b ))?( b):(a))

void print_matrix(char* desc, int m, int n, double* a, const int lda, bool colMajor = true);
void print_matrix(char* desc, int m, int n, float* a, const int lda, bool colMajor = true);

void print_eigenvalues(char* desc, int n, float* wr, float* wi);
void print_eigenvalues(char* desc, int n, double* wr, double* wi);

void print_eigenvectors(char* desc, int n, float* wi, float* v, int ldv);
void print_eigenvectors(char* desc, int n, double* wi, double* v, int ldv);

void print_vector_norm(char* desc, int m, int n, float* a, int lda);
void print_vector_norm(char* desc, int m, int n, double* a, int lda);

void print_int_vector(char* desc, int n, int* a);
double round_up(double value, int decimal_places);
float round_up(float value, int decimal_places);

