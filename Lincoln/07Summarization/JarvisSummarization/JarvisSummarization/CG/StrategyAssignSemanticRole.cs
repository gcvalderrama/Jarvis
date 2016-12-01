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
            foreach (var node in graph.Nodes.Where(c=>c.semanticroles.Contains("verb")))
            {
                var currentPath = System.IO.Path.Combine(this.propbankPath, node.nosuffix) + ".xml";
                if (System.IO.File.Exists(currentPath))
                {
                    var str = System.IO.File.ReadAllText(currentPath);
                    var propbankelements = XElement.Parse(str);
                    var propbankelement = (from c in propbankelements.Elements("predicate").Elements("roleset")
                                           select c).ElementAt(int.Parse(node.text.Replace(node.nosuffix + "-", "")) - 1);

                    var roleelement = (from c in propbankelement.Elements("roles").Elements("role")
                                       where c.Attribute("n").Value == "0"
                                       select c).FirstOrDefault();

                    if (roleelement == null)
                        node.IsPatientVerb = true;
                }
            }                  
        }
    }
}
