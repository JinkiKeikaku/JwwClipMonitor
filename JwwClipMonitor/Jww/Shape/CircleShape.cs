using CadMath2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using static CadMath2D.CadMath;
using JwwHelper;
using CadMath2D.Parameters;
using System.Reflection.Metadata;

namespace JwwClipMonitor.Jww.Shape
{
    internal class CircleShape : IShape
    {
        public CadPoint P0 = new();
        public double Radius;
        public double Flatness;
        public double Angle;
        public LineStyle LineStyle = new();


        public CircleShape(StyleConverter sc, JwwEnko s)
        {
            P0.Set(s.m_start_x, s.m_start_y);
            Radius = s.m_dHankei;
            Flatness = s.m_dHenpeiRitsu;
            Angle = RadToDeg(s.m_radKatamukiKaku);
            LineStyle.Set(sc, s);
        }

        public CircleShape(CadPoint p0, double radius, double flatness, double angle, LineStyle lineStyle)
        {
            P0.Set(p0);
            Radius = radius;
            Flatness = flatness;
            Angle = angle;
            LineStyle.Set(lineStyle);
        }

        public IShape Clone()=> new CircleShape(P0, Radius, Flatness, Angle, LineStyle);

        public CadRect GetExtent()
        {
            var r = Helpers.ArcExtent(P0, Radius, Flatness, DegToRad(Angle), 0, PI * 2.0);
            r.Inflate(LineStyle.Width, LineStyle.Width);
            return r;
        }

        public void OnDraw(Graphics g, DrawContext d)
        {
            var saved = g.Save();
            int n = 1;
            double dt = 0;
            double dr = double.MaxValue;
            while (dr > d.CirclePrecision && n < d.MaxDivideValueOfQuaterCircleForDisplay)
            {
                n++;
                dt = 0.5 * MathF.PI / n;
                dr = Radius * (1 - Cos(dt * 0.5)) * d.Scale;
            }
            PointF[] points = new PointF[n * 4];
            for (int i = 1; i < n; i++)
            {
                var t = i * dt;
                var pt = d.DocToCanvas(Cos(t) * Radius, Sin(t) * Radius * Flatness);
                points[i] = pt;
                var i1 = n * 2 - i;
                var i2 = n * 2 + i;
                var i3 = n * 4 - i;
                points[i1].X = -pt.X;
                points[i1].Y = pt.Y;
                points[i2].X = -pt.X;
                points[i2].Y = -pt.Y;
                points[i3].X = pt.X;
                points[i3].Y = -pt.Y;
            }
            points[0] = d.DocToCanvas(Radius, 0);
            points[n * 2] = d.DocToCanvas(-Radius, 0);
            points[n] = d.DocToCanvas(0, Radius * Flatness);
            points[n * 3] = d.DocToCanvas(0, -Radius * Flatness);
            var p0 = d.DocToCanvas(P0);
            g.TranslateTransform(p0.X, p0.Y);
            if (!FloatEQ(Angle, 0.0)) g.RotateTransform(d.DocToCanvasAngle(Angle));
            LineStyle.DrawLines(g, d, points, true);
            g.Restore(saved);
            //var rr = d.DocToCanvas(GetExtent());
            //g.DrawRectangle(d.Pen, rr.X, rr.Y, rr.Width, rr.Height);
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
            var pa = new OvalParameter(P0, Radius, Flatness, Angle);
            P0.Set(pa.P0);
            Radius = pa.Radius;
            Flatness = pa.Flatness;
            Angle = pa.Angle;
        }
    }
}
