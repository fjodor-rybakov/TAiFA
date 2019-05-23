using System.Collections.Generic;
using Compiler.Lexer;

namespace Compiler.RecDown
{
    public class RecDown
    {
        private readonly List<LexerInfo> _lexerInfo;
        private static int _lastListIndex = 3;

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
            return _lastListIndex < _lexerInfo.Count && _lexerInfo[0].Value == TypeLexem.VAR;
        }
        
        private bool IsType()
        {
            var value = _lexerInfo[1].Value;
            return _lastListIndex  < _lexerInfo.Count && (value == TypeLexem.INT || value == TypeLexem.STRING || value == TypeLexem.BOOL);
        }

        private bool IsColon()
        {
            return _lastListIndex < _lexerInfo.Count && _lexerInfo[2].Value == TypeLexem.COLON;
        }

        private bool IsSemicolon()
        {
            return _lastListIndex < _lexerInfo.Count && _lexerInfo[_lastListIndex].Value == TypeLexem.SEMICOLON;
        }

        private bool IsIdList()
        {
            return _lastListIndex < _lexerInfo.Count && IsList();
        }

        private bool IsList()
        {
            if (_lexerInfo[_lastListIndex].Type != TypeLexem.IDENTIFICATOR) return false;
            _lastListIndex++;
            if (_lastListIndex >= _lexerInfo.Count || _lexerInfo[_lastListIndex].Value != TypeLexem.COMMA) return true;
            _lastListIndex++;
            return IsList();
        }
    }
}