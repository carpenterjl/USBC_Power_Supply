using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using USB_Power_Supply_Application.Hardware_Interface;

namespace Power_Supply_Control_WPF.Services
{
    public class CommandProcessor
    {
        public enum PowerCommand
        {
            ReadI3v3 = 0,
            ReadI2v5 = 1,
            ReadIPositive = 2,
            ReadINegative = 3,
            ReadV5v = 4,
            ReadVUsb = 5,
            ReadVSystem = 6,
            ReadV3v3 = 7,
            ReadV2v5 = 8,
            ReadVPositive = 9,
            ReadVNegative = 10,
            ToggleV3v3 = 11,
            ToggleV2v5 = 12,
            ToggleVP = 13,
            ToggleVN = 14,
            WriteVP = 15,
            WriteVN = 16,
            IDREQ = 17,
            GETSTACK = 18,
            CONNECT = 19,
            DISCONNECT = 20,
            SetILIMP = 21,
            SetILIMN = 22,
        }

        public struct commands_Struct
        {
            public PowerCommand _cmd;

            public object? _value;

            public TaskCompletionSource<object>?
                Response;
        }

        private readonly USB_Power_Supply_HW _powerSupply;

        private readonly Channel<commands_Struct> _commandChannel;

        public CommandProcessor(USB_Power_Supply_HW powerSupply)
        {
            _commandChannel =
                Channel.CreateUnbounded<commands_Struct>();

            _ = Task.Run(ProcessLoop);

            _powerSupply = powerSupply;
        }

        public async Task<object> Enqueue(PowerCommand cmd,object? value = null)
        {
            var tcs =
            new TaskCompletionSource<object>();

            await _commandChannel.Writer.WriteAsync(
                    new commands_Struct
                    {
                        _cmd = cmd,
                        _value = value,
                        Response = tcs
                    });

            return await tcs.Task;
        }

        bool VP_State,VN_State,V3_State,V2_State = false;

        private async Task ProcessLoop()
        {
            while (true)
            {
                if (_commandChannel.Reader.TryRead(out commands_Struct cmd))
                {
                    switch (cmd._cmd)
                    {
                        /* ---------------------------
                         * CONNECTION MANAGEMENT
                         * --------------------------*/
                        case PowerCommand.CONNECT:
                            cmd.Response?.SetResult(await _powerSupply.ConnectToDevice((string?)cmd._value));
                            break;

                        case PowerCommand.DISCONNECT:
                            await _powerSupply.Disconnect();
                            cmd.Response?.SetResult(true);
                            break;

                        /* ---------------------------
                         * OUTPUT TOGGLES
                         * --------------------------*/
                        case PowerCommand.ToggleVP:
                            if(!VP_State)
                            {
                                if(await _powerSupply.EnableOutput(USB_Power_Supply_HW.Voltage_Sources.V_Positive) == "OK")
                                {
                                    VP_State = true;
                                }
                            }else
                            {
                                if(await _powerSupply.DisableOutput(USB_Power_Supply_HW.Voltage_Sources.V_Positive) == "OK")
                                {
                                    VP_State = false;
                                }
                            }
                            cmd.Response?.SetResult(VP_State);
                            break;

                        case PowerCommand.ToggleVN:
                            if (!VN_State)
                            {
                                if (await _powerSupply.EnableOutput(USB_Power_Supply_HW.Voltage_Sources.V_Negative) == "OK")
                                {
                                    VN_State = true;
                                }
                            }
                            else
                            {
                                if (await _powerSupply.DisableOutput(USB_Power_Supply_HW.Voltage_Sources.V_Negative) == "OK")
                                {
                                    VN_State = false;
                                }
                            }
                            cmd.Response?.SetResult(VN_State);
                            break;

                        case PowerCommand.ToggleV3v3:
                            if (!V3_State)
                            {
                                if (await _powerSupply.EnableOutput(USB_Power_Supply_HW.Voltage_Sources.V_3v3) == "OK")
                                {
                                    V3_State = true;
                                }
                            }
                            else
                            {
                                if (await _powerSupply.DisableOutput(USB_Power_Supply_HW.Voltage_Sources.V_3v3) == "OK")
                                {
                                    V3_State = false;
                                }
                            }
                            cmd.Response?.SetResult(V3_State);
                            break;

                        case PowerCommand.ToggleV2v5:
                            if (!V2_State)
                            {
                                if (await _powerSupply.EnableOutput(USB_Power_Supply_HW.Voltage_Sources.V_2v5) == "OK")
                                {
                                    V2_State = true;
                                }
                            }
                            else
                            {
                                if (await _powerSupply.DisableOutput(USB_Power_Supply_HW.Voltage_Sources.V_2v5) == "OK")
                                {
                                    V2_State = false;
                                }
                            }
                            cmd.Response?.SetResult(V2_State);
                            break;

                        /* ---------------------------
                         * WRITE SETPOINTS
                         * --------------------------*/
                        case PowerCommand.WriteVP:
                            cmd.Response?.SetResult(await _powerSupply.SetVoltage(
                                    USB_Power_Supply_HW.Voltage_Sources.V_Positive, Convert.ToSingle(cmd._value) / 1000.0f));
                            break;

                        case PowerCommand.WriteVN:
                            cmd.Response?.SetResult(
                                await _powerSupply.SetVoltage(
                                    USB_Power_Supply_HW.Voltage_Sources.V_Negative, Convert.ToSingle(cmd._value) / 1000.0f));
                            break;

                        /* ---------------------------
                         * CURRENT READS
                         * --------------------------*/
                        case PowerCommand.ReadI3v3:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetCurrent(
                                    USB_Power_Supply_HW.Current_Sources.I_3v3));
                            break;

                        case PowerCommand.ReadI2v5:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetCurrent(
                                    USB_Power_Supply_HW.Current_Sources.I_2v5));
                            break;

                        case PowerCommand.ReadIPositive:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetCurrent(
                                    USB_Power_Supply_HW.Current_Sources.I_Positive));
                            break;

                        case PowerCommand.ReadINegative:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetCurrent(
                                    USB_Power_Supply_HW.Current_Sources.I_Negative));
                            break;

