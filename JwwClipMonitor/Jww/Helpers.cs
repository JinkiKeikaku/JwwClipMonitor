using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using static System.MathF;
using static CadMath2D.CadMath;
using CadMath2D;

namespace JwwClipMonitor.Jww
{
    static class Helpers
    {
        /// <summary>
        /// GDIの色をGDI+の色に変換
        /// </summary>
        public static Color ToColor(this int c)
        {
            return Color.FromArgb((byte)(c & 255), (byte)((c >> 8) & 255), (byte)((c >> 16) & 255));
        }

        public static CadRect ArcExtent(
            CadPoint p0, double radius, double flatness, double rad, double startAngle, double endAngle)
        {
            CadPoint ArcPoint(double a)
            {
                var p = CadPoint.Pole(a);
                p.Magnify(radius, radius * flatness);
                p.Rotate(rad);
                return p;
            }
            var da = DegToRad(10.0);
            var r = new CadRect(ArcPoint(startAngle), ArcPoint(endAngle));
            var ssa = startAngle;
            var eea = endAngle;
            if (endAngle < startAngle)
            {
                ssa = endAngle;
                eea = startAngle;
            }
            var a = ssa;
            while (FloatLE(a, eea))// a < eea)
            {
                var p = ArcPoint(a);
                r.Spread(p);
                a += da;
            }
            r.Offset(p0);
            return r;
        }


        /// <summary>
        /// 点の引き算
        /// </summary>
        public static PointF Sub(this PointF p1, PointF p2)
        {
            return new PointF(p1.X - p2.X, p1.Y - p2.Y);
        }

        /// <summary>
        /// 点の足し算
        /// </summary>
        public static PointF Add(this PointF p1, PointF p2)
        {
            return new PointF(p1.X + p2.X, p1.Y + p2.Y);
        }


        /// <summary>
        /// 点の回転。角度単位はRadian。
        /// </summary>
        public static PointF Rotate(this PointF p, float rad)
        {
            var c = Cos(rad);
            var s = Sin(rad);
            return new PointF(p.X * c - p.Y * s, p.X * s + p.Y * c);
        }
        /// <summary>
        /// 直線[p11]-[p12]と[p21]-[p22]の交点を返す。交点がない場合はタプルの[flag]がfalse。
        /// </summary>
        public static (PointF p, bool flag) GetCrossPoint(PointF p11, PointF p12, PointF p21, PointF p22)
        {
            var dp1 = Sub(p12, p11);
            var dp2 = Sub(p22, p21);
            var dp3 = Sub(p11, p21);
            var a = dp1.X * dp2.Y - dp2.X * dp1.Y;
            if (FloatEQ(a, 0.0f)) return (new PointF(), false);
            var t = (dp2.X * dp3.Y - dp3.X * dp2.Y) / a;
            var cp = new PointF(dp1.X * t + p11.X, dp1.Y * t + p11.Y);
            return (cp, true);
        }


        /// <summary>
        /// ヘッダーの図面サイズコード（？）から用紙サイズを取得
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static SizeF GetPaperSize(int code)
        {
            if (code < 15)
            {
                return mPaperSize[code];
            }
            return mPaperSize[3];    //わからなかったらひとまずA3
        }

        /// <summary>
        /// 用紙サイズのテーブル
        /// </summary>
        static readonly SizeF[] mPaperSize = new SizeF[]{
            new SizeF(1189, 841), //A0
            new SizeF(841, 594),  //A1
            new SizeF(594, 420),  //A2
            new SizeF(420, 297),  //A3
            new SizeF(297, 210),  //A4
            new SizeF(210, 148),  //A5???使わない
            new SizeF(210, 148),  //A6???使わない
            new SizeF(148, 105),  //A7???使わない
            new SizeF(1682, 1189),  //8:2A
            new SizeF(2378, 1682),  //9:3A
            new SizeF(3364, 2378),  //10:4A
            new SizeF(4756, 3364),  //11:5A
            new SizeF(10000, 7071),  //12:10m
            new SizeF(50000, 35355),  //13:50m
            new SizeF(100000, 70711)  //14:100m
        };
    }
}
