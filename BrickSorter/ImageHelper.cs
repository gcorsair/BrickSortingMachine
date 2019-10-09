using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BrickSorter
{
    public class ImageHelper
    {
        public static Image Img { get; set; }
        public static readonly Color BlankColor = Color.FromArgb(255, 255, 255, 255);

        public static Bitmap GetBitmapReadyForProcessing(string path, double multiplier = 1)
        {
            Img = Image.FromFile(path);
            CompensateBackground();
            Resize(multiplier);
            ConvertToLowerQuality();
            return ((Bitmap)Img);
        }

        static void Resize(double multiplier)
        {
            var width = (int)Math.Round(Img.Width * multiplier);
            var height = (int)Math.Round(Img.Height * multiplier);
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(Img.HorizontalResolution, Img.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;

                using (var wrapMode = new ImageAttributes())
                {
                    graphics.DrawImage(Img, destRect, 0, 0, Img.Width, Img.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            Img = destImage;
        }

        static void CompensateBackground()
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

        static void Adjust(float gamma, float contrast)
        {
            var destRect = new Rectangle(0, 0, Img.Width, Img.Height);
            var destImage = new Bitmap(Img.Width, Img.Height);
            destImage.SetResolution(Img.HorizontalResolution, Img.VerticalResolution);

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
                    graphics.DrawImage(Img, destRect, 0, 0, Img.Width, Img.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            Img = destImage;
        }

        static bool AllCornersAreWhite()
        {
            var bmp = ((Bitmap)Img);
            return
                bmp.GetPixel(0, 0) == BlankColor &&
                bmp.GetPixel(Img.Width - 1, 0) == BlankColor &&
                bmp.GetPixel(0, Img.Height - 1) == BlankColor &&
                bmp.GetPixel(Img.Width - 1, Img.Height - 1) == BlankColor;
        }

        static void ConvertToLowerQuality()
        {
            var destRect = new Rectangle(0, 0, Img.Width, Img.Height);
            Img = ((Bitmap)Img).Clone(destRect, PixelFormat.Format8bppIndexed);
        }

        public static void DrawLine(Point start, Point end, Color color)
        {
            var destRect = new Rectangle(0, 0, Img.Width, Img.Height);
            var destImage = new Bitmap(Img.Width, Img.Height);
            destImage.SetResolution(Img.HorizontalResolution, Img.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;

                using (var wrapMode = new ImageAttributes())
                {
                    graphics.DrawImage(Img, destRect, 0, 0, Img.Width, Img.Height, GraphicsUnit.Pixel, wrapMode);
                    var myPen = new Pen(color) { Width = 1 };
                    graphics.DrawLine(myPen, start, end);
                }
            }

            Img = destImage;
        }

        public static void DrawLine(Line line, Color color)
        {
            var topX = line.GetXbyY(0);
            DrawLine(line.Point, new Point(topX, 0), color);

            var bottomX = line.GetXbyY(Img.Height);
            DrawLine(line.Point, new Point(bottomX, Img.Height), color);
        }

        public static void DrawText(Point start, string text, Brush brush)
        {
            var destRect = new Rectangle(0, 0, Img.Width, Img.Height);
            var destImage = new Bitmap(Img.Width, Img.Height);
            destImage.SetResolution(Img.HorizontalResolution, Img.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(Img, destRect, 0, 0, Img.Width, Img.Height, GraphicsUnit.Pixel);
                graphics.DrawString(text, new Font("Tahoma", 12), brush, start);
            }

            Img = destImage;
        }
    }
}
