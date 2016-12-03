using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyDomain
    {
        private CGGraph graph;

        public StrategyDomain(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("domain"))
                {
                    item.f = item.label;
                    item.description = item.label;
                    item.conceptualrole = item.label;                    
                }
            }
            foreach (var item in deletes)
            {
                graph.RemoveRelation(item);
            }
        }
    }
}
