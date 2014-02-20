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
                { "E-7", "A", "F#" }, { "G-7", "E", "C", "A" },
                { "BbMaj7", "G" }, { "B-7b5", "E", "G" }, { "E7#9", "G", "F" }, 
                { "A-7", "D" }, { "F#-7b5", "B", "D" },
                { "F-7", "Bb", "G", "D", "Eb" }, "C-7", //{ "B7alt", "D" },
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
