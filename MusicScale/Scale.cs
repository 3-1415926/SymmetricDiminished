using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    public struct Scale
    {
        public readonly ulong Mask;

        public Scale(ulong maskPattern)
        {
            if ((maskPattern & ~Common.OctaveMask) != 0)
                throw new ArgumentException("maskPattern should only include notes of one octave if populate is true");

            for (int shift = Common.OctaveLength; shift < Common.MaskLength; shift *= 2)
                maskPattern |= maskPattern << shift;

            Mask = maskPattern;
        }

        public static Scale FromNotes(IEnumerable<Note> notes)
        {
            ulong mask = 0;
            foreach (var note in notes)
            {
                mask |= (1UL << (byte)note);
            }
            return new Scale(mask);
        }

        public Scale Shift(int semitones)
        {
            semitones = Common.ModuloOctave(semitones);
            return new Scale(((Mask << semitones) | (Mask >> (Common.OctaveLength - semitones))) & Common.OctaveMask);
        }

        public IEnumerable<Note> GetNotes()
        {
            foreach (var note in Enum.GetValues(typeof(Note)))
            {
                var notePosition = (byte)note;
                var isEnabled = (this.Mask & (1UL << notePosition)) != 0;
                if (isEnabled)
                {
                    yield return (Note)note;
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Scale ? Equals((Scale)obj) : false;
        }

        public override int GetHashCode()
        {
            return Mask.GetHashCode();
        }

        public bool Equals(Chord other)
        {
            return Mask == other.Mask;
        }

        public static bool operator ==(Scale a, Scale b)
        {
            return a.Mask == b.Mask;
        }

        public static bool operator !=(Scale a, Scale b)
        {
            return a.Mask != b.Mask;
        }

        public override string ToString()
        {
            return Common.FormatMask(Mask, 0, Common.OctaveLength);
        }
    }
}
