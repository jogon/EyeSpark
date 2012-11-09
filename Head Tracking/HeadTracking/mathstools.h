/*
 * matrixtools.h
 *
 * Based on the FreeIMU libabry and Loïc Kaemmerlen KalmanFilter Library.
 * 
 */ 


#ifndef MATRIXTOOLS_H_
#define MATRIXTOOLS_H_

#include <stdlib.h> 
#include <stdio.h>



typedef struct vector
{
	 float x, y, z;
	 
} vector;

extern void vector_cross(const vector *a, const vector *b, vector *out);
extern float vector_dot(const vector *a, const vector *b);
extern void vector_normalize(vector *a);




#endif /* MATRIXTOOLS_H_ */