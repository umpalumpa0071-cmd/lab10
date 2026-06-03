using System;
using System.IO;

namespace Компилятор
{
    static class InputOutputTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine(
                "╔══════════════════════════════════╗");
            Console.WriteLine(
                "║  ТЕСТ ЛЕКСИЧЕСКОГО АНАЛИЗАТОРА  ║");
            Console.WriteLine(
                "╚══════════════════════════════════╝");

            string filePath =
                AppDomain.CurrentDomain.BaseDirectory +
                "test.pas";

            if (!File.Exists(filePath))
            {
                Console.WriteLine();
                Console.WriteLine(
                    "Файл test.pas не найден!");

                return;
            }

            Console.WriteLine();
            Console.WriteLine(
                "Исходная программа:");
            Console.WriteLine();

            string[] lines =
                File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                string lineNumber =
                    (i + 1).ToString();

                while (lineNumber.Length < 4)
                {
                    lineNumber =
                        " " + lineNumber;
                }

                Console.WriteLine(
                    lineNumber + " " + lines[i]);
            }

            InputOutput.OpenFile(filePath);

            Console.WriteLine();
            Console.WriteLine(
                "=== ЛЕКСИЧЕСКИЙ АНАЛИЗ ===");
            Console.WriteLine();

            LexicalAnalyzer lexer =
                new LexicalAnalyzer();

            byte code;

            do
            {
                code = lexer.NextSym();

                if (code != 0)
                {
                    Console.Write(
                        "Код = " + code);

                    Console.Write(
                        " | Строка "
                        + lexer.Token.LineNumber);

                    Console.Write(
                        " | Символ "
                        + lexer.Token.CharNumber);

                    if (code ==
                        LexicalAnalyzer.ident)
                    {
                        Console.Write(
                            " | Идентификатор = "
                            + lexer.AddrName);
                    }

                    if (code ==
                        LexicalAnalyzer.intc)
                    {
                        Console.Write(
                            " | Число = "
                            + lexer.NmbInt);
                    }

                    Console.WriteLine();
                }

            } while (code != 0);

            lexer.PrintOutputCodesByLine();

            InputOutput.CloseFile();

            Console.WriteLine();
            Console.WriteLine(
                "=== Тест завершён ===");
        }
    }
}