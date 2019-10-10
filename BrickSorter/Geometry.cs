using System;
using System.Drawing;

namespace BrickSorter
{
    public static class Geometry
    {
        public static int CalculateDistance(Point start, Point end)
        {
            var dx = start.X - end.X;
            var dy = start.Y - end.Y;
            return (int)Math.Round(Math.Sqrt(dx * dx + dy * dy));
        }

        public static Point GetIntersectionPoint(Line line, Line anotherLine)
        {
            var x1 = line.Point.X;
            var y1 = line.Point.Y;
            var p2 = line.GetPointAtY(-10); // doesn't matter
            var x2 = p2.X;
            var y2 = p2.Y;

            var y3 = anotherLine.Point.Y;
            var x3 = anotherLine.Point.X;
            var p4 = anotherLine.GetPointAtY(-10); // doesn't matter
            var x4 = p4.X;
            var y4 = p4.Y;

            double a = (x1 - x2) * (y3 - y4);
            double b = (y1 - y2) * (x3 - x4);
            var px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / (a - b);
            var py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / (a - b);

            return new Point((int)Math.Round(px), (int)Math.Round(py));
        }
    }
}
