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

        public static Point FindIntersection(Line a, Line b)
        {
            return new Point();
        }
    }
}
