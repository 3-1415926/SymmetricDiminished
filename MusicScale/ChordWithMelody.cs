using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    public class ChordWithMelody
    {
        public readonly string ChordNotation;
        public readonly Chord Chord;
        public readonly Note[] MelodyNotes;
        public readonly ulong MelodyMask;

        public ChordWithMelody(string chordNotation, params Note[] melodyNotes)
        {
            this.ChordNotation = chordNotation;
            this.Chord = new Chord(chordNotation);
            this.MelodyNotes = melodyNotes;
            this.MelodyMask = 0;
            foreach (var note in melodyNotes)
                this.MelodyMask |= Common.NoteMaskInAllOctaves((int)note);
        }
    }
}
