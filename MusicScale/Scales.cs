using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    using Probe = Tuple<int, Func<ChordWithMelody, ulong>, string>;

    public static class Scales
    {
        public static readonly Scale[] Major, MelodicMinor, HarmonicMajor, HarmonicMinor, Diminished, All;

        private static readonly string[] MajorNames = 
        { 
            "Ionian (I)                           major, normal", 
            "Dorian (II)                          minor, brighter",
            "Phrygian (III)                       minor, darker",
            "Lydian (IV)                          major, brighter",
            "Mixolydian (V)                       dominant 7",
            "Aeolian (VI)                         minor, normal",
            "Locrian (VII)                        minor, darkest",
        };

        private static readonly string[] MelodicMinorNames =
        {
            "Ascending melodic minor (I)          minor, not common",
            "Phrygian nat.6 (Dorian b2) (II)      <excluded>",
            "Lydian #5 (Lydian augmented) (III)   <excluded>",
            "Lydian b7 (Lydian dominant) (IV)     dominant 7",
            "Mixolydian b6 (melodic major) (V)    <excluded>",
            "Locrian nat.2 (half-diminished) (VI) <excluded>",
            "Altered (super Locrian) (VII)        dominant 7",
        };

        private static readonly string[] DiminishedNames = 
        {
            "Symmetric diminished (whole-half)    <excluded>",
            "Symmetric diminished (half-whole)    dominant 7",
        };

        private static readonly string[] HarmonicMajorNames =
        {
            "Harmonic major (I)                   <excluded>",
            "Dorian b5 (II)                       <excluded>",
            "Phrygian b4 (III)                    <excluded>",
            "Lydian b3 (IV)                       <excluded>",
            "Mixolydian b2 (V)                    <excluded>",
            "Lydian #2 #5 (VI)                    <excluded>",
            "Locrian diminished (VII)             <excluded>",
        };

        private static readonly string[] HarmonicMinorNames =
        {
            "Harmonic minor (I)                   <excluded>",
            "Locrian #6 (II)                      <excluded>",
            "Ionian aug (III)                     <excluded>",
            "Romanian (IV)                        <excluded>",
            "Phrygian dominant (V)                <excluded>",
            "Lydian #2 (VI)                       <excluded>",
            "Ultralocrian (VII)                   <excluded>",
        };

        private static readonly Tuple<Scale[], string[]>[] AllScalesNames;

        private static readonly Tuple<Scale[], string[]>[] NonHarmonicScalesNames;

        static Scales()
        {
            var majorGenerator = new Scale(Common.ParseMask("10101,1010101"));
            Major = Enumerable.Range(0, 12).Select(i => majorGenerator.Shift(-i)).ToArray();

            var melodicMinorGenerator = new Scale(Common.ParseMask("10110,1010101"));
            MelodicMinor = Enumerable.Range(0, 12).Select(i => melodicMinorGenerator.Shift(-i)).ToArray();

            var diminishedGenerator = new Scale(Common.ParseMask("101,101,101,101"));
            Diminished = Enumerable.Range(0, 3).Select(i => diminishedGenerator.Shift(-i)).ToArray();

            var harmonicMajorGenerator = new Scale(Common.ParseMask("10101,1011001"));
            HarmonicMajor = Enumerable.Range(0, 12).Select(i => harmonicMajorGenerator.Shift(-i)).ToArray();

            var harmonicMinorGenerator = new Scale(Common.ParseMask("10110,1011001"));
            HarmonicMinor = Enumerable.Range(0, 12).Select(i => harmonicMinorGenerator.Shift(-i)).ToArray();

            All = Major.Concat(MelodicMinor).Concat(Diminished).Concat(HarmonicMajor).Concat(HarmonicMinor).ToArray();

            AllScalesNames = new[]
            {
                Tuple.Create(Major, MajorNames),
                Tuple.Create(MelodicMinor, MelodicMinorNames),
                Tuple.Create(Diminished, DiminishedNames),
                Tuple.Create(HarmonicMajor, HarmonicMajorNames),
                Tuple.Create(HarmonicMinor, HarmonicMinorNames),
            };

            NonHarmonicScalesNames = new[]
            {
                Tuple.Create(Major, MajorNames),
                Tuple.Create(MelodicMinor, MelodicMinorNames),
                Tuple.Create(Diminished, DiminishedNames),
            };
        }

        private static Probe[] probes = 
            new[] { 0 }.Concat(Enumerable.Range(1, 3).SelectMany(i => new[] { -i, i })).SelectMany(d => new[] 
            { 
                new Probe(d, cwm => cwm.Chord.Mask, "Chord " + d.ToString("+0;-0;0")),
                new Probe(d, cwm => cwm.MelodyMask, "Melody " + d.ToString("+0;-0;0")),
            }).ToArray();

        public static IEnumerable<NamedScale> FindFit(Chord chord, bool includeHarmonicScales = true)
        {
            foreach (var scalesNames in includeHarmonicScales ? AllScalesNames : NonHarmonicScalesNames)
            {
                for (int i = 0; i < scalesNames.Item1.Length; i++)
                {
                    if ((chord.Mask & ~scalesNames.Item1[i].Mask) == 0 || FitsAlteredWith5th(chord, scalesNames.Item1, i))
                        yield return new NamedScale(scalesNames.Item1[i], FindName(chord.Root, i, scalesNames.Item1, scalesNames.Item2));
                }
            }
        }

        public static IEnumerable<NamedScale> FindFit(Progression progression, int index, bool includeHarmonicScales)
        {
            var results = FindFit(progression[index].Chord, includeHarmonicScales).ToList();

            var commonScaleMask = unchecked(0UL - 1);
            foreach (var result in results)
                commonScaleMask &= result.Scale.Mask;

            ulong probesMask = 0;
            foreach (var probe in probes)
            {
                probesMask |= Common.ChordInAllOctaves(probe.Item2(progression[Common.ModuloOctave(index + probe.Item1)]));
                bool foundFit = false;
                foreach (var result in results)
                {
                    if ((result.Scale.Mask & ~commonScaleMask & ~probesMask) == 0)
                    {
                        result.FitReason = probe.Item3;
                        foundFit = true;
                    }
                }
                if (foundFit)
                    break;
            }

            return results;
        }

        private static bool FitsAlteredWith5th(Chord chord, Scale[] scales, int scaleIndex)
        {
            const int alteredScaleShift = 11;
            return scales == MelodicMinor && scaleIndex == Common.ModuloOctave(alteredScaleShift - (int)chord.Root)
                && (chord.Mask & ~Common.NoteMaskInAllOctaves((int)chord.Root + 7) & ~scales[scaleIndex].Mask) == 0;
        }

        public static string FindName(Note baseNote, int scaleIndex, Scale[] scales, string[] scaleNames)
        {
            var mask = scales[0].Mask;
            int k = 0;
            for (int j = 0; j < Common.ModuloOctave(scaleIndex + (int)baseNote); j++)
            {
                if (mask % 2 != 0)
                    k++;
                mask /= 2;
            }
            return scaleNames[Common.Modulo(k, scaleNames.Length)];
        }
    }
}
