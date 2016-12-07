using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategySolveNullEdgeRelations
    {
        public void Execute(CGGraph graph)
        {
            foreach (var item in graph.Relations.Where(c=>c.label == "null_edge").ToList())
            {
                item.label = "ARG0";
                item.log += "transform nulledge to arg0";                
            }            
        }
    }
}
