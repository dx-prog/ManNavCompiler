using System.Text.RegularExpressions;

namespace ManNav.Parser {
    public class TokenRule {
        /// <summary>
        ///     Regular expression signature
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        ///     Regex options
        /// </summary>
        public string MatchOptions { get; set; }

        public Match DoMatch(string input, int startingPosition) {
            var regEx = GetSignatureRegex();
            return regEx.Match(input, startingPosition);
        }

        public Regex GetSignatureRegex() {
            return RegexCache.Get(Signature, MatchOptions);
        }
    }
}