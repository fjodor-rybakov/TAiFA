namespace Compiler.Lexer
{
    public class LexerInfo
    {
        public string Type { get; }
        public string Value { get; }
        public bool IsReserve { get; }
        
        public LexerInfo(string value, string type, bool isReserve)
        {
            Type = type;
            Value = value;
            IsReserve = isReserve;
        }
    }
}