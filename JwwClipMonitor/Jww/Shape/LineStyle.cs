using CadMath2D;
using JwwClipMonitor.Emf;
using JwwClipMonitor.Properties;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CadMath2D.CadMath;

namespace JwwClipMonitor.Jww.Shape
{
    internal class LineStyle
    {
//        public static float DefaultLineWidth = 0.25f;//0.125f;
        public Color Color;
        public float Width = Settings.Default.LineWidth;
        public float[]? Pattern;
        public bool IsHojo;

        public static Color ConvertDrawColor(Color color)
        {
            return Settings.Default.DrawLineBlack?Color.Black:color;
        }
        public static bool IsHojoLine(JwwData jj)
        {
            return jj.m_nPenColor == 9 || jj.m_nPenStyle == 9;
        }
        public static bool IsHojoDotText(JwwData jj)
        {
            return jj.m_nPenColor == 9;
        }

        public void Set(LineStyle src)
        {
            Color = src.Color;
            Width = src.Width;
            Pattern = src.Pattern;
            IsHojo = src.IsHojo;
        }
        public void Set(StyleConverter sc, JwwData s)
        {
            Color = sc.PenToColor(s.m_nPenColor);
            Pattern = sc.PenStyleToPattern(s.m_nPenStyle);
            IsHojo = IsHojoLine(s);
            Width = Settings.Default.LineWidth;
        }

        public void DrawLine(Graphics g, DrawContext d, PointF p0, PointF p1)
        {
            if (Settings.Default.HiedHojo && IsHojo) return;
            d.Pen.SetLineCap(LineCap.Flat, LineCap.Flat, DashCap.Flat);
//            d.Pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Flat);
            d.Pen.LineJoin = LineJoin.Round;
            d.Pen.Color = ConvertDrawColor(Color);
            d.Pen.Width = Width;
            d.Pen.DashStyle = DashStyle.Solid;
            if (Pattern == null || Pattern.Length < 2)
            {
                g.DrawLine(d.Pen, p0, p1);
            }
            else
            {
                DrawDashLineForEmf(g, d, Width, p0, p1);
            }
        }
        public void DrawLines(Graphics g, DrawContext d, PointF[] points, bool isClosed)
        {
            if (Settings.Default.HiedHojo && IsHojo) return;
            d.Pen.SetLineCap(LineCap.Flat, LineCap.Flat, DashCap.Flat);
            d.Pen.LineJoin = LineJoin.Round;
            d.Pen.Color = ConvertDrawColor(Color);
            d.Pen.Width = Width;
            d.Pen.DashStyle = DashStyle.Solid;
            if (Pattern == null || Pattern.Length < 2)
            {
                if (isClosed)
                {
                    g.DrawPolygon(d.Pen, points);
                }
                else
                {
                    g.DrawLines(d.Pen, points);
                }
            }
            else
            {
                DrawLinesForEmf(g, d, Width, points, isClosed);
            }
        }
        private void DrawDashLineForEmf(Graphics g, DrawContext d, float width, PointF p0, PointF p1)
        {
            var pattern = Pattern;
            if (pattern == null) return;
            var dashedLineScale = 1.0f;
            var patternLength = pattern.Sum() * width * dashedLineScale;
            var dx = p1.X - p0.X;
            var dy = p1.Y - p0.Y;
            var lineLength = MathF.Sqrt(dx * dx + dy * dy);
            if (CadMath.FloatEQ(lineLength, 0)) return;
            var m = (int)(lineLength / patternLength);
            var a = lineLength * dashedLineScale / (m * patternLength + pattern[0] * width * dashedLineScale);
            var pat = new float[pattern.Length];
            var b = a * width;
            for (var i = 0; i < pattern.Length; i++)
            {
                //mPattern[i] + 0.0f意外を加えるとパターン長が変わるので注意
                pat[i] = (i & 1) == 0 ? (pattern[i] + 0.0f) * b : (pattern[i] + 0.0f) * b;
            }
            var t = 0.0f;
            var k = 0;
            GraphicsPath path = new GraphicsPath();
            var ret = DrawLineSub(path, t, k, pat, p0, p1, lineLength, false);
            if ((ret.k & 1) == 0)
            {
                path.StartFigure();
                path.AddLine(ret.pp0, p1);
            }
            d.Pen.Width = width;
            d.Pen.DashStyle = DashStyle.Solid;
            g.DrawPath(d.Pen, path);
        }

