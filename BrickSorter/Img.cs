using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BrickSorter
{
    public class Img
    {
        private readonly Color _blankColor = Color.FromArgb(255, 255, 255, 255);
        public static Image _img;

        public Bitmap GetBitmapReadyForProcessing(string path, double multiplier = 1)
        {
            LoadFromFile(path);
            CompensateBackground();
            Resize(multiplier);
            ConvertToLowerQuality();
            return ((Bitmap)_img);
        }

        protected void LoadFromFile(string path)
        {
            _img = Image.FromFile(path);
        }

        void Resize(double multiplier)
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

        void CompensateBackground()
        {
            var gamma = 1f;
            var contrast = 1f;
            do
            {
                Adjust(gamma, contrast);
                gamma *= 1.05f;
                contrast *= 1.1f;
            } while (!AllCornersAreWhite());
        }

        void Adjust(float gamma, float contrast)
        {
            var destRect = new Rectangle(0, 0, _img.Width, _img.Height);
            var destImage = new Bitmap(_img.Width, _img.Height);
            destImage.SetResolution(_img.HorizontalResolution, _img.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;

                using (var wrapMode = new ImageAttributes())
                {
                    float[][] ptsArray =
                    {
                        new[] {contrast, 0f, 0f, 0f, 0f}, // scale red
                        new[] {0f, contrast, 0f, 0f, 0f}, // scale green
                        new[] {0f, 0f, contrast, 0f, 0f}, // scale blue
                        new[] {0f, 0f, 0f, 1f, 0f}, // don't scale alpha
                        new[] {0f, 0f, 0f, 0f, 1f}
                    };

                    wrapMode.ClearColorMatrix();
                    wrapMode.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    wrapMode.SetGamma(gamma, ColorAdjustType.Bitmap);
                    graphics.DrawImage(_img, destRect, 0, 0, _img.Width, _img.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            _img = destImage;
        }

        bool AllCornersAreWhite()
        {
            var bmp = ((Bitmap)_img);
            return
                bmp.GetPixel(0, 0) == _blankColor &&
                bmp.GetPixel(_img.Width - 1, 0) == _blankColor &&
                bmp.GetPixel(0, _img.Height - 1) == _blankColor &&
                bmp.GetPixel(_img.Width - 1, _img.Height - 1) == _blankColor;
        }

        void ConvertToLowerQuality()
        {
            var destRect = new Rectangle(0, 0, _img.Width, _img.Height);
            _img = ((Bitmap)_img).Clone(destRect, PixelFormat.Format8bppIndexed);
        }

        public static void DrawLine(Point start, Point end, Color color)
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
                    var myPen = new Pen(color) { Width = 1 };
                    graphics.DrawLine(myPen, start, end);
                }
            }

            _img = destImage;
        }

        public static void DrawLine(Line line, Color color)
        {
            var topX = line.GetXbyY(0);
            DrawLine(line.Point, new Point(topX, 0), color);

            var bottomX = line.GetXbyY(_img.Height);
            DrawLine(line.Point, new Point(bottomX, _img.Height), color);
        }

        public static void DrawText(Point start, string text, Brush brush)
        {
            var destRect = new Rectangle(0, 0, _img.Width, _img.Height);
            var destImage = new Bitmap(_img.Width, _img.Height);
            destImage.SetResolution(_img.HorizontalResolution, _img.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(_img, destRect, 0, 0, _img.Width, _img.Height, GraphicsUnit.Pixel);
                graphics.DrawString(text, new Font("Tahoma", 12), brush, start);
            }

            _img = destImage;
        }
    }
}
