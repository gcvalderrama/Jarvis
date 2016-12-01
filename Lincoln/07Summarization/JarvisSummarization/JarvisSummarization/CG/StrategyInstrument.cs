using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyInstrument
    {
        private CGGraph graph;

        public StrategyInstrument(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("instrument"))
                {
                    item.description = item.label;
                    item.f = item.label;
                    item.conceptualrole = item.label;
                }
            }
        }
    }
}
