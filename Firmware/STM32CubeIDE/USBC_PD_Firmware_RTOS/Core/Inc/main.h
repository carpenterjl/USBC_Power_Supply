/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.h
  * @brief          : Header for main.c file.
  *                   This file contains the common defines of the application.
  ******************************************************************************
  * @attention
  *
  * Copyright (c) 2026 STMicroelectronics.
  * All rights reserved.
  *
  * This software is licensed under terms that can be found in the LICENSE file
  * in the root directory of this software component.
  * If no LICENSE file comes with this software, it is provided AS-IS.
  *
  ******************************************************************************
  */
/* USER CODE END Header */

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __MAIN_H
#define __MAIN_H

#ifdef __cplusplus
extern "C" {
#endif

/* Includes ------------------------------------------------------------------*/
#include "stm32g4xx_hal.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

/* USER CODE END Includes */

/* Exported types ------------------------------------------------------------*/
/* USER CODE BEGIN ET */

/* USER CODE END ET */

/* Exported constants --------------------------------------------------------*/
/* USER CODE BEGIN EC */

/* USER CODE END EC */

/* Exported macro ------------------------------------------------------------*/
/* USER CODE BEGIN EM */

/* USER CODE END EM */

/* Exported functions prototypes ---------------------------------------------*/
void Error_Handler(void);

/* USER CODE BEGIN EFP */

/* USER CODE END EFP */

/* Private defines -----------------------------------------------------------*/
#define EN_2V5_EXT_Pin GPIO_PIN_13
#define EN_2V5_EXT_GPIO_Port GPIOC
#define EN_3V3_EXT_Pin GPIO_PIN_14
#define EN_3V3_EXT_GPIO_Port GPIOC
#define VN_EN_Pin GPIO_PIN_15
#define VN_EN_GPIO_Port GPIOC
#define ALERT_N_Pin GPIO_PIN_4
#define ALERT_N_GPIO_Port GPIOA
#define GPIO0_Pin GPIO_PIN_5
#define GPIO0_GPIO_Port GPIOA
#define GPIO1_Pin GPIO_PIN_6
#define GPIO1_GPIO_Port GPIOA
#define GPIO2_Pin GPIO_PIN_7
#define GPIO2_GPIO_Port GPIOA
#define GPIO5_Pin GPIO_PIN_4
#define GPIO5_GPIO_Port GPIOC
#define VP_EN_Pin GPIO_PIN_5
#define VP_EN_GPIO_Port GPIOC
#define RESET_PD_Pin GPIO_PIN_0
#define RESET_PD_GPIO_Port GPIOB
#define GPIO3_Pin GPIO_PIN_2
#define GPIO3_GPIO_Port GPIOB
#define SPI_SS_EXT_Pin GPIO_PIN_10
#define SPI_SS_EXT_GPIO_Port GPIOB
#define LED3_Pin GPIO_PIN_11
#define LED3_GPIO_Port GPIOB
#define LED2_Pin GPIO_PIN_13
#define LED2_GPIO_Port GPIOB
#define LED0_Pin GPIO_PIN_6
#define LED0_GPIO_Port GPIOC
#define LED1_Pin GPIO_PIN_7
#define LED1_GPIO_Port GPIOC
#define VP_SDA_Pin GPIO_PIN_8
#define VP_SDA_GPIO_Port GPIOA
#define VP_SCL_Pin GPIO_PIN_9
#define VP_SCL_GPIO_Port GPIOA
#define NDAC_SYNC_L_Pin GPIO_PIN_2
#define NDAC_SYNC_L_GPIO_Port GPIOD
#define NDAC_RDY_Pin GPIO_PIN_5
#define NDAC_RDY_GPIO_Port GPIOB
#define GPIO6_Pin GPIO_PIN_6
#define GPIO6_GPIO_Port GPIOB
#define GPIO4_Pin GPIO_PIN_9
#define GPIO4_GPIO_Port GPIOB

/* USER CODE BEGIN Private defines */

/* USER CODE END Private defines */

#ifdef __cplusplus
}
#endif

#endif /* __MAIN_H */
