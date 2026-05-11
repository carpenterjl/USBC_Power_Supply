#include "usb_cmd.h"
#include "usb_stdio.h"
#include "usbd_cdc_if.h"
#include <string.h>
#include <stdio.h>
#include <ctype.h>
#include <stdlib.h>
#include "USB_PD_core.h"


#define CMD_LINE_MAX 512

static char cmd[CMD_LINE_MAX];
static uint8_t cmd_idx = 0;
static uint8_t ERROR_CODE = 0;

static const USB_COMMANDS CMD_LIST[] =
{
	{ "HELP", 	"",       	"Show this help message",    cmd_help },
    { "SPI",  	"<data>", 	"Send data over SPI",        cmd_spi_transfer},
	{ "ID?", 	"", 		"Sends ID of device as response", cmd_id},
	{ "ERR?", 	"", 		"Responds with error status, clears led", cmd_error},
	{ "VSET",   "P/N",		"Set supply voltage for P or N side", set_supply_voltage},
	{ "VGET",   "P/N/3/2",	"Get supply voltage", get_supply_voltage},
	{ "VEN",    "P/N/3/2",	"Enable Supply", enable_supply},
	{ "VDIS",   "P/N/3/2",	"Disable Supply", disable_supply},
	{ "VMEAS",  "P/N/3/2",	"Data Logging Mode", measurement_mode},
	{ "USBPD",	"<Voltage (float)>:<Current (float)>", "Request V,I from USBC-PD Host", request_usbc_voltage},
};

#define CMDS_LEN struct_size(CMD_LIST)

void USB_Cmd_Init(void)
{
    cmd_idx = 0;
    memset(cmd, '\0', CMD_LINE_MAX);
}

static void handle_command(char *line)
{
	char *cmd = strtok(line, ":");
	char *args = strtok(NULL, "");
	if(cmd != 0)
	{
		for (size_t i = 0; i < CMDS_LEN; ++i)
		{
			if (strcasecmp(cmd, CMD_LIST[i].cmd) == 0)
			{
				if (CMD_LIST[i].handler)
				{
					CMD_LIST[i].handler(args);
				}
				return;
			}
		}
	}

	//Bytes received but no valid command detected before end of list
	ERROR_CODE |= ERROR_ARGS;
//	HAL_GPIO_WritePin(LED_0_GPIO_Port, LED_0_Pin, 0);
	return;
}

void USB_CmdTask(void)
{
    while (USB_Available())
    {
        int ch = USB_GetChar();
        if (ch < 0) return;

        if (ch == '\r' || ch == '\n')
        {
            if (cmd_idx > 0)
            {
                cmd[cmd_idx] = '\0';
                handle_command(cmd);
                cmd_idx = 0;
            }
        } else if (cmd_idx < CMD_LINE_MAX - 1)
        {
            cmd[cmd_idx++] = (char)ch;
        }
    }
}

void cmd_help(char *args)
{
	printf("Available commands:\r\n");
	for (int j = 0; j < CMDS_LEN; j++)
	{
		printf("  %-8s %-12s %s\r\n",
			CMD_LIST[j].cmd,
			CMD_LIST[j].args,
			CMD_LIST[j].description);
	}
	return;
}

void cmd_id(char *args)
{
	printf("JPOWER\n");
	return;
}

void cmd_error(char *args)
{
	printf("ERROR CODE: %d\n", ERROR_CODE);
	ERROR_CODE = 0;
	return;
}

void cmd_spi_transfer(char *args)
{
	char *len = strtok(args, ":");
	uint8_t dataLength = 0;
	if(len)
	{
		dataLength = strtoul(len, NULL, 16);
	}
	uint8_t transfer_packet[dataLength];
	uint8_t received_packet[dataLength];
	if(dataLength == 0)
	{
		ERROR_CODE |= ERROR_ARGS;
		goto ERRORSPI;
	}

	uint8_t tempData[3] = {0};
	char *data = strtok(NULL, ":");
	if(!data)
	{
		ERROR_CODE |= ERROR_ARGS;
		goto ERRORSPI;
	}

	for(uint8_t idx = 0; idx < dataLength; idx++)
	{
		tempData[0] = data[2*idx];
		tempData[1] = data[2*idx + 1];
		tempData[3] = '\0';
		transfer_packet[idx] = strtoul((char *)tempData, NULL, 16);
	}

//	if(HAL_SPI_TransmitReceive(&hspi1, transfer_packet, received_packet, dataLength, 100) != HAL_OK)
//	{
//		ERROR_CODE |= ERROR_SPI;
//		goto ERRORSPI;
//	}

	CDC_Transmit_FS(received_packet, dataLength);
	return;

	ERRORSPI:
//	HAL_GPIO_WritePin(LED_0_GPIO_Port, LED_0_Pin, 0);
	printf("SPI XFER ERROR\n");
	return;
}

void set_supply_voltage(char *args)
{
	char *supply = strtok(args, ":");
	if(!supply) return;
	char *voltage = strtok(NULL, ":");
	if(!voltage) return;
	float v_set = strtof(voltage, NULL);
	if(v_set < -20 || v_set > 20) goto ERROR_SET_VOLTAGE;

	SerialMsg_t message;
	message.requesterTask = SRC_USB;

	switch(supply[0])
	{
	case 'P':
			if(v_set < 0) goto ERROR_SET_VOLTAGE;
			message.commandID = VPOS_SET;
			message.value = (int16_t)v_set*1000;
//			SetPositiveSupply(v_set, 0);
		break;
	case 'N':
			if(v_set > 0) goto ERROR_SET_VOLTAGE;
			message.commandID = VNEG_SET;
			message.value = (int16_t)v_set*1000;
//			SetNegativeSupply(v_set, 0);
		break;
	default: goto ERRORSUPPLYSET;
	}
	osMessageQueuePut(serialDataQueueHandle, &message, 0, osWaitForever);
//	printf("OK\n");
	return;

	ERRORSUPPLYSET:
//	printf("Invalid Supply\n");
	return;

	ERROR_SET_VOLTAGE:
//	printf("Range Error\n");
	return;
}

