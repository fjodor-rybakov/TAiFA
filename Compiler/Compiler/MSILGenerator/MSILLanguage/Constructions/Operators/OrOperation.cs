using Compiler.MSILGenerator.Resources;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class OrOperation : BoolOpeartion
    {
        public override string ToMSILCode()
        {
            return GetInputParam() + ResourceManager.GetOrOperationResource();
        }
    }
}
