using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USB_Power_Supply_Application.Hardware_Interface
{
    public class Serial : ISERIAL
    {
        private SerialPort? _port;

        public bool IsConnected => _port?.IsOpen == true;

        public async Task ConnectAsync(string port, int baudRate)
        {
            await Task.Run(() =>
            {
                _port = new SerialPort(port, baudRate)
                {
                    NewLine = "\n",
                    ReadTimeout = 2000,
                    WriteTimeout = 2000
                };
                _port.Open();
            });
        }

        public async Task DisconnectAsync()
        {
            await Task.Run(() => _port?.Close());
        }

        public async Task<string> SendCommandAsync(string command, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                if (!IsConnected)
                    throw new InvalidOperationException("Not connected");

                try
                {
                    _port.DiscardInBuffer();
                    _port.WriteLine(command);
                    return _port.ReadLine();
                }
                catch
                {
                    return "";
                }
            }, token);
        }

        public void Dispose()
        {
            _port?.Dispose();
        }

        public async Task<string> ReadLineAsync()
        {
            return await Task<string>.Run(() =>
            {
                if (!IsConnected)
                    throw new InvalidOperationException("Not connected");

                return _port.ReadLine();
            });
        }

        public async Task<byte[]> ReadBytesAsync(int length)
        {
            return await Task<byte[]>.Run(() =>
            {
                if (!IsConnected)
                    throw new InvalidOperationException("Not connected");

                byte[] data = new byte[length];
                _port.Read(data, 0, length);
                return data;
            });
        }
    }
}
