using Compiler.MSILGenerator.Resources;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class SubOperation : ArithmeticOperation
    {
        public override string ToMSILCode()
        {
            return GetInputParam() + ResourceManager.GetSubOperationResource(); 
        }
    }
}
