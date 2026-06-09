using System;

namespace Компилятор
{
    class SyntaxAnalyzer
    {
        private LexicalAnalyzer _lexer;
        private byte Sym => _lexer.Symbol;

        public SyntaxAnalyzer()
        {
            _lexer = new LexicalAnalyzer();
            _lexer.NextSym();
        }

        private void Next()
        {
            _lexer.NextSym();
        }

        private void Expect(byte expected, byte errorCode)
        {
            if (Sym == expected)
            {
                Next();
                return;
            }

            InputOutput.Error(errorCode, InputOutput.PositionNow);

           
            if (Sym != LexicalAnalyzer.semicolon &&
                Sym != LexicalAnalyzer.endsy &&
                Sym != 0)
            {
                Next();
            }
        }

        private void RecoveryToStatementBoundary()
        {
            while (Sym != 0 &&
                   Sym != LexicalAnalyzer.semicolon &&
                   Sym != LexicalAnalyzer.endsy &&
                   Sym != LexicalAnalyzer.beginsy &&
                   Sym != LexicalAnalyzer.ident &&
                   Sym != LexicalAnalyzer.ifsy &&
                   Sym != LexicalAnalyzer.whilesy &&
                   Sym != LexicalAnalyzer.forsy &&
                   Sym != LexicalAnalyzer.repeatsy)
            {
                Next();
            }
        }

        public void ParseProgram()
        {
            ProgramHeader();
            VariableSection();
            CompoundStatement();
            Expect(LexicalAnalyzer.point, 53);
        }

        private void ProgramHeader()
        {
            if (Sym == LexicalAnalyzer.programsy)
                Next();
            else
                InputOutput.Error(50, InputOutput.PositionNow);

            Expect(LexicalAnalyzer.ident, 51);
            Expect(LexicalAnalyzer.semicolon, 52);
        }

        private void VariableSection()
        {
            if (Sym != LexicalAnalyzer.varsy) return;

            Next();

            while (Sym == LexicalAnalyzer.ident)
                VariableDeclaration();
        }

        private void VariableDeclaration()
        {
            IdentifierList();
            Expect(LexicalAnalyzer.colon, 54);
            TypeDescription();
            Expect(LexicalAnalyzer.semicolon, 52);
        }

        private void IdentifierList()
        {
            Expect(LexicalAnalyzer.ident, 51);

            while (Sym == LexicalAnalyzer.comma)
            {
                Next();
                Expect(LexicalAnalyzer.ident, 51);
            }
        }

        private void TypeDescription()
        {
            switch (Sym)
            {
                case LexicalAnalyzer.integersy:
                case LexicalAnalyzer.realsy:
                case LexicalAnalyzer.charsy:
                case LexicalAnalyzer.booleansy:
                    Next();
                    break;

                case LexicalAnalyzer.arraysy:
                    ArrayDescription();
                    break;

                default:
                    InputOutput.Error(61, InputOutput.PositionNow);
                    RecoveryToStatementBoundary();
                    break;
            }
        }

        private void ArrayDescription()
        {
            Expect(LexicalAnalyzer.arraysy, 61);
            Expect(LexicalAnalyzer.lbracket, 56);

            if (Sym == LexicalAnalyzer.intc)
                Next();
            else
            {
                InputOutput.Error(50, InputOutput.PositionNow);
                RecoveryToStatementBoundary();
            }

            Expect(LexicalAnalyzer.rbracket, 57);
            Expect(LexicalAnalyzer.ofsy, 62);
            TypeDescription();
        }

        private void CompoundStatement()
        {
            Expect(LexicalAnalyzer.beginsy, 50);

            while (Sym != LexicalAnalyzer.endsy && Sym != 0)
            {
                Statement();

                if (Sym == LexicalAnalyzer.semicolon)
                    Next();
                else
                    RecoveryToStatementBoundary();
            }

            Expect(LexicalAnalyzer.endsy, 63);
        }

        private void Statement()
        {
            switch (Sym)
            {
                case LexicalAnalyzer.ident:
                    Assignment();
                    break;

                case LexicalAnalyzer.ifsy:
                    IfStatement();
                    break;

                case LexicalAnalyzer.whilesy:
                    WhileStatement();
                    break;

                case LexicalAnalyzer.repeatsy:
                    RepeatStatement();
                    break;

                case LexicalAnalyzer.forsy:
                    ForStatement();
                    break;

                case LexicalAnalyzer.beginsy:
                    CompoundStatement();
                    break;

                default:
                    InputOutput.Error(50, InputOutput.PositionNow);
                    RecoveryToStatementBoundary();   
                    break;
            }
        }

        private void Assignment()
        {
            Variable();
            Expect(LexicalAnalyzer.assign, 58);
            Expression();
        }

        private void Variable()
        {
            Expect(LexicalAnalyzer.ident, 51);

            if (Sym == LexicalAnalyzer.lbracket)
            {
                Next();
                Expression();
                Expect(LexicalAnalyzer.rbracket, 57);
            }
        }

       
        private void Expression()
        {
            SimpleExpression();

            if (Sym == LexicalAnalyzer.equal ||
                Sym == LexicalAnalyzer.less ||
                Sym == LexicalAnalyzer.greater ||
                Sym == LexicalAnalyzer.lessequal ||
                Sym == LexicalAnalyzer.greaterequal ||
                Sym == LexicalAnalyzer.notequal)
            {
                Next();
                SimpleExpression();
            }
        }

        private void SimpleExpression()
        {
            Term();

            while (Sym == LexicalAnalyzer.plus || Sym == LexicalAnalyzer.minus)
            {
                Next();
                Term();
            }
        }

        private void Term()
        {
            Factor();

            while (Sym == LexicalAnalyzer.star || Sym == LexicalAnalyzer.slash)
            {
                Next();
                Factor();
            }
        }

        
        private void Factor()
        {
            switch (Sym)
            {
                case LexicalAnalyzer.ident:
                    Variable();
                    break;

                case LexicalAnalyzer.intc:
                    Next();
                    break;

                case LexicalAnalyzer.leftpar:
                    Next();
                    Expression();
                    Expect(LexicalAnalyzer.rightpar, 57);
                    break;

                default:
                    InputOutput.Error(50, InputOutput.PositionNow);
                    RecoveryToStatementBoundary(); 
                    break;
            }
        }

        private void IfStatement()
        {
            Expect(LexicalAnalyzer.ifsy, 50);
            Expression();
            Expect(LexicalAnalyzer.thensy, 59);
            Statement();

            if (Sym == LexicalAnalyzer.elsesy)
            {
                Next();
                Statement();
            }
        }

        private void WhileStatement()
        {
            Expect(LexicalAnalyzer.whilesy, 50);
            Expression();
            Expect(LexicalAnalyzer.dosy, 60);
            Statement();
        }

        private void RepeatStatement()
        {
            Expect(LexicalAnalyzer.repeatsy, 50);

            while (Sym != LexicalAnalyzer.untilsy && Sym != 0)
            {
                Statement();

                if (Sym == LexicalAnalyzer.semicolon)
                    Next();
                else
                    RecoveryToStatementBoundary();
            }

            Expect(LexicalAnalyzer.untilsy, 64);
            Expression();
        }

        private void ForStatement()
        {
            Expect(LexicalAnalyzer.forsy, 50);
            Variable();
            Expect(LexicalAnalyzer.assign, 58);
            Expression();
            Expect(LexicalAnalyzer.tosy, 50);
            Expression();
            Expect(LexicalAnalyzer.dosy, 60);
            Statement();
        }
    }
}