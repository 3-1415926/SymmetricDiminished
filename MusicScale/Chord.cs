using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MusicScale
{
    public struct Chord : IEquatable<Chord>
    {
        static readonly int[] perfectIntervals = { 4, 5, 11 };
        static readonly int[] majorIntervals = { 2, 6, 9, 13 };
        static readonly int[] minorIntervals = { 7 };
        static readonly Dictionary<int, int> intervalToSemitones;

        static Chord()
        {
            var offsets = Common.NoteOffset.Select(p => p.Value).OrderBy(x => x);
            const int dominantIndex = 4;
            int offset = offsets.Skip(dominantIndex).First();
            intervalToSemitones = 
                offsets.Concat(offsets.Select(o => o + Common.OctaveLength)).Concat(offsets.Select(o => o + Common.OctaveLength * 2))
                .Skip(dominantIndex).Select(x => x - offset)
                .Zip(Enumerable.Range(1, Common.NoteOffset.Count * 2), (a, b) => new { a, b })
                .ToDictionary(p => p.b, p => p.a);        
        }

        //TODO: support bass notation as in Cmaj7/D
        static readonly Regex chordRegex = new Regex(@"(?n)^"
            + @"(?<root>[A-G])"
            + @"((?<rootSharp>[#♯])|(?<rootFlat>[b♭]))*"
            + @"((?<minor>[-−]|m(in?)?)|(?<aug>\+|aug)|(?<dim>[o°]|dim)|(?<halfDim>[0øØ]))?"
            + @"(?<major>[MΔ]|[Mm]aj|dom)??"
            + @"(sus(?<susInt>[24]))?"
            + @"([/(]?(?<intAdd>add)?("
                + @"((?<intSharp>[+#♯ΔMj]|[Mm]aj)?" 
                    + @"(?<interval>" + string.Join("|", minorIntervals) + "))|"
                + @"(((?<intSharp>[+#♯])|(?<intFlat>[-−b♭°])|(?<intPrevMaj>[ΔMj]|[Mm]aj))?" 
                    + @"(?<interval>" + string.Join("|", majorIntervals) + "))|"
                + @"(((?<intSharp>[+#♯]|aug)|(?<intFlat>[-−b♭°]|dim)|(?<intPrevMaj>[ΔMj]|[Mm]aj))?" 
                    + @"(?<interval>" + string.Join("|", perfectIntervals) + "))"
            + @")[)]?)*$"
        );

        public readonly Note BaseNote;
        public readonly ulong Mask;

        public static Chord FromNotes(params string[] notes)
        {
            ulong mask = 0;
            int lastOffset = -1;
            foreach (var note in notes)
            {
                if (note == "")
                {
                    lastOffset += Common.OctaveLength;
                    continue;
                }
                int offset = (int)Common.ParseNote(note);
                while (offset <= lastOffset)
                    offset += Common.OctaveLength;
                mask |= Common.OneNoteMask(offset);
                lastOffset = offset;
            }
            return new Chord(Common.ParseNote(notes.First()), mask);
        }

        private Chord(Note baseNote, ulong mask)
        {
            BaseNote = baseNote;
            Mask = mask;
        }

        public Chord(string notation, params string[] extraNotes)
        {
            var match = chordRegex.Match(notation);
            if (!match.Success)
                throw new ArgumentException("Could not parse chord: " + notation);

            BaseNote = (Note)Common.ModuloOctave(Common.NoteOffset[match.Groups["root"].Value.Single()]
                + match.Groups["rootSharp"].Captures.Count
                - match.Groups["rootFlat"].Captures.Count);

            int groupIndex = 0;
            var intervals = (
                from g in chordRegex.GetGroupNames()
                where g.StartsWith("int")
                from c in match.Groups[g].Captures.Cast<Capture>()
                orderby c.Index
                let i = g == "interval" ? groupIndex++ : groupIndex
                group new { g, c } by i into ic
                select new
                {
                    Interval = int.Parse(ic.Single(ig => ig.g == "interval").c.Value),
                    Flags = ic.Where(ig => ig.g != "interval").Select(ig => ig.g).ToArray(),
                } into s
                select s).ToList();

            // Half dim notation contains an implied minor 7th, i.e. C0 = C07
            if (match.Groups["halfDim"].Success && !intervals.Any(i => i.Interval == 7))
                intervals.Add(new { Interval = 7, Flags = new string[0] });

            intervals.Sort((a, b) => a.Interval.CompareTo(b.Interval));

            // Tonic
            Mask = Common.OneNoteMask(0);

            //TODO: support sus without interval specifier (implied 4)
            // Third or sus2 or sus4
            if (match.Groups["susInt"].Success)
                Mask |= Common.OneNoteMask(intervalToSemitones[int.Parse(match.Groups["susInt"].Value)]);
            else if (!intervals.Any(i => i.Interval == 5 && !i.Flags.Any(f => f != "add")))
            {
                if (match.Groups["minor"].Success || match.Groups["dim"].Success || match.Groups["halfDim"].Success)
                    Mask |= Common.OneNoteMask(3);
                else
                    Mask |= Common.OneNoteMask(4);
            }

            // Fifth
            if (match.Groups["aug"].Success || intervals.Any(i => i.Interval == 5 && i.Flags.Contains("intSharp")))
                Mask |= Common.OneNoteMask(8);
            else if (match.Groups["dim"].Success || match.Groups["halfDim"].Success || intervals.Any(i => i.Interval == 5 && i.Flags.Contains("intFlat")))
                Mask |= Common.OneNoteMask(6);
            else
                Mask |= Common.OneNoteMask(7);

            // Additional intervals
            int previousInterval = 0;
            foreach (var i in intervals)
            {
                if (i.Interval == 5)
                    continue;

                int semitones = intervalToSemitones[i.Interval];
                if (i.Flags.Contains("intSharp") || i.Flags.Contains("intMajor"))
                    semitones++;
                else if (i.Flags.Contains("intFlat") || i.Flags.Contains("intMinor") || i.Interval == 7 && match.Groups["dim"].Success)
                    semitones--;

                Mask |= Common.OneNoteMask(semitones);

                if (!i.Flags.Contains("intAdd"))
                    for (int j = previousInterval < 7 ? 7 : previousInterval + 2; j < i.Interval; j += 2)
                    {
                        int impliedSemitones = intervalToSemitones[j];
                        if (j == 7)
                        {
                            if (match.Groups["dim"].Success)
                                impliedSemitones--;
                            if (i.Flags.Contains("intPrevMaj"))
                                impliedSemitones++;
                        }
                        Mask |= Common.OneNoteMask(impliedSemitones);
                    }

                previousInterval = i.Interval;
            }

            Mask <<= (int)BaseNote;

            foreach (var extraNote in extraNotes)
            {
                Mask |= Common.OneNoteMask((int)Common.ParseNote(extraNote));
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Chord ? Equals((Chord)obj) : false;
        }

        public override int GetHashCode()
        {
            return (Mask ^ ((ulong)BaseNote << (Common.MaskLength - 4))).GetHashCode();
        }

        public bool Equals(Chord other)
        {
            return this == other;
        }

        public static bool operator ==(Chord a, Chord b)
        {
            return a.Mask == b.Mask && a.BaseNote == b.BaseNote;
        }

        public static bool operator !=(Chord a, Chord b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return Common.FormatMask(Mask, (int)BaseNote);
        }
    }
}
