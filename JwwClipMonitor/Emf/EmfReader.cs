using CadMath2D;
using CadMath2D.Parameters;
using CadMath2D.Path;
using EmfHelper;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using static CadMath2D.CadMath;
using static CadMath2D.Curves.CurveConverter;
using System.Diagnostics;
using CadMath2D.Curves;
using JwwClipMonitor.Utility;

namespace JwwClipMonitor.Emf
{
    internal class EmfReader : IEmfReadListener
    {
        private List<EmfDeviceContext> mDcStack = new() { new EmfDeviceContext() };
        private EmfDeviceContext CurrentDC => mDcStack.Last();
        private PathMaker? mPathMaker = null;
        private EmfObjectTable mEmfObjectTable = new();
        public double Dpm = 600.0 / 25.4;
        public List<JwwData> Shapes = new();

        public EmfHeader Header = new();

        public void Read(MemoryStream ms)
        {
            Dpm = 600.0 / 25.4;
            mDcStack.Clear();
            mDcStack.Add(new EmfDeviceContext());
            mPathMaker = null;
            Shapes.Clear();
            mEmfObjectTable.Clear();

            var r = new EmfParser(this, null);
            using var mr = new BinaryReader(ms);
            r.Read(mr);
            if (Shapes.Count == 0) return;



        }


        public void OnEmfHeader(EmfRecord record, EmfHeader header)
        {
            if (header.Signature != 0x464D4520) throw new Exception("This file is not EMF file.");
            if (header.Millimeters.Width == 0 || header.Device.Width == 0) throw new Exception("Width is zero.");
            Dpm = (double)header.Device.Width / (double)header.Millimeters.Width;
            Header = header;
            Debug.WriteLine(header.ToString(), "Header");
        }

        public void OnEmfEof(EmfRecord record)
        {
        }

        public bool OnEmfComment(EmfRecord record, EmrComment comment) => true;

        public void OnEmfSetBkColor(EmfRecord record, ColorRef bkColor)
        {
            CurrentDC.BKColor = bkColor.ToColor();
        }

        public void OnEmfSetBkMode(EmfRecord record, BackgroundMode bkMode)
        {
            CurrentDC.BKMode = bkMode;
        }

        public void OnEmfSetPolyFillMode(EmfRecord record, PolygonFillMode polyFillMode)
        {
            SkipRecord(record);
        }

        public void OnEmfSetTextAlign(EmfRecord record, TextAlignment textAlign)
        {
            CurrentDC.TextAlign = textAlign;
        }

        public void OnEmfSetTextColor(EmfRecord record, ColorRef textColor)
        {
            CurrentDC.TextColor = textColor.ToColor();
        }

        public void OnEmfSetArcDirection(EmfRecord record, EmfHelper.ArcDirection direction)
        {
            CurrentDC.ArcDirection = direction;
        }

        public void OnEmfSaveDC(EmfRecord record)
        {
            mDcStack.Add(CurrentDC.Copy());
        }

        public void OnEmfRestoreDC(EmfRecord record, int savedDC)
        {
            if (savedDC >= 0) throw new Exception($"OnRestoreDC(): saved dc ({savedDC}) greater than 0.");
            for (var i = 0; i < -savedDC; i++) mDcStack.RemoveAt(mDcStack.Count - 1);
        }

        public void OnEmfSetWindowOrg(EmfRecord record, PointL org)
        {
            MapModeTranslate(() =>
            {
                CurrentDC.WindowOrgX = org.X;
                CurrentDC.WindowOrgY = org.Y;
            });
        }

        public void OnEmfSetViewportOrg(EmfRecord record, PointL org)
        {
            MapModeTranslate(() =>
            {
                CurrentDC.ViewportOrgX = org.X;
                CurrentDC.ViewportOrgY = org.Y;
            });
        }

        public void OnEmfSetWindowExtEx(EmfRecord record, SizeL s)
        {
            MapModeTranslate(() =>
            {
                if (CurrentDC.MapMode == MapMode.MM_ISOTROPIC || CurrentDC.MapMode == MapMode.MM_ANISOTROPIC)
                {
                    CurrentDC.Wxs = s.Width;
                    CurrentDC.Wys = s.Height;
                }
            });
        }

        public void OnEmfSetViewportExtEx(EmfRecord record, SizeL s)
        {
            MapModeTranslate(() =>
            {
                if (CurrentDC.MapMode == MapMode.MM_ISOTROPIC || CurrentDC.MapMode == MapMode.MM_ANISOTROPIC)
                {
                    CurrentDC.Vxs = s.Width;
                    CurrentDC.Vys = s.Height;
                }
            });
        }