        private void DrawLinesForEmf(Graphics g, DrawContext d, float width, PointF[] points, bool isClosed)
        {
            var pattern = Pattern;
            if (pattern == null) return;
            var totalLineLength = GetTotalLineLength(points, isClosed);
            if (FloatEQ(totalLineLength, 0)) return;
            var dashedLineScale = 1.0f;
            var patternLength = pattern.Sum() * width * dashedLineScale;

            var dd = isClosed ? pattern[^1] : 0.0f;
            if (dd > totalLineLength) dd = 0.0f;
            var m = (int)((totalLineLength - dd) / patternLength);
            var a = (totalLineLength - dd) * dashedLineScale / (m * patternLength + pattern[0] * width * dashedLineScale);
            var pat = new float[pattern.Length];
            for (var i = 0; i < pattern.Length; i++) pat[i] = pattern[i] * a * width;
            var t = 0.0f;
            var k = 0;
            var p0 = isClosed ? points[^1] : points[0];
            var j = isClosed ? 0 : 1;
            GraphicsPath path = new GraphicsPath();
            for (; j < points.Length; j++)
            {
                var p1 = points[j];
                var dx = p1.X - p0.X;
                var dy = p1.Y - p0.Y;
                var lineLength = MathF.Sqrt(dx * dx + dy * dy);
                if (!FloatEQ(lineLength, 0))
                {
                    var ret = DrawLineSub(path, t, k, pat, p0, p1, lineLength, t != 0.0f);
                    if ((ret.k & 1) == 0)
                    {
                        path.StartFigure();
                        path.AddLine(ret.pp0, p1);
                    }
                    t = ret.t - lineLength;
                    k = ret.k;
                    if (FloatEQ(t, 0.0))
                    {
                        t = 0.0f;
                        k++;
                    }
                }
                p0 = p1;
            }
            d.Pen.Width = width;
            d.Pen.DashStyle = DashStyle.Solid;
            g.DrawPath(d.Pen, path);
        }

        private (PointF pp0, float t, int k) DrawLineSub(GraphicsPath path, float t, int k, float[] pat, PointF p0, PointF p1, float lineLength, bool isConnected)
        {
            var e10 = new PointF((p1.X - p0.X) / lineLength, (p1.Y - p0.Y) / lineLength);
            var n = pat.Length;
            PointF pp0 = new(p0.X, p0.Y);
            PointF pp1 = new(p0.X, p0.Y);
            while (true)
            {
                var dr = pat[k % n];
                if (FloatGE(t + dr, lineLength)) break;
                t += dr;
                pp1.X = p0.X + e10.X * t;
                pp1.Y = p0.Y + e10.Y * t;
                if ((k & 1) == 0)
                {
                    if (!isConnected)
                    {
                        path.StartFigure();
                    }
                    path.AddLine(pp0, pp1);
                }
                isConnected = false;
                k++;
                pp0.X = pp1.X;
                pp0.Y = pp1.Y;
            }
            return (pp0, t, k);
        }
        float GetTotalLineLength(PointF[] points, bool isClosed)
        {
            var (p0, j) = (points[0], 1);
            var lineLength = 0.0f;
            for (; j < points.Length; j++)
            {
                PointF p1 = points[j];
                var dx = p1.X - p0.X;
                var dy = p1.Y - p0.Y;
                lineLength += MathF.Sqrt(dx * dx + dy * dy);
                p0 = p1;
            }
            if (isClosed)
            {
                PointF p1 = points[0];
                var dx = p1.X - p0.X;
                var dy = p1.Y - p0.Y;
                lineLength += MathF.Sqrt(dx * dx + dy * dy);
            }
            return lineLength;
        }
    }
}
