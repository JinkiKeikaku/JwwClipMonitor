using CadMath2D;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CadMath2D;
using static CadMath2D.CadMath;
namespace JwwClipMonitor.Jww.Shape
{
    internal class TextShape:IShape
    {
        static Bitmap tmpBmp = new Bitmap(4, 4);
        public TextStyle TextStyle = new();
        string Text;
        CadPoint P0 =new();
        double Angle; 
        public TextShape(StyleConverter sc, JwwMoji s)
        {
            Text = s.m_string;
            P0.Set(s.m_start_x, s.m_start_y);
            Angle = s.m_degKakudo;
            TextStyle.Set(sc, s);
        }

        public CadRect GetExtent()
        {
            using var g = Graphics.FromImage(tmpBmp);
            using var font = TextStyle.CreateFont();
            var ss = g.MeasureString(Text, font);
            var p1 = new CadPoint(P0.X + ss.Width, P0.Y+ss.Height);
            var r = new CadRect(P0, p1);
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


    }
}
