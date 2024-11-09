using CadMath2D;
using CadMath2D.Path;
using EmfHelper;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace JwwClipMonitor.Emf
{
    class EmfDeviceContext
    {
        public MapMode MapMode = MapMode.MM_TEXT;
        public int Vxs = 1;
        public int Vys = 1;
        public int Wxs = 1;
        public int Wys = 1;
        public int WindowOrgX;
        public int WindowOrgY;
        public int ViewportOrgX;
        public int ViewportOrgY;
        public CadPoint LastPoint = new();
        public List<IPathElement>? PathList = null;
        public LineStyle LineStyle = new(0, Color.Black, null);
        public FillStyle FillStyle = FillStyle.NullLStyle;
        public TextStyle TextStyle = TextStyle.GetSystemTextStyle();
        public Color TextColor = Color.Black;
        public Color BKColor = Color.White;
        public BackgroundMode BKMode = BackgroundMode.OPAQUE;
        public TextAlignment TextAlign = 0;
        public ArcDirection ArcDirection = ArcDirection.AD_COUNTERCLOCKWISE;
        public Matrix2D XFormM = new();
        public CadPoint XFormD = new();

        public EmfDeviceContext()
        {
            //TextStyle.TextStyle.FontHeight = 14.0f;
        }
        public EmfDeviceContext Copy()
        {
            var dc = (EmfDeviceContext)MemberwiseClone();
            dc.LastPoint = LastPoint.Copy();
            dc.PathList = PathList?.ToList();
            //dc.LineStyle = LineStyle.Copy();
            //dc.FillStyle = FillStyle.Copy();
            dc.TextStyle = TextStyle.Copy();
            dc.XFormM = XFormM.Copy();
            dc.XFormD = XFormD.Copy();
            return dc;
        }
    }
}
