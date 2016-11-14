using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace JarvisRDFToNEO4J
{
    public class AMREduSentence
    {
        public int Id { get; set; }
        public Uri urlid { get; set; }
        public string value { get; set; }
        public AMRNode Root { get; set; }
        
        public void LoadData(IGraph graph, AMRNode Target) {
            SparqlQueryParser qparser = new SparqlQueryParser();
            StringBuilder querystr = new StringBuilder();
            querystr.AppendLine("PREFIX amr-core: <http://amr.isi.edu/rdf/core-amr#>");
            querystr.AppendLine("PREFIX amr-data: <http://amr.isi.edu/amr_data#>");
            querystr.AppendLine("PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>");
            querystr.AppendLine("PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>");
            querystr.AppendLine("PREFIX amr-terms: <http://amr.isi.edu/rdf/amr-terms#>");
            //querystr.AppendLine("SELECT  ?p WHERE { ?s rdf:type ?p }");
            //querystr.Append("SELECT ?s ?sentence ?id ?root ?rtype ?amrtype");
            querystr.Append("SELECT ?rtype ?amrtype ?amrtypelbl ");
            querystr.Append("WHERE {");            
            querystr.Append("@root rdf:type ?rtype. ");
            querystr.Append("?rtype rdf:type ?amrtype. ");
            querystr.Append("?amrtype rdfs:label ?amrtypelbl ");
            querystr.Append("}");

            SparqlParameterizedString command = new SparqlParameterizedString();
            command.CommandText = querystr.ToString();
            command.SetUri("root", Target.Id);

            SparqlQuery q = qparser.ParseFromString(command);

            var rset = (SparqlResultSet)graph.ExecuteQuery(q);
            var SB = new StringBuilder();
            if (rset.Result && rset.Results.Count > 0)
            {
                foreach (var result in rset.Results)
                {
                    foreach (var r in result)
                    {
                        if (r.Key == "rtype")
                            Target.PropBank = ((UriNode)r.Value).Uri;
                        if (r.Key == "amrtypelbl")
                            Target.AmrType = r.Value.ToString();
                        if ( r.Key == "amrtype")
                            Target.AmrTypeUri = ((UriNode)r.Value).Uri;
                    }                                        
                }
            }

        }
        
        private List<AMRNode> FindInvertNodes(IGraph graph, AMRNode Parent, List<AMRNode> Historic)
        {
            List<AMRNode> response = new List<AMRNode>(); 
            SparqlQueryParser qparser = new SparqlQueryParser();
            StringBuilder querystr = new StringBuilder();
            querystr.AppendLine("PREFIX amr-core: <http://amr.isi.edu/rdf/core-amr#>");
            querystr.AppendLine("PREFIX amr-data: <http://amr.isi.edu/amr_data#>");
            querystr.AppendLine("PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>");
            querystr.AppendLine("PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>");
            querystr.AppendLine("PREFIX amr-terms: <http://amr.isi.edu/rdf/amr-terms#>");
            querystr.Append("SELECT ?child ?amrrelation ");
            querystr.Append("WHERE {");
            //querystr.Append("?amrrelation rdf:type amr-core:Role. ");
            querystr.Append("?child ?amrrelation @parenturi ");
            querystr.Append("}");

            SparqlParameterizedString command = new SparqlParameterizedString();
            command.CommandText = querystr.ToString();
            command.SetUri("parenturi", Parent.Id);

            SparqlQuery q = qparser.ParseFromString(command);

            var rset = (SparqlResultSet)graph.ExecuteQuery(q);
            var SB = new StringBuilder();
            if (rset.Result && rset.Results.Count > 0)
            {
                foreach (var result in rset.Results)
                {
                    var child = new AMRNode();
                    foreach (var r in result)
                    {
                        if (r.Key == "child")
                        {
                            if (r.Value is UriNode)
                            {
                                child.Id = ((UriNode)r.Value).Uri;
                            }
                            else if (r.Value is LiteralNode)
                            {
                                child.Id = new Uri(string.Format("http://literal/#{0}", value.ToString()));
                            }
                            else
                            {
                                throw new ApplicationException("Problem");
                            }
                        }
                        if (r.Key == "amrrelation")
                        {
                            child.Relation = ((UriNode)r.Value).Uri;
                            child.Direction = false;                           
                        }
                    }
                    //only data object and avoid recursive
                    if (child.Id.ToString().Contains("http://amr.isi.edu/amr_data") &&
                        !child.Id.ToString().EndsWith("root01") &&
                            Historic.Where(c=>c.Id.ToString() == child.Id.ToString()).Count() ==0                        
                        )
                    {
                        this.LoadData(graph, child);
                        response.Add(child); 
                    }
                }
            }
            return response;
        }

        private void LoadChildren(IGraph graph, AMRNode Parent, List<AMRNode> Historic)
        {
            //if (Parent.Id.ToString() == "http://amr.isi.edu/amr_data/3#x5")
            //    Debugger.Break();

            SparqlQueryParser qparser = new SparqlQueryParser();
            StringBuilder querystr = new StringBuilder();
            querystr.AppendLine("PREFIX amr-core: <http://amr.isi.edu/rdf/core-amr#>");
            querystr.AppendLine("PREFIX amr-data: <http://amr.isi.edu/amr_data#>");
            querystr.AppendLine("PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>");
            querystr.AppendLine("PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>");
            querystr.AppendLine("PREFIX amr-terms: <http://amr.isi.edu/rdf/amr-terms#>");            
            querystr.Append("SELECT ?child ?amrrelation ");
            querystr.Append("WHERE {");            
            querystr.Append("@parenturi ?amrrelation ?child ");
            querystr.Append("}");

            SparqlParameterizedString command = new SparqlParameterizedString();
            command.CommandText = querystr.ToString();
            command.SetUri("parenturi", Parent.Id);

            SparqlQuery q = qparser.ParseFromString(command);

            var rset = (SparqlResultSet)graph.ExecuteQuery(q);
            var SB = new StringBuilder();
            if (rset.Result && rset.Results.Count > 0)
            {
                foreach (var result in rset.Results)
                {
                    var child = new AMRNode();
                    foreach (var r in result)
                    {
                        if (r.Key == "child")
                        {
                            if (r.Value is UriNode)
                            {
                                child.Id = ((UriNode)r.Value).Uri;
                            }
                            else if (r.Value is LiteralNode)
                            {
                                child.Id = new Uri(string.Format("http://literal/#{0}", value.ToString()));
                            }
                            else
                            {
                                throw new ApplicationException("Problem");
                            }                            
                        }
                        if (r.Key == "amrrelation")
                        {
                            child.Relation = ((UriNode)r.Value).Uri;
                        }
                    }
                    //only data object and avoid recursive
                    if (child.Id.ToString().Contains("http://amr.isi.edu/amr_data") &&
                        child.Id.ToString() != Parent.Id.ToString() &&
                         Historic.Where(c => c.Id.ToString() == child.Id.ToString()).Count() == 0
                        )
                    {
                        this.LoadData(graph, child);
                        Parent.Nodes.Add(child);
                        Historic.Add(child); 
                    }                    
                }                
            }
            //find invert

            var nodes = this.FindInvertNodes(graph, Parent, Historic);

            foreach (var node in nodes)
            {                
                Parent.Nodes.Add(node);
                Historic.Add(node); 
            }

            foreach (var item in Parent.Nodes)
            {
                this.LoadChildren(graph, item, Historic);
            }
        }

        public void LoadRoot(IGraph graph)
        {
            SparqlQueryParser qparser = new SparqlQueryParser();
            StringBuilder querystr = new StringBuilder();
            querystr.AppendLine("PREFIX amr-core: <http://amr.isi.edu/rdf/core-amr#>");
            querystr.AppendLine("PREFIX amr-data: <http://amr.isi.edu/amr_data#>");
            querystr.AppendLine("PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>");
            querystr.AppendLine("PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>");
            querystr.AppendLine("PREFIX amr-terms: <http://amr.isi.edu/rdf/amr-terms#>");            
            querystr.Append("SELECT ?root ");
            querystr.Append("WHERE {");            
            querystr.Append("@sentenceuri amr-core:root ?root ");            
            querystr.Append("}");
            
            SparqlParameterizedString command = new SparqlParameterizedString();
            command.CommandText = querystr.ToString();
            command.SetUri("sentenceuri", this.urlid);
            
            SparqlQuery q = qparser.ParseFromString(command);
            
            var rset = (SparqlResultSet)graph.ExecuteQuery(q);
            var SB = new StringBuilder();
            if (rset.Result && rset.Results.Count > 0)
            {
                foreach (var result in rset.Results)
                {
                    this.Root = new AMRNode();
                    this.Root.Id = ((UriNode)result.ElementAt(0).Value).Uri;                    
                    LoadData(graph, this.Root);
                    LoadChildren(graph, this.Root, new List<AMRNode>() { this.Root });                                        
                }
            }
        }

        private List<AMRNode> ToList()
        {
            List<AMRNode> result = new List<AMRNode>();
            ToList(this.Root, result);
            return result;
        }
        private void ToList(AMRNode node, List<AMRNode> state)
        {
            if (state.Contains(node))
                return;
            state.Add(node);
            foreach (var item in node.Nodes)
            {
                ToList(item, state);
            }
        }
        public void ApplyRSTWeight(double score)
        {
            var list = this.ToList();
            foreach (var item in list)
            {
                item.RSTWeight = score;
            }
        }        
    }    
 
}
