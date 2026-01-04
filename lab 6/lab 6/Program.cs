using System;
using System.Collections.Generic;
// Поиск подстроки с использованием алгоритма Бойера-Мура
public class BoyerMoore
{
    public static List<int> Search(string text, string pattern)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern) || pattern.Length > text.Length)
            return new List<int>();

        var positions = new List<int>();
        int n = text.Length;
        int m = pattern.Length;

        // Таблица плохого символа
        var badChar = CreateBadCharacterTable(pattern);

        int s = 0;

        while (s <= n - m)
        {
            int j = m - 1;
            
            while (j >= 0 && pattern[j] == text[s + j])
                j--;

            if (j < 0)
            {
                positions.Add(s);
                
                s += (s + m < n) ? m - badChar.GetValueOrDefault(text[s + m], -1) : 1;
            }
            else
            {
                int badCharShift = j - badChar.GetValueOrDefault(text[s + j], -1);
                s += Math.Max(1, badCharShift);
            }
        }

        return positions;
    }

    
    // Создание таблицы плохого символа
    
    private static Dictionary<char, int> CreateBadCharacterTable(string pattern)
    {
        var table = new Dictionary<char, int>();
        int m = pattern.Length;
        // Заполняем таблицу позициями символов в образце
        // Используем последнее вхождение каждого символа
        for (int i = 0; i < m; i++)
            table[pattern[i]] = i;

        return table;
    }
}

public static class Program
{
    static void Main()
    {
        string text = "abcgdfddsabc";
        string pattern = "abc";

        Console.WriteLine("Текст: " + text);
        Console.WriteLine("Образец: " + pattern);
        

        var bmResults = BoyerMoore.Search(text, pattern);
        Console.WriteLine($"Найдено позиций: {bmResults.Count}");
        Console.WriteLine("Позиции: " + string.Join(", ", bmResults));
        
    }
}