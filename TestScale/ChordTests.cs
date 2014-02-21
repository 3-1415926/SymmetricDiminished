using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicScale;
using System.Diagnostics;

namespace TestScale
{
    [TestClass]
    public class ChordTests
    {
        private List<string> errors = new List<string>();

        [TestInitialize]
        public void TestInitialize()
        {
            errors.Clear();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (!errors.Any())
                return;

            Assert.Fail(Environment.NewLine + Environment.NewLine + string.Concat(errors));
        }

        private void VerifyChord(string notation, string expectedNotes)
        {
            try
            {
                var actual = new Chord(notation);
                var expected = Chord.FromNotes(expectedNotes.Split('-'));
                if (expected != actual)
                {
                    string error = notation + " !~ " + Environment.NewLine + "  " + expectedNotes + Environment.NewLine
                        + expected + " (exp)" + Environment.NewLine
                        + actual + " (act)" + Environment.NewLine + Environment.NewLine;
                    errors.Add(error);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.ToString() + Environment.NewLine + Environment.NewLine);
            }
        }

        private void VerifyChords(string[] notations, string expectedNotes)
        {
            foreach (var notation in notations)
                VerifyChord(notation, expectedNotes);
        }

        private void VerifyThrows<T>(string notation) where T : Exception
        {
            try
            {
                new Chord(notation);
                errors.Add(notation + " should have thrown a " + typeof(T).Name);
            }
            catch (T) { }
        }

        [TestMethod]
        public void BadChords()
        {
            VerifyThrows<ArgumentException>("Ah+-54738");
            VerifyThrows<ArgumentException>("F/m11");
            VerifyThrows<ArgumentException>("G-m7");
        }

        [TestMethod]
        public void ArbitraryChords()
        {
            VerifyChord("Gm7", "G-Bb-D-F");
            VerifyChord("D+add-9", "D-F#-A#-Eb");
            VerifyChord("F#augM711b13", "F#-A#-C##-E#-G#-B-D");
            VerifyChord("B0add#13", "B-D-F-A-G##");
            VerifyChord("Gbsus2b6j7+11", "Gb-Ab-Db-Ebb-F-Ab-C");
            VerifyChord("Cbb-+11b5", "Cbb-Ebbb-Gbbb-Bbbb-Dbb-Fb");
            VerifyChord("B#dimb9add-13", "B#-D#-F#-A-C#-G#");
            VerifyChord("Fadd13", "F-A-C--D");
            VerifyChord("A9sus4/G", "G-A-D-E-G-B");
            VerifyChord("D0(nat7)/A", "A-D-F-Ab-C#");
            VerifyChord("C/C", "C-E-G");
            VerifyChord("Cbm9b5/C", "C-Cb-Ebb-Gbb-Bbb-Db");
            VerifyChord("F9sus2add13/F#", "F#-F-G-C-Eb-G-D");
            VerifyChord("Bsus9", "B-E-F#-A-C#");
            VerifyChord("G7sus", "G-C-D-F");
            VerifyChord("E5", "E-B");
        }

        [TestMethod]
        public void ClassChords()
        {
            VerifyChord("Gmi7(b5)", "G-Bb-Db-F");
            VerifyChord("C7(b9)", "C-E-G-Bb-Db");
            VerifyChord("F-", "F-Ab-C");
            VerifyChord("Dmi7(b5)", "D-F-Ab-C");
            VerifyChord("G7", "G-B-D-F");
            VerifyChord("C", "C-E-G");
            VerifyChord("Cmi7", "C-Eb-G-Bb");
            VerifyChord("F7(b9)", "F-A-C-Eb-Gb");
            VerifyChord("Bb", "Bb-D-F");
            VerifyChord("Ab7", "Ab-C-Eb-Gb");
        }

