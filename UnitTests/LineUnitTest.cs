using System;
using System.Drawing;
using BrickSorter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class LineUnitTest
    {
        [TestMethod]
        public void LineInit()
        {
            var line = new Line(new Point(1, 2), 3);

            Assert.AreEqual(1, line.Point.X);
            Assert.AreEqual(2, line.Point.Y);
            Assert.AreEqual(3, line.AngleInDegrees);
            Assert.AreEqual(0.052d, line.AngleInRadians, 0.001d);
        }

        [TestMethod]
        public void ReturnsCorrectXbyY_0deg()
        {
            const int x = 10;
            var line = new Line(new Point(x, 10), 0);

            Assert.AreEqual(x, line.GetXbyY(-10));
            Assert.AreEqual(x, line.GetXbyY(0));
            Assert.AreEqual(x, line.GetXbyY(10));
            Assert.AreEqual(x, line.GetXbyY(1000));
        }

        [TestMethod]
        public void ReturnsCorrectXbyY_45degNeg()
        {
            var line = new Line(new Point(10, 10), 45);

            Assert.AreEqual(-10, line.GetXbyY(-10));
            Assert.AreEqual(0, line.GetXbyY(0));
            Assert.AreEqual(10, line.GetXbyY(10));
            Assert.AreEqual(1000, line.GetXbyY(1000));
        }

        [TestMethod]
        public void ReturnsCorrectXbyY_90deg()
        {
            var line = new Line(new Point(10, 10), 90);

            Assert.AreEqual(10, line.GetXbyY(10));
        }

        [TestMethod]
        public void Moves()
        {
            var line = new Line(new Point(-1, 10), 45);

            line.MoveRight();
            Assert.AreEqual(0, line.Point.X);
            Assert.AreEqual(10, line.Point.Y);

            line.MoveRight();
            Assert.AreEqual(1, line.Point.X);
            Assert.AreEqual(10, line.Point.Y);
        }
    }
}
