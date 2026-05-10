/*
 * System_RTOS.h
 *
 *  Created on: May 10, 2026
 *      Author: Jacob
 */

#ifndef SYSTEM_FUNCTIONS_SYSTEM_RTOS_H_
#define SYSTEM_FUNCTIONS_SYSTEM_RTOS_H_
#include "System_Functions.h"
#include "main.h"
#include "usb_device.h"
#include "cmsis_os.h"
#include "USB_PD_core.h"
#include "usb_cmd.h"

void StartDefaultTask(void *argument);
void StartBlinkyTask(void *argument);
void StartusbCommandTask(void *argument);
void StartvMeasureTask(void *argument);
void StartiMeasureTask(void *argument);
void StartResponseTask(void *argument);

#endif /* SYSTEM_FUNCTIONS_SYSTEM_RTOS_H_ */
