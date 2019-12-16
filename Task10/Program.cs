using System;
using System.Collections.Generic;
using System.Linq;

namespace Task10
{
    class Program
    {
        static void Main(string[] args)
        {
            AsteroidMap map = new AsteroidMap();
            map.Parse(Inputs.TestInput2);
            var result = map.CanSeeMost();
        }
    }

    public class AsteroidMap
    {
        public HashSet<Point> asteroids;

        public AsteroidMap()
        {
            asteroids = new HashSet<Point>();
        }

        public bool Add(Point point)
        {
            return asteroids.Add(point);
        }

        public void Parse(string map)
        {
            string[] lines = map.Split(System.Environment.NewLine);
            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y];

                for (var x = 0; x < line.Length; x++)
                {
                    var item = line[x];
                    if (item == '#')
                        asteroids.Add(new Point(x, y));
                }
            }
        }

        public (Point bestPoint, int numberSeen) CanSeeMost()
        {
            int best = -1;
            Point bestPoint = new Point(-1, -1);
            foreach (var point in asteroids)
            {
                var relativeMap = Map(point);
                var seen = relativeMap.NumberSeenFromCenter();
                if (seen > best)
                {
                    best = seen;
                    bestPoint = point;
                }
            }

            return (bestPoint, best);
        }

        public AsteroidMap Map(Point newCenter)
        {
            AsteroidMap back = new AsteroidMap();
            foreach (var point in asteroids)
            {
                back.Add(point.RelativePoint(newCenter));
            }

            return back;
        }

        public int NumberSeenFromCenter()
        {
            HashSet<Point> points = asteroids.ToHashSet();
            points.Remove(new Point(0, 0));

            foreach (var point in asteroids)
            {
                foreach (var otherPoint in asteroids)
                {
                    if (point != otherPoint)
                    {
                        if (point.IsBlocking(otherPoint))
                        {
                            points.Remove(otherPoint);
                        }
                    }
                }
            }

            return points.Count;
        }
    }

    public struct Point : IEquatable<Point>
    {
        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }

        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsBlocking(Point otherRelative)
        {
            if (X == 0 && Y == 0)
                return false;

            if (X == 0)
            {
                return otherRelative.X == 0 && otherRelative.Y / Y > 0 && otherRelative.Y % Y == 0;
            }
            else if (Y == 0)
            {
                return otherRelative.Y == 0 && otherRelative.X / X > 0 && otherRelative.X % X == 0;
            }
            else
            {
                var otherRelativeX = otherRelative.X / X;
                return otherRelativeX > 0 && otherRelativeX == otherRelative.Y / Y && otherRelative.X % X == 0 &&
                       otherRelative.Y % Y == 0;
            }
        }

        public Point RelativePoint(Point origin)
        {
            return new Point(X - origin.X, Y - origin.Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
