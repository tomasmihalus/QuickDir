using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickDir {
    public sealed class QuickDirMenuItem : ToolStripMenuItem {
        public bool IsDirectoryItem { get; }
        public string ItemPath { get; }

        public QuickDirMenuItem(string path) {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("Input path was empty.", nameof(path));

            bool isDirectory = Directory.Exists(path);

            IsDirectoryItem = isDirectory;
            ItemPath = Path.GetFullPath(path);
            Text = Path.GetFileName(path); //isDirectory ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);

            UpdateImage();
            UpdateItems(quickUpdate: true);
        }

        protected override void Dispose(bool disposing) {
            Image?.Dispose();
            Image = null;

            base.Dispose(disposing);
        }

        protected override void OnDropDownShow(EventArgs e) {
            UpdateItems(quickUpdate: false);
            base.OnDropDownShow(e);
        }

        protected override void OnDropDownItemClicked(ToolStripItemClickedEventArgs e) {
            base.OnDropDownItemClicked(e);

            if (!(e.ClickedItem is QuickDirMenuItem item) || item.IsDirectoryItem)
                return;

            using (Process process = Process.Start("explorer", $"\"{item.ItemPath}\"")) {
            }
        }

        internal void UpdateImage() {
            Image?.Dispose();

            string path = ItemPath;
            if (File.Exists(path)) {
                using (Icon icon = Icon.ExtractAssociatedIcon(path))
                    Image = icon.ToBitmap();
            } else if (Directory.Exists(path)) {
                IconHelper.ExtractAssociatedIcons(path, out Bitmap small, out Bitmap large);
                Image = small;

                large.Dispose();
            } else {
                Image = QuickResources.MissingFileIcon.ToBitmap();
            }
        }

        internal void UpdateItems(bool quickUpdate) {
            if (!IsDirectoryItem)
                return;

            string dir = ItemPath;

            ToolStripItemCollection items = DropDownItems;
            items.Clear();

            if (quickUpdate) {
                if (Directory.EnumerateDirectories(dir).Any() || Directory.EnumerateFiles(dir).Any())
                    items.Add(new QuickMenuTempItem());
            } else {
                string[] directories = Directory.GetDirectories(dir);
                string[] files = Directory.GetFiles(dir);
                ToolStripMenuItem[] newItems = new ToolStripMenuItem[directories.Length + files.Length];

                int index = 0;
                foreach (string subdir in directories) {
                    QuickDirMenuItem item = new QuickDirMenuItem(subdir);
                    item.UpdateImage();

                    newItems[index++] = item;
                }

                foreach (string filepath in files) {
                    QuickDirMenuItem item = new QuickDirMenuItem(filepath);
                    item.UpdateImage();

                    newItems[index++] = item;
                }

                items.AddRange(newItems);
            }
        }
    }
}
