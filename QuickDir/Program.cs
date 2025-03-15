using QuickDir.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickDir {
    internal static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            List<MenuItem> items = new List<MenuItem>();

            if (File.Exists("user.cfg")) {
                foreach (string line in File.ReadLines("user.cfg")) {

                }
            }

            using (NotifyIcon icon = new NotifyIcon()) {
                icon.Icon = QuickResources.tray_icon;
                icon.ContextMenu = new ContextMenu();
            }

            Application.Exit();
        }
    }
}
