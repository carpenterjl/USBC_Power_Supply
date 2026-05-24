using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static Power_Supply_Control_WPF.Services.ConsoleMessage;

namespace Power_Supply_Control_WPF.Services
{
    public class ConsoleLogger
    {
        public ObservableCollection<ConsoleMessage>  Messages { get; } = new();

        public void Add(LogLevel level, string source, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(new ConsoleMessage{
                        Timestamp = DateTime.Now,
                        Level = level,
                        Source = source,
                        Message = message,
                        Foreground = GetColor(level)});

                Trim();
            });
        }
        private void Trim()
        {
            while (Messages.Count > 5000)
                Messages.RemoveAt(0);
        }

        private Brush GetColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Info =>
                    Brushes.White,

                LogLevel.Warning =>
                    Brushes.Gold,

                LogLevel.Error =>
                    Brushes.Red,

                LogLevel.Command =>
                    Brushes.Cyan,

                LogLevel.Measurement =>
                    Brushes.LimeGreen,

                LogLevel.Serial =>
                    Brushes.DeepSkyBlue,

                LogLevel.Debug =>
                    Brushes.Gray,

                _ =>
                    Brushes.White
            };
        }
    }
}
