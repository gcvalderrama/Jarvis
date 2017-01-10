using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization
{
    class SanityXml
    {
        public static string Sanity(string input)
        {
            return input.Replace("&", "&amp;").Replace("%", "&#37;").Replace("\"\"\"", "\"'\"").Replace("\"\",\"", "\"',\"").Replace("\")\"\"", "\")'\"").Replace("\"\"'s\"", "\"'s\"");
        }
    }
}
