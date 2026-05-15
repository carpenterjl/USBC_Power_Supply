using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USB_Power_Supply_Application.GUI_Elements
{
    public partial class AdvancedCustomTrackbar : Control
    {
        // Range Properties
        [Category("Behavior")] public int Minimum { get; set; } = 0;
        [Category("Behavior")] public int Maximum { get; set; } = 100;
        [Category("Behavior")] public int TickFrequency { get; set; } = 10;
        [Category("Behavior")] public int SmallChange { get; set; } = 1;
        [Category("Behavior")] public int LargeChange { get; set; } = 10;

        private int _value = 0;
        [Category("Appearance")]
        public int Value
        {
            get => _value;
            set { _value = Math.Max(Minimum, Math.Min(Maximum, value)); Invalidate(); ValueChanged?.Invoke(this, EventArgs.Empty); }
        }

        // Custom Styling
        [Category("Appearance")] public Color TrackColor { get; set; } = Color.LightGray;
        [Category("Appearance")] public Color SliderColor { get; set; } = Color.RoyalBlue;
        [Category("Appearance")] public Color TickColor { get; set; } = Color.DarkGray;
        [Category("Appearance")] public Image? ThumbImage { get; set; }
        [Category("Appearance")] public Orientation Orientation { get; set; } = Orientation.Horizontal;
        [Category("Appearance")] public TickStyle TickStyle { get; set; } = TickStyle.BottomRight;

        public event EventHandler? ValueChanged;

        public AdvancedCustomTrackbar()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.Selectable, true); // Allows keyboard focus
            this.Size = new Size(200, 45);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            bool isHoriz = Orientation == Orientation.Horizontal;
            int trackSize = 4;
            int padding = 12;

            // 1. Draw Track
            Rectangle trackRect = isHoriz
                ? new Rectangle(padding, (Height / 2) - (trackSize / 2), Width - (padding * 2), trackSize)
                : new Rectangle((Width / 2) - (trackSize / 2), padding, trackSize, Height - (padding * 2));

            using (SolidBrush trackBrush = new SolidBrush(TrackColor))
                g.FillRectangle(trackBrush, trackRect);

            // 2. Draw Ticks
            if (TickStyle != TickStyle.None && TickFrequency > 0)
            {
                using (Pen tickPen = new Pen(TickColor))
                {
                    for (int i = Minimum; i <= Maximum; i += TickFrequency)
                    {
                        float percent = (float)(i - Minimum) / (Maximum - Minimum);
                        int pos = isHoriz
                            ? (int)(padding + percent * trackRect.Width)
                            : (int)(padding + percent * trackRect.Height);

                        if (isHoriz)
                        {
                            if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                                g.DrawLine(tickPen, pos, trackRect.Bottom + 2, pos, trackRect.Bottom + 6);
                            if (TickStyle == TickStyle.TopLeft || TickStyle == TickStyle.Both)
                                g.DrawLine(tickPen, pos, trackRect.Top - 2, pos, trackRect.Top - 6);
                        }
                        else
                        {
                            if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                                g.DrawLine(tickPen, trackRect.Right + 2, pos, trackRect.Right + 6, pos);
                            if (TickStyle == TickStyle.TopLeft || TickStyle == TickStyle.Both)
                                g.DrawLine(tickPen, trackRect.Left - 2, pos, trackRect.Left - 6, pos);
                        }
                    }
                }
            }

            // 3. Draw Thumb
            float valPercent = (float)(Value - Minimum) / (Maximum - Minimum);
            int thumbW = ThumbImage?.Width ?? 12;
            int thumbH = ThumbImage?.Height ?? 20;

            int thumbX = isHoriz ? (int)(padding + (valPercent * trackRect.Width) - (thumbW / 2)) : (Width / 2) - (thumbW / 2);
            int thumbY = isHoriz ? (Height / 2) - (thumbH / 2) : (int)(padding + (valPercent * trackRect.Height) - (thumbH / 2));

            Rectangle thumbRect = new Rectangle(thumbX, thumbY, thumbW, thumbH);

            if (ThumbImage != null) g.DrawImage(ThumbImage, thumbRect);
            else using (SolidBrush b = new SolidBrush(SliderColor)) g.FillRectangle(b, thumbRect);

            if (Focused) ControlPaint.DrawFocusRectangle(g, ClientRectangle);
        }

        // Input Handling
        protected override void OnMouseDown(MouseEventArgs e) { Focus(); UpdateValue(e.Location); base.OnMouseDown(e); }
        protected override void OnMouseMove(MouseEventArgs e) { if (e.Button == MouseButtons.Left) UpdateValue(e.Location); base.OnMouseMove(e); }

        private void UpdateValue(Point pt)
        {
            int padding = 12;
            float percent = Orientation == Orientation.Horizontal
                ? (float)(pt.X - padding) / (Width - (padding * 2))
                : (float)(pt.Y - padding) / (Height - (padding * 2));

            Value = (int)(Minimum + (Math.Max(0, Math.Min(1, percent)) * (Maximum - Minimum)));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left || keyData == Keys.Down) Value -= SmallChange;
            else if (keyData == Keys.Right || keyData == Keys.Up) Value += SmallChange;
            else if (keyData == Keys.PageUp) Value += LargeChange;
            else if (keyData == Keys.PageDown) Value -= LargeChange;
            else return base.ProcessCmdKey(ref msg, keyData);
            return true;
        }
    }
}
