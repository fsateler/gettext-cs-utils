using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Collections;
using System.Data.SqlClient;
using System.Globalization;

namespace Gettext.Cs
{
    public class DatabaseResourceReader : IResourceReader
    {
        private string dsn;
        private string language;
        private string sp;

        public DatabaseResourceReader(string dsn, CultureInfo culture)
        {
            this.dsn = dsn;
            this.language = culture.Name;
        }

        public DatabaseResourceReader(string dsn, CultureInfo culture, string sp)
        {
            this.sp = sp;
            this.dsn = dsn;
            this.language = culture.Name;
        }

        public System.Collections.IDictionaryEnumerator GetEnumerator()
        {
            Hashtable dict = new Hashtable();

            SqlConnection connection = new SqlConnection(dsn);
            SqlCommand command = connection.CreateCommand();

            if (language == "")
                language = CultureInfo.InvariantCulture.Name;

            // Use stored procedure or plain text
            if (sp == null)
            {
                command.CommandText = string.Format("SELECT MessageKey, MessageValue FROM Message WHERE Culture = '{0}'", language);
            }
            else
            {
                command.CommandText = sp;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@culture", language);
            }

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetValue(1) != System.DBNull.Value)
                        {
                            dict.Add(reader.GetString(0), reader.GetString(1));
                        }
                    }
                }

            }
            catch   // ignore missing columns in the database
            {
            }
            finally
            {
                connection.Close();
            }

            return dict.GetEnumerator();
        }

        public void Close()
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void IDisposable.Dispose()
        {
        }
    }
}
