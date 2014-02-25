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
        public readonly string Name;
        public string FitReason;

        public NamedScale(Scale scale, string name)
        {
            this.Scale = scale;
            this.Name = name;
        }

        public override string ToString()
        {
            return Scale.ToString() + "   " + Name;
        }
    }
}
