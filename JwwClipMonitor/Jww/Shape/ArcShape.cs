using CadMath2D;
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
        public CadRect GetExtent()
        {
            return Helpers.ArcExtent(P0, Radius, Flatness, DegToRad(Angle), DegToRad(StartAngle), DegToRad(SweepAngle));
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
        }

    }
}
