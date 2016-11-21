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
        public string kind { get; set; } //verb , concept
        public string description { get; set; }
        public int sentenceid { get; set; }
        public string semanticrole { get; set; }
        public CGNode() { }
        public CGNode(AMRNode node, int sentence) {
            this.sentenceid = sentence;
            this.id = node.id;
            this.label = node.label;
            this.nosuffix = node.nosuffix;
            this.rstweight = node.rstweight;
            this.kind = node.kind;
            this.description = node.description;
            this.text = node.text;
        }
        public CGNode Clone(int pid)
        {
            return new CGNode()
            {
                id = pid,
                sentenceid = this.sentenceid,                
                description = this.description,                
                kind = this.kind,
                label = this.label,
                nosuffix = this.nosuffix,
                rstweight = this.rstweight,
                text = this.text
            };
        }

    }
}
