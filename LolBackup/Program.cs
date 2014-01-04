using System;
using System.Windows.Forms;
using System.Threading;

namespace LolBackup
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ensure only one instance of app is running
            bool firstInstance;
            new Mutex(false, "Local\\LolBackup" , out firstInstance);
            if (!firstInstance)
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
