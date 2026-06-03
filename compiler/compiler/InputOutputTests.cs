using System;
using System.IO;

namespace compiler
{
    static class InputOutputTests
    {
        private static string CreateTestFile(
            string fileName,
            string content)
        {
            string path =
                AppDomain.CurrentDomain.BaseDirectory
                + fileName;

            File.WriteAllText(path, content);

            return path;
        }

        public static void RunAllTests()
        {
            Console.WriteLine(
                "=================================");
            Console.WriteLine(
                " ТЕСТ МОДУЛЯ ВВОДА-ВЫВОДА");
            Console.WriteLine(
                "=================================");

            InputOutput.PrintErrorTable();

            TestPascalProgram();
        }

        private static void TestPascalProgram()
        {
            string content =
                "program test;\n" +
                "var a,b : integer;\n" +
                "begin\n" +
                "a := 10;\n" +
                "b := a + 5;\n" +
                "end.";

            string fileName =
                CreateTestFile(
                    "test.pas",
                    content);

            InputOutput.OpenFile(fileName);

            InputOutput.Error(
                100,
                new TextPosition(2, 4));

            InputOutput.Error(
                52,
                new TextPosition(4, 7));

            while (InputOutput.Ch != '\0')
            {
                InputOutput.NextCh();
            }

            InputOutput.CloseFile();
        }
    }
}