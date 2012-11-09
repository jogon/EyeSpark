/*
   LUFA Library
   Copyright (C) Dean Camera, 2012.

   dean [at] fourwalledcubicle [dot] com
   www.lufa-lib.org
   */

/*
   Copyright 2012  Dean Camera (dean [at] fourwalledcubicle [dot] com)

   Permission to use, copy, modify, distribute, and sell this
   software and its documentation for any purpose is hereby granted
   without fee, provided that the above copyright notice appear in
   all copies and that both that the copyright notice and this
   permission notice and warranty disclaimer appear in supporting
   documentation, and that the name of the author not be used in
   advertising or publicity pertaining to distribution of the
   software without specific, written prior permission.

   The author disclaim all warranties with regard to this
   software, including all implied warranties of merchantability
   and fitness.  In no event shall the author be liable for any
   special, indirect or consequential damages or any damages
   whatsoever resulting from loss of use, data or profits, whether
   in an action of contract, negligence or other tortious action,
   arising out of or in connection with the use or performance of
   this software.
   */

/** \file
 *
 *  Main source file for the VirtualSerial demo. This file contains the main tasks of
 *  the demo and is responsible for the initial application hardware configuration.
 *
 *  Modified by Sparkfun 5/31/12 by Jordan McConnell
 *  The purpose of this to be a simple demo of how to send serial information
 *  between your 32u4 Breakout Board and computer using USB as a virtual COM port.
 *
 *  The first time this is run, Windows will want a driver, point it to the
 *  file: 'LUFA VirtualSerial.inf' in this directory.  Now your board will have
 *  two COM ports associated with your 32u4 board, one for the bootloader, and
 *  one for your applications that involve serial communication to the PC.
 *
 *  Two examples are included.  By default, this program once loaded on the
 *  board will accept information from the computer via USB and the virtual COM
 *  port, put it into a buffer and then send it straight back to the user.
 *
 *  If the default example is commented out and the second is uncommented, a
 *  simple print statement to the computer happens about every couple seconds.
 */


#include <stdlib.h>
#include <stdio.h>
#include <avr/io.h>
#include <avr/pgmspace.h>
#include <util/delay.h>
#include "types.h"
#include "defs.h"
//#include "i2c.h"
#include "VirtualSerial.h"

 
#include <avr/interrupt.h>
#include <inttypes.h>


 
 
#include "KalmanFilter.h"	
#include "KalmanFilter.cpp "// Include the Filter Library
 

  
#define sbi(var, mask)   ((var) |= (uint8_t)(1 << mask))
#define cbi(var, mask)   ((var) &= (uint8_t)~(1 << mask))

 

void init_all(void);

void initialize(void);			// Initialization routines
void init_interrupts(void);		// Timer1 interrupts initialization
void filter(void);	
 
 

/** LUFA CDC Class driver interface configuration and state information. This structure is
 *  passed to all CDC Class driver functions, so that multiple instances of the same class
 *  within a device can be differentiated from one another.
 */ 
 

static FILE USBSerialStream;
volatile char buffer[CDC_TXRX_EPSIZE];
 
USB_ClassInfo_CDC_Device_t VirtualSerial_CDC_Interface = { { 0, CDC_TX_EPNUM, CDC_TXRX_EPSIZE, false, CDC_RX_EPNUM, CDC_TXRX_EPSIZE, false, 
CDC_NOTIFICATION_EPNUM, CDC_NOTIFICATION_EPSIZE, false, }, }; 

vector anglesGyroGlobal;
unsigned int tcnt1=0b1000100010111000;	


// Globally saved angle states
vector anglesAccel = {0.0,0.0,0.0};
vector anglesGyro  = {0.0,0.0,0.0};			
vector anglesOutput = {0.0,0.0,0.0};
vector initialAngles = {0.0, 0.0, 0.0};	
	


