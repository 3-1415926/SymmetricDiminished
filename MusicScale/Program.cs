using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    public class Program
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
        }

        public static IEnumerable<NamedScale> GetBestScalePath(Progression progression)
        {
            var scalesProgression = Scales.FindFit(progression, false).ToList();
            var bestScalePaths = GetPathIdMapping(Scales.FindBestScalePaths(scalesProgression));
            
            return scalesProgression.Select((scales, i) => 
                    scales.Select((scale, j) => {
                        var key = Tuple.Create(i, j);
                        List<int> pathIds;
                        var rank = 0;
                        if (bestScalePaths.TryGetValue(key, out pathIds))
                        {
                        rank = pathIds.Count;
                        }

                        return Tuple.Create(rank, scale);
                        }).OrderByDescending(x => x.Item1).Select(x => x.Item2).First()
                    );
        }

        private static void PrintScales(Progression progression)
        {
            var scalesProgression = Scales.FindFit(progression, false).ToList();
            var bestScalePaths = GetPathIdMapping(Scales.FindBestScalePaths(scalesProgression));
            var progressionWithScales = progression.Zip(scalesProgression, (c, scales) => new { ChordWithMelody = c, Scales = scales });

            Console.OutputEncoding = System.Text.Encoding.Unicode;

            int i = 0;
            foreach (var chordWithMelodyAndScales in progressionWithScales)
            {
                var chordWithMelody = chordWithMelodyAndScales.ChordWithMelody;

                Console.WriteLine("{0} ({1}){2}",
                    chordWithMelody.ChordNotation,
                    chordWithMelody.Chord.Quality,
                    (chordWithMelody.MelodyNotes.Length != 0 ? " with " + string.Join(" ", chordWithMelody.MelodyNotes) : ""));

                int scaleIndex = 0;
                foreach (var scale in chordWithMelodyAndScales.Scales)
                {
                    Action printPathId = () => {};
                    List<int> pathIds = null;
                    bestScalePaths.TryGetValue(Tuple.Create(i, scaleIndex), out pathIds);

                    Console.Write("  {0} ", Common.FormatMask(scale.Scale.Mask, 0, Common.OctaveLength));

                    // Another way to print the scale is to start from the root (harder to see the difference between scales):
                    ////Console.Write("  {0} ", Common.FormatMask(scale.Scale.Mask, (int)progression[i].Chord.Root, Common.OctaveLength));

                    PrintPathIds(pathIds);
                    Console.WriteLine(" {0}   {1}",
                        scale.Info,
                        scale.FitReason);
                    scaleIndex++;
                }
                i++;
            }
            Console.WriteLine(Environment.NewLine + Environment.NewLine);
        }

        private static void PrintPathIds(List<int> pathIds)
        {
            var maxWidth = PathIdToConsoleColorMapping.Count;
            var pathIdsCount = pathIds == null ? 0 : pathIds.Count;

            for (int i = 0; i < maxWidth; i++)
            {
                if (i < pathIdsCount)
                {
                    var pathId = pathIds[i];
                    ConsoleColor color;
                    if (!PathIdToConsoleColorMapping.TryGetValue(pathId, out color))
                    {
                        Console.Write(' ');
                        continue;
                    }

                    Console.ForegroundColor = color;
                    Console.Write('★');
                    Console.ResetColor();

                }
                else
                {
                    Console.Write(' ');
                }
            }
        }

        private static readonly Dictionary<int, ConsoleColor> PathIdToConsoleColorMapping = new Dictionary<int, ConsoleColor>
            {
                {0, ConsoleColor.Green},
                {1, ConsoleColor.Yellow},
                {2, ConsoleColor.Blue},
                {3, ConsoleColor.Cyan},
                {4, ConsoleColor.Magenta},
                {5, ConsoleColor.Gray},
            };

        private static IDictionary<Tuple<int, int>, List<int>> GetPathIdMapping(IEnumerable<IEnumerable<int>> bestPaths)
        {
            var result = new Dictionary<Tuple<int, int>, List<int>>();
            int pathId = 0;
            foreach (var path in bestPaths)
            {
                int i = 0;
                foreach (var scaleIndex in path)
                {
                    var key = Tuple.Create(i, scaleIndex);
                    List<int> pathIds = null;
                    if (!result.TryGetValue(key, out pathIds))
                    {
                        pathIds = new List<int>();
                        result[key] = pathIds;
                    }

                    pathIds.Add(pathId);
                    i++;
                }

                pathId++;
            }

            return result;
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
            for (int i = 0; i <= progression.Count; i++)
            {
                Console.Write("{0,-" + indent + "}  ", progression[i % progression.Count].ChordNotation);
                var mask = Common.ChordInAllOctaves(progression[i % progression.Count].Chord.Mask);
                for (int j = 0; j < Common.OctaveLength * 3; j++)
                {
                    Console.Write(mask % 2 != 0 ? "*" : " ");
                    mask /= 2;
                }
                Console.WriteLine();
            }
            Console.WriteLine(noteNames);
            Console.WriteLine(Environment.NewLine + Environment.NewLine);
        }
    }
}
