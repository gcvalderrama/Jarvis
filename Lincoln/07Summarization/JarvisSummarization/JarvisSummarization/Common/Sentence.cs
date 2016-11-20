using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisSummarization.Common
{
    public class Sentence
    {
        public int name { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public List<Token> tokens { get; set; }
      
        public string text { get {
            var sb = new StringBuilder();
            foreach (var item in this.tokens)
            {
                sb.Append(string.Format(" {0} ", item.word));
            }
            return sb.ToString();
        } }

        public Sentence() {
            this.tokens = new List<Token>();
        }
        public void ReadFromXML(XElement element)
        {
            this.name = int.Parse(element.Attribute("id").Value);

        }
    }
}
