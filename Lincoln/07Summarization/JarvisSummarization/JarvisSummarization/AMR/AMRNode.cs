using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisSummarization.AMR
{
    
    public class AMRNode
    {
        public int id { get; set; }
        public int sentenceid { get; set; }
        public int name { get; set; }
        public string text { get; set; }
        public string label { get; set; }
        public string nosuffix { get; set; }
        public bool isleaf { get; set; }
        public double rstweight { get; set; }
        public string description { get; set; }

        public AMRNode Clone(int pid, int pname)
        {
            return new AMRNode() {
                id = pid,
                sentenceid =  this.sentenceid,
                name = pname,
                description = this.description,
                isleaf = this.isleaf,                
                label = this.label,
                nosuffix  = this.nosuffix,
                rstweight =  this.rstweight,
                text = this.text
            };
        }
               
        public void ReadXML(XElement nodeelement, int graphid)
        {
            this.sentenceid = graphid; 
            this.id = int.Parse(nodeelement.Attribute("gid").Value);
            this.name = int.Parse(nodeelement.Attribute("id").Value);
            this.text = nodeelement.Attribute("text").Value;
            this.label = nodeelement.Attribute("label").Value;
            this.nosuffix= nodeelement.Attribute("nosuffix").Value;
            this.isleaf = bool.Parse(nodeelement.Attribute("isleaf").Value);
        }
        
    }
}
