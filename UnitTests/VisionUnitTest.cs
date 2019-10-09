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

            Assert.AreEqual(new Point(51, 52), LeftBorder.Point);
        }

        [TestMethod]
        public void FindRightBorderTest()
        {
            FindRightBorder();

            Assert.AreEqual(new Point(91, 70), RightBorder.Point);
        }

        [TestMethod]
        public void FindAngle()
        {
            LeftBorder = new Line(new Point(51, 52), 0);
            RightBorder = new Line(new Point(91, 70), 0);

            FindAngleForLeftBorder();
            AdjustRightBorder();

            DebugVisualize(new List<Line> { LeftBorder, RightBorder });
            Assert.AreEqual(new Point(51, 52), LeftBorder.Point);
            Assert.AreEqual(40, LeftBorder.AngleInDegrees);
            Assert.AreEqual(new Point(34, 0), RightBorder.Point);
            Assert.AreEqual(40, RightBorder.AngleInDegrees);
        }

        [TestMethod]
        public void FindTopBorderTest()
        {
            LeftBorder = new Line(new Point(51, 52), 40);
            RightBorder = new Line(new Point(34, 0), 40);

            FindTopBorder();

            DebugVisualize(new List<Line> { TopBorder });
            Assert.AreEqual(new Point(50, 51), TopBorder.Point);
            Assert.AreEqual(-50, TopBorder.AngleInDegrees);
        }

        [TestMethod]
        public void FindBottomBorderTest()
        {
            LeftBorder = new Line(new Point(51, 52), 40);
            RightBorder = new Line(new Point(34, 0), 40);

            FindBottomBorder();

            DebugVisualize(new List<Line> { BottomBorder });
            Assert.AreEqual(new Point(76, 82), BottomBorder.Point);
            Assert.AreEqual(130, BottomBorder.AngleInDegrees);
        }

        private void DebugVisualize(List<Line> lines)
        {
#if DEBUG
            ImageHelper.Img = Bmp;
            foreach (var line in lines)
            {
                ImageHelper.DrawLine(line, Color.Red);
            }
            ImageHelper.Img.Save(Path.Combine(Path.GetTempPath(), "VisionUnitTest.png"));
#endif
        }
    }
}
