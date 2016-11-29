using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyPurpose
    {
        private CGGraph graph;

        public StrategyPurpose(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            foreach (var item in this.graph.Relations.Where(c => c.label == "purpose"))
            {
                item.description = "purpose relation";
                item.f = "purpose";
            }
        }
    }
}
