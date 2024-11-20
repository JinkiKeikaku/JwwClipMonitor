using CadMath2D;
using EmfHelper;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CadMath2D.CadPoint;
using static CadMath2D.CadMath;
using static System.Math;
using System.Drawing.Drawing2D;
using CadMath2D.Path;
using CadMath2D.Curves;
using System.Drawing;
using System.Reflection.Metadata;
using CadMath2D.Parameters;
using JwwClipMonitor.Properties;

namespace JwwClipMonitor.Jww.Shape
{
    internal class SolidCircle : IShape
    {
        public enum SolidType
        {
            Circle, Chord, Pie, Outer, DonutSameRatio, DonutSameWidth, Circumference, CircumferenceArc
        }
        public CadPoint P0 = new();
        public double Radius;
        public double InnerRadius;
        public double Flatness;
        public double Angle;
        public double StartAngle;
        public double SweepAngle;
        public Color Color;
        public SolidType CircleType = SolidType.Circle;



        public SolidCircle(StyleConverter sc, JwwSolid s)
        {
            P0.Set(s.m_start_x, s.m_start_y);
            Radius = s.m_end_x;
            Flatness = s.m_end_y;
            Angle = RadToDeg(s.m_DPoint2_x);
            Color = s.m_nPenColor == 10 ? s.m_Color.ToColor() : sc.PenToColor(s.m_nPenColor);
            StartAngle = RadToDeg(s.m_DPoint2_y);
            SweepAngle = RadToDeg(s.m_DPoint3_x);
            InnerRadius = s.m_DPoint3_y;
            if (s.m_nPenStyle == 101)
            {
                CircleType = InnerRadius switch
                {
                    -1 => SolidType.Outer,
                    0 => SolidType.Pie,
                    5 => SolidType.Chord,
                    _ => SolidType.Circle,
                };
            }
            else if (s.m_nPenStyle == 105)
            {
                CircleType = SolidType.DonutSameRatio;

            }
            else if (s.m_nPenStyle == 106)
            {
                CircleType = SolidType.DonutSameWidth;
            }
            else if (s.m_nPenStyle == 111)
            {
                CircleType = InnerRadius switch
                {
                    0 => SolidType.CircumferenceArc,
                    _ => SolidType.Circumference,
                };
            }
        }

        public SolidCircle(
            CadPoint p0, double radius, double innerRadius, double flatness, double angle, 
            double startAngle, double sweepAngle, Color color, SolidType circleType)
        {
            P0.Set(p0);
            Radius = radius;
            InnerRadius = innerRadius;
            Flatness = flatness;
            Angle = angle;
            StartAngle = startAngle;
            SweepAngle = sweepAngle;
            Color = color;
            CircleType = circleType;
        }
        public IShape Clone()
        {
            return new SolidCircle(
                P0, Radius, InnerRadius, Flatness, Angle, 
                StartAngle, SweepAngle, Color, CircleType);
        }
            

        public CadRect GetExtent()
        {
            var a = DegToRad(Angle);
            var sa = DegToRad(StartAngle);
            var sw = DegToRad(SweepAngle);
            switch (CircleType)
            {
                case SolidType.Pie:
                {
                    var r = Helpers.ArcExtent(P0, Radius, Flatness, a, sa, sw);
                    r.Spread(P0);
                    return r;
                }
                case SolidType.DonutSameWidth:
                {
                    var r1 = Helpers.ArcExtent(P0, Radius, Flatness, a, sa, sw);

                    var r2 = Helpers.ArcExtent(P0, InnerRadius, Flatness, a, sa, sw);
                    r1.Spread(r2);
                    return r1;
                }
                case SolidType.DonutSameRatio:
                {
                    var r1 = Helpers.ArcExtent(P0, Radius, Flatness, a, sa, sw);
                    var innerFlatness = (Radius * Flatness - (Radius - InnerRadius)) / InnerRadius;
                    if (innerFlatness < 0.0) innerFlatness = 0.0;
                    var r2 = Helpers.ArcExtent(P0, InnerRadius, innerFlatness, a, sa, sw);
                    r1.Spread(r2);
                    return r1;
                }

                case SolidType.CircumferenceArc:
                case SolidType.Chord:
                    return Helpers.ArcExtent(P0, Radius, Flatness, a, sa, sw);
                case SolidType.Outer:
                {
                    var r = Helpers.ArcExtent(P0, Radius, Flatness, a, sa, sw);
                    var p = OuterPointNotRotateNotAddP0();
                    p.Rotate(a);
                    r.Spread(p + P0);
                    return r;
                }
                case SolidType.Circle:
                case SolidType.Circumference:
                default:
                    return Helpers.ArcExtent(P0, Radius, Flatness, a, 0, PI * 2.0);
            }
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
            var oldRadius = Radius;
            var pa = new OvalArcParameter(P0, Radius, Flatness, Angle, StartAngle, SweepAngle);
            P0.Set(pa.P0);
            Radius = pa.Radius;
            Flatness = pa.Flatness;
            Angle = pa.Angle;
            StartAngle = pa.StartAngle;
            SweepAngle = pa.SweepAngle;
            InnerRadius = InnerRadius * Radius / oldRadius;
        }

