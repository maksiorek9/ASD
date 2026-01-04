using System;

class CoinChange
{
    public static long CountWays2D(int[] coins, int amount)
    {
        if (amount < 0) return 0;
        if (amount == 0) return 1;
        if (coins.Length == 0) return 0;

        int n = coins.Length;
        long[,] dp = new long[n + 1, amount + 1];

        for (int i = 0; i <= n; i++)
        {
            dp[i, 0] = 1;
        }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= amount; j++)
            {
                dp[i, j] = dp[i - 1, j];
                if (j >= coins[i - 1])
                {
                    dp[i, j] += dp[i, j - coins[i - 1]];
                }
            }
        }

        return dp[n, amount];
    }
}

public  class CoinChange2
    {
        public CoinChange2(){}
        public static long CountWays2(int[] coins, int amount)
        {
            if (amount < 0) return 0;
            if (amount == 0) return 1;
            if (coins.Length == 0) return 0;

            long[] dp = new long[amount + 1];
            dp[0] = 1;

            foreach (int coin in coins)
            {
                for (int i = coin; i <= amount; i++)
                {
                    dp[i] += dp[i - coin];
                }
            }

            return dp[amount];
        }
    }


public static class Program
{
    public static void Main()
    {
        int[] coins = [1, 2, 5, 13, 50, 10, 80, 100, 200];
        int amount = 200;

        int[] coins2 = [1, 2, 5];
        int amount2 = 5;

        long ways2 = CoinChange2.CountWays2(coins2, amount2);
        Console.WriteLine($"Количество способов размена {amount2}: {ways2}");
        // Вывод: 4 (как работает){5, 2+2+1, 2+1+1+1, 1+1+1+1+1}


        long ways = CoinChange.CountWays2D(coins, amount);
        Console.WriteLine($"Количество способов размена {amount}: {ways}");
    }
}