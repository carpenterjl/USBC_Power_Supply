using System.IO.Ports;
using System.Threading.Channels;
using System.Windows.Forms;
using USB_Power_Supply_Application.GUI_Elements;
using USB_Power_Supply_Application.Hardware_Interface;
using USB_Power_Supply_Application.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace USB_Power_Supply_Application
{
    public partial class MainWindow : Form
    {
        private ISERIAL? _serial = new Serial();
        private IUsbAdapterDevice? _usbAdapter;
        private USB_Power_Supply_HW? _powerSupply;
        private bool updatingControls = false;
        private bool VP_EN, VN_EN, V3_EN, V2_EN = false;
        private bool readyForUpdate = true;
        private bool trackingMode = false;
        private float[,] SUPPLY_READINGS = new float[4, 3];
        private float[] SYS_MEASUREMENTS = new float[3];

        private System.Windows.Forms.Timer _debounceTimerVP;
        private System.Windows.Forms.Timer _debounceTimerVN;

        private DateTime? _lastUpdateTime = null;

        //TODO: Switch to concurrent queue?
        private readonly Channel<commands_Struct> commandChannel = Channel.CreateUnbounded<commands_Struct>();

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
        }

        public struct commands_Struct
        {
            public PowerCommand _cmd;
            public Int16 _value;
        }

        public MainWindow()
        {
            InitializeComponent();
            _usbAdapter = new USB_Adapter_HW(_serial);
            _powerSupply = new USB_Power_Supply_HW(_usbAdapter);
            comboBoxPortsList.Items.Clear();
            comboBoxPortsList.Items.AddRange(SerialPort.GetPortNames());
            dataGridViewMeasurements.Rows.Clear();
            dataGridViewMeasurements.Rows.Add("SYSTEM", 0, "N/A");
            dataGridViewMeasurements.Rows.Add("USB", 0, "N/A");
            dataGridViewMeasurements.Rows.Add("5V", 0, "N/A");
            dataGridViewMeasurements.Rows.Add("3.3V", 0, 0);
            dataGridViewMeasurements.Rows.Add("2.5V", 0, 0);
            dataGridViewMeasurements.Rows.Add("VPOSITIVE", 0, 0);
            dataGridViewMeasurements.Rows.Add("VNEGATIVE", 0, 0);
            dataGridViewMeasurements.EnableHeadersVisualStyles = false;
            foreach (DataGridViewColumn col in dataGridViewMeasurements.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            timerUpdateVIW.Enabled = true;
            timerUpdateVIW.Start();
            InitializeDebounceTimers();
        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            if (_usbAdapter != null && _usbAdapter.isDeviceConnected)
            {
                await _usbAdapter.Disconnect();
                buttonConnect.Text = "Connect";
            }
            else
            {
                try
                {
                    if (comboBoxPortsList.SelectedIndex == -1) return;

                    if (_usbAdapter != null)
                        await _usbAdapter.ConnectToDevice(comboBoxPortsList.Text);
                }
                catch
                {
                    throw new Exception("Error connecting to USB device");
                }
                if (_usbAdapter != null && _usbAdapter.isDeviceConnected)
                {
                    buttonConnect.Text = "Disconnect";
                }
            }
        }

        private void checkBoxAutoMeasure_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoMeasure.Checked)
            {
                timerUpdateMeasurements.Enabled = true;
                timerUpdateMeasurements.Start();
            }
            else
            {
                timerUpdateMeasurements.Enabled = false;
                timerUpdateMeasurements.Stop();
            }
        }

        private async void buttonRefreshMeasurements_Click(object sender, EventArgs e)
        {
            if (!readyForUpdate) return;

            readyForUpdate = false;
            await updateMeasurements();
            readyForUpdate = true;
        }

        private void rotaryKnobVP_ValueChanged(object sender, EventArgs e)
        {
            if (updatingControls)
                return;

            if(rotaryKnobVP.Value > (float)numericUpDownVPLimHigh.Value)
            {
                rotaryKnobVP.Value = (float)numericUpDownVPLimHigh.Value;
            }

            if(rotaryKnobVP.Value < (float)numericUpDownVPLimLow.Value)
            {
                rotaryKnobVP.Value = (float)numericUpDownVPLimLow.Value;
            }

            updatingControls = true;
            textBoxVP.Text = rotaryKnobVP.Value.ToString("0.000");
            advancedCustomTrackbarVP.Value = (int)float.Round(rotaryKnobVP.Value);
            updatingControls = false;

            if (_powerSupply == null || _usbAdapter == null || !_usbAdapter.isDeviceConnected) return;

            if (trackingMode) return;

            if (checkBoxEnableTracking.Checked)
            {
                trackingMode = true;
                rotaryKnobVN.Value = -(rotaryKnobVP.Value);
                trackingMode = false;
            }

            _debounceTimerVP.Stop();
            _debounceTimerVP.Start();
        }

        private void rotaryKnobVN_ValueChanged(object sender, EventArgs e)
        {
            if (updatingControls)
                return;

            if (rotaryKnobVN.Value > (float)numericUpDownVNLimHigh.Value)
            {
                rotaryKnobVN.Value = (float)numericUpDownVNLimHigh.Value;
            }

            if (rotaryKnobVN.Value < (float)numericUpDownVNLimLow.Value)
            {
                rotaryKnobVN.Value = (float)numericUpDownVNLimLow.Value;
            }

            updatingControls = true;
            textBoxVN.Text = rotaryKnobVN.Value.ToString("0.000");
            advancedCustomTrackbarVN.Value = (int)float.Abs(float.Round(rotaryKnobVN.Value));
            updatingControls = false;

            if (_powerSupply == null || _usbAdapter == null || !_usbAdapter.isDeviceConnected) return;

            if (trackingMode) return;

            if(checkBoxEnableTracking.Checked)
            {
                trackingMode = true;
                rotaryKnobVP.Value = -(rotaryKnobVN.Value);
                trackingMode = false;
            }

            _debounceTimerVN.Stop();
            _debounceTimerVN.Start();
        }

        private void textBoxVP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            if (updatingControls)
                return;

            if (!float.TryParse(
                textBoxVP.Text,
                out float val))
                return;

            updatingControls = true;

            rotaryKnobVP.Value = val;
            advancedCustomTrackbarVP.Value = (int)float.Round(rotaryKnobVP.Value);
            updatingControls = false;
        }

        private void textBoxVN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            if (updatingControls)
                return;

            if (!float.TryParse(
                textBoxVN.Text,
                out float val))
                return;

            updatingControls = true;

            rotaryKnobVN.Value = -Math.Abs(val);
            advancedCustomTrackbarVN.Value = (int)float.Abs(float.Round(rotaryKnobVN.Value));
            updatingControls = false;
        }

        private void advancedCustomTrackbarVP_ValueChanged(object sender, EventArgs e)
        {
            if (updatingControls)
                return;

            updatingControls = true;

            rotaryKnobVP.Value = advancedCustomTrackbarVP.Value;
            textBoxVP.Text = rotaryKnobVP.Value.ToString("0.000");
            updatingControls = false;
        }

        private void advancedCustomTrackbarVN_ValueChanged(object sender, EventArgs e)
        {
            if (updatingControls)
                return;

            updatingControls = true;

            rotaryKnobVN.Value = -advancedCustomTrackbarVN.Value;
            textBoxVN.Text = rotaryKnobVN.Value.ToString("0.000");
            updatingControls = false;
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            /* Start Command Handler Task In Parallel with Main Window */
            _ = Task.Run(CommandHandler);
        }

        private void comboBoxPortsList_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        private void buttonToggleVP_MouseEnter(object sender, EventArgs e)
        {
            buttonToggleVP.Image = Properties.Resources.Toggle_Button_Selected;
            buttonToggleVP.Invalidate();
        }

        private void buttonToggleVP_MouseLeave(object sender, EventArgs e)
        {
            buttonToggleVP.Image = Properties.Resources.Toggle_Button;
            buttonToggleVP.Invalidate();
        }

        private void buttonToggleVN_MouseEnter(object sender, EventArgs e)
        {
            buttonToggleVN.Image = Properties.Resources.Toggle_Button_Selected;
            buttonToggleVN.Invalidate();
        }

        private void buttonToggleVN_MouseLeave(object sender, EventArgs e)
        {
            buttonToggleVN.Image = Properties.Resources.Toggle_Button;
            buttonToggleVN.Invalidate();
        }

        private void buttonToggleV3V3_MouseEnter(object sender, EventArgs e)
        {
            buttonToggleV3V3.Image = Properties.Resources.Toggle_Button_Selected;
            buttonToggleV3V3.Invalidate();
        }

        private void buttonToggleV3V3_MouseLeave(object sender, EventArgs e)
        {
            buttonToggleV3V3.Image = Properties.Resources.Toggle_Button;
            buttonToggleV3V3.Invalidate();
        }

        private void buttonToggleV2V5_MouseEnter(object sender, EventArgs e)
        {
            buttonToggleV2V5.Image = Properties.Resources.Toggle_Button_Selected;
            buttonToggleV2V5.Invalidate();
        }

        private void buttonToggleV2V5_MouseLeave(object sender, EventArgs e)
        {
            buttonToggleV2V5.Image = Properties.Resources.Toggle_Button;
            buttonToggleV2V5.Invalidate();
        }

        private void buttonDecVP_Click(object sender, EventArgs e)
        {
            float step = 0.1f;

            if (ModifierKeys == Keys.Shift)
                step = 1f;

            if (ModifierKeys == Keys.Control)
                step = 0.01f;

            if (rotaryKnobVP.Value > 0)
                rotaryKnobVP.Value -= step;

            rotaryKnobVP.Invalidate();
        }

        private void buttonIncVP_Click(object sender, EventArgs e)
        {
            float step = 0.1f;

            if (ModifierKeys == Keys.Shift)
                step = 1f;

            if (ModifierKeys == Keys.Control)
                step = 0.01f;

            if (rotaryKnobVP.Value < 20)
                rotaryKnobVP.Value += step;

            rotaryKnobVP.Invalidate();
        }

        private void buttonDecVP_MouseEnter(object sender, EventArgs e)
        {
            buttonDecVP.Image = Properties.Resources.Inc_Minus_Selected;
        }

        private void buttonDecVP_MouseLeave(object sender, EventArgs e)
        {
            buttonDecVP.Image = Properties.Resources.Inc_Minus;
        }

        private void buttonDecVN_MouseEnter(object sender, EventArgs e)
        {
            buttonDecVN.Image = Properties.Resources.Inc_Minus_Selected;
        }

        private void buttonDecVN_MouseLeave(object sender, EventArgs e)
        {
            buttonDecVN.Image = Properties.Resources.Inc_Minus;
        }

        private void buttonIncVN_MouseEnter(object sender, EventArgs e)
        {
            buttonIncVN.Image = Properties.Resources.Inc_Plus_Selected;
        }

        private void buttonIncVN_MouseLeave(object sender, EventArgs e)
        {
            buttonIncVN.Image = Properties.Resources.Inc_Plus;
        }

        private void buttonIncVP_MouseEnter(object sender, EventArgs e)
        {
            buttonIncVP.Image = Properties.Resources.Inc_Plus_Selected;
        }

        private void buttonIncVP_MouseLeave(object sender, EventArgs e)
        {
            buttonIncVP.Image = Properties.Resources.Inc_Plus;
        }

        private void buttonDecVN_Click(object sender, EventArgs e)
        {
            float step = 0.1f;

            if (ModifierKeys == Keys.Shift)
                step = 1f;

            if (ModifierKeys == Keys.Control)
                step = 0.01f;

            if (rotaryKnobVN.Value > -20)
                rotaryKnobVN.Value -= step;

            rotaryKnobVN.Invalidate();
        }

        private void buttonIncVN_Click(object sender, EventArgs e)
        {
            float step = 0.1f;

            if (ModifierKeys == Keys.Shift)
                step = 1f;

            if (ModifierKeys == Keys.Control)
                step = 0.01f;

            if (rotaryKnobVN.Value < 0)
                rotaryKnobVN.Value += step;

            rotaryKnobVN.Invalidate();
        }

        private async void buttonToggleVP_Click(object sender, EventArgs e)
        {
            commands_Struct cmd = new commands_Struct();
            cmd._cmd = PowerCommand.ToggleVP;
            await commandChannel.Writer.WriteAsync(cmd);
        }

        private async void buttonToggleVN_Click(object sender, EventArgs e)
        {
            commands_Struct cmd = new commands_Struct();
            cmd._cmd = PowerCommand.ToggleVN;
            await commandChannel.Writer.WriteAsync(cmd);
        }

        private async void buttonToggleV3V3_Click(object sender, EventArgs e)
        {
            commands_Struct cmd = new commands_Struct();
            cmd._cmd = PowerCommand.ToggleV3v3;
            await commandChannel.Writer.WriteAsync(cmd);
        }

        private async void buttonToggleV2V5_Click(object sender, EventArgs e)
        {
            commands_Struct cmd = new commands_Struct();
            cmd._cmd = PowerCommand.ToggleV2v5;
            await commandChannel.Writer.WriteAsync(cmd);
        }

        private async void timerUpdateVIW_Tick(object sender, EventArgs e)
        {
            if (_powerSupply == null) return;
            commands_Struct cmd = new commands_Struct();

            if (VP_EN)
            {
                cmd._cmd = PowerCommand.ReadVPositive;
                await commandChannel.Writer.WriteAsync(cmd);
                cmd._cmd = PowerCommand.ReadIPositive;
                await commandChannel.Writer.WriteAsync(cmd);

                float VP = SUPPLY_READINGS[0, 0];
                float IP = SUPPLY_READINGS[0, 1];
                float W_P = VP * IP;
                if (float.IsNaN(VP) || float.IsNaN(IP) || float.IsNaN(W_P)) return;
                labelVPReading.Text = VP.ToString("0.000") + "V";
                labelIPReading.Text = IP.ToString("0.000") + "A";
                labelWPReading.Text = W_P.ToString("0.000") + "W";
            }

            if (VN_EN)
            {
                cmd._cmd = PowerCommand.ReadVNegative;
                await commandChannel.Writer.WriteAsync(cmd);
                cmd._cmd = PowerCommand.ReadINegative;
                await commandChannel.Writer.WriteAsync(cmd);

                float VP = SUPPLY_READINGS[1, 0];
                float IP = SUPPLY_READINGS[1, 1];
                float W_P = float.Abs(VP * IP);
                if (float.IsNaN(VP) || float.IsNaN(IP) || float.IsNaN(W_P)) return;
                labelVNReading.Text = VP.ToString("0.000") + "V";
                labelINReading.Text = IP.ToString("0.000") + "A";
                labelWNReading.Text = W_P.ToString("0.000") + "W";
            }

            if (V3_EN)
            {
                cmd._cmd = PowerCommand.ReadV3v3;
                await commandChannel.Writer.WriteAsync(cmd);
                cmd._cmd = PowerCommand.ReadI3v3;
                await commandChannel.Writer.WriteAsync(cmd);

                float VP = SUPPLY_READINGS[2, 0];
                float IP = SUPPLY_READINGS[2, 1];
                float W_P = VP * IP;
                if (float.IsNaN(VP) || float.IsNaN(IP) || float.IsNaN(W_P)) return;
                labelV3V3.Text = VP.ToString("0.000") + "V";
                labelI3V3.Text = IP.ToString("0.000") + "A";
                labelW3V3.Text = W_P.ToString("0.000") + "W";
            }

            if (V2_EN)
            {
                cmd._cmd = PowerCommand.ReadV2v5;
                await commandChannel.Writer.WriteAsync(cmd);
                cmd._cmd = PowerCommand.ReadI2v5;
                await commandChannel.Writer.WriteAsync(cmd);

                float VP = SUPPLY_READINGS[3, 0];
                float IP = SUPPLY_READINGS[3, 1];
                float W_P = VP * IP;
                if (float.IsNaN(VP) || float.IsNaN(IP) || float.IsNaN(W_P)) return;
                labelV2V5.Text = VP.ToString("0.000") + "V";
                labelI2V5.Text = IP.ToString("0.000") + "A";
                labelW2V5.Text = W_P.ToString("0.000") + "W";
            }
        }

        private async void timerUpdateMeasurements_Tick(object sender, EventArgs e)
        {
            if (!readyForUpdate) return;

            if (checkBoxAutoMeasure.Checked)
            {
                readyForUpdate = false;
                await updateMeasurements();
                readyForUpdate = true;
            }
        }

        private async Task updateMeasurements()
        {
            commands_Struct cmd = new commands_Struct();
            if (_usbAdapter != null && _usbAdapter.isDeviceConnected && _powerSupply != null)
            {
                //Loop to increment all readings and update measurements
                for (int idx = 0; idx < 11; idx++)
                {
                    switch (idx)
                    {
                        case 0:
                            cmd._cmd = PowerCommand.ReadI3v3;
                            break;
                        case 1:
                            cmd._cmd = PowerCommand.ReadI2v5;
                            break;
                        case 2:
                            cmd._cmd = PowerCommand.ReadIPositive;
                            break;
                        case 3:
                            cmd._cmd = PowerCommand.ReadINegative;
                            break;
                        case 4:
                            cmd._cmd = PowerCommand.ReadV5v;
                            break;
                        case 5:
                            cmd._cmd = PowerCommand.ReadVUsb;
                            break;
                        case 6:
                            cmd._cmd = PowerCommand.ReadVSystem;
                            break;
                        case 7:
                            cmd._cmd = PowerCommand.ReadV3v3;
                            break;
                        case 8:
                            cmd._cmd = PowerCommand.ReadV2v5;
                            break;
                        case 9:
                            cmd._cmd = PowerCommand.ReadVPositive;
                            break;
                        case 10:
                            cmd._cmd = PowerCommand.ReadVNegative;
                            break;
                        default:
                            break;
                    }
                    await commandChannel.Writer.WriteAsync(cmd);
                }
            }
        }

        private async Task CommandHandler()
        {
            while (true)
            {
                if (commandChannel.Reader.TryRead(out commands_Struct command) == true)
                {
                    float response = float.NaN;

                    if (_powerSupply == null) return;

                    switch (command._cmd)
                    {
                        /* Current Measurements */
                        case PowerCommand.ReadI3v3:
                            response = await _powerSupply.GetCurrent(USB_Power_Supply_HW.Current_Sources.I_3v3);
                            if (float.IsNaN(response)) return;
                            SUPPLY_READINGS[2, 1] = response;
                            UpdateGrid(3, 2, response.ToString());
                            break;
                        case PowerCommand.ReadI2v5:
                            response = await _powerSupply.GetCurrent(USB_Power_Supply_HW.Current_Sources.I_2v5);
                            if (float.IsNaN(response)) return;
                            SUPPLY_READINGS[3, 1] = response;
                            UpdateGrid(4, 2, response.ToString());
                            break;
                        case PowerCommand.ReadIPositive:
                            response = await _powerSupply.GetCurrent(USB_Power_Supply_HW.Current_Sources.I_Positive);
                            if (float.IsNaN(response)) return;
                            SUPPLY_READINGS[0, 1] = response;
                            UpdateGrid(5, 2, response.ToString());
                            break;
                        case PowerCommand.ReadINegative:
                            response = await _powerSupply.GetCurrent(USB_Power_Supply_HW.Current_Sources.I_Negative);
                            if (float.IsNaN(response)) return;
                            SUPPLY_READINGS[1, 1] = response;
                            UpdateGrid(6, 2, response.ToString());
                            break;

                        /* Voltage Measurements */
                        case PowerCommand.ReadV3v3:
                            response = await _powerSupply.GetVoltage(USB_Power_Supply_HW.Voltage_Sources.V_3v3);
                            if (float.IsNaN(response)) return;
                            SUPPLY_READINGS[2, 0] = response;
                            UpdateGrid(3, 1, response.ToString());
                            break;
                        case PowerCommand.ReadV2v5:
                            response = await _powerSupply.GetVoltage(USB_Power_Supply_HW.Voltage_Sources.V_2v5);
                            if (float.IsNaN(response)) return;
                            SUPPLY_READINGS[3, 0] = response;
                            UpdateGrid(4, 1, response.ToString());
                            break;
                        case PowerCommand.ReadVPositive:
                            response = await _powerSupply.GetVoltage(USB_Power_Supply_HW.Voltage_Sources.V_Positive);
                            if (float.IsNaN(response)) return;
                            SUPPLY_READINGS[0, 0] = response;
                            UpdateGrid(5, 1, response.ToString());
                            break;
                        case PowerCommand.ReadVNegative:
                            response = await _powerSupply.GetVoltage(USB_Power_Supply_HW.Voltage_Sources.V_Negative);
                            if (float.IsNaN(response)) return;
                            SUPPLY_READINGS[1, 0] = response;
                            UpdateGrid(6, 1, response.ToString());
                            break;
                        case PowerCommand.ReadVSystem:
                            response = await _powerSupply.GetVoltage(USB_Power_Supply_HW.Voltage_Sources.V_System);
                            if (float.IsNaN(response)) return;
                            SYS_MEASUREMENTS[0] = response;
                            UpdateGrid(0, 1, response.ToString());
                            break;
                        case PowerCommand.ReadVUsb:
                            response = await _powerSupply.GetVoltage(USB_Power_Supply_HW.Voltage_Sources.V_USB);
                            if (float.IsNaN(response)) return;
                            SYS_MEASUREMENTS[1] = response;
                            UpdateGrid(1, 1, response.ToString());
                            break;
                        case PowerCommand.ReadV5v:
                            response = await _powerSupply.GetVoltage(USB_Power_Supply_HW.Voltage_Sources.V_5v);
                            if (float.IsNaN(response)) return;
                            SYS_MEASUREMENTS[2] = response;
                            UpdateGrid(2, 1, response.ToString());
                            break;

                        /* Supply Toggles */
                        case PowerCommand.ToggleV3v3:
                            await Handle3V3Toggle();
                            break;
                        case PowerCommand.ToggleV2v5:
                            await Handle2V5Toggle();
                            break;
                        case PowerCommand.ToggleVP:
                            await HandleVPToggle();
                            break;
                        case PowerCommand.ToggleVN:
                            await HandleVNToggle();
                            break;

                        /* Supply Voltage Setting */
                        case PowerCommand.WriteVP:
                            await _powerSupply.SetVoltage(USB_Power_Supply_HW.Voltage_Sources.V_Positive, (float)command._value / 1000.0f);
                            break;
                        case PowerCommand.WriteVN:
                            await _powerSupply.SetVoltage(USB_Power_Supply_HW.Voltage_Sources.V_Negative, (float)command._value / 1000.0f);
                            break;

                        /* Default = Error in Software */
                        default:
                            MessageBox.Show("Error in command processor, reached default state.", "Software Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                    }
                }
                await Task.Delay(5);
            }
        }

        private void UpdateGrid(int row, int col, string value)
        {
            this.Invoke(() =>
            {
                dataGridViewMeasurements.Rows[row].Cells[col].Value = value;
            });
        }

        private async Task Handle3V3Toggle()
        {
            if (_usbAdapter?.isDeviceConnected == true && _powerSupply != null)
            {
                string response = V3_EN
                    ? await _powerSupply.DisableOutput(USB_Power_Supply_HW.Voltage_Sources.V_3v3)
                    : await _powerSupply.EnableOutput(USB_Power_Supply_HW.Voltage_Sources.V_3v3);

                if (response?.Contains("OK") == true)
                {
                    V3_EN = !V3_EN;
                    // Update UI Labels safely
                    this.Invoke(() =>
                    {
                        labelV3Enabled.ForeColor = V3_EN ? Color.Lime : Color.Red;
                        labelV3Enabled.Text = V3_EN ? "ON" : "OFF";
                        if (!V3_EN)
                        {
                            labelV3V3.Text = "0.000V";
                            labelI3V3.Text = "0.000A";
                            labelW3V3.Text = "0.000W";
                        }
                    });
                }
            }
        }

        private async Task Handle2V5Toggle()
        {
            if (_usbAdapter?.isDeviceConnected == true && _powerSupply != null)
            {
                string response = V2_EN
                    ? await _powerSupply.DisableOutput(USB_Power_Supply_HW.Voltage_Sources.V_2v5)
                    : await _powerSupply.EnableOutput(USB_Power_Supply_HW.Voltage_Sources.V_2v5);

                if (response?.Contains("OK") == true)
                {
                    V2_EN = !V2_EN;
                    // Update UI Labels safely
                    this.Invoke(() =>
                    {
                        labelV2Enabled.ForeColor = V2_EN ? Color.Lime : Color.Red;
                        labelV2Enabled.Text = V2_EN ? "ON" : "OFF";
                        if (!V2_EN)
                        {
                            labelV2V5.Text = "0.000V";
                            labelI2V5.Text = "0.000A";
                            labelW2V5.Text = "0.000W";
                        }
                    });
                }
            }
        }

        private async Task HandleVPToggle()
        {
            if (_usbAdapter?.isDeviceConnected == true && _powerSupply != null)
            {
                string response = VP_EN
                    ? await _powerSupply.DisableOutput(USB_Power_Supply_HW.Voltage_Sources.V_Positive)
                    : await _powerSupply.EnableOutput(USB_Power_Supply_HW.Voltage_Sources.V_Positive);

                if (response?.Contains("OK") == true)
                {
                    VP_EN = !VP_EN;
                    // Update UI Labels safely
                    this.Invoke(() =>
                    {
                        labelVPEnabled.ForeColor = VP_EN ? Color.Lime : Color.Red;
                        labelVPEnabled.Text = VP_EN ? "ON" : "OFF";
                        if (!VP_EN)
                        {
                            labelVPReading.Text = "0.000V";
                            labelIPReading.Text = "0.000A";
                            labelWPReading.Text = "0.000W";
                        }
                    });
                }
            }
        }

        private async Task HandleVNToggle()
        {
            if (_usbAdapter?.isDeviceConnected == true && _powerSupply != null)
            {
                string response = VN_EN
                    ? await _powerSupply.DisableOutput(USB_Power_Supply_HW.Voltage_Sources.V_Negative)
                    : await _powerSupply.EnableOutput(USB_Power_Supply_HW.Voltage_Sources.V_Negative);

                if (response?.Contains("OK") == true)
                {
                    VN_EN = !VN_EN;
                    // Update UI Labels safely
                    this.Invoke(() =>
                    {
                        labelVNEnabled.ForeColor = VN_EN ? Color.Lime : Color.Red;
                        labelVNEnabled.Text = VN_EN ? "ON" : "OFF";
                        if (!VN_EN)
                        {
                            labelVNReading.Text = "0.000V";
                            labelINReading.Text = "0.000A";
                            labelWNReading.Text = "0.000W";
                        }
                    });
                }
            }
        }

        private void checkBoxVPAdjustLimits_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxVPAdjustLimits.Checked == true)
            {
                numericUpDownVPLimHigh.Enabled = true;
                numericUpDownVPLimLow.Enabled = true;
            }
            else
            {
                numericUpDownVPLimHigh.Enabled = false;
                numericUpDownVPLimLow.Enabled = false;
            }
        }

        private void checkBoxVNAdjustLimits_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxVNAdjustLimits.Checked == true)
            {
                numericUpDownVNLimHigh.Enabled = true;
                numericUpDownVNLimLow.Enabled = true;
            }
            else
            {
                numericUpDownVNLimHigh.Enabled = false;
                numericUpDownVNLimLow.Enabled = false;
            }
        }

        private void InitializeDebounceTimers()
        {
            _debounceTimerVP = new System.Windows.Forms.Timer { Interval = 150 };
            _debounceTimerVP.Tick += async (s, e) => {
                _debounceTimerVP.Stop();
                commands_Struct cmd = new commands_Struct();
                cmd._cmd = PowerCommand.WriteVP;
                cmd._value = (Int16)(rotaryKnobVP.Value * 1000);
                await commandChannel.Writer.WriteAsync(cmd);
                if(checkBoxEnableTracking.Checked)
                {
                    cmd._cmd = PowerCommand.WriteVN;
                    cmd._value = (Int16)(rotaryKnobVN.Value * 1000);
                    await commandChannel.Writer.WriteAsync(cmd);
                }
            };

            _debounceTimerVN = new System.Windows.Forms.Timer { Interval = 150 };
            _debounceTimerVN.Tick += async (s, e) => {
                _debounceTimerVN.Stop();
                commands_Struct cmd = new commands_Struct();
                cmd._cmd = PowerCommand.WriteVN;
                cmd._value = (Int16)(rotaryKnobVN.Value * 1000);
                await commandChannel.Writer.WriteAsync(cmd);
                if(checkBoxEnableTracking.Checked)
                {
                    cmd._cmd = PowerCommand.WriteVP;
                    cmd._value = (Int16)(rotaryKnobVP.Value * 1000);
                    await commandChannel.Writer.WriteAsync(cmd);
                }
            };
        }
    }
}
