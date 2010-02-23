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

        //HACK: the utilities require the constructor of a custom ResourceManager to have this signature,
        //even though the parameters are not used in this case.
        public DatabaseResourceManager(string name, string path, string fileformat)
            : base()
        {
            this.dsn = ConfigurationManager.AppSettings["Gettext.ConnectionString"];
            ResourceSets = new System.Collections.Hashtable();
        }

        protected override ResourceSet InternalGetResourceSet(
          CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            DatabaseResourceSet rs = null;
 
            if (ResourceSets.Contains(culture.Name))
            {
                rs = ResourceSets[culture.Name] as DatabaseResourceSet;
            }
            else
            {
                rs = new DatabaseResourceSet(dsn, culture);
                ResourceSets.Add(culture.Name, rs);
            }
            
            return rs; 
        }
    }
}
