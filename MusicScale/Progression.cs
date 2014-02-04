using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicScale
{
    class Progression : List<Tuple<Chord, string, string[]>>
    {
        public void Add(string chordNotation, params string[] extraNotes)
        {
            Add(Tuple.Create(new Chord(chordNotation, extraNotes), chordNotation, extraNotes));
        }
    }
}
