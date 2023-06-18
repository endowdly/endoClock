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
        const int DefaultRadius = 250;
        const int Spacing = 24;

	readonly Color transColor = Color.Silver;

        int clockRadius;

        ClockControl[] clocks;
        ClockCellMatrix grid;
        Rectangle screenBounds;

        FillStyleMode fillStyle = FillStyleMode.LeftToRight;
        Timer timer = new Timer();
	System.ComponentModel.Container components = null;

        public ClockContainerForm(ClockDataCollection c)
        {
            var data = c.ClockData;

            screenBounds = Screen.FromControl(this).Bounds;
            clocks = data.Select(datum => new ClockControl(datum)).ToArray();
            clockRadius = DefaultRadius;
	    timer.Interval = 1000;

            while (clocks.Length > GetMaxClocks())
            {
                clockRadius--;
            }
		
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
	    foreach (var clock in clocks)
		Controls.Add(clock);
	    
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
            return (int)Math.Floor((double)screenBounds.Height / (clockRadius + Spacing));
        }

        int GetCols()
        {
            return (int)Math.Floor((double)screenBounds.Width / (clockRadius + Spacing));
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
            int i;

            for (i = 0; i < clocks.Length; i++)
            {
                var cell = grid[i];

                cell.IsClock = true;
                cell.Clock = clocks[i];
            }
        }

        void FillTopToBottom()
        {
            int i;

            var j = 0;

            for (i = 0; i < clocks.Length; i++)
            {
                var cell = grid[j];

                cell.IsClock = true;
                cell.Clock = clocks[i];

                j = grid.GetBelow(j).Index;
            }
        }

	void SetClockSize()
	{
		var clockCells = grid.Where(cell => cell.IsClock);

		foreach (var cell in clockCells)
		{
			cell.Clock.Location = cell.Location;
			cell.Clock.Size = new Size(clockRadius, clockRadius);
			cell.Clock.Margin = new Padding(Spacing);
		}
	}

	void GetGrid()
	{
            grid = new ClockCellMatrix(GetCols(), GetRows(), clockRadius + Spacing);
	}

	void ResetSize()
	{
	    GetGrid();
	    FillGrid(fillStyle);
	    SetClockSize();
	}

        void timer_OnTick(object sender, EventArgs e)
        {
            foreach (var clock in clocks)
                clock.UpdateTime(DateTime.Now);

            Refresh();
        }
    }
}