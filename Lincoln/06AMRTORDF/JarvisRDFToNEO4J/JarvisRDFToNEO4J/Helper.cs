using System;
using System.Collections.Generic;
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

        public static String GetSentence(IGraph g, AMREduSentence sentence)
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
        public static String GetAmrNode(IGraph g, AMREduSentence sentence)
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
        public static String GetAmrNode(IGraph g, AMRNode Parent)
        {
            String result = string.Empty;
            SparqlQueryParser qparser = new SparqlQueryParser();

            //{?r = http://amr.isi.edu/rdf/amr-terms#null_edge , ?p = http://amr.isi.edu/amr_data/1#x1}

            StringBuilder querystr = new StringBuilder();
            querystr.AppendLine("PREFIX amr-terms: <http://amr.isi.edu/rdf/amr-terms#>");
            querystr.AppendLine("SELECT ?r ?p WHERE { <" + Parent.Id + ">  amr-terms:location ?p }");                        
            SparqlQuery q = qparser.ParseFromString(querystr.ToString());
            
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


        
        public static List<AMREduSentence> ReadIds(IGraph g)
        {
            var result = new List<AMREduSentence>();
            SparqlQueryParser qparser = new SparqlQueryParser();
            //Then we can parse a SPARQL string into a query
            SparqlQuery q = qparser.ParseFromString("SELECT ?s ?p WHERE { ?s <http://amr.isi.edu/rdf/core-amr#has-id> ?p }");

            var rset = (SparqlResultSet)g.ExecuteQuery(q);

            var SB = new StringBuilder();
            if (rset.Result && rset.Results.Count > 0)
            {
                foreach (var set in rset.Results)
                {
                    var sentence = new AMREduSentence();
                    foreach (var r in set)
                    {
                        if (r.Key == "s")
                        {
                            sentence.urlid = r.Value.GraphUri;
                        }
                        else
                        {
                            sentence.Id = int.Parse(r.Value.ToString());
                        }
                    }
                    result.Add(sentence);
                }
            }

            return result.OrderBy(c => c.Id).ToList();
        }
    }
}
