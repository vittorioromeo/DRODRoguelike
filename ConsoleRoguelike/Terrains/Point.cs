#region
using System;

#endregion

namespace DRODRoguelike.Terrains
{
    public class Point
    {
        public Point(int mX, int mY)
        {
            X = mX;
            Y = mY;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public Point LastConnection { get; set; }

        public static int GetDistance(Point a, Point b)
        {
            double distance = ((b.X - a.X) ^ 2) + ((b.Y - a.Y) ^ 2);

            int result = Convert.ToInt32(distance);

            return result;
        }
    }
}