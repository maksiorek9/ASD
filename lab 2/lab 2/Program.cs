using System;
using System.Collections.Generic;
using System.Linq;
//  O(n^3 + k^2), где k - количество неколлинеарных треугольников.
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

public class EfficientTriangleFinder
{
    // Оптимизированная проверка точки внутри треугольника (метод барицентрических координат)
    public static bool IsPointInsideTriangle(Point p, Point a, Point b, Point c)
    {
        // Векторы
        double v0x = c.X - a.X;
        double v0y = c.Y - a.Y;
        double v1x = b.X - a.X;
        double v1y = b.Y - a.Y;
        double v2x = p.X - a.X;
        double v2y = p.Y - a.Y;

        // Вычисляем скалярные произведения
        double dot00 = v0x * v0x + v0y * v0y;
        double dot01 = v0x * v1x + v0y * v1y;
        double dot02 = v0x * v2x + v0y * v2y;
        double dot11 = v1x * v1x + v1y * v1y;
        double dot12 = v1x * v2x + v1y * v2y;

        // Вычисляем барицентрические координаты
        double invDenom = 1.0 / (dot00 * dot11 - dot01 * dot01);
        double u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        double v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        // Точка внутри, если u >= 0, v >= 0 и u+v <= 1
        return (u >= -1e-9) && (v >= -1e-9) && (u + v <= 1.0 + 1e-9);
    }

    // Быстрая проверка вложенности треугольников с предварительными проверками
    public static bool IsTriangleInsideTriangleOptimized(Triangle t1, Triangle t2)
    {
        // Быстрая проверка по bounding box (ограничивающим прямоугольникам)
        if (!IsBoundingBoxInside(t1, t2))
            return false;

        // Проверяем все вершины первого треугольника внутри второго
        return IsPointInsideTriangle(t1.A, t2.A, t2.B, t2.C) &&
               IsPointInsideTriangle(t1.B, t2.A, t2.B, t2.C) &&
               IsPointInsideTriangle(t1.C, t2.A, t2.B, t2.C);
    }

    // Проверка по ограничивающим прямоугольникам (быстрая отсечка)
    private static bool IsBoundingBoxInside(Triangle t1, Triangle t2)
    {
        // Находим min/max координат для каждого треугольника
        (int minX1, int maxX1, int minY1, int maxY1) = GetBoundingBox(t1);
        (int minX2, int maxX2, int minY2, int maxY2) = GetBoundingBox(t2);

        // Проверяем, помещается ли bbox первого треугольника внутри второго
        return minX1 >= minX2 && maxX1 <= maxX2 && minY1 >= minY2 && maxY1 <= maxY2;
    }

    // Получение ограничивающего прямоугольника треугольника
    private static (int minX, int maxX, int minY, int maxY) GetBoundingBox(Triangle t)
    {
        double minX = Math.Min(t.A.X, Math.Min(t.B.X, t.C.X));
        double maxX = Math.Max(t.A.X, Math.Max(t.B.X, t.C.X));
        double minY = Math.Min(t.A.Y, Math.Min(t.B.Y, t.C.Y));
        double maxY = Math.Max(t.A.Y, Math.Max(t.B.Y, t.C.Y));
        return ((int minX, int maxX, int minY, int maxY))(minX, maxX, minY, maxY);
    }

