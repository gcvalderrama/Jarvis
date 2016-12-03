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

                var in_rels = this.graph.Relations.Where(c => c.Tail == item.id);
                var out_rels = this.graph.Relations.Where(c => c.Head == item.id);


                if (in_rels.Union(out_rels).Where(c => c.label == "mod" || c.label == "domain").ToList().Count() > 0)
                {

                }
                else
                {
                    item.nosuffix = "can";
                    item.log += "change possible to can";
                    item.text = "can-01";
                }               
            }
        }
    }
}
