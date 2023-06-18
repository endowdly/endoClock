﻿namespace Endo
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    class ClockControl : UserControl
    {
        const double Tau = Math.PI * 2;    // One full turn or circle.  
        const double Sixtieth = Tau / 60;  // 6 degrees represented in rads
        const double Twelfth = Tau / 12;   // 30 degrees represented in rads
        const double Fourth = Tau / 4;     // 90 degrees represented in rads

        System.ComponentModel.Container components = null;

        Point p0;
        int r;

        int s, m, h;

        double fMin, fHr;
        double aSec, aMin, aHr;

        public readonly string ClockName;
        public readonly TimeZoneInfo TimeZone;

        public bool CanDrawHourTicks = true;
        public bool CanDrawMinuteTicks = true;
        public bool CanDrawFace = false;
	public bool CanDrawName = true;
        public float TickThickness = 1.0f;
        public Color ColorHourHand;
        public Color ColorMinuteHand;
        public Color ColorSecondHand;
        public Color ColorFace;
        public Color ColorHourTick;
        public Color ColorMinuteTick;
        public Color ColorText;
        public FontFamily FaceFont;
	public int FaceFontSize;

        public DateTime Time;

        public ClockControl(ClockData data)
        {
            ColorHourHand = ColorTranslator.FromHtml(data.Colors.HourHand);
            ColorMinuteHand = ColorTranslator.FromHtml(data.Colors.MinuteHand);
            ColorSecondHand = ColorTranslator.FromHtml(data.Colors.SecondHand);
            ColorFace = ColorTranslator.FromHtml(data.Colors.Face);
            ColorHourTick = ColorTranslator.FromHtml(data.Colors.HourTick);
            ColorMinuteTick = ColorTranslator.FromHtml(data.Colors.MinuteTick);
            ColorText = ColorTranslator.FromHtml(data.Colors.Text);
	    CanDrawHourTicks = data.ShowHourTicks;
	    CanDrawMinuteTicks = data.ShowMinuteTicks;
	    CanDrawName = data.ShowLabel;
            FaceFont = new FontFamily(data.FaceFont);
            ClockName = data.Label;
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById(data.TimeZoneId);
	    FaceFontSize = 10;

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

            x.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            x.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            s = Time.Second;
            m = Time.Minute;
            h = Time.Hour;
            fMin = (m + s / 60.0);
            fHr = (h + fMin / 60.0);
            aSec = Fourth - (Sixtieth * s);
            aMin = Fourth - (Sixtieth * fMin);
            aHr = Fourth - (Twelfth * fHr);

	    DrawFace(x.Graphics);

            if (CanDrawName)
		DrawText(x.Graphics, ClockName);

            DrawLine(x.Graphics, new Pen(ColorSecondHand, TickThickness), aSec, 0.95);
            DrawLine(x.Graphics, new Pen(ColorMinuteHand, TickThickness), aMin, 0.9);
            DrawLine(x.Graphics, new Pen(ColorHourHand, TickThickness), aHr, 0.6);
        }

        void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
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
            int i;

            if (CanDrawFace)
                g.FillEllipse(new SolidBrush(ColorFace), p0.X - r, p0.Y - r, r * 2, r * 2);

            for (i = 0; i <= 60; i++)
            {
                if (i % 5 == 0 && CanDrawHourTicks)
                    DrawLine(g, new Pen(ColorHourTick, TickThickness), i * Twelfth, -0.05);
                else if (CanDrawMinuteTicks)
                    DrawLine(g, new Pen(ColorMinuteTick, TickThickness), i * Sixtieth, -0.025);
            }
        }

        void DrawText(Graphics g, string s)
        {
	    int fontSize;
            SizeF newSize;
            Font tFont;

            var drawFont = new Font(FaceFont, FaceFontSize);
            var drawBrush = new SolidBrush(ColorText);
            var drawFormat = new StringFormat();
            var x = p0.X;
            var y = p0.Y + r / 6;

            drawFormat.Alignment = StringAlignment.Center;

            for (fontSize = 04; fontSize <= FaceFontSize; fontSize++)
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
        }

	public void UpdateTime()
	{
	    UpdateTime(DateTime.Now);
	}
    }
}