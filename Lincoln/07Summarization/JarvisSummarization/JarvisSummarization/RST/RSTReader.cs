using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisSummarization.RST
{
    public class RSTReader
    {
        public RSTDocument ReadDocumentContent(string str, string name)
        {
            RSTDocument result = new RSTDocument();
            result.name = name;

            var documentxml = XElement.Parse(str);
            var tokensquery = from c in documentxml.Elements("tokens").Elements("token")
                              select c;

            foreach (var token in tokensquery)
            {
                var tk = new Common.Token();
                tk.name = int.Parse(token.Attribute("id").Value);
                tk.word = token.Attribute("word").Value;
                tk.lemma = token.Attribute("lemma").Value;
                tk.eduid = int.Parse(token.Attribute("eduidx").Value);
                tk.sentence = int.Parse(token.Attribute("sidx").Value) + 1; //here from 0 stanford from 1
                result.Tokens.Add(tk);
            }


            var treebankstr = (from c in documentxml.Elements("rstview") select c).First().Value;
            var input = new java.io.StringReader(treebankstr);
            var treeReader = new edu.stanford.nlp.trees.PennTreeReader(input);

            result.root = new RSTNode();
            result.root.Load(treeReader.readTree(), result.Tokens);

            return result;
        }
        public RSTDocument ReadDocument(string path, string name)
        {            
            var str = File.ReadAllText(path);
            return this.ReadDocumentContent(str, name);
        }
    }
}
