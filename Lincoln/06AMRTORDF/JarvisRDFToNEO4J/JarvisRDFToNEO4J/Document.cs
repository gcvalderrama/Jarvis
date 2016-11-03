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
    public class Document
    {
        public Document()
        {
            this.EDUSentences = new List<EduSentence>(); 
        }
        public string Id { get; set; }
        public List<EduSentence> EDUSentences { get; set; }
        
        public void Load(IGraph graph)
        {
            SparqlQueryParser qparser = new SparqlQueryParser();
            StringBuilder querystr = new StringBuilder();
            querystr.AppendLine("PREFIX amr-core: <http://amr.isi.edu/rdf/core-amr#>");
            querystr.AppendLine("PREFIX amr-data: <http://amr.isi.edu/amr_data#>");
            querystr.AppendLine("PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>");
            querystr.AppendLine("PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>");
            querystr.AppendLine("PREFIX amr-terms: <http://amr.isi.edu/rdf/amr-terms#>");            
            querystr.Append("SELECT ?s ?sentence ?id ");
            querystr.Append("WHERE {");
            querystr.Append("?s amr-core:has-sentence ?sentence.");
            querystr.Append("?s amr-core:has-id ?id");            
            querystr.Append("}");

            SparqlQuery q = qparser.ParseFromString(querystr.ToString());            
            var rset = (SparqlResultSet)graph.ExecuteQuery(q);

            var SB = new StringBuilder();
            if (rset.Result && rset.Results.Count > 0)
            {
                foreach (var result in rset.Results)
                {
                    var sentence = new EduSentence();
                    foreach (var r in result)
                    {
                        if (r.Key == "s")
                            sentence.urlid = ((UriNode)r.Value).Uri;
                        if (r.Key == "sentence")
                            sentence.value = r.Value.ToString();
                        if (r.Key == "id")
                            sentence.Id = int.Parse(r.Value.ToString());
                    }
                    this.EDUSentences.Add(sentence);
                    sentence.LoadRoot(graph);
                }
            }
        }
    }
}
