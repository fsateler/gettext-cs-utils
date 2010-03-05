using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Instedd.Gettext.AspExtract
{
    public interface IAspStringsParserRequestor
    {
        void Report(string text);
    }

    public interface IAspStringsParser
    {
        void Parse(String text, IAspStringsParserRequestor requestor);
    }

    public class CompositeAspStringsParser : IAspStringsParser
    {
        IAspStringsParser[] parsers;

        public CompositeAspStringsParser(params IAspStringsParser[] parsers)
        {
            this.parsers = parsers;
        }

        public void Parse(String text, IAspStringsParserRequestor requestor)
        {
            foreach (var parser in parsers)
            {
                parser.Parse(text, requestor);
            }
        }
    }

    public class CSharpAspStringsParser : IAspStringsParser
    {
        Regex regex;

        public CSharpAspStringsParser(string function)
        {
            regex = new Regex(String.Format(@"(?<!\w){0}\(""(?<text>[^""\\]*(?:\\.[^""\\]*)*)""", function));
        }

        public void Parse(String text, IAspStringsParserRequestor requestor)
        {
            foreach (Match match in regex.Matches(text))
            {
                if (match.Groups["text"].Success)
                {
                    requestor.Report(match.Groups["text"].Value);
                }
            }
        }
    }

    public class AspStringsParser : IAspStringsParser
    {
        Regex regex;

        public AspStringsParser(string tag)
        {
            regex = new Regex(String.Format(@"<\s*{0}\s*[^>]*>(?<text>.+?)</\s*{0}\s*>", tag), RegexOptions.Singleline);
        }

        public void Parse(String text, IAspStringsParserRequestor requestor)
        {
            foreach (Match match in regex.Matches(text))
            {
                if (match.Groups["text"].Success)
                {
                    requestor.Report(match.Groups["text"].Value);
                }
            }
        }
    }
}
