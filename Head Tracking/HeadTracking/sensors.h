/*
 * sensors.h
 *
 * Based on the FreeIMU libabry and Loïc Kaemmerlen KalmanFilter Library.
 * Here the different sensors are merged together and functions used to retrieve the data from the sensors are declared.
 */  

#include "mathstools.h"

#ifndef COMPASS_H_
#define COMPASS_H_


void compass_config(void);
void compass_read_data(vector *a, vector *m);
void compass_calibration (void);
float get_heading( vector *a, vector *m, vector *p);


void gyro_config(void);
void gyro_read_data(vector *g);


#endif /* COMPASS_H_ */