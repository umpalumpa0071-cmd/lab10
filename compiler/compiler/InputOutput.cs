using System;
using System.Collections.Generic;
using System.IO;

namespace compiler
{
    struct TextPosition
    {
        private uint _lineNumber;
        private byte _charNumber;

        public TextPosition(uint lineNumber = 0, byte charNumber = 0)
        {
            _lineNumber = lineNumber;
            _charNumber = charNumber;
        }

        public uint LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }

        public byte CharNumber
        {
            get { return _charNumber; }
            set { _charNumber = value; }
        }
    }

    struct Err
    {
        private TextPosition _errorPosition;
        private byte _errorCode;

        public Err(TextPosition errorPosition, byte errorCode)
        {
            _errorPosition = errorPosition;
            _errorCode = errorCode;
        }

        public TextPosition ErrorPosition
        {
            get { return _errorPosition; }
            set { _errorPosition = value; }
        }

        public byte ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }
    }

    class InputOutput
    {
        private const byte _errMax = 9;

        private static char _ch;
        private static TextPosition _positionNow;
        private static string _line;
        private static byte _lastInLine;
        private static StreamReader _file;
        private static bool _endOfFile;
        private static uint _errCount;

        private static Dictionary<byte, string> _errorTable;
        private static Dictionary<uint, List<Err>> _errorsByLine;

        static InputOutput()
        {
            _ch = '\0';
            _positionNow = new TextPosition();
            _line = "";
            _lastInLine = 0;
            _file = null;
            _endOfFile = false;
            _errCount = 0;

            _errorsByLine = new Dictionary<uint, List<Err>>();

            _errorTable = new Dictionary<byte, string>()
            {
                {1,"ошибка ввода-вывода"},
                {2,"слишком много ошибок в строке"},

                {50,"неверный символ в программе"},
                {51,"пропущен идентификатор"},
                {52,"пропущена точка с запятой"},
                {53,"пропущена точка"},
                {54,"пропущено двоеточие"},
                {55,"пропущена запятая"},
                {56,"пропущена левая скобка"},
                {57,"пропущена правая скобка"},
                {58,"пропущен оператор присваивания :="},

                {100,"использование имени не соответствует описанию"},
                {101,"ожидалось ключевое слово begin"},
                {102,"ожидалось ключевое слово end"},
                {103,"пропущено ключевое слово program"},

                {147,"тип метки не совпадает с типом выбирающего выражения"},

                {200,"целочисленная константа вне диапазона"},
                {201,"вещественная константа вне диапазона"},
                {202,"недопустимый символ в строке"},
                {203,"константа превышает допустимый предел"},

                {250,"неожиданный конец файла"}
            };
        }

        public static char Ch
        {
            get { return _ch; }
        }

        public static TextPosition PositionNow
        {
            get { return _positionNow; }
        }

        public static void OpenFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Файл не найден!");
                return;
            }

            _file = new StreamReader(fileName);

            _positionNow = new TextPosition(1, 0);
            _errCount = 0;
            _endOfFile = false;

            _errorsByLine.Clear();

            ReadNextLine();

            if (_line.Length > 0)
                _ch = _line[0];
            else
                _ch = '\0';
        }

        public static void CloseFile()
        {
            if (_file != null)
            {
                _file.Close();
                _file = null;
            }
        }

        private static void ReadNextLine()
        {
            if (_file != null && !_file.EndOfStream)
            {
                _line = _file.ReadLine();

                if (_line == null)
                    _line = "";

                _lastInLine =
                    _line.Length > 0
                    ? (byte)(_line.Length - 1)
                    : (byte)0;
            }
            else
            {
                _line = "";
                _endOfFile = true;
            }
        }

        public static void NextCh()
        {
            if (_endOfFile)
                return;

            if (_positionNow.CharNumber >= _lastInLine)
            {
                PrintCurrentLine();

                PrintErrorsForLine(
                    _positionNow.LineNumber);

                ReadNextLine();

                if (_endOfFile)
                {
                    End();
                    return;
                }

                _positionNow.LineNumber++;

                _positionNow.CharNumber = 0;
            }
            else
            {
                _positionNow.CharNumber++;
            }

            if (_line.Length > 0)
                _ch = _line[_positionNow.CharNumber];
            else
                _ch = ' ';
        }

        public static void Error(
            byte errorCode,
            TextPosition position)
        {
            uint line = position.LineNumber;

            if (!_errorsByLine.ContainsKey(line))
            {
                _errorsByLine[line] =
                    new List<Err>();
            }

            if (_errorsByLine[line].Count < _errMax)
            {
                _errorsByLine[line].Add(
                    new Err(position, errorCode));
            }
        }

        private static void PrintCurrentLine()
        {
            string number =
                _positionNow.LineNumber.ToString();

            while (number.Length < 4)
                number = " " + number;

            Console.WriteLine(
                number + " " + _line);
        }

        private static void PrintErrorsForLine(
            uint lineNumber)
        {
            if (!_errorsByLine.ContainsKey(lineNumber))
                return;

            foreach (Err err in _errorsByLine[lineNumber])
            {
                _errCount++;

                string s = "**";

                if (_errCount < 10)
                    s += "0";

                s += _errCount + "**";

                while (
                    s.Length <
                    err.ErrorPosition.CharNumber + 5)
                {
                    s += " ";
                }

                s += "^ ошибка код "
                     + err.ErrorCode;

                Console.WriteLine(s);

                Console.WriteLine(
                    "****** "
                    + _errorTable[err.ErrorCode]);
            }
        }

        private static void End()
        {
            Console.WriteLine();

            Console.WriteLine(
                "Компиляция окончена. Ошибок: "
                + _errCount);

            _ch = '\0';

            CloseFile();
        }

        public static void PrintErrorTable()
        {
            Console.WriteLine();
            Console.WriteLine(
                "ТАБЛИЦА ОШИБОК");
            Console.WriteLine(
                "----------------");

            foreach (var item in _errorTable)
            {
                Console.WriteLine(
                    item.Key + " - " + item.Value);
            }

            Console.WriteLine();
        }
    }
}