using System;
using System.IO;

namespace Компилятор
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "╔══════════════════════════════════╗");
            Console.WriteLine(
                "║ СИНТАКСИЧЕСКИЙ АНАЛИЗАТОР PASCAL ║");
            Console.WriteLine(
                "╚══════════════════════════════════╝");

            string filePath =
                AppDomain.CurrentDomain.BaseDirectory
                + "test.pas";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл test.pas не найден.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Исходная программа:");
            Console.WriteLine();

            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                Console.WriteLine((i + 1).ToString().PadLeft(4) + " " + lines[i]);
            }

            Console.WriteLine();
            Console.WriteLine("=== СИНТАКСИЧЕСКИЙ АНАЛИЗ ===");
            Console.WriteLine();

            InputOutput.OpenFile(filePath);

            SyntaxAnalyzer parser = new SyntaxAnalyzer();
            parser.ParseProgram();

            InputOutput.CloseFile();

            Console.WriteLine();
            Console.WriteLine("Анализ завершён.");
            InputOutput.PrintErrors();

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}