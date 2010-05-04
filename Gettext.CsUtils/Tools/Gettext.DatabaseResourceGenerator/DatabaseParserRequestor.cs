using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gettext.Cs;

namespace Gettext.DatabaseResourceGenerator
{
    class DatabaseParserRequestor : IGettextParserRequestor
    {
        bool insertAll;
        string culture;
        DatabaseInterface db;

        public DatabaseParserRequestor(string culture, DatabaseInterface db, bool insertAll)
        {
            this.culture = culture;
            this.db = db;
            this.insertAll = insertAll;
        }

        #region IGettextParserRequestor Members

        public void Handle(string key, string value)
        {
            if (insertAll || !String.IsNullOrEmpty(value))
            {
                this.db.InsertResource(culture, key, value);
            }
        }

        #endregion
    }
}
