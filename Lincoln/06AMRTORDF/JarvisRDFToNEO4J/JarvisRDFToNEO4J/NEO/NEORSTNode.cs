using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisRDFToNEO4J.NEO
{
    public class NEORSTNode
    {
        public string name { get; set; }
        public int edu { get; set; }
        public string form { get; set; }
        public string text { get; set; }
        public double weight { get; set; }
        public string relation { get; set; }
        public bool leaf { get; set; }

        internal List<NEORSTRelation> Relations { get; set; }
        public NEORSTNode() {
            this.Relations = new List<NEORSTRelation>();
        }        
        public NEORSTNode(RSTNode Node)
        {
            this.edu = Node.edu;
            this.form = Node.form;
            this.text = Node.text;
            this.weight = Node.Weight;
            this.relation = Node.relation;
            this.leaf = Node.leaf;
            this.Relations = new List<NEORSTRelation>(); 
            foreach (var item in Node.Children)
            {
                var rel = new NEORSTRelation();
                rel.relation = item.type.ToString();
                rel.Child = new NEORSTNode(item); 
                this.Relations.Add(rel);
            }                       
        }
    }
}