        private CadPoint OuterPointNotRotateNotAddP0()
        {
            var sa = DegToRad(StartAngle);
            var sw = DegToRad(SweepAngle);
            var ma = sw / 2.0;
            var c = Cos(ma);
            var ml = FloatEQ(c, 0.0) ? Radius : Radius / c;
            var p = CadPoint.Pole(ml, sa + ma);
            //p.Y *= Flatness;
            return p;
        }

        public void OnDraw(Graphics g, DrawContext d)
        {
            var saved = g.Save();
            var p0 = d.DocToCanvas(P0);
            var r = d.DocToCanvas(new CadRect(-Radius, -Radius, Radius, Radius));
            var sa = d.DocToCanvasAngle(StartAngle);
            var sw = d.DocToCanvasAngle(SweepAngle);
            using var b = new SolidBrush(Color);
            g.TranslateTransform(p0.X, p0.Y);
            if (!FloatEQ(Angle, 0.0)) g.RotateTransform(d.DocToCanvasAngle(Angle));
            switch (CircleType)
            {

                case SolidType.DonutSameWidth:
                case SolidType.DonutSameRatio:
                {
                    var innerStartAngle = NormalizeAngle360(StartAngle + SweepAngle);
                    var innerSweepAngle = -SweepAngle;
                    var p1 = Pole(Radius, DegToRad(StartAngle));
                    var p2 = Pole(Radius, DegToRad(StartAngle + SweepAngle));
                    p1.Y *= Flatness;
                    p2.Y *= Flatness;
                    var innerFlatness = Flatness;
                    if (CircleType == SolidType.DonutSameWidth)
                    {
                        innerFlatness = (Radius * Flatness - (Radius - InnerRadius)) / InnerRadius;
                        if (innerFlatness < 0.0) innerFlatness = 0.0;
                    }
                    var p3 = Pole(InnerRadius, DegToRad(innerStartAngle));
                    var p4 = Pole(InnerRadius, DegToRad(innerStartAngle + innerSweepAngle));
                    p3.Y *= innerFlatness;
                    p4.Y *= innerFlatness;
                    var pt = PathMaker.CreatePathList(pm =>
                    {
                        pm.BeginPath(p1);
                        pm.AddArc(Radius, p2, CurveConverter.GetArcDirection(SweepAngle), Flatness);
                        pm.AddLine(p3);
                        if(innerFlatness <= 0.0)
                        {
                            pm.AddLine(p4);
                        }
                        else
                        {
                            pm.AddArc(InnerRadius, p4, CurveConverter.GetArcDirection(innerSweepAngle), innerFlatness);
                        }
                        pm.AddLine(p1);
                    });
                    var pts = PathConverter.PathToSinglePolygon(pt, 15);
                    var pa = new System.Drawing.PointF[pts.Count];
                    for (int i = 0; i < pts.Count; i++) pa[i] = d.DocToCanvas(pts[i]);
                    var path = new GraphicsPath();
                    path.AddPolygon(pa);
                    g.FillPath(b, path);
                }
                break;

                case SolidType.Outer:
                {
                    var p3 = OuterPointNotRotateNotAddP0();
                    var p1 = CadPoint.Pole(Radius, DegToRad(StartAngle));
                    var p2 = CadPoint.Pole(Radius, DegToRad(StartAngle + SweepAngle));
                    var path = new GraphicsPath();
                    path.AddArc(r, sa, sw);
                    path.AddLine(d.DocToCanvas(p1), d.DocToCanvas(p3));
                    path.AddLine(d.DocToCanvas(p3), d.DocToCanvas(p2));
                    g.ScaleTransform(1.0f, (float)Flatness);
                    g.FillPath(b, path);
                }
                break;
                case SolidType.Chord:
                {
                    var p1 = CadPoint.Pole(Radius, DegToRad(StartAngle));
                    var p2 = CadPoint.Pole(Radius, DegToRad(StartAngle + SweepAngle));
                    var path = new GraphicsPath();
                    path.AddArc(r, sa, sw);
                    path.AddLine(d.DocToCanvas(p1), d.DocToCanvas(p2));
                    g.ScaleTransform(1.0f, (float)Flatness);
                    g.FillPath(b, path);
                }
                break;

                case SolidType.Pie:
                    g.ScaleTransform(1.0f, (float)Flatness);
                    g.FillPie(b, r.X, r.Y, r.Width, r.Height, sa, sw);
                    break;

                case SolidType.Circle:
                    g.ScaleTransform(1.0f, (float)Flatness);
                    g.FillEllipse(b, r);
                    break;
                case SolidType.Circumference:
                    d.Pen.Width = Settings.Default.LineWidth;
                    d.Pen.DashStyle = DashStyle.Solid;
                    d.Pen.Color = Color;
                    g.ScaleTransform(1.0f, (float)Flatness);
                    g.DrawEllipse(d.Pen, r.X, r.Y, r.Width, r.Height);
                    break;
                case SolidType.CircumferenceArc:
                    d.Pen.Width = Settings.Default.LineWidth;
                    d.Pen.DashStyle = DashStyle.Solid;
                    d.Pen.Color = Color;
                    g.ScaleTransform(1.0f, (float)Flatness);
                    g.DrawArc(d.Pen, r.X, r.Y, r.Width, r.Height, sa, sw);
                    break;

            }
            g.Restore(saved);

        }
    }
}
