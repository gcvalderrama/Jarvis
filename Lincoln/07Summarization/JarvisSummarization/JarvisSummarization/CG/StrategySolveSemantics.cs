using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategySolveSemantics
    {
        private CGGraph graph;
        public StrategySolveSemantics(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            var deletes = new List<CGRelation>(); 
            foreach (var relation in graph.Relations)
            {
                var head = this.graph.Nodes.Where(c => c.id == relation.Head).First();
                var tail = this.graph.Nodes.Where(c => c.id == relation.Tail).First();
                
                if (!head.text.Contains("-0") && relation.label.Contains("ARG"))
                {
                    deletes.Add(relation);                     
                }                
            }
            foreach (var item in deletes)
            {
                this.graph.RemoveRelation(item); 
            }
        }
    }
}
