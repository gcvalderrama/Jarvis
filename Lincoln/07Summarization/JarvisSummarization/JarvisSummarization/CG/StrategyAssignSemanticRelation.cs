using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisSummarization.CG
{
    class StrategyAssignSemanticRelation
    {
        private string propbankPath = @"D:\Tesis2016\Propbank\frames";
        private CGGraph graph;
        public StrategyAssignSemanticRelation(CGGraph graph)
        {
            this.graph = graph;            
        }
        
        private void ManageNotFoundArg(CGRelation item)
        {
            item.description = "arg not found in propbank";
            if (item.label == "ARG0")
            {
                item.f = "pag";
                item.conceptualrole = "agent";
            }
            else if (item.label == "ARG1")
            {
                item.f = "ppt";
                item.conceptualrole = "patient";
            }
            else if (item.label == "ARG2")
            {
                item.f = "gol";
                item.conceptualrole = "goal";
            }
        }
        private void ExecuteForVerbs(CGNode node, IEnumerable<CGRelation> out_relations)
        {
            //if node is not a verb we can not assign any role
            var currentPath = System.IO.Path.Combine(this.propbankPath, node.nosuffix) + ".xml";            
             if (!System.IO.File.Exists(currentPath))
            {
                foreach (var item in out_relations)
                {
                    ManageNotFoundArg(item);
                }        
                return;
            }            
            var str = System.IO.File.ReadAllText(currentPath);

            var propbankelements = XElement.Parse(str);
            
            var propbankelement = (from c in propbankelements.Elements("predicate").Elements("roleset")
                                   select c).ElementAt(int.Parse(node.text.Replace(node.nosuffix + "-", "")) - 1);                        
                        
            foreach (var relation in out_relations)
            {
                if (relation.label.Contains("ARG"))
                {
                    var number = relation.label.Replace("ARG", "");

                    var roleelement = (from c in propbankelement.Elements("roles").Elements("role")
                                       where c.Attribute("n").Value == number
                                       select c).FirstOrDefault();
                    if (roleelement != null)
                    {
                        relation.description = roleelement.Attribute("descr").Value.Replace("/", "").Replace("'", "").Replace(@"\", "");
                        relation.f = roleelement.Attribute("f").Value.ToLower();

                        var vnx = roleelement.Element("vnrole");
                        if (vnx != null)
                        {
                            relation.vncls = vnx.Attribute("vncls").Value;
                            relation.vntheta = vnx.Attribute("vntheta").Value;
                            relation.conceptualrole = relation.vntheta.ToLower();
                        }
                        else
                        {
                            ManageNotFoundArg(relation);                            
                        }
                    }
                    
                }
                
            }
        }

        public void Execute()
        {            
            //only to verbs
            foreach (var node in graph.Nodes.Where(c=>c.text.Contains("-0")))
            {
                var out_relations_verbs = graph.Relations.Where(c => c.Head == node.id).ToList();
                this.ExecuteForVerbs(node, out_relations_verbs);                
            }
            foreach (var item in graph.Relations)
            {
                if (string.IsNullOrWhiteSpace("unknow") || string.IsNullOrWhiteSpace(item.conceptualrole))
                {
                    var head = this.graph.Nodes.Where(c => c.id == item.Head).First();
                    var tail = this.graph.Nodes.Where(c => c.id == item.Tail).First();
                    throw new ApplicationException("error");
                }
            }
        }
    }
}
