using Compiler.MSILGenerator.Resources;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class ComparisonOperator : BoolOpeartion
    {
        public override string ToMSILCode()
        {
            return GetInputParam() + ResourceManager.GetCeqOperationResource();
        }
    }
}
