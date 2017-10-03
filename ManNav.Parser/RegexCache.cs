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
using System.Threading;

namespace ManNav.Parser {
    public class RegexCache {
        private static readonly ConcurrentDictionary<string, Regex> _cache = new ConcurrentDictionary<string, Regex>();

        public static RegexOptions GetOptions(string regexOptions) {
            var options = StandardVargSplit(regexOptions)
                .Select(m => !Enum.TryParse(m, true, out RegexOptions opt) ? RegexOptions.None : opt);


            return options.Aggregate(RegexOptions.None, (current, entry) => current | entry);
        }

        public static IEnumerable<string> StandardVargSplit(string input) {
            var options = (input ?? "").Split(',', ';', ' ', '|')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                ;
            return options;
        }

        public static Regex Get(string pattern, string options) {
            return Get(pattern, GetOptions(options));
        }

        public static Regex Get(string pattern, RegexOptions options) {
            var key = CreateKey(pattern, options);
            if (_cache.TryGetValue(key, out var regEx))
                if (regEx != null)
                    return regEx;


            ThreadPool.QueueUserWorkItem(s => { _cache[key] = new Regex(pattern, options | RegexOptions.Compiled); });

            return new Regex(pattern, options);
        }

        private static string CreateKey(string pattern, RegexOptions options) {
            return options + "\r\n" + pattern;
        }
    }
}