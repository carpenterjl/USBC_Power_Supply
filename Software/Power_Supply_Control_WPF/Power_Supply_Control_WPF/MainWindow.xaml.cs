using OpenTK.Windowing.Desktop;
using Power_Supply_Control_WPF.GUI_Elements;
using Power_Supply_Control_WPF.Services;
using ScottPlot;
using ScottPlot.Plottables;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using USB_Power_Supply_Application.Hardware_Interface;

namespace Power_Supply_Control_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timerUpdate =new() { Interval = TimeSpan.FromMilliseconds(300) };
        static bool VP_Streaming, VN_Streaming, V3_Streaming, V2_Streaming = false;
        private Power_Supply_Control_WPF.Services.AxisManager axisManager = new();
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                this.SizeChanged += Window_SizeChanged;

                ISERIAL serial = new Serial();

                IUsbAdapterDevice adapter = new USB_Adapter_HW(serial);

                USB_Power_Supply_HW hw = new USB_Power_Supply_HW(adapter);

                CommandProcessor processor = new CommandProcessor(hw);

                ConsoleLogger consoleLogger = new ConsoleLogger();

                PowerSupplyService powerService = new PowerSupplyService(processor);

                comboBoxPortsList.Items.Clear();

                string[] ports = SerialPort.GetPortNames();

                foreach (string port in ports)
                {
                    comboBoxPortsList.Items.Add(port);
                }
                PSViewModel dataContext = new PSViewModel(powerService, this, consoleLogger);
                if (ports.Length > 0)
                {
                    dataContext.SelectedPortName = ports[0];
                }
                dataContext.DeviceConnectedStatus = false;

                var adjAxis = VPPlot.Plot.Axes.GetYAxes();
                adjAxis.First().IsVisible = false;
                adjAxis = VNPlot.Plot.Axes.GetYAxes();
                adjAxis.First().IsVisible = false;
                adjAxis = V3Plot.Plot.Axes.GetYAxes();
                adjAxis.First().IsVisible = false;
                adjAxis = V2Plot.Plot.Axes.GetYAxes();
                adjAxis.First().IsVisible = false;

                foreach (PlotTrace signal in dataContext.plotPositive.Traces)
                {
                    DataLogger logger = VPPlot.Plot.Add.DataLogger();
                    var axisY = VPPlot.Plot.Axes.AddLeftAxis();
                    axisY.LabelText = signal.Name;
                    axisY.IsVisible = signal.Visible;
                    logger.IsVisible = signal.Visible;
                    logger.Axes.YAxis = axisY;
                    logger.LegendText = signal.Name;
                    logger.Axes.YAxis.IsVisible = signal.Visible;
                    logger.Color = signal.TraceColor;
                    logger.ManageAxisLimits = true;
                    logger.AxisManager = axisManager;
                    signal.Logger = logger;
                    signal.PropertyChanged +=
                    (s, e) =>
                    {
                        if (e.PropertyName == nameof(PlotTrace.Visible))
                        {
                            logger.Axes.YAxis.IsVisible = signal.Visible;
                            logger.IsVisible = signal.Visible;
                            VPPlot.Refresh();
                        }
                    };

                }

                foreach (PlotTrace signal in dataContext.plotNegative.Traces)
                {
                    DataLogger logger = VNPlot.Plot.Add.DataLogger();
                    var axisY = VNPlot.Plot.Axes.AddLeftAxis();
                    axisY.LabelText = signal.Name;
                    axisY.IsVisible = signal.Visible;
                    logger.IsVisible = signal.Visible;
                    logger.Axes.YAxis = axisY;
                    logger.LegendText = signal.Name;
                    logger.Axes.YAxis.IsVisible = signal.Visible;
                    logger.Color = signal.TraceColor;
                    logger.ManageAxisLimits = true;
                    logger.AxisManager = axisManager;
                    signal.Logger = logger;
                    signal.PropertyChanged +=
                    (s, e) =>
                    {
                        if (e.PropertyName == nameof(PlotTrace.Visible))
                        {
                            logger.Axes.YAxis.IsVisible = signal.Visible;
                            logger.IsVisible = signal.Visible;
                            VNPlot.Refresh();
                        }
                    };
                }

                foreach (PlotTrace signal in dataContext.plot3V3.Traces)
                {
                    DataLogger logger = V3Plot.Plot.Add.DataLogger();
                    var axisY = V3Plot.Plot.Axes.AddLeftAxis();
                    axisY.LabelText = signal.Name;
                    axisY.LabelText = signal.Name;
                    axisY.IsVisible = signal.Visible;
                    logger.IsVisible = signal.Visible;
                    logger.Axes.YAxis = axisY;
                    logger.LegendText = signal.Name;
                    logger.Axes.YAxis.IsVisible = signal.Visible;
                    logger.Color = signal.TraceColor;
                    logger.ManageAxisLimits = true;
                    logger.AxisManager = axisManager;
                    signal.Logger = logger;
                    signal.PropertyChanged +=
                    (s, e) =>
                    {
                        if (e.PropertyName == nameof(PlotTrace.Visible))
                        {
                            logger.Axes.YAxis.IsVisible = signal.Visible;
                            logger.IsVisible = signal.Visible;
                            V3Plot.Refresh();
                        }
                    };
                }

                foreach (PlotTrace signal in dataContext.plot2V5.Traces)
                {
                    DataLogger logger = V2Plot.Plot.Add.DataLogger();
                    var axisY = V2Plot.Plot.Axes.AddLeftAxis();
                    axisY.LabelText = signal.Name;
                    axisY.LabelText = signal.Name;
                    axisY.IsVisible = signal.Visible;
                    logger.IsVisible = signal.Visible;
                    logger.Axes.YAxis = axisY;
                    logger.LegendText = signal.Name;
                    logger.Axes.YAxis.IsVisible = signal.Visible;
                    logger.Color = signal.TraceColor;
                    logger.ManageAxisLimits = true;
                    logger.AxisManager = axisManager;
                    signal.Logger = logger;
                    signal.PropertyChanged +=
                    (s, e) =>
                    {
                        if (e.PropertyName == nameof(PlotTrace.Visible))
                        {
                            logger.Axes.YAxis.IsVisible = signal.Visible;
                            logger.IsVisible = signal.Visible;
                            V2Plot.Refresh();
                        }
                    };
                }

                VPPlot.UserInputProcessor.Disable();
                VNPlot.UserInputProcessor.Disable();
                V3Plot.UserInputProcessor.Disable();
                V2Plot.UserInputProcessor.Disable();

                VPPlot.Plot.ShowLegend(ScottPlot.Alignment.UpperRight);
                V3Plot.Plot.ShowLegend(ScottPlot.Alignment.UpperRight);
                V2Plot.Plot.ShowLegend(ScottPlot.Alignment.UpperRight);
                VNPlot.Plot.ShowLegend(ScottPlot.Alignment.UpperRight);

                VPPlot.Refresh();
                VNPlot.Refresh();
                V3Plot.Refresh();
                V2Plot.Refresh();

                VPPlot.Plot.Axes.ContinuouslyAutoscale = true;
                VNPlot.Plot.Axes.ContinuouslyAutoscale = true;
                V3Plot.Plot.Axes.ContinuouslyAutoscale = true;
                V2Plot.Plot.Axes.ContinuouslyAutoscale = true;

                timerUpdate.Tick += (s, e) =>
                {
                    VPPlot.Refresh();
                    VNPlot.Refresh();
                    V3Plot.Refresh();
                    V2Plot.Refresh();
                };

                timerUpdate.Start();

                this.DataContext = dataContext;

                dataContext.Logger.Add(ConsoleMessage.LogLevel.Info, "App", "Application Loaded.");
            }
            catch(Exception ex)
            {
                File.WriteAllText("crash.txt", ex.ToString());
                throw;
            }
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    WindowState = WindowState == WindowState.Maximized
                        ? WindowState.Normal
                        : WindowState.Maximized;

                    return;
                }

                if (this.WindowState == WindowState.Maximized)
                {
                    Point mouseScreen = PointToScreen(e.GetPosition(this));

                    double restoreWidth = RestoreBounds.Width;
                    double restoreHeight = RestoreBounds.Height;

                    // Horizontal position of mouse within maximized window
                    double percentX = e.GetPosition(this).X / ActualWidth;

                    WindowState = WindowState.Normal;

                    Left = mouseScreen.X - (restoreWidth * percentX);

                    // Keep mouse on title bar after restore
                    Top = mouseScreen.Y - 20;
                }

                this.DragMove();
            }
        }

        private void buttonMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                // Restore down to normal windowed mode
                this.WindowState = WindowState.Normal;

                // Change button icon back to the Maximize symbol (Square)
                buttonMaximize.Content = "\uE922";
            }
            else
            {
                // Maximize the window
                this.WindowState = WindowState.Maximized;

                // Change button icon to the Restore symbol (Overlapping Squares)
                buttonMaximize.Content = "\uE923";
            }
        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (buttonMaximize == null) return;

            if (this.WindowState == WindowState.Maximized)
            {
                buttonMaximize.Content = "\uE923"; // Shows Restore icon
            }
            else if (this.WindowState == WindowState.Normal)
            {
                buttonMaximize.Content = "\uE922"; // Shows Maximize icon
            }
        }
        static bool lim_p, lim_n, lim_3, lim_2 = false;
        public void addPlotLim(object Plot, double limLow, double limHigh)
        {
            switch (Plot)
            {
                case "P":
                    if(!lim_p)
                    {
                        VPPlot.Plot.Add.HorizontalLine(limLow);
                        VPPlot.Plot.Add.HorizontalLine(limHigh);
                        VPPlot.Refresh();
                        lim_p = true;
                    }
                    else
                    {
                        foreach (HorizontalLine line in VPPlot.Plot.GetPlottables().OfType<HorizontalLine>().ToList())
                        {
                            VPPlot.Plot.Remove(line);
                        }
                        lim_p = false;
                    }
                    break;
                case "N":
                    if(!lim_n)
                    {
                        VNPlot.Plot.Add.HorizontalLine(limLow);
                        VNPlot.Plot.Add.HorizontalLine(limHigh);
                        VNPlot.Refresh();
                        lim_n = true;
                    }
                    else
                    {
                        foreach (HorizontalLine line in VNPlot.Plot.GetPlottables().OfType<HorizontalLine>().ToList())
                        {
                            VNPlot.Plot.Remove(line);
                        }
                        lim_n = false;
                    }
                    break;
                case "3":
                    if(!lim_3)
                    {
                        V3Plot.Plot.Add.HorizontalLine(limLow);
                        V3Plot.Plot.Add.HorizontalLine(limHigh);
                        V3Plot.Refresh();
                        lim_3 = true;
                    }
                    else
                    {
                        foreach (HorizontalLine line in V3Plot.Plot.GetPlottables().OfType<HorizontalLine>().ToList())
                        {
                            V3Plot.Plot.Remove(line);
                        }
                        lim_3 = false;
                    }
                    break;
                case "2":
                    if(!lim_2)
                    {
                        V2Plot.Plot.Add.HorizontalLine(limLow);
                        V2Plot.Plot.Add.HorizontalLine(limHigh);
                        V2Plot.Refresh();
                        lim_2 = true;
                    }
                    else
                    {
                        foreach (HorizontalLine line in V2Plot.Plot.GetPlottables().OfType<HorizontalLine>().ToList())
                        {
                            V2Plot.Plot.Remove(line);
                        }
                        lim_2 = false;
                    }
                    break;
                default:
                    return;
            }
        }

        //Code below added for compatibility with lower resolution displays, will start in full-screen mode if resolution is <= 1920x1080
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var area = GetCurrentMonitorWorkArea(this);

                if (area.Width <= 1920 || area.Height <= 1080)
                {
                    WindowState = WindowState.Maximized;
                }
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
        }

        public static Rect GetCurrentMonitorWorkArea(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;

            const int MONITOR_DEFAULTTONEAREST = 2;

            IntPtr monitor = NativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            MONITORINFO info = new MONITORINFO();
            info.cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            NativeMethods.GetMonitorInfo(monitor, ref info);

            return new Rect(
                info.rcWork.Left,
                info.rcWork.Top,
                info.rcWork.Right - info.rcWork.Left,
                info.rcWork.Bottom - info.rcWork.Top
            );
        }

        private const double AspectRatio = 1920.0 / 1080.0;

        private bool _resizing;

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_resizing)
                return;

            _resizing = true;

            Width = Height * AspectRatio;

            _resizing = false;
        }
    }
}