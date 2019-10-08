using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BrickSorter
{
    public class Pic
    {
        private readonly Color _whiteColor = Color.FromArgb(255, 255, 255, 255);

        protected Image _img;
        //private int MagicConstant = 22 * 44 / 2; //pixels per bump - has to be determined during calibration
        protected Point _leftCorner;
        protected Point _rightCorner;
        protected Point _topCorner;
        protected Point _bottomCorner;
        protected int _angle;
        protected int _length;
        protected int _width;
        protected Direction _rotatingDirection;
        protected int _calValue;

        protected void LoadFromFile(string path)
        {
            _img = Image.FromFile(path);
        }

        protected void ResizeImage(double multiplier)
        {
            var width = (int)Math.Round(_img.Width * multiplier);
            var height = (int)Math.Round(_img.Height * multiplier);
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(_img.HorizontalResolution, _img.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;

                using (var wrapMode = new ImageAttributes())
                {
                    graphics.DrawImage(_img, destRect, 0, 0, _img.Width, _img.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            _img = destImage;
        }

        protected void Adjust()
        {
            var gamma = 1f;
            var contrast = 1f;
            do
            {
                SetGamma(gamma, contrast);
                gamma *= 1.05f;
                contrast *= 1.1f;
            } while (!AreAllCornersWhite());
        }

        protected void SetGamma(float gamma, float contrast)
        {
            var destRect = new Rectangle(0, 0, _img.Width, _img.Height);
            var destImage = new Bitmap(_img.Width, _img.Height);
            destImage.SetResolution(_img.HorizontalResolution, _img.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;

                using (var wrapMode = new ImageAttributes())
                {
                    float[][] ptsArray ={
                        new float[] {contrast, 0, 0, 0, 0}, // scale red
                        new float[] {0, contrast, 0, 0, 0}, // scale green
                        new float[] {0, 0, contrast, 0, 0}, // scale blue
                        new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                        new float[] {0, 0, 0, 0, 1}};

                    wrapMode.ClearColorMatrix();
                    wrapMode.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    wrapMode.SetGamma(gamma, ColorAdjustType.Bitmap);
                    graphics.DrawImage(_img, destRect, 0, 0, _img.Width, _img.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            _img = destImage;
        }

        protected bool AreAllCornersWhite()
        {
            var bmp = ((Bitmap)_img);
            return
                bmp.GetPixel(0, 0) == _whiteColor &&
                bmp.GetPixel(_img.Width - 1, 0) == _whiteColor &&
                bmp.GetPixel(0, _img.Height - 1) == _whiteColor &&
                bmp.GetPixel(_img.Width - 1, _img.Height - 1) == _whiteColor;
        }

        protected void Convert()
        {
            var destRect = new Rectangle(0, 0, _img.Width, _img.Height);
            _img = ((Bitmap)_img).Clone(destRect, PixelFormat.Format8bppIndexed);
        }

        protected void Save(string path)
        {
            _img.Save(path);
        }

        protected void FindLeftCorner()
        {
            var bmp = ((Bitmap)_img);
            for (var x = 0; x < bmp.Width; x++)
                for (var y = 0; y < bmp.Height; y++)
                {
                    if (bmp.GetPixel(x, y) != _whiteColor)
                    {
                        _leftCorner = new Point(x, y);
                        return;
                    }
                }
        }

        protected void FindRightCorner()
        {
            var bmp = ((Bitmap)_img);
            for (var x = bmp.Width - 1; x >= 0; x--)
                for (var y = bmp.Height - 1; y >= 0; y--)
                {
                    if (bmp.GetPixel(x, y) != _whiteColor)
                    {
                        _rightCorner = new Point(x, y);
                        return;
                    }
                }
        }

        protected void FindTopCorner()
        {
            var alphaInRadians = (Math.PI / 180) * _angle;
            var cotanBeta = 1 / Math.Tan(Math.PI / 2 - alphaInRadians);
            for (var topY = 0; topY < _leftCorner.Y; topY++)
            {
                var topX = _leftCorner.X - (_leftCorner.Y - topY) * cotanBeta;
                _topCorner = new Point((int)topX, topY);
                if (CountNonWhitePixels(_topCorner, Direction.CW, _angle + 90) > 0)
                    return;
            }
        }

        protected void FindBottomCorner()
        {
            var alphaInRadians = (Math.PI / 180) * _angle;
            var cotanBeta = 1 / Math.Tan(Math.PI / 2 - alphaInRadians);
            for (var y = 1; y < (_img.Height - _leftCorner.Y); y++)
            {
                var bottomY = (_img.Height - y);
                var bottomX = _leftCorner.X + (bottomY - _leftCorner.Y) * cotanBeta;
                _bottomCorner = new Point((int)bottomX, bottomY);
                if (CountNonWhitePixels(_bottomCorner, Direction.CW, _angle + 90) > 0)
                    return;
            }
        }

        protected void FindAngle()
        {
            SetRotationDirection();
            _angle = 0;
            var delta = _rotatingDirection == Direction.CW ? +1 : -1;
            var nonWhitePixelsBefore = 0;
            while (true)
            {
                var nonWhitePixels = CountNonWhitePixels(_leftCorner);
                Console.WriteLine($"Angle {_angle}\t NonWhitePixels {nonWhitePixels}");
                if (nonWhitePixels > 10 && nonWhitePixels < nonWhitePixelsBefore) // check this
                {
                    _angle -= delta; // step back
                    return;
                }
                nonWhitePixelsBefore = nonWhitePixels;
                _angle += delta;
            }
        }

        protected int CountNonWhitePixels(Point startPoint)
        {
            return CountNonWhitePixels(startPoint, _rotatingDirection, _angle);
        }

        protected int CountNonWhitePixels(Point startPoint, Direction rotatingDirection, int angle)
        {
            var nonWhitePixelsCounter = 0;
            var director = rotatingDirection == Direction.CW ? +1 : -1;
            var alphaInRadians = (Math.PI / 180) * angle;
            var sinAlpha = Math.Sin(alphaInRadians);
            var cosAlpha = Math.Cos(alphaInRadians);

            const double delta = 1.41;
            var c = delta;
            int x;
            int y;
            while (true)
            {
                x = startPoint.X + (int)Math.Round(c * sinAlpha);
                y = startPoint.Y + (int)Math.Round(c * cosAlpha) * director;

                if (x >= _img.Width || y < 0 || y >= _img.Height)
                    break;

                var pixel = ((Bitmap)_img).GetPixel(x, y);
                //if (pixel != _whiteColor)
                if (!(pixel == _whiteColor || pixel == Color.FromArgb(255, 255, 0, 0)))
                    nonWhitePixelsCounter++;

                c += delta;
            }
            //DrawLine(startPoint, new Point(x, y), Color.Red);
            return nonWhitePixelsCounter;
        }

        protected void DrawLine(Point start, Point end, Color color)
        {
            var destRect = new Rectangle(0, 0, _img.Width, _img.Height);
            var destImage = new Bitmap(_img.Width, _img.Height);
            destImage.SetResolution(_img.HorizontalResolution, _img.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;

                using (var wrapMode = new ImageAttributes())
                {
                    graphics.DrawImage(_img, destRect, 0, 0, _img.Width, _img.Height, GraphicsUnit.Pixel, wrapMode);
                    Pen myPen = new Pen(color) { Width = 1 };
                    graphics.DrawLine(myPen, start, end);
                }
            }

            _img = destImage;
        }

        protected void DrawText(Point start, Color color, string text)
        {
            var destRect = new Rectangle(0, 0, _img.Width, _img.Height);
            var destImage = new Bitmap(_img.Width, _img.Height);
            destImage.SetResolution(_img.HorizontalResolution, _img.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;

                using (var wrapMode = new ImageAttributes())
                {
                    graphics.DrawImage(_img, destRect, 0, 0, _img.Width, _img.Height, GraphicsUnit.Pixel, wrapMode);
                    var font = new Font(FontFamily.GenericMonospace, 10f);
                    graphics.DrawString(text, font, Brushes.Red, (float)start.X, (float)start.Y);
                }
            }

            _img = destImage;
        }

        protected void DrawLine(Point point, Color color)
        {
            var alphaInRadians = (Math.PI / 180) * _angle;
            var cotanBeta = 1 / Math.Tan(Math.PI / 2 - alphaInRadians);

            var topX = point.X - point.Y * cotanBeta;
            DrawLine(point, new Point((int)topX, 0), color);

            var bottomX = point.X + (_img.Height - point.Y) * cotanBeta;
            DrawLine(point, new Point((int)bottomX, _img.Height), color);
        }

        protected void AdjustRightCorner()
        {
            var nonWhitePixelsCounter = CountNonWhitePixels(_rightCorner);
            while (nonWhitePixelsCounter > 0)
            {
                _rightCorner.X += 1;
                nonWhitePixelsCounter = CountNonWhitePixels(_rightCorner);
            }
        }

        protected void CalculateLength()
        {
            var dx = _topCorner.X - _bottomCorner.X;
            var dy = _topCorner.Y - _bottomCorner.Y;
            _length = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy));
        }

        protected void CalculateWidth()
        {
            var dx = _topCorner.X - _rightCorner.X;
            var dy = _topCorner.Y - _rightCorner.Y;
            var c = Math.Sqrt(dx * dx + dy * dy);

            _width = (int)Math.Round(Math.Sqrt(c * c - _length * _length));
        }

        protected void SetRotationDirection()
        {
            _rotatingDirection = _leftCorner.Y < _rightCorner.Y ? Direction.CW : Direction.CCW;
        }

        protected void SetCalibration(int calValue)
        {
            _calValue = calValue;
        }

        protected string GetBrickSize(string imgPath)
        {
            LoadFromFile(imgPath);
            Convert();
            Adjust();
            FindLeftCorner();
            FindRightCorner();
            FindAngle();
            AdjustRightCorner();
            FindTopCorner();
            FindBottomCorner();
            CalculateLength();
            CalculateWidth();

            var shortSide = Math.Round((Math.Min(_width, _length) / (double)_calValue));
            var longSide = Math.Round((Math.Max(_width, _length) / (double)_calValue));
            return $"{shortSide}x{longSide}";
        }

        protected enum Direction
        {
            CCW,
            CW
        }
    }
}
