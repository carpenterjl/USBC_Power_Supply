/*
 * System_Functions.c
 *
 *  Created on: Apr 27, 2026
 *      Author: Jacob
 */

#include "System_Functions.h"
#include "USB_PD_core.h"
#include "stdio.h"
#include "usbd_cdc_if.h"
extern I2C_HandleTypeDef hi2c2;
extern SPI_HandleTypeDef hspi3;
extern ADC_HandleTypeDef hadc1;

/* PDO Variables */
extern USB_PD_StatusTypeDef PD_status[USBPORT_MAX];
extern USB_PD_SNK_PDO_TypeDef PDO_SNK[USBPORT_MAX][3];
extern USB_PD_SRC_PDOTypeDef PDO_FROM_SRC[USBPORT_MAX][7];
extern uint8_t PDO_FROM_SRC_Valid[USBPORT_MAX];
extern uint8_t PDO_FROM_SRC_Num_Sel[USBPORT_MAX];
extern uint8_t PDO_FROM_SRC_Num[USBPORT_MAX];
extern uint8_t Policy_Engine_State[USBPORT_MAX];
extern uint8_t Go_disable_once[USBPORT_MAX];
extern uint8_t Final_Nego_done[USBPORT_MAX];
extern uint8_t Core_Process_suspended;
I2C_HandleTypeDef *hi2c[I2CBUS_MAX];
unsigned int Address;
unsigned int AddressSize = I2C_MEMADD_SIZE_8BIT;
USB_PD_I2C_PORT STUSB45DeviceConf[USBPORT_MAX];
uint32_t timer_cnt = 0;
int Flag_count = 0;
int PB_press=0;
int Time_elapse=1;
extern uint8_t Cut[USBPORT_MAX];
uint8_t USB_PD_Interupt_Flag[USBPORT_MAX];
uint8_t USB_PD_Interupt_PostponedFlag[USBPORT_MAX];
uint8_t push_button_Action_Flag[USBPORT_MAX];
uint8_t Timer_Action_Flag[USBPORT_MAX];
uint8_t connection_flag[USBPORT_MAX]={1};
uint8_t HR_rcv_flag[USBPORT_MAX] = {0};
uint32_t VBUS_Current_limitation[USBPORT_MAX] = {5},Previous_VBUS_Current_limitation[USBPORT_MAX];

V_ADC SYS_Voltage_Map[7] =
{
		{V_System, ADC_CHANNEL_6},
		{V_USB, ADC_CHANNEL_10},
		{V_Positive, ADC_CHANNEL_1},
		{V_Negative, ADC_CHANNEL_9},
		{V_3v3, ADC_CHANNEL_7},
		{V_5v, ADC_CHANNEL_2},
		{V_2v5, ADC_CHANNEL_8},
};

I_ADC SYS_Current_Map[4] =
{
		{I_Positive, ADC_CHANNEL_12},
		{I_Negative, ADC_CHANNEL_4},
		{I_3v3, ADC_CHANNEL_5},
		{I_2v5, ADC_CHANNEL_11},
};

uint16_t adc_dma_buffer[11] = {0};

/**
 * Set the positive supply output voltage, with the max current limit
 * If max current is 0, current limit will be ignored.
 * Current limiting -> To be implemented as threshold at which supply is disabled.
 *
 * Note that output regulator is a buck regulator, set_voltage must be lower than VSYS
 */
void SetPositiveSupply(float set_voltage, float max_current)
{
	//PCB Circuit -> R_DAC Code from set_voltage
	//Max Current -> TODO:Update system struct for monitoring
	uint8_t dac_addr_7bit = 0x58;
	uint8_t dac_data[2] = {0};
	float R_VAL = ( 59600.0f / (set_voltage - 0.596f) ) - 3300.0f;
	if(R_VAL >= 50000) R_VAL = 50000.0f * 255.0/256.0;
	uint8_t R_DAC = (uint8_t)(( R_VAL / 50000.0f ) * 255);
//	//TODO: Limit R_DAC based on system voltage. Temporarily set for 9V Limit
//	if(R_DAC < 20) R_DAC = 20;
	dac_data[0] = 0x00;
	dac_data[1] = R_DAC;
	HAL_I2C_Master_Transmit(&hi2c2, dac_addr_7bit, dac_data, 2, 25);
	return;
}

