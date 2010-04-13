using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Configuration;

namespace Gettext.Cs
{
    public class DatabaseResourceManager : System.Resources.ResourceManager
    {
        private string dsn;
        private string sp;

        public DatabaseResourceManager()
            : base()
        {
            this.dsn = ConfigurationManager.AppSettings["Gettext.ConnectionString"] ?? ConfigurationManager.ConnectionStrings["Gettext"].ConnectionString;
            ResourceSets = new System.Collections.Hashtable();
        }

        public DatabaseResourceManager(string storedProcedure)
            : this()
        {
            this.sp = storedProcedure;
        }

        // Hack: kept for compatibility
        public DatabaseResourceManager(string name, string path, string fileformat)
            : this()
        {
        }

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            DatabaseResourceSet rs = null;
 
            if (ResourceSets.Contains(culture.Name))
            {
                rs = ResourceSets[culture.Name] as DatabaseResourceSet;
            }
            else
            {
                rs = new DatabaseResourceSet(dsn, culture, sp);
                ResourceSets.Add(culture.Name, rs);
            }
            
            return rs; 
        }
    }
}
