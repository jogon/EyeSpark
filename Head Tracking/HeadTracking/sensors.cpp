/*
 * sensors.cpp
 *
 * Based on the FreeIMU libabry and Loïc Kaemmerlen KalmanFilter Library.
 * Here the different sensors are merged together and functions used to retrieve the data from the sensors are declared.
 */ 

#include <avr/io.h>  
#include <math.h>
#include <stdlib.h>
#include <string.h>
#include <util/delay.h>	
	
#include "mathstools.h"
#include "sensors.h"
#include "i2c.h"

#include "itg3205.h"
#include "adxl345.h"
#include "hmc5883.h"





// Calibration values using compass_calibration. This needs to be done!

vector m_min = {-492, -503, -549};
vector m_max = {579, 494, 322};


void compass_config(void)
{

//Compass configuration

	//enable accelerometer
	accelerometer_init();

	//enable magnetometer
	magnetometer_init();
}




// Returns a set of acceleration and raw magnetic readings from the compass.
void compass_read_data(vector *a, vector *m)
{

	unsigned char axl, axh, ayl, ayh, azl, azh, mxh, mxl, myh, myl, mzh, mzl;
	uint8_t dummy;
	// read accelerometer values
	

	//Read X values
	//0x32 data registers
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(0xA6);    //write to ADXL
	i2cWaitForComplete();
	i2cSendByte(0x32);    //X0 data register
	i2cWaitForComplete();
	
	i2cSendStop();		 //repeat start
	i2cSendStart();

	i2cWaitForComplete();
	i2cSendByte(0xA7);    //read from ADXL
	i2cWaitForComplete();
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	axl = i2cGetReceivedByte();	//x low byte
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	dummy = i2cGetReceivedByte();	//must do a multiple byte read?
	i2cWaitForComplete();
	i2cSendStop();	
	
	//0x33 data registers
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(0xA6);    //write to ADXL
	i2cWaitForComplete();
	i2cSendByte(0x33);    //X1 data register
	i2cWaitForComplete();
	
	i2cSendStop();		 //repeat start
	i2cSendStart();

	i2cWaitForComplete();
	i2cSendByte(0xA7);    //read from ADXL
	i2cWaitForComplete();
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	axh = i2cGetReceivedByte();	//x high byte
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	dummy = i2cGetReceivedByte();	//must do a multiple byte read?
	i2cWaitForComplete();
	i2cSendStop();
	
	
	//Read Y value
	//0x34 data registers
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(0xA6);    //write to ADXL
	i2cWaitForComplete();
	i2cSendByte(0x34);    //Y0 data register
	i2cWaitForComplete();
	
	i2cSendStop();		 //repeat start
	i2cSendStart();

	i2cWaitForComplete();
	i2cSendByte(0xA7);    //read from ADXL
	i2cWaitForComplete();
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	ayl = i2cGetReceivedByte();	//x low byte
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	dummy = i2cGetReceivedByte();	//must do a multiple byte read?
	i2cWaitForComplete();
	i2cSendStop();	
	
	//0x35 data registers
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(0xA6);    //write to ADXL
	i2cWaitForComplete();
	i2cSendByte(0x35);    //Y1 data register
	i2cWaitForComplete();
	
	i2cSendStop();		 //repeat start
	i2cSendStart();

	i2cWaitForComplete();
	i2cSendByte(0xA7);    //read from ADXL
	i2cWaitForComplete();
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	ayh = i2cGetReceivedByte();	//y high byte
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	dummy = i2cGetReceivedByte();	//must do a multiple byte read?
	i2cWaitForComplete();
	i2cSendStop();
	
	//Read Z Value
	//0x36 data registers
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(0xA6);    //write to ADXL
	i2cWaitForComplete();
	i2cSendByte(0x36);    //Z0 data register
	i2cWaitForComplete();
	
	i2cSendStop();		 //repeat start
	i2cSendStart();

	i2cWaitForComplete();
	i2cSendByte(0xA7);    //read from ADXL
	i2cWaitForComplete();
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	azl = i2cGetReceivedByte();	//z low byte
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	dummy = i2cGetReceivedByte();	//must do a multiple byte read?
	i2cWaitForComplete();
	i2cSendStop();	
	
	//0x37 data registers
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(0xA6);    //write to ADXL
	i2cWaitForComplete();
	i2cSendByte(0x37);    //Z1 data register
	i2cWaitForComplete();
	
	i2cSendStop();		 //repeat start
	i2cSendStart();

	i2cWaitForComplete();
	i2cSendByte(0xA7);    //read from ADXL
	i2cWaitForComplete();
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	azh = i2cGetReceivedByte();	//z high byte
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	dummy = i2cGetReceivedByte();	//must do a multiple byte read?
	i2cWaitForComplete();
	i2cSendStop();
	 
	//read magnetometer values
	 
	
	magnetometer_init();
	
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(0x3D);    //write to HMC
	i2cWaitForComplete();
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	mxh = i2cGetReceivedByte();	//x high byte
	i2cWaitForComplete();
	
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	mxl = i2cGetReceivedByte();	//x low byte
	i2cWaitForComplete();
	 
	
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	mzh = i2cGetReceivedByte();	
	i2cWaitForComplete();      //z high byte
	
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	mzl = i2cGetReceivedByte();	//z low byte
	i2cWaitForComplete();
	 
	
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	myh = i2cGetReceivedByte();	//y high byte
	i2cWaitForComplete();
	
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	myl = i2cGetReceivedByte();	//y low byte
	i2cWaitForComplete();
	 
	
	i2cSendByte(0x3D);         //must reach 0x09 to go back to 0x03
	i2cWaitForComplete();
	
	i2cSendStop();	
	
	a->x = axh << 8 | axl;
	a->y = ayh << 8 | ayl;
	a->z = azh << 8 | azl;
	m->x = mxh << 8 | mxl;
	m->y = myh << 8 | myl;
	m->z = mzh << 8 | mzl;
}




