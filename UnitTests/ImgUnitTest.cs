using System.Drawing;
using BrickSorter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [DeploymentItem(@"TestData\six.png")]
    [DeploymentItem(@"TestData\six_LineAndText.png")]
    [DeploymentItem(@"TestData\six_Lines.png")]
    [DeploymentItem(@"TestData\two.png")]
    [DeploymentItem(@"TestData\two_processed.png")]
    [TestClass]
    public class ImgUnitTest : Img
    {
        [TestMethod]
        public void GetBitmapReadyForProcessing()
        {
            GetBitmapReadyForProcessing("two.png", 0.5d);

            Assert.IsTrue(BitmapComparer.CompareMemCmp((Bitmap)Image.FromFile("two_processed.png"), (Bitmap)_img));
        }

        [TestMethod]
        public void DrawLineAndText()
        {
            LoadFromFile("six.png");

            var start = new Point(0,0);
            var end = new Point(50,100);
            DrawLine(start, end, Color.Red);
            DrawText(end, "(50,100)", Brushes.Black);

            Assert.IsTrue(BitmapComparer.CompareMemCmp((Bitmap)Image.FromFile("six_LineAndText.png"), (Bitmap)_img));
        }

        [TestMethod]
        public void DrawLines()
        {
            LoadFromFile("six.png");

            var line0 = new Line(new Point(10,10), 0);
            DrawLine(line0, Color.Red);
            DrawText(line0.Point, $"{line0.AngleInDegrees}°", Brushes.Red);

            var line10 = new Line(new Point(30, 10), 10);
            DrawLine(line10, Color.Red);
            DrawText(line10.Point, $"{line10.AngleInDegrees}°", Brushes.Red);

            var line45 = new Line(new Point(10, 50), 45);
            DrawLine(line45, Color.Blue);
            DrawText(line45.Point, $"{line45.AngleInDegrees}°", Brushes.Blue);

            var line85 = new Line(new Point(100, 100), 85);
            DrawLine(line85, Color.Chocolate);
            DrawText(line85.Point, $"{line85.AngleInDegrees}°", Brushes.Chocolate);

            var line90 = new Line(new Point(150, 100), 90);
            DrawLine(line90, Color.Black);
            DrawText(line90.Point, $"{line90.AngleInDegrees}°", Brushes.Black);

            var line60neg = new Line(new Point(200, 200), -60);
            DrawLine(line60neg, Color.Green);
            DrawText(line60neg.Point, $"{line60neg.AngleInDegrees}°", Brushes.Green);

            Assert.IsTrue(BitmapComparer.CompareMemCmp((Bitmap)Image.FromFile("six_Lines.png"), (Bitmap)_img));
        }
    }
}
