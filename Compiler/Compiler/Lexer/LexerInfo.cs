namespace Compiler.Lexer
{
    public class LexerInfo
    {
        public string Type { get; }
        public string Value { get; }
        public bool IsReserve { get; }
        public int LineNumber { get; }
        public int PosNumber { get; }
        
        public LexerInfo(string value, string type, bool isReserve, int lineNumber, int posNumber)
        {
            Type = type;
            Value = value;
            IsReserve = isReserve;
            LineNumber = lineNumber;
            PosNumber = posNumber;
        }
    }
}