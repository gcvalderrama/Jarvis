using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class CGInformativeAspect
    {
        public double weight { get {
                return this.Who.Select(c => c.pagerank).Sum() + 
                    this.What.Select(c => c.pagerank).Sum() + 
                    this.Who_affected.Select(c=>c.pagerank).Sum() +
                    this.Why.Select(c=>c.pagerank).Sum();
            } }
        [JsonIgnore]
        public List<CGNode> Who { get; set; } //agent
        [JsonIgnore]
        public List<CGNode> What { get; set; } // rel
        [JsonIgnore]
        public List<CGNode> Who_affected { get; set; } // patient or gol
        [JsonIgnore]
        public List<CGNode> Why { get; set; }
        public int name { get; set; }
        public CGInformativeAspect()
        {
            this.Who = new List<CGNode>();
            this.What = new List<CGNode>();
            this.Who_affected = new List<CGNode>();
            this.Why = new List<CGNode>();
        }

        //pending 
        public string Where { get; set; }        
        public string How { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in this.Who)
            {
                sb.Append(string.Format(" who: {0} ({1}) ", item.nosuffix , item.id));
            }
            foreach (var item in this.What)
            {
                sb.Append(string.Format(" what: {0} ({1}) ", item.nosuffix, item.id));
            }
            foreach (var item in this.Who_affected)
            {
                sb.Append(string.Format(" who_a: {0} ({1}) ", item.nosuffix, item.id));
            }
            foreach (var item in this.Why)
            {
                sb.Append(string.Format(" why: {0} ({1}) ", item.nosuffix, item.id));
            }
            return string.Format(" {0} : {1}  ", this.weight, sb.ToString());
        }

    }
}