                        /* ---------------------------
                         * VOLTAGE READS
                         * --------------------------*/
                        case PowerCommand.ReadV3v3:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetVoltage(
                                    USB_Power_Supply_HW.Voltage_Sources.V_3v3));
                            break;

                        case PowerCommand.ReadV2v5:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetVoltage(
                                    USB_Power_Supply_HW.Voltage_Sources.V_2v5));
                            break;

                        case PowerCommand.ReadVPositive:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetVoltage(
                                    USB_Power_Supply_HW.Voltage_Sources.V_Positive));
                            break;

                        case PowerCommand.ReadVNegative:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetVoltage(
                                    USB_Power_Supply_HW.Voltage_Sources.V_Negative));
                            break;

                        case PowerCommand.ReadV5v:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetVoltage(
                                    USB_Power_Supply_HW.Voltage_Sources.V_5v));
                            break;

                        case PowerCommand.ReadVUsb:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetVoltage(
                                    USB_Power_Supply_HW.Voltage_Sources.V_USB));
                            break;

                        case PowerCommand.ReadVSystem:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetVoltage(
                                    USB_Power_Supply_HW.Voltage_Sources.V_System));
                            break;

                        /* ---------------------------
                         * SYSTEM / DEBUG
                         * --------------------------*/
                        case PowerCommand.IDREQ:
                            cmd.Response?.SetResult(
                                await _powerSupply.RequestID());
                            break;

                        case PowerCommand.GETSTACK:
                            cmd.Response?.SetResult(
                                await _powerSupply.GetStackSpace((uint)cmd._value));
                            break;

                        /* ---------------------------
                         * CURRENT LIMITING
                         * --------------------------*/
                        case PowerCommand.SetILIMN:
                            cmd.Response?.SetResult(
                                await _powerSupply.SetCurrentLimit((float)cmd._value, USB_Power_Supply_HW.Voltage_Sources.V_Negative));
                            break;

                        case PowerCommand.SetILIMP:
                            cmd.Response?.SetResult(
                                await _powerSupply.SetCurrentLimit((float)cmd._value, USB_Power_Supply_HW.Voltage_Sources.V_Positive));
                            break;

                        /* ---------------------------
                         * FALLBACK
                         * --------------------------*/
                        default:
                            cmd.Response?.SetResult("ERROR");
                            break;
                    }
                }
                await Task.Delay(5);
            }
        }
    }
}
