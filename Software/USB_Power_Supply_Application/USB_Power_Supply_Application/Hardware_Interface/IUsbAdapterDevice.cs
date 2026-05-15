using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USB_Power_Supply_Application.Hardware_Interface
{
    public interface IUsbAdapterDevice
    {
        enum GPIO_PINS_DEF
        {
            I2C_SCL_PIN = 1,
            I2C_SDA_PIN = 2,
            SPI_MISO_PIN = 4,
            SPI_CLK_PIN = 8,
            SPI_MOSI_PIN = 16,
            SPI_SS_PIN = 32,
            GPIO_PIN1 = 64,
            GPIO_PIN2 = 128,
        }

        bool isDeviceConnected { get; }
        Task<string> SendRawAsync(string command, CancellationToken token = default);
        Task ConnectToDevice(string deviceId, CancellationToken token = default);
        Task<byte[]> ReadBytes(int length);
        Task Disconnect();
    }
}
