using System;
using System.Collections.Generic;
using System.Linq;

namespace lab1;

public class Point
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
    public Point() { }

    public override bool Equals(object obj)
    {
        if (obj is Point other)
            return X == other.X && Y == other.Y;
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

public class Geometry
{
    // Алгоритм Джарвиса 
    public static List<Point> JarvisAlgorithm(Point[] points)
    {
        if (points.Length < 3)
            return new List<Point>(points);

        // Находим самую правую точку
        int rightmost = 0;
        for (int i = 1; i < points.Length; i++)
        {
            if (points[i].X < points[rightmost].X ||
                (points[i].X == points[rightmost].X && points[i].Y < points[rightmost].Y))
            {
                rightmost = i;
            }
        }

        List<Point> hull = new List<Point>();
        int current = rightmost;
        int next;

        do
        {
            hull.Add(points[current]);
            next = (current + 1) % points.Length;

            for (int i = 0; i < points.Length; i++)
            {
                // Используем векторное произведение для определения поворота
                int cross = CrossProduct(points[current], points[i], points[next]);
                if (cross > 0 || (cross == 0 && Distance(points[current], points[i]) > Distance(points[current], points[next])))
                {
                    next = i;
                }
            }

            current = next;
        } while (current != rightmost);

        return hull;
    }

    // Вспомогательные методы для геометрических вычислений
    private static int CrossProduct(Point a, Point b, Point c)
    {
        return (int)((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X));
    }

    private static double Distance(Point a, Point b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    // 1. Пересечение двух прямых
    public static Point? LineLineIntersection(Point p1, Point p2, Point p3, Point p4)
    {
        // Уравнения прямых: A1x + B1y = C1 и A2x + B2y = C2
        double A1 = p2.Y - p1.Y;
        double B1 = p1.X - p2.X;
        double C1 = A1 * p1.X + B1 * p1.Y;

        double A2 = p4.Y - p3.Y;
        double B2 = p3.X - p4.X;
        double C2 = A2 * p3.X + B2 * p3.Y;

        double determinant = A1 * B2 - A2 * B1;

        if (determinant == 0)
            return null; // Прямые параллельны или совпадают

        double x = (B2 * C1 - B1 * C2) / determinant;
        double y = (A1 * C2 - A2 * C1) / determinant;

        return new Point((int)Math.Round(x), (int)Math.Round(y));
    }

    // 2. Пересечение прямой и отрезка
    public static Point? LineSegmentIntersection(Point p1, Point p2, Point p3, Point p4)
    {
        double D = -(p2.X-p1.X)*(p4.Y-p3.Y) + (p2.Y-p1.Y)*(p4.X-p3.X);
        if (D == 0)
            return null;
        double t = (
            (p3.Y - p1.Y) * (-1) * (p4.Y - p3.Y) + (p4.X - p3.X) * (p3.X - p1.X)
        ) / D;
        if (t>=0 && t<=1 )
        {
            return new Point(p1.X + t * (p2.X - p1.X), p1.Y + t * (p2.Y - p1.Y));
        }

        return null;
    }

    // 3. Пересечение двух отрезков
    public static Point? SegmentSegmentIntersection(Point p1, Point p2, Point p3, Point p4)
    {
        
        double D = -(p2.X-p1.X)*(p4.Y-p3.Y) + (p2.Y-p1.Y)*(p4.X-p3.X);
        if (D == 0)
            return null;
        double X = (p3.X - p1.X) * (-1) * (p4.Y - p3.Y) + (p4.X - p3.X) * (p3.Y - p1.Y);
        double Y = (p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y-p1.Y);
        
        
        return new Point(X / D, Y / D);
    }


     

    // 4. Пересечение прямой и окружности
    public static List<Point> LineCircleIntersection(Point lineP1, Point lineP2, Point center, double radius)
    {
        List<Point> intersections = new List<Point>();

        // Перенос начала координат в центр окружности
        Point A = new Point(lineP1.X - center.X, lineP1.Y - center.Y);
        Point B = new Point(lineP2.X - center.X, lineP2.Y - center.Y);

        double dx = B.X - A.X;
        double dy = B.Y - A.Y;

        // Параметры квадратного уравнения
        double a = dx * dx + dy * dy;
        double b = 2 * (A.X * dx + A.Y * dy);
        double c = A.X * A.X + A.Y * A.Y - radius * radius;

        double discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
            return intersections; // Нет пересечений

        if (Math.Abs(discriminant) < 1e-9)
        {
            // Касание
            double t = -b / (2 * a);
            int x = (int)Math.Round(A.X + t * dx + center.X);
            int y = (int)Math.Round(A.Y + t * dy + center.Y);
            intersections.Add(new Point(x, y));
        }
        else
        {
            // Два пересечения
            double sqrtDisc = Math.Sqrt(discriminant);
            double t1 = (-b + sqrtDisc) / (2 * a);
            double t2 = (-b - sqrtDisc) / (2 * a);

            int x1 = (int)Math.Round(A.X + t1 * dx + center.X);
            int y1 = (int)Math.Round(A.Y + t1 * dy + center.Y);
            intersections.Add(new Point(x1, y1));

            int x2 = (int)Math.Round(A.X + t2 * dx + center.X);
            int y2 = (int)Math.Round(A.Y + t2 * dy + center.Y);
            intersections.Add(new Point(x2, y2));
        }

        return intersections;
    }

    // 5. Пересечение отрезка и окружности
    public static List<Point> SegmentCircleIntersection(Point segP1, Point segP2, Point center, double radius)
    {
        // Находим все пересечения прямой и окружности
        var lineIntersections = LineCircleIntersection(segP1, segP2, center, radius);
        List<Point> result = new List<Point>();

        // Проверяем, лежат ли точки пересечения на отрезке
        double minX = Math.Min(segP1.X, segP2.X);
        double maxX = Math.Max(segP1.X, segP2.X);
        double minY = Math.Min(segP1.Y, segP2.Y);
        double maxY = Math.Max(segP1.Y, segP2.Y);

        foreach (var point in lineIntersections)
        {
            if (point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY)
            {
                result.Add(point);
            }
        }

        return result;
    }

    // 6. Пересечение двух окружностей
    public static List<Point> CircleCircleIntersection(Point center1, double radius1, Point center2, double radius2)
    {
        List<Point> intersections = new List<Point>();

        // Расстояние между центрами
        double dx = center2.X - center1.X;
        double dy = center2.Y - center1.Y;
        double d = Math.Sqrt(dx * dx + dy * dy);

        // Проверяем условия пересечения
        if (d > radius1 + radius2 || d < Math.Abs(radius1 - radius2))
            return intersections; // Нет пересечений

        if (Math.Abs(d) < 1e-9 && Math.Abs(radius1 - radius2) < 1e-9)
            return intersections; // Бесконечное число точек (совпадающие окружности)

        // Расстояние от центра первой окружности до линии пересечения
        double a = (radius1 * radius1 - radius2 * radius2 + d * d) / (2 * d);

        // Высота (расстояние от линии центров до линии пересечения)
        double h = Math.Sqrt(radius1 * radius1 - a * a);

        // Точка P2
        double x2 = center1.X + (dx * a) / d;
        double y2 = center1.Y + (dy * a) / d;

        if (Math.Abs(h) < 1e-9)
        {
            // Касание
            intersections.Add(new Point((int)Math.Round(x2), (int)Math.Round(y2)));
        }
        else
        {
            // Две точки пересечения
            double x3 = x2 + (dy * h) / d;
            double y3 = y2 - (dx * h) / d;
            intersections.Add(new Point((int)Math.Round(x3), (int)Math.Round(y3)));

            double x4 = x2 - (dy * h) / d;
            double y4 = y2 + (dx * h) / d;
            intersections.Add(new Point((int)Math.Round(x4), (int)Math.Round(y4)));
        }

        return intersections;
    }

    // Вспомогательные методы
    
}

public class Program
{
    internal static void Main(string[] args)
    {
        Console.WriteLine("=== Тестирование выпуклой оболочки ===");
        Point[] points = {
            new Point(0,0), new Point(0,3), new Point(1,1), 
            new Point(2,2), new Point(4,4), new Point(1,2), 
            new Point(3,1), new Point(3,3)
        };

        

        Console.WriteLine("\nАлгоритм Джарвиса :");
        var hull2 = Geometry.JarvisAlgorithm(points);
        foreach (var p in hull2)
        {
            Console.WriteLine($"({p.X}, {p.Y})");
        }

        Console.WriteLine("\n=== Тестирование геометрических операций ===");

        // Тест пересечения двух прямых
        Console.WriteLine("\n1. Пересечение двух прямых:");
        Point p1 = new Point(0, 0);
        Point p2 = new Point(1, 1);
        Point p3 = new Point(0, 4);
        Point p4 = new Point(4, 0);
        var lineIntersection = Geometry.LineLineIntersection(p1, p2, p3, p4);
        Console.WriteLine($"Прямые (0,0)-(1,1) и (0,4)-(4,0) пересекаются в:\n {lineIntersection}");

        // Тест пересечения прямой и отрезка
        Console.WriteLine("\n2. Пересечение прямой и отрезка:");
        Point segP1 = new Point(0, 0);
        Point segP2 = new Point(4, 4);
        Point segP3 = new Point(1, 0);
        Point segP4 = new Point(1, 5);
        var lineSegIntersection = Geometry.LineSegmentIntersection(segP1, segP2, segP3, segP4);
        Console.WriteLine($"Прямая (0,0)-(4,4) и отрезок (1,0)-(1,5) пересекаются в:\n {lineSegIntersection}");

        // Тест пересечения двух отрезков
        Console.WriteLine("\n3. Пересечение двух отрезков:");
        var segIntersection = Geometry.SegmentSegmentIntersection(segP1, segP2, p3, p4);
        Console.WriteLine($"Отрезки (0,0)-(4,4) и (0,4)-(4,0) пересекаются в:\n {segIntersection}");

        // Тест пересечения прямой и окружности
        Console.WriteLine("\n4. Пересечение прямой и окружности:");
        Point center = new Point(2, 2);
        double radius = 2;
        var lineCircleIntersections = Geometry.LineCircleIntersection(p1, p2, center, radius);
        Console.WriteLine($"Прямая (0,0)-(4,4) и окружность с центром (2,2) радиусом 2:");
        foreach (var point in lineCircleIntersections)
        {
            Console.WriteLine($"  Точка пересечения: {point}");
        }

        // Тест пересечения отрезка и окружности
        Console.WriteLine("\n5. Пересечение отрезка и окружности:");
        var segCircleIntersections = Geometry.SegmentCircleIntersection(new Point(0, 0), new Point(4, 0), center, radius);
        Console.WriteLine($"Отрезок (0,0)-(4,0) и окружность с центром (2,2) радиусом 2:");
        foreach (var point in segCircleIntersections)
        {
            Console.WriteLine($"  Точка пересечения: {point}");
        }

        // Тест пересечения двух окружностей
        Console.WriteLine("\n6. Пересечение двух окружностей:");
        Point center2 = new Point(4, 2);
        double radius2 = 2;
        var circleCircleIntersections = Geometry.CircleCircleIntersection(center, radius, center2, radius2);
        Console.WriteLine($"Окружность 1: центр (2,2), радиус 2");
        Console.WriteLine($"Окружность 2: центр (4,2), радиус 2");
        foreach (var point in circleCircleIntersections)
        {
            Console.WriteLine($"  Точка пересечения: {point}");
        }
    }
}