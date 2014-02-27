using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    public class ScaleInfo
    {
        private static int MaxNameLength;

        public readonly string Name;
        public readonly Quality Quality;
        public readonly Brightness Brightness;
        public readonly bool IsCommon;
        public readonly ScaleOrigin Origin;
        public readonly int Degree;

        public ScaleInfo(string name, ScaleOrigin origin, int degree, 
            bool isCommon = false, Quality quality = Quality.Undefined, Brightness brightness = Brightness.Undefined)
        {
            this.Name = name;
            this.Quality = quality;
            this.Brightness = brightness;
            this.IsCommon = isCommon;
            this.Origin = origin;
            this.Degree = degree;

            if (MaxNameLength < Name.Length)
                MaxNameLength = Name.Length;
        }

        public override string ToString()
        {
            return string.Format("{0,-" + MaxNameLength + "} {2,-8} {1,-9}",// from {4,-19} at {5} {3,-8}",
                Name, 
                Quality != Quality.Undefined ? Quality.ToString() : "", 
                Brightness != Brightness.Undefined ? Brightness.ToString() : "", 
                IsCommon ? "[COMMON]" : "", 
                Origin, 
                Degree);
        }
    }
}