        [TestMethod]
        public void WikiChords()
        {
            VerifyChords(new[] { "C", "CM", "Cmaj", "CΔ" }, "C-E-G");
            VerifyChords(new[] { "Cm", "Cmin", "C−" }, "C-E♭-G");
            VerifyChords(new[] { "C+", "Caug", "CM♯5", "CM+5" }, "C-E-G♯");
            VerifyChords(new[] { "Co", "Cdim", "Cm♭5", "Cm°5" }, "C-E♭-G♭");
            VerifyChords(new[] { "C6", "CM6", "Cmaj6" }, "C-E-G-A");
            VerifyChords(new[] { "Cm6", "Cmin6" }, "C-E♭-G-A");
            VerifyChords(new[] { "C7", "Cdom7" }, "C-E-G-B♭");
            VerifyChords(new[] { "CM7", "Cmaj7", "Cj7", "CΔ7" }, "C-E-G-B");
            VerifyChords(new[] { "Cm7", "Cmin7", "C-7" }, "C-E♭-G-B♭");
            VerifyChords(new[] { "C+7", "Caug7", "C7♯5", "C7+5" }, "C-E-G♯-B♭");
            VerifyChords(new[] { "Co7", "Cdim7", "C°7" }, "C-E♭-G♭-B♭♭");
            VerifyChords(new[] { "Cø", "Cø7", "CØ", "Cmin7dim5", "Cm7♭5", "CØ7", "Cm7°5", "C−7°5", "C−7♭5" }, "C-E♭-G♭-B♭");
            VerifyChords(new[] { "CmM7", "Cm/M7", "Cm(M7)", "Cminmaj7", "Cmin/maj7", "Cmin(maj7)", "C−M7", "C−Δ7", "Cm♯7" }, "C-E♭-G-B");
            VerifyChords(new[] { "C+M7", "Caugmaj7", "CM7♯5", "CM7+5" }, "C-E-G♯-B");
            VerifyChords(new[] { "C7♭5", "Cdom7dim5" }, "C-E-G♭-B♭");
            VerifyChords(new[] { "CM9", "Cmaj9", "CΔ9" }, "C-E-G-B-D");
            VerifyChords(new[] { "C9", "Cdom9" }, "C-E-G-B♭-D");
            VerifyChords(new[] { "CmM9", "Cminmaj9", "C−M9" }, "C-E♭-G-B-D");
            VerifyChords(new[] { "Cm9", "Cmin9", "C−9" }, "C-E♭-G-B♭-D");
            VerifyChords(new[] { "C+M9", "Caugmaj9" }, "C-E-G♯-B-D");
            VerifyChords(new[] { "C+9", "Caug9", "C9♯5" }, "C-E-G♯-B♭-D");
            VerifyChords(new[] { "CØ9" }, "C-E♭-G♭-B♭-D");
            VerifyChords(new[] { "CØ♭9" }, "C-E♭-G♭-B♭-D♭");
            VerifyChords(new[] { "C°9", "Cdim9" }, "C-E♭-G♭-B♭♭-D");
            VerifyChords(new[] { "C°♭9", "Cdim♭9" }, "C-E♭-G♭-B♭♭-D♭");
            VerifyChords(new[] { "C9♭5" }, "C-E-G♭-B♭-D");
            VerifyChords(new[] { "C9♯11" }, "C-E-G-B♭-D-F♯");
            VerifyChords(new[] { "C11", "Cdom11" }, "C-E-G-B♭-D-F");
            VerifyChords(new[] { "CM11", "Cmaj11" }, "C-E-G-B-D-F");
            VerifyChords(new[] { "CmM11", "Cminmaj11", "C−M11" }, "C-E♭-G-B-D-F");
            VerifyChords(new[] { "Cm11", "Cmin11", "C−11" }, "C-E♭-G-B♭-D-F");
            VerifyChords(new[] { "C+M11", "Caugmaj11" }, "C-E-G♯-B-D-F");
            VerifyChords(new[] { "C+11", "Caug11", "C11♯5" }, "C-E-G♯-B♭-D-F");
            //VerifyChords(new[] { "CØ11" }, "C-E♭-G♭-B♭-D♭-F");
            //VerifyChords(new[] { "C°11", "Cdim11" }, "C-E♭-G♭-B♭♭-D♭-F♭");
            VerifyChords(new[] { "CM13", "Cmaj13", "CΔ13" }, "C-E-G-B-D-F-A");
            VerifyChords(new[] { "C13", "Cdom13" }, "C-E-G-B♭-D-F-A");
            VerifyChords(new[] { "CmM13", "Cminmaj13", "C−M13" }, "C-E♭-G-B-D-F-A");
            VerifyChords(new[] { "Cm13", "Cmin13", "C−13" }, "C-E♭-G-B♭-D-F-A");
            VerifyChords(new[] { "C+M13", "Caugmaj13" }, "C-E-G♯-B-D-F-A");
            VerifyChords(new[] { "C+13", "Caug13", "C13♯5" }, "C-E-G♯-B♭-D-F-A");
            VerifyChords(new[] { "CØ13" }, "C-E♭-G♭-B♭-D-F-A");
        }
    }
}
