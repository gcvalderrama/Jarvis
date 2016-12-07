using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyVerbToConcept
    {
        private CGGraph graph;

        public StrategyVerbToConcept(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            foreach (var item in this.graph.Nodes.Where(c=>c.text.Contains("-0")))
            {
                if (this.graph.IsLeaf(item))
                {
                    item.text = item.nosuffix;
                }                
            }            
        }
    }
}
