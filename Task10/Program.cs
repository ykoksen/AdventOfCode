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
            map.Parse(Inputs.Task10aInput);
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
                var seen = SeenFromPoint(point);
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

        public int SeenFromPoint(Point newCenter)
        {
            Dictionary<Vector, Point> newMap = new Dictionary<Vector, Point>();
            foreach (var point in asteroids)
            {
                if (point == newCenter)
                    continue;

                var relPoint = point.RelativePoint(newCenter);
                if (newMap.TryGetValue(relPoint.Vector, out Point existing))
                {
                    if (existing.Multiplier > relPoint.Multiplier)
                    {
                        newMap[relPoint.Vector] = relPoint;
                    }
                }
                else
                {
                    newMap.Add(relPoint.Vector, relPoint);
                }
            }

            return newMap.Count;
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

        public Vector Vector { get; }

        public int Multiplier { get; }

        public int X => Vector.X * Multiplier;

        public int Y => Vector.Y * Multiplier;

        public Point(Vector vector, int multiplier)
        {
            if (multiplier < 0)
            {
                Vector = new Vector(vector.X*-1, vector.Y*-1);
                Multiplier = multiplier * -1;
            }
            else
            {
                Vector = vector;
                Multiplier = multiplier;
            }
        }

        public Point(int x, int y)
        {
            this = CreatePoint(x, y);
        }

        public Point RelativePoint(Point origin)
        {
            return new Point(X - origin.X, Y - origin.Y);
        }

        public static Point CreatePoint(int x, int y)
        {
            switch ((x, y))
            {
                case (0, 0):
                    return new Point(new Vector(0, 0), 0);
                case (0, _):
                    return new Point(new Vector(0, 1), y);
                case (_, 0):
                    return new Point(new Vector(1, 0), x);
                case (_, _):
                    int gcd = MyMath.GCD(x, y);
                    return new Point(new Vector(x / gcd, y / gcd), gcd);
            }
        }

        public override string ToString()
        {
            return $"Vector: {Vector} - Multiplier: {Multiplier}";
        }
    }

    public struct Vector : IEquatable<Vector>
    {
        public bool Equals(Vector other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Vector left, Vector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector left, Vector right)
        {
            return !left.Equals(right);
        }

        public int X { get; }
        public int Y { get; }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
