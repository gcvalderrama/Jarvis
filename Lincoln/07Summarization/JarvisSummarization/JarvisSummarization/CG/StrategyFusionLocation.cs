using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class StrategyFusionLocation
    {
        public void Execute(CGGraph graph)
        {
            foreach (var item in graph.Relations.Where(c=>c.label== "location"))
            {

                
            }
        }
    }
}
