using Shell32;
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

        private static readonly string FolderIconOverridePathIco = Path.Combine("resources", "folder_override.ico");
        private static readonly string FolderIconOverridePathPng = Path.Combine("resources", "folder_override.png");
        private static readonly string FolderIconOverridePathJpg = Path.Combine("resources", "folder_override.jpg");

        public QuickDirMenuItem(string path) {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                throw new ArgumentException("Input path was empty.", nameof(path));

            bool isDirectory = Directory.Exists(path);

            IsDirectoryItem = isDirectory;
            ItemPath = Path.GetFullPath(path);

            switch (Path.GetExtension(path)) {
                case ".lnk":
                    Text = Path.GetFileNameWithoutExtension(path);
                    break;
                case ".url":
                    Text = Path.GetFileNameWithoutExtension(path);
                    break;
                default:
                    Text = Path.GetFileName(path); //isDirectory ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);
                    break;
            }

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

            if (e.ClickedItem is QuickDirMenuItem item)
                item.OpenItem();
        }

        internal void OpenItem(bool fileOnly = true) {
            if (fileOnly && IsDirectoryItem)
                return;

            using (Process process = Process.Start("explorer", $"\"{ItemPath}\"")) {
            }
        }

        internal void UpdateImage() {
            Image?.Dispose();

            string path = ItemPath;
            if (File.Exists(path)) {
                using (Icon icon = Icon.ExtractAssociatedIcon(path))
                    Image = icon.ToBitmap();
            } else if (Directory.Exists(path)) {
                Image img;
                if (File.Exists(FolderIconOverridePathIco)) {
                    img = Image.FromFile(FolderIconOverridePathIco);
                } else if (File.Exists(FolderIconOverridePathPng)) {
                    img = Image.FromFile(FolderIconOverridePathPng);
                } else if (File.Exists(FolderIconOverridePathJpg)) {
                    img = Image.FromFile(FolderIconOverridePathJpg);
                } else {
                    IconHelper.ExtractAssociatedIcons(path, out Bitmap small, out Bitmap large);

                    img = small;
                    large.Dispose();
                }

                Image = img;
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
