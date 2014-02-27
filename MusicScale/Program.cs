using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: MusicScale.exe <notation-file.txt>");
                Console.WriteLine("    Notation file format:");
                Console.WriteLine("        <chord>: <notes> | <chord>: notes | ...");
                Console.WriteLine("    e.g.:");
                Console.WriteLine("        Cm7: F G Bb D | Dmaj7: B C# | ...");
                return;
            }

            var progression = Progression.Parse(File.ReadAllText(args[0]));
            PrintScales(progression);
            PrintPatterns(progression);

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static void PrintScales(Progression progression)
        {
            for (int i = 0; i < progression.Count; i++)
            {
                var scales = Scales.FindFit(progression, i, false);
                Console.WriteLine(progression[i].ChordNotation + (progression[i].MelodyNotes.Length != 0 ? " with " + string.Join(" ", progression[i].MelodyNotes) : ""));
                foreach (var scale in scales)
                    Console.WriteLine("  {0}   {1}   {2}",
                        Common.FormatMask(scale.Scale.Mask, (int)progression[i].Chord.Root, Common.OctaveLength),
                        scale.Info,
                        scale.FitReason);
            }
            Console.WriteLine(Environment.NewLine + Environment.NewLine);
        }

        private static void PrintPatterns(Progression progression)
        {
            int minShift = 0;
            int indent = progression[0].ChordNotation.Length;
            var shifts = new int[progression.Count + 1];
            for (int i = 1; i <= progression.Count; i++)
            {
                int bassDiff = progression[i % progression.Count].Chord.Bass 
                    - progression[(i - 1) % progression.Count].Chord.Bass;
                if (bassDiff > Common.OctaveLength / 2)
                    shifts[i] = shifts[i - 1] - 1;
                else if (bassDiff < -Common.OctaveLength / 2)
                    shifts[i] = shifts[i - 1] + 1;
                minShift = Math.Min(minShift, shifts[i]);
                indent = Math.Max(indent, progression[i % progression.Count].ChordNotation.Length);
            }

            for (int i = 0; i <= progression.Count; i++)
            {
                Console.WriteLine("{0,-" + indent + "}  {1}", progression[i % progression.Count].ChordNotation, 
                    Common.FormatMask(Common.ChordInAllOctaves(progression[i % progression.Count].Chord.Mask), length: Common.OctaveLength * 3, noteChar: '*', gapChar: ' ', octaveSplit: null, midSplit: null));
            }
            Console.WriteLine(Environment.NewLine + Environment.NewLine);
        }
    }
}
