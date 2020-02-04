using Compiler.MSILGenerator.Resources;
using Compiler.MSILGenerator.Utils;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators.IfOperator
{
    public class EndIf : IMSILConstruction
    {
        public string Metka { get; private set; }
        private bool _isIfElse;

        public EndIf( string metka, bool isIfElse = false )
        {
            Metka = metka;
            _isIfElse = isIfElse;
        }

        public string ToMSILCode()
        {
            return _isIfElse ? ResourceManager.GetGotoOperationResource().Replace( Constants.RESOURCE_METKA, Metka ) 
                : ResourceManager.GetDeclareMetkaResource().Replace( Constants.RESOURCE_METKA, Metka );
        }
    }
}
