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

  The author disclaims all warranties with regard to this
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
 *  Main source file for the GenericHID demo. This file contains the main tasks of the demo and
 *  is responsible for the initial application hardware configuration.
 */

#include "GenericHID.h"
#include <util/delay.h>

vector anglesAccel = {0.0,0.0,0.0};
unsigned int tcnt1=0b1000100010111000;
int t = 0;

int main(void)
{
	init_interrupts();
	tempArray[0] = 1;
	
	SetupHardware();
	
	init_all();
	_delay_ms(100);
	 sei();
	 	
	GlobalInterruptEnable();

	for (;;)
	{

	}
}

/** Configures the board hardware and chip peripherals for the demo's functionality. */
void SetupHardware(void)
{
	/* Disable watchdog if enabled by bootloader/fuses */
	MCUSR &= ~(1 << WDRF);
	wdt_disable();

	/* Disable clock division */
	clock_prescale_set(clock_div_1);

	/* Hardware Initialization */
	USB_Init();
}

/** Event handler for the USB_Connect event. This indicates that the device is enumerating via the status LEDs and
 *  starts the library USB task to begin the enumeration and USB management process.
 */
void EVENT_USB_Device_Connect(void)
{
	/* Indicate USB enumerating */
	LEDs_SetAllLEDs(LEDMASK_USB_ENUMERATING);
}

/** Event handler for the USB_Disconnect event. This indicates that the device is no longer connected to a host via
 *  the status LEDs and stops the USB management task.
 */
void EVENT_USB_Device_Disconnect(void)
{
	/* Indicate USB not ready */
	LEDs_SetAllLEDs(LEDMASK_USB_NOTREADY);
}

/** Event handler for the USB_ConfigurationChanged event. This is fired when the host sets the current configuration
 *  of the USB device after enumeration, and configures the generic HID device endpoints.
 */
void EVENT_USB_Device_ConfigurationChanged(void)
{
	bool ConfigSuccess = true;

	/* Setup HID Report Endpoints */
	ConfigSuccess &= Endpoint_ConfigureEndpoint(GENERIC_IN_EPADDR, EP_TYPE_INTERRUPT, GENERIC_EPSIZE, 1);
	ConfigSuccess &= Endpoint_ConfigureEndpoint(GENERIC_OUT_EPADDR, EP_TYPE_INTERRUPT, GENERIC_EPSIZE, 1);

	/* Indicate endpoint configuration success or failure */
	LEDs_SetAllLEDs(ConfigSuccess ? LEDMASK_USB_READY : LEDMASK_USB_ERROR);
}

/** Event handler for the USB_ControlRequest event. This is used to catch and process control requests sent to
 *  the device from the USB host before passing along unhandled control requests to the library for processing
 *  internally.
 */
void EVENT_USB_Device_ControlRequest(void)
{
	/* Handle HID Class specific requests */
	switch (USB_ControlRequest.bRequest)
	{
		case HID_REQ_GetReport:
			if (USB_ControlRequest.bmRequestType == (REQDIR_DEVICETOHOST | REQTYPE_CLASS | REQREC_INTERFACE))
			{
				uint8_t GenericData[GENERIC_REPORT_SIZE];
				CreateGenericHIDReport(GenericData);

				Endpoint_ClearSETUP();

				/* Write the report data to the control endpoint */
				Endpoint_Write_Control_Stream_LE(&GenericData, sizeof(GenericData));
				Endpoint_ClearOUT();
			}

			break;
		case HID_REQ_SetReport:
			if (USB_ControlRequest.bmRequestType == (REQDIR_HOSTTODEVICE | REQTYPE_CLASS | REQREC_INTERFACE))
			{
				uint8_t GenericData[GENERIC_REPORT_SIZE];

				Endpoint_ClearSETUP();

				/* Read the report data from the control endpoint */
				Endpoint_Read_Control_Stream_LE(&GenericData, sizeof(GenericData));
				Endpoint_ClearIN();

				ProcessGenericHIDReport(GenericData);
			}

			break;
	}
}

/** Function to process the last received report from the host.
 *
 *  \param[in] DataArray  Pointer to a buffer where the last received report has been stored
 */
void ProcessGenericHIDReport(uint8_t* DataArray)
{
	/*
		This is where you need to process reports sent from the host to the device. This
		function is called each time the host has sent a new report. DataArray is an array
		holding the report sent from the host.
	*/

	
	for(int i = 0; i < 8; i++){
		
		tempArray[i] = 128;
	}
}

/** Function to create the next report to send back to the host at the next reporting interval.
 *
 *  \param[out] DataArray  Pointer to a buffer where the next report data should be stored
 */
void CreateGenericHIDReport(uint8_t* DataArray)
{
	/*
		This is where you need to create reports to be sent to the host from the device. This
		function is called each time the host is ready to accept a new report. DataArray is
		an array to hold the report to the host.
	*/

	
	for(int i = 0; i < 2; i++){
		
		DataArray[i] = 0xFF & ((int)anglesAccel.x >> 8*i);
	}
	for(int i = 0; i < 2; i++){
		
		DataArray[i+2] = 0xFF & ((int)anglesAccel.y >> 8*i);	
	}
	for(int i = 0; i < 2; i++){
		
		DataArray[i+4] = 0xFF & ((int)anglesAccel.z >> 8*i);	
	}
	DataArray[6] = 0x00;
	DataArray[7] = 0x00;
	
	
}

void HID_Task(void)
{
	/* Device must be connected and configured for the task to run */
	if (USB_DeviceState != DEVICE_STATE_Configured)
	  return;
	
	Endpoint_SelectEndpoint(GENERIC_OUT_EPADDR);

	/* Check to see if a packet has been sent from the host */
	if (Endpoint_IsOUTReceived())
	{
		/* Check to see if the packet contains data */
		if (Endpoint_IsReadWriteAllowed())
		{
			/* Create a temporary buffer to hold the read in report from the host */
			uint8_t GenericData[GENERIC_REPORT_SIZE];

			/* Read Generic Report Data */
			Endpoint_Read_Stream_LE(&GenericData, sizeof(GenericData), NULL);

			/* Process Generic Report Data */
			ProcessGenericHIDReport(GenericData);
		}
		/* Finalize the stream transfer to send the last packet */
		Endpoint_ClearOUT();
	}

	Endpoint_SelectEndpoint(GENERIC_IN_EPADDR);

	/* Check to see if the host is ready to accept another packet */
	if (Endpoint_IsINReady())
	{
		/* Create a temporary buffer to hold the report to send to the host */
		uint8_t GenericData[GENERIC_REPORT_SIZE];

		/* Create Generic Report Data */
		CreateGenericHIDReport(GenericData);

		/* Write Generic Report Data */
		Endpoint_Write_Stream_LE(&GenericData, sizeof(GenericData), NULL);

		/* Finalize the stream transfer to send the last packet */
		Endpoint_ClearIN();
	}
}

// Timer1 Interrupt handler
ISR(TIMER1_OVF_vect) 
{		 
	t++;
	TCNT1 = tcnt1;		// Reload the timer value
	sei();				// Re-enable the timer as fast as possible
	if(t >= 10)
	{
		anglesAccel = accelcompass_angle_acquisition();
		HID_Task();
		USB_USBTask();
		t = 0;
    } 
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



