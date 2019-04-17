using System.Collections.Generic;
using Compiler.Lexer1;

namespace Compiler.RecDown
{
    public class RecDown
    {
        private List<LexerInfo> _lexerInfo;
        private static int lastListIndex = 3;

        public RecDown(List<LexerInfo> lexerInfo)
        {
            _lexerInfo = lexerInfo;
        }
        
        public bool CheckVar()
        {
            return IsVar() && IsType() && IsColon() && IsIdList() && IsSemicolon();
        }

        private bool IsVar()
        {
            return lastListIndex < _lexerInfo.Count && _lexerInfo[0].Value == TypeLexem.VAR;
        }
        
        private bool IsType()
        {
            string value = _lexerInfo[1].Value;
            return lastListIndex  < _lexerInfo.Count && (value == TypeLexem.INT || value == TypeLexem.STRING || value == TypeLexem.BOOL);
        }

        private bool IsColon()
        {
            return lastListIndex < _lexerInfo.Count && _lexerInfo[2].Value == TypeLexem.COLON;
        }

        private bool IsSemicolon()
        {
            return lastListIndex < _lexerInfo.Count && _lexerInfo[lastListIndex].Value == TypeLexem.SEMICOLON;
        }

        private bool IsIdList()
        {
            return lastListIndex < _lexerInfo.Count && IsList();
        }

        private bool IsList()
        {
            if (_lexerInfo[lastListIndex].Type != TypeLexem.IDENTIFICATOR) return false;
            lastListIndex++;
            if (lastListIndex >= _lexerInfo.Count || _lexerInfo[lastListIndex].Value != TypeLexem.COMMA) return true;
            lastListIndex++;
            return IsList();
        }
    }
}