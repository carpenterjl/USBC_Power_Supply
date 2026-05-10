/* usb_stdio.h ------------------------------------------------------------*/
#ifndef USB_STDIO_H
#define USB_STDIO_H

#include <stdint.h>

void USB_STDIO_Init(void);            /* call once, after USB stack is up   */
int	 USB_STDIO_RxHandler(uint8_t *buf, uint32_t len);
int  USB_Available(void);             /* #bytes waiting in RX buffer        */
int  USB_GetChar(void);               /* non blocking getchar() replacement */
void USB_PutChar(uint8_t c);          /* low level putchar() if you need it */

#endif
