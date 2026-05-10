/*
 * HMI_Display.c
 *
 *  Created on: Nov 23, 2025
 *      Author: Jacob
 */

#include "stm32g4xx_hal.h"
#include "HMI_Display.h"
#include "string.h"
#include "System_Functions.h"

static uint8_t UART_TX_PACKET[256] = {0};
static uint8_t UART_RX_PACKET[256] = {0};
static uint16_t crc_16 = 0;

const unsigned char auchCRCHi[] =
{
	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,0x00, 0xC1, 0x81,
	0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81,0x40, 0x01, 0xC0,
	0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1,0x81, 0x40, 0x01,
	0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,0xC0, 0x80, 0x41,
	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,0x00, 0xC1, 0x81,
	0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80,0x41, 0x01, 0xC0,
	0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,0x80, 0x41, 0x01,
	0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00,0xC1, 0x81, 0x40,
	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,0x00, 0xC1, 0x81,
	0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,0x40, 0x01, 0xC0,
	0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1,0x81, 0x40, 0x01,
	0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01,0xC0, 0x80, 0x41,
	0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,0x00, 0xC1, 0x81,
	0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,0x40, 0x01, 0xC0,
	0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,0x80, 0x41, 0x01,
	0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,0xC0, 0x80, 0x41,
	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,0x00, 0xC1, 0x81,
	0x40
};
const char auchCRCLo[] =
{
	0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7,0x05, 0xC5, 0xC4,
	0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB,0x0B, 0xC9, 0x09,
	0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE,0xDF, 0x1F, 0xDD,
	0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2,0x12, 0x13, 0xD3,
	0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32,0x36, 0xF6, 0xF7,
	0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E,0xFE, 0xFA, 0x3A,
	0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B,0x2A, 0xEA, 0xEE,
	0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27,0xE7, 0xE6, 0x26,
	0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1,0x63, 0xA3, 0xA2,
	0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD,0x6D, 0xAF, 0x6F,
	0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8,0xB9, 0x79, 0xBB,
	0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4,0x74, 0x75, 0xB5,
	0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,0x50, 0x90, 0x91,
	0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94,0x54, 0x9C, 0x5C,
	0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59,0x58, 0x98, 0x88,
	0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D,0x4D, 0x4C, 0x8C,
	0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83,0x41, 0x81, 0x80,
	0x40
};

unsigned short CRC16(unsigned char *pMsg, unsigned short DataLen)
{
	unsigned char uchCRCHi = 0xFF ; /* CRC higher byte initialization */
	unsigned char uchCRCLo = 0xFF ; /* CRC lower byte initialization */
	unsigned uIndex ; /* CRC index */

	while (DataLen--) /* Loop for Calculation */
	{
		uIndex = uchCRCLo ^ *pMsg++ ; /* Calculate CRC */
		uchCRCLo = uchCRCHi ^ auchCRCHi[uIndex] ;
		uchCRCHi = auchCRCLo[uIndex] ;
	}

	return (uint16_t)(uchCRCLo << 8 | uchCRCHi);
}

/**
 * HMI_PACKET Definition:
 * Byte:	Data:
 * 0		Address HI
 * 1		Address LO
 * 2*N		Data (multiple of 2 bytes!)
 *
 * packet_length = ADDRESS(2),DATA(2^n) = 2 + data_length
 */
HMI_STATUS HMI_Write_Data(uint8_t *HMI_PACKET, uint8_t packet_length)
{
//	HAL_UART_DMAStop(&huart2);
	UART_TX_PACKET[0] = 0x5A; //Header HI
	UART_TX_PACKET[1] = 0xA5; //Header LO
	UART_TX_PACKET[2] = packet_length + 3; //Length  (+3 for CMD + CRC)
	UART_TX_PACKET[3] = 0x10; //Write Command
	uint8_t packet_idx = 0;

	if(packet_length > 252) return HMI_ERROR;

	while(packet_idx < packet_length) //copy up to CRC
	{
		UART_TX_PACKET[4 + packet_idx++] = HMI_PACKET[packet_idx];
	}
	crc_16 = CRC16(&UART_TX_PACKET[3], packet_length + 1);
	UART_TX_PACKET[4 + packet_idx] = ( crc_16 >> 8 ) & 0xFF;
	UART_TX_PACKET[5 + packet_idx] = crc_16 & 0xFF;

	if(HAL_UART_Transmit(&huart2, UART_TX_PACKET, 6 + packet_length, 100) != HAL_OK) //Transmit full packet with CRC
	{
		return HMI_ERROR;
	}
//	startReceivingData();
	return HMI_OK;
}

