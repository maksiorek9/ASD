public class SimpleTSP
{
    private int[,] dist;
    private int n;

    public SimpleTSP(int[,] distances)
    {
        dist = distances;
        n = distances.GetLength(0);
    }

    public int FindMinCost()
    {
        int size = 1 << n;
        int[,] dp = new int[size, n];

        // Инициализация
        for (int i = 0; i < size; i++)
        for (int j = 0; j < n; j++)
            dp[i, j] = int.MaxValue; 

        dp[1, 0] = 0;

        // Основной алгоритм
        for (int mask = 1; mask < size; mask++)
        {
            for (int last = 0; last < n; last++)
            {
                if ((mask & (1 << last)) == 0) continue;

                for (int prev = 0; prev < n; prev++)
                {
                    if ((mask & (1 << prev)) == 0) continue;
                    if (prev == last) continue;

                    int prevMask = mask ^ (1 << last);
                    // тут проверка чтобы небыло перепонения
                    if (dp[prevMask, prev] == int.MaxValue) continue;
                
                    long newCost = (long)dp[prevMask, prev] + dist[prev, last];
                
                    if (newCost < dp[mask, last])
                        dp[mask, last] = (int)newCost;
                }
            }
        }

        // Ищем минимальный цикл
        int minCost = int.MaxValue;
        int fullMask = size - 1;

        for (int i = 1; i < n; i++)
        {
            int total = dp[fullMask, i] + dist[i, 0];
            if (total < minCost)
                minCost = total;
        }

        return minCost;
    }
}

class Program
{
    static void Main()
    {
        int[,] dist =
        {
            { 0, 10, 15, 20 },
            { 10, 0, 35, 25 },
            { 15, 35, 0, 30 },
            { 20, 25, 30, 0 }
        };

        var tsp = new SimpleTSP(dist);
        int minCost = tsp.FindMinCost();

        Console.WriteLine(minCost);
    }
}