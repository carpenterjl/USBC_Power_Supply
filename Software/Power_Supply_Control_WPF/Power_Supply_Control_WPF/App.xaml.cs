using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace Power_Supply_Control_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                File.WriteAllText(
                    "UnhandledException.txt",
                    e.ExceptionObject.ToString());
            };

            DispatcherUnhandledException += (s, e) =>
            {
                File.WriteAllText(
                    "DispatcherException.txt",
                    e.Exception.ToString());
            };
        }
    }

}
