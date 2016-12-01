using JarvisSummarization.AMR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class CGRelation
    {
        public int Head { get; set; }
        public int Tail { get; set; }
        public string label { get; set; }
        public string vncls { get; set; }
        public string vntheta { get; set; }
        public string description { get; set; }
        public string f { get; set; }
        public string log { get; set; }
        public int ocurrences { get; set; }
        public string conceptualrole { get; set; }
        
        public CGRelation()
        {

        }
        public CGRelation(AMRRelation relation)
        {
            this.Head = relation.Head;
            this.Tail = relation.Tail;
            this.label = relation.label;            
        }
        public CGRelation Clone()
        {
            var rel = new CGRelation();
            rel.Head = this.Head;
            rel.Tail = this.Tail;
            rel.label = this.label;
            rel.vncls = this.vncls;
            rel.vntheta = this.vntheta;
            rel.description = this.description;
            rel.conceptualrole = this.conceptualrole;
            rel.f = this.f;
            return rel;
        }
    }
}