HMI_STATUS HMI_Read_Data(uint8_t *REC_PACKET, uint16_t VAR_ADDRESS, uint8_t READ_LENGTH)
{
	HAL_UART_DMAStop(&huart2);
	//	  5AA5 07 03 7005 0001 AB01 Read Hour
	UART_TX_PACKET[0] = 0x5A;
	UART_TX_PACKET[1] = 0xA5;
	UART_TX_PACKET[2] = 0x07; //Length
	UART_TX_PACKET[3] = 0x03; //Read Commmand
	UART_TX_PACKET[4] = ( VAR_ADDRESS >> 8 )  & 0xFF; //ADDR HI
	UART_TX_PACKET[5] = VAR_ADDRESS & 0xFF; //ADDR LO
	UART_TX_PACKET[6] = 0x00; //LEN HI (0)
	UART_TX_PACKET[7] = READ_LENGTH; //LEN LO (0-255)
	crc_16 = CRC16(&UART_TX_PACKET[3], 5);
	UART_TX_PACKET[8] = ( crc_16 >> 8 ) & 0xFF; //CRC HI
	UART_TX_PACKET[9] = crc_16 & 0xFF; //CRC LO

//	UART_TX_PACKET[0] = 0x5A;
//	UART_TX_PACKET[1] = 0xA5;
//	UART_TX_PACKET[2] = 0x07; //Length
//	UART_TX_PACKET[3] = 0x03; //Read Commmand
//	UART_TX_PACKET[4] = 0x70; //ADDR HI
//	UART_TX_PACKET[5] = 0x05; //ADDR LO
//	UART_TX_PACKET[6] = 0x00; //LEN HI (0)
//	UART_TX_PACKET[7] = 0x01; //LEN LO (0-255)
//	UART_TX_PACKET[8] = 0xAB; //CRC HI
//	UART_TX_PACKET[9] = 0x01; //CRC LO
//	HAL_UART_Transmit(&huart2, UART_TX_PACKET, 10, 100);
//	HAL_UART_Receive(&huart2, UART_RX_PACKET, 12, 100);

	// Send Read Packet
	if(HAL_UART_Transmit(&huart2, UART_TX_PACKET, 10, 100) != HAL_OK)
	{
		return HMI_ERROR;
	}

	//Receive Data
	if(HAL_UART_Receive(&huart2, REC_PACKET, 10 + 2*READ_LENGTH, 100) != HAL_OK)
	{
		return HMI_ERROR;
	}

	startReceivingData();
	return HMI_OK;
}

HMI_STATUS HMI_Update_Number_Label(float NUMBER, uint16_t ADDRESS)
{
//	HAL_UART_DMAStop(&huart2);
	static uint8_t update_packet[4] = {0};
	int16_t NUM_BYTES = (int16_t)(NUMBER * 1000);
	//Limit for voltage is +/- 20,000

	update_packet[0] = (ADDRESS >> 8) & 0xFF;//ADDR HI
	update_packet[1] = ADDRESS & 0xFF;//ADDR LO
	update_packet[2] = (NUM_BYTES >> 8) & 0xFF;//VAL 3 (Bytes 3,2)
	update_packet[3] = NUM_BYTES & 0xFF;//VAL 1  (Bytes 1,0)
	HMI_Write_Data(update_packet, 4);

//	startReceivingData();
	return HMI_OK;
}

HMI_STATUS HMI_Update_FileName(char *file_name)
{
	HMI_STATUS status = HMI_OK;
	static uint8_t update_packet[128] = {0};
	update_packet[0] = 0x56;//ADDR HI
	update_packet[1] = 0x78;//ADDR LO
	static uint8_t data_length = 0;
	if(file_name == NULL || strlen(file_name) == 0 || strlen(file_name) > 64) return HMI_ERROR;
	for(data_length = 0; data_length < strlen(file_name); data_length++)
	{
		update_packet[data_length + 2] = file_name[data_length];
	}
	update_packet[data_length++] = 0;
	update_packet[data_length++] = 0;
	HMI_Write_Data(update_packet, data_length + 2);
	return HMI_OK;
}

