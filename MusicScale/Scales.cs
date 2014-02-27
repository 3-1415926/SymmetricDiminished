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

        private static readonly ScaleInfo[] MajorNames = 
        { 
            new ScaleInfo("Ionian",     ScaleOrigin.CommonMajor, 1, true, Quality.Major,    Brightness.Normal  ),
            new ScaleInfo("Dorian",     ScaleOrigin.CommonMajor, 2, true, Quality.Minor,    Brightness.Brighter),
            new ScaleInfo("Phrygian",   ScaleOrigin.CommonMajor, 3, true, Quality.Minor,    Brightness.Darker  ),
            new ScaleInfo("Lydian",     ScaleOrigin.CommonMajor, 4, true, Quality.Major,    Brightness.Brighter),
            new ScaleInfo("Mixolydian", ScaleOrigin.CommonMajor, 5, true, Quality.Dominant  ),
            new ScaleInfo("Aeolian",    ScaleOrigin.CommonMajor, 6, true, Quality.Minor,    Brightness.Normal  ),
            new ScaleInfo("Locrian",    ScaleOrigin.CommonMajor, 7, true, Quality.Minor,    Brightness.Darkest ),
        };

        private static readonly ScaleInfo[] MelodicMinorNames =
        {
            new ScaleInfo("Ascending melodic minor",         ScaleOrigin.MelodicMinor, 1, true, Quality.Minor),
            new ScaleInfo("Phrygian nat.6 (Dorian b2)",      ScaleOrigin.MelodicMinor, 2),
            new ScaleInfo("Lydian #5 (Lydian augmented)",    ScaleOrigin.MelodicMinor, 3),
            new ScaleInfo("Lydian b7 (Lydian dominant)",     ScaleOrigin.MelodicMinor, 4, true, Quality.Dominant),
            new ScaleInfo("Mixolydian b6 (melodic major)",   ScaleOrigin.MelodicMinor, 5),
            new ScaleInfo("Locrian nat.2 (half-diminished)", ScaleOrigin.MelodicMinor, 6),
            new ScaleInfo("Altered (super Locrian)",         ScaleOrigin.MelodicMinor, 7, true, Quality.Dominant),
        };

        private static readonly ScaleInfo[] DiminishedNames = 
        {
            new ScaleInfo("Symmetric diminished (whole-half)", ScaleOrigin.SymmetricDiminished, 1),
            new ScaleInfo("Symmetric diminished (half-whole)", ScaleOrigin.SymmetricDiminished, 2, true, Quality.Dominant),
        };

        private static readonly ScaleInfo[] HarmonicMajorNames =
        {
            new ScaleInfo("Harmonic major",     ScaleOrigin.HarmonicMajor, 1),
            new ScaleInfo("Dorian b5",          ScaleOrigin.HarmonicMajor, 2),
            new ScaleInfo("Phrygian b4",        ScaleOrigin.HarmonicMajor, 3),
            new ScaleInfo("Lydian b3",          ScaleOrigin.HarmonicMajor, 4),
            new ScaleInfo("Mixolydian b2",      ScaleOrigin.HarmonicMajor, 5),
            new ScaleInfo("Lydian #2 #5",       ScaleOrigin.HarmonicMajor, 6),
            new ScaleInfo("Locrian diminished", ScaleOrigin.HarmonicMajor, 7),
        };

        private static readonly ScaleInfo[] HarmonicMinorNames =
        {
            new ScaleInfo("Harmonic minor",    ScaleOrigin.HarmonicMinor, 1),
            new ScaleInfo("Locrian #6",        ScaleOrigin.HarmonicMinor, 2),
            new ScaleInfo("Ionian aug",        ScaleOrigin.HarmonicMinor, 3),
            new ScaleInfo("Romanian",          ScaleOrigin.HarmonicMinor, 4),
            new ScaleInfo("Phrygian dominant", ScaleOrigin.HarmonicMinor, 5),
            new ScaleInfo("Lydian #2",         ScaleOrigin.HarmonicMinor, 6),
            new ScaleInfo("Ultralocrian",      ScaleOrigin.HarmonicMinor, 7),
        };

        private static readonly Tuple<Scale[], ScaleInfo[]>[] AllScalesInfos;

        private static readonly Tuple<Scale[], ScaleInfo[]>[] NonHarmonicScalesInfos;

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

            AllScalesInfos = new[]
            {
                Tuple.Create(Major, MajorNames),
                Tuple.Create(MelodicMinor, MelodicMinorNames),
                Tuple.Create(Diminished, DiminishedNames),
                Tuple.Create(HarmonicMajor, HarmonicMajorNames),
                Tuple.Create(HarmonicMinor, HarmonicMinorNames),
            };

            NonHarmonicScalesInfos = new[]
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
            foreach (var scalesInfos in includeHarmonicScales ? AllScalesInfos : NonHarmonicScalesInfos)
            {
                for (int i = 0; i < scalesInfos.Item1.Length; i++)
                {
                    if ((chord.Mask & ~scalesInfos.Item1[i].Mask) == 0 || FitsAlteredWith5th(chord, scalesInfos.Item1, i))
                        yield return new NamedScale(scalesInfos.Item1[i], FindInfo(chord.Root, i, scalesInfos.Item1, scalesInfos.Item2));
                }
            }
        }

        public static IEnumerable<NamedScale> FindFit(Progression progression, int index, bool includeHarmonicScales)
        {
            var allResults = FindFit(progression[index].Chord, includeHarmonicScales).ToList();
            var results = allResults.Where(scale => scale.Info.IsCommon);
               // && (progression[index].Chord.Quality == Quality.Undefined ? true : progression[index].Chord.Quality == scale.Info.Quality));

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

        public static ScaleInfo FindInfo(Note baseNote, int scaleIndex, Scale[] scales, ScaleInfo[] scaleInfos)
        {
            var mask = scales[0].Mask;
            int k = 0;
            for (int j = 0; j < Common.ModuloOctave(scaleIndex + (int)baseNote); j++)
            {
                if (mask % 2 != 0)
                    k++;
                mask /= 2;
            }
            return scaleInfos[Common.Modulo(k, scaleInfos.Length)];
        }
    }
}
