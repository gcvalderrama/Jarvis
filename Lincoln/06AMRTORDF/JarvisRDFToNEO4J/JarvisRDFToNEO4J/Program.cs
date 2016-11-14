using edu.stanford.nlp.ling;
using edu.stanford.nlp.process;
using edu.stanford.nlp.trees;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using java.util.function;
using java.util;

namespace JarvisRDFToNEO4J
{

    public class CustomStringLabelFactory : StringLabelFactory
    {
        public override Label newLabelFromString(string labelStr)
        {
            return base.newLabelFromString(labelStr);
        }
        public override Label newLabel(string labelStr)
        {
            return base.newLabel(labelStr);
        }
        public override Label newLabel(Label oldLabel)
        {
            return base.newLabel(oldLabel);
        }
    }
    public class CustomTreeNormalizer : TreeNormalizer
    {
        public override string normalizeNonterminal(string category)
        {
            return base.normalizeNonterminal(category);
        }
        public override string normalizeTerminal(string leaf)
        {
            return base.normalizeTerminal(leaf);
        }
        public override Tree normalizeWholeTree(Tree tree, TreeFactory tf)
        {
            return base.normalizeWholeTree(tree, tf);
        }
    }
    public class CustomTokenizerAdapter : PennTreebankTokenizer
    {
        public CustomTokenizerAdapter(java.io.Reader reader) : base(reader) {
        }        
                
        private List<string> Continues = new List<string>() { @"/", "-" };
        private List<string> Reserved = new List<string>() { "(", ")" };


        private bool peek = false;

        public override string getNext()
        {            
            var str = base.getNext();
            if (!Reserved.Contains(str))
            {
                this.peek = true;
                string tmp = this.getNext();
                while (!Reserved.Contains(tmp))
                {
                    
                    str += tmp;
                    this.peek = true;
                    tmp = this.getNext();
                }
            }
            if (this.peek)
            {
                this.st.pushBack();
            }
            return str;
        }
        public override List tokenize()
        {
            return base.tokenize();
        }
        public override bool isEol(string str)
        {
            return base.isEol(str);
        }
        public override void setEolString(string eolString)
        {
            base.setEolString(eolString);
        }
        public override void forEachRemaining(Consumer value)
        {
            base.forEachRemaining(value);
        }


    }
    class Program
    {
        public static string Root = @"D:\Tesis2016\Jarvis\Lincoln\06AMRTORDF\Output";
        

        static void Main(string[] args)
        {



            

            var tf = new edu.stanford.nlp.trees.LabeledScoredTreeFactory( new CustomStringLabelFactory());
            
            var str = "(x2 / score :null_edge(x1 / null_tag) :null_edge(x3 / null_tag)	:time(xap0 / before	:quant(x5 / temporal - quantity	:unit(y / year) :null_edge(x4 / null_tag))))";
            var input = new java.io.StringReader(str);

            var treeReader = new edu.stanford.nlp.trees.PennTreeReader(input, tf , new CustomTreeNormalizer(), new CustomTokenizerAdapter(input));

            var t = treeReader.readTree();

            
            TreePrint p = new TreePrint("penn" );
            p.printTree(t); 
            


            //READ RST INFORMATION
            RSTTree tree = new RSTTree("lincon");
            tree.Load(Path.Combine(Root, "rst.xml"));
            tree.EvaluateODonell();
            
            var sum = tree.Summarize();            

            //READ AMR INFORMATION FOR EACH EDU AND ASSOCIATTE THE ODONELL SCORE
            IGraph g = new Graph();
            var parser = new VDS.RDF.Parsing.RdfXmlParser();
            //   NTriplesParser ntparser = new NTriplesParser();            
            parser.Load(g, Path.Combine(Root, "output.xml"));
            var document = new AMRDocument();
            document.Load(g);
            
            foreach (var item in document.EDUSentences)
            {
                item.ApplyRSTWeight(sum.Where(c => c.edu == item.Id).Select(c => c.Weight).First());
            }
            
            //var rstdocument = new RSTDocumentRepository();
            //rstdocument.DeleteAllNodes(); 
            //rstdocument.Save(tree);

            AMRNEORepository repo = new AMRNEORepository();
            repo.DeleteAllNodes();
            repo.SaveDocument(document);











            //var ids = Helper.ReadIds(g);
            //foreach (var item in ids)
            //{

            //    item.sentence = Helper.GetSentence(g, item);
            //    item.AddNodes(g);


            //    if (item.id == 22)
            //    {
            //        Console.WriteLine(item.urlid);
            //        Console.WriteLine(item.sentence);
            //        Console.WriteLine(item.Root.uriid);
            //        Console.WriteLine(item.Root.Term.uriid);
            //        Console.WriteLine(item.Root.Term.type);
            //    }

            //}

            //SparqlQueryParser qparser = new SparqlQueryParser();
            ////Then we can parse a SPARQL string into a query

            //StringBuilder querystr = new StringBuilder();            
            //querystr.AppendLine("PREFIX amr-core: <http://amr.isi.edu/rdf/core-amr#>");
            //querystr.AppendLine("PREFIX amr-data: <http://amr.isi.edu/amr_data#>");
            //querystr.AppendLine("PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>");
            //querystr.AppendLine("PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>");            
            //querystr.AppendLine("PREFIX amr-terms: <http://amr.isi.edu/rdf/amr-terms#>");
            ////querystr.AppendLine("SELECT  ?p WHERE { ?s rdf:type ?p }");
            ////querystr.Append("SELECT ?s ?sentence ?id ?root ?rtype ?amrtype");
            //querystr.Append("SELECT ?root ?rtype  ?amrtypelbl ");
            //querystr.Append("WHERE {");
            //querystr.Append("?s amr-core:has-sentence ?sentence.");
            //querystr.Append("?s amr-core:has-id ?id.");
            //querystr.Append("?s amr-core:root ?root. ");
            //querystr.Append("?root rdf:type ?rtype. ");
            //querystr.Append("?rtype rdf:type ?amrtype. ");
            //querystr.Append("?amrtype rdfs:label ?amrtypelbl. ");
            //querystr.Append("}");

            //SparqlQuery q = qparser.ParseFromString(querystr.ToString());

            ////http://amr.isi.edu/rdf/core-amr#has-id
            //var rset = (SparqlResultSet)g.ExecuteQuery(q);

            //var SB = new StringBuilder();
            //if (rset.Result && rset.Results.Count > 0)
            //{
            //    foreach (var result in rset.Results)
            //    {
            //        foreach (var r in result)
            //        {                        
            //            Console.WriteLine(r.Key + " " + r.Value);
            //        }

            //        //Do what you want with each result
            //    }
            //}
            //File.WriteAllText("dic.txt", SB.ToString());
            //http://amr.isi.edu/amr_data/22#root01

            //foreach (var item in g.Triples)
            //{


            //    Console.WriteLine(item.Subject);

            //}



            //foreach (var node in g.Nodes)
            //{
            //    Console.WriteLine(node.ToString());
            //}

            //g.SaveToFile("output.rdf");


        }
    }
}
