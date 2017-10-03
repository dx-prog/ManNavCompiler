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

using System;
using System.Collections.Generic;

namespace ManNav.Parser {
    /// <summary>
    ///     A description of a token
    /// </summary>
    [Serializable]
    public class TokenDefinition : TokenRule, ITokenDefinition {
        public List<TokenDefinition> Rules { get; } = new List<TokenDefinition>();


        /// <summary>
        ///     A special name for the token such as "SCOPE_BEGIN" or "MATH"
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        ///     The processing priority for the parser
        /// </summary>

        public int? ParsingPriority { get; set; }

        /// <summary>
        ///     Infix to post fix order
        /// </summary>

        public int? ExecutionPriority { get; set; }

        /// <summary>
        ///     Token type
        /// </summary>
        public TokenType? Type { get; set; }
    }
}