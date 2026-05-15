namespace USB_Power_Supply_Application
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            buttonConnect = new Button();
            comboBoxPortsList = new ComboBox();
            labelVPositive = new Label();
            labelVNegative = new Label();
            labelVPReading = new Label();
            labelIPReading = new Label();
            labelWPReading = new Label();
            labelWNReading = new Label();
            labelINReading = new Label();
            labelVNReading = new Label();
            labelVPSet = new Label();
            labelVNSet = new Label();
            label3v3 = new Label();
            label2v5 = new Label();
            labelW3V3 = new Label();
            labelI3V3 = new Label();
            labelV3V3 = new Label();
            labelW2V5 = new Label();
            labelI2V5 = new Label();
            labelV2V5 = new Label();
            checkBoxAutoMeasure = new CheckBox();
            buttonRefreshMeasurements = new Button();
            label1 = new Label();
            labelSelectCOMPort = new Label();
            rotaryKnobVP = new USB_Power_Supply_Application.GUI_Elements.RotaryKnob();
            advancedCustomTrackbarVP = new USB_Power_Supply_Application.GUI_Elements.AdvancedCustomTrackbar();
            rotaryKnobVN = new USB_Power_Supply_Application.GUI_Elements.RotaryKnob();
            advancedCustomTrackbarVN = new USB_Power_Supply_Application.GUI_Elements.AdvancedCustomTrackbar();
            buttonDecVP = new Button();
            buttonIncVP = new Button();
            buttonToggleVP = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            labelVPEnabled = new Label();
            labelVNEnabled = new Label();
            labelV3Enabled = new Label();
            labelV2Enabled = new Label();
            textBoxVP = new TextBox();
            textBoxVN = new TextBox();
            buttonToggleVN = new Button();
            buttonToggleV3V3 = new Button();
            buttonToggleV2V5 = new Button();
            buttonDecVN = new Button();
            buttonIncVN = new Button();
            timerUpdateVIW = new System.Windows.Forms.Timer(components);
            timerUpdateMeasurements = new System.Windows.Forms.Timer(components);
            labelVPLimHigh = new Label();
            numericUpDownVPLimHigh = new NumericUpDown();
            numericUpDownVPLimLow = new NumericUpDown();
            labelVPLimLow = new Label();
            numericUpDownVNLimLow = new NumericUpDown();
            labelVNLimLow = new Label();
            numericUpDownVNLimHigh = new NumericUpDown();
            labelVNLimHigh = new Label();
            checkBoxVPAdjustLimits = new CheckBox();
            checkBoxVNAdjustLimits = new CheckBox();
            checkBoxEnableTracking = new CheckBox();
            checkBoxOnlyStack = new CheckBox();
            tableLayoutPanelGrids = new TableLayoutPanel();
            dataGridViewMeasurements = new DataGridView();
            Source = new DataGridViewTextBoxColumn();
            Voltage = new DataGridViewTextBoxColumn();
            Current = new DataGridViewTextBoxColumn();
            dataGridViewTaskStack = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)numericUpDownVPLimHigh).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownVPLimLow).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownVNLimLow).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownVNLimHigh).BeginInit();
            tableLayoutPanelGrids.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewMeasurements).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTaskStack).BeginInit();
            SuspendLayout();
            // 
            // buttonConnect
            // 
            buttonConnect.Location = new Point(5, 5);
            buttonConnect.Name = "buttonConnect";
            buttonConnect.Size = new Size(126, 57);
            buttonConnect.TabIndex = 0;
            buttonConnect.Text = "Connect";
            buttonConnect.UseVisualStyleBackColor = true;
            buttonConnect.Click += buttonConnect_Click;
            // 
            // comboBoxPortsList
            // 
            comboBoxPortsList.FormattingEnabled = true;
            comboBoxPortsList.Location = new Point(137, 23);
            comboBoxPortsList.Name = "comboBoxPortsList";
            comboBoxPortsList.Size = new Size(165, 23);
            comboBoxPortsList.TabIndex = 1;
            comboBoxPortsList.Click += comboBoxPortsList_Click;
            // 
            // labelVPositive
            // 
            labelVPositive.AutoSize = true;
            labelVPositive.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            labelVPositive.ForeColor = Color.Snow;
            labelVPositive.Location = new Point(97, 78);
            labelVPositive.Name = "labelVPositive";
            labelVPositive.Size = new Size(112, 32);
            labelVPositive.TabIndex = 6;
            labelVPositive.Text = "VPositive";
            // 
            // labelVNegative
            // 
            labelVNegative.AutoSize = true;
            labelVNegative.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            labelVNegative.ForeColor = Color.Snow;
            labelVNegative.Location = new Point(411, 78);
            labelVNegative.Name = "labelVNegative";
            labelVNegative.Size = new Size(127, 32);
            labelVNegative.TabIndex = 7;
            labelVNegative.Text = "VNegative";
            // 
            // labelVPReading
            // 
            labelVPReading.AutoSize = true;
            labelVPReading.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelVPReading.ForeColor = Color.Snow;
            labelVPReading.Location = new Point(12, 494);
            labelVPReading.Name = "labelVPReading";
            labelVPReading.Size = new Size(55, 50);
            labelVPReading.TabIndex = 8;
            labelVPReading.Text = "V:";
            // 
            // labelIPReading
            // 
            labelIPReading.AutoSize = true;
            labelIPReading.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelIPReading.ForeColor = Color.Snow;
            labelIPReading.Location = new Point(12, 557);
            labelIPReading.Name = "labelIPReading";
            labelIPReading.Size = new Size(42, 50);
            labelIPReading.TabIndex = 9;
            labelIPReading.Text = "I:";
            // 
            // labelWPReading
            // 
            labelWPReading.AutoSize = true;
            labelWPReading.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelWPReading.ForeColor = Color.Snow;
            labelWPReading.Location = new Point(12, 623);
            labelWPReading.Name = "labelWPReading";
            labelWPReading.Size = new Size(67, 50);
            labelWPReading.TabIndex = 10;
            labelWPReading.Text = "W:";
            // 
            // labelWNReading
            // 
            labelWNReading.AutoSize = true;
            labelWNReading.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelWNReading.ForeColor = Color.Snow;
            labelWNReading.Location = new Point(331, 623);
            labelWNReading.Name = "labelWNReading";
            labelWNReading.Size = new Size(67, 50);
            labelWNReading.TabIndex = 13;
            labelWNReading.Text = "W:";
            // 
            // labelINReading
            // 
            labelINReading.AutoSize = true;
            labelINReading.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelINReading.ForeColor = Color.Snow;
            labelINReading.Location = new Point(331, 557);
            labelINReading.Name = "labelINReading";
            labelINReading.Size = new Size(42, 50);
            labelINReading.TabIndex = 12;
            labelINReading.Text = "I:";
            // 
            // labelVNReading
            // 
            labelVNReading.AutoSize = true;
            labelVNReading.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelVNReading.ForeColor = Color.Snow;
            labelVNReading.Location = new Point(331, 494);
            labelVNReading.Name = "labelVNReading";
            labelVNReading.Size = new Size(55, 50);
            labelVNReading.TabIndex = 11;
            labelVNReading.Text = "V:";
            // 
            // labelVPSet
            // 
            labelVPSet.AutoSize = true;
            labelVPSet.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelVPSet.ForeColor = Color.Snow;
            labelVPSet.Location = new Point(12, 392);
            labelVPSet.Name = "labelVPSet";
            labelVPSet.Size = new Size(55, 50);
            labelVPSet.TabIndex = 14;
            labelVPSet.Text = "V:";
            // 
            // labelVNSet
            // 
            labelVNSet.AutoSize = true;
            labelVNSet.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelVNSet.ForeColor = Color.Snow;
            labelVNSet.Location = new Point(331, 392);
            labelVNSet.Name = "labelVNSet";
            labelVNSet.Size = new Size(55, 50);
            labelVNSet.TabIndex = 15;
            labelVNSet.Text = "V:";
            // 
            // label3v3
            // 
            label3v3.AutoSize = true;
            label3v3.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            label3v3.ForeColor = Color.Snow;
            label3v3.Location = new Point(721, 78);
            label3v3.Name = "label3v3";
            label3v3.Size = new Size(108, 32);
            label3v3.TabIndex = 16;
            label3v3.Text = "3.3V EXT";
            // 
            // label2v5
            // 
            label2v5.AutoSize = true;
            label2v5.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            label2v5.ForeColor = Color.Snow;
            label2v5.Location = new Point(1021, 78);
            label2v5.Name = "label2v5";
            label2v5.Size = new Size(108, 32);
            label2v5.TabIndex = 17;
            label2v5.Text = "2.5V EXT";
            // 
            // labelW3V3
            // 
            labelW3V3.AutoSize = true;
            labelW3V3.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelW3V3.ForeColor = Color.Snow;
            labelW3V3.Location = new Point(693, 309);
            labelW3V3.Name = "labelW3V3";
            labelW3V3.Size = new Size(67, 50);
            labelW3V3.TabIndex = 22;
            labelW3V3.Text = "W:";
            // 
            // labelI3V3
            // 
            labelI3V3.AutoSize = true;
            labelI3V3.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelI3V3.ForeColor = Color.Snow;
            labelI3V3.Location = new Point(693, 243);
            labelI3V3.Name = "labelI3V3";
            labelI3V3.Size = new Size(42, 50);
            labelI3V3.TabIndex = 21;
            labelI3V3.Text = "I:";
            // 
            // labelV3V3
            // 
            labelV3V3.AutoSize = true;
            labelV3V3.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelV3V3.ForeColor = Color.Snow;
            labelV3V3.Location = new Point(693, 180);
            labelV3V3.Name = "labelV3V3";
            labelV3V3.Size = new Size(55, 50);
            labelV3V3.TabIndex = 20;
            labelV3V3.Text = "V:";
            // 
            // labelW2V5
            // 
            labelW2V5.AutoSize = true;
            labelW2V5.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelW2V5.ForeColor = Color.Snow;
            labelW2V5.Location = new Point(996, 309);
            labelW2V5.Name = "labelW2V5";
            labelW2V5.Size = new Size(67, 50);
            labelW2V5.TabIndex = 25;
            labelW2V5.Text = "W:";
            // 
            // labelI2V5
            // 
            labelI2V5.AutoSize = true;
            labelI2V5.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelI2V5.ForeColor = Color.Snow;
            labelI2V5.Location = new Point(996, 243);
            labelI2V5.Name = "labelI2V5";
            labelI2V5.Size = new Size(42, 50);
            labelI2V5.TabIndex = 24;
            labelI2V5.Text = "I:";
            // 
            // labelV2V5
            // 
            labelV2V5.AutoSize = true;
            labelV2V5.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelV2V5.ForeColor = Color.Snow;
            labelV2V5.Location = new Point(996, 180);
            labelV2V5.Name = "labelV2V5";
            labelV2V5.Size = new Size(55, 50);
            labelV2V5.TabIndex = 23;
            labelV2V5.Text = "V:";
            // 
            // checkBoxAutoMeasure
            // 
            checkBoxAutoMeasure.AutoSize = true;
            checkBoxAutoMeasure.ForeColor = Color.Snow;
            checkBoxAutoMeasure.Location = new Point(1269, 78);
            checkBoxAutoMeasure.Name = "checkBoxAutoMeasure";
            checkBoxAutoMeasure.Size = new Size(176, 19);
            checkBoxAutoMeasure.TabIndex = 26;
            checkBoxAutoMeasure.Text = "Auto-Update Measurements";
            checkBoxAutoMeasure.UseVisualStyleBackColor = true;
            checkBoxAutoMeasure.CheckedChanged += checkBoxAutoMeasure_CheckedChanged;
            // 
            // buttonRefreshMeasurements
            // 
            buttonRefreshMeasurements.Location = new Point(1466, 78);
            buttonRefreshMeasurements.Name = "buttonRefreshMeasurements";
            buttonRefreshMeasurements.Size = new Size(192, 23);
            buttonRefreshMeasurements.TabIndex = 28;
            buttonRefreshMeasurements.Text = "Refresh Measurements";
            buttonRefreshMeasurements.UseVisualStyleBackColor = true;
            buttonRefreshMeasurements.Click += buttonRefreshMeasurements_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.FromArgb(71, 74, 82);
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI Semibold", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Snow;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(1827, 66);
            label1.TabIndex = 29;
            label1.Text = "USBC Power Supply V1.0";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelSelectCOMPort
            // 
            labelSelectCOMPort.AutoSize = true;
            labelSelectCOMPort.BackColor = Color.FromArgb(71, 74, 82);
            labelSelectCOMPort.ForeColor = Color.Snow;
            labelSelectCOMPort.Location = new Point(137, 5);
            labelSelectCOMPort.Name = "labelSelectCOMPort";
            labelSelectCOMPort.Size = new Size(152, 15);
            labelSelectCOMPort.TabIndex = 30;
            labelSelectCOMPort.Text = "Select the COM Port below.";
            // 
            // rotaryKnobVP
            // 
            rotaryKnobVP.Flipped = false;
            rotaryKnobVP.Font = new Font("Segoe UI", 12F);
            rotaryKnobVP.Location = new Point(63, 159);
            rotaryKnobVP.Maximum = 20F;
            rotaryKnobVP.Minimum = 0F;
            rotaryKnobVP.Name = "rotaryKnobVP";
            rotaryKnobVP.Size = new Size(180, 180);
            rotaryKnobVP.TabIndex = 31;
            rotaryKnobVP.Text = "rotaryKnob1";
            rotaryKnobVP.Value = 0F;
            rotaryKnobVP.ValueChanged += rotaryKnobVP_ValueChanged;
            // 
            // advancedCustomTrackbarVP
            // 
            advancedCustomTrackbarVP.LargeChange = 10;
            advancedCustomTrackbarVP.Location = new Point(40, 345);
            advancedCustomTrackbarVP.Maximum = 20;
            advancedCustomTrackbarVP.Minimum = 0;
            advancedCustomTrackbarVP.Name = "advancedCustomTrackbarVP";
            advancedCustomTrackbarVP.Orientation = Orientation.Horizontal;
            advancedCustomTrackbarVP.Size = new Size(239, 40);
            advancedCustomTrackbarVP.SliderColor = Color.LimeGreen;
            advancedCustomTrackbarVP.SmallChange = 1;
            advancedCustomTrackbarVP.TabIndex = 32;
            advancedCustomTrackbarVP.Text = "advancedCustomTrackbar1";
            advancedCustomTrackbarVP.ThumbImage = null;
            advancedCustomTrackbarVP.TickColor = Color.DarkGray;
            advancedCustomTrackbarVP.TickFrequency = 1;
            advancedCustomTrackbarVP.TickStyle = TickStyle.BottomRight;
            advancedCustomTrackbarVP.TrackColor = Color.LightGray;
            advancedCustomTrackbarVP.Value = 0;
            advancedCustomTrackbarVP.ValueChanged += advancedCustomTrackbarVP_ValueChanged;
            // 
            // rotaryKnobVN
            // 
            rotaryKnobVN.Flipped = true;
            rotaryKnobVN.Font = new Font("Segoe UI", 12F);
            rotaryKnobVN.Location = new Point(382, 159);
            rotaryKnobVN.Maximum = 0F;
            rotaryKnobVN.Minimum = -20F;
            rotaryKnobVN.Name = "rotaryKnobVN";
            rotaryKnobVN.Size = new Size(180, 180);
            rotaryKnobVN.TabIndex = 33;
            rotaryKnobVN.Text = "rotaryKnob1";
            rotaryKnobVN.Value = 0F;
            rotaryKnobVN.ValueChanged += rotaryKnobVN_ValueChanged;
            // 
            // advancedCustomTrackbarVN
            // 
            advancedCustomTrackbarVN.LargeChange = 10;
            advancedCustomTrackbarVN.Location = new Point(360, 345);
            advancedCustomTrackbarVN.Maximum = 20;
            advancedCustomTrackbarVN.Minimum = 0;
            advancedCustomTrackbarVN.Name = "advancedCustomTrackbarVN";
            advancedCustomTrackbarVN.Orientation = Orientation.Horizontal;
            advancedCustomTrackbarVN.Size = new Size(239, 40);
            advancedCustomTrackbarVN.SliderColor = Color.LimeGreen;
            advancedCustomTrackbarVN.SmallChange = 1;
            advancedCustomTrackbarVN.TabIndex = 34;
            advancedCustomTrackbarVN.Text = "advancedCustomTrackbar1";
            advancedCustomTrackbarVN.ThumbImage = null;
            advancedCustomTrackbarVN.TickColor = Color.DarkGray;
            advancedCustomTrackbarVN.TickFrequency = 1;
            advancedCustomTrackbarVN.TickStyle = TickStyle.BottomRight;
            advancedCustomTrackbarVN.TrackColor = Color.LightGray;
            advancedCustomTrackbarVN.Value = 0;
            advancedCustomTrackbarVN.ValueChanged += advancedCustomTrackbarVN_ValueChanged;
            // 
            // buttonDecVP
            // 
            buttonDecVP.Image = Properties.Resources.Inc_Minus;
            buttonDecVP.Location = new Point(12, 280);
            buttonDecVP.Name = "buttonDecVP";
            buttonDecVP.Size = new Size(63, 59);
            buttonDecVP.TabIndex = 35;
            buttonDecVP.UseVisualStyleBackColor = true;
            buttonDecVP.Click += buttonDecVP_Click;
            buttonDecVP.MouseEnter += buttonDecVP_MouseEnter;
            buttonDecVP.MouseLeave += buttonDecVP_MouseLeave;
            // 
            // buttonIncVP
            // 
            buttonIncVP.Image = Properties.Resources.Inc_Plus;
            buttonIncVP.Location = new Point(233, 280);
            buttonIncVP.Name = "buttonIncVP";
            buttonIncVP.Size = new Size(63, 59);
            buttonIncVP.TabIndex = 36;
            buttonIncVP.UseVisualStyleBackColor = true;
            buttonIncVP.Click += buttonIncVP_Click;
            buttonIncVP.MouseEnter += buttonIncVP_MouseEnter;
            buttonIncVP.MouseLeave += buttonIncVP_MouseLeave;
            // 
            // buttonToggleVP
            // 
            buttonToggleVP.BackgroundImage = (Image)resources.GetObject("buttonToggleVP.BackgroundImage");
            buttonToggleVP.BackgroundImageLayout = ImageLayout.Center;
            buttonToggleVP.Image = Properties.Resources.Toggle_Button;
            buttonToggleVP.Location = new Point(66, 113);
            buttonToggleVP.Name = "buttonToggleVP";
            buttonToggleVP.Size = new Size(180, 35);
            buttonToggleVP.TabIndex = 39;
            buttonToggleVP.UseVisualStyleBackColor = true;
            buttonToggleVP.Click += buttonToggleVP_Click;
            buttonToggleVP.MouseEnter += buttonToggleVP_MouseEnter;
            buttonToggleVP.MouseLeave += buttonToggleVP_MouseLeave;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Snow;
            label2.Location = new Point(66, 149);
            label2.Name = "label2";
            label2.Size = new Size(69, 25);
            label2.TabIndex = 41;
            label2.Text = "Status:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Snow;
            label3.Location = new Point(382, 149);
            label3.Name = "label3";
            label3.Size = new Size(69, 25);
            label3.TabIndex = 42;
            label3.Text = "Status:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.Snow;
            label4.Location = new Point(693, 149);
            label4.Name = "label4";
            label4.Size = new Size(69, 25);
            label4.TabIndex = 43;
            label4.Text = "Status:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.Snow;
            label5.Location = new Point(996, 149);
            label5.Name = "label5";
            label5.Size = new Size(69, 25);
            label5.TabIndex = 44;
            label5.Text = "Status:";
            // 
            // labelVPEnabled
            // 
            labelVPEnabled.AutoSize = true;
            labelVPEnabled.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelVPEnabled.ForeColor = Color.Red;
            labelVPEnabled.Location = new Point(200, 149);
            labelVPEnabled.Name = "labelVPEnabled";
            labelVPEnabled.Size = new Size(46, 25);
            labelVPEnabled.TabIndex = 45;
            labelVPEnabled.Text = "OFF";
            // 
            // labelVNEnabled
            // 
            labelVNEnabled.AutoSize = true;
            labelVNEnabled.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelVNEnabled.ForeColor = Color.Red;
            labelVNEnabled.Location = new Point(516, 149);
            labelVNEnabled.Name = "labelVNEnabled";
            labelVNEnabled.Size = new Size(46, 25);
            labelVNEnabled.TabIndex = 46;
            labelVNEnabled.Text = "OFF";
            // 
            // labelV3Enabled
            // 
            labelV3Enabled.AutoSize = true;
            labelV3Enabled.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelV3Enabled.ForeColor = Color.Red;
            labelV3Enabled.Location = new Point(827, 149);
            labelV3Enabled.Name = "labelV3Enabled";
            labelV3Enabled.Size = new Size(46, 25);
            labelV3Enabled.TabIndex = 47;
            labelV3Enabled.Text = "OFF";
            // 
            // labelV2Enabled
            // 
            labelV2Enabled.AutoSize = true;
            labelV2Enabled.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelV2Enabled.ForeColor = Color.Red;
            labelV2Enabled.Location = new Point(1130, 149);
            labelV2Enabled.Name = "labelV2Enabled";
            labelV2Enabled.Size = new Size(46, 25);
            labelV2Enabled.TabIndex = 48;
            labelV2Enabled.Text = "OFF";
            // 
            // textBoxVP
            // 
            textBoxVP.BackColor = Color.FromArgb(71, 74, 82);
            textBoxVP.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBoxVP.ForeColor = Color.Snow;
            textBoxVP.Location = new Point(66, 393);
            textBoxVP.Name = "textBoxVP";
            textBoxVP.Size = new Size(213, 50);
            textBoxVP.TabIndex = 49;
            textBoxVP.KeyDown += textBoxVP_KeyDown;
            // 
            // textBoxVN
            // 
            textBoxVN.BackColor = Color.FromArgb(71, 74, 82);
            textBoxVN.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBoxVN.ForeColor = Color.Snow;
            textBoxVN.Location = new Point(386, 393);
            textBoxVN.Name = "textBoxVN";
            textBoxVN.Size = new Size(213, 50);
            textBoxVN.TabIndex = 50;
            textBoxVN.KeyDown += textBoxVN_KeyDown;
            // 
            // buttonToggleVN
            // 
            buttonToggleVN.BackgroundImage = (Image)resources.GetObject("buttonToggleVN.BackgroundImage");
            buttonToggleVN.BackgroundImageLayout = ImageLayout.Center;
            buttonToggleVN.Image = Properties.Resources.Toggle_Button;
            buttonToggleVN.Location = new Point(382, 113);
            buttonToggleVN.Name = "buttonToggleVN";
            buttonToggleVN.Size = new Size(180, 35);
            buttonToggleVN.TabIndex = 51;
            buttonToggleVN.UseVisualStyleBackColor = true;
            buttonToggleVN.Click += buttonToggleVN_Click;
            buttonToggleVN.MouseEnter += buttonToggleVN_MouseEnter;
            buttonToggleVN.MouseLeave += buttonToggleVN_MouseLeave;
            // 
            // buttonToggleV3V3
            // 
            buttonToggleV3V3.BackgroundImage = (Image)resources.GetObject("buttonToggleV3V3.BackgroundImage");
            buttonToggleV3V3.BackgroundImageLayout = ImageLayout.Center;
            buttonToggleV3V3.Image = Properties.Resources.Toggle_Button;
            buttonToggleV3V3.Location = new Point(693, 113);
            buttonToggleV3V3.Name = "buttonToggleV3V3";
            buttonToggleV3V3.Size = new Size(180, 35);
            buttonToggleV3V3.TabIndex = 52;
            buttonToggleV3V3.UseVisualStyleBackColor = true;
            buttonToggleV3V3.Click += buttonToggleV3V3_Click;
            buttonToggleV3V3.MouseEnter += buttonToggleV3V3_MouseEnter;
            buttonToggleV3V3.MouseLeave += buttonToggleV3V3_MouseLeave;
            // 
            // buttonToggleV2V5
            // 
            buttonToggleV2V5.BackgroundImage = (Image)resources.GetObject("buttonToggleV2V5.BackgroundImage");
            buttonToggleV2V5.BackgroundImageLayout = ImageLayout.Center;
            buttonToggleV2V5.Image = Properties.Resources.Toggle_Button;
            buttonToggleV2V5.Location = new Point(996, 113);
            buttonToggleV2V5.Name = "buttonToggleV2V5";
            buttonToggleV2V5.Size = new Size(180, 35);
            buttonToggleV2V5.TabIndex = 53;
            buttonToggleV2V5.UseVisualStyleBackColor = true;
            buttonToggleV2V5.Click += buttonToggleV2V5_Click;
            buttonToggleV2V5.MouseEnter += buttonToggleV2V5_MouseEnter;
            buttonToggleV2V5.MouseLeave += buttonToggleV2V5_MouseLeave;
            // 
            // buttonDecVN
            // 
            buttonDecVN.Image = Properties.Resources.Inc_Minus;
            buttonDecVN.Location = new Point(335, 280);
            buttonDecVN.Name = "buttonDecVN";
            buttonDecVN.Size = new Size(63, 59);
            buttonDecVN.TabIndex = 54;
            buttonDecVN.UseVisualStyleBackColor = true;
            buttonDecVN.Click += buttonDecVN_Click;
            buttonDecVN.MouseEnter += buttonDecVN_MouseEnter;
            buttonDecVN.MouseLeave += buttonDecVN_MouseLeave;
            // 
            // buttonIncVN
            // 
            buttonIncVN.Image = Properties.Resources.Inc_Plus;
            buttonIncVN.Location = new Point(552, 280);
            buttonIncVN.Name = "buttonIncVN";
            buttonIncVN.Size = new Size(63, 59);
            buttonIncVN.TabIndex = 55;
            buttonIncVN.UseVisualStyleBackColor = true;
            buttonIncVN.Click += buttonIncVN_Click;
            buttonIncVN.MouseEnter += buttonIncVN_MouseEnter;
            buttonIncVN.MouseLeave += buttonIncVN_MouseLeave;
            // 
            // timerUpdateVIW
            // 
            timerUpdateVIW.Interval = 500;
            timerUpdateVIW.Tick += timerUpdateVIW_Tick;
            // 
            // timerUpdateMeasurements
            // 
            timerUpdateMeasurements.Interval = 1000;
            timerUpdateMeasurements.Tick += timerUpdateMeasurements_Tick;
            // 
            // labelVPLimHigh
            // 
            labelVPLimHigh.AutoSize = true;
            labelVPLimHigh.ForeColor = Color.Snow;
            labelVPLimHigh.Location = new Point(9, 157);
            labelVPLimHigh.Name = "labelVPLimHigh";
            labelVPLimHigh.Size = new Size(51, 15);
            labelVPLimHigh.TabIndex = 56;
            labelVPLimHigh.Text = "Limit(H)";
            // 
            // numericUpDownVPLimHigh
            // 
            numericUpDownVPLimHigh.DecimalPlaces = 1;
            numericUpDownVPLimHigh.Enabled = false;
            numericUpDownVPLimHigh.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDownVPLimHigh.Location = new Point(12, 180);
            numericUpDownVPLimHigh.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numericUpDownVPLimHigh.Name = "numericUpDownVPLimHigh";
            numericUpDownVPLimHigh.Size = new Size(55, 23);
            numericUpDownVPLimHigh.TabIndex = 57;
            numericUpDownVPLimHigh.TextAlign = HorizontalAlignment.Center;
            numericUpDownVPLimHigh.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // numericUpDownVPLimLow
            // 
            numericUpDownVPLimLow.DecimalPlaces = 1;
            numericUpDownVPLimLow.Enabled = false;
            numericUpDownVPLimLow.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDownVPLimLow.Location = new Point(12, 243);
            numericUpDownVPLimLow.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numericUpDownVPLimLow.Name = "numericUpDownVPLimLow";
            numericUpDownVPLimLow.Size = new Size(55, 23);
            numericUpDownVPLimLow.TabIndex = 59;
            numericUpDownVPLimLow.TextAlign = HorizontalAlignment.Center;
            // 
            // labelVPLimLow
            // 
            labelVPLimLow.AutoSize = true;
            labelVPLimLow.ForeColor = Color.Snow;
            labelVPLimLow.Location = new Point(9, 220);
            labelVPLimLow.Name = "labelVPLimLow";
            labelVPLimLow.Size = new Size(48, 15);
            labelVPLimLow.TabIndex = 58;
            labelVPLimLow.Text = "Limit(L)";
            // 
            // numericUpDownVNLimLow
            // 
            numericUpDownVNLimLow.DecimalPlaces = 1;
            numericUpDownVNLimLow.Enabled = false;
            numericUpDownVNLimLow.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDownVNLimLow.Location = new Point(334, 243);
            numericUpDownVNLimLow.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            numericUpDownVNLimLow.Minimum = new decimal(new int[] { 20, 0, 0, int.MinValue });
            numericUpDownVNLimLow.Name = "numericUpDownVNLimLow";
            numericUpDownVNLimLow.Size = new Size(52, 23);
            numericUpDownVNLimLow.TabIndex = 63;
            numericUpDownVNLimLow.TextAlign = HorizontalAlignment.Center;
            numericUpDownVNLimLow.Value = new decimal(new int[] { 20, 0, 0, int.MinValue });
            // 
            // labelVNLimLow
            // 
            labelVNLimLow.AutoSize = true;
            labelVNLimLow.ForeColor = Color.Snow;
            labelVNLimLow.Location = new Point(331, 220);
            labelVNLimLow.Name = "labelVNLimLow";
            labelVNLimLow.Size = new Size(48, 15);
            labelVNLimLow.TabIndex = 62;
            labelVNLimLow.Text = "Limit(L)";
            // 
            // numericUpDownVNLimHigh
            // 
            numericUpDownVNLimHigh.DecimalPlaces = 1;
            numericUpDownVNLimHigh.Enabled = false;
            numericUpDownVNLimHigh.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDownVNLimHigh.Location = new Point(334, 180);
            numericUpDownVNLimHigh.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            numericUpDownVNLimHigh.Minimum = new decimal(new int[] { 20, 0, 0, int.MinValue });
            numericUpDownVNLimHigh.Name = "numericUpDownVNLimHigh";
            numericUpDownVNLimHigh.Size = new Size(52, 23);
            numericUpDownVNLimHigh.TabIndex = 61;
            numericUpDownVNLimHigh.TextAlign = HorizontalAlignment.Center;
            // 
            // labelVNLimHigh
            // 
            labelVNLimHigh.AutoSize = true;
            labelVNLimHigh.ForeColor = Color.Snow;
            labelVNLimHigh.Location = new Point(331, 157);
            labelVNLimHigh.Name = "labelVNLimHigh";
            labelVNLimHigh.Size = new Size(51, 15);
            labelVNLimHigh.TabIndex = 60;
            labelVNLimHigh.Text = "Limit(H)";
            // 
            // checkBoxVPAdjustLimits
            // 
            checkBoxVPAdjustLimits.AutoSize = true;
            checkBoxVPAdjustLimits.ForeColor = Color.Snow;
            checkBoxVPAdjustLimits.Location = new Point(9, 113);
            checkBoxVPAdjustLimits.Name = "checkBoxVPAdjustLimits";
            checkBoxVPAdjustLimits.Size = new Size(58, 34);
            checkBoxVPAdjustLimits.TabIndex = 64;
            checkBoxVPAdjustLimits.Text = "Adust\r\nLimits";
            checkBoxVPAdjustLimits.UseVisualStyleBackColor = true;
            checkBoxVPAdjustLimits.CheckedChanged += checkBoxVPAdjustLimits_CheckedChanged;
            // 
            // checkBoxVNAdjustLimits
            // 
            checkBoxVNAdjustLimits.AutoSize = true;
            checkBoxVNAdjustLimits.ForeColor = Color.Snow;
            checkBoxVNAdjustLimits.Location = new Point(324, 114);
            checkBoxVNAdjustLimits.Name = "checkBoxVNAdjustLimits";
            checkBoxVNAdjustLimits.Size = new Size(58, 34);
            checkBoxVNAdjustLimits.TabIndex = 65;
            checkBoxVNAdjustLimits.Text = "Adust\r\nLimits";
            checkBoxVNAdjustLimits.UseVisualStyleBackColor = true;
            checkBoxVNAdjustLimits.CheckedChanged += checkBoxVNAdjustLimits_CheckedChanged;
            // 
            // checkBoxEnableTracking
            // 
            checkBoxEnableTracking.AutoSize = true;
            checkBoxEnableTracking.ForeColor = Color.Snow;
            checkBoxEnableTracking.Location = new Point(256, 78);
            checkBoxEnableTracking.Name = "checkBoxEnableTracking";
            checkBoxEnableTracking.Size = new Size(109, 19);
            checkBoxEnableTracking.TabIndex = 66;
            checkBoxEnableTracking.Text = "Enable Tracking";
            checkBoxEnableTracking.UseVisualStyleBackColor = true;
            // 
            // checkBoxOnlyStack
            // 
            checkBoxOnlyStack.AutoSize = true;
            checkBoxOnlyStack.ForeColor = Color.Snow;
            checkBoxOnlyStack.Location = new Point(1664, 78);
            checkBoxOnlyStack.Name = "checkBoxOnlyStack";
            checkBoxOnlyStack.Size = new Size(115, 19);
            checkBoxOnlyStack.TabIndex = 68;
            checkBoxOnlyStack.Text = "Only check stack";
            checkBoxOnlyStack.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelGrids
            // 
            tableLayoutPanelGrids.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanelGrids.ColumnCount = 1;
            tableLayoutPanelGrids.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelGrids.Controls.Add(dataGridViewMeasurements, 0, 1);
            tableLayoutPanelGrids.Controls.Add(dataGridViewTaskStack, 0, 0);
            tableLayoutPanelGrids.Location = new Point(1269, 103);
            tableLayoutPanelGrids.Name = "tableLayoutPanelGrids";
            tableLayoutPanelGrids.RowCount = 2;
            tableLayoutPanelGrids.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelGrids.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelGrids.Size = new Size(558, 580);
            tableLayoutPanelGrids.TabIndex = 69;
            // 
            // dataGridViewMeasurements
            // 
            dataGridViewMeasurements.AllowUserToAddRows = false;
            dataGridViewMeasurements.AllowUserToDeleteRows = false;
            dataGridViewMeasurements.AllowUserToResizeColumns = false;
            dataGridViewMeasurements.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.PaleGreen;
            dataGridViewCellStyle1.SelectionBackColor = Color.MediumSeaGreen;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.ControlText;
            dataGridViewMeasurements.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewMeasurements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewMeasurements.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewMeasurements.BackgroundColor = Color.FromArgb(71, 74, 82);
            dataGridViewMeasurements.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(71, 74, 82);
            dataGridViewCellStyle2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.Snow;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(71, 74, 82);
            dataGridViewCellStyle2.SelectionForeColor = Color.Snow;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dataGridViewMeasurements.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dataGridViewMeasurements.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewMeasurements.Columns.AddRange(new DataGridViewColumn[] { Source, Voltage, Current });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.MediumAquamarine;
            dataGridViewCellStyle3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = Color.MediumSeaGreen;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dataGridViewMeasurements.DefaultCellStyle = dataGridViewCellStyle3;
            dataGridViewMeasurements.Dock = DockStyle.Fill;
            dataGridViewMeasurements.Location = new Point(3, 293);
            dataGridViewMeasurements.Name = "dataGridViewMeasurements";
            dataGridViewMeasurements.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Control;
            dataGridViewCellStyle4.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            dataGridViewMeasurements.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewMeasurements.RowHeadersVisible = false;
            dataGridViewCellStyle5.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewMeasurements.RowsDefaultCellStyle = dataGridViewCellStyle5;
            dataGridViewMeasurements.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewMeasurements.Size = new Size(552, 284);
            dataGridViewMeasurements.TabIndex = 69;
            // 
            // Source
            // 
            Source.HeaderText = "Source";
            Source.Name = "Source";
            Source.ReadOnly = true;
            // 
            // Voltage
            // 
            Voltage.HeaderText = "Voltage";
            Voltage.Name = "Voltage";
            Voltage.ReadOnly = true;
            // 
            // Current
            // 
            Current.HeaderText = "Current";
            Current.Name = "Current";
            Current.ReadOnly = true;
            // 
            // dataGridViewTaskStack
            // 
            dataGridViewTaskStack.AllowUserToAddRows = false;
            dataGridViewTaskStack.AllowUserToDeleteRows = false;
            dataGridViewTaskStack.AllowUserToResizeColumns = false;
            dataGridViewTaskStack.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.BackColor = Color.PaleGreen;
            dataGridViewCellStyle6.SelectionBackColor = Color.MediumSeaGreen;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.ControlText;
            dataGridViewTaskStack.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            dataGridViewTaskStack.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewTaskStack.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewTaskStack.BackgroundColor = Color.FromArgb(71, 74, 82);
            dataGridViewTaskStack.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = Color.FromArgb(71, 74, 82);
            dataGridViewCellStyle7.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle7.ForeColor = Color.Snow;
            dataGridViewCellStyle7.SelectionBackColor = Color.FromArgb(71, 74, 82);
            dataGridViewCellStyle7.SelectionForeColor = Color.Snow;
            dataGridViewCellStyle7.WrapMode = DataGridViewTriState.True;
            dataGridViewTaskStack.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            dataGridViewTaskStack.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTaskStack.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = Color.MediumAquamarine;
            dataGridViewCellStyle8.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle8.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = Color.MediumSeaGreen;
            dataGridViewCellStyle8.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.False;
            dataGridViewTaskStack.DefaultCellStyle = dataGridViewCellStyle8;
            dataGridViewTaskStack.Dock = DockStyle.Fill;
            dataGridViewTaskStack.Location = new Point(3, 3);
            dataGridViewTaskStack.Name = "dataGridViewTaskStack";
            dataGridViewTaskStack.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = SystemColors.Control;
            dataGridViewCellStyle9.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle9.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = DataGridViewTriState.True;
            dataGridViewTaskStack.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            dataGridViewTaskStack.RowHeadersVisible = false;
            dataGridViewCellStyle10.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewTaskStack.RowsDefaultCellStyle = dataGridViewCellStyle10;
            dataGridViewTaskStack.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTaskStack.Size = new Size(552, 284);
            dataGridViewTaskStack.TabIndex = 68;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Task";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Stack Remaining (Bytes)";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(47, 49, 54);
            ClientSize = new Size(1827, 685);
            Controls.Add(tableLayoutPanelGrids);
            Controls.Add(checkBoxOnlyStack);
            Controls.Add(checkBoxEnableTracking);
            Controls.Add(checkBoxVNAdjustLimits);
            Controls.Add(checkBoxVPAdjustLimits);
            Controls.Add(numericUpDownVNLimLow);
            Controls.Add(labelVNLimLow);
            Controls.Add(numericUpDownVNLimHigh);
            Controls.Add(labelVNLimHigh);
            Controls.Add(numericUpDownVPLimLow);
            Controls.Add(labelVPLimLow);
            Controls.Add(numericUpDownVPLimHigh);
            Controls.Add(labelVPLimHigh);
            Controls.Add(buttonIncVN);
            Controls.Add(buttonDecVN);
            Controls.Add(buttonToggleV2V5);
            Controls.Add(buttonToggleV3V3);
            Controls.Add(buttonToggleVN);
            Controls.Add(textBoxVN);
            Controls.Add(textBoxVP);
            Controls.Add(labelV2Enabled);
            Controls.Add(labelV3Enabled);
            Controls.Add(labelVNEnabled);
            Controls.Add(labelVPEnabled);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(buttonToggleVP);
            Controls.Add(buttonIncVP);
            Controls.Add(buttonDecVP);
            Controls.Add(advancedCustomTrackbarVN);
            Controls.Add(rotaryKnobVN);
            Controls.Add(advancedCustomTrackbarVP);
            Controls.Add(rotaryKnobVP);
            Controls.Add(labelSelectCOMPort);
            Controls.Add(buttonRefreshMeasurements);
            Controls.Add(checkBoxAutoMeasure);
            Controls.Add(labelW2V5);
            Controls.Add(labelI2V5);
            Controls.Add(labelV2V5);
            Controls.Add(labelW3V3);
            Controls.Add(labelI3V3);
            Controls.Add(labelV3V3);
            Controls.Add(label2v5);
            Controls.Add(label3v3);
            Controls.Add(labelVNSet);
            Controls.Add(labelVPSet);
            Controls.Add(labelWNReading);
            Controls.Add(labelINReading);
            Controls.Add(labelVNReading);
            Controls.Add(labelWPReading);
            Controls.Add(labelIPReading);
            Controls.Add(labelVPReading);
            Controls.Add(labelVNegative);
            Controls.Add(labelVPositive);
            Controls.Add(comboBoxPortsList);
            Controls.Add(buttonConnect);
            Controls.Add(label1);
            MinimumSize = new Size(1843, 724);
            Name = "MainWindow";
            Text = "USB Power Supply Control Application";
            Shown += MainWindow_Shown;
            Resize += MainWindow_Resize;
            ((System.ComponentModel.ISupportInitialize)numericUpDownVPLimHigh).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownVPLimLow).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownVNLimLow).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownVNLimHigh).EndInit();
            tableLayoutPanelGrids.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewMeasurements).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTaskStack).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonConnect;
        private ComboBox comboBoxPortsList;
        private Label labelVPositive;
        private Label labelVNegative;
        private Label labelVPReading;
        private Label labelIPReading;
        private Label labelWPReading;
        private Label labelWNReading;
        private Label labelINReading;
        private Label labelVNReading;
        private Label labelVPSet;
        private Label labelVNSet;
        private Label label3v3;
        private Label label2v5;
        private Label labelW3V3;
        private Label labelI3V3;
        private Label labelV3V3;
        private Label labelW2V5;
        private Label labelI2V5;
        private Label labelV2V5;
        private CheckBox checkBoxAutoMeasure;
        private Button buttonRefreshMeasurements;
        private Label label1;
        private Label labelSelectCOMPort;
        private GUI_Elements.RotaryKnob rotaryKnobVP;
        private GUI_Elements.AdvancedCustomTrackbar advancedCustomTrackbarVP;
        private GUI_Elements.RotaryKnob rotaryKnobVN;
        private GUI_Elements.AdvancedCustomTrackbar advancedCustomTrackbarVN;
        private Button buttonDecVP;
        private Button buttonIncVP;
        private Button buttonToggleVP;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label labelVPEnabled;
        private Label labelVNEnabled;
        private Label labelV3Enabled;
        private Label labelV2Enabled;
        private TextBox textBoxVP;
        private TextBox textBoxVN;
        private Button buttonToggleVN;
        private Button buttonToggleV3V3;
        private Button buttonToggleV2V5;
        private Button buttonDecVN;
        private Button buttonIncVN;
        private System.Windows.Forms.Timer timerUpdateVIW;
        private System.Windows.Forms.Timer timerUpdateMeasurements;
        private Label labelVPLimHigh;
        private NumericUpDown numericUpDownVPLimHigh;
        private NumericUpDown numericUpDownVPLimLow;
        private Label labelVPLimLow;
        private NumericUpDown numericUpDownVNLimLow;
        private Label labelVNLimLow;
        private NumericUpDown numericUpDownVNLimHigh;
        private Label labelVNLimHigh;
        private CheckBox checkBoxVPAdjustLimits;
        private CheckBox checkBoxVNAdjustLimits;
        private CheckBox checkBoxEnableTracking;
        private CheckBox checkBoxOnlyStack;
        private TableLayoutPanel tableLayoutPanelGrids;
        private DataGridView dataGridViewMeasurements;
        private DataGridViewTextBoxColumn Source;
        private DataGridViewTextBoxColumn Voltage;
        private DataGridViewTextBoxColumn Current;
        private DataGridView dataGridViewTaskStack;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}
