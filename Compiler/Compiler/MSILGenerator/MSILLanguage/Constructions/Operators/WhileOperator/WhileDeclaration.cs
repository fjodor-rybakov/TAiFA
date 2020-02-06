using Compiler.MSILGenerator.Resources;
using Compiler.MSILGenerator.Utils;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators.WhileOperator
{
    public class WhileDeclaration : IMSILConstruction
    {
        public string Metka { get; private set; }

        public WhileDeclaration( string metka )
        {
            Metka = metka;
        }

        public string ToMSILCode()
        {
            return ResourceManager.GetDeclareMetkaResource().Replace( Constants.RESOURCE_METKA, Metka );
        }
    }
}
