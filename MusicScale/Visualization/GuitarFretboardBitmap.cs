using System.Drawing;
using System.Drawing.Imaging;

namespace MusicScale.Visualization
{
    public class GuitarFretboardBitmap
    {
        private const int FretWidth = 30;

        private const int FretStringHeight = 15;

        private const int FretMargin = 8;

        private const int StringsWidth = 2;

        private const int NoteCircleRadius = 5;

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
            graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, this.width, this.height);
            graphics.FillRectangle(new SolidBrush(Color.Beige), FretWidth, 0, this.width - FretWidth, this.height);
            graphics.DrawRectangle(new Pen(Color.Black, 2), FretWidth, 0, this.width - FretWidth, this.height);

            var bordersPen = new Pen(Color.Black, 2);
            var stringsPen = new Pen(Color.Gray, StringsWidth);

            for (int i = 0; i < this.guitar.FretCount; i++)
            {
                graphics.DrawLine(bordersPen, (i + 1) * FretWidth, 0, (i + 1) * FretWidth, this.height);
            }

            for (int i = 0; i < this.guitar.StringsCount; i++)
            {
                int y = this.GetStringY(i);
                graphics.DrawLine(stringsPen, FretWidth, y, this.width, y);
            }

            var fretHintBrush = new SolidBrush(Color.SandyBrown);
            var centerY = this.height / 2;

            this.DrawNoteCircle(graphics, fretHintBrush, this.GetFretMiddleX(5), centerY, NoteCircleRadius);
            this.DrawNoteCircle(graphics, fretHintBrush, this.GetFretMiddleX(7), centerY, NoteCircleRadius);
            this.DrawNoteCircle(
                graphics,
                fretHintBrush,
                this.GetFretMiddleX(12),
                centerY - FretStringHeight,
                NoteCircleRadius);
            this.DrawNoteCircle(
                graphics,
                fretHintBrush,
                this.GetFretMiddleX(12),
                centerY + FretStringHeight,
                NoteCircleRadius);
        }

        public void DrawScale(Scale scale)
        {
            foreach (var note in scale.GetNotes())
            {
                this.DrawNote(note);
            }
        }

        public void DrawNote(Note note)
        {
            for (int i = 1; i < 7; i++)
            {
                this.DrawNote(new NoteInOctave(note, i));
            }
        }

        public void DrawNote(NoteInOctave note)
        {
            var graphics = Graphics.FromImage(this.bitmap);
            var noteBrush = new SolidBrush(Color.DarkSlateBlue);
            var notePen = new Pen(Color.DarkGray);

            for (int i = 0; i < this.guitar.StringsCount; i++)
            {
                var fret = this.guitar.LocateNoteOnString(note, i);

                if (fret == -1)
                {
                    continue;
                }

                var centerX = this.GetFretMiddleX(fret);
                var centerY = this.GetStringY(i);

                this.DrawNoteCircle(graphics, noteBrush, centerX, centerY, NoteCircleRadius, notePen);
            }
        }

        public void SaveToFile(string path)
        {
            this.bitmap.Save(path, ImageFormat.Png);
        }

        private void DrawNoteCircle(Graphics graphics, Brush brush, int centerX, int centerY, int radius, Pen borderPen = null)
        {
            graphics.FillEllipse(brush, centerX - radius, centerY - radius, 2 * radius, 2 * radius);
            if (borderPen != null)
            {
                graphics.DrawEllipse(borderPen, centerX - radius, centerY - radius, 2 * radius, 2 * radius);
            }
        }

        private int GetFretMiddleX(int fret)
        {
            return (FretWidth * fret) + (FretWidth / 2);
        }

        private int GetStringY(int stringNum)
        {
            return FretMargin + (stringNum * FretStringHeight);
        }

        private int GetFretboardWidth()
        {
            return FretWidth * (guitar.FretCount + 1);
        }

        private int GetFretboardHeight()
        {
            return FretStringHeight * (guitar.StringsCount - 1) + 2 * FretMargin;
        }
    }
}
