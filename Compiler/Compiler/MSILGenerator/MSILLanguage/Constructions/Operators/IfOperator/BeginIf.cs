using Compiler.MSILGenerator.Resources;
using Compiler.MSILGenerator.Utils;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators.IfOperator
{
    public class BeginIf : IMSILConstruction
    {
        public string Metka { get; private set; }

        public BeginIf( string metka )
        {
            Metka = metka;
        }

        public string ToMSILCode()
        {
            return ResourceManager.GetBrFalseOperationResource().Replace( Constants.RESOURCE_METKA, Metka );
        }
    }
}
