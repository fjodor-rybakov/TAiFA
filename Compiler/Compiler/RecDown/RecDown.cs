using System.Collections.Generic;
using Compiler.Lexer1;

namespace Compiler.RecDown
{
    public class RecDown
    {
        private List<LexerInfo> _lexerInfo;

        public RecDown(List<LexerInfo> lexerInfo)
        {
            _lexerInfo = lexerInfo;
        }
        
        public bool CheckVar()
        {
            var lastListIndex = 3;
            return IsVar(_lexerInfo[0].Value) && 
                   IsType(_lexerInfo[1].Value) &&
                   _lexerInfo[2].Value == TypeLexem.COLON && 
                   IsIdList(ref lastListIndex) && 
                   _lexerInfo[lastListIndex].Value == TypeLexem.SEMICOLON;
        }

        private bool IsVar(string value)
        {
            return value == TypeLexem.VAR;
        }
        
        private bool IsType(string value)
        {
            return value == TypeLexem.INT || value == TypeLexem.STRING || value == TypeLexem.BOOL;
        }

        private bool IsIdList(ref int index)
        {
            return _lexerInfo[index] != null && IsList(ref index);
        }

        private bool IsList(ref int index)
        {
            if (_lexerInfo[index].Type != TypeLexem.IDENTIFICATOR) return false;
            index++;
            if (index >= _lexerInfo.Count || _lexerInfo[index].Value != TypeLexem.COMMA) return true;
            index++;
            return IsList(ref index);
        }
    }
}