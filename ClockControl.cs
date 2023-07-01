﻿namespace Endo
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    [Flags]
    enum ClockState
    {
        Normal = 0,
        Afternoon = 1,
        Ahead = 2,
        Behind = 4, 
    }

    class ClockControl : UserControl
    {
        const double Tau = Math.PI * 2;    // One full turn or circle.  
        const double Sixtieth = Tau / 60;  // 6 degrees represented in rads
        const double Twelfth = Tau / 12;   // 30 degrees represented in rads
        const double Fourth = Tau / 4;     // 90 degrees represented in rads (fourth tau or half pi)
        const string Ahead = "+1";
        const string Behind = "-1";

        static readonly TimeSpace Noon = new TimeSpace(12, 0, 0);

        System.ComponentModel.Container components = null;

        string dayLeadLag = string.Empty; 
        ClockState state;
        Point p0;  // This is the origin point or the center of the clock.
        int r;     // The radius of the clock.

        int s, m, h;
        double fMin, fHr;
        double aSec, aMin, aHr;

        int fontFaceSize = 10; 
        float tickThickness = 1.0f;

        public readonly string ClockName;
        public readonly TimeZoneInfo TimeZone;

        public bool CanDrawHourTicks;
        public bool CanDrawMinuteTicks;
        public bool CanDrawFace;
        public bool CanDrawName; 
        public bool CanDrawSecondHand;
        public Color ColorHourHand;
        public Color ColorMinuteHand;
        public Color ColorSecondHand;
        public Color ColorFace;
        public Color ColorIndicator;
        public Color ColorHourTick;
        public Color ColorMinuteTick;
        public Color ColorText;
        public FontFamily FaceFont;

        public DateTime Time;

        public int FaceFontSize
        {
            get { return faceFontSize; }
            set 
            {
                if (value > 0)
                    fontFaceSize = FaceFontSize;
            }
        }

        public float TickThickness
        {
            get { return tickThickness; }
            set 
            {
                if (value > 0)
                    fontFaceSize = FaceFontSize;
            }
        }

        public ClockControl(ClockData data)
        {
            ColorHourHand = ColorTranslator.FromHtml(data.Colors.HourHand);
            ColorMinuteHand = ColorTranslator.FromHtml(data.Colors.MinuteHand);
            ColorSecondHand = ColorTranslator.FromHtml(data.Colors.SecondHand);
            ColorIndicator = ColorTranslator.FromHtml(data.Colors.Indicator); 
            ColorFace = ColorTranslator.FromHtml(data.Colors.Face);
            ColorHourTick = ColorTranslator.FromHtml(data.Colors.HourTick);
            ColorMinuteTick = ColorTranslator.FromHtml(data.Colors.MinuteTick);
            ColorText = ColorTranslator.FromHtml(data.Colors.Text);
            CanDrawHourTicks = data.ShowHourTicks;
            CanDrawMinuteTicks = data.ShowMinuteTicks;
            CanDrawName = data.ShowLabel;
            CanDrawSecondHand = data.ShowSecondHand;
            FaceFont = new FontFamily(data.FaceFont);
            ClockName = data.Label;
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById(data.TimeZoneId);

            UpdateTime();
            ResetSize();
            InitializeComponent();
        }

        protected override void OnResize(EventArgs x)
        {
            base.OnResize(x);
            ResetSize();
        }

        protected override void OnPaint(PaintEventArgs x)
        {
            base.OnPaint(x);

            // These options let the clock run smoothly and look better.
            x.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            x.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

            s = Time.Second;
            m = Time.Minute;
            h = Time.Hour;
            fMin = (m + s / 60.0);    // Fraction minute-- minute hand moves in between ticks
            fHr = (h + fMin / 60.0);  // Fractional hour-- hour hand moves in between ticks

            // The angles of the hands in radians. Pi is towards the 0,1 direction.
            // Since noon is at the 0,0 direction, we have to rotate backwards by a fourth.
            aSec = Fourth - (Sixtieth * s);
            aMin = Fourth - (Sixtieth * fMin);
            aHr = Fourth - (Twelfth * fHr);

            DrawFace(x.Graphics);

            if (state.HasFlag(ClockState.Ahead))
                dayLeadLag = Ahead;

            if (state.HasFlag(ClockState.Behind))
                dayLeadLag = Behind; 

            if (CanDrawName)
                DrawText(x.Graphics, ClockName, p0.X, p0.Y + r / 6);

            DrawText(x.Graphics, dayLeadLag, p0.X, p0.Y + r / 3); 

            if (CanDrawSecondHand)
                DrawLine(x.Graphics, new Pen(ColorSecondHand, TickThickness), aSec, 0.95);

            DrawLine(x.Graphics, new Pen(ColorMinuteHand, TickThickness), aMin, 0.9);
            DrawLine(x.Graphics, new Pen(ColorHourHand, TickThickness), aHr, 0.6);
        }

        void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DoubleBuffered = true;
        }

        void ResetSize()
        {
            p0 = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
            r = Math.Min(p0.X, p0.Y) - Margin.All;

            Refresh();
        }

        void DrawLine(Graphics g, Pen pen, double angle, double length)
        {
            int x0 = p0.X + (length > 0 ? 0 : Convert.ToInt32(r * (1 + length) * Math.Cos(angle)));
            int y0 = p0.Y + (length > 0 ? 0 : Convert.ToInt32(r * (1 + length) * Math.Sin(-angle)));
            int x1 = p0.X + Convert.ToInt32(r * (length > 0 ? length : 1) * Math.Cos(angle));
            int y1 = p0.Y + Convert.ToInt32(r * (length > 0 ? length : 1) * Math.Sin(-angle));

            g.DrawLine(pen, x0, y0, x1, y1);
        }

        void DrawFace(Graphics g)
        { 
            if (CanDrawFace)
                g.FillEllipse(new SolidBrush(ColorFace), p0.X - r, p0.Y - r, r * 2, r * 2);

            if (state.HasFlag(ClockState.Afternoon))
                g.DrawEllipse(new Pen(ColorIndicator, TickThickness), p0.X - r / 12,  p0.Y - r / 2, r / 6, r / 6);

            for (var i = 0; i <= 60; i++)
            {
                if (i % 5 == 0 && CanDrawHourTicks)
                    DrawLine(g, new Pen(ColorHourTick, TickThickness), i * Twelfth, -0.05);
                else if (CanDrawMinuteTicks)
                    DrawLine(g, new Pen(ColorMinuteTick, TickThickness), i * Sixtieth, -0.025);
            }
        }

        void DrawText(Graphics g, string s, float x, float y)
        {
            int fontSize;
            SizeF newSize;
            Font tFont;

            var drawFont = new Font(FaceFont, FaceFontSize);
            var drawBrush = new SolidBrush(ColorText);
            var drawFormat = new StringFormat();

            drawFormat.Alignment = StringAlignment.Center;

            for (fontSize = 01; fontSize <= FaceFontSize; fontSize++)
            {
                tFont = new Font(drawFont.Name, fontSize, drawFont.Style);
                newSize = g.MeasureString(s, tFont);

                if ((r * 0.55) > Convert.ToInt32(newSize.Width))
                    drawFont = tFont;
            }

            g.DrawString(s, drawFont, drawBrush, x, y, drawFormat);
        }

        public void UpdateTime(DateTime t)
        {
            Time = TimeZoneInfo.ConvertTime(t, TimeZoneInfo.Local, TimeZone);

            if (Time.TimeOfDay > Noon)
                state |= ClockState.Afternoon;

            if (Time.Date > t.Date)
                state |= ClockState.Ahead;
            
            if (Time.Date < t.Date)
                state |= ClockState.Behind;
        }

        public void UpdateTime()
        {
            UpdateTime(DateTime.Now);
        }
    }
}