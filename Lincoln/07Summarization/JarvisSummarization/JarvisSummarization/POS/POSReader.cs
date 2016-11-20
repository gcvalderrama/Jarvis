using JarvisSummarization.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisSummarization.POS
{
    public class POSReader
    {
        public Document Load(string Path)
        {
            Document result = new Document();

            result.name = System.IO.Path.GetFileNameWithoutExtension(Path);

            var str = File.ReadAllText(Path);
            var documentxml = XElement.Parse(str);

            var sentencesxml = from c in documentxml.Elements("document").Elements("sentences").Elements("sentence")
                        select c;

            int tokenpos = 0;

            foreach (var sentencexml in sentencesxml)
            {
                var sentence = new Sentence();
                sentence.ReadFromXML(sentencexml);
                result.sentences.Add(sentence);                 

                var tokensxml = from c in sentencexml.Elements("tokens").Elements("token")
                             select c;

                foreach (var tokenxml in tokensxml)
                {
                    var token = new Token();
                    token.ReadFromXML(tokenxml, tokenpos, sentence.name);
                    sentence.tokens.Add(token);

                    tokenpos++;
                }
            }
            return result; 
        }
    }
}
