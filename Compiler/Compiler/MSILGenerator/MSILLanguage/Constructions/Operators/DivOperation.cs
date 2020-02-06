using Compiler.MSILGenerator.Resources;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class DivOperation : ArithmeticOperation
    {
        public override string ToMSILCode()
        {
            return GetInputParam() + ResourceManager.GetDivOperationResource();
        }
    }
}
