import os
import clr

current_path = os.path.dirname(os.path.abspath(__file__))
project_root_path = os.path.join(current_path, '..')
clr.AddReferenceToFileAndPath(
    os.path.join(project_root_path, 'MusicScale', 'bin', 'Debug', 'MusicScale.exe'))

import MusicScale
from MusicScale import Visualization

Note = MusicScale.Note
N = MusicScale.N

def get_scale(notes, root=None):
    original_root = notes[0]
    scale = MusicScale.Scale.FromNotes(notes)
    if root is not None:
        semitones = int(root) - int(original_root)
        if semitones < 0:
            semitones += 12
        scale = scale.Shift(semitones)
    return scale


def draw_scale(guitar, scale, path):
    gb = Visualization.GuitarFretboardBitmap(guitar)
    gb.DrawFretboard()

    if isinstance(scale, MusicScale.Scale):
        gb.DrawScale(scale)
    elif isinstance(scale, (list, tuple)):
        for note in scale:
            gb.DrawNote(note)

    gb.SaveToFile(path)


ukulele = Visualization.Guitar(
    stringsCount=4,
    tuning=[N.A(4), N.E(3), N.C(3), N.G(4)]
)


guitar = Visualization.Guitar()
