using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnu.Getopt;
using System.Reflection;
using Gettext.Cs;
using System.IO;
using System.Data.SqlClient;

namespace Gettext.DatabaseResourceGenerator
{
    class Program
    {
        private const string CONNECTION_STRING_TEMPLATE = "Data Source={0}; Initial Catalog={1}; Integrated Security=SSPI;";

        static void Main(string[] args)
        {
            var getopt = new Getopt(Assembly.GetExecutingAssembly().GetName().Name, args, "i:s:d:t:c:") { Opterr = false };

            string input = null;
            string serverName = null;
            string databaseName = null;
            string tableName = null;
            string culture = null;

            int option;
            while (-1 != (option = getopt.getopt()))
            {
                switch (option)
                {
                    case 'i': input = getopt.Optarg; break;
                    case 's': serverName = getopt.Optarg; break;
                    case 'd': databaseName = getopt.Optarg; break;
                    case 't': tableName = getopt.Optarg; break;
                    case 'c': culture = getopt.Optarg; break;
                    default: PrintUsage(); return;
                }
            }

            if (input == null || serverName == null || databaseName == null || tableName == null)
            {
                PrintUsage();
                return;
            }

            using (var connection = new SqlConnection(string.Format(CONNECTION_STRING_TEMPLATE, serverName, databaseName)))
            {
                try
                {
                    Console.WriteLine("Parsing input po file...");
                    var parser = new PoParser();
                    var resources = parser.ParseIntoDictionary(new StringReader(File.ReadAllText(input)));

                    Console.WriteLine("Connecting to database...");
                   
                    connection.Open();

                    CreateI18NTable(tableName, connection);

                    var command = connection.CreateCommand();

                    if (resources.Keys.Count == 0)
                    {
                        Console.WriteLine("There are no messages to insert or update");
                        return;
                    }
                    
                    string listOfKeys = null;
                    if (resources.Keys.Count == 1)
                        listOfKeys = "'" + resources.Keys.First().Replace("'", "''") + "'";
                    else
                        listOfKeys = resources.Keys.Skip(1).Aggregate("'" + resources.Keys.First().Replace("'", "''") + "'", (list, next) => list + ", '" + next.Replace("'", "''") + "'"); 

                    Console.WriteLine(string.Format("Looking for {0}", listOfKeys));

                    command.CommandText = string.Format("SELECT MessageKey FROM {0} WHERE CULTURE = '{1}' AND MessageKey IN ({2})", tableName, culture, listOfKeys);

                    var queryReader = command.ExecuteReader();

                    var resourcesToUpdate = new Dictionary<string, string>();
                    while (queryReader.Read())
                    {
                         var resourcesEntry = resources.First(entry => entry.Key == (string)queryReader[0]);
                         resourcesToUpdate.Add(resourcesEntry.Key, resourcesEntry.Value);
                    }
                    queryReader.Close();

                    var resourcesToInsert = resources.Where(entry => !resourcesToUpdate.Keys.Contains(entry.Key));

                    var trans = connection.BeginTransaction();

                    command.Transaction = trans;

                    Console.WriteLine(string.Format("Updating {0} preexisting messages...", resourcesToUpdate.Count));

                    foreach (var resourceToUpdate in resourcesToUpdate)
                    {
                        command.CommandText = string.Format("UPDATE {0} SET MessageValue = '{1}' WHERE CULTURE = '{2}' AND MessageKey = '{3}'",
                                                                tableName, resourceToUpdate.Value.Replace("'", "''"), culture, resourceToUpdate.Key.Replace("'", "''"));

                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine(string.Format("Inserting {0} new messages...", resourcesToInsert.Count()));

                    foreach (var resourceToInsert in resourcesToInsert)
                    {
                        command.CommandText = string.Format("INSERT INTO {0} VALUES('{1}','{2}','{3}')",
                                                                tableName, culture, resourceToInsert.Key.Replace("'", "''"), resourceToInsert.Value.Replace("'", "''"));

                        command.ExecuteNonQuery();
                    }
                    
                    trans.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

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
            Console.WriteLine("Usage: {0} -iINPUTFILE -sDATABASESERVERNAME -dDATABASENAME -tTABLENAME -cCULTURE", Assembly.GetExecutingAssembly().GetName().Name);
            Console.WriteLine(" INPUTFILE Input file must be in po format.");
            Console.WriteLine(" DATABASESERVERNAME Name of the server where the target database is.");
            Console.WriteLine(" DATABASENAME Name of the target database.");
            Console.WriteLine(" TABLENAME Name for the resources table to be created/updated with the resource entries.");
            Console.WriteLine(" CULTURE Four digit code of the given po file's culture.");
        }
    }
}