/**
 * Set the negative supply output voltage, with the max current limit
 * If max current is 0, current limit will be ignored.
 * Current limiting -> To be implemented as threshold at which supply is disabled.
 *
 * Note that output regulator is buck/boost but resistor divider limits minimum to -20V
 *
 * Voltage is set to negative absolute value of set_voltage
 */
void SetNegativeSupply(float set_voltage, float max_current)
{
	//PCB Circuit -> R_DAC Code from set_voltage
	//Max Current -> TODO:Update system struct for monitoring
	float R_VAL = fabs(set_voltage)*2640 - 3800;
	if(R_VAL >= 50000) R_VAL = 50000.0f * 1023.0/1024.0;
	uint16_t R_DAC = (uint16_t)(( R_VAL / 50000 ) * 1023);

	// 1. UNLOCK the wiper
	uint16_t unlock_cmd = 0x1802; // Command 6: Allow update of wiper position
	HAL_GPIO_WritePin(NDAC_SYNC_L_GPIO_Port, NDAC_SYNC_L_Pin, 0);
	HAL_SPI_Transmit(&hspi3, (uint8_t*)&unlock_cmd, 1, 10);
	HAL_GPIO_WritePin(NDAC_SYNC_L_GPIO_Port, NDAC_SYNC_L_Pin, 1);

	// 1. Set the wiper value
	uint16_t wiper_val = (1 << 10) | (R_DAC);
	HAL_GPIO_WritePin(NDAC_SYNC_L_GPIO_Port, NDAC_SYNC_L_Pin, 0);
	HAL_SPI_Transmit(&hspi3, (uint8_t*)&wiper_val, 1, 10);
	HAL_GPIO_WritePin(NDAC_SYNC_L_GPIO_Port, NDAC_SYNC_L_Pin, 1);

	return;
}

/**
 * Disable all supply outputs immediately.
 * Disables the following:
 * Positive switching regulator
 * Negative switching regulator
 * 3.3V External Output
 * 2.5V External Output
 */
void DisableAllOutputs(void)
{
	HAL_GPIO_WritePin(VP_EN_GPIO_Port, VP_EN_Pin, 0);
	HAL_GPIO_WritePin(VN_EN_GPIO_Port, VN_EN_Pin, 0);
	HAL_GPIO_WritePin(EN_3V3_EXT_GPIO_Port, EN_3V3_EXT_Pin, 1);
	HAL_GPIO_WritePin(EN_2V5_EXT_GPIO_Port, EN_2V5_EXT_Pin, 1);
	return;
}

/**
 * Read the supply voltage from the desired voltage monitoring source.
 */
float ReadVoltage(Voltage_Sources V_Source)
{
	float voltage = 0.0f;
	uint32_t adc_val = 0;
	adc_val = Read_ADC_Channel(&hadc1, SYS_Voltage_Map[V_Source].ADC_Channel);
	voltage = (float)(adc_val / 4096.0) * 2.5f;
//	voltage = Read_Calibrated_Voltage(adc_val);
	return voltage;
}

/**
 * Read the supply current from the desired current monitoring source.
 */
float ReadCurrent(Current_Sources I_Source)
{
	float current = 0.0f;
	uint32_t adc_val = 0;
	adc_val = Read_ADC_Channel(&hadc1, SYS_Current_Map[I_Source].ADC_Channel);
	current = Read_Calibrated_Voltage(adc_val);
	return current;
}

/**
 * Read the power output from the supply.
 * Only supplies which have both voltage and current monitoring are supported.
 */
