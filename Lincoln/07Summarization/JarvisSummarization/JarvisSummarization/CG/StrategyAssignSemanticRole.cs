using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisSummarization.CG
{
    class StrategyAssignSemanticRole
    {
        private string propbankPath = @"D:\Tesis2016\Propbank\frames";
        private CGGraph graph;
        public StrategyAssignSemanticRole(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            foreach (var item in this.graph.Nodes)
            {
                if (item.text.Contains("-0"))
                {
                    item.AddSemanticRole("verb");
                }
                else
                {
                    item.AddSemanticRole("concept");
                }
            }             
        }
    }
}
