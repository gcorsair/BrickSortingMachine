using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace BrickSorter
{
    public class Vision
    {
        private readonly Color _blankColor = Color.FromArgb(255, 255, 255, 255);
        protected Bitmap _bmp;
        protected Line _leftBorder;
        protected Line _rightBorder;
        protected Line _topBorder;
        protected Line _bottomBorder;

        protected void SetBmp(Bitmap bmp)
        {
            _bmp = bmp;
        }

        protected void FindLeftBorder()
        {
            for (var x = 0; x < _bmp.Width; x++)
                for (var y = 0; y < _bmp.Height; y++)
                {
                    if (_bmp.GetPixel(x, y) == _blankColor) continue;
                    _leftBorder = new Line(new Point(x, y), 0);
                    return;
                }
        }

        protected void FindRightBorder()
        {
            for (var x = _bmp.Width - 1; x >= 0; x--)
                for (var y = _bmp.Height - 1; y >= 0; y--)
                {
                    if (_bmp.GetPixel(x, y) == _blankColor) continue;
                    _rightBorder = new Line(new Point(x, y), 0);
                    return;
                }
        }

        protected void FindAngleForLeftBorder()
        {
            var direction = _leftBorder.Point.Y < _rightBorder.Point.Y ? Directions.BottomRight : Directions.TopRight;
            var delta = _leftBorder.Point.Y < _rightBorder.Point.Y ? +1 : -1;
            var nonWhitePixelsBefore = 0;
            while (true)
            {
                var nonWhitePixels = CountNonWhitePixels(_leftBorder, direction);
                if (nonWhitePixels > 10 && nonWhitePixels < nonWhitePixelsBefore) // check this
                {
                    _leftBorder.AngleInDegrees -= delta; // step back
                    return;
                }
                nonWhitePixelsBefore = nonWhitePixels;
                _leftBorder.AngleInDegrees += delta;
            }
        }

        protected void AdjustRightBorder()
        {
            _rightBorder.AngleInDegrees = _leftBorder.AngleInDegrees;
            var direction = _rightBorder.AngleInDegrees > 0 ? Directions.BottomRight : Directions.BottomLeft;
            _rightBorder.Point = new Point(_rightBorder.GetXbyY(0), 0);
            var nonWhitePixelsCounter = CountNonWhitePixels(_rightBorder, direction);
            while (nonWhitePixelsCounter > 0)
            {
                _rightBorder.MoveRight();
                nonWhitePixelsCounter = CountNonWhitePixels(_rightBorder, direction);
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

                if (x < 0 || x >= _bmp.Width || y < 0 || y >= _bmp.Height)
                    break;

                var pixel = _bmp.GetPixel(x, y);
                if (pixel != _blankColor)
                    nonWhitePixelsCounter++;

                c += delta;
            }

            return nonWhitePixelsCounter;
        }

        protected void FindTopBorder()
        {
            var direction = _leftBorder.AngleInDegrees > 0 ? Directions.TopRight : Directions.BottomRight;
            // we are moving along left border
            for (var y = 0; y < _leftBorder.Point.Y; y++)
            {
                var x = _leftBorder.GetXbyY(y);
                _topBorder = new Line(new Point(x, y), _leftBorder.AngleInDegrees - 90);
                if (CountNonWhitePixels(_topBorder, direction) > 0)
                    return;
            }
        }

        protected void FindBottomBorder()
        {
            var direction = _leftBorder.AngleInDegrees > 0 ? Directions.TopRight : Directions.BottomRight;
            // we are moving along left border
            for (var y = _bmp.Height - 1; y > _leftBorder.Point.Y; y--)
            {
                var x = _leftBorder.GetXbyY(y);
                _bottomBorder = new Line(new Point(x, y), _leftBorder.AngleInDegrees + 90);
                if (CountNonWhitePixels(_bottomBorder, direction) > 0)
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