        public void OnEmfSetMapMode(EmfRecord record, MapMode mode)
        {
            Debug.WriteLine($"OnEmfSetMapMode(): {mode}", GetType().Name);
            MapModeTranslate(() =>
            {
                //私のパソコンはこの値。
                switch (mode)
                {
                    case MapMode.MM_TEXT:
                        CurrentDC.Wxs = 1;
                        CurrentDC.Wys = 1;
                        CurrentDC.Vxs = 1;
                        CurrentDC.Vys = 1;
                        break;
                    case MapMode.MM_ANISOTROPIC:
                    case MapMode.MM_ISOTROPIC:
                        if (CurrentDC.MapMode != MapMode.MM_ANISOTROPIC && CurrentDC.MapMode != MapMode.MM_ANISOTROPIC)
                        {
                            CurrentDC.Wxs = 5090;
                            CurrentDC.Wys = 2860;
                            CurrentDC.Vxs = 1920;
                            CurrentDC.Vys = -1080;
                        }
                        break;
                    case MapMode.MM_LOMETRIC:
                        CurrentDC.Wxs = 5090;
                        CurrentDC.Wys = 2860;
                        CurrentDC.Vxs = 1920;
                        CurrentDC.Vys = -1080;
                        break;
                    case MapMode.MM_HIMETRIC:
                        CurrentDC.Wxs = 50900;
                        CurrentDC.Wys = 28600;
                        CurrentDC.Vxs = 1920;
                        CurrentDC.Vys = -1080;
                        break;
                    case MapMode.MM_LOENGLISH:
                        CurrentDC.Wxs = 2004;
                        CurrentDC.Wys = 1126;
                        CurrentDC.Vxs = 1920;
                        CurrentDC.Vys = -1080;
                        break;
                    case MapMode.MM_HIENGLISH:
                        CurrentDC.Wxs = 20039;
                        CurrentDC.Wys = 11260;
                        CurrentDC.Vxs = 1920;
                        CurrentDC.Vys = -1080;
                        break;
                    case MapMode.MM_TWIPS:
                        CurrentDC.Wxs = 28857;
                        CurrentDC.Wys = 16214;
                        CurrentDC.Vxs = 1920;
                        CurrentDC.Vys = -1080;
                        break;
                }
                CurrentDC.MapMode = mode;

            });
        }

        public void OnEmfSetWorldTransform(EmfRecord record, XForm xform)
        {
            CurrentDC.XFormM = new(xform.M11, xform.M12, xform.M21, xform.M22);
            CurrentDC.XFormD = new CadPoint(xform.Dx, xform.Dy);
        }

        public void OnEmfModifyWorldTransform(EmfRecord record, XForm xform, ModifyWorldTransformMode mode)
        {
            switch (mode)
            {
                case ModifyWorldTransformMode.MWT_IDENTITY:
                    CurrentDC.XFormM = new();
                    CurrentDC.XFormD = new();
                    break;
                case ModifyWorldTransformMode.MWT_SET:
                    CurrentDC.XFormM = new(xform.M11, xform.M12, xform.M21, xform.M22);
                    CurrentDC.XFormD = new CadPoint(xform.Dx, xform.Dy);
                    break;
                case ModifyWorldTransformMode.MWT_LEFTMULTIPLY:
                {
                    var ml = new Matrix2D(xform.M11, xform.M12, xform.M21, xform.M22);
                    var m = ml * CurrentDC.XFormM;
                    var pl = new CadPoint(xform.Dx, xform.Dy);
                    var p0 = pl + ml * CurrentDC.XFormD;
                    CurrentDC.XFormM = m;
                    CurrentDC.XFormD = p0;
                }
                break;
                case ModifyWorldTransformMode.MWT_RIGHTMULTIPLY:
                {
                    var mr = new Matrix2D(xform.M11, xform.M12, xform.M21, xform.M22);
                    var m = CurrentDC.XFormM * mr;
                    var pr = new CadPoint(xform.Dx, xform.Dy);
                    var p0 = CurrentDC.XFormD + CurrentDC.XFormM * pr;
                    CurrentDC.XFormM = m;
                    CurrentDC.XFormD = p0;
                }
                break;
            }
        }

        public void OnEmfSetRop2(EmfRecord record, BinaryRasterOperation op)
        {
            SkipRecord(record);
        }

        public void OnEmfSetMiterLimit(EmfRecord record, uint miterLimit)
        {
            SkipRecord(record);
        }

        public void OnEmfSetStretchBltMode(EmfRecord record, StretchMode mode)
        {
            SkipRecord(record);
        }

        public void OnEmfSetIcmMode(EmfRecord record, ICMMode mode)
        {
            SkipRecord(record);
        }

        public void OnEmfSelectClipPath(EmfRecord record, RegionMode mode)
        {
            SkipRecord(record);
        }

        public void OnEmfMoveToEx(EmfRecord record, PointL p)
        {
            var p0 = ToPaper(p);
            if (mPathMaker != null)
            {
                mPathMaker.BeginPath(p0);
            }
            else
            {
                CurrentDC.LastPoint = p0;
            }
        }

        public void OnEmfLineTo(EmfRecord record, PointL p)
        {
            var p0 = ToPaper(p);
            if (mPathMaker != null)
            {
                mPathMaker.AddLine(p0);
            }
            else
            {
                AddLine(CurrentDC.LastPoint, p0);
                CurrentDC.LastPoint = p0;
            }
        }

        public void OnEmfPolyPolyline(EmfRecord record, EmrPolyPolyline ppl)
        {
            OnPolyPolyline(ppl, false);
        }

