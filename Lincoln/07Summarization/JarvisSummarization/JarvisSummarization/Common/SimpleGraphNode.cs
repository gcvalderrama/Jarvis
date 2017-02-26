using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.Common
{
    public class SimpleGraphNode
    {
        public int Id { get; set; }
        public string Label { get; set; }
    }
    public class SimpleGraphRelation
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string Label { get; set; }
    }
}
