using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisRDFToNEO4J.NEO
{
    public class NEORSTRelation
    {
        public string relation { get; set; }
        public NEORSTNode Child { get; set; }
    }
}
