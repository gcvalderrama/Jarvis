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
        
        public int  sentenceid { get; set; }        
        public bool IsPatientVerb { get; set; }
        public string description { get; set; }

        public string constant { get; set; }

        public List<string> semanticroles { get; protected set; }        
        public List<string> FusionNodes { get; protected set; }
        
        public string hypernym { get; set; }

        public string log { get; set; }
        public CGNode() {
            this.semanticroles = new List<string>();
            this.FusionNodes = new List<string>(); 
        }
        public CGNode(AMRNode node, int sentenceid) : this() {            
            this.sentenceid = sentenceid;
            this.id = node.id;
            this.label = node.label;
            this.nosuffix = node.nosuffix;
            this.rstweight = node.rstweight;                        
            this.text = node.text;
        }
        public void AddSemanticRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ApplicationException("error");
            if (!this.semanticroles.Contains(role))
            {
                this.semanticroles.Add(role); 
            }
        }
        public void AddFusionNode(CGNode Node)
        {
            string format = string.Format("{0}:{1}:{2}",Node.sentenceid, Node.id, Node.nosuffix);
            if (!this.FusionNodes.Contains(format))
            {
                this.FusionNodes.Add(format);
            }
        }
        public CGNode Clone(int pid)
        {
            return new CGNode()
            {
                id = pid,
                sentenceid = this.sentenceid,                                
                label = this.label,
                nosuffix = this.nosuffix,
                rstweight = this.rstweight,
                text = this.text
            };
        }

    }
}
