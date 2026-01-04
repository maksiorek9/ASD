public class Knuth_Morris_Pratt{
    static int[] GetPrefix(string s)
    {
        int[] result = new int[s.Length];
        result[0] = 0;
        int index = 0;

        for (int i = 1; i < s.Length; i++)
        {
            int k = result[i - 1];
            while (s[k] != s[i] && k > 0)
            {
                k = result[k - 1];
            }
            if (s[k] == s[i])
            {
                result[i] = k + 1;
            }
            else
            {
                result[i] = 0;
            }
        }
        return result;
    }

    public static List<int> FindSubstring(string pattern, string text)
    {
        int[] pf = GetPrefix(pattern);
        int index = 0;
        var count =  new List<int>();

        for (int i = 0; i < text.Length; i++)
        {
            while (index > 0 && pattern[index] != text[i]) 
                index = pf[index - 1]; 
            
            if (pattern[index] == text[i]) 
                index++;
            
            if (index == pattern.Length)
            {
                count.Add(i-index+1);
                index = 0;
                
            }
        }

        return count;
    }
    
    
    
}

public static class Program
{
    public static void Main()
    {
      
        string text = "abcgdfddsabc";
        string pattern = "abc";

        
        Console.WriteLine($"Текст: {text}");
        Console.WriteLine($"Шаблон: {pattern}");
        
        var rkResults = Knuth_Morris_Pratt.FindSubstring(pattern, text);
        
        Console.WriteLine($"Найдено позиций: {rkResults.Count}");
        Console.WriteLine("Позиции: " + string.Join(", ", rkResults));
       
    }
}