float ReadPower(Voltage_Sources V_Source, Current_Sources I_Source)
{
	float voltage = 0.0f;
	float current = 0.0f;
	float watts = 0.0f;
	if(V_Source > 6 && I_Source > 3)
	{
		return 0;
	}
	if(V_Source <= 6)
	{
		switch(V_Source)
		{
			case V_System:
				voltage = 0;
				current = 0;
				watts = 0;
				break;
			case V_USB:
				voltage = 0;
				current = 0;
				watts = 0;
				break;
			case V_Positive:
				voltage = ReadVoltage(V_Positive);
				current = ReadCurrent(I_Positive);
				break;
			case V_Negative:
				voltage = ReadVoltage(V_Negative);
				current = ReadCurrent(I_Negative);
				break;
			case V_3v3:
				voltage = ReadVoltage(V_3v3);
				current = ReadCurrent(I_3v3);
				break;
			case V_5v:
				voltage = 0;
				current = 0;
				watts = 0;
				break;
			case V_2v5:
				voltage = ReadVoltage(V_2v5);
				current = ReadCurrent(I_2v5);
				break;
			default:
				voltage = 0;
				current = 0;
				watts = 0;
				break;
		}
	}else
	{
		switch(I_Source)
		{
			case I_Positive:
				voltage = ReadVoltage(V_Positive);
				current = ReadCurrent(I_Positive);
				break;
			case I_Negative:
				voltage = ReadVoltage(V_Negative);
				current = ReadCurrent(I_Negative);
				break;
			case I_3v3:
				voltage = ReadVoltage(V_3v3);
				current = ReadCurrent(I_3v3);
				break;
			case I_2v5:
				voltage = ReadVoltage(V_2v5);
				current = ReadCurrent(I_2v5);
				break;
			default:
				voltage = 0;
				current = 0;
				watts = 0;
				break;
		}
	}
	watts = voltage * current;
	return watts;
}

uint32_t Read_ADC_Channel(ADC_HandleTypeDef *hadc, uint32_t channel)
{
	static uint32_t last_channel = ADC_CHANNEL_1;
    ADC_ChannelConfTypeDef sConfig = {0};
    HAL_ADC_Stop_DMA(hadc);

    __HAL_ADC_CLEAR_FLAG(hadc, ADC_FLAG_EOC | ADC_FLAG_EOS | ADC_FLAG_OVR);

    sConfig.Channel = last_channel;
    sConfig.Rank = ADC_REGULAR_RANK_2;
    HAL_ADC_ConfigChannel(hadc, &sConfig);

    sConfig.Channel = channel;
    sConfig.Rank = ADC_REGULAR_RANK_1;
    sConfig.SamplingTime = ADC_SAMPLETIME_92CYCLES_5;
    sConfig.SingleDiff = ADC_SINGLE_ENDED;
    sConfig.OffsetNumber = ADC_OFFSET_NONE;
    sConfig.Offset = 0;

    HAL_ADC_ConfigChannel(hadc, &sConfig);
    last_channel = channel;

    return 0;
}

float Read_Calibrated_Voltage(uint16_t raw_adc_reading)
{
    uint16_t vrefint_cal = *VREFINT_CAL_ADDR;

    uint32_t vrefint_data = Read_ADC_Channel(&hadc1, ADC_CHANNEL_VREFINT);

    if (vrefint_data == 0) return 0.0f;

    float actual_vref = 3.0f * ((float)vrefint_cal / (float)vrefint_data);

    float pin_voltage = ((float)raw_adc_reading / 4096.0f) * actual_vref;

    return pin_voltage;
}

