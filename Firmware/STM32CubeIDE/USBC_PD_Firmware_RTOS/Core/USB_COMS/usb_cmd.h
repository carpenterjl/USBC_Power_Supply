#ifndef USB_CMD_H
#define USB_CMD_H
#include "System_Functions.h"
void USB_CmdTask(void);   // Call this from your main loop
void USB_Cmd_Init(void);  // Optional init step if needed
int parse_hex_bytes(char *argstr, uint8_t *out, int maxlen);
int parse_args(char *argstr, char *out, int maxlen);

typedef struct
{
    const char *cmd;             // Command name ("SPI", "PING", etc.)
    const char *args;            // Argument format ("<byte1> <byte2>", "", etc.)
    const char *description;     // Description ("Send SPI data", etc.)
    void (*handler)(char *args); // Pointer to the function that runs this command
} USB_COMMANDS;

typedef struct
{
	char* config;
	void (*config_handler)(char *args);
} CONFIGS;

typedef enum
{
	ERROR_NONE,
	ERROR_SPI,
	ERROR_GPIO,
	ERROR_ARGS,
	ERROR_SD,
}ERROR_CODES;

void cmd_help(char *args);
void cmd_id(char *args);
void cmd_error(char *args);
void cmd_spi_transfer(char *args);
void set_supply_voltage(char *args);
void get_supply_voltage(char *args);
void get_supply_current(char *args);
void enable_supply(char *args);
void disable_supply(char *args);
void measurement_mode(char *args);
void request_usbc_voltage(char *args);
#endif
