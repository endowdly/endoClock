namespace Endo
{
	using System;
    	using System.Collections.Generic;
    	using System.Drawing;
    	using System.Drawing.Drawing2D;
    	using System.Linq;
    	using System.Windows.Forms;

	partial class ClockContainerForm : Form
	{
	
		protected override void OnKeyDown(KeyEventArgs x)
		{
	  		base.OnKeyDown(x);

			if (x.KeyCode == Keys.F1)
				ShowHelp();

			if (x.Control && x.KeyCode == Keys.OemPeriod)
				IncreaseTickThickness();
		
			if (x.Control && x.KeyCode == Keys.Oemcomma)
				DecreaseTickThickness();

			if (x.Control && x.KeyCode == Keys.OemOpenBrackets)
				DecreaseFontSize();

			if (x.Control && x.KeyCode == Keys.OemCloseBrackets)
				IncreaseFontSize();

			if (x.Control && x.KeyCode == Keys.H)
				ToggleShowClockName();

			if (x.Control && x.KeyCode == Keys.F)
				ToggleShowClockFace();

			if (x.Control && x.KeyCode == Keys.Oemplus)
				IncreaseSize();
		
			if (x.Control && x.KeyCode == Keys.OemMinus)
				DecreaseSize();

			if (x.KeyCode == Keys.F11)
				ToggleFullScreen();

			if (x.Alt && x.KeyCode == Keys.R)
				HotReload();
			
			if (x.Control && x.KeyCode == Keys.Q || x.KeyCode == Keys.X)
				Quit();

			if (x.Alt && x.KeyCode == Keys.F)
				FlipFill();
		}

		void FlipFill()
		{
			if (fillStyle == FillStyleMode.LeftToRight)
			{
				fillStyle = FillStyleMode.TopToBottom;
			}
			else
			{
				fillStyle = FillStyleMode.LeftToRight;
			}

			ResetSize();
		}
	
		void Quit()
		{
			const string message = "Are you sure you want to close the clock overlay?";
			const string caption = "Exit";
	
			var canQuit = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			
			if (canQuit == DialogResult.Yes)
			{
				Application.Exit();
			}
		}

		void ShowHelp()
		{
			MessageBox.Show("Help is a work in progress : (");
		}

		void HotReload()
		{	
			
			try
			{
				ClockDataCollection data = ClockDataManager.GetConfig();
				Controls.Clear();
			
				var clockData = data.ClockData;
			
				clocks = clockData.Select(datum => new ClockControl(datum)).ToArray();

				foreach (var clock in clocks)
					Controls.Add(clock);
				ResetSize();
			}
			catch 
			{
				MessageBox.Show("Error with config file!");
				return;
			}
		
			
		}

		// Psuedo fullscreen as the form should always meet screen bounds.
		void ToggleFullScreen()
		{
			if (FormBorderStyle == FormBorderStyle.None)
			{
				FormBorderStyle = FormBorderStyle.Sizable;
			}
			else
			{
				FormBorderStyle = FormBorderStyle.None;
			}
		
		}

		void IncreaseTickThickness()
		{
			foreach (var clock in clocks)
				clock.TickThickness += 0.5f;

			Refresh();
		}

		void DecreaseTickThickness()
		{
			foreach (var clock in clocks)
				clock.TickThickness -= 0.5f;

			Refresh();
		}

		// The size controls circumvent the ClockCellMatrix autosizing and exception avoidance.
		// Easiest solution is just to catch and release.
		void IncreaseSize()
		{
			clockRadius += 5;
			
			try
			{
				ResetSize();
			} 
			catch { } 
		}

		void DecreaseSize()
		{
			clockRadius -= 5;
			
			try
			{
				ResetSize();
			}
			catch { }
		}

		void DecreaseFontSize()
		{
			foreach (var clock in clocks)
				clock.FaceFontSize--;
				
			Refresh();
		}
		
		void IncreaseFontSize()
		{
			foreach (var clock in clocks)
				clock.FaceFontSize++;
			Refresh();
		}

		void ToggleShowClockName()
		{
			foreach (var clock in clocks)
				clock.CanDrawName = !clock.CanDrawName;

			Refresh();	
		}


		void ToggleShowClockFace()
		{
			foreach (var clock in clocks)
				clock.CanDrawFace = !clock.CanDrawFace;

			Refresh();	
		}
	}
}