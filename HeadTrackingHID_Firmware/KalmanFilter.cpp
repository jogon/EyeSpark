/*
 * KalmanFilter.cpp
 *
 * Created: 21/12/2011 16:43:10
 *  Author: Loïc Kaemmerlen
 */ 

#include <avr/io.h>  
#include <math.h>
#include <stdlib.h>
#include <string.h>
#include <util/delay.h>	

#include "KalmanFilter.h"
#include "sensors.h"	
#include "sensors.cpp"
#include "mathstools.h"	
#include "mathstools.cpp"	




//////////////////// ACCELEROMETER PART //////////////////////

// Converts given acceleration raw data in g
void accel_g (vector *a)
{
	float res = 2.0;  //the resolution defined for the accelerometer, +-'RES' g
	
	// Calculates the acceleration
	a->x = ((a->x )/32767.0)*res;
	a->y = ((a->y )/32767.0)*res;
	a->z = ((a->z )/32767.0)*res;
	
}

// Computes angles from scaled and shifted acceleration data
vector accel_angle (vector a)
{
	vector angles = {0.0,0.0,0.0};
	
	// Calculates Force Vector value:
	double R = sqrt((a.x*a.x) + (a.y*a.y) + (a.z*a.z));
	
	// Computes inclinations based on the accelerometer values, in degrees
	if (a.z<0)
	{
		angles.x = -acos(a.x / R)*57.29;
		angles.y = -acos(a.y / R)*57.29;
	}
	else
	{
		angles.x = acos(a.x / R)*57.29;
		angles.y = acos(a.y / R)*57.29;		
	}

	angles.z = acos(a.z / R)*57.29;
	
	
	// Shifts the angles so as to be 0 on horizontal position
	
	angles.x -= 90.0;
	if (angles.x <-180.0)
		{angles.x +=360.0;}
	angles.y -= 90.0;
	if (angles.y <-180.0)
		{angles.y +=360.0;}
	

	
	return angles ;
	
	
}


// Whole function to get angles from accelerometer.
vector accel_angle_acquisition(void)
{
	vector a;
	vector m;
	vector angles;
		
	compass_read_data(&a, &m);
	accel_g(&a);	
	
	angles = accel_angle(a);

// Ensures that Z gets negative if pointing downward	
	if (angles.x < 0)
	{
		angles.z = - angles.z;
	}
	return angles;
}

// Whole function to get angles from accelerometer + compass
vector accelcompass_angle_acquisition(void)
{
	vector a;
	vector m;
	
	// Vector p should be defined as pointing forward, parallel to the ground, with coordinates {X, Y, Z}.
	vector p = {0.0, -1.0, 0.0};
	vector angles;
	float heading;
		
	compass_read_data(&a, &m);
	heading = get_heading(&a,&m,&p);
	accel_g(&a);	
	
	angles = accel_angle(a);
	
	angles.z= heading;
	
	return angles;
}