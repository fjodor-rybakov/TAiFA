using Compiler.MSILGenerator.Resources;
using Compiler.MSILGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler.MSILGenerator.MSILLanguage.Constructions.Functions
{
    public class MainFunction : IMSILConstruction
    {
        private List<IMSILConstruction> _functionBody;

        public MainFunction( List<IMSILConstruction> functionBody )
        {
            _functionBody = functionBody;
        }

        public string ToMSILCode()
        {
            string funcBodyMSIL = "";
            foreach ( var item in _functionBody )
            {
                funcBodyMSIL += item.ToMSILCode();
            }

            var commandCode = ResourceManager.GetMainFunctionResource();
            return commandCode.Replace( Constants.RESOURCE_FUNCTION_BODY, funcBodyMSIL );
        }
    }
}
