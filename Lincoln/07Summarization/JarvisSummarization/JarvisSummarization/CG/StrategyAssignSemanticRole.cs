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
        private List<string> Terms = new List<string>() {
            "or", "and",
            "every",  "many" , "this", "any", "less" };
        public void Execute()
        {
            foreach (var item in this.graph.Nodes)
            {
                var in_rels = this.graph.Relations.Where(c => c.Tail == item.id);
                var out_rels = this.graph.Relations.Where(c => c.Head == item.id);
                if (item.text.Contains("-0"))
                {
                    item.AddSemanticRole("verb");
                }
                foreach (var rel in in_rels)
                {
                    int number;
                    if (Terms.Contains(item.nosuffix)  ||
                        rel.label.StartsWith("prep-")  || 
                        int.TryParse(item.nosuffix, out number))
                    {
                        item.AddSemanticRole("term");
                    }
                    else
                    {
                        item.AddSemanticRole(rel.conceptualrole); 
                    }
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
