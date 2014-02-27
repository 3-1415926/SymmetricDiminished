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
            //PrintScales(progression);
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

            string noteNames = string.Format("{0,-" + indent + "}  {1}", "", string.Concat(Enumerable.Range(0, 3).SelectMany(_ =>
                Enumerable.Range(0, Common.OctaveLength).GroupJoin(Common.NoteOffset, o => o, i => i.Value, (o, ii) => ii.Any() ? ii.Single().Key : '.'))));
            Console.WriteLine(noteNames);
            var defaultColor = Console.BackgroundColor;
            for (int i = 0; i <= progression.Count; i++)
            {
                Console.Write("{0,-" + indent + "}  ", progression[i % progression.Count].ChordNotation);
                var mask = Common.ChordInAllOctaves(progression[i % progression.Count].Chord.Mask);
                for (int j = 0; j < Common.OctaveLength * 3; j++)
                {
                    Console.BackgroundColor = j % Common.OctaveLength == 0 ? ConsoleColor.DarkGreen : 
                        j % Common.OctaveLength == 5 ? ConsoleColor.DarkBlue : defaultColor;
                    Console.Write(mask % 2 != 0 ? "*" : " ");
                    mask /= 2;
                }
                Console.BackgroundColor = defaultColor;
                Console.WriteLine();
            }
            Console.WriteLine(noteNames);
            Console.WriteLine(Environment.NewLine + Environment.NewLine);
        }
    }
}
