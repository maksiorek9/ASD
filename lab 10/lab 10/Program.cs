using System;


public static class EggDropSolver
{
    // Симуляция проверки этажа
    public static bool IsEggBroken(int floor, int criticalFloor)
    {
        return floor >= criticalFloor;
    }

    // Поиск критического этажа
    public static (int foundFloor, int attempts) FindCriticalFloor(int maxFloors, int criticalFloor)
    {
        int attempts = 0;
        int step = 14;
        int currentFloor = 0;
        int lastSafeFloor = 0;

        // Используем первое яйцо для определения диапазона
        while (currentFloor < maxFloors)
        {
            // Определяем следующий этаж для проверки
            int nextFloor = Math.Min(lastSafeFloor + step, maxFloors);
            attempts++;

            if (IsEggBroken(nextFloor, criticalFloor))
            {
                // Первое яйцо разбилось, переходим ко второму
                currentFloor = nextFloor;
                break;
            }
            else
            {
                // Яйцо не разбилось, продолжаем с меньшим шагом
                lastSafeFloor = nextFloor;
                step--;
                if (step < 1) step = 1;
            }
        }

        // Используем второе яйцо для точного поиска
        // Проверяем этажи от lastSafeFloor + 1 до currentFloor - 1 (или currentFloor если дошли до конца)
        int startFloor = lastSafeFloor + 1;
        int endFloor = Math.Min(currentFloor - 1, maxFloors);

        if (currentFloor == 0)
        {
            // Если первое яйцо ни разу не разбилось
            endFloor = maxFloors;
        }

        for (int floor = startFloor; floor <= endFloor; floor++)
        {
            attempts++;
            if (IsEggBroken(floor, criticalFloor))
            {
                return (floor, attempts);
            }
        }

        // Если ни одно яйцо не разбилось
        return (maxFloors + 1, attempts); // Критический этаж выше максимального
    }
}

public class Program
{
    public static void Main()
        {
            int totalFloors = 100;
            int maxAttempts = 0;
            
            var result = EggDropSolver.FindCriticalFloor(totalFloors, 100);
            Console.WriteLine($"Найдено: {result.foundFloor}, попыток: {result.attempts}");
            
            // Покажем последовательность бросков первого яйца
            Console.WriteLine("\nОптимальная последовательность бросков первого яйца:");
            int step = 14;
            int floor = 0;
            int attempt = 1;
            
            while (floor < totalFloors)
            {
                floor = Math.Min(floor + step, totalFloors);
                Console.WriteLine($"  Бросок {attempt}: этаж {floor}");
                step--;
                attempt++;
                if (step < 1) step = 1;
            }
        }
}
/* если будет непонят моими словами
Бросаем первое яйцо с этажа
x, затем x + ( x − 1 )
x+(x−1), затем x + ( x − 1 ) + ( x − 2 )
x+(x−1)+(x−2) и т.д., пока оно не разобьётся.
Когда первое яйцо разбилось на шаге k, используем второе яйцо, чтобы проверить все этажи между предыдущим успешным броском и текущим, пока не найдём точный критический этаж.
Минимальное число бросков в худшем случае достигается, когда сумма арифметической прогрессии
x + ( x − 1 ) + ( x − 2 ) + ⋯ + 1
x+(x−1)+(x−2)+⋯+1 покрывает все 100 этажей:
x ( x + 1 ) / 2 ≥ 100
Решаем:
x(x+1)/2≥100

x≈14 (так как 14×15/2=105≥10014×15/2=105≥100).

Таким образом, максимум 14 бросков в худшем случае.
   */