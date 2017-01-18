using GenerateDocsFromSintacticParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Jarvis
{
    public class Mention
    {
        public bool Enable = true;
        public int Sentence { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int HeadLoc { get; set; }
        public Token Head { get; set; }
        public string Text { get; set; }
        public Mention Root { get; set; }
        public int GetLen()
        {
            return this.End - this.Start;
        }
        public override string ToString()
        {
            return this.Text;
        }
        public void Read(XElement mention, IList<Sentence> sentences)
        {
            this.Sentence = int.Parse(mention.Element("sentence").Value);
            this.HeadLoc = int.Parse(mention.Element("head").Value);
            this.Start = int.Parse(mention.Element("start").Value);
            this.End = int.Parse(mention.Element("end").Value);
            var senInstance = sentences.Where(c => c.Id == this.Sentence).First();

            var sb = new StringBuilder();
            for (int i = Start-1; i < End-1; i++)
            {
                sb.Append(WordHelper.ProcessWord(senInstance, i));
            }            
            this.Text = sb.ToString();                        
        }
        public bool Contains(Mention mention)
        {
            return  this.Sentence == mention.Sentence && this.Start <= mention.Start && this.End >= mention.End; 
        }
    }
}
