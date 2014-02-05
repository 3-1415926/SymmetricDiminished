using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicScale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestScale
{
    [TestClass]
    public class ScalesTests
    {
        [TestMethod]
        public void AllScalesDifferent()
        {
            for (int i = 0; i < Scales.All.Length - 1; i++)
                for (int j = i + 1; j < Scales.All.Length; j++)
                    if (Scales.All[i] == Scales.All[j])
                        Assert.Fail("Scales equal:" + Environment.NewLine + Scales.All[i] + Environment.NewLine + Scales.All[j]);
        }

        [TestMethod]
        public void NoTripleOneOrZeroScales()
        {
            for (int i = 0; i < Scales.All.Length; i++)
            {
                int ones = 0;
                int zeroes = 0;
                int len = 0;
                ulong mask = Scales.All[i].Mask;
                for (int j = 0; j < Common.MaskLength; j++)
                {
                    if ((mask % 2) == 0)
                    {
                        if (++zeroes > 2)
                            Assert.Fail("Scale contains three consecutive zeroes: " + Scales.All[i]);
                        ones = 0;
                    }
                    else
                    {
                        if (++ones > 2)
                            Assert.Fail("Scale contains three consecutive ones: " + Scales.All[i]);
                        zeroes = 0;
                    }
                    len++;
                    mask /= 2;
                }
            }
        }

        [TestMethod]
        public void FindScaleName()
        {
            AssertContains("Ionian", Scales.FindFit(Chord.FromNotes("C", "D", "E", "F", "G", "A", "B")).Single().Name);
            AssertContains("Dorian", Scales.FindFit(Chord.FromNotes("D", "E", "F", "G", "A", "B", "C")).Single().Name);
            AssertContains("Phrygian", Scales.FindFit(Chord.FromNotes("E", "F", "G", "A", "B", "C", "D")).Single().Name);
            AssertContains("Lydian", Scales.FindFit(Chord.FromNotes("F", "G", "A", "B", "C", "D", "E")).Single().Name);
            AssertContains("Mixolydian", Scales.FindFit(Chord.FromNotes("G", "A", "B", "C", "D", "E", "F")).Single().Name);
            AssertContains("Aeolian", Scales.FindFit(Chord.FromNotes("A", "B", "C", "D", "E", "F", "G")).Single().Name);
            AssertContains("Locrian", Scales.FindFit(Chord.FromNotes("B", "C", "D", "E", "F", "G", "A")).Single().Name);

            AssertContains("(I)", Scales.FindFit(Chord.FromNotes("C", "D", "Eb", "F", "G", "A", "B")).Single().Name);
            AssertContains("(II)", Scales.FindFit(Chord.FromNotes("D", "Eb", "F", "G", "A", "B", "C")).Single().Name);
            AssertContains("(III)", Scales.FindFit(Chord.FromNotes("Eb", "F", "G", "A", "B", "C", "D")).Single().Name);
            AssertContains("(IV)", Scales.FindFit(Chord.FromNotes("F", "G", "A", "B", "C", "D", "Eb")).Single().Name);
            AssertContains("(V)", Scales.FindFit(Chord.FromNotes("G", "A", "B", "C", "D", "Eb", "F")).Single().Name);
            AssertContains("(VI)", Scales.FindFit(Chord.FromNotes("A", "B", "C", "D", "Eb", "F", "G")).Single().Name);
            AssertContains("(VII)", Scales.FindFit(Chord.FromNotes("B", "C", "D", "Eb", "F", "G", "A")).Single().Name);

            AssertContains("Locrian",         Scales.FindFit(Chord.FromNotes("G","Ab","Bb","C","Db","Eb","F")).Single().Name);
            AssertContains("(half-whole)",    Scales.FindFit(Chord.FromNotes("C","Db","Eb","Fb","Gb","G","A","Bb")).Single().Name);
            AssertContains("Aeolian",         Scales.FindFit(Chord.FromNotes("F","G","Ab","Bb","C","Db","Eb")).Single().Name);
            AssertContains("Phrygian",        Scales.FindFit(Chord.FromNotes("F","Gb","Ab","Bb","C","Db","Eb")).Single().Name);
            AssertContains("half-diminished", Scales.FindFit(Chord.FromNotes("D","E","F","G","Ab","Bb","C")).Single().Name);
            AssertContains("Dorian b2",       Scales.FindFit(Chord.FromNotes("C","Db","Eb","F","G","A","Bb")).Single().Name);
            AssertContains("Mixolydian",      Scales.FindFit(Chord.FromNotes("Ab","Bb","C","Db","Eb","F","Gb")).Single().Name);
        }

        private void AssertContains(string substring, string actualString)
        {
            Assert.IsTrue(actualString.Contains(substring), "\"" + actualString + "\" does not contain \"" + substring + "\"");
        }
    }
}
