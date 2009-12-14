using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Resources;
using Gnu.Getopt;
using Gettext.Cs;

namespace Instedd.Gettext.Msgfmt
{
    class Program
    {
        static void Main(string[] args)
        {
            var getopt = new Getopt(Assembly.GetExecutingAssembly().GetName().Name, args, "i:o:") { Opterr = false };

            string input = null;
            string output = null;

            int option;
            while (-1 != (option = getopt.getopt()))
            {
                switch (option)
                {
                    case 'i': input = getopt.Optarg; break;
                    case 'o': output = getopt.Optarg; break;

                    default: PrintUsage(); return;
                }
            }

            if (input == null || output == null)
            {
                PrintUsage();
                return;
            }

            try
            {
                if (!File.Exists(input))
                {
                    Console.WriteLine("File {0} not found", input);
                    return;
                }

                Dictionary<string, string> entries;
                var parser = new PoParser();
                using (var reader = new StreamReader(input))
                {
                    entries = parser.ParseIntoDictionary(reader);
                }

                using (var writer = new ResourceWriter(output))
                {
                    foreach (var kv in entries)
                    {
                        try { writer.AddResource(kv.Key, kv.Value); }
                        catch (Exception e) { Console.WriteLine("Error adding item {0}: {1}", kv.Key, e.Message); }
                    }
                    writer.Generate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during execution: {0}", ex.Message);
                return;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Gettext Cs Tools");
            Console.WriteLine("----------------");
            Console.WriteLine();
            Console.WriteLine("Custom message formatter from .po to .resources");
            Console.WriteLine("Usage: {0} -iINPUTFILE -oOUTPUTFILE", Assembly.GetExecutingAssembly().GetName().Name);
            Console.WriteLine(" Input file must be in po format.");
            Console.WriteLine(" Output file is NET resources file.");
        }
    }
}
