using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Instedd.Gettext.AspExtract
{
    public class AspStringsExtractor
    {
        IAspStringsParser parser;
        String outputExtension;
        String keyword;

        public bool Notify { get; set; }

        public AspStringsExtractor(string keyword, string outputExtension, IAspStringsParser parser)
        {
            this.keyword = keyword;
            this.parser = parser;
            this.outputExtension = outputExtension;
        }

        private class ParserRequestor : IAspStringsParserRequestor, IDisposable
        {
            string path;
            string keyword;

            TextWriter writer;

            public ParserRequestor(string outputPath, string keyword)
            {
                this.keyword = keyword;
                this.path = outputPath;

                this.writer = new StreamWriter(path, false);
            }

            #region IAspStringsParserRequestor Members

            public void Report(string text)
            {
                writer.WriteLine(@"{0}(@""{1}"");", keyword, Escape(text));
            }

            private string Escape(string text)
            {
                return text.Replace('\r', ' ').Replace("\"", "\"\""); 
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                if (this.writer != null)
                {
                    this.writer.Dispose();
                }
            }

            #endregion
        }

        public void Extract(string path)
        {
            ExtractFilesWithPattern(path, "*.aspx");
            ExtractFilesWithPattern(path, "*.ascx");
            ExtractFilesWithPattern(path, "*.Master");
        }

        private void ExtractFilesWithPattern(string path, string pattern)
        {
            foreach (var file in Directory.GetFiles(path, pattern, SearchOption.AllDirectories))
            {
                ExtractFromFile(file);
            }
        }

        private void ExtractFromFile(string file)
        {
            var output = String.Format("{0}.{1}", file, outputExtension);
            
            using (var requestor = new ParserRequestor(output, keyword))
            {
                using (var reader = new StreamReader(file))
                {
                    parser.Parse(reader.ReadToEnd(), requestor);
                }
            }

            if (Notify)
            {
                Console.Out.WriteLine("Parsed file {0} and output written to {1}.", file, output);
            }
        }
    }
}
