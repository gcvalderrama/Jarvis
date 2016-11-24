using JarvisSummarization.AMR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class CGNode
    {
        public int id { get; set; }

        public string text { get; set; }
        public string label { get; set; }        
        public string nosuffix { get; set; }
        public double rstweight { get; set; }                
        public double pagerank { get; set; }
        public string constant { get; set; }
        public int eduid { get; set; }        
        public string description { get; set; }
        public List<string> semanticroles { get; protected set; }        
        public List<int> FusionNodes { get; protected set; }
        
        public string hypernym { get; set; }

        public string log { get; set; }
        public CGNode() {
            this.semanticroles = new List<string>();
            this.FusionNodes = new List<int>(); 
        }
        public CGNode(AMRNode node, int eduid) : this() {            
            this.eduid = eduid;
            this.id = node.id;
            this.label = node.label;
            this.nosuffix = node.nosuffix;
            this.rstweight = node.rstweight;                        
            this.text = node.text;
        }
        public void AddSemanticRole(string role)
        {
            if (!this.semanticroles.Contains(role))
            {
                this.semanticroles.Add(role); 
            }
        }
        public void AddFusionNode(int Node)
        {
            if (!this.FusionNodes.Contains(Node))
            {
                this.FusionNodes.Add(Node);
            }
        }
        public CGNode Clone(int pid)
        {
            return new CGNode()
            {
                id = pid,
                eduid = this.eduid,                                
                label = this.label,
                nosuffix = this.nosuffix,
                rstweight = this.rstweight,
                text = this.text
            };
        }

    }
}
