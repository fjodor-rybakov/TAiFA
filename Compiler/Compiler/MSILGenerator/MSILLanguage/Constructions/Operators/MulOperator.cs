using Compiler.MSILGenerator.Resources;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class MulOperator : ArithmeticOperation
    {
        public override string ToMSILCode()
        {
            return $"{GetInputParam()}{ResourceManager.GetMulOperationResource()}";
        }
    }
}
