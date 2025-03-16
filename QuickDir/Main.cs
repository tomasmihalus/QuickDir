using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickDir {
    public sealed class Main : ApplicationContext {
        private NotifyIcon Icon = new NotifyIcon() { Icon = QuickResources.TrayIcon, Visible = true };

        private static readonly HashSet<Main> Instances = new HashSet<Main>();
        private static readonly MethodInfo ShowContextMenuMethodInfo = typeof(NotifyIcon)
            .GetMethod(nameof(ShowContextMenu), BindingFlags.Instance | BindingFlags.NonPublic);

        public Main() {
            Instances.Add(this);
            ThreadExit += Main_ThreadExit;

            Icon.ContextMenuStrip = new QuickMenuStrip() { AutoClose = true };
            Icon.MouseClick += (sender, e) => {
                if (e.Button != MouseButtons.Left)
                    return;

                ShowContextMenu();
            };
        }

        private void Main_ThreadExit(object sender, EventArgs e) {
            Icon?.Dispose();
            Icon = null;
        }

        private void ShowContextMenu() {
            ShowContextMenuMethodInfo.Invoke(Icon, null);
        }

        public static void ExitApplication(object sender, EventArgs e) {
            foreach (Main instance in Instances)
                instance.Main_ThreadExit(sender, e);

            Application.Exit();
        }
    }
}
