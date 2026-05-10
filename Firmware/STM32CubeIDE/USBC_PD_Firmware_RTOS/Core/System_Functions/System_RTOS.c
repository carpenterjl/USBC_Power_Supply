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
					break;
					case USB_PD_GET:
					break;
					case VPOS_SET:
					break;
					case VNEG_SET:
					break;

					/* --- Voltage Rails Getters --- */
					case V3V3_GET:
					break;
					case V2V5_GET:
					break;
					case VUSB_GET:
					break;
					case VSYS_GET:
					break;
					case V5V_GET:
					break;
					case VMCU_GET:
					break;
					case VNEG_GET:
					break;
					case VPOS_GET:
					break;

					/* --- Current Sensors Getters --- */
					case IPOS_GET:
					break;
					case INEG_GET:
					break;
					case I3V3_GET:
					break;
					case I2V5_GET:
					break;

					/* --- Rail Enables --- */
					case VP_ENABLE:
					break;
					case VN_ENABLE:
					break;
					case V3_ENABLE:
					break;
					case V2_ENABLE:
					break;

					/* --- Rail Disables --- */
					case VP_DISABLE:
					break;
					case VN_DISABLE:
					break;
					case V3_DISABLE:
					break;
					case V2_DISABLE: // Note: Matches your typo 'DIABLE'
					break;

					/* --- Global Controls --- */
					case ALL_ENABLE:
					break;
					case ALL_DISABLE:
					break;

					default:
					// Handle unknown command ID
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