void EnableOutput(Voltage_Sources V_Source)
{
	switch(V_Source)
	{
	case V_Positive:
		HAL_GPIO_WritePin(VP_EN_GPIO_Port, VP_EN_Pin, 1);
		break;
	case V_Negative:
		HAL_GPIO_WritePin(VP_EN_GPIO_Port, VN_EN_Pin, 1);
		break;
	case V_3v3:
		HAL_GPIO_WritePin(VP_EN_GPIO_Port, EN_3V3_EXT_Pin, 0);
		break;
	case V_2v5:
		HAL_GPIO_WritePin(VP_EN_GPIO_Port, EN_2V5_EXT_Pin, 0);
		break;
	default: break;
	}
	return;
}

void DisableOutput(Voltage_Sources V_Source)
{
	switch(V_Source)
	{
	case V_Positive:
		HAL_GPIO_WritePin(VP_EN_GPIO_Port, VP_EN_Pin, 0);
		break;
	case V_Negative:
		HAL_GPIO_WritePin(VP_EN_GPIO_Port, VN_EN_Pin, 0);
		break;
	case V_3v3:
		HAL_GPIO_WritePin(VP_EN_GPIO_Port, EN_3V3_EXT_Pin, 1);
		break;
	case V_2v5:
		HAL_GPIO_WritePin(VP_EN_GPIO_Port, EN_2V5_EXT_Pin, 1);
		break;
	default: break;
	}
	return;
}

int Device_Manager(uint8_t Usb_Port)
{
  int Status;

  Status = Get_Device_STATUS(Usb_Port);
  switch(Status)
  {
  case Not_Connected :
#ifdef PRINTF
    printf("\r\n ---- Usb_Port #0: NOT CONNECTED  ---------------\r\n");
#endif
    break;
  case TypeC_Only :
    Print_Type_C_Only_Status(Usb_Port);
    break;
  case Connected_Unknown_SRC_PDOs :
    {
    Send_Soft_reset_Message(Usb_Port);
    Final_Nego_done[Usb_Port] = 0;
    }
    break;
  case Connected_5V_PD :
#ifdef PRINTF
    printf(" ---> Final PDO_Negociated 5V \r\n");
#endif
    break;
  case Connected_no_Match_found :
#ifdef PRINTF
    printf(" ---> No Matching Found \r\n");
#endif
    break;
  case Connected_Matching_ongoing :
    {
    Send_Soft_reset_Message(Usb_Port);
    Final_Nego_done[Usb_Port] = 0;
    }
#ifdef PRINTF
    printf(" ---> Matching on going reset sent \r\n");
#endif
    break;
  case Connected_Mached :
#ifdef PRINTF
    printf(" ---> Final PDO_Negociated \r\n");
#endif
    break;
  case Not_Connected_attached_wait :
#ifdef PRINTF
    printf("\r\n ---- Usb_Port #0: in Attached wait state -------\r\n");
    printf("\r\n ---- Usb_Port #0: reset by register ongoing  -------\r\n");
#endif
    break;
  case Hard_Reset_ongoing :
#ifdef PRINTF
    printf("----------HW reset ongoing ---------\r\n");
#endif
    break;
  default: // Policy_Defauld
    break;
  }
  if (VBUS_Current_limitation[Usb_Port] != Previous_VBUS_Current_limitation[Usb_Port])
  {
    Previous_VBUS_Current_limitation[Usb_Port] = VBUS_Current_limitation[Usb_Port];
#ifdef PRINTF

    printf("\r\n - Current limit modified I bus set to %d mA ---\r\n",VBUS_Current_limitation[Usb_Port]);
#endif
  }
  return 0;

}

void SendResponse(SerialMsg_t *source_message, char *results)
{
    char buffer[32];
    if(results == NULL) return;
    strcpy(buffer, results);
    buffer[strlen(buffer)] = '\n';
    if (source_message->requesterTask == SRC_USB)
    {
    	CDC_Transmit_FS((uint8_t*)buffer, strlen(buffer) + 1);
    }
    else if (source_message->requesterTask == SRC_UART)
    {
//        HAL_UART_Transmit(&huart2, (uint8_t*)buffer, len, 100);
    }
    return;
}