        public void OnEmfPolyPolygon(EmfRecord record, EmrPolyPolyline ppl)
        {
            OnPolyPolyline(ppl, true);
        }

        public void OnEmfPolygon(EmfRecord record, EmrPolygon ppl)
        {
            OnPolyline(ToPaper(ppl.Points), true, false);
        }

        public void OnEmfPolyline(EmfRecord record, EmrPolygon ppl)
        {
            OnPolyline(ToPaper(ppl.Points), false, false);
        }

        public void OnEmfPolylineTo(EmfRecord record, EmrPolygon ppl)
        {
            OnPolyline(ToPaper(ppl.Points), false, true);
        }

        public void OnEmfPolyBezier(EmfRecord record, EmrPolygon ppl)
        {
            SkipRecord(record);
        }

        public void OnEmfPolyBezierTo(EmfRecord record, EmrPolygon ppl)
        {
            var pts = ToPaper(ppl.Points);
            if (OnPolyBezier(pts, true))
            {
                CurrentDC.LastPoint = pts[^1].Copy();
            }
        }

        public void OnEmfRectangle(EmfRecord record, RectL box)
        {
            var r = ToPaper(box);
            if (mPathMaker != null)
            {
                mPathMaker.AddPolyline(r.GetVertices(), true);
            }
            else
            {
                OnPolyline(r.GetVertices(), true, false);
            }
        }

        public void OnEmfRoundRect(EmfRecord record, RectL box, SizeL corner)
        {
            var r = ToPaper(box);
            var c = ToPaper(corner);
            if (mPathMaker != null)
            {
                mPathMaker.AddRoundRect(r, Abs(c.Width) / 2, Abs(c.Height) / 2);
            }
            else
            {
                var path = PathMaker.CreatePathList(pm =>
                {
                    pm.AddRoundRect(r, Abs(c.Width) / 2, Abs(c.Height) / 2);
                });
                AddPath(path, true, true);
            }
        }

        public void OnEmfEllipse(EmfRecord record, RectL box)
        {
            OnEllipse(box);
        }

        public void OnEmfArc(EmfRecord record, EmrArc ppl)
        {
            OnArc(ppl.Box, ppl.Start, ppl.End);
        }

        public void OnEmfArcTo(EmfRecord record, EmrArc ppl)
        {
            var op = OnArc(ppl.Box, ppl.Start, ppl.End);
            CurrentDC.LastPoint = op.GetEndPoint();
        }

        public void OnEmfAngleArc(EmfRecord record, PointL center, uint radius, float start, float sweep)
        {
            if (radius <= 0) return;
            var size = ToPaper(new SizeL((int)radius, (int)radius));
            var r = Abs(size.Width);
            var f = Abs(size.Height) / r;
            var op = new OvalArcParameter(ToPaper(center), r, f, 0.0, Sign(size.Height) * start, Sign(size.Height) * sweep);
            if (mPathMaker != null)
            {
                mPathMaker.AddLine(op.GetStartPoint());
                mPathMaker.AddArc(op.Radius, op.GetEndPoint(), GetArcDirection(op.SweepAngle), op.Flatness, 0.0);
            }
            else
            {
                AddLine(CurrentDC.LastPoint, op.GetStartPoint());
                op.Transform(GetMatrix());
                var arc = new JwwEnko();
                arc.m_start_x = op.P0.X;
                arc.m_start_y = op.P0.Y;
                arc.m_dHankei = op.Radius;
                arc.m_dHenpeiRitsu = op.Flatness;
                arc.m_radKaishiKaku = DegToRad(op.StartAngle);
                arc.m_radEnkoKaku = DegToRad(op.SweepAngle);
                SetLineStyle(arc);
                AddShape(arc);
            }
            CurrentDC.LastPoint = op.GetEndPoint();
        }

        public void OnEmfPie(EmfRecord record, EmrArc ppl)
        {
            OnPie(ppl.Box, ppl.Start, ppl.End);
        }

        public void OnEmfChord(EmfRecord record, EmrArc ppl)
        {
            OnChord(ppl.Box, ppl.Start, ppl.End);
        }

        public void OnEmfExtTextOutW(EmfRecord record, EmrExtTextOutW ppl)
        {
            if ((ppl.Text.Options & (int)ExtTextOutOptions.ETO_OPAQUE) != 0)
            {
                var bkBounds = ToPaper(ppl.Text.ClipBounds, false);
                if (!bkBounds.IsEmpty())
                {
                    AddFillPolygon(bkBounds.GetVertices(), CurrentDC.BKColor);
                }
            }
            AddText(ppl);
        }

        public void OnEmfBitBlt(EmfRecord record, EmrBitBlt ppl)
        {
            SkipRecord(record);
        }

        public void OnEmfStretchDibBits(EmfRecord record, EmrStretchDibits ppl)
        {
            SkipRecord(record);
        }

