﻿using Compiler.MSILGenerator.MSILLanguage.Constructions.Utils;
using Compiler.MSILGenerator.Resources;
using Compiler.MSILGenerator.Utils;
using System.Collections.Generic;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class VariableDeclarationOperator : IMSILConstruction
    {
        private List<string> _names;
        private VariableType _type;

        public VariableDeclarationOperator( List<string> names, VariableType type )
        {
            _names = names;
            _type = type;
        }

        public string ToMSILCode()
        {
            var commandCode = ResourceManager.GetVariableDeclarationOperatorResouce();
            var commandBody = "";
            foreach ( var name in _names )
            {
                commandBody += $"{VariableTypeHelper.GetMSILRepresentation( _type )} {name},";
            }
            commandBody = commandBody.Remove( commandBody.Length - 1 );
            return commandCode.Replace( Constants.RESOURCE_FUNCTION_BODY, commandBody );
        }
    }
}
