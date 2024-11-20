using JwwHelper;
using System.Collections.Generic;
using System.Drawing;
using CadMath2D;
using JwwClipMonitor.Jww.Shape;
using System.Drawing.Drawing2D;

namespace JwwClipMonitor.Jww
{
    /// <summary>
    /// 描画用の情報保持クラス
    /// </summary>
    class DrawContext
    {
        private Dictionary<int, Color> mColorMap = null;
        public Pen Pen = new(Color.Black, 0.0f);
        public float Scale = 1.0f;
        public List<BlockEntity> BlockEntities { get; }
        public CadRect Bounds = new();
        public double CirclePrecision = 0.1;
        public int MaxDivideValueOfQuaterCircleForDisplay = 64;
        public DrawContext(CadRect bounds, List<BlockEntity> blockEntities)
        {
            Bounds.Set(bounds);
            BlockEntities = blockEntities;
        }

        /// <summary>
        /// DocumentとGDI+の半径などの変換。
        /// </summary>
        public float DocToCanvas(double radius)
        {
            return (float)radius;
        }

        /// <summary>
        /// DocumentとGDI+の座標変換。Jwwは上が正なのでｙ座標のみ符号を変える。
        /// </summary>
        public PointF DocToCanvas(double x, double y)
        {
            return new PointF((float)x, (float)-y);
        }

        /// <summary>
        /// DocumentとGDI+の座標変換。Jwwは上が正なのでｙ座標のみ符号を変える。
        /// </summary>
        public PointF DocToCanvas(CadPoint p)
        {
            return DocToCanvas(p.X, p.Y);
        }

        /// <summary>
        /// DocumentとGDI+の座標変換。
        /// </summary>
        public RectangleF DocToCanvas(CadRect r)
        {
            return new RectangleF((float)r.Left, -(float)r.Top, (float)(r.Right - r.Left), (float)(r.Top - r.Bottom));
        }

        /// <summary>
        /// DocumentとGDI+の角度の変換。Jwwの角度は左回り。GDI+は右回り。
        /// 符号を変えるだけだが座標変換に合わせて間違えないようにあえてこれを使う。
        /// </summary>
        public float DocToCanvasAngle(double angle)
        {
            return -(float)angle;
        }

        //public void ApplyLineStyle(LineStyle lineStyle)
        //{
        //    Pen.Color = lineStyle.Color;
        //    Pen.Width = lineStyle.Width;
        //    if(lineStyle.Pattern == null)
        //    {
        //        Pen.DashStyle = DashStyle.Solid;
        //    }
        //    else
        //    {
        //        Pen.DashStyle = DashStyle.Custom;
        //        Pen.DashPattern = lineStyle.Pattern;
        //    }
        //}
        public void ApplyDotStyle(DotStyle dotStyle)
        {
            Pen.Color = LineStyle.ConvertDrawColor(dotStyle.Color);
            Pen.Width = dotStyle.Width;
            Pen.DashStyle = DashStyle.Solid;
        }
    }
}
