using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class CGWhy
    {
        public double pagerank
        {
            get
            {
                return this.Items.Sum(c => c.Item1.pagerank + c.Item2.Sum(d => d.pagerank));
            }
        }
        public List<Tuple<CGNode, IEnumerable<CGNode>>> Items { get; set; }
        public CGWhy()
        {
            this.Items = new List<Tuple<CGNode, IEnumerable<CGNode>>>();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in this.Items)
            {
                sb.Append("why :" + item.Item1.text);
                foreach (var modifier in item.Item2)
                {
                    sb.Append("why modifier " + modifier.text);
                }
            }
            return sb.ToString();
        }
    }

    public class CGWhoAffected
    {
        public double pagerank
        {
            get
            {
                return this.Items.Sum(c => c.Item1.pagerank + c.Item2.Sum(d => d.pagerank));
            }
        }
        public List<Tuple<CGNode, IEnumerable<CGNode>>> Items { get; set; }
        public CGWhoAffected()
        {
            this.Items = new List<Tuple<CGNode, IEnumerable<CGNode>>>();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in this.Items)
            {
                sb.AppendLine("who affected:" + item.Item1.text);
                foreach (var modifier in item.Item2)
                {
                    sb.AppendLine("who affected modifier " + modifier.text);
                }
            }
            return sb.ToString();
        }
    }
    public class CGWho
    {        
        public double pagerank { get {
                return this.Items.Sum(c => c.Item1.pagerank + c.Item2.Sum(d => d.pagerank));
            } }
        public List<Tuple<CGNode, IEnumerable<CGNode>>> Items { get; set; }
        public CGWho()
        {
            this.Items = new List<Tuple<CGNode, IEnumerable<CGNode>>>();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in this.Items)
            {
                sb.Append("who :" + item.Item1.text);
                foreach (var modifier in item.Item2)
                {
                    sb.Append("who modifier " + modifier.text);
                }
            }
            return sb.ToString();
        }
    }
    public class CGWhat
    {
        public double pagerank
        {
            get
            {
                return this.Items.Sum(c => c.Item1.pagerank + c.Item2.Sum(d => d.pagerank));
            }
        }
        public List<Tuple<CGNode, IEnumerable<CGNode>>> Items { get; set; }
        public CGWhat()
        {
            this.Items = new List<Tuple<CGNode, IEnumerable<CGNode>>>();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in this.Items)
            {
                sb.Append("what :" + item.Item1.text);
                foreach (var modifier in item.Item2)
                {
                    sb.Append("what modifier " + modifier.text);
                }
            }
            return sb.ToString();
        }
    }
    public class CGInformativeAspect
    {
        public double weight { get {
                return this.Who.pagerank + 
                    this.What.pagerank + 
                    this.Who_affected.pagerank  +
                    this.Why.pagerank;
            } }
        [JsonIgnore]
        public CGWho Who { get; set; } //agent
        [JsonIgnore]
        public CGWhat What { get; set; } // rel
        [JsonIgnore]
        public CGWhoAffected Who_affected { get; set; } // patient or gol
        [JsonIgnore]
        public CGWhy Why { get; set; }
        public int name { get; set; }
        public CGInformativeAspect()
        {
            this.Who = new CGWho(); 
            this.What = new CGWhat();
            this.Who_affected = new CGWhoAffected();
            this.Why = new CGWhy();
        }

        //pending 
        public string Where { get; set; }        
        public string How { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("==========================================================================");
            sb.AppendLine(this.Who.ToString());            
            sb.AppendLine(this.What.ToString());
            sb.AppendLine(this.Who_affected.ToString());
            sb.AppendLine(this.Why.ToString());
            sb.AppendLine(string.Format("weight {0}", this.weight) );
            sb.AppendLine("==========================================================================");
            return sb.ToString();
        }

    }
}
