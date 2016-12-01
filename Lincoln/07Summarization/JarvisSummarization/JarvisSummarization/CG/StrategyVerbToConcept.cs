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
            var nodes = from c in this.graph.Nodes.Where(c => c.text.Contains("-0")) select c;

            foreach (var item in nodes)
            {
                var out_rels = this.graph.Relations.
                    Where(c => c.Head == item.id).ToList();
                if (out_rels.Count == 0)
                    item.text = item.nosuffix; 
            }            
        }
    }
}
