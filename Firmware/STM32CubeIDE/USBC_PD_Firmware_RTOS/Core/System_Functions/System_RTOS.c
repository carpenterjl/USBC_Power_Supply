/*
 * System_RTOS.c
 *
 *  Created on: May 10, 2026
 *      Author: Jacob
 */

#include "System_RTOS.h"

/* USER CODE BEGIN Header_StartDefaultTask */
/**
  * @brief  Function implementing the mainSystemTask thread.
  * @param  argument: Not used
  * @retval None
  */
/* USER CODE END Header_StartDefaultTask */
void StartDefaultTask(void *argument)
{
  /* init code for USB_Device */
  MX_USB_Device_Init();
  /* USER CODE BEGIN 5 */
  int Usb_Port = 0;

  HAL_ADCEx_Calibration_Start(&hadc1, ADC_SINGLE_ENDED);

  HAL_ADC_Start_DMA(&hadc1, (uint32_t *)adc_dma_buffer, 11);

  USBC_PD_Init(Usb_Port);

  osDelay(1000/portTICK_PERIOD_MS);

  Print_SNK_PDO(Usb_Port);
  Print_PDO_FROM_SRC(Usb_Port);
  Read_RDO(Usb_Port);
  Print_RDO(Usb_Port);
  Update_PDO(Usb_Port, 2, 20000, 1000);
  Update_Valid_PDO_Number(Usb_Port, 2);
  Send_Soft_reset_Message(Usb_Port);

  USB_Cmd_Init();

  /* Infinite loop */
  for(;;)
  {
    osDelay(1);
  }

  //Terminate task in case of exit of main while loop
  osThreadTerminate(NULL);
  /* USER CODE END 5 */
}

/* USER CODE BEGIN Header_StartBlinkyTask */
/**
* @brief Function implementing the ledStatusTask thread.
* @param argument: Not used
* @retval None
*/
/* USER CODE END Header_StartBlinkyTask */
void StartBlinkyTask(void *argument)
{
  /* USER CODE BEGIN StartBlinkyTask */
  /* Infinite loop */
  for(;;)
  {
	  HAL_GPIO_TogglePin(LED0_GPIO_Port, LED0_Pin);
	  osDelay(LED_PERIOD_HALF);
  }
  //Terminate task in case of exit of main while loop
  osThreadTerminate(NULL);
  /* USER CODE END StartBlinkyTask */
}

/* USER CODE BEGIN Header_StartusbCommandTask */
/**
* @brief Function implementing the usbCommandTask thread.
* @param argument: Not used
* @retval None
*/
/* USER CODE END Header_StartusbCommandTask */
void StartusbCommandTask(void *argument)
{
  /* USER CODE BEGIN StartusbCommandTask */
  /* Infinite loop */
  for(;;)
  {
	  USB_CmdTask();
	  osDelay(USB_RESPONSE_PERIOD);
  }
  //Terminate task in case of exit of main while loop
  osThreadTerminate(NULL);
  /* USER CODE END StartusbCommandTask */
}

/* USER CODE BEGIN Header_StartvMeasureTask */
/**
* @brief Function implementing the vMeasureTask thread.
* @param argument: Not used
* @retval None
*/
/* USER CODE END Header_StartvMeasureTask */
void StartvMeasureTask(void *argument)
{
  /* USER CODE BEGIN StartvMeasureTask */
  /* Infinite loop */
  for(;;)
  {
    osDelay(1);
  }
  //Terminate task in case of exit of main while loop
  osThreadTerminate(NULL);
  /* USER CODE END StartvMeasureTask */
}

/* USER CODE BEGIN Header_StartiMeasureTask */
/**
* @brief Function implementing the iMeasureTask thread.
* @param argument: Not used
* @retval None
*/
/* USER CODE END Header_StartiMeasureTask */
void StartiMeasureTask(void *argument)
{
  /* USER CODE BEGIN StartiMeasureTask */
  /* Infinite loop */
  for(;;)
  {
    osDelay(1);
  }
  //Terminate task in case of exit of main while loop
  osThreadTerminate(NULL);
  /* USER CODE END StartiMeasureTask */
}

