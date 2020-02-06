using Compiler.MSILGenerator.Resources;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Utils
{
    public class Initializator : IMSILConstruction
    {
        public string ToMSILCode()
        {
            return ResourceManager.GetInitializeResource();
        }
    }
}
