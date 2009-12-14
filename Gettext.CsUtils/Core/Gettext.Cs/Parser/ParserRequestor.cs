using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gettext.Cs
{
    /// <summary>
    /// Interface for retrieving data from the parser.
    /// </summary>
    public interface IGettextParserRequestor
    {
        /// <summary>
        /// Handles a key value pair parsed from the po file.
        /// </summary>
        void Handle(string key, string value);
    }

    /// <summary>
    /// Collects data from the parser into a dictionary.
    /// </summary>
    public class DictionaryGettextParserRequestor : Dictionary<String, String>, IGettextParserRequestor
    {
        #region IGettextParserRequestor Members

        public void Handle(string key, string value)
        {
            this[key] = value;
        }

        #endregion
    }
}
