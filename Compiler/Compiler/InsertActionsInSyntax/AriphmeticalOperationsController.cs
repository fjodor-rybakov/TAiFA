using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Enums;

namespace Compiler.InsertActionsInSyntax
{
    public class AriphmeticalOperationsController
    {
        public List<TermType> OperandTypes { get; } = new List<TermType>();

        public void AddNewNumber( Term number )
        {
            TermType type = GetVariableType( number.Type );

            OperandTypes.Add( type );

            bool numbersIsUnique = OperandTypes.Distinct().Count() == 1;

            if ( !numbersIsUnique )
                throw new ApplicationException( $"Not all number types are equal. Number:{number.Value} in row {number.RowPosition}." );
        }

        public void AddNewVariable( Variable variable, Term currentTerm )
        {
            OperandTypes.Add( variable.Type.Type );

            bool numbersIsUnique = OperandTypes.Distinct().Count() == 1;

            if ( !numbersIsUnique )
                throw new ApplicationException( $"Not all variable types are equal. Number:{variable.Identifier.Value} in row {currentTerm.RowPosition}." );
        }

        private TermType GetVariableType( TermType termType )
        {
            switch ( termType )
            {
                case Compiler.Enums.TermType.BinaryWholeNumber:
                case Compiler.Enums.TermType.DecimalWholeNumber:
                    return Compiler.Enums.TermType.Int;
                case TermType.DecimalFixedPointNumber:
                    return TermType.Double;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Clear()
        {
            OperandTypes.Clear();
        }
    }
}
