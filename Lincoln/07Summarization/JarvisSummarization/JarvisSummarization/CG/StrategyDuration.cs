using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyDuration
    {
        private CGGraph graph;

        public StrategyDuration(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            
            foreach (var item in this.graph.Relations.ToList())
            {
                if (item.label.StartsWith("duration"))
                {
                    var node = this.graph.Nodes.Where(c => c.id == item.Tail).Single();
                    this.graph.RecursiveRemoveSubGraph(node);
                }
            }
            
        }
    }
}
