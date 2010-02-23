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

        public DatabaseResourceReader
           (string dsn, CultureInfo culture)
        {
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

            command.CommandText = string.Format("SELECT MessageKey, MessageValue FROM Message WHERE Culture = '{0}'", language);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetValue(1) != System.DBNull.Value)
                        dict.Add(reader.GetString(0), reader.GetString(1));
                }

                reader.Close();
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
