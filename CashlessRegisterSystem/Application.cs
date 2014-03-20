using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ViltjesSysteem
{
    public static class App
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GUI());
        }
    }
}
