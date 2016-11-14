using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisRDFToNEO4J
{
    public class AMRNode
    {
        public List<AMRNode> Nodes { get; set; }
        public Uri Relation { get; set; }
        public bool Direction = true;
        public double RSTWeight { get; set; }
        public Uri Id { get; set; }
        public Uri AmrTypeUri { get; set; }
        public string AmrType { get; set; }
        public Uri PropBank { get; set; }
        public AMRNode()
        {
            this.Nodes = new List<AMRNode>();
        }        
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
        
        public string Name
        {
            get
            {
                return Id.Fragment.Replace("#", "").Replace("-", "_");
            }
        }
                
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


        #region Functions

        public void ApplyRSTWeight(double weight, List<AMRNode> State)
        {
            this.RSTWeight = weight;
            foreach (var item in this.Nodes)
            {
                ApplyRSTWeight(weight, State);
            }
        }       

        #endregion


    }
}
