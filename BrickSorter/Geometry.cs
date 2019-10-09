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
            var y1 = line.Point.Y;
            var x1 = line.Point.X;
            var y2 = -10; // doesn't matter
            var x2 = line.GetXbyY(y2);

            var y3 = anotherLine.Point.Y;
            var x3 = anotherLine.Point.X;
            var y4 = -10; // doesn't matter
            var x4 = anotherLine.GetXbyY(y4);

            double a = (x1 - x2) * (y3 - y4);
            double b = (y1 - y2) * (x3 - x4);
            var px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / (a - b);
            var py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / (a - b);

            return new Point((int)Math.Round(px), (int)Math.Round(py));
        }
    }
}
