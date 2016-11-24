using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisSummarization.AMR
{
    public class AMRRelation
    {
        public int Head { get; set; }
        public int Tail { get; set; }
        public string label { get; set; }                
        
        
        public void ReadXML(XElement xmlrelation)
        {
            this.Head = int.Parse(xmlrelation.Attribute("head").Value);
            this.Tail = int.Parse(xmlrelation.Attribute("tail").Value);
            this.label= xmlrelation.Attribute("label").Value;
        }
        public AMRRelation Clone()
        {
            var rel = new AMRRelation();
            rel.Head = this.Head;
            rel.Tail = this.Tail;
            rel.label = this.label;            
            return rel;
        }
    }
}
