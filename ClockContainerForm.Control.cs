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

            if (x.Alt && x.KeyCode == Keys.S)
                ToggleShowSecondHand();

            if (x.Control && x.KeyCode == Keys.S)
                Save();
        }

        void FlipFill()
        {
            if (FillStyle == FillStyleMode.LeftToRight)
                FillStyle = FillStyleMode.TopToBottom;
            else
                FillStyle = FillStyleMode.LeftToRight;

            ResetSize();
        }

        void Quit()
        {
            const string message = "Are you sure you want to close the clock overlay?";
            const string caption = "Exit";

            var canQuit = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (canQuit == DialogResult.Yes)
                Application.Exit();
        }

        void Save()
        {
            const string message = "Are you sure you want to save the current overlay?";
            const string caption = "Save";

            var canSave = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (canSave == DialogResult.Yes)
                ClockDataManager.Save(this);
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

                Clocks = clockData.Select(datum => new ClockControl(datum)).ToArray();

                foreach (var clock in Clocks)
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
            try
            {
                foreach (var clock in Clocks)
                    clock.TickThickness += 0.5f;

                Refresh();
            }
            catch { }
        }

        void DecreaseTickThickness()
        {
            try
            {
                foreach (var clock in Clocks)
                    clock.TickThickness -= 0.5f;

                Refresh();

            }
            catch { }
        }

        // The size controls circumvent the ClockCellMatrix autosizing and exception avoidance.
        // Easiest solution is just to catch and release.
        void IncreaseSize()
        {
            ClockRadius += 5;

            try
            {
                ResetSize();
            }
            catch { }
        }

        void DecreaseSize()
        {
            ClockRadius -= 5;

            try
            {
                ResetSize();
            }
            catch { }
        }

        void DecreaseFontSize()
        {
            try
            {
                foreach (var clock in Clocks)
                    clock.FaceFontSize--;

                Refresh();
            }
            catch { }
        }

        void IncreaseFontSize()
        {
            try
            {
                foreach (var clock in Clocks)
                    clock.FaceFontSize++;

                Refresh();
            }
            catch { }
        }

        void ToggleShowClockName()
        {
            foreach (var clock in Clocks)
                clock.CanDrawName = !clock.CanDrawName;

            Refresh();
        }


        void ToggleShowClockFace()
        {
            foreach (var clock in Clocks)
                clock.CanDrawFace = !clock.CanDrawFace;

            Refresh();
        }

        void ToggleShowSecondHand()
        {
            foreach (var clock in Clocks)
                clock.CanDrawSecondHand = !clock.CanDrawSecondHand;

            Refresh();
        }
    }
}