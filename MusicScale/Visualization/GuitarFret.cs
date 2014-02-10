using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MusicScale.Visualization
{
    using System.Drawing.Imaging;

    public static class KnownGuitarTunings
    {
        public static IList<NoteInOctave> Standard = new List<NoteInOctave> { N.E(4), N.B(3), N.G(3), N.D(3), N.A(2), N.E(2) };
    }

    public class Guitar
    {
        public int StringsCount { get; private set; }

        public int FretCount { get; private set; }

        public IList<NoteInOctave> Tuning { get; private set; }

        public Guitar(int stringsCount = 6, IEnumerable<NoteInOctave> tuning = null, int fretCount = 16)
        {
            this.Tuning = tuning == null ? KnownGuitarTunings.Standard : tuning.ToList();

            this.StringsCount = stringsCount;

            this.FretCount = fretCount;
        }
    }
}
