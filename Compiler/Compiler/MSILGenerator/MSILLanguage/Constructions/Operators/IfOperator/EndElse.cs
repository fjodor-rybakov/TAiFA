using Compiler.MSILGenerator.Resources;
using Compiler.MSILGenerator.Utils;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators.IfOperator
{
    public class EndElse : IMSILConstruction
    {
        public string Metka { get; private set; }

        public EndElse( string metka )
        {
            Metka = metka;
        }

        public string ToMSILCode()
        {
            return ResourceManager.GetDeclareMetkaResource().Replace( Constants.RESOURCE_METKA, Metka );
        }
    }
}
