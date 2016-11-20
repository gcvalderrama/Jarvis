using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisSummarization.Common
{
    public class Token
    {
        public int name { get; set; }
        public int sentencepos { get; set; }
        public string word { get; set; }
        public string lemma { get; set; }
        public string pos { get; set; }
        public string ner { get; set; }
        public int sentence { get; set; }
        #region RST
        public int eduid { get; set; }

        private double _rstweight;
        public double rstweight {
            get {
                return this._rstweight;
            }
            set {
                if (value > this._rstweight) //update with the most valuable rst score
                    this._rstweight = value;
            } }
        #endregion

        public void ReadFromXML(XElement element, int name, int sentence)
        {
            this.sentence = sentence;
            this.sentencepos = int.Parse(element.Attribute("id").Value);
            this.name = name;            
            this.word = element.Element("word").Value;
            this.lemma = element.Element("lemma").Value;
            this.pos = element.Element("POS").Value;
            this.ner = element.Element("NER").Value;
        }
    }
}
