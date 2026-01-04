public class BacktrackingGraphColoring
{
    private readonly int[,] graph;
    private readonly int vertices;
    private int[] colorAssignment;
    private int chromaticNumber = int.MaxValue;
    private int[] bestColoring;
    
    public BacktrackingGraphColoring(int[,] adjacencyMatrix)
    {
        graph = adjacencyMatrix;
        vertices = adjacencyMatrix.GetLength(0);
        colorAssignment = new int[vertices];
        bestColoring = new int[vertices];
    }
    
    public (int colors, int[] coloring) FindOptimalColoring(int maxColors)
    {
        chromaticNumber = int.MaxValue;
        
        for (int colors = 1; colors <= maxColors; colors++)
        {
            if (ColorGraph(0, colors))
            {
                return (colors, bestColoring.Take(vertices).ToArray());
            }
        }
        
        return (chromaticNumber, bestColoring);
    }
    
    private bool ColorGraph(int vertex, int availableColors)
    {
        if (vertex == vertices)
        {
            // Все вершины раскрашены
            if (availableColors < chromaticNumber)
            {
                chromaticNumber = availableColors;
                Array.Copy(colorAssignment, bestColoring, vertices);
            }
            return true;
        }
        
        bool success = false;
        
        for (int color = 1; color <= availableColors; color++)
        {
            if (IsSafe(vertex, color))
            {
                colorAssignment[vertex] = color;
                
                if (ColorGraph(vertex + 1, availableColors))
                {
                    success = true;
                }
                
                colorAssignment[vertex] = 0; // Backtrack
            }
        }
        
        return success;
    }
    
    private bool IsSafe(int vertex, int color)
    {
        for (int i = 0; i < vertices; i++)
        {
            if (graph[vertex, i] == 1 && colorAssignment[i] == color)
            {
                return false;
            }
        }
        return true;
    }
}


class Program
{
    static void Main()
    {
        // Пример графа (матрица смежности)
        int[,] graph = {
            {0, 1, 1, 1, 0},
            {1, 0, 1, 0, 1},
            {1, 1, 0, 1, 0},
            {1, 0, 1, 0, 1},
            {0, 1, 0, 1, 0}
        };
        
        
        Console.WriteLine("\n=== Алгоритм с возвратом (Backtracking) ===");
        var backtracking = new BacktrackingGraphColoring(graph);
        var (colors, coloring) = backtracking.FindOptimalColoring(5);
        Console.WriteLine($"Хроматическое число: {colors}");
        Console.WriteLine("Раскраска: " + string.Join(", ", coloring));
        
    }
    
}