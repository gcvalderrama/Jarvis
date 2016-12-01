using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyPossibleToVerb
    {
        private CGGraph graph;
        public StrategyPossibleToVerb(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            var nodes = this.graph.Nodes.Where(c => c.text == "possible");

            foreach (var item in nodes)
            {
                item.nosuffix = "can";                
                item.log += "change possible to can";
                item.text = "can-01";
            }
        }
    }
}
