using JwwClipMonitor.Properties;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace JwwClipMonitor.Jww.Shape
{
    internal class StyleConverter
    {
        const float JWW_LINETYPE_PITCH_BASE = 1.0f;
        private JwwHelper.JwwReader StyleReader = new();
        private Dictionary<int, Color> mColorMap = new();
        private Dictionary<int, float[]?> mLinePatternMap = new();

        public StyleConverter()
        {
            var buf = Resources.template;
            StyleReader.Read(buf, Completed);
        }

        private void Completed(JwwHelper.JwwReader reader)
        {
            var header = StyleReader.Header;
            for (var i = 0; i < 10; i++)
            {
                var c = (int)header.m_aPenColor[i];
                var col = c.ToColor();
                if (col == Color.FromArgb(255, 255, 255)) col = Color.Black;
                mColorMap[i] = col;
            }
            for (var i = 0; i <= 256; i++)
            {
                var c = (int)header.m_aPenColor_SXF[i];
                var col = c.ToColor();
                if (col == Color.FromArgb(255, 255, 255))
                {
                    col = Color.Black;
                }
                mColorMap[i + 100] = col;
            }
            mLinePatternMap[0] = null;
            mLinePatternMap[1] = null;
            for (var j = 0; j < header.m_alLType.Length; j++)
            {
                var pitch = header.m_anTokushuSenPich[j] * JWW_LINETYPE_PITCH_BASE;
                var pat = CreateDotPattern(header.m_alLType[j], pitch);
                if (pat?.Length == 0) pat = null;
                mLinePatternMap[j + 2] = pat;
            }
            for (var j = 0; j < header.m_alLType_Double.Length; j++)
            {
                var pitch = header.m_anPrtTokushuSenPich[j] * JWW_LINETYPE_PITCH_BASE;
                var pat = CreateDotPattern(header.m_alLType_Double[j], pitch);
                if (pat?.Length == 0) pat = null;
                mLinePatternMap[j + 16] = pat;
            }
            for (var j = 0; j < header.m_alLType_SXF.Length; j++)
            {
                var pitch = header.m_anTokushuSenPich_SXF[j] * JWW_LINETYPE_PITCH_BASE;
                var pat = CreateDotPattern(header.m_alLType_SXF[j], pitch);
                if (pat?.Length == 0) pat = null;
                mLinePatternMap[j + 30] = pat;
            }
        }


        public Color PenToColor(int pen)
        {
            return mColorMap.GetValueOrDefault(pen, Color.Black);
        }

        public float[]? PenStyleToPattern(int style)
        {
            return mLinePatternMap.GetValueOrDefault(style, null);
        }

        static public float[]? CreateDotPattern(uint jwwLinePattern, float pitch)
        {
            var basePattern = CreateDotPatternSub(jwwLinePattern, pitch);
            if (basePattern == null) return null;
            if ((basePattern.Length & 1) != 0) return basePattern;
            for(var k = 2; k <= basePattern.Length / 2; k += 2)
            {
                if(basePattern.Length % k != 0) continue;
                var flag = true;
                for(var j = k; j < basePattern.Length; j+= k)
                {
                    for(var i=0; i < k; i++)
                    {
                        if (basePattern[i] != basePattern[i + j])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (!flag) break;
                }
                if(flag) return basePattern[..k];
            }
            return basePattern;

        }

        static public float[]? CreateDotPatternSub(uint jwwLinePattern, float pitch)
        {
            if (jwwLinePattern == 0 || pitch <= 0) return null;
            var dot = new List<int>();
            var spc = new List<int>();
            uint b = 0x8000_0000;
            var dc = 0;
            var sc = 0;
            for (var i = 0; i < 32; i++)
            {
                var a = b & jwwLinePattern;
                if (dc > 0)
                {
                    if (a != 0)
                    { //dot
                        dc++;
                    }
                    else
                    {
                        dot.Add(dc);
                        dc = 0;
                        sc = 1;
                    }
                }
                else if (sc > 0)
                {
                    if (a == 0)
                    { //spc
                        sc++;
                    }
                    else
                    {
                        spc.Add(sc);
                        sc = 0;
                        dc = 1;
                    }
                }
                else
                {
                    if (a != 0) dc = 1; else sc = 1;
                }
                b >>= 1;
            }
            if (dc > 0) dot.Add(dc);
            if (sc > 0) spc.Add(sc);
            if (dot.Count > spc.Count)
            {
                dot[0] += dot[^1];//.Last();
                dot.RemoveAt(dot.Count - 1);
            }
            else if (dot.Count < spc.Count)
            {
                spc[0] += spc[^1];//.Last();
                spc.RemoveAt(dot.Count - 1);
            }
            if (dot.Count != spc.Count)
            {
                Debug.WriteLine($"JwwReader MakeDotPattern spc and dot size not equal : {jwwLinePattern}");
            }
            if (dot.Count + spc.Count > 1)
            {
                var dotPattern = new float[dot.Count + spc.Count];
                for (var i = 0; i < dot.Count; i++)
                {
                    var c = dot[i];
                    dotPattern[i * 2] = c * pitch;
                }
                for (var i = 0; i < spc.Count; i++)
                {
                    var c = spc[i];
                    dotPattern[i * 2 + 1] = c * pitch;
                }
                return dotPattern;
            }
            return null;
        }

    }
}
