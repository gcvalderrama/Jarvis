using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WordNet = LAIR.ResourceAPIs.WordNet;
namespace JarvisSummarization.AMR
{
    public class AMRDocument
    {
        public List<AMRGraph> Graphs { get; set; }
        
        private string propbankPath;
        public AMRDocument(string propbankPath)
        {
            this.propbankPath = propbankPath; 
            this.Graphs = new List<AMRGraph>(); 
        }
        private void ProcessKind()
        {
            foreach (var g in this.Graphs)
            {
                foreach (var node in g.Nodes)
                {
                    if (node.text.Contains("-0"))
                    {
                        node.kind = "verb";
                    }
                    else
                    {
                        var in_relations = g.Relations.Where(c => c.Tail == node.name);
                        if (in_relations.Count() == 0) continue;
                        if (in_relations.Count() > 1)
                        {
                            node.kind = "concept";
                            continue;
                        }
                        else
                        {
                            var relation = in_relations.First();
                            if (relation.label.Contains("ARG"))
                            {
                                node.kind = "concept";
                            }
                            else
                            {
                                node.kind = relation.label;
                            }
                        }
                    }
                }
            }
        }

        private void ProcessRole()
        {
            foreach (var g in this.Graphs)
            {
                foreach (var node in g.Nodes.Where(c=>c.kind== "verb").ToList())
                {
                    var relations = g.Relations.Where(c =>
                            c.Head == node.name && c.label.Contains("ARG")).ToList();

                    var currentPath = Path.Combine(this.propbankPath, node.nosuffix) + ".xml";

                    if (!File.Exists(currentPath))
                    {
                        node.description = "unknow";
                        foreach (var relation in relations)
                        {
                            relation.description = "unknow-propbank";                            
                            relation.f = "unknow";
                        }
                        continue;
                    }

                    var str = File.ReadAllText(currentPath);

                    var propbankelements = XElement.Parse(str);

                    var propbankelement = (from c in propbankelements.Elements("predicate").Elements("roleset")                                
                                select c).ElementAt( int.Parse(node.text.Replace(node.nosuffix + "-", "")) -1 );

                    node.description = propbankelement.Attribute("name").Value;
                    
                    foreach (var relation in relations)
                    {
                        if (!relation.label.Contains("-of"))
                        {
                            var number = relation.label.Replace("ARG", "");

                            var roleelement = (from c in propbankelement.Elements("roles").Elements("role")
                                               where c.Attribute("n").Value == number
                                               select c).First();

                            relation.description = roleelement.Attribute("descr").Value.Replace("/", "").Replace("'", "").Replace(@"\", "");
                            relation.f = roleelement.Attribute("f").Value.ToLower();

                            var tail = g.Nodes.Where(c => c.name == relation.Tail).First();
                            
                            var vnx = roleelement.Element("vnrole");
                            if (vnx != null)
                            {
                                relation.vncls = vnx.Attribute("vncls").Value;
                                relation.vntheta = vnx.Attribute("vntheta").Value;
                            }
                        }
                        else
                        {
                            relation.description = "unknow-of";                            
                            relation.f = "unknow";
                        }
                    }                   
                }                
            }
        }

        //we will create a virtual AMRGraph that has access to all nodes and guarantee a minimum conection                  
        public void Digest()
        {   
            ProcessKind();            
            ProcessRole();
        }
        public void LoadRSTInformation(RST.RSTDocument Document)
        {
            foreach (var graph in this.Graphs)
            {
                var tokens = Document.Tokens.Where(c => c.sentence == graph.name).OrderBy(c=>c.sentencepos).ToList();
                foreach (var node in graph.Nodes)
                {
                    var token = tokens.Where(c => c.lemma == node.nosuffix).OrderByDescending(c=>c.rstweight).FirstOrDefault();
                    if (token != null)
                    {
                        node.rstweight = token.rstweight;
                    }
                }
            }            
        }
    }
}
