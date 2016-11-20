using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisSummarization.AMR
{
    public class AMRGraph
    {
        public int name { get; set; }
        [JsonIgnore]
        public List<AMRNode> Nodes { get; set; }
        [JsonIgnore]
        public List<AMRRelation> Relations { get; set;  }
        public AMRGraph()
        {
            this.Nodes = new List<AMRNode>();
            this.Relations = new List<AMRRelation>();

        }
        public void ReadXML(XElement graphelement)
        {
            name = int.Parse(graphelement.Attribute("id").Value);

            var xmlnodes = from c in graphelement.Elements("nodes").Elements("node")
                           select c;

            foreach (var xmlnode in xmlnodes)
            {
                var node = new AMRNode();
                node.ReadXML(xmlnode, this.name);
                this.Nodes.Add(node);
            }

            var edgesxml = from c in graphelement.Elements("edges").Elements("edge")
                           select c;

            foreach (var xmledge in edgesxml)
            {
                var relation = new AMRRelation();
                relation.ReadXML(xmledge);
                this.Relations.Add(relation);                
            }
        }
    }
}