    // Эффективный поиск вложенных треугольников
    public static List<(Triangle outer, Triangle inner)> FindNestedTrianglesOptimized(Point[] points)
    {
        List<(Triangle outer, Triangle inner)> result = new List<(Triangle outer, Triangle inner)>();
        int n = points.Length;

        if (n < 6) return result; // Нужно минимум 6 точек

        // Предварительно сортируем точки по X, затем по Y для лучшей локальности
        Point[] sortedPoints = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();

        // Генерируем треугольники с использованием кэша
        List<Triangle> triangles = GenerateTrianglesWithCache(sortedPoints);

        // Сортируем треугольники по площади для более эффективной проверки
        triangles = triangles.OrderBy(t => GetTriangleArea(t)).ToList();

        // Используем хеш-сет для отслеживания уже проверенных пар
        HashSet<string> checkedPairs = new HashSet<string>();

        // Проверяем треугольники попарно
        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle t1 = triangles[i];
            var bbox1 = GetBoundingBox(t1);

            for (int j = i + 1; j < triangles.Count; j++)
            {
                Triangle t2 = triangles[j];
                
                // Создаем уникальный ключ для пары треугольников
                string pairKey = $"{Math.Min(i, j)}-{Math.Max(i, j)}";
                if (checkedPairs.Contains(pairKey))
                    continue;
                
                checkedPairs.Add(pairKey);

                // Быстрая проверка по площадям (меньший треугольник потенциально внутри большего)
                if (GetTriangleArea(t1) >= GetTriangleArea(t2))
                    continue;

                // Проверка по bounding box
                var bbox2 = GetBoundingBox(t2);
                if (bbox1.minX < bbox2.minX || bbox1.maxX > bbox2.maxX ||
                    bbox1.minY < bbox2.minY || bbox1.maxY > bbox2.maxY)
                    continue;

                // Детальная проверка
                if (IsTriangleInsideTriangleOptimized(t1, t2))
                {
                    result.Add((t2, t1)); // t2 больше и содержит t1
                }
            }
        }

        return result;
    }

    // Генерация треугольников с устранением коллинеарных точек
    private static List<Triangle> GenerateTrianglesWithCache(Point[] points)
    {
        List<Triangle> triangles = new List<Triangle>();
        int n = points.Length;

        for (int i = 0; i < n - 2; i++)
        {
            Point a = points[i];
            
            for (int j = i + 1; j < n - 1; j++)
            {
                Point b = points[j];
                
                for (int k = j + 1; k < n; k++)
                {
                    Point c = points[k];
                    
                    // Пропускаем коллинеарные точки
                    if (IsCollinear(a, b, c))
                        continue;
                    
                    triangles.Add(new Triangle(a, b, c));
                }
            }
        }

        return triangles;
    }

    // Проверка на коллинеарность трех точек
    private static bool IsCollinear(Point a, Point b, Point c)
    {
        // Векторное произведение = 0 для коллинеарных точек
        return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X) == 0;
    }

    // Вычисление площади треугольника (без деления на 2 для сравнения)
    private static double GetTriangleArea(Triangle t)
    {
        return Math.Abs((t.B.X - t.A.X) * (t.C.Y - t.A.Y) - 
                        (t.C.X - t.A.X) * (t.B.Y - t.A.Y));
    }

    // Оптимизированный метод для проверки одного треугольника внутри другого
    public static bool IsTriangleCompletelyInside(Point[] triangle1, Point[] triangle2)
    {
        if (triangle1.Length != 3 || triangle2.Length != 3)
            return false;

        Triangle t1 = new Triangle(triangle1[0], triangle1[1], triangle1[2]);
        Triangle t2 = new Triangle(triangle2[0], triangle2[1], triangle2[2]);

        return IsTriangleInsideTriangleOptimized(t1, t2);
    }
}

public class Triangle
{
    public Point A { get; set; }
    public Point B { get; set; }
    public Point C { get; set; }
    
    public Triangle(Point a, Point b, Point c)
    {
        A = a;
        B = b;
        C = c;
    }
    
    public override string ToString()
    {
        return $"[{A}, {B}, {C}]";
    }
    
    // Метод для получения площади
    public double GetArea()
    {
        return Math.Abs((B.X - A.X) * (C.Y - A.Y) - (C.X - A.X) * (B.Y - A.Y)) / 2.0;
    }
}

