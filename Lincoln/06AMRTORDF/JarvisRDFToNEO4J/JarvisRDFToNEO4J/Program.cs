using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace JarvisRDFToNEO4J
{
    public static class Helper
    {

        public static String GetSentence(IGraph g, Sentence sentence)
        {
            String result = string.Empty;
            SparqlQueryParser qparser = new SparqlQueryParser();
            var str = "SELECT ?p WHERE { <" + sentence.urlid + ">  <http://amr.isi.edu/rdf/core-amr#has-sentence> ?p }";
            SparqlQuery q = qparser.ParseFromString(str);
            var rset = (SparqlResultSet)g.ExecuteQuery(q);
            var SB = new StringBuilder();
            if (rset.Result && rset.Results.Count > 0)
            {
                foreach (var set in rset.Results)
                {
                    foreach (var r in set)
                    {
                        result = r.Value.ToString();
                    }
                }
            }
            return result;
        }
        public static String GetAmrRoot(IGraph g, Sentence sentence)
        {
            String result = string.Empty;
            SparqlQueryParser qparser = new SparqlQueryParser();
            var str = "SELECT ?p WHERE { <" + sentence.urlid + ">  <http://amr.isi.edu/rdf/core-amr#root> ?p }";
            SparqlQuery q = qparser.ParseFromString(str);
            var rset = (SparqlResultSet)g.ExecuteQuery(q);
            var SB = new StringBuilder();
            if (rset.Result && rset.Results.Count > 0)
            {
                foreach (var set in rset.Results)
                {
                    foreach (var r in set)
                    {
                        result = r.Value.ToString();
                    }
                }
            }
            return result;
        }

        public static AmrTerm GetAmrTerm(IGraph g, AmrNode node)
        {
            AmrTerm result = new AmrTerm();
            SparqlQueryParser qparser = new SparqlQueryParser();
            var str = "SELECT ?p WHERE { <" + node.uriid + ">  <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?p }";
            SparqlQuery q = qparser.ParseFromString(str);
            var rset = (SparqlResultSet)g.ExecuteQuery(q);
            var SB = new StringBuilder();
            if (rset.Result && rset.Results.Count > 0)
            {
                foreach (var set in rset.Results)
                {
                    foreach (var r in set)
                    {
                        result.uriid = r.Value.ToString();                        
                    }
                }
            }
            qparser = new SparqlQueryParser();
            str = "SELECT ?p WHERE { <" + result.uriid + ">  <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?p }";
            q = qparser.ParseFromString(str);
            rset = (SparqlResultSet)g.ExecuteQuery(q);
            
            if (rset.Result && rset.Results.Count > 0)
            {
                foreach (var set in rset.Results)
                {
                    foreach (var r in set)
                    {
                        result.type = r.Value.ToString();
                    }
                }
            }


            //s http://amr.isi.edu/frames/ld/v1.2.2/dedicate-01
            //x http://www.w3.org/1999/02/22-rdf-syntax-ns#type
            //p http://amr.isi.edu/rdf/core-amr#Frame

            return result;
        }

        public static List<Sentence> ReadIds(IGraph g)
        {
            var result = new List<Sentence>();
            SparqlQueryParser qparser = new SparqlQueryParser();
            //Then we can parse a SPARQL string into a query
            SparqlQuery q = qparser.ParseFromString("SELECT ?s ?p WHERE { ?s <http://amr.isi.edu/rdf/core-amr#has-id> ?p }");

            var rset = (SparqlResultSet)g.ExecuteQuery(q);

            var SB = new StringBuilder();
            if (rset.Result && rset.Results.Count > 0)
            {
                foreach (var set in rset.Results)
                {
                    var sentence = new Sentence();
                    foreach (var r in set)
                    {
                        if (r.Key == "s")
                        {
                            sentence.urlid = r.Value.ToString();
                        }
                        else
                        {
                            sentence.id = int.Parse(r.Value.ToString());
                        }
                    }
                    result.Add(sentence);
                }
            }

            return result.OrderBy(c => c.id).ToList();
        }
    }
    public class Sentence {
        public int id { get; set; }
        public string urlid { get; set; }
        public string sentence { get; set; }
        public AmrNode Root { get; set; }
        public void AddRoot(IGraph graph)
        {

            Root = new AmrNode();
            Root.uriid = Helper.GetAmrRoot(graph, this);
            Root.Term = Helper.GetAmrTerm(graph, this.Root); 
             
        }
    }
    public class AmrTerm
    {
        public string uriid { get; set; }
        public string type { get; set; } 
    }
    public class AmrNode {

        public string uriid { get; set; }
        public AmrTerm Term = new AmrTerm(); 


    }


    class Program
    {
        public static string Root = @"D:\Tesis2016\Jarvis\Lincoln\06AMRTORDF\Output";
        

        static void Main(string[] args)
        {
            IGraph g = new Graph();
            IGraph h = new Graph();
            var parser = new VDS.RDF.Parsing.RdfXmlParser();
            //   NTriplesParser ntparser = new NTriplesParser();
            //Load using a Filename
            parser.Load(g, Path.Combine(Root, "output.xml"));

            var ids = Helper.ReadIds(g);
            foreach (var item in ids)
            {

                item.sentence = Helper.GetSentence(g, item);
                item.AddRoot(g);



                Console.WriteLine(item.urlid);
                Console.WriteLine(item.sentence);
                Console.WriteLine(item.Root.uriid);
                Console.WriteLine(item.Root.Term.uriid);
                Console.WriteLine(item.Root.Term.type);
            }

            //SparqlQueryParser qparser = new SparqlQueryParser();
            ////Then we can parse a SPARQL string into a query
            //SparqlQuery q = 
            //    qparser.ParseFromString("SELECT ?s ?x ?p WHERE { ?s ?x ?p }");

            ////http://amr.isi.edu/rdf/core-amr#has-id
            //var rset = (SparqlResultSet)g.ExecuteQuery(q);

            //var SB = new StringBuilder(); 
            //if (rset.Result && rset.Results.Count > 0 )
            //{
            //    foreach (var result in rset.Results)
            //    {
            //        foreach (var r in result)
            //        {
            //            SB.AppendLine(r.Key + " " + r.Value);
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
