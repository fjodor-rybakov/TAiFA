using Compiler.Enums;

namespace Compiler.InsertActionsInSyntax
{
    public interface IVariablesTableController
    {
        void CreateTable();
        void DestroyLastTable();
        void DefineNewType( Term type );
        void DefineIdentifier( Term identifier );
        Variable GetVariable( int id );
    }
}
