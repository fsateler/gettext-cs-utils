using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Gettext.DatabaseResourceGenerator
{
    class DatabaseInterface : IDisposable
    {
        string connString;
        string insertSP;
        string deleteSP;

        SqlConnection conn;
        SqlTransaction trans;

        public bool CheckDatabaseExists { get; set; }

        public DatabaseInterface(string connString, string insertSP, string deleteSP)
        {
            this.connString = connString;
            this.insertSP = insertSP;
            this.deleteSP = deleteSP;

            this.conn = new SqlConnection(connString);
        }

        public DatabaseInterface Init()
        {
            this.conn.Open();
            this.trans = conn.BeginTransaction();
            return this;
        }

        public void Commit()
        {
            this.trans.Commit();
        }

        public void InsertResource(string culture, string key, string value)
        {
            try
            {
                var command = new SqlCommand() { CommandText = insertSP, CommandType = System.Data.CommandType.StoredProcedure, Connection = conn, Transaction = trans };
                command.Parameters.AddWithValue("@culture", culture);
                command.Parameters.AddWithValue("@key", key);
                command.Parameters.AddWithValue("@value", value);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error inserting resource for culture {0} key '{1}' value '{2}': {3}", culture, key, value, ex.Message);
                throw;
            }
        }

        public void DeleteResourceSet(string culture)
        {
            try
            {
                var command = new SqlCommand() { CommandText = deleteSP, CommandType = System.Data.CommandType.StoredProcedure, Connection = conn, Transaction = trans };
                command.Parameters.AddWithValue("@culture", culture);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error deleting resource set for culture {0}: {1}", culture, ex.Message);
                throw;
            }
        }

        public void CheckSPs()
        {
            if (!ExistsSP(insertSP)) CreateInsertSP();
            if (!ExistsSP(deleteSP)) CreateDeleteSP();
        }

        private void CreateDeleteSP()
        {
            // TODO: Create delete SP
            throw new NotImplementedException(String.Format("Delete stored procedure '{0}' unimplemented", deleteSP));
        }

        private void CreateInsertSP()
        {
            // TODO: Create insert SP
            throw new NotImplementedException(String.Format("Insert stored procedure '{0}' unimplemented", insertSP));
        }

        private bool ExistsSP(string sp)
        {
            try
            {
                string cmd = String.Format("SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'P', N'PC')", sp);

                var command = new SqlCommand(cmd, conn, trans);
                var count = (int)command.ExecuteScalar();

                return count > 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error checking SP {0}: {1}", sp, ex.Message);
                throw;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.conn != null)
            {
                this.conn.Dispose();
            }
        }

        #endregion


    }
}
