using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    public static class Common
    {
        private const ulong One = 1ul;

        public const int OctaveLength = 12;
        public const int MaskLength = sizeof(ulong) * 8;
        public const ulong OctaveMask = (One << OctaveLength) - 1;

        public static readonly Dictionary<char, int> NoteOffset = new Dictionary<char, int> { { 'C', 0 }, { 'D', 2 }, { 'E', 4 }, { 'F', 5 }, { 'G', 7 }, { 'A', 9 }, { 'B', 11 } };

        public static int ModuloOctave(int semitones)
        {
            return Modulo(semitones, OctaveLength);
        }

        public static int Modulo(int number, int @base)
        {
            int result = number % @base;
            return result < 0 ? result + @base : result;
        }

        public static ulong ParseMask(string str)
        {
            ulong result = 0;
            bool hadDigits = false;
            for (int i = str.Length - 1; i >= 0; i--)
            {
                char digit = str[i];
                if (digit != '1' && digit != '0')
                    continue;

                if (hadDigits)
                    result <<= 1;
                hadDigits = true;

                if (digit == '1')
                    result |= 1;
            }
            return result;
        }

        public static string FormatMask(ulong num, int offset = 0, int length = int.MaxValue, char noteChar = '1', char gapChar = '0', char? octaveSplit = ' ', char? midSplit = ',')
        {
            const int divideOctaveAt = 5;
            int count = offset;
            num >>= offset;
            var result = new StringBuilder();
            while (num != 0 && length > 0)
            {
                result.Append((num & 1) == 1 ? noteChar : gapChar);
                num >>= 1;
                count++;
                if (num != 0)
                {
                    if (octaveSplit != null && count % OctaveLength == 0)
                        result.Append(octaveSplit);
                    else if (midSplit != null && count % OctaveLength == divideOctaveAt)
                        result.Append(midSplit);
                }
                length--;
            }
            return result.ToString();
        }

        public static Note ParseNote(string note)
        {
            int offset = NoteOffset[note[0]];
            for (int j = 1; j < note.Length; j++)
                switch (note[j])
                {
                    case 'b':
                    case '♭':
                        offset = Common.ModuloOctave(offset - 1);
                        break;
                    case '#':
                    case '♯':
                        offset = Common.ModuloOctave(offset + 1);
                        break;
                    default:
                        throw new ArgumentException("Could not parse note symbol: " + note);
                }
            return (Note)offset;
        }

        public static ulong OneNoteMask(int noteOffset)
        {
            return One << noteOffset;
        }

        public static ulong NoteMaskInAllOctaves(int noteOffset)
        {
            noteOffset = ModuloOctave(noteOffset);
            ulong result = 0;
            for (ulong gen = One << noteOffset; gen != 0; gen <<= OctaveLength)
                result |= gen;
            return result;
        }

        public static ulong ChordInAllOctaves(ulong chordMask)
        {
            ulong result = 0;
            ulong noteMask = NoteMaskInAllOctaves(0);
            for (int i = 0; i < OctaveLength; i++, noteMask <<= 1)
                if ((noteMask & chordMask) != 0)
                    result |= noteMask;
            return result;
        }

        public static int GetSetBitsCount(ulong x)
        {
            int result = 0;

            while (x > 0)
            {
                if ((x & 1) == 1)
                {
                    result += 1;
                }
                x >>= 1;
            }

            return result;
        }
    }
}
