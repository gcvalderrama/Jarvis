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

namespace JarvisRDFToNEO4J
{
    class Program
    {
        public static string Root = @"D:\Tesis2016\Jarvis\Lincoln\06AMRTORDF\Output";
        

        static void Main(string[] args)
        {
            
            //IGraph g = new Graph();
            //IGraph h = new Graph();
            //var parser = new VDS.RDF.Parsing.RdfXmlParser();
            ////   NTriplesParser ntparser = new NTriplesParser();
            ////Load using a Filename
            //parser.Load(g, Path.Combine(Root, "output.xml"));


            //var document = new Document();
            //document.Load(g);
            
            //AMRNEORepository repo = new AMRNEORepository();
            //repo.SaveDocument(document);

                       

            var rstdocument = new RSTDocumentRepository();
            rstdocument.Load(Path.Combine(Root, "rst.xml")); 







            


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