        public void OnEmfSelectObject(EmfRecord record, uint ihObject)
        {
            var style = mEmfObjectTable.GetStyleFromId(ihObject);
            switch (style)
            {
                case LineStyle s:
                    CurrentDC.LineStyle = s; break;
                case FillStyle s:
                    CurrentDC.FillStyle = s; break;
                case TextStyle s:
                    CurrentDC.TextStyle = s; break;
                default:
                    Debug.WriteLine($"OnEmfSelectObject(): id={ihObject}", GetType().Name);
                    break;
            }
        }

        public void OnEmfDeleteObject(EmfRecord record, uint ihObject)
        {
            mEmfObjectTable.DeleteObject(ihObject);
        }

        public void OnEmfCreatePen(EmfRecord record, EmrCreatePen ppl)
        {
            mEmfObjectTable.CreatePen(ppl);
        }

        public void OnEmfExtCreatePen(EmfRecord record, EmrExtCreatePen ppl)
        {
            mEmfObjectTable.CreatePen(ppl);
        }

        public void OnEmfCreateBrushIndirect(EmfRecord record, EmrCreateBrushIndirect ppl)
        {
            mEmfObjectTable.CreateBrush(ppl);
        }

        public void OnEmfExtCreateFontIndirect(EmfRecord record, EmrExtCreateFontIndirectW ppl)
        {
            mEmfObjectTable.CreateFont(ppl);
        }

        public void OnEmfBeginPath(EmfRecord record)
        {
            mPathMaker = new PathMaker();
            CurrentDC.PathList = null;
        }

        public void OnEmfEndPath(EmfRecord record)
        {
            if (mPathMaker != null)
            {
                CurrentDC.PathList = mPathMaker.GetPathList();
            }
            mPathMaker = null;
        }

        public void OnEmfCloseFigure(EmfRecord record)
        {
            mPathMaker?.EndPath(true);
        }

        public void OnEmfStrokePath(EmfRecord record, RectL bounds)
        {
            AddPath(CurrentDC.PathList, true, false);
        }

        public void OnEmfStrokeAndFillPath(EmfRecord record, RectL bounds)
        {
            AddPath(CurrentDC.PathList, true, true);
        }

        public void OnEmfFillPath(EmfRecord record, RectL bounds)
        {
            if (CurrentDC.PathList != null)
            {
                AddPath(CurrentDC.PathList, false, true);
                CurrentDC.PathList = null;
            }
            else
            {
                Debug.WriteLine($"OnEmfFillPath(): path is null", GetType().Name);
            }
        }

        private void OnEllipse(RectL box)
        {
            var rect = ToPaper(box);
            rect.Sort();
            if (rect.IsEmpty()) return;
            var p0 = rect.Center();
            var radius = rect.Width() * 0.5;
            var f = rect.Height() / rect.Width();
            if (mPathMaker != null)
            {
                mPathMaker.AddCircle(p0, radius, f, 0.0);
            }
            else
            {
                AddSolidArc(100, p0 , radius, f, 0.0, 0, 360);
                //var m = GetMatrix();
                //var s = new OvalParameter(p0, radius, f, 0.0);
                //s.Transform(m);
                //if (!CurrentDC.FillStyle.IsNull)
                //{
                //    var js = new JwwSolid();
                //    js.m_nPenStyle = 101;//円ソリッド
                //    js.m_DPoint3_y = 100;//全円
                //    js.m_start_x = s.P0.X;
                //    js.m_start_y = s.P0.Y;
                //    js.m_end_x = s.Radius;
                //    js.m_end_y = s.Flatness;
                //    js.m_DPoint2_x = DegToRad(s.Angle);
                //    var col = CurrentDC.FillStyle.Color;
                //    js.m_nPenColor = 10;
                //    js.m_Color = col.R + (col.G << 8) + (col.B << 16);
                //    AddShape(js);
                //}
                AddArc(p0, radius, f, 0, 0, 360, true);
                //if (!CurrentDC.LineStyle.IsNull)
                //{
                //    var js = new JwwEnko();
                //    js.m_start_x = s.P0.X;
                //    js.m_start_y = s.P0.Y;
                //    js.m_bZenEnFlg = 1;
                //    js.m_radKaishiKaku = 0;
                //    js.m_dHankei = s.Radius;
                //    js.m_dHenpeiRitsu = s.Flatness;
                //    js.m_radEnkoKaku = 2.0 * Math.PI;
                //    js.m_radKatamukiKaku = DegToRad(s.Angle);
                //    AddShape(js);
                //}
            }
        }

        private OvalArcParameter OnArc(RectL box, PointL start, PointL end)
        {
            var op = GetOvalArcParameter(box, start, end);
            if (mPathMaker != null)
            {
                mPathMaker.BeginPath(op.GetStartPoint());
                mPathMaker.AddArc(op.Radius, op.GetEndPoint(), GetArcDirection(op.SweepAngle), op.Flatness, 0.0);
            }
            else
            {
                AddArc(op.P0, op.Radius, op.Flatness, op.Angle, op.StartAngle, op.SweepAngle, false);
            }
            return op;
        }

