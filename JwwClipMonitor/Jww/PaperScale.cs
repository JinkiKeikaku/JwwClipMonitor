using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww
{
    class PaperScale
    {
        /// <summary>
        /// 縮尺名 ex. 1:10
        /// </summary>
        public string Name { get; set; } = "1:1";
        /// <summary>
        /// 縮尺
        /// </summary>
        public double Scale { get; set; } = 1.0;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PaperScale()
        {
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">縮尺名 ex. 1:10</param>
        /// <param name="scale">縮尺</param>
        public PaperScale(string name, double scale)
        {
            this.Name = name;
            this.Scale = scale;
        }
        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }
}
