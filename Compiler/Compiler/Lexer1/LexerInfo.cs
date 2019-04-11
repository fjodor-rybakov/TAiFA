namespace Compiler.Lexer1
{
    public class LexerInfo
    {
        public string Type { get; }
        public string Value { get; }
        
        public LexerInfo(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}