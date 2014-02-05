using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    class Program
    {
        static void Main(string[] args)
        {
            var progression = new Progression
            { 
                "Gmi7(b5)", "C7(b9)", "F-", 
                { "Dmi7(b5)", "G" }, { "G7", "D#" }, "C6maj7",  
                "Cmi7", "F7(b9)", "Bbmaj7",
                "Ab7", { "G7", "Bb" },
            };
            foreach (var chord in progression)
            {
                var scales = Scales.FindFit(chord.Item1);
                Console.WriteLine(chord.Item2 + (chord.Item3.Length != 0 ? " with " + string.Join(" ", chord.Item3) : ""));
                foreach (var scale in scales)
                    Console.WriteLine("  " + Common.FormatMask(scale.Scale.Mask, (int)chord.Item1.BaseNote, Common.OctaveLength) + " " + scale.Name);
            }
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
