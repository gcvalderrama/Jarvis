using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyPersonFusion
    {
        private CGGraph graph;
        public StrategyPersonFusion(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGNode> deletes = new List<CGNode>();
            List<CGRelation> delete_rels = new List<CGRelation>(); 
            var nodes = this.graph.Nodes.Where(c => c.text == "person");
            foreach (var node in nodes)
            {
                var out_rels = this.graph.Relations.Where(c => c.Head == node.id);
                if (out_rels.Count() > 0)   
                {                    
                    foreach (var output in out_rels)
                    {                 
                        var tail = this.graph.Nodes.Where(c => c.id == output.Tail).Single();
                        node.text = node.text + " " +tail.text ;
                        node.nosuffix = node.nosuffix + " "+ tail.nosuffix ;
                        deletes.Add(tail);
                        delete_rels.Add(output); 
                    }    
                }
                else
                {
                    throw new ApplicationException();
                }                
            }
            foreach (var item in deletes)
            {
                this.graph.RemoveNode(item);
            }
            foreach (var item in delete_rels)
            {
                this.graph.RemoveRelation(item);
            }
        }
    }
}