void get_supply_voltage(char *args)
{
	char *supply = strtok(args, ":");
	if(!supply) return;

//	float v_supp = 0;

	SerialMsg_t message;
	message.requesterTask = SRC_USB;

	switch(supply[0])
	{
	case 'P':
//		v_supp = ReadVoltage(V_Positive);
		message.commandID = VPOS_GET;
		break;
	case 'N':
//		v_supp = ReadVoltage(V_Negative);
		message.commandID = VNEG_GET;
		break;
	case '3':
//		v_supp = ReadVoltage(V_3v3);
		message.commandID = V3V3_GET;
		break;
	case '2':
//		v_supp = ReadVoltage(V_2v5);
		message.commandID = V2V5_GET;
	break;
	default: goto ERRORSUPPLYSET;
	}
	osMessageQueuePut(serialDataQueueHandle, &message, 0, osWaitForever);
//	printf("%.3f\n", v_supp);
	return;

	ERRORSUPPLYSET:
//	printf("Invalid Supply\n");
	return;
}

void enable_supply(char *args)
{
	char *supply = strtok(args, ":");
	if(!supply) return;
	SerialMsg_t message;
	message.requesterTask = SRC_USB;

	switch(supply[0])
	{
	case 'P':
		message.commandID = VP_ENABLE;
//		EnableOutput(V_Positive);
		break;
	case 'N':
		message.commandID = VN_ENABLE;
//		EnableOutput(V_Negative);
		break;
	case '3':
		message.commandID = V3_ENABLE;
//		EnableOutput(V_3v3);
		break;
	case '2':
		message.commandID = V2_ENABLE;
//		EnableOutput(V_2v5);
		break;
	default: goto ERRORSUPPLYSET;
	}
	osMessageQueuePut(serialDataQueueHandle, &message, 0, osWaitForever);
	HAL_GPIO_WritePin(LED3_GPIO_Port, LED3_Pin, GPIO_PIN_RESET);
//	printf("OK\n");
	return;

	ERRORSUPPLYSET:
//	printf("Invalid Supply\n");
	return;
}

void disable_supply(char *args)
{
	char *supply = strtok(args, ":");
	if(!supply) return;
	SerialMsg_t message;
	message.requesterTask = SRC_USB;

	switch(supply[0])
	{
	case 'P':
		message.commandID = VP_DISABLE;
//		DisableOutput(V_Positive);
		break;
	case 'N':
		message.commandID = VN_DISABLE;
//		DisableOutput(V_Negative);
		break;
	case '3':
		message.commandID = V3_DISABLE;
//		DisableOutput(V_3v3);
		break;
	case '2':
		message.commandID = V2_DISABLE;
//		DisableOutput(V_2v5);
		break;
	default: goto ERRORSUPPLYSET;
	}
	osMessageQueuePut(serialDataQueueHandle, &message, 0, osWaitForever);
	HAL_GPIO_WritePin(LED3_GPIO_Port, LED3_Pin, GPIO_PIN_SET);
//	printf("OK\n");
	return;

	ERRORSUPPLYSET:
//	printf("Invalid Supply\n");
	return;
}
extern uint16_t *adc_dma_buf;
extern ADC_HandleTypeDef hadc1;

//extern uint8_t should_start_adc;
void measurement_mode(char *args)
{
//	char *supply = strtok(args, ":");
//	if(!supply) return;
//	switch(supply[0])
//	{
//	case 'P':
//		ReadVoltage(V_Positive);
//		break;
//	case 'N':
//		ReadVoltage(V_Negative);
//		break;
//	case '3':
//		ReadVoltage(V_3v3);
//		break;
//	case '2':
//		ReadVoltage(V_2v5);
//		break;
//	default: goto ERRORSUPPLYSET;
//	}

	//temp test
//	ReadVoltage(V_5v);
//	should_start_adc = !should_start_adc;
//	printf("OK\n");
	return;

	ERRORSUPPLYSET:
	printf("Invalid Supply\n");
	return;
}

void request_usbc_voltage(char *args)
{
	char *request = strtok(args, ":");
	if(!request) return;
	float v_set = strtof(request, NULL);
	if(v_set > 20) v_set = 20;
	request = strtok(NULL, ":");
	if(!request) return;
	float i_set = strtof(request, NULL);
	if(i_set > 4) i_set = 4;

//	Update_PDO(0, 2, (int)v_set*1000,(int)i_set*1000);
//	Update_Valid_PDO_Number(0, 2);
//	Send_Soft_reset_Message(0);

	//TODO: Remove responses here (in usb_cmd.c), add message to queue instead for ResponseTask to handle
	SerialMsg_t message;
	message.requesterTask = SRC_USB;
	message.commandID = USB_PD_GET;
	message.value = (int16_t)v_set*1000;
	osMessageQueuePut(serialDataQueueHandle, &message, 0, osWaitForever);
	return;
}
