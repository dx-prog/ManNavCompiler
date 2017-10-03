/***************************************************************
*  Copyright 2017 David Garica
*
*   Licensed under the Apache License, Version 2.0 (the "License");
*   you may not use this file except in compliance with the License.
*   You may obtain a copy of the License at
*
*       http://www.apache.org/licenses/LICENSE-2.0
*      
*   Unless required by applicable law or agreed to in writing, software
*   distributed under the License is distributed on an "AS IS" BASIS,
*   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*   See the License for the specific language governing permissions and
*   limitations under the License.
*
*   See Also:
* https://raw.githubusercontent.com/dx-prog/ManNavCompiler/master/LICENSE
* *************************************************************/

namespace ManNav.Parser {
    public enum TokenType {
        /// <summary>
        ///     The token is ignored
        /// </summary>
        Ignored,

        /// <summary>
        ///     The token can be compiled, and is an operator of some kind
        /// </summary>
        Operator,

        /// <summary>
        ///     The token is a variable or constant, and can be acted upon by an
        ///     operator
        /// </summary>
        Operand,

        /// <summary>
        ///     The token results in a generation of a complex set of instruction
        /// </summary>
        Keyword,

        /// <summary>
        ///     The token is to be interpreted by the compiler
        /// </summary>
        Preprocessor,

        /// <summary>
        ///     The token represents an instruction to the parser to changes
        ///     its behavior
        /// </summary>
        ParserInstruction
    }
}