using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyCompressGraph
    {
        private CGGraph graph;
        public StrategyCompressGraph(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            //we will remove all single nodes based on the idea that we don't have any relation between them
            List<CGNode> deletes = new List<CGNode>();
            foreach (var item in this.graph.Nodes)
            {
                var out_rels = from c in this.graph.Relations.Where(c => c.Head == item.id) select c; 
                var in_rels = from c in this.graph.Relations.Where(c => c.Tail== item.id) select c;
                if ((out_rels.Count() + in_rels.Count()) == 0)
                    deletes.Add(item);
            }
            foreach (var item in deletes)
            {
                this.graph.RemoveNode(item); 
            }
        }
    }
}
