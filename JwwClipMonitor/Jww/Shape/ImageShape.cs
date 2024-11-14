using CadMath2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CadMath2D.CadMath;

namespace JwwClipMonitor.Jww.Shape
{
    internal class ImageShape:IShape
    {
        public CadPoint P0 = new();
        public double Width;
        public double Height;
        public double Angle;
        public Image Image;

        public ImageShape(CadPoint p0, double width, double height, double angle, Bitmap image)
        {
            P0.Set(p0);
            Width = width;
            Height = height;
            Angle = angle;
            Image = image;
        }

        public IShape Clone()
        {
            return new ImageShape(P0, Width, Height, Angle, new Bitmap(Image));
            throw new NotImplementedException();
        }

        public CadRect GetExtent()
        {
            var r = new CadRect(P0, new CadSize(Width, Height));
            r.SpreadRectByRotate(P0, DegToRad(Angle));  
            return r;
        }


        public void OnDraw(Graphics g, DrawContext d)
        {
            var saved = g.Save();
            var p = d.DocToCanvas(P0);
            g.TranslateTransform(p.X, p.Y);
            g.RotateTransform(d.DocToCanvasAngle(Angle));
            g.DrawImage(Image, 0, -(float)Height, (float)Width, (float)Height);
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
            var rp = CadPointHelper.MagnifyRotatedRectangle(Width, Height, Angle, mx, my);
            Width = rp.Width;
            Height = rp.Height;
            Angle = rp.AngleDeg;
            P0.Rotate(p0, DegToRad(Angle));
        }

    }
}
