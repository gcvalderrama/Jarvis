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
            foreach (var node in graph.Nodes)
            {
                var in_relations = graph.Relations.Where(c => c.Tail == node.id);
                if (in_relations.Count() > 0)
                {
                    foreach (var rel in in_relations)
                    {
                        node.AddSemanticRole(rel.f);
                    }
                }
                else {
                    //caso no relaciones puede ser un verbo o agente
                    if (node.text.Contains("-0"))
                    {
                        node.AddSemanticRole("rel");
                    }
                    else {
                        node.AddSemanticRole("pag");
                    }
                }
                //siempre si es un verbo es un rel 
                if (node.text.Contains("-0"))
                {
                    node.AddSemanticRole("rel");
                }
            }

            foreach (var node in graph.Nodes.Where(c=>c.semanticroles.Contains("rel")))
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
