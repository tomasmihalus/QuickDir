using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickDir {
    public sealed class QuickDirMenuColorTable : ProfessionalColorTable {
        private const double BrightnessMultiplier = 0.95;

        public override Color ImageMarginGradientBegin
            => MultiplyColor(base.ImageMarginGradientBegin, BrightnessMultiplier);

        public override Color ImageMarginGradientEnd
            => MultiplyColor(base.ImageMarginGradientEnd, BrightnessMultiplier);

        public override Color ImageMarginGradientMiddle
            => MultiplyColor(base.ImageMarginGradientMiddle, BrightnessMultiplier);

        private Color MultiplyColor(Color color, double multiplier) {
            double r = color.R * multiplier;
            double g = color.G * multiplier;
            double b = color.B * multiplier;

            return Color.FromArgb(color.A, (byte)r, (byte)g, (byte)b);
        }
    }
}
