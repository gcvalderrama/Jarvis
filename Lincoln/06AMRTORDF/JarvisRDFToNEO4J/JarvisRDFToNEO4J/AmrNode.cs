using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisRDFToNEO4J
{
    public class AmrNode
    {

        public AmrNode()
        {
            this.Nodes = new List<AmrNode>();
        }
        public Uri Relation { get; set; }
        public bool Direction = true;
        
        
        public string RelationName
        {
            get
            {
                
                string result = null;
                if (!string.IsNullOrWhiteSpace(this.Relation.Fragment))
                {
                    result = Relation.Fragment.Replace("#", "").Replace("-", "_");
                }
                else
                {
                    result = Relation.Segments.Last().Replace("-","_").Replace(".", "_");
                }

                if (!Direction)
                    result = "OF-" + result;
                return result; 

            }
        }
        public Uri Id { get; set; }
        public string Name
        {
            get
            {
                return Id.Fragment.Replace("#", "").Replace("-", "_");
            }
        }
        public Uri AmrTypeUri { get; set; }
        public string AmrType { get; set; }
        public Uri PropBank { get; set; }
        
        public string PropBankName
        {
            get
            {
                if (PropBank.ToString().StartsWith("http://amr.isi.edu/frames/ld/v1.2.2"))
                {
                    return PropBank.ToString().Replace("http://amr.isi.edu/frames/ld/v1.2.2/", "");

                }
                else
                {
                    return PropBank.Fragment.Replace("#", "").Replace("-", "_");
                }                
            }
        }
        public string Description
        {
            get
            {
                return string.Format("{0} {1}", this.Name, this.PropBankName);
            }
        }

        public List<AmrNode> Nodes { get; set; }

    }
}
