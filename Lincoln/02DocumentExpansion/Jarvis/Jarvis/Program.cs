using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace Jarvis
{
        
    class Program
    {
        public class Mention
        {
            public int Sentence { get; set; }
            public int Start { get; set; }
            public int End { get; set; }
            public int Head { get; set; }
            public string Text { get; set; }

            public int GetLen()
            {
                return this.End - this.Start; 
            }
            public override string ToString()
            {
                return this.Text;
            }
        }
        public class Token
        {
            public int Id { get; set; }
            public string Word { get; set; }
            public int CharacterOffsetBegin { get; set; }
            public int CharacterOffsetEnd { get; set; }
            public string POS { get; set; }
        }
        public class Sentence
        {
            public int Id;
            public LinkedList<Token> Tokens = new LinkedList<Token>(); 
        }
        public class Coreference
        {
            public Mention Root { get; set; }
            public IList<Mention> Mentions { get; set; }
            public Coreference()
            {
                Mentions = new List<Mention>(); 
            }
        }

        static IList<Coreference> GetCoReferences(XElement document)
        {
            List<Coreference> result = new List<Coreference>();

            var coreferences = from c in document.Elements("document").Elements("coreference").Elements("coreference")
                            select c;

            foreach (var item in coreferences)
            {
                var co = new Coreference(); 

                var mentions = from c in item.Elements("mention")
                               select c;
                foreach (var mention in mentions)
                {
                    var m = new Mention()
                    {
                        Text = mention.Element("text").Value,
                        Head = int.Parse(mention.Element("head").Value),
                        Start = int.Parse(mention.Element("start").Value),
                        End = int.Parse(mention.Element("end").Value),
                        Sentence = int.Parse(mention.Element("sentence").Value)
                    };
                    if (mention.Attributes("representative").Count() != 0)
                    {
                        co.Root = m;
                    }
                    else
                    {
                        co.Mentions.Add(m);
                    }                                                         
                }
                result.Add(co);
            }
            return result; 
        }
        static IList<Sentence> GetSentences(XElement document)
        {
            List<Sentence> result = new List<Sentence>();

            
            var sentences = from c in document.Elements("document").Elements("sentences").Elements("sentence")
                            select c;

            int id = 1; 
            foreach (var sentence in sentences)
            {
                Sentence sen = new Sentence();
                sen.Id = id;   
                var tokens = from c in sentence.Elements("tokens").Elements("token")
                             select c;
                foreach (var token in tokens)
                {
                    var m = new Token()
                    {                          
                        CharacterOffsetBegin = int.Parse(token.Element("CharacterOffsetBegin").Value),
                        CharacterOffsetEnd = int.Parse(token.Element("CharacterOffsetEnd").Value),
                        Id = int.Parse(token.Attribute("id").Value),
                        Word = token.Element("word").Value,
                        POS = token.Element("POS").Value
                    };                    
                    sen.Tokens.AddLast(m); 
                }
                result.Add(sen);
                id++;
            }            
            return result;
        }
        static void Main(string[] args)
        {
            var valid_pos = new List<String>() { "NN", "NNS", "NNP", "VB", "VBD" };
            var directory = @"D:\Tesis2016\Jarvis\Lincoln\02DocumentExpansion\Input";
            foreach (var item in Directory.GetFiles(directory, "*.xml"))
            {
                var sb_document = new StringBuilder(); 
                var document = XElement.Load(item);
                var references =  GetCoReferences(document);
                var sentences = GetSentences(document);    
                            
                foreach (var sentence in sentences)
                {

                    #region coreference 
                    var senreference = references.Where(c => c.Mentions.Where(d => d.Sentence == sentence.Id).Count() > 0).ToList();
                    foreach (var core in senreference)
                    {
                        var head_node = sentence.Tokens.Where(c => c.Id == core.Root.Head).First();

                        if (!valid_pos.Contains(head_node.POS))
                        {
                            continue;
                        }
                        var mentions = core.Mentions.Where(c => c.Sentence == sentence.Id)
                            .OrderBy(c => c.Start).ToList();
                        foreach (var mention in mentions)
                        {

                            var start_token = sentence.Tokens.Where(c => c.Id == mention.Start).First();
                            var end_token = sentence.Tokens.Where(c => c.Id == mention.End - 1).First();
                            var cursor = sentence.Tokens.Find(start_token).Next;
                            var replace = new List<Token>();
                            for (int i = 0; i < mention.GetLen() - 1; i++)
                            {
                                replace.Add(cursor.Value);
                                cursor = cursor.Next;
                            }

                            foreach (var del in replace)
                            {
                                sentence.Tokens.Remove(del);
                            }

                            cursor = sentence.Tokens.Find(start_token);
                            cursor.Value = new Token()
                            {
                                Id = start_token.Id,
                                CharacterOffsetBegin = start_token.CharacterOffsetBegin,
                                CharacterOffsetEnd = end_token.CharacterOffsetEnd,
                                Word = core.Root.Text
                            };

                        }
                    }


                    #endregion

                    StringBuilder sb_sentence = new StringBuilder();
                    foreach (var token in sentence.Tokens)
                    {
                        sb_sentence.Append(token.Word + " ");
                    }
                    sb_document.AppendLine(sb_sentence.ToString());
                }
                File.WriteAllText(Path.GetFileName(item), sb_document.ToString());
            }            
        }
    }
}
