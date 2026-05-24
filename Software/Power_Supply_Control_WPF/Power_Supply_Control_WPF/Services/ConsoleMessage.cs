using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Power_Supply_Control_WPF.Services
{
    public class ConsoleMessage
    {
        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            Command,
            Measurement,
            Serial,
            Debug
        }

        public DateTime Timestamp { get; set; }

        public LogLevel Level { get; set; }

        public string Message { get; set; }

        public string Source { get; set; }

        public Brush Foreground { get; set; }
    }
}
