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
            string InputDir = @"D:\Tesis2016\Jarvis\Final\Training\01Coreference-Output";
            string OutputDir = @"D:\Tesis2016\Jarvis\Final\Training\021NoDocumentExpantion";

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

                        if (word.Word == "-LRB-")
                        {
                            sb_sentence.Append("(");
                        }
                        else if (word.Word == "-RRB-")
                        {
                            sb_sentence.Append(")");
                        }
                        else if ( string.Compare(word.Word , "'s", true) == 0 && (word.POS == "VBP" || word.POS == "VBZ"))
                        {
                            sb_sentence.Append("is ");
                        }
                        else if (string.Compare(word.Word, "'re", true)  == 0 && word.POS == "VBP")
                        {
                            sb_sentence.Append("are ");
                        }
                        else if (string.Compare(word.Word, "'m", true) == 0 && word.POS == "VBP")
                        {
                            sb_sentence.Append("am ");
                        }
                        else if (word.Word == "'ve" && (word.POS == "VBP" || word.POS == "VB"))
                        {
                            sb_sentence.Append("have ");
                        }
                        else if (word.Word == "'ll" && word.POS == "MD")
                        {
                            sb_sentence.Append("will ");
                        }
                        else if (word.Word == "'d" && word.POS == "MD")
                        {
                            sb_sentence.Append("would ");
                        }

                        else if (word.Word == "n't" && word.POS == "RB")
                        {
                            sb_sentence.Append("not ");
                        }
                        else if (word.Word == "'10s" || word.Word == "'20s" || word.Word == "'30s" || word.Word == "'40s" ||
                            word.Word == "'50s" || word.Word == "'60s" || word.Word == "'70s" || word.Word == "'80s" ||
                            word.Word == "'90s")
                        {
                            sb_sentence.Append(word.Word.Replace("'", "") + " ");
                        }
                        else if (word.Word == "." || word.Word == "`" || word.Word == "'" || word.Word == "''" || word.Word == "``")
                        {

                        }
                        else
                        {                            
                            if (next != null)
                            {
                                if (((next.Word == "'s" || next.Word == "'") && next.POS == "POS") )
                                {
                                    sb_sentence.Append(word.Word);
                                }
                                else
                                {
                                    sb_sentence.Append(word.Word + " ");
                                }
                            }
                            else
                            {
                                sb_sentence.Append(word.Word + " ");
                            }
                        }
                        
                    }
                    sb_document.AppendLine(sb_sentence.ToString());
                }
                var ouputfile = Path.Combine(OutputDir, Path.GetFileNameWithoutExtension(item));                
                File.WriteAllText(ouputfile + ".txt", sb_document.ToString());
            }
        }
    }
}
