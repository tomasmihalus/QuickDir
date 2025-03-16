using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickDir {
    public sealed class QuickMenuStrip : ContextMenuStrip {
        public QuickMenuStrip() {
            UpdateItems();
        }

        private void AddQuickDirectory(object sender, EventArgs e) {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            string path = dialog.SelectedPath;
            if (!Directory.Exists(path)) {
                MessageBox.Show(
                    Localizer.GetLocalizedString("invalid_dir_selected_text", "Specified path was not a valid directory."),
                    Localizer.GetLocalizedString("invalid_dir_selected_caption", "Invalid directory"));
                return;
            }

            string configPath = QuickResources.UserConfigFile;
            List<string> paths;

            if (File.Exists(configPath)) {
                paths = new List<string>(File.ReadLines(configPath)) { path };
            } else {
                paths = new List<string>() { path };
            }

            paths.Sort();
            File.WriteAllLines(configPath, paths);

            UpdateItems();
        }

        private void UpdateItems() {
            ToolStripItemCollection items = Items;
            items.Clear();

            items.Add("+", null, AddQuickDirectory);
            items.Add(new ToolStripSeparator());
            
            string configPath = QuickResources.UserConfigFile;
            if (File.Exists(configPath)) {
                foreach (string line in File.ReadLines(configPath)) {
                    if (!Directory.Exists(line))
                        continue;

                    QuickDirMenuItem item = new QuickDirMenuItem(line);
                    items.Add(item);
                }
            } else {
                File.Create(configPath).Close();
            }

            if (items.Count > 2)
                items.Add(new ToolStripSeparator());

            items.Add("Exit", null, Main.ExitApplication);
        }
    }
}
