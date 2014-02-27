using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    public class NamedScale
    {
        public readonly Scale Scale;
        public readonly ScaleInfo Info;
        public string FitReason;

        public NamedScale(Scale scale, ScaleInfo info)
        {
            this.Scale = scale;
            this.Info = info;
        }

        public override string ToString()
        {
            return Scale.ToString() + "   " + Info;
        }
    }
}