        private void AddArc(CadPoint p0, double radius, double flatness, double angle, double start, double sweep, bool isCircle)
        {
            if (!CurrentDC.LineStyle.IsNull)
            {
                var m = GetMatrix();
                var s = new OvalArcParameter(p0, radius, flatness, angle, start, sweep);
                s.Transform(m);
                var js = new JwwEnko();
                js.m_start_x = s.P0.X;
                js.m_start_y = s.P0.Y;
                js.m_bZenEnFlg = isCircle ? 1 : 0;
                js.m_dHankei = s.Radius;
                js.m_dHenpeiRitsu = s.Flatness;
                js.m_radKaishiKaku = DegToRad(s.StartAngle);
                js.m_radEnkoKaku = DegToRad(s.SweepAngle);
                js.m_radKatamukiKaku = DegToRad(s.Angle);
                AddShape(js);
            }
        }

        private void AddSolidArc(int arcType, CadPoint p0, double radius, double flatness, double angle, double start, double sweep)
        {
            if (!CurrentDC.FillStyle.IsNull)
            {
                var s = new OvalArcParameter(p0, radius, flatness, angle, start, sweep);
                var m = GetMatrix();
                s.Transform(m);
                var js = new JwwSolid();
                js.m_nPenStyle = 101;//円ソリッド
                js.m_DPoint3_y = arcType;//扇形
                js.m_start_x = s.P0.X;
                js.m_start_y = s.P0.Y;
                js.m_end_x = s.Radius;
                js.m_end_y = s.Flatness;
                js.m_DPoint2_x = DegToRad(s.Angle);
                js.m_DPoint2_y = DegToRad(s.StartAngle);
                js.m_DPoint3_x = DegToRad(s.SweepAngle);
                var col = CurrentDC.FillStyle.Color;
                js.m_nPenColor = 10;
                js.m_Color = col.R + (col.G << 8) + (col.B << 16);
                AddShape(js);
            }
        }

        private void OnPie(RectL box, PointL start, PointL end)
        {
            var op = GetOvalArcParameter(box, start, end);
            if (mPathMaker != null)
            {
                mPathMaker.BeginPath(op.GetStartPoint());
                mPathMaker.AddArc(op.Radius, op.GetEndPoint(), GetArcDirection(op.SweepAngle), op.Flatness, 0.0);
                mPathMaker.AddLine(op.P0);
                mPathMaker.EndPath(true);
            }
            else
            {
                AddSolidArc(0, op.P0, op.Radius, op.Flatness, op.Angle, op.StartAngle, op.SweepAngle);
                //if (!CurrentDC.FillStyle.IsNull)
                //{
                //    var s = op.Copy();
                //    var m = GetMatrix();
                //    s.Transform(m);
                //    var js = new JwwSolid();
                //    js.m_nPenStyle = 101;//円ソリッド
                //    js.m_DPoint3_y = 0;//扇形
                //    js.m_start_x = s.P0.X;
                //    js.m_start_y = s.P0.Y;
                //    js.m_end_x = s.Radius;
                //    js.m_end_y = s.Flatness;
                //    js.m_DPoint2_x = DegToRad(s.Angle);
                //    js.m_DPoint2_y = DegToRad(s.StartAngle);
                //    js.m_DPoint3_x = DegToRad(s.SweepAngle);
                //    var col = CurrentDC.FillStyle.Color;
                //    js.m_nPenColor = 10;
                //    js.m_Color = col.R + (col.G << 8) + (col.B << 16);
                //    AddShape(js);
                //}
                AddLine(op.P0, op.GetStartPoint());
                AddArc(op.P0, op.Radius, op.Flatness, op.Angle, op.StartAngle, op.SweepAngle, false);
                AddLine(op.GetEndPoint(), op.P0);
                //var path = PathMaker.CreatePathList(pm =>
                //{
                //    pm.BeginPath(op.GetStartPoint());
                //    pm.AddArc(op.Radius, op.GetEndPoint(), GetArcDirection(op.SweepAngle), op.Flatness, 0.0);
                //    pm.AddLine(op.P0);
                //    pm.EndPath(true);
                //});
                //AddPath(path, true, true);
            }
        }

        private void OnChord(RectL box, PointL start, PointL end)
        {
            var op = GetOvalArcParameter(box, start, end);
            if (mPathMaker != null)
            {
                mPathMaker.BeginPath(op.GetStartPoint());
                mPathMaker.AddArc(op.Radius, op.GetEndPoint(), GetArcDirection(op.SweepAngle), op.Flatness, 0.0);
                mPathMaker.EndPath(true);
            }
            else
            {
                AddSolidArc(5, op.P0, op.Radius, op.Flatness, op.Angle, op.StartAngle, op.SweepAngle);
                AddLine(op.GetEndPoint(), op.GetStartPoint());
                //var path = PathMaker.CreatePathList(pm =>
                //{
                //    pm.BeginPath(op.GetStartPoint());
                //    pm.AddArc(op.Radius, op.GetEndPoint(), GetArcDirection(op.SweepAngle), op.Flatness, 0.0);
                //    pm.EndPath(true);
                //});
                //AddPath(path, true, true);
            }
        }

