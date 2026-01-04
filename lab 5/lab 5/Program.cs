using System;
using System.Collections.Generic;
// Поиск подстроки с использованием алгоритма Рабина-Карпа
public class RabinKarp
{
    private const int Base = 256; // Количество символов в алфавите

    private const int Prime = 101; // Простое число для хеширования


    //Список позиций, где найден образец
    public static List<int> Search(string text, string pattern)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern) || pattern.Length > text.Length)
            return new List<int>();

        var positions = new List<int>();
        int n = text.Length;
        int m = pattern.Length;

        // Вычисляем хеш образца и первого окна текста
        long patternHash = 0;
        long textHash = 0;
        long h = 1;

        // h = Base^(m-1) % Prime
        for (int i = 0; i < m - 1; i++)
            h = (h * Base) % Prime;

        // Вычисляем хеши
        for (int i = 0; i < m; i++)
        {
            patternHash = (Base * patternHash + pattern[i]) % Prime;
            textHash = (Base * textHash + text[i]) % Prime;
        }

        // Скользим по тексту
        for (int i = 0; i <= n - m; i++)
        {
            // Проверяем хеши
            if (patternHash == textHash)
            {
                // Если хеши совпали, проверяем посимвольно
                bool match = true;
                for (int j = 0; j < m; j++)
                {
                    if (text[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    positions.Add(i);
            }

            // Вычисляем хеш следующего окна
            if (i < n - m)
            {
                textHash = (Base * (textHash - text[i] * h) + text[i + m]) % Prime;

                // Если хеш отрицательный, делаем его положительным
                if (textHash < 0)
                    textHash = (textHash + Prime);
            }
        }

        return positions;
    }

}

public class Program
{
    public static void Main()
    {
        string text = "abcgdfddsabc";
        string pattern = "abc";
        
        Console.WriteLine("Текст: " + text);
        Console.WriteLine("Образец: " + pattern);
        
        var rkResults = RabinKarp.Search(text, pattern);
        Console.WriteLine($"Найдено позиций: {rkResults.Count}");
        Console.WriteLine("Позиции: " + string.Join(", ", rkResults));
    }
}