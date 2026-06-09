using System;

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

        public const byte integersy = 130;
        public const byte realsy = 131;
        public const byte charsy = 132;
        public const byte booleansy = 133;

        private byte _symbol;
        private TextPosition _token;
        private string _addrName;
        private int _nmbInt;

        private Keywords _keywords;

        public LexicalAnalyzer()
        {
            _symbol = 0;
            _token = new TextPosition();
            _addrName = "";
            _nmbInt = 0;
            _keywords = new Keywords();
        }

        public byte Symbol => _symbol;
        public TextPosition Token => _token;
        public string AddrName => _addrName;
        public int NmbInt => _nmbInt;

        private bool IsLetter(char ch) => char.IsLetter(ch) || ch == '_';
        private bool IsDigit(char ch) => char.IsDigit(ch);

        public byte NextSym()
        {
            SkipWhitespace();

            _token = InputOutput.PositionNow;
            char ch = InputOutput.Ch;

            if (ch == '\0')
            {
                _symbol = 0;
                return 0;
            }

            if (IsLetter(ch))
                return ScanWord();

            if (IsDigit(ch))
                return ScanNumber();

            return ScanSymbol(ch);
        }

        private void SkipWhitespace()
        {
            while (InputOutput.Ch == ' ' || InputOutput.Ch == '\t')
                InputOutput.NextCh();
        }

        private byte ScanWord()
        {
            string word = "";
            while (IsLetter(InputOutput.Ch) || IsDigit(InputOutput.Ch))
            {
                word += InputOutput.Ch;
                InputOutput.NextCh();
            }

            if (_keywords.IsKeyword(word))
            {
                _symbol = _keywords.GetCode(word);
            }
            else
            {
                _symbol = ident;
                _addrName = word;
            }
            return _symbol;
        }

        private byte ScanNumber()
        {
            string number = "";
            while (IsDigit(InputOutput.Ch))
            {
                number += InputOutput.Ch;
                InputOutput.NextCh();
            }

            try
            {
                _nmbInt = Convert.ToInt32(number);
            }
            catch
            {
                _nmbInt = 0;
                InputOutput.Error(200, InputOutput.PositionNow); 
            }

            _symbol = intc;
            return _symbol;
        }

        private byte ScanSymbol(char ch)
        {
            switch (ch)
            {
                case '+': _symbol = plus; break;
                case '-': _symbol = minus; break;
                case '*': _symbol = star; break;
                case '/': _symbol = slash; break;

                case '=': _symbol = equal; break;
                case ';': _symbol = semicolon; break;
                case ',': _symbol = comma; break;

                case '(': _symbol = leftpar; break;
                case ')': _symbol = rightpar; break;

                case '[': _symbol = lbracket; break;
                case ']': _symbol = rbracket; break;

                case '.': _symbol = point; break;

                case ':':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        _symbol = assign;
                        InputOutput.NextCh();
                        return _symbol;
                    }
                    else
                        _symbol = colon;
                    return _symbol;

                case '<':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=') { _symbol = lessequal; InputOutput.NextCh(); }
                    else if (InputOutput.Ch == '>') { _symbol = notequal; InputOutput.NextCh(); }
                    else _symbol = less;
                    return _symbol;

                case '>':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=') { _symbol = greaterequal; InputOutput.NextCh(); }
                    else _symbol = greater;
                    return _symbol;

                default:
                    InputOutput.Error(50, InputOutput.PositionNow);
                    InputOutput.NextCh();
                    return NextSym(); 
            }

            InputOutput.NextCh();
            return _symbol;
        }
    }
}