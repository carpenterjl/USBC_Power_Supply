using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USB_Power_Supply_Application.Hardware_Interface;
using static Power_Supply_Control_WPF.Services.CommandProcessor;

namespace Power_Supply_Control_WPF.Services
{
    public class PowerSupplyService
    {
        private readonly CommandProcessor _processor;
        private bool _vpEnabled = false;
        private bool _vnEnabled = false;
        private bool _v3Enabled = false;
        private bool _v2Enabled = false;

        public PowerSupplyService(CommandProcessor processor)
        {
            _processor = processor;
        }

        public async Task<bool> ToggleVP()
        {
            return (bool)await _processor.Enqueue(PowerCommand.ToggleVP);
        }

        public async Task SetVP(float value)
        {
            await _processor.Enqueue(PowerCommand.WriteVP,(short)(value * 1000));
        }

        public async Task<float?> ReadVPVoltage()
        {
            return (float?)(await _processor.Enqueue(PowerCommand.ReadVPositive));
        }

        public Task<bool> GetVPState()
        {
            return Task.FromResult(_vpEnabled);
        }

        public async Task<bool> ToggleVN()
        {
            return (bool)await _processor.Enqueue(PowerCommand.ToggleVN);
        }

        public async Task SetVN(float value)
        {
            await _processor.Enqueue(PowerCommand.WriteVN, (short)(value * 1000));
        }

        public async Task<float?> ReadVNVoltage()
        {
            return (float?)(await _processor.Enqueue(PowerCommand.ReadVNegative));
        }

        public Task<bool> GetVNState()
        {
            return Task.FromResult(_vnEnabled);
        }

        public async Task<bool> ToggleV3()
        {
            return (bool)await _processor.Enqueue(PowerCommand.ToggleV3v3);
        }

        public Task<bool> GetV3State()
        {
            return Task.FromResult(_v3Enabled);
        }

        public async Task<float?> ReadV3Voltage()
        {
            return (float?)(await _processor.Enqueue(PowerCommand.ReadV3v3));
        }

        public async Task<bool> ToggleV2()
        {
            return (bool)await _processor.Enqueue(PowerCommand.ToggleV2v5);
        }

        public Task<bool> GetV2State()
        {
            return Task.FromResult(_v2Enabled);
        }

        public async Task<float?> ReadV2Voltage()
        {
            return (float?)(await _processor.Enqueue(PowerCommand.ReadV2v5));
        }

        public async Task<string?> CheckDeviceID()
        {
            return (string?)(await _processor.Enqueue(PowerCommand.IDREQ));
        }

        public async Task<bool?> ConnectToDevice(string COM_PORT)
        {
            object response = await _processor.Enqueue(PowerCommand.CONNECT, COM_PORT);
            return (bool?)response;
        }

        public async Task Disconnect()
        {
            await _processor.Enqueue(PowerCommand.DISCONNECT);
        }

        public async Task<float> ReadCurrent(USB_Power_Supply_HW.Current_Sources I_Source)
        {
            float current = 0;
            switch (I_Source)
            {
                case USB_Power_Supply_HW.Current_Sources.I_3v3:
                    current = (float)await _processor.Enqueue(PowerCommand.ReadI3v3);
                    break;
                case USB_Power_Supply_HW.Current_Sources.I_Positive:
                    current = (float)await _processor.Enqueue(PowerCommand.ReadIPositive);
                    break;
                case USB_Power_Supply_HW.Current_Sources.I_Negative:
                    current = (float)await _processor.Enqueue(PowerCommand.ReadINegative);
                    break;
                case USB_Power_Supply_HW.Current_Sources.I_2v5:
                    current = (float)await _processor.Enqueue(PowerCommand.ReadI2v5);
                    break;
                default: break;
            }
            return current;
        }
    }
}
