using System.Drawing;
using BrickSorter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GeometryUnitTest
    {
        [TestMethod]
        public void Distance0()
        {
            var start = new Point(1, 1); var end = new Point(1, 1);
            Assert.AreEqual(0, Geometry.CalculateDistance(start, end));
        }

        [TestMethod]
        public void Distance1()
        {
            var start = new Point(1, 0); var end = new Point(0, 0);
            Assert.AreEqual(1, Geometry.CalculateDistance(start, end));
        }

        [TestMethod]
        public void DistanceSqrt2()
        {
            var start = new Point(0, 0); var end = new Point(1, 1);
            Assert.AreEqual(1, Geometry.CalculateDistance(start, end));
        }

        [TestMethod]
        public void DistanceSqrt200()
        {
            var start = new Point(0, 0); var end = new Point(100, 100);
            Assert.AreEqual(141, Geometry.CalculateDistance(start, end));
        }

        [TestMethod]
        public void IntersectionParallel()
        {
            var a = new Line(new Point(0, 0), 0);
            var b = new Line(new Point(0, 0), 0);
            Assert.AreEqual(new Point(int.MinValue, int.MinValue), Geometry.GetIntersectionPoint(a, b));
        }

        [TestMethod]
        public void Intersection90()
        {
            var a = new Line(new Point(0, 0), 90);
            var b = new Line(new Point(0, 0), 0);
            Assert.AreEqual(new Point(0, 0), Geometry.GetIntersectionPoint(a, b));
        }

        [TestMethod]
        public void Intersection90Again()
        {
            var a = new Line(new Point(0, 1), 90);
            var b = new Line(new Point(1, 0), 0);
            Assert.AreEqual(new Point(1, 1), Geometry.GetIntersectionPoint(a, b));
        }

        [TestMethod]
        public void Intersection45()
        {
            var a = new Line(new Point(3, 3), 45);
            var b = new Line(new Point(5, -5), -45);
            Assert.AreEqual(new Point(0, 0), Geometry.GetIntersectionPoint(a, b));
        }
    }
}
