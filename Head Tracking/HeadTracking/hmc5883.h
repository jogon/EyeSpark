/**	These functions are used to extract the data from the HMC5883L magnetometer.
 *	
 * Based on the FreeIMU libabry and Loïc Kaemmerlen KalmanFilter Library.
 * 
 */ 
void magnetometer_values(void);
void magnetometer_init(void);
void print_hmc5883(FILE USBSerialStream);
void magnetometer_map(uint16_t * x, uint16_t * y, uint16_t * z);

uint16_t X_MAG, Y_MAG, Z_MAG;

void magnetometer_init(void) {	 

	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(0x3C);    //write to HMC
	i2cWaitForComplete();
	i2cSendByte(0x00);    //Configuration Register A
	i2cWaitForComplete();
	i2cSendByte(0x70);    //8 average, 15Hz, normal measurement 0x70
	i2cWaitForComplete();
	i2cSendStop();
	
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(0x3C);    //write to HMC
	i2cWaitForComplete();
	i2cSendByte(0x01);    //Configuration Register B
	i2cWaitForComplete();
	i2cSendByte(0xA0);    //gain = 5
	i2cWaitForComplete();
	i2cSendStop();
	 
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(0x3C);    //write to HMC
	i2cWaitForComplete();
	i2cSendByte(0x02);    //mode register
	i2cWaitForComplete();
	i2cSendByte(0x00);    //continuous measurement mode
	i2cWaitForComplete();
	i2cSendStop();
}
void print_hmc5883(FILE USBSerialStream) {
	
	magnetometer_values();
	fprintf(&USBSerialStream,"Magnetometer X axis = %4d \n", X_MAG);
	fprintf(&USBSerialStream,"Magnetometer Y axis = %4d \n", Y_MAG);
	fprintf(&USBSerialStream,"Magnetometer Z axis = %4d \n", Z_MAG);
	fprintf(&USBSerialStream,"\n\r");

	delay_ms(20);
}  
void magnetometer_values(void) {
	/*
		The magnetometer values must be read consecutively
		in order to move the magnetometer pointer. Therefore the x, y, and z
		outputs need to be kept in this function. To read the magnetometer 
		values, call the function magnetometer(), then global vars 
		X_MAGag, Y_MAGag, Z_MAGag.
	*/
	
	magnetometer_init();
	
	uint8_t xh, xl, yh, yl, zh, zl;
	
	//must read all six registers plus one to move the pointer back to 0x03
	
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(0x3D);    //write to HMC
	i2cWaitForComplete();
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	xh = i2cGetReceivedByte();	//x high byte
	i2cWaitForComplete();
	
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	xl = i2cGetReceivedByte();	//x low byte
	i2cWaitForComplete();
	X_MAG = xl|(xh << 8);
	
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	zh = i2cGetReceivedByte();	
	i2cWaitForComplete();      //z high byte
	
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	zl = i2cGetReceivedByte();	//z low byte
	i2cWaitForComplete();
	Z_MAG = zl|(zh << 8);
	
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	yh = i2cGetReceivedByte();	//y high byte
	i2cWaitForComplete();
	
	i2cReceiveByte(TRUE);
	i2cWaitForComplete();
	yl = i2cGetReceivedByte();	//y low byte
	i2cWaitForComplete();
	Y_MAG = yl|(yh << 8);
	
	i2cSendByte(0x3D);         //must reach 0x09 to go back to 0x03
	i2cWaitForComplete();
	
	i2cSendStop();	
}
