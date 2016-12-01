using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.Common
{
    public class Document
    {
        public string name { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public List<Sentence> sentences { get; private set; }

        public int NumberOfWords { get; set; }
        public Document()
        {
            this.sentences = new List<Sentence>();
        }
        public void AddSentence(Sentence s)
        {
            this.sentences.Add(s);
        }
    }
}
