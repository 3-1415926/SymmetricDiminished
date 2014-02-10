using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MusicScale.Visualization
{
    using System.Drawing.Imaging;

    public static class KnownGuitarTunings
    {
        public static IList<NoteInOctave> Standard = new List<NoteInOctave> { N.E(4), N.B(3), N.G(3), N.D(3), N.A(2), N.E(2) };
    }

    public class Guitar
    {
        public int StringsCount { get; private set; }

        public int FretCount { get; private set; }

        public IList<NoteInOctave> Tuning { get; private set; }

        public Guitar(int stringsCount = 6, IEnumerable<NoteInOctave> tuning = null, int fretCount = 16)
        {
            this.Tuning = tuning == null ? KnownGuitarTunings.Standard : tuning.ToList();

            this.StringsCount = stringsCount;

            this.FretCount = fretCount;
        }
    }

    public class GuitarFretboardBitmap
    {
        private const int FretWidth = 20;

        private const int FretStringHeight = 10;

        private const int FretMargin = 5;

        private int width;

        private int height;

        private Bitmap bitmap;

        private Guitar guitar;

        public GuitarFretboardBitmap(Guitar guitar)
        {
            this.guitar = guitar;
            this.width = this.GetFretboardWidth();
            this.height = this.GetFretboardHeight();
            this.bitmap = new Bitmap(this.width, this.height);
        }

        public void DrawFret()
        {
            var graphics = Graphics.FromImage(this.bitmap);
            graphics.FillRectangle(new SolidBrush(Color.Transparent), 0, 0, this.width, this.height);
            graphics.FillRectangle(new SolidBrush(Color.Beige), FretWidth, 0, this.width - FretWidth, this.height);
            graphics.DrawRectangle(new Pen(Color.Black, 2), FretWidth, 0, this.width - FretWidth, this.height);
        }

        public void SaveToFile(string path)
        {
            this.bitmap.Save(path, ImageFormat.Png);
        }

        private int GetFretboardWidth()
        {
            return FretWidth * (guitar.FretCount + 1);
        }

        private int GetFretboardHeight()
        {
            return FretStringHeight * guitar.StringsCount + 2 * FretMargin;
        }
    }
}