public class Program
{
    internal static void Main(string[] args)
    {
        Console.WriteLine("=== Эффективный поиск вложенных треугольников ===");
        
        // Тестовые данные
        Point[] points = {
            new Point(0, 0),   // 0
            new Point(0, 10),  // 1
            new Point(10, 0),  // 2
            new Point(2, 2),   // 3
            new Point(2, 8),   // 4
            new Point(8, 2),   // 5
            new Point(4, 4),   // 6
            new Point(4, 6),   // 7
            new Point(6, 4),   // 8
            new Point(5, 5)    // 9
        };
        
        Console.WriteLine($"Всего точек: {points.Length}");
        
        // Эффективный поиск
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var nestedTriangles = EfficientTriangleFinder.FindNestedTrianglesOptimized(points);
        watch.Stop();
        
        Console.WriteLine($"\nВремя выполнения: {watch.ElapsedMilliseconds} мс");
        Console.WriteLine($"Найдено вложенных треугольников: {nestedTriangles.Count}");
        
        // Вывод результатов
        int count = 0;
        foreach (var (outer, inner) in nestedTriangles.Take(10)) 
        {
            Console.WriteLine($"{++count}. Внешний: {outer} (площадь: {outer.GetArea():F2})");
            Console.WriteLine($"   Внутренний: {inner} (площадь: {inner.GetArea():F2})");
        }
        
        if (nestedTriangles.Count > 10)
            Console.WriteLine($"... и еще {nestedTriangles.Count - 10} пар");
    
    }
}

/* O(n^6) вроде как тут
public class TriangleFinder
   {
       // Метод для проверки, лежит ли точка внутри треугольника
       public static bool IsPointInsideTriangle(Point p, Point a, Point b, Point c)
       {
           // Используем барицентрические координаты
           double areaABC = TriangleArea(a, b, c);
           double areaPBC = TriangleArea(p, b, c);
           double areaPCA = TriangleArea(p, c, a);
           double areaPAB = TriangleArea(p, a, b);
           
           // Точка внутри, если сумма площадей трех треугольников равна площади основного
           return Math.Abs(areaABC - (areaPBC + areaPCA + areaPAB)) < 1e-9;
       }
       
       // Метод для вычисления площади треугольника
       private static double TriangleArea(Point a, Point b, Point c)
       {
           return Math.Abs((b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y)) / 2.0;
       }
       
       // Метод для проверки, является ли треугольник 1 вложенным в треугольник 2
       public static bool IsTriangleInsideTriangle(Point a1, Point b1, Point c1, 
                                                  Point a2, Point b2, Point c2)
       {
           // Проверяем, все ли вершины первого треугольника лежат внутри второго
           return IsPointInsideTriangle(a1, a2, b2, c2) &&
                  IsPointInsideTriangle(b1, a2, b2, c2) &&
                  IsPointInsideTriangle(c1, a2, b2, c2);
       }
       
       // Метод для поиска всех вложенных треугольников в множестве точек
       public static List<(Triangle outer, Triangle inner)> FindNestedTriangles(Point[] points)
       {
           List<(Triangle, Triangle)> result = new List<(Triangle, Triangle)>();
           
           if (points.Length < 6)
               return result; // Нужно минимум 6 точек для двух треугольников
           
           // Генерируем все возможные тройки точек (треугольники)
           List<Triangle> triangles = new List<Triangle>();
           
           for (int i = 0; i < points.Length - 2; i++)
           {
               for (int j = i + 1; j < points.Length - 1; j++)
               {
                   for (int k = j + 1; k < points.Length; k++)
                   {
                       triangles.Add(new Triangle(points[i], points[j], points[k]));
                   }
               }
           }
           
           // Проверяем все пары треугольников на вложенность
           for (int i = 0; i < triangles.Count; i++)
           {
               for (int j = 0; j < triangles.Count; j++)
               {
                   if (i == j) continue;
                   
                   Triangle t1 = triangles[i];
                   Triangle t2 = triangles[j];
                   
                   // Проверяем в обе стороны: t1 внутри t2 или t2 внутри t1
                   if (IsTriangleInsideTriangle(t1.A, t1.B, t1.C, t2.A, t2.B, t2.C))
                   {
                       result.Add((t2, t1)); // t2 - внешний, t1 - внутренний
                   }
                   else if (IsTriangleInsideTriangle(t2.A, t2.B, t2.C, t1.A, t1.B, t1.C))
                   {
                       result.Add((t1, t2)); // t1 - внешний, t2 - внутренний
                   }
               }
           }
           
           return result;
       }
   }
   
   // Класс для представления треугольника
   public class Triangle
   {
       public Point A { get; set; }
       public Point B { get; set; }
       public Point C { get; set; }
       
       public Triangle(Point a, Point b, Point c)
       {
           A = a;
           B = b;
           C = c;
       }
       
       public override string ToString()
       {
           return $"[{A}, {B}, {C}]";
       }
   }
   */