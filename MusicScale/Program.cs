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
            //// Lesson 03
            //var progression = new Progression
            //{ 
            //    { "E-7", "A", "F#" }, { "G-7", "E", "C", "A" },
            //    { "BbMaj7", "G" }, { "B-7b5", "E", "G" }, { "E7#9", "G", "F" }, 
            //    { "A-7", "D" }, { "F#-7b5", "B", "D" },
            //    { "F-7", "Bb", "G", "D", "Eb" }, "C-7", "B7#5#9",
            //};

            // Lesson 04
            var progression = new Progression
            {
                { "Am", "B", "G" }, "Am(nat7)", { "Am7", "B" }, "Em7",
                { "F", "B" }, { "F#o", "G" }, { "C/G", "A" }, { "G7sus4", "E", "F" }, { "C", "A" },
                { "D/F#" }, { "Fma7", "D", "B" }, { "C/E", "B", "A" }, "Eb", "A7",
                { "Ab", "G", "F" }, { "Fm7", "G" }, { "Bb7sus4", "G", "C" /*, "E" */ }, "Cma7",
                { "Bm7", "E" }, { "Bb7", "E" }, { "Am", "D" }, { "Fm/Ab" }, { "G", "A" }, { "D7/F#" }, "Fma7",
                "Cma7/G", { "G7sus4", "E" }, "C", { "Bm7", "E" }, "Bb7",
            };

            foreach (var chord in progression)
            {
                var scales = Scales.FindFit(chord.Item1);
                Console.WriteLine(chord.Item2 + (chord.Item3.Length != 0 ? " with " + string.Join(" ", chord.Item3) : ""));
                foreach (var scale in scales)
                    Console.WriteLine("  " + Common.FormatMask(scale.Scale.Mask, (int)chord.Item1.Root, Common.OctaveLength) + " " + scale.Name);
            }
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
