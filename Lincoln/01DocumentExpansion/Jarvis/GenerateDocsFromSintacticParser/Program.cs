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
    public class Coreference
    {
        public Mention Root { get; set; }
        public IList<Mention> Mentions { get; set; }
        public Coreference()
        {
            Mentions = new List<Mention>();
        }
    }
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


        private void NONER()
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
                        else if (string.Compare(word.Word, "'s", true) == 0 && (word.POS == "VBP" || word.POS == "VBZ"))
                        {
                            sb_sentence.Append("is ");
                        }
                        else if (string.Compare(word.Word, "'re", true) == 0 && word.POS == "VBP")
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
                                if (((next.Word == "'s" || next.Word == "'") && next.POS == "POS"))
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

        static IList<Coreference> GetCoReferences(XElement document,
            IList<Sentence> Sentences)
        {
            List<Coreference> result = new List<Coreference>();

            var coreferences = from c in document.Elements("document").Elements("coreference").Elements("coreference")
                               select c;

            foreach (var item in coreferences)
            {
                var co = new Coreference();
                var mentions = from c in item.Elements("mention")
                               select c;
                Mention root = null;
                foreach (var mention in mentions)
                {
                    var m = new Mention();
                    m.Read(mention, Sentences);
                    m.Head = Sentences.Where(c => c.Id == m.Sentence).First().Tokens.Where(c => c.SentenceLoc == m.HeadLoc).First();
                    if (mention.Attributes("representative").Count() != 0)
                    {
                        co.Root = m;
                        root = m;
                    }
                    else
                    {
                        m.Root = root;
                        co.Mentions.Add(m);
                    }
                }
                result.Add(co);
            }
            var Roots = result.Select(c => c.Root).ToList();
            //obtain non root mention and avoid recursive mentions
            foreach (var coreference in result)
            {
                foreach (var mention in coreference.Mentions)
                {
                    if (Roots.Where(c => c.Contains(mention)).Count() > 0)
                        mention.Enable = false;
                }

            }

            return result;
        }
        private static void NER()
        {
            var Debug = true;  
            var ValidTarget_Pos = new List<String>() { "PRP", "PRP$" };
            var ValidReplace_Pos = new List<String>() { "NN", "NNS", "NNP", "NNPS" };
            string InputDir = @"D:\Tesis2016\Jarvis\Final\Training\01CoreferenceOutput\";
            string OutputDir = @"D:\Tesis2016\Jarvis\Final\Training\02000ExpantionDebug\";
            foreach (var item in Directory.GetFiles(InputDir))
            {
                var sb_document = new StringBuilder();
                var document = XElement.Load(item);
                var sentences = GetSentences(document);
                var references = GetCoReferences(document, sentences);

                foreach (var sentence in sentences)
                {
                    #region coreference 

                    var senreference = (from c in references.SelectMany(x => x.Mentions)
                                        where c.Sentence == sentence.Id &&
                                              c.Enable
                                              && ValidTarget_Pos.Contains(c.Head.POS)
                                              && ValidReplace_Pos.Contains(c.Root.Head.POS)
                                        select c).ToList();


                    var testinterception = new List<int>();
                    var abort = false;
                    foreach (var itemsen in senreference)
                    {
                        var tmp = Enumerable.Range(itemsen.Start, itemsen.End - itemsen.Start);

                        if (testinterception.Intersect(tmp).Count() == 0)
                        {
                            testinterception.AddRange(tmp);
                        }
                        else
                        {
                            abort = true;
                            break;
                        }
                    }
                    if (!abort)
                    {
                        //valid_pos.Contains( c.Head.POS)
                        foreach (var core in senreference)
                        {
                            var start_token = sentence.Tokens.Where(c => c.SentenceLoc == core.Start).First();
                            var end_token = sentence.Tokens.Where(c => c.SentenceLoc == core.End - 1).First();
                            var cursor = sentence.Tokens.Find(start_token).Next;
                            var replace = new List<Token>();
                            for (int i = 0; i < core.GetLen() - 1; i++)
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
                                CharacterOffsetEnd = end_token.CharacterOffsetEnd
                            };
                            if (Debug)
                            {
                                cursor.Value.Word = string.Format("[{0}({1})| {2}({3})]", core.Text, core.Head.POS, core.Root.Text, core.Root.Head.POS);
                            }
                            else
                            {
                                cursor.Value.Word = core.Root.Text;
                            }
                        }
                    }

                    #endregion
                    StringBuilder sb_sentence = new StringBuilder();
                    for (int i = 0; i < sentence.Tokens.Count; i++)
                    {   
                        sb_sentence.Append(WordHelper.ProcessWord(sentence, i));                        
                    }
                    sb_document.AppendLine(sb_sentence.ToString());
                }
                var ouputfile = Path.Combine(OutputDir, Path.GetFileNameWithoutExtension(item));
                File.WriteAllText(ouputfile + ".txt", sb_document.ToString());
            }
        }

        static void Main(string[] args)
        {
            NER();
        }
    }
}
