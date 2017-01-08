using Jarvis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GenerateDocsFromSintacticParser
{
    class Program
    {
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
                int loc = 1;
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
                    m.SentenceLoc = loc;
                    sen.Tokens.AddLast(m);
                    loc++;
                }
                result.Add(sen);
                id++;
            }
            return result;
        }


        static void Main(string[] args)
        {
            string InputDir = @"D:\Tesis2016\Jarvis\Lincoln\00Input\Output";
            string OutputDir = @"D:\Tesis2016\Jarvis\Lincoln\01DocumentExpansion\OutputClean";

            foreach (var item in Directory.GetFiles(InputDir))
            {
                var sb_document = new StringBuilder();
                var document = XElement.Load(item);
                var sentences = GetSentences(document);
                foreach (var sentence in sentences)
                {
                    StringBuilder sb_sentence = new StringBuilder();
                    for (int i = 0; i < sentence.Tokens.Count; i++)
                    {
                        Token next = null;
                        if (i + 1 < sentence.Tokens.Count)
                        {
                            next = sentence.Tokens.ElementAt(i + 1);
                        }
                        var word = sentence.Tokens.ElementAt(i);
                        if (next != null)
                        {
                            if (next.Word == "'s" || next.Word == ",")
                            {
                                sb_sentence.Append(word.Word);
                            }                                
                            else
                            {
                                if (word.Word == "-LRB-")
                                {
                                    sb_sentence.Append("(");
                                }
                                else if (word.Word == "-RRB-")
                                {
                                    sb_sentence.Append(")");
                                }
                                else
                                {
                                    sb_sentence.Append(word.Word + " ");
                                }

                            }
                        }

                    }
                    sb_document.AppendLine(sb_sentence.ToString());
                }
                var ouputfile = Path.Combine(OutputDir, Path.GetFileNameWithoutExtension(item));                
                File.WriteAllText(ouputfile + ".txt", sb_document.ToString().Replace(" 's ", "'s ").Replace("''", "\"").Replace("``", "\""));
            }
        }
    }
}
