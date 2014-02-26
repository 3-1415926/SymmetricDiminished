using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    public class Progression : List<ChordWithMelody>
    {
        private static readonly char chordSeparator = ':';
        private static readonly char[] noteSeparators = new[] { ' ', ',', '\r', '\n', '\t' };
        private static readonly char[] barSeparators = new[] { '|' };

        public static Progression Parse(string notation)
        {
            var progression = new Progression();
            var chordsWithMelody = notation.Split(barSeparators).Select(cwm => cwm.Trim()).Where(cwm => cwm != "").ToArray();
            foreach (var chordWithMelody in chordsWithMelody)
            {
                int chordSeparatorIndex = chordWithMelody.IndexOf(chordSeparator);
                var chordNotation = (chordSeparatorIndex >= 0 ? chordWithMelody.Substring(0, chordSeparatorIndex) : chordWithMelody).Trim();
                var melodyNotes = (chordSeparatorIndex >= 0 ? chordWithMelody.Substring(chordSeparatorIndex + 1) : "")
                    .Split(noteSeparators, StringSplitOptions.RemoveEmptyEntries).Select(m => Common.ParseNote(m)).ToArray();
                progression.Add(new ChordWithMelody(chordNotation, melodyNotes));
            }
            return progression;
        }
    }
}
