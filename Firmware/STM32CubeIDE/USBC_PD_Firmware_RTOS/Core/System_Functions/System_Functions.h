/*
 * System_Functions.h
 *
 *  Created on: Apr 27, 2026
 *      Author: Jacob
 */

#ifndef SYSTEM_FUNCTIONS_SYSTEM_FUNCTIONS_H_
#define SYSTEM_FUNCTIONS_SYSTEM_FUNCTIONS_H_
#include "stm32g4xx_hal.h"
#include "main.h"
#include "math.h"
#include "cmsis_os.h"
typedef enum
{
	V_System,
	V_USB,
	V_Positive,
	V_Negative,
	V_3v3,
	V_5v,
	V_2v5
}Voltage_Sources;

typedef enum
{
	I_Positive,
	I_Negative,
	I_3v3,
	I_2v5
}Current_Sources;

typedef struct
{
	Voltage_Sources V_Source;
	uint32_t ADC_Channel;
}V_ADC;

typedef struct
{
	Current_Sources I_Source;
	uint32_t ADC_Channel;
}I_ADC;

void SetPositiveSupply(float set_voltage, float max_current);
void SetNegativeSupply(float set_voltage, float max_current);
void DisableAllOutputs(void);
void EnableOutput(Voltage_Sources V_Source);
void DisableOutput(Voltage_Sources V_Source);
float ReadVoltage(Voltage_Sources V_Source);
float ReadCurrent(Current_Sources I_Source);
float ReadPower(Voltage_Sources V_Source, Current_Sources I_Source);

uint32_t Read_ADC_Channel(ADC_HandleTypeDef *hadc, uint32_t channel);
float Read_Calibrated_Voltage(uint16_t raw_adc_reading);

int Device_Manager(uint8_t Usb_Port);

typedef enum {
    SRC_USB,
    SRC_UART,
    SRC_INTERNAL
} MsgSource_t;

typedef struct {
    uint8_t commandID;
    uint16_t value;
    MsgSource_t requesterTask; // Optional: so the response task knows who to notify
} SerialMsg_t;

extern osMessageQueueId_t serialDataQueueHandle;
extern UART_HandleTypeDef huart2;
void SendResponse(SerialMsg_t *source_message, char *results);

typedef enum
{
	USB_PD_SET 	= 	0x00,
	USB_PD_GET 	= 	0x01,
	VPOS_SET	=	0x02,
	VPOS_GET	=	0x03,
	VNEG_SET	=	0x04,
	VNEG_GET	=	0x05,
	V3V3_GET	=	0x06,
	V2V5_GET	=	0x07,
	IPOS_GET	=	0x08,
	INEG_GET	=	0x09,
	I3V3_GET	=	0x0A,
	I2V5_GET	=	0x0B,
	VUSB_GET	=	0x0C,
	VSYS_GET	=	0x0D,
	V5V_GET		=	0x0E,
	VMCU_GET	=	0x0F,
	VP_ENABLE	=	0x10,
	VN_ENABLE	=	0x11,
	V3_ENABLE	=	0x12,
	V2_ENABLE	=	0x13,
	VP_DISABLE	=	0x14,
	VN_DISABLE	=	0x15,
	V3_DISABLE	=	0x16,
	V2_DISABLE	=	0x17,
	ALL_ENABLE	=	0x18,
	ALL_DISABLE	=	0x19,
}SYSTEM_COMMANDS_ENUM;

#define LED_PERIOD_HALF (500/portTICK_PERIOD_MS)
#define USB_RESPONSE_PERIOD (5/portTICK_PERIOD_MS)

#define struct_size(a) (sizeof(a)/sizeof(a[0]))
#endif /* SYSTEM_FUNCTIONS_SYSTEM_FUNCTIONS_H_ */
