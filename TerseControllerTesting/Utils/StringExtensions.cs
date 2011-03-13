using System.Text.RegularExpressions;

namespace TerseControllerTesting.Utils
{
    public static class StringExtensions
    {
        public static string ReReplace(this string str, string pattern, string replace, RegexOptions options = RegexOptions.None)
        {
            return new Regex(pattern, options).Replace(str, replace);
        }

        public static string ReReplace(this string str, string pattern, MatchEvaluator replacer, RegexOptions options = RegexOptions.None)
        {
            return new Regex(pattern, options).Replace(str, replacer);
        }

        public static string UnCamelCaseify(this string str)
        {
            return str.ReReplace(@"([A-Z])", @" $1")
                .ReReplace(@"([A-Za-z])([0-9])", "$1 $2")
                .ToLower()
                .ReReplace(@"[^\w]", " ")
                .Trim()
                .ReReplace("^[a-z]", c => c.Captures[0].Value.ToUpper())
                .ReReplace(@"\s+", " ")
                .ToSentenceCase()
                .ReReplace(@"\bid\b", "ID");
        }

        public static string ToSentenceCase(this string str)
        {
            return str
                .ToLower()
                .ReReplace("^[a-z]", c => c.Captures[0].Value.ToUpper());
        }
    }
}
