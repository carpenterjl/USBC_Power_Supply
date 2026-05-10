/* usb_stdio.c ------------------------------------------------------------*/
#include "usb_stdio.h"
#include "usbd_cdc_if.h"
#include <stdio.h>
#include <unistd.h>

/* ---------- simple circular RX buffer ---------- */
#define RX_BUF_SZ 2048
static uint8_t  rxBuf[RX_BUF_SZ];
static volatile uint16_t head, tail;

static inline uint16_t nextIdx(uint16_t i) { return (i + 1U) & (RX_BUF_SZ-1U); }

/* called from the Cube generated CDC_Receive_FS() ----------------------- */
static inline uint16_t buf_used(void)
{
    return (head - tail) & (RX_BUF_SZ - 1U);      // 0 … 255
}

static inline uint16_t buf_free(void)
{
    return (tail - head - 1U) & (RX_BUF_SZ - 1U); // 0 … 255-1
}

int USB_STDIO_RxHandler(uint8_t *buf, uint32_t len)
{
    if (len > buf_free())          // **strictly greater**
        return 0;                  // not enough room

    for (uint32_t i = 0; i < len; ++i) {
        rxBuf[head] = buf[i];
        head = nextIdx(head);      // or (head + 1U) & 255
    }
    return 1;
}


/* public helpers -------------------------------------------------------- */

void USB_STDIO_Init(void) { /* nothing yet, kept for symmetry */ }

int USB_Available(void)
{
    return buf_used();             // unchanged
}

int USB_GetChar(void)
{
    if (tail == head) return -1;
    uint8_t c = rxBuf[tail];
    tail = nextIdx(tail);
    return c;
}

void USB_PutChar(uint8_t c) { CDC_Transmit_FS(&c, 1U); }

/* -------- libc retargeting (newlib / nanolib) -------------------------- */
int _write(int fd, char *ptr, int len)        /* stdout / stderr */
{
    if (fd == STDOUT_FILENO || fd == STDERR_FILENO) {
        /* simple – wait while USB busy; you can add your own TX ring buf */
        while (CDC_Transmit_FS((uint8_t *)ptr, len) == USBD_BUSY) ;
        return len;
    }
    return  -1;
}

int _read(int fd, char *ptr, int len)         /* stdin */
{
    if (fd != STDIN_FILENO) return  -1;

    int i = 0;
    /* non blocking – return whatever is already in buffer */
    while (i < len && USB_Available())
    {
        int c = USB_GetChar();
        if (c < 0) break;
        ptr[i++] = (char)c;
    }
    return i;
}
