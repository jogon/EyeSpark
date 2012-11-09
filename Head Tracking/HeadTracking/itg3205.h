/*
 * Based on the FreeIMU libabry and Loïc Kaemmerlen KalmanFilter Library.
 * 
 */

#define ITG3205_R 0xD1	// ADD pin is pulled low
#define ITG3205_W 0xD0

void gyro_init(void);
void print_itg3205(FILE  USBSerialStream);

unsigned char x_gyro(void);
unsigned char y_gyro(void);
unsigned char z_gyro(void);

void gyro_init(void) {
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(ITG3205_W);	// write 0xB4
	i2cWaitForComplete();
	i2cSendByte(0x3E);	// write register address
	i2cWaitForComplete();
	i2cSendByte(0x80);
	i2cWaitForComplete();
	i2cSendStop();
	
	delay_ms(10);
	
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(ITG3205_W);	// write 0xB4
	i2cWaitForComplete();
	i2cSendByte(0x16);	// write register address
	i2cWaitForComplete();
	i2cSendByte(0x18);  // DLPF_CFG = 0, FS_SEL = 3
	i2cWaitForComplete();
	i2cSendStop();	
	
	delay_ms(10);
	
	i2cSendStart();
	i2cWaitForComplete();
	i2cSendByte(ITG3205_W);	// write 0xB4
	i2cWaitForComplete();
	i2cSendByte(0x3E);	// write register address
	i2cWaitForComplete();
	i2cSendByte(0x00);
	i2cWaitForComplete();
	i2cSendStop();
}

void print_itg3205(FILE USBSerialStream) {
	
	fprintf(&USBSerialStream,"Gyroscope X axis = %4d \n", x_gyro());
	fprintf(&USBSerialStream,"Gyroscope Y axis = %4d \n", y_gyro());
	fprintf(&USBSerialStream,"Gyroscope Z axis = %4d \n", z_gyro());
	fprintf(&USBSerialStream,"\n\r");

	delay_ms(20);
}
unsigned char x_gyro(void) {

	unsigned char xh, xl, data;
	
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
	
	xh = i2cGetReceivedByte();	// Get MSB result
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
	
	xl = i2cGetReceivedByte();	// Get LSB result
	i2cWaitForComplete();
	i2cSendStop();
	
	data = xl|(xh << 8);
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
	return data;
}
unsigned char y_gyro(void) {

	unsigned char yh, yl, data;
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
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
	
	yh = i2cGetReceivedByte();	// Get MSB result
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
	
	yl = i2cGetReceivedByte();	// Get LSB result
	i2cWaitForComplete();
	i2cSendStop();
	
	data = yl|(yh << 8);
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
	return data;
}
unsigned char z_gyro(void) {
	unsigned char zh, zl, data;
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
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
	
	zh = i2cGetReceivedByte();	// Get MSB result
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
	
	zl = i2cGetReceivedByte();	// Get LSB result
	i2cWaitForComplete();
	i2cSendStop();
	
	data = zl|(zh << 8);
	
	cbi(TWCR, TWEN);	// Disable TWI
	sbi(TWCR, TWEN);	// Enable TWI
	
	return data;
}

