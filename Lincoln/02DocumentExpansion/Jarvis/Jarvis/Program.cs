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
            public bool Root { get; set; }
        }
        public class Token
        {
            public int Id { get; set; }
            public string Word { get; set; }
            public int CharacterOffsetBegin { get; set; }
            public int CharacterOffsetEnd { get; set; }
        }

        static IList<Mention> GetCoReferences(XElement document)
        {
            List<Mention> result = new List<Mention>();

            var coreferences = from c in document.Elements("document").Elements("coreference").Elements("coreference")
                            select c;

            foreach (var item in coreferences)
            {
                var mentions = from c in item.Elements("mention")
                               select c;
                foreach (var mention in mentions)
                {
                    result.Add(
                        new Mention() {
                            Text = mention.Element("text").Value,
                            Head = int.Parse(mention.Element("head").Value),
                            Start = int.Parse(mention.Element("start").Value),
                            End = int.Parse(mention.Element("end").Value)
                        });
                }
            }

            return result; 
        }
        static void Main(string[] args)
        {
            var directory = @"D:\Tesis2016\Jarvis\Lincoln\02DocumentExpansion\Input";
            foreach (var item in Directory.GetFiles(directory, "*.xml"))
            {


                var document = XElement.Load(item);

                var references =  GetCoReferences(document);

                foreach (var mention in references)
                {
                    Console.WriteLine(mention.Text);
                }

                var sentences = from c in document.Elements("document").Elements("sentences").Elements("sentence")
                             select c;

                foreach (var sentence in sentences)
                {

                    var tokens = from c in sentence.Elements("tokens").Elements("token")
                                 select c;

                    foreach (var token in tokens)
                    {
                        Console.WriteLine(token.ToString());
                    }
                }
            }
        }
    }
}
