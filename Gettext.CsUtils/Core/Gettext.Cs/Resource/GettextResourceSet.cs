using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Gettext.Cs
{
    public class GettextResourceSet : System.Resources.ResourceSet
    {
        public GettextResourceSet(string filename)
            : base(new GettextResourceReader(File.OpenRead(filename)))
        {
        }

        public GettextResourceSet(Stream stream)
            : base(new GettextResourceReader(stream))
        {
        }

        public override Type GetDefaultReader()
        {
            return typeof(Gettext.Cs.GettextResourceReader);
        }
    }
}
