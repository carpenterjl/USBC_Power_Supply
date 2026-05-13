using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USB_Power_Supply_Application.Hardware_Interface
{
    public interface ISERIAL
    {
        bool IsConnected { get; }
        Task ConnectAsync(string port, int baudRate);
        Task DisconnectAsync();
        Task<string> SendCommandAsync(string command, CancellationToken token = default);

        Task<string> ReadLineAsync();

        Task<byte[]> ReadBytesAsync(int length);
    }
}
