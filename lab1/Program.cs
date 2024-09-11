// See https://aka.ms/new-console-template for more information

internal class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Введите строку");

        
        
        string strochka = Console.ReadLine(); //получаем строку
        int colvoskobka = 0;// кароче схема такая считаем количество скобок и есть ли лишние символы

        foreach (var VARIABLE in strochka)
        {
            if (VARIABLE == '{' || VARIABLE == '}' || VARIABLE == '{' || VARIABLE == '}'|| VARIABLE == '{' || VARIABLE == '}')
            {
                
            }
            else
            {
                colvoskobka+=10;
                break;
            }
            
        }
       


        if (strochka != null && (strochka.Contains('{') || strochka.Contains('}'))) 
        { 
            colvoskobka += 1;
        }
        if (strochka != null &&(strochka.Contains('[') || strochka.Contains(']'))) 
        { 
            colvoskobka += 1;
        }
        if (strochka != null && (strochka.Contains('(') || strochka.Contains(')')))
        { 
            colvoskobka += 1;
        }

        
        switch (colvoskobka)
        {
            case 1:
                Console.WriteLine("Строка существует");
                break;
            case 3:
                Console.WriteLine("Строка существует");
                break;
            default:
                Console.WriteLine("Строка не существует");
                break;
                
        }

    }
}