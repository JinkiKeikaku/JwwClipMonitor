using CadMath2D;
using JwwClipMonitor.Jww.Shape;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Math;

namespace JwwClipMonitor.Jww
{
    internal class JwwReader
    {
        Dictionary<int, GroupShape> mBlockMap = new();
        List<JwwDataList> mBlocks = null!;
        public Document Document = new();
        public void Read(MemoryStream ms)
        {
            Document = new();
            var jwr = new JwwHelper.JwwClipReader();
            jwr.Read(ms.ToArray(), Completed);
        }

        private void Completed(JwwClipReader clipReader)
        {
            var sc = new StyleConverter();
            mBlockMap.Clear();
            mBlocks = clipReader.DataListList;
            //最後の要素は画像ファイルの保存場所が入るようだ。
            //今のところ画像は対応しないので無視する。
            for(var i = 0; i < clipReader.DataList.Count; i++)
            {
                var js = clipReader.DataList[i];
                try
                {
                    var s = Convert(sc, js);
                    if (s != null) Document.Shapes.Add(s);
                }catch(Exception ex)
                {
                    Debug.WriteLine($"{ex.Message}", GetType().Name);
                }
            }

            if (Document.Shapes.Count > 0)
            {
                Document.Bounds.Set(Document.Shapes[0].GetExtent());
            }
            for (var i = 1; i < Document.Shapes.Count; i++)
            {
                Document.Bounds.Spread(Document.Shapes[i].GetExtent());
            }
        }
        private IShape? Convert(StyleConverter sc, JwwData js)
        {
            switch (js)
            {
                case JwwSen s:
                    return new LineShape(sc, s);
                case JwwEnko s:
                    return s.m_bZenEnFlg == 1 ? new CircleShape(sc, s) : new ArcShape(sc, s);
                case JwwTen s:
                    return new DotShape(sc, s);
                case JwwMoji s:
                {
                    //画像は未対応
                    //var img = CreateImageShape(s);
                    //if (img != null) return img;
                    if (Abs(s.m_end_x - s.m_start_x) <= 0.1 && Abs(s.m_end_y - s.m_start_y) <= 0.1) return null;
                    return new TextShape(sc, s);
                }
                case JwwSolid s:
                {
                    if (s.m_nPenStyle < 101) return new SolidPolygon(sc, s);
                    return new SolidCircle(sc, s);
                }
                case JwwSunpou s:
                {
                    var g = new GroupShape();
                    var sen = Convert(sc, s.m_Sen);
                    if (sen != null) g.Add(sen);
                    var moji = Convert(sc, s.m_Moji);
                    if (moji != null) g.Add(moji);
                    return g;
                }
                case JwwBlock jj:
                {
                    IShape? s = null;
                    if (mBlockMap.ContainsKey(jj.m_nNumber))
                    {
                        s = mBlockMap[jj.m_nNumber].Clone();
                    }
                    else
                    {
                        var be = mBlocks.Find(x => x.m_nNumber == jj.m_nNumber);
                        if (be != null)
                        {
                            var g = new GroupShape();
                            mBlockMap[jj.m_nNumber] = g;
                            be.EnumerateDataList(b =>
                            {
                                var ss = Convert(sc, b);
                                if (ss != null)
                                {
                                    g.Add(ss);
                                }
                                return true;
                            });
                            s = g.Clone();
                        }
                    }
                    if (s != null)
                    {
                        var p0 = new CadPoint();
                        s.Scale(p0, jj.m_dBairitsuX, jj.m_dBairitsuY);
                        s.Rotate(p0, jj.m_radKaitenKaku);
                        s.Offset(jj.m_DPKijunTen_x, jj.m_DPKijunTen_y);
                    }
                    return s;
                }
            }
            return null;
        }

        ImageShape? CreateImageShape(JwwMoji jj)
        {
            if (!jj.m_string.StartsWith("^@BM")) return null;
            var s0 = jj.m_string.Substring(4);
            var s1 = s0.Split(',');
            var name = s1[0];
            Debug.WriteLine($"org_name={name}", GetType().Name);
            var tmpFolder = Path.GetTempPath();
            name = name.Replace("%temp%", tmpFolder);
            name = name.Replace("\\", "/");
            Debug.WriteLine($"replace_name={name}", GetType().Name);
            var image = Bitmap.FromFile(name) as Bitmap;
            if(image == null) return null;
            var p0 = new CadPoint(jj.m_start_x, jj.m_start_y);
            if (double.TryParse(s1[1], out double w) &&
                double.TryParse(s1[2], out double h))
            {
                double angle = 0;
                if(s1.Length > 6)
                {
                    double.TryParse(s1[6], out angle);
                }
                return new ImageShape(p0, w, h, angle, image);
            }
            return null;

        }
    }

}
