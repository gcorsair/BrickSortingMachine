using System;
using System.Drawing;

namespace BrickSorter
{
    public class Line
    {
        public Line(Point point, int angleInDegrees)
        {
            Point = point;
            AngleInDegrees = angleInDegrees;
        }
        public int AngleInDegrees { get; set; }
        public double AngleInRadians
        {
            get => (Math.PI / 180) * AngleInDegrees;
            set => AngleInDegrees = (int)Math.Round(value / (Math.PI / 180));
        }
        public Point Point { get; set; }
        public void MoveRight()
        {
            Point = new Point(Point.X + 1, Point.Y);
        }
        public void MoveLeft()
        {
            Point = new Point(Point.X - 1, Point.Y);
        }
        public Point GetPointAtY(int y)
        {
            var cotangentBeta = 1 / Math.Tan(Math.PI / 2 - AngleInRadians);

            if (double.IsInfinity(cotangentBeta))
                return new Point(Point.X - 100, Point.Y);

            var x = (int)Math.Round(Point.X - (Point.Y - y) * cotangentBeta);
            return new Point(x, y);
        }
    }
}