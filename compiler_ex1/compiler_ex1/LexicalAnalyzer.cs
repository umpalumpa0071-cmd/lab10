using System;
using System.Collections.Generic;

namespace Компилятор
{
    class LexicalAnalyzer
    {
        public const byte ident = 2;
        public const byte intc = 15;

        public const byte semicolon = 14;
        public const byte comma = 20;
        public const byte colon = 5;
        public const byte point = 61;

        public const byte leftpar = 9;
        public const byte rightpar = 4;
        public const byte lbracket = 11;
        public const byte rbracket = 12;

        public const byte plus = 70;
        public const byte minus = 71;
        public const byte star = 21;
        public const byte slash = 60;

        public const byte equal = 16;
        public const byte less = 65;
        public const byte greater = 66;
        public const byte lessequal = 67;
        public const byte greaterequal = 68;
        public const byte notequal = 69;

        public const byte assign = 51;

        public const byte programsy = 122;
        public const byte varsy = 105;
        public const byte beginsy = 113;
        public const byte endsy = 104;
        public const byte ifsy = 56;
        public const byte thensy = 52;
        public const byte elsesy = 32;
        public const byte whilesy = 114;
        public const byte dosy = 54;
        public const byte repeatsy = 121;
        public const byte untilsy = 53;
        public const byte forsy = 109;
        public const byte tosy = 103;
        public const byte arraysy = 115;
        public const byte ofsy = 101;

        private byte _symbol;
        private TextPosition _token;
        private string _addrName;
        private int _nmbInt;

        private Dictionary<string, byte> _keywords;

        private Dictionary<uint, List<byte>> _codesByLine;

        public LexicalAnalyzer()
        {
            _symbol = 0;
            _token = new TextPosition();
            _addrName = "";
            _nmbInt = 0;

            _codesByLine =
                new Dictionary<uint, List<byte>>();

            InitKeywords();
        }

        public byte Symbol
        {
            get 
            {
                return _symbol; 
            }
        }

        public TextPosition Token
        {
            get 
            {
                return _token; 
            }
        }

        public string AddrName
        {
            get 
            {
                return _addrName; 
            }
        }

        public int NmbInt
        {
            get 
            {
                return _nmbInt; 
            }
        }

        private void InitKeywords()
        {
            _keywords =
                new Dictionary<string, byte>();

            _keywords["program"] = programsy;
            _keywords["var"] = varsy;
            _keywords["begin"] = beginsy;
            _keywords["end"] = endsy;

            _keywords["if"] = ifsy;
            _keywords["then"] = thensy;
            _keywords["else"] = elsesy;

            _keywords["while"] = whilesy;
            _keywords["do"] = dosy;

            _keywords["repeat"] = repeatsy;
            _keywords["until"] = untilsy;

            _keywords["for"] = forsy;
            _keywords["to"] = tosy;

            _keywords["array"] = arraysy;
            _keywords["of"] = ofsy;
        }

        private bool IsLetter(char ch)
        {
            return Char.IsLetter(ch) || ch == '_';
        }

        private bool IsDigit(char ch)
        {
            return Char.IsDigit(ch);
        }

        private void AddCode(byte code)
        {
            uint line =
                _token.LineNumber;

            if (!_codesByLine.ContainsKey(line))
            {
                _codesByLine[line] =
                    new List<byte>();
            }

            _codesByLine[line].Add(code);
        }

        public byte NextSym()
        {
            while (
                InputOutput.Ch == ' ' ||
                InputOutput.Ch == '\t')
            {
                InputOutput.NextCh();
            }

            _token.LineNumber =
                InputOutput.PositionNow.LineNumber;

            _token.CharNumber =
                InputOutput.PositionNow.CharNumber;

            char ch = InputOutput.Ch;

            if (ch == '\0')
            {
                _symbol = 0;
                return 0;
            }


            if (IsLetter(ch))
            {
                string name = "";

                while (
                    IsLetter(InputOutput.Ch) ||
                    IsDigit(InputOutput.Ch))
                {
                    name += InputOutput.Ch;
                    InputOutput.NextCh();
                }

                string lower =
                    name.ToLower();

                if (_keywords.ContainsKey(lower))
                {
                    _symbol =
                        _keywords[lower];
                }
                else
                {
                    _symbol = ident;
                    _addrName = name;
                }

                AddCode(_symbol);
                return _symbol;
            }


            if (IsDigit(ch))
            {
                _nmbInt = 0;

                while (IsDigit(InputOutput.Ch))
                {
                    int digit =
                        InputOutput.Ch - '0';

                    if (_nmbInt <= 3276)
                    {
                        _nmbInt =
                            _nmbInt * 10 + digit;
                    }
                    else
                    {
                        InputOutput.Error(
                            200,
                            InputOutput.PositionNow);
                    }

                    InputOutput.NextCh();
                }

                _symbol = intc;

                AddCode(_symbol);

                return _symbol;
            }


            switch (ch)
            {
                case '+':
                    _symbol = plus;
                    InputOutput.NextCh();
                    break;

                case '-':
                    _symbol = minus;
                    InputOutput.NextCh();
                    break;

                case '*':
                    _symbol = star;
                    InputOutput.NextCh();
                    break;

                case '/':
                    _symbol = slash;
                    InputOutput.NextCh();
                    break;

                case '=':
                    _symbol = equal;
                    InputOutput.NextCh();
                    break;

                case ';':
                    _symbol = semicolon;
                    InputOutput.NextCh();
                    break;

                case ',':
                    _symbol = comma;
                    InputOutput.NextCh();
                    break;

                case '(':
                    _symbol = leftpar;
                    InputOutput.NextCh();
                    break;

                case ')':
                    _symbol = rightpar;
                    InputOutput.NextCh();
                    break;

                case '[':
                    _symbol = lbracket;
                    InputOutput.NextCh();
                    break;

                case ']':
                    _symbol = rbracket;
                    InputOutput.NextCh();
                    break;

                case '.':
                    _symbol = point;
                    InputOutput.NextCh();
                    break;

                case ':':
                    InputOutput.NextCh();

                    if (InputOutput.Ch == '=')
                    {
                        _symbol = assign;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = colon;
                    }
                    break;

                case '<':
                    InputOutput.NextCh();

                    if (InputOutput.Ch == '=')
                    {
                        _symbol = lessequal;
                        InputOutput.NextCh();
                    }
                    else if (InputOutput.Ch == '>')
                    {
                        _symbol = notequal;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = less;
                    }

                    break;

                case '>':
                    InputOutput.NextCh();

                    if (InputOutput.Ch == '=')
                    {
                        _symbol = greaterequal;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = greater;
                    }

                    break;

                default:

                    InputOutput.Error(
                        50,
                        InputOutput.PositionNow);

                    InputOutput.NextCh();

                    return NextSym();
            }

            AddCode(_symbol);

            return _symbol;
        }

        public void PrintOutputCodesByLine()
        {
            Console.WriteLine();
            Console.WriteLine(
                "=== КОДЫ ЛЕКСЕМ ПО СТРОКАМ ===");
            Console.WriteLine();

            foreach (var item in _codesByLine)
            {
                Console.Write(
                    "Строка "
                    + item.Key
                    + ": ");

                foreach (byte code in item.Value)
                {
                    Console.Write(code + " ");
                }

                Console.WriteLine();
            }
        }
    }
}