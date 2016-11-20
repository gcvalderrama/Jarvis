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
        public WordNet.WordNetEngine _wordNetEngine = new WordNet.WordNetEngine(@"D:\Tesis2016\WordnetAPI\resources\", false);
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
                    var str = File.ReadAllText(Path.Combine(this.propbankPath, node.nosuffix) + ".xml");

                    var propbankelements = XElement.Parse(str);

                    var propbankelement = (from c in propbankelements.Elements("predicate").Elements("roleset")
                                where c.Attribute("id").Value == node.text.Replace("-", ".")
                                select c).First();

                    node.description = propbankelement.Attribute("name").Value;

                    var relations = g.Relations.Where(c => 
                            c.Head == node.name && c.label.Contains("ARG") ).ToList();                    

                    foreach (var relation in relations)
                    {
                        var number = relation.label.Replace("ARG", "");

                        var roleelement = (from c in propbankelement.Elements("roles").Elements("role")
                                          where c.Attribute("n").Value == number
                                          select c).First();

                        relation.description = roleelement.Attribute("descr").Value;
                        relation.role = roleelement.Attribute("f").Value;
                        var vnx = roleelement.Element("vnrole");
                        if (vnx != null)
                        {
                            relation.vncls = vnx.Attribute("vncls").Value;
                            relation.vntheta = vnx.Attribute("vntheta").Value;
                        }
                    }                   
                }                
            }
        }

        //we will create a virtual AMRGraph that has access to all nodes and guarantee a minimum conection 
        public void ProcessARGOf()
        {
            foreach (var g in this.Graphs)
            {
                var relations = from c in g.Relations.Where(c => c.label.Contains("-of"))
                                select c;

                foreach (var item in relations)
                {
                    item.label = item.label.Replace("-of", "");
                    var tmp = item.Head;
                    item.Head = item.Tail;
                    item.Tail = tmp;                     
                }
            }
        }
        public void Digest()
        {
            ProcessARGOf();
            ProcessKind();
        //    ProcessRole();
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
