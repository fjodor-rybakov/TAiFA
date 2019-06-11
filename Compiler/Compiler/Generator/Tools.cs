namespace Compiler.Generator
{
    public class Tools
    {
        public static bool IsTerminal(string value)
        {
            return value[0] != '<' && value[value.Length - 1] != '>';
        }
    }
}