/* USER CODE BEGIN Header_StartResponseTask */
/**
* @brief Function implementing the cmdResponseTask thread.
* @param argument: Not used
* @retval None
*/
/* USER CODE END Header_StartResponseTask */
void StartResponseTask(void *argument)
{
  /* USER CODE BEGIN StartResponseTask */
	SerialMsg_t messageInQueue;
	char response[16] = {'\0'};
	float voltage,current = 0;
  /* Infinite loop */
  for(;;)
  {
	  if(osMessageQueueGetCount(serialDataQueueHandle) > 0)
	  {
		  if(osMessageQueueGet(serialDataQueueHandle, &messageInQueue, 0, 0) == osOK)
		  {
			  switch(messageInQueue.commandID)
			  {
			  	  	/* --- PD & Voltage Settings --- */
					case USB_PD_SET:
						voltage = (float)messageInQueue.value / 1000.0f;
						Update_PDO(0, 2, voltage*1000, 1000);
						Send_Soft_reset_Message(0);
						sprintf(response, "OK");
					break;
					case USB_PD_GET:
					break;
					case VPOS_SET:
						voltage = (float)messageInQueue.value / 1000.0f;
						SetPositiveSupply(voltage, 0);
						sprintf(response, "OK");
					break;
					case VNEG_SET:
						voltage = (float)messageInQueue.value / 1000.0f;
						SetNegativeSupply(voltage, 0);
						sprintf(response, "OK");
					break;

					/* --- Voltage Rails Getters --- */
					case V3V3_GET:
						voltage = (float)adc_dma_buffer[5] / 4095.0f * 2.5f * 2;
						sprintf(response, "%.3f", voltage);
					break;
					case V2V5_GET:
						voltage = (float)adc_dma_buffer[6] / 4095.0f * 2.5f * 2;
						sprintf(response, "%.3f", voltage);
					break;
					case VUSB_GET:
						voltage = (float)adc_dma_buffer[8] / 4095.0f * 2.5f * 11;
						sprintf(response, "%.3f", voltage);
					break;
					case VSYS_GET:
						voltage = (float)adc_dma_buffer[4] / 4095.0f * 2.5f * 11;
						sprintf(response, "%.3f", voltage);
					break;
					case V5V_GET:
						voltage = (float)adc_dma_buffer[1] / 4095.0f * 2.5f;
						sprintf(response, "%.3f", voltage);
					break;
					case VMCU_GET:
					break;
					case VNEG_GET:
						voltage = (float)adc_dma_buffer[7] / 4095.0f * 2.5f * 23.32 - (((float)adc_dma_buffer[4] / 4095.0f * 2.5f * 11) * 1.33);
						sprintf(response, "%.3f", voltage);
					break;
					case VPOS_GET:
						voltage = (float)adc_dma_buffer[0] / 4095.0f * 2.5f * 11;
						sprintf(response, "%.3f", voltage);
					break;

					/* --- Current Sensors Getters --- */
					case IPOS_GET:
						current = (float)adc_dma_buffer[10] / 4095.0f * 2.5f * 2;
						sprintf(response, "%.3f", current);
					break;
					case INEG_GET:
						current = (float)adc_dma_buffer[2] / 4095.0f * 2.5f * 1.111;
						sprintf(response, "%.3f", current);
					break;
					case I3V3_GET:
						current = (float)adc_dma_buffer[3] / 4095.0f * 2.5f * 2;
						sprintf(response, "%.3f", current);
					break;
					case I2V5_GET:
						current = (float)adc_dma_buffer[9] / 4095.0f * 2.5f * 2;
						sprintf(response, "%.3f", current);
					break;

					/* --- Rail Enables --- */
					case VP_ENABLE:
						EnableOutput(V_Positive);
						sprintf(response, "OK");
					break;
					case VN_ENABLE:
						EnableOutput(V_Negative);
						sprintf(response, "OK");
					break;
					case V3_ENABLE:
						EnableOutput(V_3v3);
						sprintf(response, "OK");
					break;
					case V2_ENABLE:
						EnableOutput(V_2v5);
						sprintf(response, "OK");
					break;

					/* --- Rail Disables --- */
					case VP_DISABLE:
						DisableOutput(V_Positive);
						sprintf(response, "OK");
					break;
					case VN_DISABLE:
						DisableOutput(V_Negative);
						sprintf(response, "OK");
					break;
					case V3_DISABLE:
						DisableOutput(V_3v3);
						sprintf(response, "OK");
					break;
					case V2_DISABLE:
						DisableOutput(V_2v5);
						sprintf(response, "OK");
					break;

					/* --- Global Controls --- */
					case ALL_ENABLE:
						EnableOutput(V_Positive);
						EnableOutput(V_Negative);
						EnableOutput(V_3v3);
						EnableOutput(V_2v5);
						sprintf(response, "OK");
					break;
					case ALL_DISABLE:
						DisableOutput(V_Positive);
						DisableOutput(V_Negative);
						DisableOutput(V_3v3);
						DisableOutput(V_2v5);
						sprintf(response, "OK");
					break;

					default:
						// Handle unknown command ID
						sprintf(response, "ERR");
					break;
			  }
			  SendResponse(&messageInQueue, response);
		  }
	  }
    osDelay(1);
  }
  //Terminate task in case of exit of main while loop
  osThreadTerminate(NULL);
  /* USER CODE END StartResponseTask */
}
