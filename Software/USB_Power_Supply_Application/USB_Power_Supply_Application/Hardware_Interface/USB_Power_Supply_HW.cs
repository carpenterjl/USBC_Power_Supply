using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USB_Power_Supply_Application.Hardware_Interface
{
    internal class USB_Power_Supply_HW
    {
        private readonly IUsbAdapterDevice? _usbAdapter;

        public enum Voltage_Sources
        {
            V_System,
            V_USB,
            V_Positive,
            V_Negative,
            V_3v3,
            V_5v,
            V_2v5
        }

        public enum Current_Sources
        {
            I_Positive,
            I_Negative,
            I_3v3,
            I_2v5
        }

        public USB_Power_Supply_HW(IUsbAdapterDevice adapter)
        {
            this._usbAdapter = adapter;
        }

        public async Task<string> SetVoltage(Voltage_Sources VSource, float VSet)
        {
            string response = "ERR";
            string? command = null;
            switch(VSource)
            {
                case  Voltage_Sources.V_Positive:
                    if(VSet > 20 || VSet < 1.5) return response;
                    command = $"VSET:P:{VSet:F3}:";
                    break;
                case Voltage_Sources.V_Negative:
                    if (VSet > -1.5 || VSet < -20) return response;
                    command = $"VSET:N:{VSet:F3}";
                    break;
                default:
                    return response;
            }
            if (_usbAdapter != null && _usbAdapter.isDeviceConnected)
            {
                response = await _usbAdapter.SendRawAsync(command);
            }
            return response;
        }

        public async Task<float> GetVoltage(Voltage_Sources VSource)
        {
            string response = "0.000";
            string? command = null;
            switch(VSource)
            {
                case Voltage_Sources.V_Positive:
                    command = "VGET:P:";
                    break;
                case Voltage_Sources.V_Negative:
                    command = "VGET:N:";
                    break;
                case Voltage_Sources.V_2v5:
                    command = "VGET:2:";
                    break;
                case Voltage_Sources.V_3v3:
                    command = "VGET:3:";
                    break;
                case Voltage_Sources.V_USB:
                    command = "VGET:U:";
                    break;
                case Voltage_Sources.V_System:
                    command = "VGET:S:";
                    break;
                case Voltage_Sources.V_5v:
                    command = "VGET:5:";
                    break;
                default: return float.Parse(response);
            }
            if (_usbAdapter != null && _usbAdapter.isDeviceConnected)
            {
                response = await _usbAdapter.SendRawAsync(command);
            }
            try
            {
                return float.Parse(response);
            }
            catch
            {
                return float.NaN;
            }
        }

        public async Task<float> GetCurrent(Current_Sources ISource)
        {
            string response = "0.000";
            string? command = null;
            switch(ISource)
            {
                case Current_Sources.I_Negative:
                    command = "IGET:N:";
                    break;
                case Current_Sources.I_Positive:
                    command = "IGET:P:";
                    break;
                case Current_Sources.I_3v3:
                    command = "IGET:3:";
                    break;
                case Current_Sources.I_2v5:
                    command = "IGET:2:";
                    break;
                default: return float.Parse(response);
            }
                        if (_usbAdapter != null && _usbAdapter.isDeviceConnected)
            {
                response = await _usbAdapter.SendRawAsync(command);
            }
            if (_usbAdapter != null && _usbAdapter.isDeviceConnected)
            {
                response = await _usbAdapter.SendRawAsync(command);
            }
            try
            {
                return float.Parse(response);
            }
            catch
            {
                return float.NaN;
            }
        }

        public async Task<string> EnableOutput(Voltage_Sources VSource)
        {
            string response = "ERR";
            string? command = null;
            switch (VSource)
            {
                case Voltage_Sources.V_Positive:
                    command = "VEN:P:";
                    break;
                case Voltage_Sources.V_Negative:
                    command = "VEN:N:";
                    break;
                case Voltage_Sources.V_2v5:
                    command = "VEN:2:";
                    break;
                case Voltage_Sources.V_3v3:
                    command = "VEN:3:";
                    break;
                default: return response;
            }
            if (_usbAdapter != null && _usbAdapter.isDeviceConnected)
            {
                response = await _usbAdapter.SendRawAsync(command);
            }
            return response;
        }

        public async Task<string> DisableOutput(Voltage_Sources VSource)
        {
            string response = "ERR";
            string command = null;
            switch (VSource)
            {
                case Voltage_Sources.V_Positive:
                    command = "VDIS:P:";
                    break;
                case Voltage_Sources.V_Negative:
                    command = "VDIS:N:";
                    break;
                case Voltage_Sources.V_2v5:
                    command = "VDIS:2:";
                    break;
                case Voltage_Sources.V_3v3:
                    command = "VDIS:3:";
                    break;
                default: return response;
            }
            if (_usbAdapter != null && _usbAdapter.isDeviceConnected)
            {
                response = await _usbAdapter.SendRawAsync(command);
            }
            return response;
        }

        public async Task<string> RequestID()
        {
            string response = "ERR";
            string command = "ID?";
            if (_usbAdapter != null && _usbAdapter.isDeviceConnected)
            {
                response = await _usbAdapter.SendRawAsync(command);
            }
            return response;
        }

        public async Task<string> GetStackSpace(uint TaskID)
        {
            string response = "ERR";
            string command = $"STACK:{TaskID}:";
            if (_usbAdapter != null && _usbAdapter.isDeviceConnected)
            {
                response = await _usbAdapter.SendRawAsync(command);
            }
            return response;
        }
    }
}
