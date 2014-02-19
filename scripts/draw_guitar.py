import clr

clr.AddReferenceToFileAndPath(r'MusicScale\bin\Debug\MusicScale.exe')

import MusicScale
from MusicScale import Visualization

guitar = Visualization.Guitar()

gb = Visualization.GuitarFretboardBitmap(guitar)
gb.DrawFret()

gb.DrawScale(MusicScale.Scales.HarmonicMajor[0])

gb.SaveToFile("guitar.png")
