using System;



public static class KadaneAlgorithm
{
    public readonly record struct Result(int MaxSum, int StartIndex, int EndIndex, int[] Subarray);

    public static Result FindMaxSubarray(ReadOnlySpan<int> nums)
    {
        if (nums.Length == 0)
            throw new ArgumentException("Массив не может быть пустым");

        int maxSum = nums[0];
        int currentSum = nums[0];
        int startIndex = 0;
        int endIndex = 0;
        int tempStart = 0;

        for (int i = 1; i < nums.Length; i++)
        {
            // Если текущая сумма отрицательная, начинаем новый подмассив
            if (currentSum < 0)
            {
                currentSum = nums[i];
                tempStart = i;
            }
            else
            {
                currentSum += nums[i];
            }

            // Если нашли новую максимальную сумму
            if (currentSum > maxSum)
            {
                maxSum = currentSum;
                startIndex = tempStart;
                endIndex = i;
            }
        }

        // Извлекаем подмассив
        int subarrayLength = endIndex - startIndex + 1;
        int[] subarray = new int[subarrayLength];
        nums[startIndex..(endIndex + 1)].CopyTo(subarray);

        return new Result(maxSum, startIndex, endIndex, subarray);
    }
}

// Пример использования с коллекционными выражениями C# 12
class Program
{
    static void Main()
    {
        // Использование коллекционных выражений
        int[] testArray1 = [-2, 1, -3, 4, -1, 2, 1, -5, 4];
        int[] testArray2 = [1, 2, 3, -10, 4, 5];
        int[] testArray3 = [-5, -4, -3, -2, -1];
        int[] testArray4 = [];

        List<int[]> testArrays = [testArray1, testArray2, testArray3, testArray4];

        foreach (var array in testArrays)
        {
            Console.WriteLine($"Анализируем массив: [{string.Join(", ", array)}]");
            
            try
            {
                var result = KadaneAlgorithm.FindMaxSubarray(array);
                Console.WriteLine($"✓ Максимальная сумма: {result.MaxSum}");
                Console.WriteLine($"✓ Подмассив: [{string.Join(", ", result.Subarray)}]");
                Console.WriteLine($"✓ Индексы: {result.StartIndex}..{result.EndIndex}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
            
            Console.WriteLine(new string('═', 60));
        }
    }
}