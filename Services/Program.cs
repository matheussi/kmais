using System;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Services
{
    static class Program
    {
        static Boolean PrevInstance()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //if (PrevInstance())
            //{
            //    MessageBox.Show("A aplicação ja está em execução.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    Application.Exit();
            //    return;
            //}

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
