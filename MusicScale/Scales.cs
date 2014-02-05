using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    public static class Scales
    {
        public static readonly Scale[] Major, MelodicMinor, Diminished, All;

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

            var diminishedGenerator = new Scale(Common.ParseMask("101,101,101,101"));
            Diminished = Enumerable.Range(0, 3).Select(i => diminishedGenerator.Shift(-i)).ToArray();

            All = Major.Concat(MelodicMinor).Concat(Diminished).ToArray();

            AllScalesNames = new[]
            {
                Tuple.Create(Major, MajorNames),
                Tuple.Create(MelodicMinor, MelodicMinorNames),
                Tuple.Create(Diminished, DiminishedNames),
            };
        }

        public static IEnumerable<NamedScale> FindFit(Chord chord)
        {
            foreach (var scalesNames in AllScalesNames)
                for (int i = 0; i < scalesNames.Item1.Length; i++)
                    if ((chord.Mask & ~scalesNames.Item1[i].Mask) == 0)
                    {
                        var mask = scalesNames.Item1[0].Mask;
                        int k = 0;
                        for (int j = 0; j < Common.ModuloOctave(i + (int)chord.BaseNote); j++)
                        {
                            if (mask % 2 != 0)
                                k++;
                            mask /= 2;
                        }
                        yield return new NamedScale(scalesNames.Item1[i], scalesNames.Item2[Common.Modulo(k, scalesNames.Item2.Length)]);
                    }
        }
    }
}
