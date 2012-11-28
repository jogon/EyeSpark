/*
 * KalmanFilter.h
 *
 * Created: 21/12/2011 16:42:42
 *  Author: Loïc Kaemmerlen
 */ 


#ifndef KALMANFILTER_H_
#define KALMANFILTER_H_

#include "mathstools.h"

// Converts given acceleration raw data in g
void accel_g (vector *a);
// Computes angles from scaled and shifted acceleration data
vector accel_angle (vector a);
// Whole function to get angles from accelerometer
vector accel_angle_acquisition(void);
// Whole function to get angles from accelerometer + compass
vector accelcompass_angle_acquisition(void);

#endif /* KALMANFILTER_H_ */