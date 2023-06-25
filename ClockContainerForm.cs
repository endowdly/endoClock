﻿namespace Endo
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Linq;
    using System.Windows.Forms;

    partial class ClockContainerForm : Form
    {
        const int Spacing = 13;

        readonly Color transColor = Color.Silver;

        ClockCellMatrix grid;
        Rectangle screenBounds; 

        Timer timer = new Timer();
        System.ComponentModel.Container components = null;

        public int ClockRadius; 
        public ClockControl[] Clocks;
        public FillStyleMode FillStyle;

        public ClockContainerForm(ClockDataCollection c)
        {
            var data = c.ClockData;

            screenBounds = Screen.FromControl(this).Bounds;
            Clocks = data.Select(datum => new ClockControl(datum)).ToArray();
            ClockRadius = c.ClockSize;
            FillStyle = c.FillStyle;
            timer.Interval = 1000;

            while (Clocks.Length > GetMaxClocks())
                ClockRadius--;

            ResetSize();
            InitializeComponent();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing && (components != null))
                components.Dispose();

            base.Dispose(isDisposing);
        }

        protected override void OnResize(EventArgs x)
        {
            base.OnResize(x);
            ResetSize();
        }

        void InitializeComponent()
        {
            foreach (var clock in Clocks)
                Controls.Add(clock);

            Text = "Clock";
            KeyPreview = true;
            BackColor = transColor;
            TransparencyKey = transColor;
            ShowIcon = false;
            Bounds = screenBounds;
            TopMost = true;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 10);
            timer.Tick += new EventHandler(timer_OnTick);

            timer.Start();
        }

        int GetRows()
        {
            return (int)Math.Floor((double)screenBounds.Height / (ClockRadius + Spacing));
        }

        int GetCols()
        {
            return (int)Math.Floor((double)screenBounds.Width / (ClockRadius + Spacing));
        }

        int GetMaxClocks()
        {
            return GetRows() * GetCols();
        }

        void FillGrid(FillStyleMode fillStyle)
        {
            switch (fillStyle)
            {
                case FillStyleMode.LeftToRight:
                    FillLeftToRight();
                    break;

                case FillStyleMode.TopToBottom:
                    FillTopToBottom();
                    break;
            }
        }

        void FillLeftToRight()
        {
            for (var i = 0; i < Clocks.Length; i++)
            {
                var cell = grid[i];

                cell.IsClock = true;
                cell.Clock = Clocks[i];
            }
        }

        void FillTopToBottom()
        { 
            int j = 0;

            for (var i = 0; i < Clocks.Length; i++)
            {
                var cell = grid[j];

                cell.IsClock = true;
                cell.Clock = Clocks[i];

                j = grid.GetBelow(j).Index;
            }
        }

        void SetClocksize()
        {
            var clockCells = grid.Where(cell => cell.IsClock);

            foreach (var cell in clockCells)
            {
                cell.Clock.Location = cell.Location;
                cell.Clock.Size = new Size(ClockRadius, ClockRadius);
                cell.Clock.Margin = new Padding(Spacing);
            }
        }

        void GetGrid()
        {
            grid = new ClockCellMatrix(GetCols(), GetRows(), ClockRadius + Spacing);
        }

        void ResetSize()
        {
            GetGrid();
            FillGrid(FillStyle);
            SetClocksize();
        }

        void timer_OnTick(object sender, EventArgs e)
        {
            foreach (var clock in Clocks)
                clock.UpdateTime(DateTime.Now);

            Refresh();
        }
    }
}