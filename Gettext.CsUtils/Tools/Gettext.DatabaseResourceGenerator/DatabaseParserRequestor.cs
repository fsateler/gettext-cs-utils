using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gettext.Cs;

namespace Gettext.DatabaseResourceGenerator
{
    class DatabaseParserRequestor : IGettextParserRequestor
    {
        string culture;
        DatabaseInterface db;

        public DatabaseParserRequestor(string culture, DatabaseInterface db)
        {
            this.culture = culture;
            this.db = db;
        }

        #region IGettextParserRequestor Members

        public void Handle(string key, string value)
        {
            this.db.InsertResource(culture, key, value);
        }

        #endregion
    }
}