        private OvalArcParameter GetOvalArcParameter(RectL box, PointL start, PointL end)
        {
            var rect = ToPaper(box);
            rect.Sort();
            if (rect.IsEmpty()) return new();
            var p0 = rect.Center();
            var ps = ToPaper(start) - p0;
            var pe = ToPaper(end) - p0;
            var f = rect.Height() / rect.Width();
            ps.Y *= f;
            pe.Y *= f;
            var radius = rect.Width() * 0.5;
            var sa = ps.GetAngle360();
            var ea = pe.GetAngle360();
            var sw = GetArcSweepAngle(sa, ea);
            return new OvalArcParameter(p0, radius, f, 0.0, sa, sw);
        }

        private double GetArcSweepAngle(double start, double end)
        {
            var sw = SubtractAngleDeg(end, start);
            if (FloatEQ(sw, 0.0)) sw = 360.0;
            if (CurrentDC.ArcDirection == EmfHelper.ArcDirection.AD_CLOCKWISE) sw = -sw;
            var m = GetMapModeRatio(false);
            if (m.mx * m.my < 0.0) sw = -sw;
            return sw;
        }

        public void OnNotImplementRecord(EmfRecord record)
        {
            SkipRecord(record);
        }
        private CadSize ToPaper(SizeL size, bool ignoreMapMode = false)
        {
            var a = GetMapModeRatio(ignoreMapMode);
            return new CadSize(size.Width * a.mx / Dpm, size.Height * a.my / Dpm);
        }


        private double ToPaper(double x, bool ignoreMapMode = false)
        {
            //MM_ANISOTROPICの時は考えない。
            var a = GetMapModeRatio(ignoreMapMode);
            return x * a.mx / Dpm;
        }

        private CadRect ToPaper(RectL r, bool ignoreMapMode = false)
        {
            var p1 = ToPaper(r.Left, r.Bottom, ignoreMapMode);
            var p2 = ToPaper(r.Right, r.Top, ignoreMapMode);
            return new CadRect(p1, p2);
        }


        private CadPoint ToPaper(double x, double y, bool ignoreMapMode = false)
        {
            var a = GetMapModeRatio(ignoreMapMode);
            return new(
                (((double)x - CurrentDC.WindowOrgX) * a.mx + CurrentDC.ViewportOrgX) / Dpm,
                (-(((double)y - CurrentDC.WindowOrgY) * a.my + CurrentDC.ViewportOrgY)) / Dpm);
        }

        private CadPoint ToPaper(PointS p, bool ignoreMapMode = false) => ToPaper(p.X, p.Y, ignoreMapMode);
        private CadPoint ToPaper(PointL p, bool ignoreMapMode = false) => ToPaper(p.X, p.Y, ignoreMapMode);

        private List<CadPoint> ToPaper(IReadOnlyList<PointS> points, bool ignoreMapMode = false)
        {
            var ret = new List<CadPoint>();
            foreach (var p in points) ret.Add(ToPaper(p, ignoreMapMode));
            return ret;
        }

        private List<CadPoint> ToPaper(IReadOnlyList<PointL> points, bool ignoreMapMode = false)
        {
            var ret = new List<CadPoint>();
            foreach (var p in points) ret.Add(ToPaper(p, ignoreMapMode));
            return ret;
        }
        private (double mx, double my) GetMapModeRatio(bool ignoreMapMode)
        {
            if (ignoreMapMode) return (1.0, 1.0);
            switch (CurrentDC.MapMode)
            {
                //本当は最初にMM_ISOTROPIC、MM_ISOTROPICにした直後は、MM_LOMETRICと同じ値になるがそこは無視する。
                case MapMode.MM_ISOTROPIC:
                {
                    var mx = (double)CurrentDC.Vxs / CurrentDC.Wxs;
                    var my = (double)CurrentDC.Vys / CurrentDC.Wys;
                    return (mx, Sign(my) * Abs(mx));
                }
                default:
                {
                    var mx = (double)CurrentDC.Vxs / CurrentDC.Wxs;
                    var my = (double)CurrentDC.Vys / CurrentDC.Wys;
                    return (mx, my);
                }
            }
        }
        private void MapModeTranslate(Action action)
        {
            var p0 = ToPaper(new PointL());
            var scaleOld = GetMapModeRatio(false);
            action();
            var p1 = ToPaper(new PointL());
            CurrentDC.LastPoint += (p1 - p0);
            var scaleNew = GetMapModeRatio(false);
            CurrentDC.TextStyle.FontHeight *= (float)Abs(scaleNew.my / scaleOld.my);
        }

        private void AddShape(JwwData data)
        {
            Shapes.Add(data);
        }

        private void SetLineStyle(JwwData shape)
        {

        }

        private void SkipRecord(EmfRecord record)
        {
            Debug.WriteLine($"Skip {record}");
        }

        private TransformMatrix GetMatrix()
        {
            var p1 = ToPaper(0, 0);
            var dp = ToPaper(CurrentDC.XFormD.X, CurrentDC.XFormD.Y) - p1;
            var m1 = TransformMatrix.CreateOffsetMatrix(p1.X, p1.Y);
            var m2 = new TransformMatrix(CurrentDC.XFormM.A11, CurrentDC.XFormM.A12, CurrentDC.XFormM.A21, CurrentDC.XFormM.A22, dp.X, dp.Y);
            var m3 = TransformMatrix.CreateOffsetMatrix(-p1.X, -p1.Y);
            return m1 * m2 * m3;
        }

