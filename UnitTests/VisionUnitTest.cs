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
    [DeploymentItem(@"TestData\pacman2.png")]
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
            Assert.AreEqual(40, _leftBorder.AngleInDegrees);
            Assert.AreEqual(new Point(34, 0), _rightBorder.Point);
            Assert.AreEqual(40, _rightBorder.AngleInDegrees);
        }

        [TestMethod]
        public void FindTopBorderTest()
        {
            _leftBorder = new Line(new Point(51, 52), 40);
            _rightBorder = new Line(new Point(34, 0), 40);

            FindTopBorder();

            DebugVisualize(new List<Line> { _topBorder });
            Assert.AreEqual(new Point(50, 51), _topBorder.Point);
            Assert.AreEqual(-50, _topBorder.AngleInDegrees);
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
        public void MeasureBrick()
        {
            SetBmp((Bitmap)Image.FromFile("circle.png"));
            FindLeftBorder();
            DebugVisualize(new List<Line> { _leftBorder });
            FindRightBorder();
            DebugVisualize(new List<Line> { _leftBorder, _rightBorder });
            FindAngleForLeftBorder();
            DebugVisualize(new List<Line> { _leftBorder, _rightBorder });
            AdjustRightBorder();
            DebugVisualize(new List<Line> { _leftBorder, _rightBorder });
            FindTopBorder();
            DebugVisualize(new List<Line> { _leftBorder, _rightBorder, _topBorder });
            FindBottomBorder();
            DebugVisualize(new List<Line> { _leftBorder, _rightBorder, _topBorder, _bottomBorder });

            var a = Geometry.GetIntersectionPoint(_leftBorder, _topBorder);
            var b = Geometry.GetIntersectionPoint(_topBorder, _rightBorder);
            var c = Geometry.GetIntersectionPoint(_rightBorder, _bottomBorder);
            var d = Geometry.GetIntersectionPoint(_bottomBorder, _leftBorder);

            var ab = Geometry.CalculateDistance(a, b);
            var bc = Geometry.CalculateDistance(b, c);

            var width = Math.Min(ab, bc);
            var length = Math.Max(ab, bc);

            ImageHelper.Img = _bmp;
            ImageHelper.DrawRectangle(a, b, c, d, Color.Red);
            ImageHelper.DrawText(new Point(10, 10), $"{length}x{width}", Brushes.Red);
            ImageHelper.Img.Save(Path.Combine(Path.GetTempPath(), "VisionUnitTest.png"));
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
