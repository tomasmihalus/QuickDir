using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuickDir {
    public static class IconHelper {
        public static void ExtractAssociatedIcons(string path, out Bitmap small, out Bitmap large) {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path) && !Directory.Exists(path)) {
                small = QuickResources.MissingFileIcon.ToBitmap();
                large = QuickResources.MissingFileIcon.ToBitmap();
                return;
                //throw new FileNotFoundException($"Could not find file of directory '{path}'.", Path.GetFileName(path));
            }

            NativeHelper.SHFILEINFO shinfo = new NativeHelper.SHFILEINFO();
            // extract small icon
            {
                NativeHelper.SHGetFileInfo(path, 0U, ref shinfo, (uint)Marshal.SizeOf<NativeHelper.SHFILEINFO>(),
                    NativeHelper.SHGFI_ICON | NativeHelper.SHGFI_SMALLICON);

                if (shinfo.hIcon != IntPtr.Zero) {
                    small = Bitmap.FromHicon(shinfo.hIcon);
                    NativeHelper.DestroyIcon(shinfo.hIcon);
                } else {
                    using (Icon temp = Icon.ExtractAssociatedIcon(path))
                        small = temp.ToBitmap();
                }
            }

            // extract large icon
            {
                NativeHelper.SHGetFileInfo(path, 0U, ref shinfo, (uint)Marshal.SizeOf<NativeHelper.SHFILEINFO>(),
                    NativeHelper.SHGFI_ICON | NativeHelper.SHGFI_LARGEICON);

                if (shinfo.hIcon != IntPtr.Zero) {
                    large = Bitmap.FromHicon(shinfo.hIcon);
                    NativeHelper.DestroyIcon(shinfo.hIcon);
                } else {
                    using (Icon temp = Icon.ExtractAssociatedIcon(path))
                        large = temp.ToBitmap();
                }
            }
        }
    }
}
