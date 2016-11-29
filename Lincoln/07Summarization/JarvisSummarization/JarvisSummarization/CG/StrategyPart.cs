using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyPart
    {
        private CGGraph graph;

        public StrategyPart(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("part"))
                    deletes.Add(item);
            }
            foreach (var item in deletes)
            {
                graph.RemoveRelation(item);
            }
        }
    }
}
