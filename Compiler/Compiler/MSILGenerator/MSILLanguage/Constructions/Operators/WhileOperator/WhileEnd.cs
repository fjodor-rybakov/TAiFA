using Compiler.MSILGenerator.Resources;
using Compiler.MSILGenerator.Utils;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators.WhileOperator
{
    public class WhileEnd : IMSILConstruction
    {
        public string StartMetka { get; private set; }
        public string FinishMetka { get; private set; }

        public WhileEnd( string startMetka, string finishMetka )
        {
            StartMetka = startMetka;
            FinishMetka = finishMetka;
        }

        public string ToMSILCode()
        {
            return ResourceManager.GetGotoOperationResource().Replace( Constants.RESOURCE_METKA, StartMetka ) +
                  ResourceManager.GetDeclareMetkaResource().Replace( Constants.RESOURCE_METKA, FinishMetka );
        }
    }
}
