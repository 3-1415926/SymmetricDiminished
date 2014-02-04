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
                { "Dmi7(b5)", "G" }, { "G7", "D#" }, "Cmaj7", { "D7", "Bb" },  
                "Gmi7(b5)", { "C7(b9)", "A" }, "F-", 
                { "Dmi7(b5)", "G" }, { "G7", "D#" }, "Cmaj7", "C-",  
                "Cmi7", "F7(b9)", "Bbmaj7",
                { "Ab7", "Bb" }, { "D-7", "G" }, { "G7", "Bb" },
                "Gmi7(b5)", "C7(b9)", "F-", 
                { "Db7", "G" }, { "G7", "Eb" }, "C6", { "D7", "Bb" },
            };
            foreach (var chord in progression)
            {
                var scales = Scales.FindFit(chord.Item1);
                Console.WriteLine(chord.Item2 + (chord.Item3.Length != 0 ? " with " + string.Join(" ", chord.Item3) : ""));
                foreach (var scale in scales)
                    Console.WriteLine("  " + scale);
            }
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
