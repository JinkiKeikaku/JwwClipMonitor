using CadMath2D;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CadMath2D.CadMath;
using CadMath2D.Parameters;
namespace JwwClipMonitor.Jww.Shape
{
    internal class TextShape:IShape
    {
        static Bitmap tmpBmp = new Bitmap(4, 4);
        public CadPoint P0 = new();
        public string Text;
        public double Angle;
        public TextStyle TextStyle = new();

        public TextShape(StyleConverter sc, JwwMoji s)
        {
            Text = s.m_string;
            P0.Set(s.m_start_x, s.m_start_y);
            Angle = s.m_degKakudo;
            TextStyle.Set(sc, s);
        }

        public TextShape(CadPoint p0, string text, double angle, TextStyle textStyle)
        {
            P0.Set(p0);
            Text = text;
            Angle = angle;
            TextStyle.Set(textStyle);
        }

        public IShape Clone()=> new TextShape(P0, Text, Angle, TextStyle);

        public CadRect GetNotRotatedExtent()
        {
            using var g = Graphics.FromImage(tmpBmp);
            using var font = TextStyle.CreateFont();
            var ss = g.MeasureString(Text, font);
            var p1 = new CadPoint(P0.X + ss.Width, P0.Y + ss.Height);
            return new CadRect(P0, p1);
        }
        public CadRect GetExtent()
        {
            var r = GetNotRotatedExtent();
            r.SpreadRectByRotate(P0, DegToRad(Angle));
            return r;
        }
        public void OnDraw(Graphics g, DrawContext d)
        {
            var saved = g.Save();
            var p0 = d.DocToCanvas(P0);
            g.TranslateTransform(p0.X, p0.Y);
            g.RotateTransform(d.DocToCanvasAngle(Angle));
            TextStyle.Draw(g, d, Text);
            g.Restore(saved);
        }
        public void Offset(double dx, double dy)
        {
            P0.Offset(dx, dy);
        }
        public void Rotate(CadPoint p0, double angleRad)
        {
            P0.Rotate(p0, angleRad);
            Angle = NormalizeAngle360(Angle + RadToDeg(angleRad));
        }
        public void Scale(CadPoint p0, double mx, double my)
        {
            P0.Magnify(p0, mx, my);
            var r = GetNotRotatedExtent();
            if (r.Height() == 0 || r.Width() == 0) return;
            var rp = CadPointHelper.MagnifyRotatedRectangle(r.Width(), r.Height(), Angle, mx, my);
            var sx = rp.Width / r.Width();
            var sy = rp.Height / r.Height();
            TextStyle.Width *= (float)sx;
            TextStyle.Space *= (float)sx;
            TextStyle.Height *= (float)sy;
            Angle = rp.AngleDeg;
        }
    }
}
