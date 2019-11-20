namespace Compiler.Lexer
{
    public static class TypeLexem
    {
        public const string IDENTIFICATOR = "identifier";
        public const string NUMBER2 = "NUMBER2";
        public const string NUMBER8 = "NUMBER8";
        public const string NUMBER10 = "integer";
        public const string NUMBER16 = "NUMBER16";
        public const string DECIMAL = "decimal";
        public const string UNKNOWN = "unknwon";
        public const string OPERATION = "operator";
        public const string DELIMITER = "delimetrs";

        public const string INT = "int";
        public const string STRING = "string";
        public const string BOOL = "bool";
        public const string SEMICOLON = ";";
        public const string COMMA = ",";
        public const string COLON = ":";
        public const string VAR = "var";
        

        public static string GetToken(AutomateData automateData, int lastState)
        {
            const int finishStateNumber2 = 4;
            const int finishStateNumber8 = 6;
            const int finishStateNumber16 = 8;
            const int finishStateNumberOne = 2;
            const int finishStateNumberTwo = 4;
            const int finishStateNumberThree = 7;
            const int finishStateNumberFour = 9;
            switch (automateData.Type)
            {
                case "identificator":
                    return IDENTIFICATOR;
                case "number10" when lastState == finishStateNumberOne:
                    return NUMBER10;
                case "number10" when lastState == finishStateNumberTwo:
                    return DECIMAL;
                case "number10" when lastState == finishStateNumberThree:
                    return DECIMAL;
                case "number10" when lastState == finishStateNumberFour:
                    return NUMBER10;
                case "number2816" when lastState == finishStateNumber2:
                    return NUMBER2;
                case "number2816" when lastState == finishStateNumber8:
                    return NUMBER8;
                case "number2816" when lastState == finishStateNumber16:
                    return NUMBER16;
                default:
                    return UNKNOWN;
            }
        }
    }
}