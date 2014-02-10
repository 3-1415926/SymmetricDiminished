using System;
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

        private double preciseZeroFretWidth;

        private Bitmap bitmap;

        private Guitar guitar;

        public GuitarFretboardBitmap(Guitar guitar)
        {
            this.guitar = guitar;
            this.width = this.GetFretboardWidth();
            this.height = this.GetFretboardHeight();
            this.bitmap = new Bitmap(this.width, this.height);
            this.preciseZeroFretWidth = this.GetPreciseZeroFretWidth();
        }

        public void DrawFretboard()
        {
            var preciseZeroFretWidthInt = (int)this.preciseZeroFretWidth;
            var graphics = Graphics.FromImage(this.bitmap);
            graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, this.width, this.height);
            graphics.FillRectangle(new SolidBrush(Color.Beige), preciseZeroFretWidthInt, 0, this.width - preciseZeroFretWidthInt, this.height);
            graphics.DrawRectangle(new Pen(Color.Black, 2), preciseZeroFretWidthInt, 0, this.width - preciseZeroFretWidthInt, this.height);

            var bordersPen = new Pen(Color.Black, 2);
            var stringsPen = new Pen(Color.Gray, StringsWidth);

            for (int i = 0; i < this.guitar.FretCount; i++)
            {
                var fretX = (int)this.GetFretPosition(i);
                graphics.DrawLine(bordersPen, fretX, 0, fretX, this.height);
            }

            for (int i = 0; i < this.guitar.StringsCount; i++)
            {
                int y = this.GetStringY(i);
                graphics.DrawLine(stringsPen, preciseZeroFretWidthInt, y, this.width, y);
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

        private double GetPreciseZeroFretWidth()
        {

            var multiplier = 0.0;
            for (int i = 0; i <= this.guitar.FretCount; i++)
            {
                multiplier += Math.Pow(2, -i / 12.0);
            }

            return this.width / multiplier;
        }

        private int GetFretPosition(int fret)
        {
            if (fret > this.guitar.FretCount)
            {
                throw new ArgumentOutOfRangeException("fret");
            }

            if (fret == this.guitar.FretCount)
            {
                return this.width;
            }

            if (fret == 0)
            {
                return (int)this.preciseZeroFretWidth;
            }

            var multiplier = 0.0;
            for (int i = 0; i <= fret; i++)
            {
                multiplier += Math.Pow(2, -i / 12.0);
            }

            return (int)(multiplier * this.preciseZeroFretWidth);
        }

        private int GetFretMiddleX(int fret)
        {
            if (fret == 0)
            {
                return (int)this.preciseZeroFretWidth / 2;
            }

            return (this.GetFretPosition(fret) + this.GetFretPosition(fret - 1)) / 2;
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
