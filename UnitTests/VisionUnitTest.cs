using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using BrickSorter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [DeploymentItem(@"TestData\two_processed.png")]
    [DeploymentItem(@"TestData\extra.png")]
    [DeploymentItem(@"TestData\circle.png")]
    [DeploymentItem(@"TestData\pacman.png")]
    [DeploymentItem(@"TestData\two_processed_flip.png")]
    [DeploymentItem(@"TestData\six.png")]
    [TestClass]
    public class VisionUnitTest : Vision
    {
        [TestInitialize]
        public void LoadBitmap()
        {
            SetBmp((Bitmap)Image.FromFile("two_processed.png"));
        }

        [TestMethod]
        public void FindLeftBorderTest()
        {
            FindLeftBorder();

            Assert.AreEqual(new Point(51, 52), _leftBorder.Point);
        }

        [TestMethod]
        public void FindRightBorderTest()
        {
            FindRightBorder();

            Assert.AreEqual(new Point(91, 70), _rightBorder.Point);
        }

        [TestMethod]
        public void FindAngle()
        {
            _leftBorder = new Line(new Point(51, 52), 0);
            _rightBorder = new Line(new Point(91, 70), 0);

            FindAngleForLeftBorder();
            AdjustRightBorder();

            DebugVisualize(new List<Line> { _leftBorder, _rightBorder });
            Assert.AreEqual(new Point(50, 52), _leftBorder.Point);
            Assert.AreEqual(37, _leftBorder.AngleInDegrees);
            Assert.AreEqual(new Point(40, 0), _rightBorder.Point);
            Assert.AreEqual(37, _rightBorder.AngleInDegrees);
        }

        [TestMethod]
        public void FindTopBorderTest()
        {
            _leftBorder = new Line(new Point(51, 52), 40);
            _rightBorder = new Line(new Point(34, 0), 40);

            FindTopBorder();

            DebugVisualize(new List<Line> { _topBorder });
            Assert.AreEqual(new Point(50, 51), _topBorder.Point);
            Assert.AreEqual(130, _topBorder.AngleInDegrees);
        }

        [TestMethod]
        public void FindBottomBorderTest()
        {
            _leftBorder = new Line(new Point(51, 52), 40);
            _rightBorder = new Line(new Point(34, 0), 40);

            FindBottomBorder();

            DebugVisualize(new List<Line> { _bottomBorder });
            Assert.AreEqual(new Point(77, 83), _bottomBorder.Point);
            Assert.AreEqual(130, _bottomBorder.AngleInDegrees);
        }

        [TestMethod]
        public void PixelsCounter()
        {
            SetBmp((Bitmap)Image.FromFile("circle.png"));
            const int delta = 1;
            var line = new Line(new Point(150, 150), 0);
            var min = 300;
            var max = 0;
            while (line.AngleInDegrees <= 90)
            {
                var nonWhitePixels = CountNonWhitePixels(line);

                if (nonWhitePixels > max)
                    max = nonWhitePixels;

                if (nonWhitePixels < min)
                    min = nonWhitePixels;

                line.AngleInDegrees += delta;
            }

            Console.WriteLine($"{min}..{max}");
            Assert.IsTrue((max - min) <= 1);
        }

        [TestMethod]
        public void MeasureBrick()
        {
            Assert.AreEqual("129x106",GetBrickDimensions("pacman.png"));
            Assert.AreEqual("129x128",GetBrickDimensions("circle.png"));
            Assert.AreEqual("126x87",GetBrickDimensions("extra.png"));
            Assert.AreEqual("136x87",GetBrickDimensions("six.png"));
            Assert.AreEqual("41x21",GetBrickDimensions("two_processed_flip.png"));
            Assert.AreEqual("42x24",GetBrickDimensions("two_processed.png"));
        }

        private string GetBrickDimensions(string file)
        {
            SetBmp((Bitmap) Image.FromFile(file));
            FindLeftBorder();
            FindRightBorder();
            FindAngleForLeftBorder();
            AdjustRightBorder();
            FindTopBorder();
            FindBottomBorder();

            var a = Geometry.GetIntersectionPoint(_leftBorder, _topBorder);
            var b = Geometry.GetIntersectionPoint(_topBorder, _rightBorder);
            var c = Geometry.GetIntersectionPoint(_rightBorder, _bottomBorder);
            var d = Geometry.GetIntersectionPoint(_bottomBorder, _leftBorder);

            var ab = Geometry.CalculateDistance(a, b);
            var bc = Geometry.CalculateDistance(b, c);

            var width = Math.Min(ab, bc);
            var length = Math.Max(ab, bc);

            var dimensions = $"{length}x{width}";
#if DEBUG
            ImageHelper.Img = _bmp;
            ImageHelper.DrawRectangle(a, b, c, d, Color.Red);
            ImageHelper.DrawText(new Point(10, 10), dimensions, Brushes.Red);
            ImageHelper.Img.Save(Path.Combine(Path.GetTempPath(), "VisionUnitTest.png"));
#endif
            return dimensions;
        }

        private void DebugVisualize(List<Line> lines)
        {
#if DEBUG
            ImageHelper.Img = _bmp;
            foreach (var line in lines)
            {
                ImageHelper.DrawLine(line, Color.Red);
            }
            ImageHelper.Img.Save(Path.Combine(Path.GetTempPath(), "VisionUnitTest.png"));
#endif
        }
    }
}
