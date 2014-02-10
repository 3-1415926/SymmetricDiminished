using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    public delegate NoteInOctave NoteFabric(int octave);

    public struct NoteInOctave : IEquatable<NoteInOctave>
    {
        public Note Note;

        public int Octave;

        public int Semiintervals
        {
            get
            {
                return (this.Octave * Common.OctaveLength) + (byte)this.Note;
            }
        }

        public NoteInOctave(Note note, int octave)
        {
            if (octave < 1)
            {
                throw new ArgumentOutOfRangeException("octave", "Octave must be greater than zero");
            }

            this.Note = note;
            this.Octave = octave;
        }

        public static int operator -(NoteInOctave note1, NoteInOctave note2)
        {
            return note1.Semiintervals - note2.Semiintervals;
        }

        public static bool operator <(NoteInOctave note1, NoteInOctave note2)
        {
            return note1.Semiintervals < note2.Semiintervals;
        }

        public static bool operator <=(NoteInOctave note1, NoteInOctave note2)
        {
            return note1.Semiintervals <= note2.Semiintervals;
        }

        public static bool operator >(NoteInOctave note1, NoteInOctave note2)
        {
            return note1.Semiintervals > note2.Semiintervals;
        }

        public static bool operator >=(NoteInOctave note1, NoteInOctave note2)
        {
            return note1.Semiintervals >= note2.Semiintervals;
        }

        public static bool operator ==(NoteInOctave note1, NoteInOctave note2)
        {
            return note1.Semiintervals == note2.Semiintervals;
        }

        public static bool operator !=(NoteInOctave note1, NoteInOctave note2)
        {
            return note1.Semiintervals != note2.Semiintervals;
        }

        public static NoteFabric CreateNoteFabric(Note note)
        {
            return octave => new NoteInOctave(note, octave);
        }

        public bool Equals(NoteInOctave other)
        {
            return this.Note == other.Note && this.Octave == other.Octave;
        }
    }

    public static class N
    {
        public static NoteFabric C = NoteInOctave.CreateNoteFabric(Note.C);

        public static NoteFabric Csharp_Dflat = NoteInOctave.CreateNoteFabric(Note.Csharp_Dflat);

        public static NoteFabric D = NoteInOctave.CreateNoteFabric(Note.D);

        public static NoteFabric Dsharp_Eflat = NoteInOctave.CreateNoteFabric(Note.Dsharp_Eflat);

        public static NoteFabric E = NoteInOctave.CreateNoteFabric(Note.E);

        public static NoteFabric F = NoteInOctave.CreateNoteFabric(Note.F);

        public static NoteFabric Fsharp_Gflat = NoteInOctave.CreateNoteFabric(Note.Fsharp_Gflat);

        public static NoteFabric G = NoteInOctave.CreateNoteFabric(Note.G);

        public static NoteFabric Gsharp_Aflat = NoteInOctave.CreateNoteFabric(Note.Gsharp_Aflat);

        public static NoteFabric A = NoteInOctave.CreateNoteFabric(Note.A);

        public static NoteFabric Asharp_Bflat = NoteInOctave.CreateNoteFabric(Note.Asharp_Bflat);

        public static NoteFabric B = NoteInOctave.CreateNoteFabric(Note.B);
    }
}
