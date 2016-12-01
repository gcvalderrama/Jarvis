using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyManner
    {
        private CGGraph graph;

        public StrategyManner(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {            
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("manner"))
                {
                    item.description = item.label;
                    item.f = item.label;
                    item.conceptualrole = item.label;
                }
            }            
        }
    }
}