int main(void) {

	//Kalman stuff

	init_interrupts();
	anglesGyroGlobal.x=0.0;
	anglesGyroGlobal.y=0.0;
	anglesGyroGlobal.z=0.0;
	
	_delay_ms(100);	
	 
	 	
	// Count keeps track of # of characters in the buffer for the first example
	// It's used to keep track of milliseconds passed in the second example.
	uint16_t count = 0;

	SetupHardware();
	init_all();
	_delay_ms(100);
	
	initialAngles = accelcompass_angle_acquisition();
	initialAngles.x = -initialAngles.x;

	// Comment to deactivate interrupts, uncomment to work with timer1 interrupts
	 sei();

	/* Create a regular character stream for the interface so that it can be used with the stdio.h functions */
  	CDC_Device_CreateStream(&VirtualSerial_CDC_Interface, &USBSerialStream);

	LEDs_SetAllLEDs(LEDMASK_USB_NOTREADY);
	sei();

	while(1) {       
        count++;
        if(count >= 2000)
        {
            count=0;

		//	print_adxl345(USBSerialStream);
		//	print_itg3205(USBSerialStream);
		//	print_hmc5883(USBSerialStream);
         //   _delay_ms(1000);
        }

        // LUFA USB functions necessary for USB/virtual serial communication to work
	    CDC_Device_USBTask(&VirtualSerial_CDC_Interface);
	    USB_USBTask();
    }
}
/** Configures the board hardware and chip peripherals for the demo's functionality. */
void SetupHardware(void) {

	/* Disable watchdog if enabled by bootloader/fuses */
	MCUSR &= ~(1 << WDRF);
	wdt_disable();

	/* Disable clock division */
	clock_prescale_set(clock_div_1);

	// Initialize char buffer to all zeroes
	for(uint8_t i=0; i < CDC_TXRX_EPSIZE; i++)
		buffer[i]=0;
	
	/* Hardware Initialization */
	LEDs_Init();
	USB_Init();
}
/** Event handler for the library USB Connection event. */
void EVENT_USB_Device_Connect(void) {
	
	LEDs_SetAllLEDs(LEDMASK_USB_ENUMERATING);
}
/** Event handler for the library USB Disconnection event. */
void EVENT_USB_Device_Disconnect(void) {
	
	LEDs_SetAllLEDs(LEDMASK_USB_NOTREADY);
}
/** Event handler for the library USB Configuration Changed event. */
void EVENT_USB_Device_ConfigurationChanged(void) {
	
	bool ConfigSuccess = true;
	ConfigSuccess &= CDC_Device_ConfigureEndpoints(&VirtualSerial_CDC_Interface);
	LEDs_SetAllLEDs(ConfigSuccess ? LEDMASK_USB_READY : LEDMASK_USB_ERROR);
}
/** Event handler for the library USB Control Request reception event. */
void EVENT_USB_Device_ControlRequest(void) {
	
	CDC_Device_ProcessControlRequest(&VirtualSerial_CDC_Interface);
}

void filter(void)
{		
	
	// Get new sensor values
	anglesAccel = accelcompass_angle_acquisition();
 	anglesGyro = gyro_angle_acquisition();
	
	
	anglesOutput.x += anglesGyro.x; //{ xk(0,0), xk(1,1), xk(2,2) };
	anglesOutput.y += anglesGyro.y;
	anglesOutput.z += anglesGyro.z;
	
	anglesAccel.x = -anglesAccel.x;
	
	/*
	fprintf(&USBSerialStream,"Angle in X = %d \n",(int) anglesAccel.z);
	fprintf(&USBSerialStream,"Angle in Y = %d \n",(int) anglesAccel.x);
	fprintf(&USBSerialStream,"Angle in Z = %d \n",(int) anglesAccel.y); 
	*/
	 	
	if( (anglesAccel.z-initialAngles.z) <= -15)
		fprintf(&USBSerialStream,"Movement Left\n");
		
	else if( (anglesAccel.z-initialAngles.z) >= 15)
		fprintf(&USBSerialStream,"Movement Right\n");
		
	else if((anglesAccel.y-initialAngles.y) >= 15 )
		fprintf(&USBSerialStream,"Movement Down\n");
		
	else if((anglesAccel.y-initialAngles.y) <= -15 && (anglesAccel.y-initialAngles.y) >= -100 )
		fprintf(&USBSerialStream,"Movement Up\n"); 
		
	else if((anglesAccel.x-initialAngles.x) >= 20)
	fprintf(&USBSerialStream,"Tilt Left\n"); 
		
	else if((anglesAccel.x-initialAngles.x) <= -20 )
		fprintf(&USBSerialStream,"Tilt Right\n"); 
		 

}		

// Timer1 Interrupt handler
ISR(TIMER1_OVF_vect) 
{		 
	TCNT1 = tcnt1;		// Reload the timer value
	sei();				// Re-enable the timer as fast as possible
	filter();
 
}  

void init_all(void) {

    DDRD  = 0b00000001; //PD0 (SCL) is set as an output, PD1 (SDA) as an input.
	PORTD = 0b00000011; //SCL is set as HIGH and SDA is set as pullup.
	i2cInit();
	compass_config();
	gyro_config();
}
 


void init_interrupts(void)
{
	  /* First disable the timer overflow interrupt while we're configuring */  
      TIMSK1 &= ~(1<<TOIE1);  
      
      /* Configure timer2 in normal mode (pure counting, no PWM etc.) */  
      TCCR1A &= ~((1<<WGM11) | (1<<WGM10));  
      TCCR1B &= ~((1<<WGM13) | (1<<WGM12));  
      
 
      /* Disable Compare Match A interrupt enable (only want overflow) */  
      TIMSK1 &= ~(1<<OCIE1A);  
      
      // Prescalar = 8.  Time between interrupts ~=32ms;
      TCCR1B &= ~(1<<CS12);			// Clear bit
	  TCCR1B |= (1<<CS11);          // Set bit 
	  TCCR1B &= ~(1<<CS10);			// Clear bit
      
      /* Finally load and enable the timer */  
      TCNT1 = tcnt1;  
      TIMSK1 |= (1<<TOIE1);  
}

