using System.Collections.Generic;

namespace MusicScale.Visualization
{
    public static class KnownGuitarTunings
    {
        public static IList<NoteInOctave> Standard = new List<NoteInOctave> { N.E(4), N.B(3), N.G(3), N.D(3), N.A(2), N.E(2) };
    }
}