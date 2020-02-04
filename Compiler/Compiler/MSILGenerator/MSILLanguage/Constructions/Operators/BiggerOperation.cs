using Compiler.MSILGenerator.Resources;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class BiggerOperation : IMSILConstruction
    {
        public string ToMSILCode()
        {
            return ResourceManager.GetBiggerOperationResource();
        }
    }
}
