import sys
import os
from System.IO import File

from scales import draw_scale, guitar

from MusicScale import Program
from MusicScale import Progression

input_path = sys.argv[1]
output_dir = sys.argv[2]

progression = Progression.Parse(File.ReadAllText(input_path))
scales = Program.GetBestScalePath(progression)

scale_groups = {}

if not os.path.isdir(output_dir):
    os.makedirs(output_dir)

summary_output_path = os.path.join(output_dir, 'summary.txt')
with open(summary_output_path, 'w') as fp:
    max_scale_group = 0
    for chord_with_scale in zip(progression, scales):
        chord, named_scale = chord_with_scale
        if named_scale.Scale in scale_groups:
            scale_group = scale_groups[named_scale.Scale]
        else:
            max_scale_group += 1
            scale_group = scale_groups[named_scale.Scale] = max_scale_group
            scale_png_path = os.path.join(output_dir, '%s.png' % scale_group)
            draw_scale(guitar, named_scale.Scale, scale_png_path)

        line = '%6s\t%s\t%s\n' % (chord.ChordNotation, scale_group, named_scale.ToString())
        fp.write(line)
        sys.stdout.write(line)
