using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Power_Supply_Control_WPF.GUI_Elements
{
    public partial class RotaryKnob : UserControl
    {
        private bool dragging = false;

        public event EventHandler ValueChanged;

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum),
                typeof(double),
                typeof(RotaryKnob),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum),
                typeof(double),
                typeof(RotaryKnob),
                new FrameworkPropertyMetadata(
                    30.0,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(RotaryKnob),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnValueChanged));

        public static readonly DependencyProperty FlippedProperty =
            DependencyProperty.Register(
                nameof(Flipped),
                typeof(bool),
                typeof(RotaryKnob),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(ArcColor),
                typeof(Brush),
                typeof(RotaryKnob),
                new FrameworkPropertyMetadata(
                    Brushes.Lime,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush ArcColor
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public bool Flipped
        {
            get => (bool)GetValue(FlippedProperty);
            set => SetValue(FlippedProperty, value);
        }

        private static void OnValueChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            RotaryKnob knob = (RotaryKnob)d;

            double clamped =
                Math.Max(
                    knob.Minimum,
                    Math.Min(
                        knob.Maximum,
                        (double)e.NewValue));

            knob.SetCurrentValue(
                ValueProperty,
                clamped);

            knob.ValueChanged?.Invoke(
                knob,
                EventArgs.Empty);

            knob.InvalidateVisual();
        }

        public RotaryKnob()
        {
            InitializeComponent();

            MinWidth = 100;
            MinHeight = 100;

            Focusable = true;
        }

        protected override void OnRender(
            DrawingContext dc)
        {
            base.OnRender(dc);

            double width = ActualWidth;
            double height = ActualHeight;

            Point center =
                new Point(width / 2, height / 2);

            double radius =
                Math.Min(width, height) / 2 - 20;

            double startAngle = 135;
            double sweepAngle = 270;

            double valuePercent =
                (Value - Minimum) /
                (Maximum - Minimum);

            double indicatorPercent =
                Flipped ?
                1.0 - valuePercent :
                valuePercent;

            double currentAngle =
                startAngle +
                sweepAngle * indicatorPercent;

            //
            // SHADOW
            //
            dc.DrawEllipse(
                new SolidColorBrush(
                    Color.FromArgb(60, 0, 0, 0)),
                null,
                new Point(center.X + 6, center.Y + 6),
                radius,
                radius);

            //
            // OUTER RING
            //
            dc.DrawEllipse(
                new RadialGradientBrush(
                    Color.FromRgb(90, 90, 90),
                    Color.FromRgb(20, 20, 20)),
                null,
                center,
                radius,
                radius);

            //
            // ARC TRACK
            //
            DrawArc(
                dc,
                center,
                radius - 8,
                startAngle,
                sweepAngle,
                new Pen(
                    new SolidColorBrush(
                        Color.FromRgb(50, 50, 50)),
                    10));

            //
            // ACTIVE ARC
            //
            double activeSweep =
                Flipped ?
                sweepAngle * (1.0 - valuePercent) :
                sweepAngle * valuePercent;

            DrawArc(
                dc,
                center,
                radius - 8,
                startAngle,
                activeSweep,
                new Pen(
                    ArcColor,
                    10));

            //
            // INNER KNOB
            //
            double innerRadius =
                radius - 35;

            dc.DrawEllipse(
                new RadialGradientBrush(
                    Color.FromRgb(110, 110, 110),
                    Color.FromRgb(30, 30, 30)),
                null,
                center,
                innerRadius,
                innerRadius);

            //
            // INDICATOR
            //
            double indicatorRad =
                currentAngle *
                Math.PI / 180.0;

            Point indicatorEnd =
                new Point(
                    center.X +
                    Math.Cos(indicatorRad) *
                    (radius - 15),

                    center.Y +
                    Math.Sin(indicatorRad) *
                    (radius - 15));

            dc.DrawLine(
                new Pen(
                    Brushes.WhiteSmoke,
                    5),
                center,
                indicatorEnd);

            //
            // VALUE TEXT
            //
            FormattedText ft =
                new FormattedText(
                    Value.ToString("0.000"),
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),
                    20,
                    Brushes.LimeGreen,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

            dc.DrawText(
                ft,
                new Point(
                    center.X - ft.Width / 2,
                    center.Y + ft.Height * 3 / 2));
        }

        private void DrawArc(
            DrawingContext dc,
            Point center,
            double radius,
            double startAngle,
            double sweepAngle,
            Pen pen)
        {
            if (sweepAngle <= 0)
                return;

            double startRad =
                startAngle * Math.PI / 180.0;

            double endRad =
                (startAngle + sweepAngle) *
                Math.PI / 180.0;

            Point startPoint =
                new Point(
                    center.X +
                    Math.Cos(startRad) * radius,

                    center.Y +
                    Math.Sin(startRad) * radius);

            Point endPoint =
                new Point(
                    center.X +
                    Math.Cos(endRad) * radius,

                    center.Y +
                    Math.Sin(endRad) * radius);

            bool largeArc =
                sweepAngle > 180;

            PathFigure pf =
                new PathFigure
                {
                    StartPoint = startPoint
                };

            pf.Segments.Add(
                new ArcSegment(
                    endPoint,
                    new Size(radius, radius),
                    0,
                    largeArc,
                    SweepDirection.Clockwise,
                    true));

            PathGeometry pg =
                new PathGeometry();

            pg.Figures.Add(pf);

            dc.DrawGeometry(
                null,
                pen,
                pg);
        }

        protected override void OnMouseDown(
            MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            dragging = true;

            CaptureMouse();

            UpdateValueFromMouse(
                e.GetPosition(this));
        }

        protected override void OnMouseMove(
            MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!dragging)
                return;

            UpdateValueFromMouse(
                e.GetPosition(this));
        }

        protected override void OnMouseUp(
            MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            dragging = false;

            ReleaseMouseCapture();
        }

        protected override void OnMouseWheel(
            MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            double step = 0.1;

            if (Keyboard.Modifiers ==
                ModifierKeys.Shift)
            {
                step = 1.0;
            }

            if (Keyboard.Modifiers ==
                ModifierKeys.Control)
            {
                step = 0.01;
            }

            Value +=
                e.Delta > 0 ?
                step :
                -step;
        }

        private void UpdateValueFromMouse(
            Point p)
        {
            Point center =
                new Point(
                    ActualWidth / 2,
                    ActualHeight / 2);

            double dx =
                p.X - center.X;

            double dy =
                p.Y - center.Y;

            double angle =
                Math.Atan2(dy, dx) *
                180.0 / Math.PI;

            if (angle < 0)
                angle += 360;

            double startAngle = 135;
            double sweepAngle = 270;

            if (angle < startAngle - 45)
                angle += 360;

            angle =
                Math.Max(
                    startAngle,
                    Math.Min(
                        startAngle + sweepAngle,
                        angle));

            double percent =
                (angle - startAngle) /
                sweepAngle;

            if (Flipped)
                percent = 1.0 - percent;

            Value =
                Minimum +
                ((Maximum - Minimum) *
                percent);
        }
    }
}