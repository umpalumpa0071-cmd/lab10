using System;

namespace Компилятор
{
    class Program
    {
        static void Main(string[] args)
        {
            InputOutputTests.RunAllTests();

            Console.WriteLine();
            Console.WriteLine(
                "Нажмите любую клавишу...");

            Console.ReadKey();
        }
    }
}