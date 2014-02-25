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
            for (int i = 0; i < progression.Count; i++)
            {
                var scales = Scales.FindFit(progression, i, true);
                Console.WriteLine(progression[i].Chord + (progression[i].MelodyNotes.Length != 0 ? " with " + string.Join(" ", progression[i].MelodyNotes) : ""));
                foreach (var scale in scales)
                    Console.WriteLine("  {0}   {1,-54}   {2}", 
                        Common.FormatMask(scale.Scale.Mask, (int)progression[i].Chord.Root, Common.OctaveLength), 
                        scale.Name,
                        scale.FitReason);
            }
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
