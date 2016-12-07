using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyMod
    {
        private CGGraph graph;

        public StrategyMod(CGGraph graph)
        {
            this.graph = graph;
        }
        private List<string> stopwords = new List<string>() { "so", "any", "that", "this"};
        public void Execute()
        {

            List<CGNode> deletes = new List<CGNode>();
            List<CGRelation> delete_relations = new List<CGRelation>(); 
                
            foreach (var item in this.graph.Relations)
            {                
                if (item.label.StartsWith("mod"))
                {
                    var head = this.graph.Nodes.Where(c => c.id == item.Head).Single();
                    var tail = this.graph.Nodes.Where(c => c.id == item.Tail).Single();
                    if (this.graph.IsLeaf(tail))
                    {
                        if (stopwords.Contains(tail.label))
                        {
                            delete_relations.Add(item);
                            deletes.Add(tail);
                        }
                        else
                        {
                            delete_relations.Add(item);
                            head.log += "mod fussion";
                            head.text = tail.text +  " "+ head.text;
                            head.nosuffix = tail.nosuffix + " " + head.nosuffix;                            
                        }
                    }                    
                }
            }
            foreach (var item in delete_relations)
            {
                this.graph.RemoveRelation(item);
            }
            foreach (var item in deletes)
            {
                this.graph.RemoveNode(item); 
            }
        }
    }
}
