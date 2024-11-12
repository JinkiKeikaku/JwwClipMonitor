using JwwClipMonitor.Properties;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace JwwClipMonitor.Jww.Shape
{
    internal class StyleConverter
    {
        private JwwHelper.JwwReader StyleReader = new();
        private Dictionary<int, Color> mColorMap = new();

        public StyleConverter()
        {
            var buf = Resources.template;
            StyleReader.Read(buf, Completed);
        }

        private void Completed(JwwHelper.JwwReader reader)
        {
            for (var i = 0; i < 10; i++)
            {
                var c = (int)StyleReader.Header.m_aPenColor[i];
                var col = c.ToColor();
                if (col == Color.FromArgb(255, 255, 255)) col = Color.Black;
                mColorMap[i] = col;
            }
            for (var i = 0; i <= 256; i++)
            {
                var c = (int)StyleReader.Header.m_aPenColor_SXF[i];
                var col = c.ToColor();
                if (col == Color.FromArgb(255, 255, 255))
                {
                    col = Color.Black;
                }
                mColorMap[i + 100] = col;
            }
        }


        public Color PenToColor(int pen)
        {
            return mColorMap.GetValueOrDefault(pen, Color.Black);
        }
    }
}
