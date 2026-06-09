using System.Collections.Generic;

namespace Компилятор
{
    class Keywords
    {
        private Dictionary<string, byte> _kw;

        public Keywords()
        {
            _kw = new Dictionary<string, byte>();

            _kw["program"] = LexicalAnalyzer.programsy;

            _kw["var"] = LexicalAnalyzer.varsy;
            _kw["begin"] = LexicalAnalyzer.beginsy;
            _kw["end"] = LexicalAnalyzer.endsy;

            _kw["if"] = LexicalAnalyzer.ifsy;
            _kw["then"] = LexicalAnalyzer.thensy;
            _kw["else"] = LexicalAnalyzer.elsesy;

            _kw["while"] = LexicalAnalyzer.whilesy;
            _kw["do"] = LexicalAnalyzer.dosy;

            _kw["repeat"] = LexicalAnalyzer.repeatsy;
            _kw["until"] = LexicalAnalyzer.untilsy;

            _kw["for"] = LexicalAnalyzer.forsy;
            _kw["to"] = LexicalAnalyzer.tosy;

            
            _kw["array"] = LexicalAnalyzer.arraysy;
            _kw["of"] = LexicalAnalyzer.ofsy;

            
            _kw["integer"] = LexicalAnalyzer.integersy;
            _kw["real"] = LexicalAnalyzer.realsy;
            _kw["char"] = LexicalAnalyzer.charsy;
            _kw["boolean"] = LexicalAnalyzer.booleansy;
        }

        public bool IsKeyword(string word)
        {
            return _kw.ContainsKey(word.ToLower());
        }

        public byte GetCode(string word)
        {
            word = word.ToLower();

            if (_kw.ContainsKey(word))
                return _kw[word];

            return 0;
        }
    }
}