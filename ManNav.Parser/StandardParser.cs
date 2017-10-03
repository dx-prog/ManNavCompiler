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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ManNav.Parser {
    public class StandardParser {
        private static readonly XName PrefixNode = XName.Get("prefix", "http://signature");
        private readonly ConcurrentBag<TokenDefinition> _tokens = new ConcurrentBag<TokenDefinition>();
        private TokenDefinition[] _sortedOrder;

        public void LoadScript(string scriptText) {
            var text = XDocument.Parse(scriptText);

            var counter = new Counter();
            foreach (var n in text.Elements())
            foreach (var entry in ProcessNode(n, counter))
                _tokens.Add(entry);
        }

        /// <summary>
        ///     Registers a token with the parser
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public StandardParser RegisterToken(TokenDefinition token) {
            _tokens.Add(token);
            return this;
        }

        public StandardParser Initialize() {
            if (_tokens.IsEmpty)
                throw new InvalidOperationException();

            _sortedOrder = _tokens.OrderBy(x => x.ParsingPriority).ToArray();
            return this;
        }

        public SymbolicToken[] GetTokens(string input) {
            var tokens = new List<SymbolicToken>();


            for (var i = 0; i < input.Length;) {
                IEnumerable<SymbolicToken> matchedTokens = null;
                Match match = null;

                foreach (var token in _sortedOrder) {
                    var m = token.DoMatch(input, i);
                    if (!m.Success)
                        continue;

                    var matchedTokensSucess = TestAgainstCaptureLogic(token, m, input);
                    if (null == matchedTokensSucess)
                        continue;

                    matchedTokens = matchedTokensSucess;
                    match = m;
                    break;
                }

                if (match == null) {
                    tokens.Add(new SymbolicToken(null, null, null, i));
                    i++;
                }
                else {
                    foreach (var token in matchedTokens) {
                        tokens.Add(token);
                        i += token.Match.Length;
                    }
                }
            }

            return tokens.ToArray();
        }

        private static IEnumerable<TokenDefinition> ProcessNode(XElement n, Counter counter) {
            var parent = n.Parent;
            var prefix = "";
            if (null != parent)
                prefix = parent.Attribute(PrefixNode)?.Value;

            if (n.Name == "define") {
                var t = new TokenDefinition {
                    ExecutionPriority = int.Parse(n.Attribute("priority")?.Value ?? "0"),
                    ParsingPriority = counter.Increment(),
                    Name = n.Attribute("name")?.Value,
                    Signature = prefix + n.Attribute("signature")?.Value,
                    MatchOptions = n.Attribute("options")?.Value,
                    Type = Enum.Parse<TokenType>(n.Attribute("type")?.Value ?? "operator", true)
                };

                foreach (var e in n.Elements())
                    t.Rules.AddRange(ProcessNode(e, counter));

                yield return t;
            }
            else {
                // ReSharper disable once TailRecursiveCall
                foreach (var e in n.Elements())
                foreach (var child in ProcessNode(e, counter))

                    yield return child;
            }
        }

        /// <summary>
        ///     Regex from signature pattern may have capture groups that tell if a match is
        ///     bad
        /// </summary>
        /// <param name="token"></param>
        /// <param name="m"></param>
        /// <param name="original">the original search string</param>
        /// <returns></returns>
        private static IEnumerable<SymbolicToken> TestAgainstCaptureLogic(
            TokenDefinition token,
            Match m,
            string original) {
            var ret = new List<SymbolicToken>();
            ret.Add(new SymbolicToken(m.Value, token, m, m.Index));

            var floatingOffset = 0;
            foreach (var rule in token.Rules) {
                //if (!string.IsNullOrWhiteSpace(rule.Name) && m.Groups[rule.Name].Success == false)
                //    yield return new SymbolicToken(m.Value, token, m);
                var subMatch = rule.DoMatch(original, floatingOffset + m.Index + m.Length);
                if (subMatch.Success) {
                    ret.Add(new SymbolicToken(subMatch.Value, rule, subMatch, subMatch.Index));
                    floatingOffset += subMatch.Length;
                }
            }

            return ret.OrderBy(x => x.Match.Index);
        }

        private class Counter {
            private int Value { get; set; }

            public int Increment() {
                return Value++;
            }
        }
    }
}