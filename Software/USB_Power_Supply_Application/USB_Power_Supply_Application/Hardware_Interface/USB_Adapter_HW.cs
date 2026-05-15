namespace USB_Power_Supply_Application.Hardware_Interface
{
    public class USB_Adapter_HW : IUsbAdapterDevice
    {
        private readonly ISERIAL? _iSerial;

        public USB_Adapter_HW(ISERIAL serial)
        {
            _iSerial = serial;
        }

        public bool isDeviceConnected => _iSerial != null && _iSerial.IsConnected;

        public async Task ConnectToDevice(string deviceId, CancellationToken token = default)
        {
            if(isDeviceConnected)
            {
                return;
            }else
            {
                if(_iSerial != null)
                {
                    await _iSerial.ConnectAsync(deviceId, 115200);
                    await _iSerial.SendCommandAsync("ID?");
                    string device_id = await _iSerial.SendCommandAsync("ID?");
                    if (device_id != null)
                    {
                        if(!device_id.Contains("JPOWER"))
                        {
                            await Disconnect();
                        }
                    }
                }
            }
        }

        public async Task Disconnect()
        {
            if(!isDeviceConnected)
            {
                return;
            }else
            {
                if (_iSerial != null)
                {
                    await _iSerial.DisconnectAsync();
                }
            }
        }

        public async Task<byte[]> ReadBytes(int length)
        {
            if (_iSerial != null)
            {
                return await _iSerial.ReadBytesAsync(length);
            }
            else
            {
                throw new Exception("Serial port was null");
            }
        }

        public async Task<string> SendRawAsync(string command, CancellationToken token = default)
        {
            if (_iSerial != null)
            {
                return await _iSerial.SendCommandAsync(command, token);
            }
            else
            {
                throw new Exception("Serial port was null");
            }
        }


    }
}
