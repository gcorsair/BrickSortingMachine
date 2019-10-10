using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace BrickSorter
{
    public class Vision
    {
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
                    if (_bmp.GetPixel(x, y) == ImageHelper.BlankColor) continue;
                    _leftBorder = new Line(new Point(x, y), 0);
                    return;
                }
        }

        protected void FindRightBorder()
        {
            for (var x = _bmp.Width - 1; x >= 0; x--)
                for (var y = _bmp.Height - 1; y >= 0; y--)
                {
                    if (_bmp.GetPixel(x, y) == ImageHelper.BlankColor) continue;
                    _rightBorder = new Line(new Point(x, y), 0);
                    return;
                }
        }

        protected void FindAngleForLeftBorder()
        {
            var delta = _leftBorder.Point.Y < _rightBorder.Point.Y ? +1 : -1;
            while (true)
            {
                var nonWhitePixels = CountNonWhitePixels(_leftBorder);
                if (nonWhitePixels > 2) // check this
                {
                    return;
                }

                if (nonWhitePixels == 0)
                    _leftBorder.MoveRight();
                else
                    _leftBorder.MoveLeft();

                _leftBorder.AngleInDegrees += delta;
            }
        }

        protected void AdjustRightBorder()
        {
            _rightBorder.AngleInDegrees = _leftBorder.AngleInDegrees;
            _rightBorder.Point = _rightBorder.GetPointAtY(0);
            var nonWhitePixelsCounter = CountNonWhitePixels(_rightBorder);
            while (nonWhitePixelsCounter > 0)
            {
                _rightBorder.MoveRight();
                nonWhitePixelsCounter = CountNonWhitePixels(_rightBorder);
            }
        }

        protected int CountNonWhitePixels(Line line)
        {
            var nonWhitePixelsCounter = 0;
            var sinAlpha = Math.Sin(line.AngleInRadians);
            var cosAlpha = Math.Cos(line.AngleInRadians);

            const double delta = 1.3;
            var c = delta;
            int x;
            int y;
            while (c < Geometry.CalculateDistance(new Point(0, 0), new Point(_bmp.Height, _bmp.Width)))
            {
                x = line.Point.X + (int)Math.Round(c * sinAlpha);
                y = line.Point.Y + (int)Math.Round(c * cosAlpha);

                c += delta;

                if (x < 0 || x >= _bmp.Width || y < 0 || y >= _bmp.Height)
                    continue;

                var pixel = _bmp.GetPixel(x, y);
                if (pixel != ImageHelper.BlankColor)
                    nonWhitePixelsCounter++;
            }

            return nonWhitePixelsCounter;
        }

        protected void FindTopBorder()
        {
            var angle = _leftBorder.AngleInDegrees < 0 ? -90 : +90;
            _topBorder = new Line(new Point(0, 0), _leftBorder.AngleInDegrees + angle);
            // we are moving along left border
            for (var y = 0; y < _leftBorder.Point.Y; y++)
            {
                _topBorder.Point = _leftBorder.GetPointAtY(y);
                if (CountNonWhitePixels(_topBorder) > 0)
                    return;
            }
        }

        protected void FindBottomBorder()
        {
            var angle = _leftBorder.AngleInDegrees < 0 ? -90 : +90;
            _bottomBorder = new Line(new Point(0,0), _leftBorder.AngleInDegrees + angle);
            // we are moving along left border
            for (var y = _bmp.Height - 1; y > _leftBorder.Point.Y; y--)
            {
                _bottomBorder.Point = _leftBorder.GetPointAtY(y);
                if (CountNonWhitePixels(_bottomBorder) > 0)
                    return;
            }
        }
    }
}