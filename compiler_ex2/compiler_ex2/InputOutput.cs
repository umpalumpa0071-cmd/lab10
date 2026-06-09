using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    struct TextPosition
    {
        private uint _lineNumber;
        private byte _charNumber;

        public TextPosition(uint ln = 0, byte cn = 0)
        {
            _lineNumber = ln;
            _charNumber = cn;
        }

        public uint LineNumber 
        {
            get => _lineNumber; set => _lineNumber = value; 
        }
        public byte CharNumber 
        {
            get => _charNumber; set => _charNumber = value; 
        }
    }

    struct Err
    {
        private TextPosition _errorPosition;
        private byte _errorCode;

        public Err(TextPosition pos, byte code)
        {
            _errorPosition = pos;
            _errorCode = code;
        }

        public TextPosition ErrorPosition { get => _errorPosition; set => _errorPosition = value; }
        public byte ErrorCode { get => _errorCode; set => _errorCode = value; }
    }

    class InputOutput
    {
        private static char _ch;
        private static TextPosition _positionNow;
        private static string _line;
        private static byte _lastInLine;
        private static StreamReader _file;
        private static bool _endOfFile;
        private static List<Err> _errors;
        private static Dictionary<byte, string> _errorTable;

        static InputOutput()
        {
            _ch = '\0';
            _positionNow = new TextPosition(1, 0);
            _line = "";
            _lastInLine = 0;
            _file = null;
            _endOfFile = false;
            _errors = new List<Err>();
            _errorTable = new Dictionary<byte, string>();
            InitErrors();
        }

        private static void InitErrors()
        {
            _errorTable[1] = "Ошибка ввода-вывода";
            _errorTable[50] = "Неверный символ";
            _errorTable[51] = "Пропущен идентификатор";
            _errorTable[52] = "Пропущена точка с запятой";
            _errorTable[53] = "Пропущена точка";
            _errorTable[54] = "Пропущено двоеточие";
            _errorTable[55] = "Пропущена запятая";
            _errorTable[56] = "Пропущена левая скобка";
            _errorTable[57] = "Пропущена правая скобка";
            _errorTable[58] = "Пропущен оператор :=";
            _errorTable[59] = "Пропущено ключевое слово THEN";
            _errorTable[60] = "Пропущено ключевое слово DO";
            _errorTable[61] = "Неверное описание типа";
            _errorTable[62] = "Пропущено OF";
            _errorTable[63] = "Пропущено END";
            _errorTable[64] = "Пропущено UNTIL";
            _errorTable[200] = "Целое число вне диапазона";
            _errorTable[201] = "Вещественное число вне диапазона";
            _errorTable[202] = "Ошибка строковой константы";
            _errorTable[203] = "Слишком длинная константа";
            _errorTable[250] = "Неожиданный конец файла";
        }

        public static char Ch => _ch;
        public static TextPosition PositionNow => _positionNow;

        public static void OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден.");
                return;
            }

            _file = new StreamReader(filePath);
            _positionNow = new TextPosition(1, 0);
            _endOfFile = false;
            ReadNextLine();

            _ch = (_line.Length > 0) ? _line[0] : '\0';
        }

        public static void CloseFile()
        {
            _file?.Close();
            _file = null;
        }

        private static void ReadNextLine()
        {
            if (_file == null || _file.EndOfStream)
            {
                _endOfFile = true;
                _line = "";
                return;
            }

            _line = _file.ReadLine() ?? "";
            if (_line.Length == 0) _line = " ";
            _lastInLine = (byte)(_line.Length - 1);
        }

        public static void NextCh()
        {
            if (_endOfFile)
            {
                _ch = '\0';
                return;
            }

            if (_positionNow.CharNumber >= _lastInLine)
            {
                ReadNextLine();
                if (_endOfFile)
                {
                    _ch = '\0';
                    return;
                }

                _positionNow.LineNumber++;
                _positionNow.CharNumber = 0;
                _ch = _line[0];
            }
            else
            {
                _positionNow.CharNumber++;
                _ch = _line[_positionNow.CharNumber];
            }
        }

        public static void Error(byte errorCode, TextPosition position)
        {
            _errors.Add(new Err(position, errorCode));
            Console.WriteLine(
                "Ошибка " + errorCode +
                " (" + GetErrorText(errorCode) + ")" +
                " Строка " + position.LineNumber +
                ", Символ " + position.CharNumber);
        }

        public static string GetErrorText(byte errorCode)
        {
            return _errorTable.ContainsKey(errorCode) ? _errorTable[errorCode] : "Неизвестная ошибка";
        }

        public static void PrintErrors()
        {
            if (_errors.Count == 0)
            {
                Console.WriteLine("Ошибок не обнаружено.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("=== СПИСОК ОШИБОК ===");

            foreach (Err e in _errors)
            {
                Console.WriteLine(
                    "Код " + e.ErrorCode +
                    " : " + GetErrorText(e.ErrorCode));
            }
        }
    }
}