void Process_UART_Data(uint8_t length_received)
{
	__disable_irq();
    static uint8_t parser_buf[255];
    static uint16_t parser_len = 0;

    if (parser_len + length_received > sizeof(parser_buf))
        parser_len = 0;

    memcpy(&parser_buf[parser_len], UART_RX_PACKET, length_received);
    parser_len += length_received;

    uint16_t i = 0;
    while (i + 3 < parser_len)
    {
        if (parser_buf[i] == 0x5A && parser_buf[i + 1] == 0xA5)
        {
            uint8_t pkt_len = parser_buf[i + 2];
            if (i + 3 + pkt_len > parser_len)
                break; // not full command

            uint8_t cmd = parser_buf[i + 3];

            if (cmd == 0x10)
            {
                //Ignore this (ACK from write response)
//                parser_len = 0;
//                memset(parser_buf, 0, sizeof(parser_buf));
            	i += 3 + pkt_len;
            	continue;
            }
            else if (cmd == 0x41)
            {
            	// Command found, process and reset buf so it doesn't get procesesed twice
                Handle_Touchscreen_Packet(&parser_buf[i]);
//                parser_len = 0;
//                memset(parser_buf, 0, sizeof(parser_buf));
                i += 3 + pkt_len;
                continue;
            }
            //Skip processed commands, continue searching for more
            i += 3 + pkt_len;
        }
        else //Didn't find header yet
        {
            i++;
        }
    }

    // Compact leftover if packet was split at the end of buffer
    if (i < parser_len)
    {
        memmove(parser_buf, &parser_buf[i], parser_len - i);
        parser_len -= i;
    }
    else
    {
        parser_len = 0;
    }
    __enable_irq();
    startReceivingData();
    return;
}
static uint8_t vbit_status = 0;
void Handle_Touchscreen_Packet(uint8_t *packet)
{
    // Verify header just in case
    if (packet[0] != 0x5A || packet[1] != 0xA5)
        return;

    uint8_t len = packet[2];
    uint8_t cmd = packet[3];
    uint16_t address = (packet[4] << 8) | packet[5];
    uint16_t value = (packet[6] << 8) | packet[7];

//    uint16_t crc = (packet[8] << 8) | packet[9];

    // Optionally verify CRC if you have a CRC16 function
    // if (crc != calc_crc16(&packet[2], len)) return;

//    printf("CMD=0x%02X, ADDR=0x%04X, VAL=0x%04X, CRC=0x%04X\r\n",
//            cmd, address, value, crc);

    // Example: handle touch commands
    if (cmd == 0x41)
    {
        switch (address)
        {
        	//Default button press
            case 0xFFFF:
            	switch(value)
            	{
            	//VOUTP TOGGLE
            	case 0x0020:
            		if(vbit_status & 0x01)
            		{
            			vbit_status &= ~0x01;
            			DisableOutput(V_Positive);
            		}else
            		{
            			vbit_status |= 0x01;
            			EnableOutput(V_Positive);
            		}
            		uint8_t hmi_bit_update[4] = {0};
            		hmi_bit_update[0] = 0x00;
            		hmi_bit_update[1] = 0x28;
            		hmi_bit_update[2] = 0x00;
            		hmi_bit_update[3] = vbit_status;
            		HMI_Write_Data(hmi_bit_update, 4);
            		break;

				//VOUTN TOGGLE
            	case 0x0021:
            		if(vbit_status & 0x02)
					{
						vbit_status &= ~0x02;
						DisableOutput(V_Negative);
					}else
					{
						vbit_status |= 0x02;
						EnableOutput(V_Negative);
					}
					hmi_bit_update[0] = 0x00;
					hmi_bit_update[1] = 0x28;
					hmi_bit_update[2] = 0x00;
					hmi_bit_update[3] = vbit_status;
					HMI_Write_Data(hmi_bit_update, 4);
            		break;

				//Monitoring Mode Entered
            	case 0x0022:
            		break;

            	//Home Button
            	case 0x0023:
            		break;

				//Shutdown External Supplies
            	case 0x0024:
            		DisableAllOutputs();
            		break;

            	default: break;
            	}
            	break;

			//VP Keyboard response
			case 0x013C:
				int16_t v_set = (packet[6] << 8) | packet[7];
				float voltage_setting = (float)v_set/1000.0f;
				if(voltage_setting < 20)
				{
					SetPositiveSupply(voltage_setting, 0);
				}
				break;
			//IP Keyboard response
			case 0x0146:
				int16_t i_set = (packet[6] << 8) | packet[7];
				float current_setting = (float)i_set/1000.0f;
				if(current_setting < 3)
				{
					SetPositiveSupply(0, current_setting);
				}
				break;
			//VN Keyboard response
			case 0x0150:
				v_set = (packet[6] << 8) | packet[7];
				voltage_setting = (float)v_set/1000.0f;
				if(voltage_setting > -21)
				{
					SetNegativeSupply(voltage_setting, 0);
				}
				break;
			//IN Keyboard response
			case 0x015A:
				i_set = (packet[6] << 8) | packet[7];
				current_setting = (float)i_set/1000.0f;
				if(current_setting < 3)
				{
					SetNegativeSupply(0, current_setting);
				}
				break;
            default: break;
        }
    }
    return;
}

void startReceivingData()
{
//	HAL_UARTEx_ReceiveToIdle_DMA(&huart2, UART_RX_PACKET, sizeof(UART_RX_PACKET)); //Restart DMA
	memset(UART_RX_PACKET, '\0', sizeof(UART_RX_PACKET));
//	HAL_UART_Receive_DMA(&huart2, UART_RX_PACKET, sizeof(UART_RX_PACKET)); //Restart DMA
	HAL_UARTEx_ReceiveToIdle_IT(&huart2, UART_RX_PACKET, sizeof(UART_RX_PACKET));
	return;
}
