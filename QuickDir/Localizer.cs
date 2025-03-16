using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDir {
    public static class Localizer {
        public static void SetLanguage(string language) {
            string path = Path.Combine("resources", $"{language}.loc");
            if (!File.Exists(path))
                throw new FileNotFoundException($"Could not find localization file '{path}'.", Path.GetFileName(path));

            Dictionary<string, string> loc = s_Localization;
            loc.Clear();

            foreach (string line in File.ReadLines(path)) {
                int index = line.IndexOf('=');
                if (index == -1)
                    continue;

                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1).Trim();
                loc[key] = value;
            }
        }

        public static string GetLocalizedString(string label, string defaultValue) {
            return s_Localization.TryGetValue(label, out string result) ? result : defaultValue;
        }

        private static readonly Dictionary<string, string> s_Localization = new Dictionary<string, string>();
    }
}