// This will print on the serial min and max values of the compass reading
void compass_calibration (void)
{
	vector a={0.0,0.0,0.0};
	vector m={0.0,0.0,0.0};
		
	vector mmin={0.0,0.0,0.0};
	vector mmax={0.0,0.0,0.0};

		
	while(1)
	{
		
		compass_read_data(&a,&m);
		
		
// Mmin handler		
		if(m.x  <  mmin.x)
		mmin.x  =  m.x;
		
		if(m.y  <  mmin.y)
		mmin.y  =  m.y;
		
		if(m.z  <  mmin.z)
		mmin.z  =  m.z;
		
// Mmax handler
		if(m.x  >  mmax.x)
		mmax.x  =  m.x;
		
		if(m.y  >  mmax.y)
		mmax.y  =  m.y;
		
		if(m.z  >  mmax.z)
		mmax.z  =  m.z;	
	
	}

}

// Returns a heading (in degrees) given an acceleration vector a due to gravity, a magnetic vector m, and a facing vector p.
float get_heading(vector *a, vector *m, vector *p)
{
	
	// shift and scale
	m->x = (m->x - m_min.x) / (m_max.x - m_min.x) * 2.0 - 1.0;
	m->y = (m->y - m_min.y) / (m_max.y - m_min.y) * 2.0 - 1.0;
	m->z = (m->z - m_min.z) / (m_max.z - m_min.z) * 2.0 - 1.0;
	
	
	vector E;
	vector N;

	// cross magnetic vector (magnetic north + inclination) with "down" (acceleration vector) to produce "east"
	vector_cross(m, a, &E);
	vector_normalize(&E);

	// cross "down" with "east" to produce "north" (parallel to the ground)
	vector_cross(a, &E, &N);
	vector_normalize(&N);

	// compute heading
	float heading = round(atan2(vector_dot(&E, p), vector_dot(&N, p)) * 180 / M_PI);
	
	if (heading > 180.0)
		heading -= 360.0;
	if (heading < -180.0)
		heading += 360.0;
	return heading;
}

void gyro_config(void)
{
 	 gyro_init();

}


void gyro_read_data(vector *g)
{
	// read gyroscope X values
	unsigned char  gxh, gxl, gyh, gyl, gzh, gzl;
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(ITG3205_W);	// write 
	i2cWaitForComplete();
	i2cSendByte(0x1D);	   // x high address
	i2cWaitForComplete();
	i2cSendStart();
	
	i2cSendByte(ITG3205_R);	// read
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	
	gxh = i2cGetReceivedByte();	// Get MSB result
	i2cWaitForComplete();
	i2cSendStop();
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(ITG3205_W);	// write
	i2cWaitForComplete();
	i2cSendByte(0x1E);	    // x low address
	i2cWaitForComplete();
	i2cSendStart();
	
	i2cSendByte(ITG3205_R);	// read
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	
	gxl = i2cGetReceivedByte();	// Get LSB result
	i2cWaitForComplete();
	i2cSendStop();
	
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
	
	//Read Y values
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(ITG3205_W);	// write
	i2cWaitForComplete();
	i2cSendByte(0x1F);	// y high address
	i2cWaitForComplete();
	i2cSendStart();
	
	i2cSendByte(ITG3205_R);	 // read
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	
	gyh = i2cGetReceivedByte();	// Get MSB result
	i2cWaitForComplete();
	i2cSendStop();
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(ITG3205_W);	// write
	i2cWaitForComplete();
	i2cSendByte(0x20);	// y low address
	i2cWaitForComplete();
	i2cSendStart();
	
	i2cSendByte(ITG3205_R);	// read
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	
	gyl = i2cGetReceivedByte();	// Get LSB result
	i2cWaitForComplete();
	i2cSendStop();
	
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
	//Read Z values
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(ITG3205_W);	// write
	i2cWaitForComplete();
	i2cSendByte(0x21);	// z high address
	i2cWaitForComplete();
	i2cSendStart();
	
	i2cSendByte(ITG3205_R);	// read
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	
	gzh = i2cGetReceivedByte();	// Get MSB result
	i2cWaitForComplete();
	i2cSendStop();
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(ITG3205_W);	// write
	i2cWaitForComplete();
	i2cSendByte(0x22);	// z low address
	i2cWaitForComplete();
	i2cSendStart();
	
	i2cSendByte(ITG3205_R);	// read
	i2cWaitForComplete();
	i2cReceiveByte(FALSE);
	i2cWaitForComplete();
	
	gzl = i2cGetReceivedByte();	// Get LSB result
	i2cWaitForComplete();
	i2cSendStop();
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
	g->x = gxh << 8 | gxl;
	g->y = gyh << 8 | gyl;
	g->z = gzh << 8 | gzl;
	
	g->x = g->x + 57.0;
	g->y = g->y + 13.0;
	g->z = g->z + 5.0;


}