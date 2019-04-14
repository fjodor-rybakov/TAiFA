namespace Compiler.Lexer1
{
    public static class TypeLexem
    {
        private const string IDENTIFICATOR = "IDENTIFICATOR";
        private const string NUMBER2 = "NUMBER2";
        private const string NUMBER8 = "NUMBER8";
        private const string NUMBER10 = "NUMBER10";
        private const string NUMBER16 = "NUMBER16";
        private const string DECIMAL = "DECIMAL";
        private const string UNKNOWN = "UNKNOWN";
        public const string OPERATION = "OPERATION";
        public const string DELIMITER = "DELIMITER";

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