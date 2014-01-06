#include <avr/io.h>
#include <avr/interrupt.h>
#include <avr/wdt.h>

#include "usbdrv.h"

#define F_CPU 12000000L
#include <util/delay.h>

#define USB_PB0_OFF 0
#define USB_PB0_ON  1

// this gets called when custom control message is received
USB_PUBLIC uchar usbFunctionSetup(uchar data[8])
{
    usbRequest_t *rq = (void *)data; // cast data to correct type
        
    switch(rq->bRequest) { // custom command is in the bRequest field
    case USB_PB0_ON:
        PORTB |= 1; // PB0 on
        return 0;
    case USB_PB0_OFF: 
        PORTB &= ~1; // PB0 off
        return 0;
    }

    return 0; // should not get here
}

int main()
{
	uchar i;
	DDRB |= 1; // PB0 as output
	DDRB |= (1<<1); // PB1 as output

    wdt_enable(WDTO_1S); // enable 1s watchdog timer

    usbInit();
        
    usbDeviceDisconnect(); // enforce re-enumeration
    for(i = 0; i<250; i++) // wait 500 ms
	{ 
        wdt_reset(); // keep the watchdog happy
        _delay_ms(2);
    }
    usbDeviceConnect();
        
    sei(); // Enable interrupts after re-enumeration
     
	PORTB |= (1<<1); // PB1 on
	 
    while(1)
	{
		
        wdt_reset(); // keep the watchdog happy
        usbPoll();
    }
        
    return 0;
}