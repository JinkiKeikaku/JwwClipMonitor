using CadMath2D;
using CadMath2D.Parameters;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CadMath2D.CadMath;
using static System.Math;

namespace JwwClipMonitor.Jww.Shape
{
    internal class ArcShape : IShape
    {
        public CadPoint P0 = new();
        public double Radius;
        public double Flatness;
        public double Angle;
        public double StartAngle;
        public double SweepAngle;
        public LineStyle LineStyle = new();


        public ArcShape(StyleConverter sc, JwwEnko s)
        {
            P0.Set(s.m_start_x, s.m_start_y);
            Radius = s.m_dHankei;
            Flatness = s.m_dHenpeiRitsu;
            Angle = RadToDeg(s.m_radKatamukiKaku);
            StartAngle = RadToDeg(s.m_radKaishiKaku);
            SweepAngle = RadToDeg(s.m_radEnkoKaku);
            LineStyle.Set(sc, s);
        }

        public ArcShape(
            CadPoint p0, double radius, double flatness, double angle, 
            double startAngle, double sweepAngle, LineStyle lineStyle)
        {
            P0.Set(p0);
            Radius = radius;
            Flatness = flatness;
            Angle = angle;
            StartAngle = startAngle;
            SweepAngle = sweepAngle;
            LineStyle.Set(lineStyle);
        }

        public IShape Clone()
        {
            return new ArcShape(P0, Radius, Flatness, Angle, StartAngle, SweepAngle, LineStyle);
        }

        public CadRect GetExtent()
        {
            var r = Helpers.ArcExtent(P0, Radius, Flatness, DegToRad(Angle), DegToRad(StartAngle), DegToRad(StartAngle+SweepAngle));
            r.Inflate(LineStyle.Width, LineStyle.Width);
            return r;
        }
        public void OnDraw(Graphics g, DrawContext d)
        {
            var saved = g.Save();
            int n = 0;
            double dt = 0;
            double dr = double.MaxValue;
            double dsw = DegToRad(SweepAngle);
            while (dr > d.CirclePrecision && n < d.MaxDivideValueOfQuaterCircleForDisplay)
            {
                n++;
                dt = dsw / n;
                dr = Radius * (1 - Cos(dt * 0.5)) * d.Scale;
            }
            //            DebugLog.Log(this, $"n={n}");
            PointF[] points = new PointF[n + 1];
            var sa0 = DegToRad(StartAngle);
            for (int i = 0; i <= n; i++)
            {
                var t = sa0 + i * dt;
                points[i] = d.DocToCanvas(Cos(t) * Radius, Sin(t) * Radius * Flatness);
            }
            var p0 = d.DocToCanvas(P0);
            g.TranslateTransform(p0.X, p0.Y);
            if (!FloatEQ(Angle, 0.0)) g.RotateTransform(d.DocToCanvasAngle(Angle));
            LineStyle.DrawLines(g, d, points, false);
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
            var pa = new OvalArcParameter(P0, Radius, Flatness, Angle, StartAngle, SweepAngle);
            P0.Set(pa.P0);
            Radius = pa.Radius;
            Flatness = pa.Flatness;
            Angle = pa.Angle;
            StartAngle = pa.StartAngle;
            SweepAngle = pa.SweepAngle;
        }
    }
}
