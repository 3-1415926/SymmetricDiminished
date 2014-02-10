using System.Collections.Generic;
using System.Linq;

namespace MusicScale.Visualization
{
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

        public int LocateNoteOnString(NoteInOctave note, int stringNum)
        {
            var lowestNote = this.Tuning[stringNum];
            if (lowestNote > note)
            {
                return -1;
            }

            var semitones = note - lowestNote;
            if (semitones > this.FretCount)
            {
                return -1;
            }

            return semitones;
        }
    }
}