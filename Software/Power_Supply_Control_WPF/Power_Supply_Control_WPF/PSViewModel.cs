using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Power_Supply_Control_WPF.Services;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Power_Supply_Control_WPF
{
    public class PSViewModel : INotifyPropertyChanged
    {
        private bool ConsoleDebugMode = true;
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly PowerSupplyService _powerSupplyService;
        private bool autoUpdates = false;
        public ObservableCollection<MeasurementRow> Measurements { get; }  = new();

        public IAsyncRelayCommand ToggleVPCommand { get; }

        public IAsyncRelayCommand ToggleVNCommand { get; }

        public IAsyncRelayCommand ToggleV3V3Command { get; }

        public IAsyncRelayCommand ToggleV2V5Command { get; }

        public IAsyncRelayCommand ConnectToTarget { get; }

        public IAsyncRelayCommand SetPositiveSupply { get; }

        public IAsyncRelayCommand SetNegativeSupply { get; }

        public IAsyncRelayCommand SaveVP { get; }
        public IAsyncRelayCommand SaveVN { get; }
        public IAsyncRelayCommand SaveV3 { get; }
        public IAsyncRelayCommand SaveV2 { get; }

        public IAsyncRelayCommand GetUpdate {  get; }

        public IAsyncRelayCommand ClearGraphPositive { get; }
        public IAsyncRelayCommand ClearGraphNegative { get; }
        public IAsyncRelayCommand ClearGraph3V3 { get; }
        public IAsyncRelayCommand ClearGraph2V5 { get; }

        public IAsyncRelayCommand PopOutGraphPositive { get; }
        public IAsyncRelayCommand PopOutGraphNegative { get; }
        public IAsyncRelayCommand PopOutGraph3V3 { get; }
        public IAsyncRelayCommand PopOutGraph2V5 { get; }

        public IAsyncRelayCommand AddLimtsPositive { get; }
        public IAsyncRelayCommand AddLimtsNegative { get; }
        public IAsyncRelayCommand AddLimts3V3 { get; }
        public IAsyncRelayCommand AddLimts2V5 { get; }

        public IAsyncRelayCommand CurrentLimitP { get; }
        public IAsyncRelayCommand CurrentLimitN { get; }

        public IAsyncRelayCommand IncrementVoltageP { get; }
        public IAsyncRelayCommand IncrementVoltageN { get; }
        public IAsyncRelayCommand DecrementVoltageP { get; }
        public IAsyncRelayCommand DecrementVoltageN { get; }


        public MeasurementRow SYSTEM;
        public MeasurementRow USB;
        public MeasurementRow V5;
        public MeasurementRow V33;
        public MeasurementRow V25;
        public MeasurementRow VP;
        public MeasurementRow VN;

        System.Windows.Threading.DispatcherTimer timerAutoUpdate = new() { Interval = TimeSpan.FromMilliseconds(100) };

        private Power_Supply_Control_WPF.Services.AxisManager axisManager = new();
        private readonly MainWindow _mainWindow;
        public ConsoleLogger Logger { get; }

        public PSViewModel(PowerSupplyService powerSupplyService, MainWindow main_window, ConsoleLogger logger)
        {
            _powerSupplyService = powerSupplyService;
            _mainWindow = main_window;
            Logger = logger;
            ToggleVPCommand = new AsyncRelayCommand(ToggleVP);
            ToggleVNCommand = new AsyncRelayCommand(ToggleVN);
            ToggleV3V3Command = new AsyncRelayCommand(ToggleV3);
            ToggleV2V5Command = new AsyncRelayCommand(ToggleV2);
            ConnectToTarget = new AsyncRelayCommand(Connect);
            SetPositiveSupply = new AsyncRelayCommand(SetVP);
            SetNegativeSupply = new AsyncRelayCommand(SetVN);
            SaveVP = new AsyncRelayCommand(SaveVPFile);
            SaveVN = new AsyncRelayCommand(SaveVNFile);
            SaveV3 = new AsyncRelayCommand(SaveV3File);
            SaveV2 = new AsyncRelayCommand(SaveV2File);
            GetUpdate = new AsyncRelayCommand(UpdateMeasurements);
            ClearGraphPositive = new AsyncRelayCommand(ClearGraphP);
            ClearGraphNegative = new AsyncRelayCommand(ClearGraphN);
            ClearGraph3V3 = new AsyncRelayCommand(ClearGraph3);
            ClearGraph2V5 = new AsyncRelayCommand(ClearGraph2);
            PopOutGraphPositive = new AsyncRelayCommand(PopOutGraphP);
            PopOutGraphNegative = new AsyncRelayCommand(PopOutGraphN);
            PopOutGraph3V3 = new AsyncRelayCommand(PopOutGraph3);
            PopOutGraph2V5 = new AsyncRelayCommand(PopOutGraph2);
            AddLimtsPositive = new AsyncRelayCommand(AddPlotLimP);
            AddLimtsNegative = new AsyncRelayCommand(AddPlotLimN);
            AddLimts3V3 = new AsyncRelayCommand(AddPlotLim3);
            AddLimts2V5 = new AsyncRelayCommand(AddPlotLim2);
            CurrentLimitP = new AsyncRelayCommand(setILimP);
            CurrentLimitN = new AsyncRelayCommand(setILimN);
            IncrementVoltageP = new AsyncRelayCommand(IncrementVP);
            IncrementVoltageN = new AsyncRelayCommand(IncrementVN);
            DecrementVoltageP = new AsyncRelayCommand(DecrementVP);
            DecrementVoltageN = new AsyncRelayCommand(DecrementVN);

            plotPositive = CreatePlot("Positive Supply");
            plotNegative = CreatePlot("Negative Supply");
            plot3V3 = CreatePlot("3V3 Supply");
            plot2V5 = CreatePlot("2V5 Supply");

            SYSTEM = new MeasurementRow { Name = "SYSTEM", Voltage = 0, Current = 0 };
            USB = new MeasurementRow { Name = "USB", Voltage = 0, Current = 0 };
            V5 = new MeasurementRow { Name = "5V", Voltage = 0, Current = 0 };
            V33 = new MeasurementRow { Name = "3.3V", Voltage = 0, Current = 0 };
            V25 = new MeasurementRow { Name = "2.5V", Voltage = 0, Current = 0 };
            VP = new MeasurementRow { Name = "VPOSITIVE", Voltage = 0, Current = 0 };
            VN = new MeasurementRow { Name = "VNEGATIVE", Voltage = 0, Current = 0 };

            Measurements.Add(SYSTEM);
            Measurements.Add(USB);
            Measurements.Add(V5);
            Measurements.Add(V33);
            Measurements.Add(V25);
            Measurements.Add(VP);
            Measurements.Add(VN);

            timerAutoUpdate.Tick += async (s, e) =>
            {
                await UpdateMeasurements();
            };

            Logger.Messages.CollectionChanged +=
                (s, e) =>
                {
                    _mainWindow.Dispatcher.BeginInvoke(
                        new Action(() =>
                        {
                            if (_mainWindow.ConsoleBox.Items.Count > 0)
                            {
                                _mainWindow.ConsoleBox.ScrollIntoView(_mainWindow.ConsoleBox.Items[_mainWindow.ConsoleBox.Items.Count - 1]);
                            }
                        }),
                        System.Windows.Threading.DispatcherPriority.Background);
                };
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private double _voltagePositive;
        private double _voltageNegative;
        private double VLIM_P_H = 20;
        private double VLIM_P_L = 0;
        private double VLIM_N_H = 0;
        private double VLIM_N_L = -20;
        private double _currentLimP = 4000;
        private double _currentLimN = 1000;

        private bool vpEnabled;
        private bool vnEnabled;
        private bool v3Enabled;
        private bool v2Enabled;

        private bool? deviceConnected;
        private string? deviceCOMPort;

        public double ILIM_P
        {
            get => _currentLimP;
            set
            {
                if (_currentLimP != value)
                {
                    _currentLimP = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ILIM_N
        {
            get => _currentLimN;
            set
            {
                if (_currentLimN != value)
                {
                    _currentLimN = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Voltage_Positive
        {
            get => _voltagePositive;
            set
            {
                if (_voltagePositive != value && value <= VLIM_P_H && value >= VLIM_P_L)
                {
                    _voltagePositive = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Voltage_Negative
        {
            get => _voltageNegative;
            set
            {
                if (_voltageNegative != value && value <= 0 && value >= -20)
                {
                    _voltageNegative = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Voltage_Positive_Limit_High
        {
            get => VLIM_P_H;
            set
            {
                if (VLIM_P_H != value && value <= 20 && value >= VLIM_P_L)
                {
                    VLIM_P_H = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Voltage_Positive_Limit_Low
        {
            get => VLIM_P_L;
            set
            {
                if (VLIM_P_L != value && value <= VLIM_P_H && value >= 0)
                {
                    VLIM_P_L = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Voltage_Negative_Limit_High
        {
            get => VLIM_N_H;
            set
            {
                if (VLIM_N_H != value && value <= 0 && value >= VLIM_N_L)
                {
                    VLIM_N_H = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Voltage_Negative_Limit_Low
        {
            get => VLIM_N_L;
            set
            {
                if (VLIM_N_L != value && value < VLIM_N_H && value >= -20)
                {
                    VLIM_N_L = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? SelectedPortName
        {
            get => deviceCOMPort;
            set
            {
                if (deviceCOMPort != value)
                {
                    deviceCOMPort = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool VPEnabled
        {
            get => vpEnabled;
            set
            {
                if (vpEnabled != value)
                {
                    vpEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool VNEnabled
        {
            get => vnEnabled;
            set
            {
                if (vnEnabled != value)
                {
                    vnEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool V3Enabled
        {
            get => v3Enabled;
            set
            {
                if (v3Enabled != value)
                {
                    v3Enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool V2Enabled
        {
            get => v2Enabled;
            set
            {
                if (v2Enabled != value)
                {
                    v2Enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DeviceConnectedStatus
        {
            get => deviceConnected == true;
            set
            {
                if (deviceConnected != value)
                {
                    deviceConnected = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AutoUpdateChecked
        {
            get => autoUpdates;
            set
            {
                if (autoUpdates != value)
                {
                    autoUpdates = value;
                    if (autoUpdates)
                    {
                        timerAutoUpdate.Start();
                        if(ConsoleDebugMode)
                        {
                            Logger.Add(ConsoleMessage.LogLevel.Measurement, "App", "Updates started.");
                        }
                    }
                    else
                    {
                        timerAutoUpdate.Stop();
                        if (ConsoleDebugMode)
                        {
                            Logger.Add(ConsoleMessage.LogLevel.Measurement, "App", "Updates stopped.");
                        }
                    }
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<MeasurementSample> dataSourcePositiveSupply { get; } = new();
        public ObservableCollection<MeasurementSample> dataSourceNegativeSupply { get; } = new();
        public ObservableCollection<MeasurementSample> dataSource3V3Supply { get; } = new();
        public ObservableCollection<MeasurementSample> dataSource2V5Supply { get; } = new();

        public PlotSource plotPositive { get; }
        public PlotSource plotNegative { get; }
        public PlotSource plot3V3 { get; }
        public PlotSource plot2V5 { get; }

        private async Task ToggleVP()
        {
            if(deviceConnected == true)
            {
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Sent", $"VPos:{(VPEnabled ? "ENABLED" : "DISABLED")}");
                }
                VPEnabled = await _powerSupplyService.ToggleVP();
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Received", "");
                }
                Logger.Add(ConsoleMessage.LogLevel.Command, "Toggle", $"VPos:{(VPEnabled ? "ENABLED" : "DISABLED")}");
            }
        }

        private async Task ToggleVN()
        {
            if (deviceConnected == true)
            {
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Sent", $"VNeg:{(VPEnabled ? "ENABLED" : "DISABLED")}");
                }
                VNEnabled = await _powerSupplyService.ToggleVN();
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Received", "");
                }
                Logger.Add(ConsoleMessage.LogLevel.Command, "Toggle", $"VNeg:{(VNEnabled ? "ENABLED" : "DISABLED")}");
            }
        }

        private async Task ToggleV3()
        {
            if (deviceConnected == true)
            {
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Sent", $"V3.3:{(VPEnabled ? "ENABLED" : "DISABLED")}");
                }
                V3Enabled = await _powerSupplyService.ToggleV3();
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Received", "");
                }
                Logger.Add(ConsoleMessage.LogLevel.Command, "Toggle", $"V3.3:{(V3Enabled ? "ENABLED" : "DISABLED")}");
            }
        }

        private async Task ToggleV2()
        {
            if (deviceConnected == true)
            {
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Sent", $"V2.5:{(VPEnabled ? "ENABLED" : "DISABLED")}");
                }
                V2Enabled = await _powerSupplyService.ToggleV2();
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Received", "");
                }
                Logger.Add(ConsoleMessage.LogLevel.Command, "Toggle", $"V2.5:{(V2Enabled ? "ENABLED" : "DISABLED")}");
            }
        }

        private async Task SetVP()
        {
            if (deviceConnected == true)
            {
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Sent", $"VPSET");
                }
                await _powerSupplyService.SetVP((float)_voltagePositive);
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Received", "");
                }
                Logger.Add(ConsoleMessage.LogLevel.Command, "Config", $"VPos;V Set to: {(float)_voltagePositive:F3} Volts");
            }
        }

        private async Task GetVP()
        {
            if (deviceConnected == true)
            {
                float? VPositive = await _powerSupplyService.ReadVPVoltage();
            }
        }

        private async Task SetVN()
        {
            if (deviceConnected == true)
            {
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Sent", $"VNSET");
                }
                await _powerSupplyService.SetVN((float)_voltageNegative);
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Received", "");
                }
                Logger.Add(ConsoleMessage.LogLevel.Command, "Config", $"VNeg;V Set to: {(float)_voltagePositive:F3} Volts");
            }
        }

        private async Task GetVN()
        {
            if (deviceConnected == true)
            {
                float? VNegative = await _powerSupplyService.ReadVNVoltage();
            }
        }

        private async Task GetV3()
        {
            if (deviceConnected == true)
            {
                float? V3 = await _powerSupplyService.ReadV3Voltage();
            }
        }

        private async Task GetV2()
        {
            if (deviceConnected == true)
            {
                float? V2 = await _powerSupplyService.ReadV2Voltage();
            }
        }

        private async Task setILimP()
        {
            if (deviceConnected == true)
            {
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Sent", $"IPSET");
                }
                await _powerSupplyService.setIP((float)_currentLimP);
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Received", "");
                }
                Logger.Add(ConsoleMessage.LogLevel.Command, "Config", $"VPos;Current Limit Set to: {(float)_currentLimP:F3} mA");
            }
        }

        private async Task setILimN()
        {
            if (deviceConnected == true)
            {
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Sent", $"INSET");
                }
                await _powerSupplyService.setIN((float)_currentLimN);
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Received", "");
                }
                Logger.Add(ConsoleMessage.LogLevel.Command, "Config", $"VNeg;Current Limit Set to: {(float)_currentLimN:F3} mA");
            }
        }

        private int ReadIndex = 0;
        private async Task ReadNext()
        {
            if(deviceConnected == true)
            {
                switch(ReadIndex)
                {
                    case 0:
                        float? VP = await _powerSupplyService.ReadVPVoltage();
                        break;
                    default:
                        break;
                }
                if(ReadIndex == 1)
                {

                }
                ++ReadIndex;
            }
        }

        private async Task CheckID()
        {
            if(deviceConnected == true)
            {
                string? ID_DEVICE = await _powerSupplyService.CheckDeviceID();
            }
        }

        private async Task Connect()
        {
            if (deviceCOMPort == null) return;

            if (deviceConnected == false)
            {
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "App", "Connecting to " + deviceCOMPort);
                }
                bool? Connected = await _powerSupplyService.ConnectToDevice(deviceCOMPort);
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, deviceCOMPort, $"{(Connected == true ? "Conncted" : "Error")}");
                }
                this.DeviceConnectedStatus = Connected == true;
                Logger.Add(ConsoleMessage.LogLevel.Info, "App", "Device Connected.");
            }
            else
            {
                await _powerSupplyService.Disconnect();
                this.DeviceConnectedStatus = false;
                Logger.Add(ConsoleMessage.LogLevel.Info, "App", "Device Disconnected.");
            }
        }

        private bool _isUpdating = false;
        static DateTime startMeasTime = DateTime.MinValue;
        static DateTime startMeasTime2 = DateTime.MinValue;
        static DateTime startMeasTime3 = DateTime.MinValue;
        static DateTime startMeasTime4 = DateTime.MinValue;
        private async Task UpdateMeasurements()
        {
            if (deviceCOMPort == null) return;

            if (_isUpdating)
            {
                //Logger.Add(ConsoleMessage.LogLevel.Warning, "App", "Thread called too soon.");
                return;
            }

            if (deviceConnected == true)
            {
                _isUpdating = true;
                if(startMeasTime == DateTime.MinValue) startMeasTime = DateTime.Now;
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Updating", "VP,IP");
                }
                float vp = (float)await _powerSupplyService.ReadVPVoltage();
                float ip = (float)await _powerSupplyService.ReadCurrent(USB_Power_Supply_Application.Hardware_Interface.USB_Power_Supply_HW.Current_Sources.I_Positive);
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Response", "VP,IP");
                }
                float pp = vp * ip;
                MeasurementSample sp = new() { Timestamp = DateTime.Now, Voltage = vp, Current = ip, Power = pp, };
                dataSourcePositiveSupply.Add(sp);
                double t = sp.Timestamp.ToOADate();
                TimeSpan duration = DateTime.Now - startMeasTime;
                t = duration.TotalMilliseconds;
                if (plotPositive.Traces[0].Visible || plotPositive.Traces[1].Visible || plotPositive.Traces[2].Visible)
                {
                    plotPositive.Traces[0].AddPoint(t, vp);
                    plotPositive.Traces[1].AddPoint(t, ip);
                    plotPositive.Traces[2].AddPoint(t, pp);
                }
                VP.Voltage = vp;
                VP.Current = ip;
                if (startMeasTime2 == DateTime.MinValue) startMeasTime2 = DateTime.Now;
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Updating", "VN,IN");
                }
                float vn = (float)await _powerSupplyService.ReadVNVoltage();
                float ineg = (float)await _powerSupplyService.ReadCurrent(USB_Power_Supply_Application.Hardware_Interface.USB_Power_Supply_HW.Current_Sources.I_Negative);
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Response", "VN,IN");
                }
                float pn = vn * ineg;
                MeasurementSample sn = new() { Timestamp = DateTime.Now, Voltage = vn, Current = ineg, Power = pn };
                dataSourceNegativeSupply.Add(sn);
                duration = DateTime.Now - startMeasTime2;
                t = duration.TotalMilliseconds;
                if (plotNegative.Traces[0].Visible || plotNegative.Traces[1].Visible || plotNegative.Traces[2].Visible)
                {
                    plotNegative.Traces[0].AddPoint(t, vn);
                    plotNegative.Traces[1].AddPoint(t, ineg);
                    plotNegative.Traces[2].AddPoint(t, pn);
                }
                VN.Voltage = vn;
                VN.Current = ineg;
                if (startMeasTime3 == DateTime.MinValue) startMeasTime3 = DateTime.Now;
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Updating", "V3,I3");
                }
                float v3 = (float)await _powerSupplyService.ReadV3Voltage();
                float i3 = (float)await _powerSupplyService.ReadCurrent(USB_Power_Supply_Application.Hardware_Interface.USB_Power_Supply_HW.Current_Sources.I_3v3);
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Response", "V3,I3");
                }
                float p3 = v3 * i3;
                MeasurementSample s3 = new() { Timestamp = DateTime.Now, Voltage = v3, Current = i3, Power = p3 };
                dataSource3V3Supply.Add(s3);
                duration = DateTime.Now - startMeasTime3;
                t = duration.TotalMilliseconds;
                if (plot3V3.Traces[0].Visible || plot3V3.Traces[1].Visible || plot3V3.Traces[2].Visible)
                {
                    plot3V3.Traces[0].AddPoint(t, v3);
                    plot3V3.Traces[1].AddPoint(t, i3);
                    plot3V3.Traces[2].AddPoint(t, p3);
                }
                V33.Voltage = v3;
                V33.Current = i3;
                if (startMeasTime4 == DateTime.MinValue) startMeasTime4 = DateTime.Now;
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Updating", "V2,I2");
                }
                float v2 = (float)await _powerSupplyService.ReadV2Voltage();
                float i2 = (float)await _powerSupplyService.ReadCurrent(USB_Power_Supply_Application.Hardware_Interface.USB_Power_Supply_HW.Current_Sources.I_2v5);
                if (ConsoleDebugMode)
                {
                    Logger.Add(ConsoleMessage.LogLevel.Debug, "Response", "V2,I2");
                }
                float p2 = v2 * i2;
                MeasurementSample s2 = new() { Timestamp = DateTime.Now, Voltage = v2, Current = i2, Power = p2 };
                dataSource2V5Supply.Add(s2);
                duration = DateTime.Now - startMeasTime4;
                t = duration.TotalMilliseconds;
                if (plot2V5.Traces[0].Visible || plot2V5.Traces[1].Visible || plot2V5.Traces[2].Visible)
                {
                    plot2V5.Traces[0].AddPoint(t, v2);
                    plot2V5.Traces[1].AddPoint(t, i2);
                    plot2V5.Traces[2].AddPoint(t, p2);
                }
                V25.Voltage = v2;
                V25.Current = i2;
                _isUpdating = false;
                //Logger.Add(ConsoleMessage.LogLevel.Measurement, "App", "Updates completed.");
            }
        }

        private PlotSource CreatePlot(string name)
        {
            PlotSource p =  new() { Name = name };

            p.Traces.Add(new PlotTrace()
                {
                    Name = "Voltage",
                    Visible = false,
                    TraceColor = ScottPlot.Color.FromSDColor(System.Drawing.Color.MediumBlue),
                });

            p.Traces.Add(new PlotTrace()
                {
                    Name = "Current",
                    Visible = false,
                    TraceColor = ScottPlot.Color.FromSDColor(System.Drawing.Color.OrangeRed),
            });

            p.Traces.Add(new PlotTrace()
                {
                    Name = "Power",
                    Visible = false,
                    TraceColor = ScottPlot.Color.FromSDColor(System.Drawing.Color.Green),
            });

            return p;
        }

        public async Task SaveAllCSV(string filename, PlotSource plot)
        {
            await using StreamWriter sw = new(filename, false);

            await sw.WriteLineAsync("Milliseconds," + "Volts,Amps,Watts,");

            var vpV = plot.VoltageTrace.Logger!.Data.Coordinates;

            var vpI = plot.CurrentTrace.Logger!.Data.Coordinates;

            var vpP = plot.PowerTrace.Logger!.Data.Coordinates;

            int count = vpV.Count;

            for (int idx = 0; idx < count; idx++)
            {
                await sw.WriteLineAsync($"{vpV[idx].X}," + $"{vpV[idx].Y}," + $"{vpI[idx].Y}," + $"{vpP[idx].Y}");
            }
            Logger.Add(ConsoleMessage.LogLevel.Info, "App", $"File '{filename}' Saved.");
        }

        public async Task SaveVPFile()
        {
            SaveFileDialog dlg = new();

            dlg.Filter = "CSV Files (*.csv)|*.csv";

            dlg.DefaultExt = ".csv";

            dlg.FileName = $"Positive_Supply_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            bool? result = dlg.ShowDialog();

            if (result != true)
                return;

            await SaveAllCSV(dlg.FileName, plotPositive);
        }

        public async Task SaveVNFile()
        {
            SaveFileDialog dlg = new();

            dlg.Filter = "CSV Files (*.csv)|*.csv";

            dlg.DefaultExt = ".csv";

            dlg.FileName = $"Negative_Supply_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            bool? result = dlg.ShowDialog();

            if (result != true)
                return;

            await SaveAllCSV(dlg.FileName, plotNegative);
        }

        public async Task SaveV3File()
        {
            SaveFileDialog dlg = new();

            dlg.Filter = "CSV Files (*.csv)|*.csv";

            dlg.DefaultExt = ".csv";

            dlg.FileName = $"3V3_Supply_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            bool? result = dlg.ShowDialog();

            if (result != true)
                return;

            await SaveAllCSV(dlg.FileName, plot3V3);
        }

        public async Task SaveV2File()
        {
            SaveFileDialog dlg = new();

            dlg.Filter = "CSV Files (*.csv)|*.csv";

            dlg.DefaultExt = ".csv";

            dlg.FileName = $"2V5_Supply_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            bool? result = dlg.ShowDialog();

            if (result != true)
                return;

            await SaveAllCSV(dlg.FileName, plot2V5);
        }

        private Task ClearGraphP()
        {
            startMeasTime = DateTime.MinValue;
            plotPositive.VoltageTrace.Clear();
            plotPositive.CurrentTrace.Clear();
            plotPositive.PowerTrace.Clear();
            Logger.Add(ConsoleMessage.LogLevel.Info, "App", "Graph Cleared.");
            return Task.CompletedTask;
        }

        private Task ClearGraphN()
        {
            startMeasTime2 = DateTime.MinValue;
            plotNegative.VoltageTrace.Clear();
            plotNegative.CurrentTrace.Clear();
            plotNegative.PowerTrace.Clear();
            Logger.Add(ConsoleMessage.LogLevel.Info, "App", "Graph Cleared.");
            return Task.CompletedTask;
        }

        private Task ClearGraph3()
        {
            startMeasTime3 = DateTime.MinValue;
            plot3V3.VoltageTrace.Clear();
            plot3V3.CurrentTrace.Clear();
            plot3V3.PowerTrace.Clear();
            Logger.Add(ConsoleMessage.LogLevel.Info, "App", "Graph Cleared.");
            return Task.CompletedTask;
        }

        private Task ClearGraph2()
        {
            startMeasTime4 = DateTime.MinValue;
            plot2V5.VoltageTrace.Clear();
            plot2V5.CurrentTrace.Clear();
            plot2V5.PowerTrace.Clear();
            Logger.Add(ConsoleMessage.LogLevel.Info, "App", "Graph Cleared.");
            return Task.CompletedTask;
        }

        private Task AddPlotLimP()
        {
            foreach(PlotTrace signal in plotPositive.Traces)
            {
                if(signal.Visible)
                {
                    double max = signal.Logger.Data.Coordinates.Max(c => c.Y);
                    double min = signal.Logger.Data.Coordinates.Min(c => c.Y);
                    _mainWindow.addPlotLim("P", min, max);
                    Logger.Add(ConsoleMessage.LogLevel.Info, "App", $"Added plot limits for {signal.Name}");
                    Logger.Add(ConsoleMessage.LogLevel.Measurement, "Limits", $"Low: {max:F3}, High: {min:F3}");
                }
            }
            return Task.CompletedTask;
        }

        private Task AddPlotLimN()
        {
            foreach (PlotTrace signal in plotNegative.Traces)
            {
                if (signal.Visible)
                {
                    double max = signal.Logger.Data.Coordinates.Max(c => c.Y);
                    double min = signal.Logger.Data.Coordinates.Min(c => c.Y);
                    _mainWindow.addPlotLim("N", min, max);
                    Logger.Add(ConsoleMessage.LogLevel.Info, "App", $"Added plot limits for {signal.Name}");
                    Logger.Add(ConsoleMessage.LogLevel.Measurement, "Limits", $"Low: {max:F3}, High: {min:F3}");
                }
            }
            return Task.CompletedTask;
        }

        private Task AddPlotLim3()
        {
            foreach (PlotTrace signal in plot3V3.Traces)
            {
                if (signal.Visible)
                {
                    double max = signal.Logger.Data.Coordinates.Max(c => c.Y);
                    double min = signal.Logger.Data.Coordinates.Min(c => c.Y);
                    _mainWindow.addPlotLim("3", min, max);
                    Logger.Add(ConsoleMessage.LogLevel.Info, "App", $"Added plot limits for {signal.Name}");
                    Logger.Add(ConsoleMessage.LogLevel.Measurement, "Limits", $"Low: {max:F3}, High: {min:F3}");
                }
            }
            return Task.CompletedTask;
        }

        private Task AddPlotLim2()
        {
            foreach (PlotTrace signal in plot2V5.Traces)
            {
                if (signal.Visible)
                {
                    double max = signal.Logger.Data.Coordinates.Max(c => c.Y);
                    double min = signal.Logger.Data.Coordinates.Min(c => c.Y);
                    _mainWindow.addPlotLim("2", min, max);
                    Logger.Add(ConsoleMessage.LogLevel.Info, "App", $"Added plot limits for {signal.Name}");
                    Logger.Add(ConsoleMessage.LogLevel.Measurement, "Limits", $"Low: {max:F3}, High: {min:F3}");
                }
            }
            return Task.CompletedTask;
        }

        System.Windows.Threading.DispatcherTimer timerUpdateWindow = new() { Interval = TimeSpan.FromMilliseconds(300) };
        System.Windows.Threading.DispatcherTimer timerUpdateWindowN = new() { Interval = TimeSpan.FromMilliseconds(300) };
        System.Windows.Threading.DispatcherTimer timerUpdateWindow3 = new() { Interval = TimeSpan.FromMilliseconds(300) };
        System.Windows.Threading.DispatcherTimer timerUpdateWindow2 = new() { Interval = TimeSpan.FromMilliseconds(300) };

        private Task PopOutGraphP()
        {
            Window window = new Window();
            ScottPlot.WPF.WpfPlot myPlot = new();
            var adjAxis = myPlot.Plot.Axes.GetYAxes();
            adjAxis.First().IsVisible = false;
            foreach (PlotTrace signal in plotPositive.Traces)
            {
                DataLogger logger = myPlot.Plot.Add.DataLogger();
                var axisY = myPlot.Plot.Axes.AddLeftAxis();
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
                signal.PopUpLogger = logger;
                signal.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName == nameof(PlotTrace.Visible))
                    {
                        logger.Axes.YAxis.IsVisible = signal.Visible;
                        logger.IsVisible = signal.Visible;
                        myPlot.Refresh();
                    }
                };
            }
            myPlot.UserInputProcessor.Disable();
            myPlot.Plot.ShowLegend(ScottPlot.Alignment.UpperRight);
            myPlot.Refresh();
            myPlot.Plot.Axes.ContinuouslyAutoscale = true;

            timerUpdateWindow.Tick += (s, e) =>
            {
                myPlot.Refresh();

            };
            timerUpdateWindow.Start();

            window.Content = myPlot;
            window.Title = "Positive Supply";
            window.Closing += (s, e) =>
            {
                timerUpdateWindow.Stop();
            };
            window.Show();
            return Task.CompletedTask;
        }

        private Task PopOutGraphN()
        {
            Window window = new Window();
            ScottPlot.WPF.WpfPlot myPlot = new();
            var adjAxis = myPlot.Plot.Axes.GetYAxes();
            adjAxis.First().IsVisible = false;
            foreach (PlotTrace signal in plotNegative.Traces)
            {
                DataLogger logger = myPlot.Plot.Add.DataLogger();
                var axisY = myPlot.Plot.Axes.AddLeftAxis();
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
                signal.PopUpLogger = logger;
                signal.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName == nameof(PlotTrace.Visible))
                    {
                        logger.Axes.YAxis.IsVisible = signal.Visible;
                        logger.IsVisible = signal.Visible;
                        myPlot.Refresh();
                    }
                };
            }
            myPlot.UserInputProcessor.Disable();
            myPlot.Plot.ShowLegend(ScottPlot.Alignment.UpperRight);
            myPlot.Refresh();
            myPlot.Plot.Axes.ContinuouslyAutoscale = true;

            timerUpdateWindowN.Tick += (s, e) =>
            {
                myPlot.Refresh();

            };
            timerUpdateWindowN.Start();

            window.Content = myPlot;
            window.Title = "Negative Supply";
            window.Closing += (s, e) =>
            {
                timerUpdateWindowN.Stop();
            };
            window.Show();
            return Task.CompletedTask;
        }

        private Task PopOutGraph3()
        {
            Window window = new Window();
            ScottPlot.WPF.WpfPlot myPlot = new();
            var adjAxis = myPlot.Plot.Axes.GetYAxes();
            adjAxis.First().IsVisible = false;
            foreach (PlotTrace signal in plot3V3.Traces)
            {
                DataLogger logger = myPlot.Plot.Add.DataLogger();
                var axisY = myPlot.Plot.Axes.AddLeftAxis();
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
                signal.PopUpLogger = logger;
                signal.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName == nameof(PlotTrace.Visible))
                    {
                        logger.Axes.YAxis.IsVisible = signal.Visible;
                        logger.IsVisible = signal.Visible;
                        myPlot.Refresh();
                    }
                };
            }
            myPlot.UserInputProcessor.Disable();
            myPlot.Plot.ShowLegend(ScottPlot.Alignment.UpperRight);
            myPlot.Refresh();
            myPlot.Plot.Axes.ContinuouslyAutoscale = true;

            timerUpdateWindow3.Tick += (s, e) =>
            {
                myPlot.Refresh();

            };
            timerUpdateWindow3.Start();

            window.Content = myPlot;
            window.Title = "3.3V Supply";
            window.Closing += (s, e) =>
            {
                timerUpdateWindow3.Stop();
            };
            window.Show();
            return Task.CompletedTask;
        }

        private Task PopOutGraph2()
        {
            Window window = new Window();
            ScottPlot.WPF.WpfPlot myPlot = new();
            var adjAxis = myPlot.Plot.Axes.GetYAxes();
            adjAxis.First().IsVisible = false;
            foreach (PlotTrace signal in plot2V5.Traces)
            {
                DataLogger logger = myPlot.Plot.Add.DataLogger();
                var axisY = myPlot.Plot.Axes.AddLeftAxis();
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
                signal.PopUpLogger = logger;
                signal.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName == nameof(PlotTrace.Visible))
                    {
                        logger.Axes.YAxis.IsVisible = signal.Visible;
                        logger.IsVisible = signal.Visible;
                        myPlot.Refresh();
                    }
                };
            }
            myPlot.UserInputProcessor.Disable();
            myPlot.Plot.ShowLegend(ScottPlot.Alignment.UpperRight);
            myPlot.Refresh();
            myPlot.Plot.Axes.ContinuouslyAutoscale = true;

            timerUpdateWindow2.Tick += (s, e) =>
            {
                myPlot.Refresh();
            };
            timerUpdateWindow2.Start();

            window.Content = myPlot;
            window.Title = "2.5V Supply";
            window.Closing += (s, e) =>
            {
                timerUpdateWindow2.Stop();
            };
            window.Show();
            return Task.CompletedTask;
        }

        private float changeValue = 0.1f;

        private Task IncrementVP()
        {
            if(Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Voltage_Positive += 0.1*changeValue;
            }else
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                Voltage_Positive += 10 * changeValue;
            }
            else
                Voltage_Positive += changeValue;

            return Task.CompletedTask;
        }

        private Task IncrementVN()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Voltage_Negative += 0.1 * changeValue;
            }
            else
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                Voltage_Negative += 10 * changeValue;
            }
            else
                Voltage_Negative += changeValue;
            return Task.CompletedTask;
        }

        private Task DecrementVP()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Voltage_Positive -= 0.1 * changeValue;
            }
            else
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                Voltage_Positive -= 10 * changeValue;
            }
            else
                Voltage_Positive -= changeValue;
            return Task.CompletedTask;
        }

        private Task DecrementVN()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Voltage_Negative -= 0.1 * changeValue;
            }
            else
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                Voltage_Negative -= 10 * changeValue;
            }
            else
                Voltage_Negative -= changeValue;
            return Task.CompletedTask;
        }
    }
}
