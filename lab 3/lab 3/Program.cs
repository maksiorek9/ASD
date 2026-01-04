using System.Text;

namespace lab3;

public static class FiniteAutomatonPatternSearch
{
   
    // Таблица переходов: состояние -> (символ -> следующее состояние)
    private static Dictionary<int, Dictionary<char, int>> BuildTransitionTable(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            throw new ArgumentException("Шаблон не может быть пустым", nameof(pattern));

        int m = pattern.Length;
        var transitionTable = new Dictionary<int, Dictionary<char, int>>();

        
        for (int state = 0; state <= m; state++)
        {
            transitionTable[state] = new Dictionary<char, int>();
        }

        // Автоматически определяем алфавит из шаблона и добавляем символы
        var alphabet = new HashSet<char>(pattern);

        for (int state = 0; state <= m; state++)
        {
            // Для каждого символа в алфавите
            foreach (char c in alphabet)
            {
                // Начинаем с предположения, что можем перейти в следующее состояние
                int nextState = Math.Min(m, state + 1);

                // Пока не найдем правильный переход
                while (nextState > 0 && pattern[nextState - 1] != c)
                {
                    // Используем предыдущие переходы для поиска
                    nextState = transitionTable[nextState - 1].TryGetValue(c, out int val) ? val : 0;
                }

                transitionTable[state][c] = nextState;
                

                transitionTable[state][c] = nextState;
            }
        }

        return transitionTable;
    }

   
    // Находит все вхождения шаблона в тексте
    public static IEnumerable<int> SearchPattern(string text, string pattern)
    {
        var transitionTable = BuildTransitionTable(pattern);
        int m = pattern.Length;
        int state = 0;

        for (int i = 0; i < text.Length; i++)
        {
            char currentChar = text[i];

            // Если есть переход для текущего символа
            if (transitionTable[state].TryGetValue(currentChar, out int nextState))
            {
                state = nextState;
            }
            else
            {
                // Если символа нет в алфавите, возвращаемся в начальное состояние
                state = 0;
            }

            // Если достигли конечного состояния - нашли вхождение
            if (state == m)
            {
                state = 0;
                yield return i - m + 1;
            }
        }
    }
}



    

    

// Пример использования
public static class Program
{
    public static void Main()
    {
      
        string text = "abcccbabc";
        string pattern = "abc";

      
        Console.WriteLine($"Текст: {text}");
        Console.WriteLine($"Шаблон: {pattern}\n");
        

        var positions = FiniteAutomatonPatternSearch.SearchPattern(text, pattern).ToList();
        
        if (positions.Count > 0)
        {
            Console.WriteLine($"Найдено вхождений: {positions.Count}");
            Console.WriteLine($"Позиции: {string.Join(", ", positions)}");
        }
        else
        {
            Console.WriteLine("Вхождений не найдено");
        }
        

       
    }
}

