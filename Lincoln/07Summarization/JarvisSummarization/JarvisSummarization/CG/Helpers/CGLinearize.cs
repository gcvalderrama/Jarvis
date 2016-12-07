using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG.Helpers
{
    public class CGLinearize
    {
        private CGGraph graph;

        public CGLinearize(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute(CGNode Node)
        {
            
        }
        public void Recursive(List<CGNode> Visited, CGNode Target)
        {
            var children = this.graph.GetChildren(Target);
            Visited.Add(Target);
            foreach (var item in children)
            {                
                this.Recursive(Visited, item);
            }
        }
    }
}
