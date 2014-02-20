using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
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
            "Ascending melodic minor (I)          <excluded>",
            "Phrygian nat.6 (Dorian b2) (II)      <excluded>",
            "Lydian #5 (Lydian augmented) (III)   <excluded>",
            "Lydian b7 (Lydian dominant) (IV)     dominant 7",
            "Mixolydian b6 (melodic major) (V)    <excluded>",
            "Locrian nat.2 (half-diminished) (VI) <excluded>",
            "Altered (super Locrian) (VII)        dominant 7",
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

        private static readonly string[] DiminishedNames = 
        {
            "Symmetric diminished (whole-half)    dominant 7",
            "Symmetric diminished (half-whole)    <excluded>",
        };

        private static readonly Tuple<Scale[], string[]>[] AllScalesNames;

        static Scales()
        {
            var majorGenerator = new Scale(Common.ParseMask("10101,1010101"));
            Major = Enumerable.Range(0, 12).Select(i => majorGenerator.Shift(-i)).ToArray();

            var melodicMinorGenerator = new Scale(Common.ParseMask("10110,1010101"));
            MelodicMinor = Enumerable.Range(0, 12).Select(i => melodicMinorGenerator.Shift(-i)).ToArray();

            var harmonicMajorGenerator = new Scale(Common.ParseMask("10101,1011001"));
            HarmonicMajor = Enumerable.Range(0, 12).Select(i => harmonicMajorGenerator.Shift(-i)).ToArray();

            var harmonicMinorGenerator = new Scale(Common.ParseMask("10110,1011001"));
            HarmonicMinor = Enumerable.Range(0, 12).Select(i => harmonicMinorGenerator.Shift(-i)).ToArray();

            var diminishedGenerator = new Scale(Common.ParseMask("101,101,101,101"));
            Diminished = Enumerable.Range(0, 3).Select(i => diminishedGenerator.Shift(-i)).ToArray();

            All = Major.Concat(MelodicMinor).Concat(HarmonicMajor).Concat(HarmonicMinor).Concat(Diminished).ToArray();

            AllScalesNames = new[]
            {
                Tuple.Create(Major, MajorNames),
                Tuple.Create(MelodicMinor, MelodicMinorNames),
                Tuple.Create(HarmonicMajor, HarmonicMajorNames),
                Tuple.Create(HarmonicMinor, HarmonicMinorNames),
                Tuple.Create(Diminished, DiminishedNames),
            };
        }

        public static IEnumerable<NamedScale> FindFit(Chord chord)
        {
            foreach (var scalesNames in AllScalesNames)
                for (int i = 0; i < scalesNames.Item1.Length; i++)
                {
                    if ((chord.Mask & ~scalesNames.Item1[i].Mask) == 0 ||
                        chord.HasAlteredNotes && scalesNames.Item1 == MelodicMinor // special handling for implied natural 5th in Altered scale
                            && (chord.Mask & ~Common.NoteMaskInAllOctaves((int)chord.BaseNote + 7) & ~scalesNames.Item1[i].Mask) == 0)
                        yield return new NamedScale(scalesNames.Item1[i], FindName(chord.BaseNote, i, scalesNames.Item1, scalesNames.Item2));
                }
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
