using System;
using System.Collections.Generic;

public class Item
{
    public int Weight { get; set; }
    public int Value { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class KnapsackSolver
{
    private List<Item> bestCombination = new();
    private int bestValue = 0;
    private int capacity;
    private List<Item> items;

    public (int maxValue, List<Item> items) SolveBruteForce(List<Item> items, int capacity)
    {
        this.items = items;
        this.capacity = capacity;
        bestCombination.Clear();
        bestValue = 0;

        ExploreCombinations(0, new List<Item>(), 0, 0);

        return (bestValue, bestCombination);
    }

    private void ExploreCombinations(int index, List<Item> currentCombination,
                                     int currentWeight, int currentValue)
    {
        // Базовый случай: рассмотрели все предметы
        if (index >= items.Count)
        {
            // Проверяем, является ли текущая комбинация лучше
            if (currentWeight <= capacity && currentValue > bestValue)
            {
                bestValue = currentValue;
                bestCombination = new List<Item>(currentCombination);
            }
            return;
        }

        var currentItem = items[index];

        // Вариант 1: не берем текущий предмет
        ExploreCombinations(
            index + 1,
            currentCombination,
            currentWeight,
            currentValue
        );

        // Вариант 2: берем текущий предмет (если помещается)
        if (currentWeight + currentItem.Weight <= capacity)
        {
            currentCombination.Add(currentItem);
            ExploreCombinations(
                index + 1,
                currentCombination,
                currentWeight + currentItem.Weight,
                currentValue + currentItem.Value
            );
            currentCombination.RemoveAt(currentCombination.Count - 1);
        }
    }
}

class Program
{
    static void Main()
    {
        var items = new List<Item>
        {
            new() { Name = "Ноутбук", Weight = 3, Value = 2000 },
            new() { Name = "Фотоаппарат", Weight = 1, Value = 1500 },
            new() { Name = "Книга", Weight = 1, Value = 500 },
            new() { Name = "Бутылка воды", Weight = 2, Value = 100 }
        };

        int capacity = 5;

        Console.WriteLine("Решение задачи о рюкзаке методом полного перебора");
        Console.WriteLine($"Вместимость рюкзака: {capacity}\n");

        Console.WriteLine("Доступные предметы:");
        foreach (var item in items)
        {
            Console.WriteLine($"- {item.Name}: вес={item.Weight}, ценность={item.Value}");
        }

        var solver = new KnapsackSolver();
        var (maxValue, selectedItems) = solver.SolveBruteForce(items, capacity);

        Console.WriteLine("\nОптимальное решение:");
        Console.WriteLine($"Максимальная ценность: {maxValue}");
        Console.WriteLine($"Выбрано предметов: {selectedItems.Count}");
        Console.WriteLine("Выбранные предметы:");

        int totalWeight = 0;
        foreach (var item in selectedItems)
        {
            Console.WriteLine($"  {item.Name} (вес: {item.Weight}, ценность: {item.Value})");
            totalWeight += item.Weight;
        }
        Console.WriteLine($"Общий вес: {totalWeight}/{capacity}");
    }
}