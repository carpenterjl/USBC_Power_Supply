/*
 * HMI_Display.h
 *
 *  Created on: Nov 23, 2025
 *      Author: Jacob
 */

#ifndef HMI_DISPLAY_HMI_DISPLAY_H_
#define HMI_DISPLAY_HMI_DISPLAY_H_

extern UART_HandleTypeDef huart2;

typedef enum
{
	HMI_OK,
	HMI_ERROR
}HMI_STATUS;

typedef enum
{
	HMI_HOME_SCREEN,
	HMI_MANUAL_MODE_SCREEN,
	HMI_AUTO_MODE_SCREEN,
	HMI_SETTINGS_SCREEN,
	HMI_FILE_INFO_SCREEN,
}HMI_SCREEN;

typedef struct
{
	HMI_STATUS DISPLAY_STATUS;
	HMI_SCREEN DISPLAY_MODE;
}HMI_Info;

unsigned short CRC16(unsigned char *pMsg, unsigned short DataLen);

HMI_STATUS HMI_Write_Data(uint8_t *HMI_PACKET, uint8_t packet_length);
HMI_STATUS HMI_Read_Data(uint8_t *REC_PACKET, uint16_t VAR_ADDRESS, uint8_t READ_LENGTH);

void startReceivingData();
void Process_UART_Data(uint8_t length_received);
void Handle_Touchscreen_Packet(uint8_t *packet);

HMI_STATUS HMI_Update_Number_Label(float NUMBER, uint16_t ADDRESS);
HMI_STATUS HMI_Update_FileName(char *file_name);

#endif /* HMI_DISPLAY_HMI_DISPLAY_H_ */
