using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace BrickSorter
{
    public class Vision
    {
        protected Bitmap Bmp;
        protected Line LeftBorder;
        protected Line RightBorder;
        protected Line TopBorder;
        protected Line BottomBorder;

        private readonly Color _blankColor = Color.FromArgb(255, 255, 255, 255);

        protected void SetBmp(Bitmap bmp)
        {
            Bmp = bmp;
        }

        protected void FindLeftBorder()
        {
            for (var x = 0; x < Bmp.Width; x++)
                for (var y = 0; y < Bmp.Height; y++)
                {
                    if (Bmp.GetPixel(x, y) == _blankColor) continue;
                    LeftBorder = new Line(new Point(x, y), 0);
                    return;
                }
        }

        protected void FindRightBorder()
        {
            for (var x = Bmp.Width - 1; x >= 0; x--)
                for (var y = Bmp.Height - 1; y >= 0; y--)
                {
                    if (Bmp.GetPixel(x, y) == _blankColor) continue;
                    RightBorder = new Line(new Point(x, y), 0);
                    return;
                }
        }

        protected void FindAngleForLeftBorder()
        {
            var direction = LeftBorder.Point.Y < RightBorder.Point.Y ? Directions.BottomRight : Directions.TopRight;
            var delta = LeftBorder.Point.Y < RightBorder.Point.Y ? +1 : -1;
            var nonWhitePixelsBefore = 0;
            while (true)
            {
                var nonWhitePixels = CountNonWhitePixels(LeftBorder, direction);
                if (nonWhitePixels > 10 && nonWhitePixels < nonWhitePixelsBefore) // check this
                {
                    LeftBorder.AngleInDegrees -= delta; // step back
                    return;
                }
                nonWhitePixelsBefore = nonWhitePixels;
                LeftBorder.AngleInDegrees += delta;
            }
        }

        protected void AdjustRightBorder()
        {
            RightBorder.AngleInDegrees = LeftBorder.AngleInDegrees;
            var direction = RightBorder.AngleInDegrees > 0 ? Directions.BottomRight : Directions.BottomLeft;
            RightBorder.Point = new Point(RightBorder.GetXbyY(0), 0);
            var nonWhitePixelsCounter = CountNonWhitePixels(RightBorder, direction);
            while (nonWhitePixelsCounter > 0)
            {
                RightBorder.MoveRight();
                nonWhitePixelsCounter = CountNonWhitePixels(RightBorder, direction);
            }
        }

        protected int CountNonWhitePixels(Line line, Directions.Direction direction)
        {
            var nonWhitePixelsCounter = 0;
            var sinAlpha = Math.Sin(line.AngleInRadians);
            var cosAlpha = Math.Cos(line.AngleInRadians);

            const double delta = 1.41;
            var c = delta;
            while (true)
            {
                var x = line.Point.X + (int)Math.Round(c * sinAlpha) * direction.dx;
                var y = line.Point.Y + (int)Math.Round(c * cosAlpha) * direction.dy;

                if (x < 0 || x >= Bmp.Width || y < 0 || y >= Bmp.Height)
                    break;

                var pixel = Bmp.GetPixel(x, y);
                if (pixel != _blankColor)
                    nonWhitePixelsCounter++;

                c += delta;
            }

            return nonWhitePixelsCounter;
        }

        protected void FindTopBorder()
        {
            var direction = LeftBorder.AngleInDegrees > 0 ? Directions.TopRight : Directions.BottomRight;
            // we are moving along left border
            for (var y = 0; y < LeftBorder.Point.Y; y++)
            {
                var x = LeftBorder.GetXbyY(y);
                TopBorder = new Line(new Point(x, y), LeftBorder.AngleInDegrees - 90);
                if (CountNonWhitePixels(TopBorder, direction) > 0)
                    return;
            }
        }

        protected void FindBottomBorder()
        {
            var direction = LeftBorder.AngleInDegrees > 0 ? Directions.TopRight : Directions.BottomRight;
            // we are moving along left border
            for (var y = Bmp.Height - 1; y > LeftBorder.Point.Y; y--)
            {
                var x = LeftBorder.GetXbyY(y);
                BottomBorder = new Line(new Point(x, y), LeftBorder.AngleInDegrees + 90);
                if (CountNonWhitePixels(BottomBorder, direction) > 0)
                    return;
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static class Directions
        {
            public static Direction TopRight;
            public static Direction TopLeft;
            public static Direction BottomRight;
            public static Direction BottomLeft;

            public class Direction
            {
                public int dx { get; }
                public int dy { get; }

                public Direction(int dx, int dy)
                {
                    this.dx = dx;
                    this.dy = dy;
                }
            }

            static Directions()
            {
                TopRight = new Direction(+1, -1);
                TopLeft = new Direction(-1, -1);
                BottomRight = new Direction(+1, +1);
                BottomLeft = new Direction(-1, +1);
            }
        }
    }
}