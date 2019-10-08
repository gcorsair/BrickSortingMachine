using System;
using System.Drawing;

namespace BrickSorter
{
    public class Vision
    {
        private readonly Color _blankColor = Color.FromArgb(255, 255, 255, 255);
        private readonly Direction _topRight = new Direction(+1, -1);
        private readonly Direction _topLeft = new Direction(-1, -1);
        private readonly Direction _bottomRight = new Direction(+1, +1);
        private readonly Direction _bottomLeft = new Direction(-1, +1);
        protected Bitmap _bmp;
        protected Line _leftBorder;
        protected Line _rightBorder;
        protected Line _topBorder;
        protected Line _bottomBorder;

        protected int _length;
        protected int _width;
        protected int _calValue;

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
            var direction = _leftBorder.Point.Y < _rightBorder.Point.Y ? _bottomRight : _topRight;
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
            var direction = _rightBorder.AngleInDegrees > 0 ? _bottomRight : _bottomLeft;
            _rightBorder.Point = new Point(_rightBorder.GetXbyY(0), 0);
            var nonWhitePixelsCounter = CountNonWhitePixels(_rightBorder, direction);
            while (nonWhitePixelsCounter > 0)
            {
                _rightBorder.MoveRight();
                nonWhitePixelsCounter = CountNonWhitePixels(_rightBorder, direction);
            }
        }

        protected int CountNonWhitePixels(Line line, Direction direction)
        {
            var nonWhitePixelsCounter = 0;
            var sinAlpha = Math.Sin(line.AngleInRadians);
            var cosAlpha = Math.Cos(line.AngleInRadians);

            const double delta = 1.41;
            var c = delta;
            while (true)
            {
                var x = line.Point.X + (int)Math.Round(c * sinAlpha) * direction.Dx;
                var y = line.Point.Y + (int)Math.Round(c * cosAlpha) * direction.Dy;

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
            var direction = _leftBorder.AngleInDegrees > 0 ? _topRight : _bottomRight;
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
            var direction = _leftBorder.AngleInDegrees > 0 ? _topRight : _bottomRight;
            // we are moving along left border
            for (var y = _bmp.Height - 1; y > _leftBorder.Point.Y; y--)
            {
                var x = _leftBorder.GetXbyY(y);
                _bottomBorder = new Line(new Point(x, y), _leftBorder.AngleInDegrees + 90);
                if (CountNonWhitePixels(_bottomBorder, direction) > 0)
                    return;
            }
        }



        //protected void CalculateLength()
        //{
        //    var dx = _topBorder.X - _bottomBorder.X;
        //    var dy = _topBorder.Y - _bottomBorder.Y;
        //    _length = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy));
        //}

        //protected void CalculateWidth()
        //{
        //    var dx = _topBorder.X - _rightBorder.X;
        //    var dy = _topBorder.Y - _rightBorder.Y;
        //    var c = Math.Sqrt(dx * dx + dy * dy);

        //    _width = (int)Math.Round(Math.Sqrt(c * c - _length * _length));
        //}

        protected void SetCalibration(int calValue)
        {
            _calValue = calValue;
        }

        //protected string GetBrickSize(string imgPath)
        //{
        //    LoadFromFile(imgPath);
        //    ConvertToLowerQuality();
        //    CompensateBackground();
        //    FindLeftBorder();
        //    FindRightCorner();
        //    FindAngleForLeftBorder();
        //    AdjustRightBorder();
        //    FindTopBorder();
        //    FindBottomBorder();
        //    CalculateLength();
        //    CalculateWidth();

        //    var shortSide = Math.Round((Math.Min(_width, _length) / (double)_calValue));
        //    var longSide = Math.Round((Math.Max(_width, _length) / (double)_calValue));
        //    return $"{shortSide}x{longSide}";
        //}

        public class Direction
        {
            public int Dx { get; }
            public int Dy { get; }

            public Direction(int dx, int dy)
            {
                Dx = dx;
                Dy = dy;
            }
        }
    }
}