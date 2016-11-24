using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyLocation
    {
        private CGGraph graph;

        public StrategyLocation(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("location"))
                    deletes.Add(item);
            }
            foreach (var item in deletes)
            {
                graph.Relations.Remove(item);
            }
        }
    }
}
