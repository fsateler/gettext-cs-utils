using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.IO;

namespace Gettext.Cs
{
    public class GettextResourceReader : IResourceReader
    {
        Stream stream;

        public GettextResourceReader(Stream stream)
        {
            this.stream = stream;
        }
        
        #region IResourceReader Members

        public void Close()
        {
            if (stream != null)
            {
                this.stream.Close();
            }
        }

        public System.Collections.IDictionaryEnumerator GetEnumerator()
        {
            if (stream == null)
            {
                throw new ArgumentNullException("Input stream cannot be null");
            }
            
            using (var reader = new StreamReader(stream))
            {
                return new PoParser().ParseIntoDictionary(reader).GetEnumerator();
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Dispose();
            }
        }

        #endregion
    }
}
