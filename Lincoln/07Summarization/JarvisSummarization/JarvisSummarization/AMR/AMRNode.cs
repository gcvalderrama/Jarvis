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
        public int graphid { get; set; }
        public int name { get; set; }
        public string text { get; set; }
        public string label { get; set; }
        public string nosuffix { get; set; }
        public bool isleaf { get; set; }
        public double rstweight { get; set; }
        public string kind { get; set; } //verb , concept

        public string description { get; set; }
        
        
        public void ReadXML(XElement nodeelement, int graphid)
        {
            this.graphid = graphid; 
            this.id = int.Parse(nodeelement.Attribute("gid").Value);
            this.name = int.Parse(nodeelement.Attribute("id").Value);
            this.text = nodeelement.Attribute("text").Value;
            this.label = nodeelement.Attribute("label").Value;
            this.nosuffix= nodeelement.Attribute("nosuffix").Value;
            this.isleaf = bool.Parse(nodeelement.Attribute("isleaf").Value);
            this.kind = "root";

        }
        
    }
}