        private void AddLine(CadPoint p0, CadPoint p1)
        {
            if (CurrentDC.LineStyle.IsNull) return;
            var line = new JwwSen();
            var lp = new LineParameter(p0, p1);
            lp.Transform(GetMatrix());
            line.m_start_x = lp.P0.X;
            line.m_start_y = lp.P0.Y;
            line.m_end_x = lp.P1.X;
            line.m_end_y = lp.P1.Y;
            SetLineStyle(line);
            AddShape(line);
        }

        private void AddPolyline(IReadOnlyList<CadPoint> points, bool isPolygon)
        {
            //if (fill)
            //{
            //    AddFillPolygon(points);
            //}
            if (CurrentDC.LineStyle.IsNull) return;
            var p0 = points[0];
            for (var i = 1; i < points.Count; i++)
            {
                var p1 = points[i];
                AddLine(p0, p1);
                p0 = p1;
            }
            if (isPolygon) AddLine(p0, points[0]);
        }

        private void AddPath(IReadOnlyList<IPathElement> pathList, bool stroke, bool fill)
        {
            var ptsList = PathConverter.PathToPolylineList(pathList, 16);
            if (fill)
            {
                var poly = PolylineHelper.PolygonsToSinglePolygon(ptsList);
                AddFillPolygon(poly);
            }
            if (stroke)
            {
                foreach (var pts in ptsList) AddPolyline(pts, true);
            }
        }

        private void AddFillPolygon(IReadOnlyList<CadPoint> poly)
        {
            if (CurrentDC.FillStyle.IsNull) return;
            AddFillPolygon(poly, CurrentDC.FillStyle.Color);
        }
        private void AddFillPolygon(IReadOnlyList<CadPoint> poly, Color col)
        {
            var m = GetMatrix();
            var pp = new List<CadPoint>();
            foreach (var p in poly)
            {
                pp.Add(m * p);
            }
            var ts = PolylineHelper.ConvertPolygonToTriangle(pp);
            var jwwColor= col.R + (col.G << 8) + (col.B << 16);

            foreach (var t in ts)
            {
                var js = new JwwSolid()
                {
                    m_start_x = t.P0.X,
                    m_start_y = t.P0.Y,
                    m_end_x = t.P1.X,
                    m_end_y = t.P1.Y
                };
                js.m_nPenColor = 10;
                js.m_Color = jwwColor;

                js.m_DPoint2_x = t.P2.X;
                js.m_DPoint2_y = t.P2.Y;
                js.m_DPoint3_x = t.P0.X;
                js.m_DPoint3_y = t.P0.Y;
                js.m_nPenStyle = 0;
                AddShape(js);
            }
        }

        private void AddText(EmrExtTextOutW ppl)
        {
            var basis = GetTextBasis();

            var m = GetMatrix();
            var ts = CurrentDC.TextStyle;
            var js = new JwwMoji();
            js.m_string = EmfUtility.CharsToString(ppl.Text.Chars);
            var h = 1.0 * ToPaper(ts.FontHeight * (FloatEQ(ppl.EyScale, 0.0) ? 1.0f : 1.0f));// 25.64f / Abs(ppl.EyScale)));
            var size = ShapeHelper.GetStringSize(js.m_string, ts, h);
            if (js.m_string == "" || CadMath.FloatEQ(size.size.Width, 0) || CadMath.FloatEQ(size.size.Height, 0)) return;
            var p00 = ToPaper(ppl.Text.Reference);
            var p0 = p00.Copy();
            p0.X += size.size.Width * basis.Item1;
            p0.Y += size.size.Height * (-basis.Item2);
            if (basis.Item3) p0.Y -= size.ry * h;// size.size.Height;
            var p1 = p0.Copy();
            var p2 = p0.Copy();
            p1.X += size.size.Width;
            p2.Y += h;
            var m2 = new Matrix2D(m.A, m.B, m.C, m.D);
            var rp = CadPointHelper.TransformedRectangle(size.size.Width, h, ts.Angle, m2);
            var angle = DegToRad(ts.Angle);
            p0.Rotate(p00, angle);
            p1.Rotate(p00, angle);
            p0 = m * p0;
            p1 = m * p1;
            js.m_degKakudo = rp.AngleDeg;
            js.m_start_x = p0.X;
            js.m_start_y = p0.Y;
            js.m_end_x = p1.X;
            js.m_end_y = p1.Y;
            js.m_nMojiShu += ts.IsItalic ? 10000 : 0;
            js.m_nMojiShu += ts.IsBold ? 20000 : 0;
            js.m_strFontName = ts.FontName;
            js.m_dSizeY = rp.Height;
            js.m_dSizeX = js.m_dSizeY;
            if (js.m_string != "") AddShape(js);
        }


