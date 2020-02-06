using Compiler.MSILGenerator.MSILLanguage.Constructions.Utils;

namespace Compiler.MSILGenerator.Utils
{
    public static class VariableTypeHelper
    {
        public static string GetMSILRepresentation( VariableType type )
        {
            switch ( type )
            {
                case VariableType.Bool:
                    return "int32";
                case VariableType.String:
                    return "string";
                case VariableType.Integer:
                    return "int32";
                default:
                    return "";
            }
        }
    }
}
