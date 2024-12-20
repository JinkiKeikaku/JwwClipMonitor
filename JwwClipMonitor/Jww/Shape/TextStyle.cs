﻿using JwwClipMonitor.Properties;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww.Shape
{
    internal class TextStyle
    {
        public string FontName = "Arial";
        public float Height;
        public float Width;
        public float Space;
        public Color Color;
        public bool IsHojo;

        public void Set(TextStyle src)
        {
            FontName = src.FontName;
            Height = src.Height;
            Width = src.Width;
            Space = src.Space; ;
            Color = src.Color;
            IsHojo = src.IsHojo;
        }
        public void Set(StyleConverter sc, JwwMoji s)
        {
            Color = sc.PenToColor(s.m_nPenColor);
            FontName = s.m_strFontName;
            Height = (float)s.m_dSizeY;
            Width = (float)s.m_dSizeX;
            Space = (float)s.m_dKankaku;
            IsHojo=LineStyle.IsHojoDotText(s);
        }

        public void Draw(Graphics g, DrawContext d, string text)
        {
            if (Settings.Default.HiedHojo && IsHojo) return;
            using var brush = new SolidBrush(LineStyle.ConvertDrawColor(Color));
            using var font = CreateFont();
            g.DrawString(text, font, brush, new PointF(0, -Height));

        }

        public Font CreateFont()
        {
            return new Font(FontName, Height, GraphicsUnit.Pixel);
        }

    }
}
