using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeSoft.Data
{


    using Microsoft.CSharp;
    using Microsoft.VisualBasic;
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

        public static class StringExtensions
        {
            private static readonly Regex _splitNameRegex = new Regex("[\\W_]+");
            private static readonly Regex _properWordRegex = new Regex("([A-Z][a-z]*)|([0-9]+)");
            private static readonly Regex _identifierRegex = new Regex("[^\\p{Ll}\\p{Lu}\\p{Lt}\\p{Lo}\\p{Nd}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Cf}\\p{Pc}\\p{Lm}]");
            private static readonly Regex _htmlIdentifierRegex = new Regex("[^A-Za-z0-9-_:\\.]");
            private static readonly Regex _whitespace = new Regex("\\s");
            private static readonly Regex _linkRegex = new Regex("\\b\r\n                (                       # Capture 1: entire matched URL\r\n                  (?:\r\n                    https?://               # http or https protocol\r\n                    |                       #   or\r\n                    www\\d{0,3}[.]           # \"www.\", \"www1.\", \"www2.\" … \"www999.\"\r\n                    |                           #   or\r\n                    [a-z0-9.\\-]+[.][a-z]{2,4}/  # looks like domain name followed by a slash\r\n                  )\r\n                  (?:                       # One or more:\r\n                    [^\\s()<>]+                  # Run of non-space, non-()<>\r\n                    |                           #   or\r\n                    \\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\)  # balanced parens, up to 2 levels\r\n                  )+\r\n                  (?:                       # End with:\r\n                    \\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\)  # balanced parens, up to 2 levels\r\n                    |                               #   or\r\n                    [^\\s`!()\\[\\]{};:'\".,<>?«»“”‘’]        # not a space or one of these punct chars\r\n                  )\r\n                )", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
            private static readonly string[] _salutations = new string[38]
            {
      "MR",
      "MRS",
      "MS",
      "MISS",
      "DR",
      "SIR",
      "MADAM",
      "SIR",
      "MONSIEUR",
      "MADEMOISELLE",
      "MADAME",
      "SIRE",
      "COL",
      "SENOR",
      "SR",
      "SENORA",
      "SRA",
      "SENORITA",
      "SRTA",
      "HERR",
      "FRAU",
      "DHR",
      "HR",
      "FR",
      "SHRI",
      "SHRIMATI",
      "SIGNORE",
      "SIG",
      "SIGNORA",
      "SIG.RA",
      "PAN",
      "PANI",
      "SENHOR",
      "SENHORA",
      "SENHORITA",
      "MENEER",
      "MEVROU",
      "MEJUFFROU"
            };
            private const string _paraBreak = "\n\n";
            private const string _link = "<a href=\"{0}\">{1}</a>";
            private const string _linkWithRel = "<a href=\"{0}\" rel=\"{1}\">{2}</a>";

            /// <summary>Truncates the specified text.</summary>
            /// <param name="text">The text to truncate.</param>
            /// <param name="keep">The number of characters to keep.</param>
            /// <returns>A truncate String.</returns>
            public static string Truncate(this string text, int keep)
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;
                string str = text.NormalizeLineEndings((string)null);
                if (str.Length <= keep)
                    return str;
                return str.Substring(0, keep - 3) + "...";
            }

            public static string Truncate(this string text, int length, string ellipsis, bool keepFullWordAtEnd)
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;
                if (text.Length < length)
                    return text;
                text = text.Substring(0, length);
                if (keepFullWordAtEnd && text.LastIndexOf(' ') > 0)
                    text = text.Substring(0, text.LastIndexOf(' '));
                return string.Format("{0}{1}", (object)text, (object)ellipsis);
            }

            public static string TruncatePath(this string path, int length)
            {
                if (string.IsNullOrEmpty(path))
                    return string.Empty;
                if (path.Length <= length)
                    return path;
                int length1 = "...".Length;
                if (length <= length1)
                    return "...";
                bool flag = true;
                string str1 = "";
                string str2 = "";
                int index1 = 0;
                int num = 0;
                string[] strArray = path.Split(Path.DirectorySeparatorChar);
                for (int index2 = 0; index2 < strArray.Length; ++index2)
                {
                    if (flag)
                    {
                        string str3 = string.Format("{0}{1}", (object)strArray[index1], (object)Path.DirectorySeparatorChar);
                        if (str1.Length + str2.Length + str3.Length + length1 <= length)
                        {
                            str1 += str3;
                            if (str3 != Path.DirectorySeparatorChar.ToString())
                                flag = false;
                            ++index1;
                        }
                        else
                            break;
                    }
                    else
                    {
                        int index3 = strArray.Length - num - 1;
                        string str3 = string.Format("{0}{1}", (object)Path.DirectorySeparatorChar, (object)strArray[index3]);
                        if (str1.Length + str2.Length + str3.Length + length1 <= length)
                        {
                            str2 = str3 + str2;
                            if (str3 != Path.DirectorySeparatorChar.ToString())
                                flag = true;
                            ++num;
                        }
                        else
                            break;
                    }
                }
                if (str2 == string.Empty)
                {
                    string str3 = strArray[strArray.Length - 1];
                    str2 = str3.Substring(str3.Length + length1 + str1.Length - length, length - length1 - str1.Length);
                }
                return string.Format("{0}{1}{2}", (object)str1, (object)"...", (object)str2);
            }

            /// <summary>
            /// Calculates a hash code for the string that is guaranteed to be stable across .NET versions.
            /// </summary>
            /// <param name="value">The string to hash.</param>
            /// <returns>The hash code</returns>
            public static int GetStableHashCode(this string value)
            {
                int num1 = 0;
                int index = 0;
                while (index < value.Length - 1)
                {
                    int num2 = (num1 << 5) - num1 + (int)value[index];
                    num1 = (num2 << 5) - num2 + (int)value[index + 1];
                    index += 2;
                }
                if (index < value.Length)
                    num1 = (num1 << 5) - num1 + (int)value[index];
                return num1;
            }

            public static string NormalizeLineEndings(this string text, string lineEnding = null)
            {
                if (string.IsNullOrEmpty(lineEnding))
                    lineEnding = Environment.NewLine;
                text = text.Replace("\r\n", "\n");
                if (lineEnding != "\n")
                    text = text.Replace("\r\n", lineEnding);
                return text;
            }

            /// <summary>
            /// Indicates whether the specified String object is null or an empty string
            /// </summary>
            /// <param name="item">A String reference</param>
            /// <returns>
            ///     <c>true</c> if is null or empty; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsNullOrEmpty(this string item)
            {
                return string.IsNullOrEmpty(item);
            }

            /// <summary>
            /// Indicates whether a specified string is null, empty, or consists only of white-space characters
            /// </summary>
            /// <param name="item">A String reference</param>
            /// <returns>
            ///      <c>true</c> if is null or empty; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsNullOrWhiteSpace(this string item)
            {
                if (!string.IsNullOrEmpty(item))
                    return item.All<char>(new Func<char, bool>(char.IsWhiteSpace));
                return true;
            }

            public static string AsNullIfEmpty(this string items)
            {
                if (!string.IsNullOrEmpty(items))
                    return items;
                return (string)null;
            }

            public static string AsNullIfWhiteSpace(this string items)
            {
                if (!items.IsNullOrWhiteSpace())
                    return items;
                return (string)null;
            }

            /// <summary>Determines if the string looks like JSON content.</summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsJson(this string value)
            {
                if (string.IsNullOrEmpty(value))
                    return false;
                if (value.StartsWith("{") || value.EndsWith("}"))
                    return true;
                if (value.StartsWith("["))
                    return value.EndsWith("]");
                return false;
            }

            /// <summary>Formats a string without throwing a FormatException.</summary>
            /// <param name="format">A String reference</param>
            /// <param name="args">Object parameters that should be formatted</param>
            /// <returns>Formatted string if no error is thrown, else reutrns the format parameter.</returns>
            public static string SafeFormat(this string format, params object[] args)
            {
                try
                {
                    return string.Format(format, args);
                }
                catch (FormatException ex)
                {
                    return format;
                }
            }

            ///// <summary>Uses the string as a format</summary>
            ///// <param name="format">A String reference</param>
            ///// <param name="args">Object parameters that should be formatted</param>
            ///// <returns>Formatted string</returns>
            //public static string FormatWith(this string format, params object[] args)
            //{
            //    format.Require<string>("format").NotNullOrEmpty();
            //    return string.Format(format, args);
            //}

            ///// <summary>Applies a format to the item</summary>
            ///// <param name="item">Item to format</param>
            ///// <param name="format">Format string</param>
            ///// <returns>Formatted string</returns>
            //public static string FormatAs(this object item, string format)
            //{
            //    format.Require<string>("format").NotNullOrEmpty();
            //    return string.Format(format, item);
            //}

            ///// <summary>Uses the string as a format.</summary>
            ///// <param name="format">A String reference</param>
            ///// <param name="source">Object that should be formatted</param>
            ///// <returns>Formatted string</returns>
            //public static string FormatName(this string format, object source)
            //{
            //    format.Require<string>("format").NotNullOrEmpty();
            //    return NameFormatter.Format(format, source);
            //}

            ///// <summary>Applies a format to the item</summary>
            ///// <param name="item">Item to format</param>
            ///// <param name="format">Format string</param>
            ///// <returns>Formatted string</returns>
            //public static string FormatNameAs(this object item, string format)
            //{
            //    format.Require<string>("format").NotNullOrEmpty();
            //    return NameFormatter.Format(format, item);
            //}

            /// <summary>
            /// Creates a string from the sequence by concatenating the result
            /// of the specified string selector function for each element.
            /// </summary>
            public static string ToConcatenatedString<T>(this IEnumerable<T> values, Func<T, string> stringSelector)
            {
                return values.ToConcatenatedString<T>(stringSelector, string.Empty);
            }

            /// <summary>
            /// Creates a string from the sequence by concatenating the result
            /// of the specified string selector function for each element.
            /// </summary>
            /// <param name="action"></param>
            /// <param name="separator">The string which separates each concatenated item.</param>
            /// <param name="values"></param>
            public static string ToConcatenatedString<T>(this IEnumerable<T> values, Func<T, string> action, string separator)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (T obj in values)
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append(separator);
                    stringBuilder.Append(action(obj));
                }
                return stringBuilder.ToString();
            }

            /// <summary>
            /// Converts an IEnumerable of values to a delimited String.
            /// </summary>
            /// <typeparam name="T">The type of objects to delimit.</typeparam>
            /// <param name="values">The IEnumerable string values to convert.</param>
            /// <param name="delimiter">The delimiter.</param>
            /// <returns>A delimited string of the values.</returns>
            public static string ToDelimitedString<T>(this IEnumerable<T> values, string delimiter)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (T obj in values)
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append(delimiter);
                    stringBuilder.Append(obj.ToString());
                }
                return stringBuilder.ToString();
            }

            /// <summary>
            /// Converts an IEnumerable of values to a delimited String.
            /// </summary>
            /// <param name="values">The IEnumerable string values to convert.</param>
            /// <returns>A delimited string of the values.</returns>
            public static string ToDelimitedString(this IEnumerable<string> values)
            {
                return values.ToDelimitedString(",");
            }

            /// <summary>
            /// Converts an IEnumerable of values to a delimited String.
            /// </summary>
            /// <param name="values">The IEnumerable string values to convert.</param>
            /// <param name="delimiter">The delimiter.</param>
            /// <returns>A delimited string of the values.</returns>
            public static string ToDelimitedString(this IEnumerable<string> values, string delimiter)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string str in values)
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append(delimiter);
                    stringBuilder.Append(str);
                }
                return stringBuilder.ToString();
            }

            /// <summary>Converts a string to use camelCase.</summary>
            /// <param name="value">The value.</param>
            /// <returns>The to camel case. </returns>
            public static string ToCamelCase(this string value)
            {
                if (string.IsNullOrEmpty(value))
                    return value;
                string pascalCase = value.ToPascalCase();
                if (pascalCase.Length > 2)
                    return ((int)char.ToLower(pascalCase[0])).ToString() + pascalCase.Substring(1);
                return pascalCase.ToLower();
            }

            /// <summary>Converts a string to use PascalCase.</summary>
            /// <param name="value">Text to convert</param>
            /// <returns>The string</returns>
            public static string ToTitleCase(this string value)
            {
                if (value.IsMixedCase())
                    return value;
                return ((int)char.ToUpper(value[0])).ToString() + value.Substring(1).ToLower();
            }

            /// <summary>Converts a string to use PascalCase.</summary>
            /// <param name="value">Text to convert</param>
            /// <returns>The string</returns>
            public static string ToPascalCase(this string value)
            {
                return value.ToPascalCase(StringExtensions._splitNameRegex);
            }

            /// <summary>Converts a string to an C# escaped literal String.</summary>
            /// <param name="value">Text to escape</param>
            /// <returns>The escaped string</returns>
            public static string ToCSharpLiteral(this string value)
            {
                StringWriter stringWriter = new StringWriter();
                new CSharpCodeProvider().GenerateCodeFromExpression((CodeExpression)new CodePrimitiveExpression((object)value), (TextWriter)stringWriter, (CodeGeneratorOptions)null);
                return stringWriter.GetStringBuilder().ToString();
            }

            /// <summary>Converts a string to a valid C# identifier String.</summary>
            /// <param name="value">Text to convert.</param>
            /// <returns>The valid identifier</returns>
            public static string ToCSharpIdentifier(this string value)
            {
                return new CSharpCodeProvider().CreateEscapedIdentifier(StringExtensions._identifierRegex.Replace(value, string.Empty));
            }

            /// <summary>Converts a string to an VB escaped literal String.</summary>
            /// <param name="value">Text to escape</param>
            /// <returns>The escaped string</returns>
            public static string ToVbLiteral(this string value)
            {
                StringWriter stringWriter = new StringWriter();
                new VBCodeProvider().GenerateCodeFromExpression((CodeExpression)new CodePrimitiveExpression((object)value), (TextWriter)stringWriter, (CodeGeneratorOptions)null);
                return stringWriter.GetStringBuilder().ToString();
            }

            /// <summary>Converts a string to a valid C# identifier String.</summary>
            /// <param name="value">Text to convert.</param>
            /// <returns>The valid identifier</returns>
            public static string ToVbIdentifier(this string value)
            {
                return new VBCodeProvider().CreateEscapedIdentifier(StringExtensions._identifierRegex.Replace(value, string.Empty));
            }

            /// <summary>Converts a string to a valid HTML identifier String.</summary>
            /// <param name="value">Text to convert.</param>
            /// <returns>The valid identifier</returns>
            public static string ToHtmlIdentifier(this string value)
            {
                string str = StringExtensions._htmlIdentifierRegex.Replace(value, string.Empty);
                if (str.StartsWith("__"))
                    str = "_" + str;
                return str;
            }

            /// <summary>Converts a string to a valid .NET identifier String.</summary>
            /// <param name="value">Text to convert.</param>
            /// <returns>The valid identifier</returns>
            public static string ToIdentifier(this string value)
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");
                string s = StringExtensions._identifierRegex.Replace(value, string.Empty);
                if (s.StartsWith("__") || char.IsDigit(s, 0))
                    s = "_" + s;
                return s;
            }

            /// <summary>
            /// Checks to see if a string is a valid .NET identifier String.
            /// </summary>
            /// <param name="value">String identifier to check.</param>
            /// <returns>Returns true if value is a valid identifier</returns>
            public static bool IsValidIdentifier(this string value)
            {
                if (value.IsNullOrWhiteSpace() || StringExtensions._identifierRegex.IsMatch(value))
                    return false;
                if (char.IsLetter(value[0]))
                    return true;
                if ((int)value[0] == 95)
                    return value.Length > 1;
                return false;
            }

            /// <summary>Checks to see if a string is a valid .NET namespace.</summary>
            /// <param name="value">String identifier to check.</param>
            /// <returns>Returns true if value is a valid namespace.</returns>
            public static bool IsValidNamespace(this string value)
            {
                if (value.IsNullOrWhiteSpace())
                    return false;
                return !((IEnumerable<string>)value.Split('.')).Any<string>((Func<string, bool>)(v => !v.IsValidIdentifier()));
            }

            /// <summary>Replicates the given String.</summary>
            /// <param name="value">Text to replicate</param>
            /// <param name="count">Times to replicate</param>
            /// <returns>The replicated string</returns>
            public static string Replicate(this string value, int count)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int index = 0; index < count; ++index)
                    stringBuilder.Append(value);
                return stringBuilder.ToString();
            }

            /// <summary>Converts a string to use PascalCase.</summary>
            /// <param name="value">Text to convert</param>
            /// <param name="splitRegex">Regular Expression to split words on.</param>
            /// <returns>The string</returns>
            public static string ToPascalCase(this string value, Regex splitRegex)
            {
                if (string.IsNullOrEmpty(value))
                    return value;
                bool flag = value.IsMixedCase();
                string[] strArray = splitRegex.Split(value);
                StringBuilder stringBuilder = new StringBuilder();
                if (strArray.Length > 1)
                {
                    foreach (string str in strArray)
                    {
                        if (str.Length > 1)
                        {
                            stringBuilder.Append(char.ToUpper(str[0]));
                            stringBuilder.Append(flag ? str.Substring(1) : str.Substring(1).ToLower());
                        }
                        else
                            stringBuilder.Append(str);
                    }
                }
                else if (value.Length > 1)
                {
                    stringBuilder.Append(char.ToUpper(value[0]));
                    stringBuilder.Append(flag ? value.Substring(1) : value.Substring(1).ToLower());
                }
                else
                    stringBuilder.Append(value.ToUpper());
                return stringBuilder.ToString();
            }

            /// <summary>
            /// Takes a NameIdentifier and spaces it out into words "Name Identifier".
            /// </summary>
            /// <param name="value">The value to convert.</param>
            /// <returns>The string</returns>
            public static string[] ToWords(this string value)
            {
                List<string> stringList = new List<string>();
                value = value.ToPascalCase();
                foreach (Match match in StringExtensions._properWordRegex.Matches(value))
                {
                    if (!match.Value.IsNullOrWhiteSpace())
                        stringList.Add(match.Value);
                }
                return stringList.ToArray();
            }

            /// <summary>
            /// Takes a NameIdentifier and spaces it out into words "Name Identifier".
            /// </summary>
            /// <param name="value">The value to convert.</param>
            /// <returns>The string</returns>
            public static string ToSpacedWords(this string value)
            {
                string[] words = value.ToWords();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string str in words)
                {
                    stringBuilder.Append(str);
                    stringBuilder.Append(' ');
                }
                return stringBuilder.ToString().Trim();
            }

            /// <summary>Removes all whitespace from a String.</summary>
            /// <param name="s">Initial String.</param>
            /// <returns>String with no whitespace.</returns>
            public static string RemoveWhiteSpace(this string s)
            {
                return StringExtensions._whitespace.Replace(s, string.Empty);
            }

            public static string ReplaceFirst(this string s, string find, string replace)
            {
                int length = s.IndexOf(find);
                if (length < 0)
                    return s;
                string str1 = s.Substring(0, length);
                string str2 = s.Substring(length + find.Length);
                return str1 + replace + str2;
            }

            public static void AppendFormatLine(this StringBuilder sb, string format, params string[] args)
            {
                sb.AppendLine(string.Format(format, (object[])args));
            }

            /// <summary>
            /// Returns a copy of this string converted to HTML markup.
            /// </summary>
            //public static string ToHtml(this string s)
            //{
            //    return s.ToHtml((string)null);
            //}

            /// <summary>
            /// Returns a copy of this string converted to HTML markup.
            /// </summary>
            /// <param name="s"> </param>
            /// <param name="rel">If specified, links will have the rel attribute set to this value
            /// attribute</param>
            //public static string ToHtml(this string s, string rel)
            //{
            //    s = s.NormalizeLineEndings("\n");
            //    StringBuilder sb = new StringBuilder();
            //    int num;
            //    for (int index = 0; index < s.Length; index = num + "\n\n".Length)
            //    {
            //        int startIndex = index;
            //        num = s.IndexOf("\n\n", startIndex);
            //        if (num < 0)
            //            num = s.Length;
            //        string s1 = s.Substring(startIndex, num - startIndex).Trim();
            //        if (s1.Length > 0)
            //            StringExtensions.EncodeParagraph(s1, sb, rel);
            //    }
            //    return sb.ToString();
            //}

            ///// <summary>Encodes a single paragraph to HTML.</summary>
            ///// <param name="s">Text to encode</param>
            ///// <param name="sb">StringBuilder to write results</param>
            ///// <param name="rel">If specified, links will have the rel attribute set to this value
            ///// attribute</param>
            //private static void EncodeParagraph(string s, StringBuilder sb, string rel = null)
            //{
            //    sb.AppendLine("<p>");
            //    s = HttpUtility.HtmlEncode(s);
            //    s = s.Replace("\n", "<br />\r\n");
            //    s = string.IsNullOrEmpty(rel) ? StringExtensions._linkRegex.Replace(s, string.Format("<a href=\"{0}\">{1}</a>", (object)"$1", (object)"$1")) : StringExtensions._linkRegex.Replace(s, string.Format("<a href=\"{0}\" rel=\"{1}\">{2}</a>", (object)"$1", (object)rel, (object)"$1"));
            //    StringExtensions.EncodeLinks(s, sb, rel);
            //    sb.AppendLine("\r\n</p>");
            //}

            public static MatchCollection GetHyperlinkMatches(this string value)
            {
                if (string.IsNullOrEmpty(value))
                    return (MatchCollection)null;
                return StringExtensions._linkRegex.Matches(value);
            }

            /// <summary>Encodes [[URL]] and [[Text][URL]] links to HTML.</summary>
            /// <param name="s">Text to encode</param>
            /// <param name="sb">StringBuilder to write results</param>
            /// <param name="rel">If specified, links will have the rel attribute set to this value
            /// attribute</param>
            private static void EncodeLinks(string s, StringBuilder sb, string rel)
            {
                int startIndex1 = 0;
                while (startIndex1 < s.Length)
                {
                    int startIndex2 = startIndex1;
                    startIndex1 = s.IndexOf("[[", startIndex1);
                    if (startIndex1 < 0)
                        startIndex1 = s.Length;
                    sb.Append(s.Substring(startIndex2, startIndex1 - startIndex2));
                    if (startIndex1 < s.Length)
                    {
                        int startIndex3 = startIndex1 + 2;
                        int num = s.IndexOf("]]", startIndex3);
                        if (num < 0)
                            num = s.Length;
                        string str1 = s.Substring(startIndex3, num - startIndex3);
                        int length = str1.IndexOf("][");
                        string str2;
                        if (length >= 0)
                        {
                            str2 = str1.Substring(length + 2);
                            str1 = str1.Substring(0, length);
                        }
                        else
                            str2 = str1;
                        if (string.IsNullOrEmpty(rel))
                            sb.Append(string.Format("<a href=\"{0}\">{1}</a>", (object)str2, (object)str1));
                        else
                            sb.Append(string.Format("<a href=\"{0}\" rel=\"{1}\">{2}</a>", (object)str2, (object)rel, (object)str1));
                        startIndex1 = num + 2;
                    }
                }
            }

            /// <summary>
            /// Creates a string to be used in HTML that won't be automatically turned into a link.
            /// </summary>
            /// <param name="url"></param>
            /// <returns>A </returns>
            public static string ToNonAutoLinkUrl(this string url)
            {
                if (string.IsNullOrEmpty(url))
                    return url;
                int length1 = url.IndexOf(':');
                if (length1 > 0)
                    url = url.Substring(0, length1) + "<span>:</span>" + url.Substring(length1 + 1);
                int length2 = url.LastIndexOf('.');
                if (length2 > 0)
                    url = url.Substring(0, length2) + "<span>.</span>" + url.Substring(length2 + 1);
                return url;
            }

            public static string ReplaceMultiple(this string s, IDictionary<string, string> replaceMap)
            {
                return s.ReplaceMultiple(replaceMap, false);
            }

            public static string ReplaceMultiple(this string s, IDictionary<string, string> replaceMap, bool isRegexFind)
            {
                SortedDictionary<int, StringExtensions.ReplaceKey> indexes = new SortedDictionary<int, StringExtensions.ReplaceKey>();
                foreach (KeyValuePair<string, string> replace in (IEnumerable<KeyValuePair<string, string>>)replaceMap)
                {
                    if (isRegexFind)
                        StringExtensions.FindMultipleRegex(ref s, replace.Key, (IDictionary<int, StringExtensions.ReplaceKey>)indexes);
                    else
                        StringExtensions.FindMultipleString(ref s, replace.Key, (IDictionary<int, StringExtensions.ReplaceKey>)indexes);
                }
                if (indexes.Count <= 0)
                    return s;
                return StringExtensions.BuildReplaceMultiple(ref s, indexes, replaceMap);
            }

            private static void FindMultipleRegex(ref string s, string find, IDictionary<int, StringExtensions.ReplaceKey> indexes)
            {
                MatchCollection matchCollection = new Regex(find, RegexOptions.IgnoreCase).Matches(s);
                for (int index = 0; index < matchCollection.Count; ++index)
                    indexes.Add(matchCollection[index].Index, new StringExtensions.ReplaceKey()
                    {
                        Key = find,
                        Length = matchCollection[index].Length
                    });
            }

            private static void FindMultipleString(ref string s, string find, IDictionary<int, StringExtensions.ReplaceKey> indexes)
            {
                int startIndex = 0;
                int key;
                do
                {
                    key = s.IndexOf(find, startIndex);
                    if (key >= 0)
                    {
                        indexes.Add(key, new StringExtensions.ReplaceKey()
                        {
                            Key = find,
                            Length = find.Length
                        });
                        startIndex = key + find.Length;
                    }
                }
                while (key >= 0);
            }

            private static string BuildReplaceMultiple(ref string s, SortedDictionary<int, StringExtensions.ReplaceKey> indexes, IDictionary<string, string> replaceMap)
            {
                StringBuilder stringBuilder = new StringBuilder();
                int startIndex = 0;
                foreach (KeyValuePair<int, StringExtensions.ReplaceKey> index in indexes)
                {
                    stringBuilder.Append(s.Substring(startIndex, index.Key - startIndex));
                    stringBuilder.Append(replaceMap[index.Value.Key]);
                    startIndex = index.Key + index.Value.Length;
                }
                stringBuilder.Append(s.Substring(startIndex));
                return stringBuilder.ToString();
            }

            /// <summary>Strips NewLines and Tabs</summary>
            /// <param name="s">The string to strip.</param>
            /// <returns>Stripped String.</returns>
            public static string StripInvisible(this string s)
            {
                return s.Replace("\r\n", " ").Replace('\n', ' ').Replace('\t', ' ');
            }

            /// <summary>Returns true if s contains substring value.</summary>
            /// <param name="s">Initial value</param>
            /// <param name="value">Substring value</param>
            /// <returns>Boolean</returns>
            public static bool Contains(this string s, string value)
            {
                return s.IndexOf(value) > -1;
            }

            /// <summary>Returns true if s contains substring value.</summary>
            /// <param name="s">Initial value</param>
            /// <param name="value">Substring value</param>
            /// <param name="comparison">StringComparison options.</param>
            /// <returns>Boolean</returns>
            public static bool Contains(this string s, string value, StringComparison comparison)
            {
                return s.IndexOf(value, comparison) > -1;
            }

            /// <summary>
            /// Indicates whether a string contains x occurrences of a String.
            /// </summary>
            /// <param name="s">The string to search.</param>
            /// <param name="value">The string to search for.</param>
            /// <returns>
            ///     <c>true</c> if the string contains at least two occurrences of {value}; otherwise, <c>false</c>.
            /// </returns>
            public static bool ContainsMultiple(this string s, string value)
            {
                return s.ContainsMultiple(value, 2);
            }

            /// <summary>
            /// Indicates whether a string contains x occurrences of a String.
            /// </summary>
            /// <param name="s">The string to search.</param>
            /// <param name="value">The string to search for.</param>
            /// <param name="count">The number of occurrences to search for.</param>
            /// <returns>
            ///     <c>true</c> if the string contains at least {count} occurrences of {value}; otherwise, <c>false</c>.
            /// </returns>
            public static bool ContainsMultiple(this string s, string value, int count)
            {
                if (count == 0)
                    return true;
                int num = s.IndexOf(value);
                if (num > -1)
                    return s.Substring(num + 1).ContainsMultiple(value, --count);
                return false;
            }

            public static string[] SplitAndTrim(this string s, params string[] separator)
            {
                if (s.IsNullOrEmpty())
                    return new string[0];
                string[] strArray = separator == null || separator.Length == 0 ? s.Split((char[])null, StringSplitOptions.RemoveEmptyEntries) : s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                for (int index = 0; index < strArray.Length; ++index)
                    strArray[index] = strArray[index].Trim();
                return strArray;
            }

            public static string[] SplitAndTrim(this string s, params char[] separator)
            {
                if (s.IsNullOrEmpty())
                    return new string[0];
                string[] strArray = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                for (int index = 0; index < strArray.Length; ++index)
                    strArray[index] = strArray[index].Trim();
                return strArray;
            }

            /// <summary>Convert UTF8 string to ASCII.</summary>
            /// <param name="s">The UTF8 String.</param>
            /// <returns>The ASCII String.</returns>
            public static string ToASCII(this string s)
            {
                return Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(Encoding.ASCII.EncodingName, (EncoderFallback)new EncoderReplacementFallback(string.Empty), (DecoderFallback)new DecoderExceptionFallback()), Encoding.UTF8.GetBytes(s)));
            }

            /// <summary>
            /// Do any of the strings contain both uppercase and lowercase characters?
            /// </summary>
            /// <param name="values">String values.</param>
            /// <returns>True if any contain mixed cases.</returns>
            public static bool IsMixedCase(this IEnumerable<string> values)
            {
                foreach (string s in values)
                {
                    if (s.IsMixedCase())
                        return true;
                }
                return false;
            }

            /// <summary>Is the string all lower case characters?</summary>
            /// <param name="s">The value.</param>
            /// <returns>True if all lower case.</returns>
            public static bool IsAllLowerCase(this string s)
            {
                if (s.IsNullOrEmpty())
                    return false;
                return !s.ContainsUpper();
            }

            /// <summary>Is the string all upper case characters?</summary>
            /// <param name="s">The value.</param>
            /// <returns>True if all upper case.</returns>
            public static bool IsAllUpperCase(this string s)
            {
                if (s.IsNullOrEmpty())
                    return false;
                return !s.ContainsLower();
            }

            /// <summary>Does string contain uppercase characters?</summary>
            /// <param name="s">The value.</param>
            /// <returns>True if contain upper case.</returns>
            public static bool ContainsUpper(this string s)
            {
                if (s.IsNullOrEmpty())
                    return false;
                return ((IEnumerable<char>)s.ToArray<char>()).Any<char>(new Func<char, bool>(char.IsUpper));
            }

            /// <summary>Does string contain lowercase characters?</summary>
            /// <param name="s">The value.</param>
            /// <returns>True if contain lower case.</returns>
            public static bool ContainsLower(this string s)
            {
                if (s.IsNullOrEmpty())
                    return false;
                return ((IEnumerable<char>)s.ToArray<char>()).Any<char>(new Func<char, bool>(char.IsLower));
            }

            /// <summary>
            /// Does string contain both uppercase and lowercase characters?
            /// </summary>
            /// <param name="s">The value.</param>
            /// <returns>True if contain mixed case.</returns>
            public static bool IsMixedCase(this string s)
            {
                if (s.IsNullOrEmpty())
                    return false;
                bool flag1 = false;
                bool flag2 = false;
                foreach (char c in s)
                {
                    if (char.IsUpper(c))
                        flag1 = true;
                    if (char.IsLower(c))
                        flag2 = true;
                }
                if (flag2)
                    return flag1;
                return false;
            }

            public static Dictionary<string, string> ParseConfigValues(this string value)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
                string str1 = value;
                char[] chArray = new char[1] { ';' };
                foreach (string str2 in str1.Split(chArray))
                {
                    string[] strArray = str2.Split('=');
                    if (strArray.Length == 2)
                        dictionary.Add(strArray[0].Trim(), strArray[1].Trim());
                    else
                        dictionary.Add(str2.Trim(), (string)null);
                }
                return dictionary;
            }

            public static string ToCommandLineArgument(this string path)
            {
                if (string.IsNullOrEmpty(path))
                    return string.Empty;
                if (!path.Contains(" "))
                    return path;
                return string.Format("\"{0}\"", (object)path);
            }

            public static string ToFileExtension(this string path)
            {
                if (string.IsNullOrEmpty(path))
                    return string.Empty;
                string str = Path.GetExtension(path) ?? string.Empty;
                if (str.Length <= 0)
                    return string.Empty;
                return str.ToLowerInvariant();
            }

            public static string GetDomainOfUrl(string url)
            {
                int num1 = url.IndexOf("://");
                int startIndex = num1 == -1 ? 0 : num1 + 3;
                int num2 = url.IndexOf('/', startIndex);
                if (num2 != -1)
                    return url.Substring(0, num2 + 1);
                return url;
            }

            public static bool IsSalutation(this string value)
            {
                value = value.ToASCII();
                value = value.Trim();
                value = value.TrimEnd('.');
                return ((IEnumerable<string>)StringExtensions._salutations).Any<string>((Func<string, bool>)(s => s.ToUpper() == value));
            }

            private class ReplaceKey
            {
                public string Key { get; set; }

                public int Length { get; set; }
            }
        }
    

}
