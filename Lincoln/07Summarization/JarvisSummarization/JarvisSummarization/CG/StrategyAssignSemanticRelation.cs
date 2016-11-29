﻿using System;
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
        private void ManageRelation(CGRelation relation)
        {
            var head = this.graph.Nodes.Where(c => c.id == relation.Head).First();
            var tail = this.graph.Nodes.Where(c => c.id == relation.Tail).First();
                        
            if (relation.label == "ARG0")
            {
                relation.description = "non prop bank found relation, default arg0 agent";
                relation.f = "pag";
            }
            else if (relation.label == "ARG1")
            {
                relation.description = "non prop bank found relation, default arg1 patient";
                relation.f = "ppt";
            }
            else if (relation.label == "ARG2")
            {
                //instrument/attribute
                relation.description = "non prop bank found relation, default arg2 gol";
                relation.f = "gol";
            }
            else if (relation.label == "ARG3")
            {
                //instrument/attribute
                relation.description = "non prop bank found relation, default arg3 start";
                relation.f = "start";
            }
            else if ( string.IsNullOrWhiteSpace(relation.f))
            {
                relation.description = "unknow-propbank";
                relation.f = "unknow";
            }

        }
        private void ExecuteForVerbs(CGNode node, IEnumerable<CGRelation> out_relations)
        {
            //if node is not a verb we can not assign any role
            var currentPath = System.IO.Path.Combine(this.propbankPath, node.nosuffix) + ".xml";
            if (!node.text.Contains("-0") || !System.IO.File.Exists(currentPath))
            {
                foreach (var relation in out_relations)
                {
                    ManageRelation(relation);
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
                        }
                    }
                    else
                    {
                        ManageRelation(relation);
                    }
                }
                else
                {
                    ManageRelation(relation);
                }
            }
        }

        public void Execute()
        {
            
            //only to verbs
            foreach (var node in graph.Nodes)
            {
                var out_relations_verbs = graph.Relations.Where(c => c.Head == node.id).ToList();
                this.ExecuteForVerbs(node, out_relations_verbs);                
            }

            foreach (var item in graph.Relations)
            {
                if (item.f.Contains("unknow"))
                {
                    var head = this.graph.Nodes.Where(c => c.id == item.Head).First();
                    var tail = this.graph.Nodes.Where(c => c.id == item.Tail).First();
                    throw new ApplicationException("error");
                }
            }
        }
    }
}