        private (double, double, bool) GetTextBasis()
        {
            var x = 0.0;
            var y = 0.0;
            var tx = (int)CurrentDC.TextAlign & (int)TextAlignment.TA_HORIZONTAL_MASK;
            var ty = (int)CurrentDC.TextAlign & (int)TextAlignment.TA_VERTICAL_MASK;
            if (tx == (int)TextAlignment.TA_RIGHT)
            {
                x = 1.0;
            }
            if (tx == (int)TextAlignment.TA_CENTER)
            {
                x = 0.5; ;
            }
            if (ty == (int)TextAlignment.TA_TOP) y = 1.0;
            var baseLine = ty == (int)TextAlignment.TA_BASELINE;
            return (x, y, baseLine);
        }


        private void OnPolylineNotPath(IReadOnlyList<CadPoint> points, bool isPolygon, bool isTo)
        {
            int pos = 0;
            if (isTo && points.Count == 1)
            {
                AddLine(CurrentDC.LastPoint, points[pos++]);
            }
            else if (!isTo && points.Count == 2)
            {
                var p0 = points[pos++];
                var p1 = points[pos++];
                AddLine(p0, p1);
            }
            else
            {
                var tmp = new List<CadPoint>();
                if (isTo) tmp.Add(CurrentDC.LastPoint);
                tmp.AddRange(points);
                if (isPolygon)
                {
                    AddFillPolygon(tmp);
                }
                AddPolyline(tmp, isPolygon);
            }

        }

        private bool OnPolyline(IReadOnlyList<CadPoint> points, bool isPolygon, bool isTo)
        {
            if (points.Count == 0) return false;
            if (mPathMaker != null)
            {
                var pos = 0;
                if (!isTo) mPathMaker.BeginPath(points[pos++]);
                for (; pos < points.Count; pos++) mPathMaker.AddLine(points[pos]);
                if (isPolygon) mPathMaker.EndPath(true);
            }
            else
            {
                OnPolylineNotPath(points, isPolygon, isTo);
            }
            return true;
        }


        private void OnPolyPolyline(EmrPolyPolyline ppl, bool isPolygon)
        {
            var pps = new List<List<CadPoint>>();
            {
                var pos = 0;
                for (var j = 0; j < ppl.NumberOfPolylines; j++)
                {
                    var m = ppl.PolylinePointCount[j];
                    var ps = new List<CadPoint>();
                    for (var i = 0; i < m; i++) ps.Add(ToPaper(ppl.Points[pos++]));
                    pps.Add(ps);
                }
            }
            if (pps.Count == 0) return;
            if (mPathMaker != null)
            {
                foreach (var ps in pps)
                {
                    var pos = 0;
                    mPathMaker.BeginPath(ps[pos++]);
                    for (; pos < ps.Count; pos++) mPathMaker.AddLine(ps[pos]);
                    mPathMaker.EndPath(isPolygon);
                }
            }
            else
            {
                foreach (var ps in pps)
                {
                    OnPolylineNotPath(ps, isPolygon, false);
                }
            }
        }

        private bool OnPolyBezier(IReadOnlyList<CadPoint> pts, bool isTo)
        {
            if (pts.Count < 3) return false;
            if (mPathMaker != null)
            {
                var pos = 0;
                if (!isTo) mPathMaker.BeginPath(pts[pos++]);
                for (; pos < pts.Count; pos += 3)
                {
                    mPathMaker.AddBezier(pts[pos], pts[pos + 1], pts[pos + 2]);
                }
            }
            else
            {
                var pos = 0;
                var bzs = new List<CadPoint>();
                if (isTo)
                {
                    bzs.Add(CurrentDC.LastPoint);
                }
                else
                {
                    bzs.Add(pts[pos++]);
                }
                for (; pos < pts.Count; pos++)
                {
                    bzs.Add(pts[pos]);
                }
                var pa = new List<CadPoint>();
                Beziers.EnumBezier3Points(bzs, (p0, p1, p2, p3, _) =>
                {
                    var n = 4;
                    var dr = double.MaxValue;
                    var dt = 1.0;
                    while (dr > 1.0)
                    {
                        n += 4;
                        var m = n * 2;
                        dt = 1.0 / m;
                        var bp0 = p0;
                        var drMax = 0.0;
                        for (var k = 1; k < m; k += 2)
                        {
                            var t = dt * k;
                            var bp1 = Beziers.GetBezier3Point(p0, p1, p2, p3, t);
                            var bp2 = Beziers.GetBezier3Point(p0, p1, p2, p3, t + dt);
                            var bpm = ((bp2 + bp0) * 0.5 - bp1);
                            drMax = Math.Max(drMax, bpm.Hypot());
                            bp0 = bp2;
                        }
                        dr = drMax;
                    }
                    dt = 1.0 / n;
                    for (var i = 0; i < n; i++)
                    {
                        var p = Beziers.GetBezier3Point(p0, p1, p2, p3, dt * i);
                        pa.Add(p);
                    }
                    return true;
                });
                pa.Add(bzs.Last());
                AddFillPolygon(pa);
                AddPolyline(pa, false);
            }
            return true;
        }

    }
}
