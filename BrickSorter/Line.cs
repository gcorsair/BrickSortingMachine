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
        public int GetXbyY(int y)
        {
            var cotangentBeta = 1 / Math.Tan(Math.PI / 2 - AngleInRadians);
            if (double.IsNegativeInfinity(cotangentBeta)) // over 9000 :D
                cotangentBeta = -9001;
            if (double.IsPositiveInfinity(cotangentBeta)) // over 9000 :D
                cotangentBeta = 9001;
            return (int)Math.Round(Point.X - (Point.Y - y) * cotangentBeta);
        }
    }
}