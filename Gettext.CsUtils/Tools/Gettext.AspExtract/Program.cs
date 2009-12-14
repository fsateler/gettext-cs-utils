using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Gnu.Getopt;
using System.Diagnostics;

namespace Instedd.Gettext.AspExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            var getopt = new Getopt(Assembly.GetExecutingAssembly().GetName().Name, args, "t:k:e:p:f:") { Opterr = false };

            string tag = null;
            string keyword = null;
            string extension = null;
            string path = null;
            string function = null;

            int option;
            while (-1 != (option = getopt.getopt()))
            {
                switch (option)
                {
                    case 't': tag = getopt.Optarg; break;
                    case 'k': keyword = getopt.Optarg; break;
                    case 'e': extension = getopt.Optarg; break;
                    case 'p': path = getopt.Optarg; break;
                    case 'f': function = getopt.Optarg; break;

                    default: PrintUsage(); return;
                }
            }

            if (keyword == null || extension == null || path == null)
            {
                PrintUsage(); return;
            }

            if (tag == null && function == null)
            {
                PrintUsage(); return;
            }

            try
            {
                IAspStringsParser tagParser = tag == null ? null : new AspStringsParser(tag);
                IAspStringsParser csParser = function == null ? null : new CSharpAspStringsParser(function);

                IAspStringsParser parser;
                if (tagParser != null && csParser != null)
                {
                    parser = new CompositeAspStringsParser(tagParser, csParser);
                }
                else
                {
                    parser = tagParser ?? csParser;
                }

                var extractor = new AspStringsExtractor(keyword, extension, parser);
                extractor.Extract(path);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failure processing files: {0}", ex.Message);
                return;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Gettext Cs Tools");
            Console.WriteLine("----------------");
            Console.WriteLine();
            Console.WriteLine("Extracts all text contained in a specified tag from asp net files.");
            Console.WriteLine("For each file processed, another file is created with a function processable by xgettext.");
            Console.WriteLine();
            Console.WriteLine("Usage: {0} -tTAG -fFUNCTION -kKEYWORD -eEXTENSION -pPATH", Assembly.GetExecutingAssembly().GetName().Name);
            Console.WriteLine(" Tag: extract text contained in this tag from all aspx and aspc files.");
            Console.WriteLine(" Function: extract text contained in the first parameter of this C# function from all aspx and aspc files.");
            Console.WriteLine(" Keyword: function name to use in the generation of the new file.");
            Console.WriteLine(" Extension: extension to append to asp files to create the new ones.");
            Console.WriteLine(" Path: path to process for asp files.");
        }
    }
}
