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
            Assert.AreEqual(new Point(51, 52), _leftBorder.Point);
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
            Assert.AreEqual(new Point(76, 82), _bottomBorder.Point);
            Assert.AreEqual(130, _bottomBorder.AngleInDegrees);
        }

        private void DebugVisualize(List<Line> lines)
        {
#if DEBUG
            Img._img = _bmp;
            foreach (var line in lines)
            {
                Img.DrawLine(line, Color.Red);
            }
            Img._img.Save(Path.Combine(Path.GetTempPath(), "VisionUnitTest.png"));
#endif
        }
    }
}
