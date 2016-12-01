using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyStopWords
    {
        private CGGraph graph;
        public StrategyStopWords(CGGraph graph)
        {
            this.graph = graph;
        }
        public List<string> StopWords = new List<string>() { "this", "that" };
        public void Execute()
        {
            var deletes = new List<CGRelation>();
            var delete_nodes = new List<CGNode>();
            foreach (var node in this.graph.Nodes)
            {
                if (StopWords.Contains(node.nosuffix))
                {
                    var relations = this.graph.Relations.Where(c => c.Tail == node.id);
                    deletes.AddRange(relations);
                    delete_nodes.Add(node);
                }
            }
            foreach (var item in delete_nodes)
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
