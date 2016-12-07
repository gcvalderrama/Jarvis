using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class StrategyEntity
    {
        private CGGraph graph;
        public StrategyEntity(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            List<CGNode> deletes_node = new List<CGNode>();
            foreach (var namerelation in this.graph.Relations.Where(c=>c.label=="name"))
            {
                var head = this.graph.Nodes.Where(c => c.id == namerelation.Head).Single();
                var tail = this.graph.Nodes.Where(c => c.id == namerelation.Tail).Single();
                var oprels = this.graph.Relations.Where(c => c.Head == tail.id && c.label.StartsWith("op")).OrderBy(c=>c.Tail).ToList();
                
                head.Entity = head.nosuffix;
                head.text += " named ";
                foreach (var rel in oprels)
                {
                    var tailop = this.graph.Nodes.Where(c => c.id == rel.Tail).Single();                    
                    head.text += " " + tailop.text;              
                    deletes_node.Add(tailop);                    
                    deletes.Add(rel);
                }
                head.nosuffix = head.text;
                deletes_node.Add(tail);
                deletes.Add(namerelation);
            }
            foreach (var item in deletes_node)
            {
                this.graph.RemoveNode(item);
            }
            foreach (var item in deletes)
            {
                this.graph.RemoveRelation(item); 
            }

        }
    }
}
