using Compiler.MSILGenerator.Resources;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class SmallerOperation : IMSILConstruction
    {
        public string ToMSILCode()
        {
            return ResourceManager.GetSmallerOperationResource();
        }
    }
}
