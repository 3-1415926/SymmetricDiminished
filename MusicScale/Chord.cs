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

        static readonly Regex chordRegex = new Regex(@"(?n)^"
            + @"(?<root>[A-G][#♯b♭]*)"
            + @"((?<minor>[-−]|m(in?)?)|(?<aug>\+|aug)|(?<dim>[Oo°]|dim)|(?<halfDim>[0øØ]))?"
            + @"(?<major>[MΔ]|[Mm]aj?|dom)??"
            + @"([/(]?((?<intAdd>add)?("
                + @"((?<intSharp>[+#♯♮ΔMj]|nat|[Mm]aj?)?" 
                    + @"(?<interval>" + string.Join("|", minorIntervals) + "))|"
                + @"(((?<intSharp>[+#♯])|(?<intFlat>[-−b♭°])|(?<intPrevMaj>[ΔMj]|[Mm]aj?))?" 
                    + @"(?<interval>" + string.Join("|", majorIntervals) + "))|"
                + @"(((?<intSharp>[+#♯]|aug)|(?<intFlat>[-−b♭°]|dim)|(?<intPrevMaj>[ΔMj]|[Mm]aj?))?" 
                    + @"(?<interval>" + string.Join("|", perfectIntervals) + "))"
            + @")|(sus(?<susInt>[24]?)))[)]?)*?"
            + @"(/(?<bass>[A-G][#♯b♭]*))?$"
        );

        public readonly Note Root;
        public readonly Note Bass;
        public readonly ulong Mask;
        public readonly Quality Quality;

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

        private Chord(Note root, ulong mask)
        {
            Root = root;
            Bass = root;
            Mask = mask;
            Quality = Quality.Undefined;
        }

        public Chord(string notation, params string[] extraNotes)
        {
            var match = chordRegex.Match(notation);
            if (!match.Success)
                throw new ArgumentException("Could not parse chord: " + notation);

            Root = Common.ParseNote(match.Groups["root"].Value);
            Quality = Quality.Undefined;

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

            // Third or sus2 or sus4
            if (match.Groups["susInt"].Success)
            {
                int susInterval = string.IsNullOrEmpty(match.Groups["susInt"].Value) ? 4 : int.Parse(match.Groups["susInt"].Value);
                Mask |= Common.OneNoteMask(intervalToSemitones[susInterval]);
            }
            else if (!intervals.Any(i => i.Interval == 5 && !i.Flags.Any(f => f != "add")))
            {
                if (match.Groups["minor"].Success || match.Groups["dim"].Success || match.Groups["halfDim"].Success)
                {
                    Mask |= Common.OneNoteMask(3);
                    if (match.Groups["minor"].Success)
                        Quality = Quality.Minor;
                }
                else
                {
                    Mask |= Common.OneNoteMask(4);
                    Quality = Quality.Major;
                }
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
            bool foundMinor7th = false;
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
                            if (impliedSemitones < Common.NoteOffset.Last().Value)
                                foundMinor7th = true;                        }
                        Mask |= Common.OneNoteMask(impliedSemitones);
                    }

                if (i.Interval == 7 && semitones < Common.NoteOffset.Last().Value)
                    foundMinor7th = true;

                previousInterval = i.Interval;
            }

            if (foundMinor7th && (Quality == Quality.Major || match.Groups["susInt"].Success
                || match.Groups["halfDim"].Success) || match.Groups["dim"].Success)
                Quality = Quality.Dominant;

            Bass = Root;
            if (match.Groups["bass"].Success)
                Bass = Common.ParseNote(match.Groups["bass"].Value);

            if (Root < Bass)
                Root += Common.OctaveLength;

            Mask <<= (int)Root;

            Mask |= Common.OneNoteMask((int)Bass);

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
            return Mask.GetHashCode();
        }

        public bool Equals(Chord other)
        {
            return this == other;
        }

        public static bool operator ==(Chord a, Chord b)
        {
            return a.Mask == b.Mask;
        }

        public static bool operator !=(Chord a, Chord b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return Common.FormatMask(Mask, (int)Bass);
        }
    }
}
