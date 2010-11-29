using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnu.Getopt;
using System.Reflection;
using Gettext.Cs;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace Gettext.DatabaseResourceGenerator
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var getopt = new Getopt(Assembly.GetExecutingAssembly().GetName().Name, args, "s:i:c:nad") { Opterr = false };

            string input = null;
            string culture = null;

            bool dontDeleteSet = false;
            bool insertAll = false;
            bool skipValidation = false;

            string connString = ConfigurationManager.ConnectionStrings["Gettext"].ConnectionString;
            string insertSP = ConfigurationManager.AppSettings["SP.Insert"];
            string deleteSP = ConfigurationManager.AppSettings["SP.Delete"];

            int option;
            while (-1 != (option = getopt.getopt()))
            {
                switch (option)
                {
                    case 'i': input = getopt.Optarg; break;
                    case 'c': culture = getopt.Optarg; break;
                    case 'n': dontDeleteSet = true; break;
                    case 's': connString = getopt.Optarg; break;
                    case 'a': insertAll = true; break;
                    case 'd': skipValidation = true; break;
                    default: PrintUsage(); return;
                }
            }

            if (input == null)
            {
                PrintUsage();
                return;
            }

            if (connString == null || insertSP == null || deleteSP == null)
            {
                Console.Out.WriteLine("Ensure that connection string, insert and delete stored procedures are set in app config.");
                return;
            }

            try
            {
                using (var db = new DatabaseInterface(connString, insertSP, deleteSP) { CheckDatabaseExists = !skipValidation })
                {
                    db.Init();
                    db.CheckSPs();

                    if (!dontDeleteSet)
                    {
                        db.DeleteResourceSet(culture);
                    }

                    var requestor = new DatabaseParserRequestor(culture, db, insertAll);
                    new PoParser().Parse(new StreamReader(input), requestor);

                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Exception in program: {0}\n{1}", ex.Message, ex.StackTrace);
            }

        }

        [Obsolete]
        private static void CreateI18NTable(string tableName, SqlConnection connection)
        {
            Console.WriteLine("Checking if the given table exists...");

            var command = connection.CreateCommand();
            command.CommandText = "select case when exists((select * from information_schema.tables where table_name = '" + tableName + "')) then 1 else 0 end";

            bool existsTable = (int)command.ExecuteScalar() == 1;

            if (!existsTable)
            {
                var trans = connection.BeginTransaction();

                command.Transaction = trans;

                Console.WriteLine(string.Format("Table {0} does not exist. Creating table...", tableName));
                command.CommandText = string.Format("CREATE TABLE {0} (Culture varchar(20) NOT NULL, MessageKey varchar(500) NOT NULL, MessageValue varchar(500))", tableName);
                command.ExecuteNonQuery();

                command.CommandText = string.Format("ALTER TABLE {0} ADD CONSTRAINT PK_{0} PRIMARY KEY CLUSTERED (Culture, MessageKey)", tableName);
                command.ExecuteNonQuery();

                trans.Commit();
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Gettext Cs Tools");
            Console.WriteLine("----------------");
            Console.WriteLine();
            Console.WriteLine("Parses a .po file and adds its entries to a database's table.");
            Console.WriteLine("Then you can use a DatabaseResourceManager to use those resources at runtime.");
            Console.WriteLine("Usage: {0} -iINPUTFILE -cCULTURE -sCONNSTRING", Assembly.GetExecutingAssembly().GetName().Name);
            Console.WriteLine(" INPUTFILE Input file must be in po format.");
            Console.WriteLine(" CULTURE Culture for the resource set to handle.");
            Console.WriteLine(" CONNSTRING Connection string to override app config.");
            Console.WriteLine(" CULTURE Culture for the resource set to handle.");
            Console.WriteLine("Options:");
            Console.WriteLine(" -n Dont delete previous resource set.");
            Console.WriteLine(" -a Insert all values, regardless of being empty.");
            Console.WriteLine(" -d Skip database exists validation.");

        }
    }
}
