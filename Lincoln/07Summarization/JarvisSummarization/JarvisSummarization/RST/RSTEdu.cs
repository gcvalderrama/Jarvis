using JarvisSummarization.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.RST
{
    public class RSTEdu
    {
        public int name { get; set; }

        public int sentence { get; set; }

        private double _weight;

        public string text
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var item in this.tokens)
                {
                    sb.Append(string.Format(" {0} ", item.word));
                }
                return sb.ToString();
            }
        }

        public double weight
        {
            get { return _weight; }
            set {
                _weight = value;
                foreach (var item in this.tokens)
                {
                    item.rstweight = value;
                }
            }
        }

        [JsonIgnore]
        public List<Token> tokens { get; set; }

        public RSTEdu()
        {
            this.tokens = new List<Token>();  
        }
        
    }
}
