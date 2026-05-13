using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace USB_Power_Supply_Application.GUI_Elements
{
    public partial class RotaryKnob : Control
    {
        private bool dragging = false;

        private bool flipped = false;

        private float minimum = 0;
        private float maximum = 30;
        private float value = 5;

        public event EventHandler ValueChanged;

        [Category("Appearance")]
        public bool Flipped
        {
            get => flipped;
            set
            {
                flipped = value;
                Invalidate();
            }
        }

        [Category("Behavior")]
        public float Minimum
        {
            get => minimum;
            set
            {
                minimum = value;
                Invalidate();
            }
        }

        [Category("Behavior")]
        public float Maximum
        {
            get => maximum;
            set
            {
                maximum = value;
                Invalidate();
            }
        }

        [Category("Behavior")]
        public float Value
        {
            get => value;
            set
            {
                float clamped =
                    Math.Max(minimum,
                    Math.Min(maximum, value));

                if (Math.Abs(this.value - clamped) > 0.0001f)
                {
                    this.value = clamped;

                    ValueChanged?.Invoke(this, EventArgs.Empty);

                    Invalidate();
                }
            }
        }

        public RotaryKnob()
        {
            DoubleBuffered = true;

            Size = new Size(220, 220);

            Font = new Font("Segoe UI", 12f);

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect =
                new Rectangle(20, 20, Width - 40, Height - 40);

            Point center =
                new Point(Width / 2, Height / 2);

            float radius = rect.Width / 2f;

            //
            // Sweep Geometry
            //
            float startAngle = 135f;
            float sweepAngle = 270f;

            //
            // Logical percentage
            //
            float valuePercent =
                (Value - Minimum) /
                (Maximum - Minimum);

            //
            // Indicator percentage
            //
            float indicatorPercent =
                Flipped ?
                1f - valuePercent :
                valuePercent;

            float currentAngle =
                startAngle +
                (sweepAngle * indicatorPercent);

            //
            // SHADOW
            //
            Rectangle shadowRect =
                new Rectangle(
                    rect.X + 6,
                    rect.Y + 6,
                    rect.Width,
                    rect.Height);

            using (SolidBrush sb =
                new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
            {
                g.FillEllipse(sb, shadowRect);
            }

            //
            // OUTER RING
            //
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(rect);

                using (PathGradientBrush pgb =
                    new PathGradientBrush(path))
                {
                    pgb.CenterColor =
                        Color.FromArgb(90, 90, 90);

                    pgb.SurroundColors =
                        new[] { Color.FromArgb(20, 20, 20) };

                    g.FillEllipse(pgb, rect);
                }
            }

            //
            // ARC TRACK
            //
            Rectangle arcRect =
                new Rectangle(
                    rect.X + 8,
                    rect.Y + 8,
                    rect.Width - 16,
                    rect.Height - 16);

            using (Pen p =
                new Pen(Color.FromArgb(50, 50, 50), 10))
            {
                p.StartCap = LineCap.Round;
                p.EndCap = LineCap.Round;

                g.DrawArc(
                    p,
                    arcRect,
                    startAngle,
                    sweepAngle);
            }

            //
            // ACTIVE ARC
            //
            using (Pen p =
                new Pen(Color.LimeGreen, 10))
            {
                p.StartCap = LineCap.Round;
                p.EndCap = LineCap.Round;

                if (!Flipped)
                {
                    //
                    // Normal:
                    // fill left -> right
                    //
                    g.DrawArc(
                        p,
                        arcRect,
                        startAngle,
                        sweepAngle * valuePercent);
                }
                else
                {
                    //
                    // Flipped:
                    // fill right -> left
                    //
                    float arcStart =
                        startAngle;

                    float arcSweep =
                        sweepAngle * (1f - valuePercent);

                    g.DrawArc(
                        p,
                        arcRect,
                        arcStart,
                        arcSweep);
                }
            }

            //
            // TICK MARKS + LABELS
            //
            for (int i = 0; i <= 30; i++)
            {
                float t = i / 30f;

                float angle =
                    startAngle +
                    (sweepAngle * t);

                double rad =
                    angle * Math.PI / 180.0;

                bool major =
                    (i % 5 == 0);

                float outer =
                    radius - 2;

                float inner =
                    major ?
                    radius - 18 :
                    radius - 10;

                PointF p1 = new PointF(
                    center.X +
                    (float)Math.Cos(rad) * inner,

                    center.Y +
                    (float)Math.Sin(rad) * inner);

                PointF p2 = new PointF(
                    center.X +
                    (float)Math.Cos(rad) * outer,

                    center.Y +
                    (float)Math.Sin(rad) * outer);

                using (Pen pen =
                    new Pen(
                        major ?
                        Color.White :
                        Color.Gray,
                        major ? 2 : 1))
                {
                    g.DrawLine(pen, p1, p2);
                }

                //
                // LABELS
                //
                if (major)
                {
                    float labelRadius =
                        radius + 10;

                    PointF labelPoint =
                        new PointF(
                            center.X +
                            (float)Math.Cos(rad) * labelRadius,

                            center.Y +
                            (float)Math.Sin(rad) * labelRadius);

                    float labelPercent =
                        Flipped ?
                        1f - t :
                        t;

                    float labelValue =
                        Minimum +
                        ((Maximum - Minimum) * labelPercent);

                    string label =
                        labelValue.ToString("0");

                    using (Font tickFont =
                        new Font("Segoe UI", 8f))
                    {
                        SizeF sz =
                            g.MeasureString(label, tickFont);

                        g.DrawString(
                            label,
                            tickFont,
                            Brushes.WhiteSmoke,
                            labelPoint.X - sz.Width / 2,
                            labelPoint.Y - sz.Height / 2);
                    }
                }
            }

            //
            // INNER KNOB
            //
            Rectangle innerRect =
                new Rectangle(
                    rect.X + 35,
                    rect.Y + 35,
                    rect.Width - 70,
                    rect.Height - 70);

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(innerRect);

                using (PathGradientBrush pgb =
                    new PathGradientBrush(path))
                {
                    pgb.CenterColor =
                        Color.FromArgb(110, 110, 110);

                    pgb.SurroundColors =
                        new[] { Color.FromArgb(30, 30, 30) };

                    g.FillEllipse(pgb, innerRect);
                }
            }

            //
            // INDICATOR
            //
            double indicatorRad =
                currentAngle *
                Math.PI / 180.0;

            float indicatorLength =
                innerRect.Width / 2f - 15;

            PointF indicatorEnd =
                new PointF(
                    center.X +
                    (float)Math.Cos(indicatorRad) *
                    indicatorLength,

                    center.Y +
                    (float)Math.Sin(indicatorRad) *
                    indicatorLength);

            using (Pen p =
                new Pen(Color.WhiteSmoke, 5))
            {
                p.StartCap = LineCap.Round;
                p.EndCap = LineCap.Round;

                g.DrawLine(
                    p,
                    center,
                    indicatorEnd);
            }

            //
            // CENTER TEXT
            //
            string text =
                Value.ToString("0.000");

            using (Font valueFont =
                new Font(
                    "Segoe UI",
                    20f,
                    FontStyle.Bold))
            {
                SizeF sz =
                    g.MeasureString(text, valueFont);

                g.DrawString(
                    text,
                    valueFont,
                    Brushes.LimeGreen,
                    center.X - sz.Width / 2,
                    center.Y - sz.Height / 2);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            dragging = true;

            UpdateValueFromMouse(e.Location);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!dragging)
                return;

            UpdateValueFromMouse(e.Location);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            dragging = false;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            float step = 0.1f;

            if (ModifierKeys == Keys.Shift)
                step = 1f;

            if (ModifierKeys == Keys.Control)
                step = 0.01f;

            Value +=
                e.Delta > 0 ?
                step :
                -step;
        }

        private void UpdateValueFromMouse(Point p)
        {
            Point center =
                new Point(
                    Width / 2,
                    Height / 2);

            float dx =
                p.X - center.X;

            float dy =
                p.Y - center.Y;

            double angle =
                Math.Atan2(dy, dx) *
                180.0 / Math.PI;

            if (angle < 0)
                angle += 360;

            float startAngle = 135f;
            float sweepAngle = 270f;

            float adjustedAngle =
                (float)angle;

            if (adjustedAngle < startAngle)
                adjustedAngle += 360f;

            adjustedAngle =
                Math.Max(
                    startAngle,
                    Math.Min(
                        startAngle + sweepAngle,
                        adjustedAngle));

            float percent =
                (adjustedAngle - startAngle) /
                sweepAngle;

            if (Flipped)
                percent = 1f - percent;

            Value =
                Minimum +
                ((Maximum - Minimum) * percent);
        }
    }
}