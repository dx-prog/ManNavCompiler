﻿/***************************************************************
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
using System.Xml.Linq;
using ManNav.Parser;

namespace ManNav {
    internal class Program {
        private static void Main(string[] args) {
            var parser = new StandardParser();
            parser.LoadScript(XDocument.Load(".\\vramscript.xml").ToString());

            foreach (var token in parser.Initialize().GetTokens("( ( 11.1+ 17.5++) * 23) / --4"))
                Console.WriteLine("{0}[{1}]={2}", token.Name, token.Match?.Index, token.Text);
        }
    }
}