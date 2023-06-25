namespace Endo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    static class Program
    {        
        [STAThread]
        static void Main()
        {
            try
            {
                ClockDataCollection data = ClockDataManager.GetConfig();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);	    
                Application.Run(new ClockContainerForm(data));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}