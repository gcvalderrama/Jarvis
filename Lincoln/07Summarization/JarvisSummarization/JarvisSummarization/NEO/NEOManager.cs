using JarvisSummarization.CG;
using JarvisSummarization.RST;
using Neo4j.Driver.V1;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.NEO
{
    public class NEOManager
    {
        private IDriver CreateDriver()
        {
            return GraphDatabase.Driver("bolt://localhost:7687", Neo4j.Driver.V1.AuthTokens.Basic("neo4j", "Pa$$w0rd"));
        }
        public IGraphClient CreateClient()
        {
            return new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "Pa$$w0rd");
        }
        public void DeleteAllNodes()
        {
            using (var driver = CreateDriver())
            {
                using (var session = driver.Session())
                {
                    var tran = session.BeginTransaction();
                    tran.Run("MATCH(n) OPTIONAL MATCH (n) -[r] - () DELETE n, r");
                    tran.Success();
                }
            }
        }

        #region POS


        public void SaveDocument(Common.Document Document)
        {
            using (var client = this.CreateClient())
            {
                client.Connect();
                client.Cypher.Merge("(d:Document { name : {name}})").OnCreate()
                .Set("d = {newObject}")
                .WithParams(
                    new
                    {
                        name = Document.name,
                        newObject = Document
                    }
                ).ExecuteWithoutResults();

                foreach (var sentence in Document.sentences)
                {
                    client.Cypher.Merge("(s:Sentence { name : {name}})").OnCreate()
                     .Set("s = {newObject}")
                     .WithParams(
                         new
                         {
                             name = sentence.name,
                             newObject = sentence
                         }
                     ).ExecuteWithoutResults();


                    client.Cypher.Match("(d:Document)", "(s:Sentence)")
                    .Where( (Common.Document d) => d.name == Document.name )
                    .AndWhere((Common.Sentence s) => s.name == sentence.name)
                    .Create("(d)-[:has]->(s)").ExecuteWithoutResults();

                    foreach (var token in sentence.tokens)
                    {                        

                        client.Cypher.Merge("(t:Token { name : {name}})").OnCreate()
                         .Set("t = {newObject}")
                         .WithParams(
                             new
                             {
                                 name = token.name,
                                 newObject = token
                             }
                         ).ExecuteWithoutResults();


                        client.Cypher.Match("(s:Sentence)", "(t:Token)")
                        .Where((Common.Sentence s) => s.name == sentence.name)
                        .AndWhere((Common.Token t) => t.name == token.name)
                        .Create("(s)-[:has]->(t)").ExecuteWithoutResults();
                    }
                }                
            }
        }




        #endregion

        #region RST
        public void DeleteAllRST()
        {
            using (var driver = this.CreateDriver())
            {
                using (var session = driver.Session())
                {
                    var tran = session.BeginTransaction();
                    tran.Run("MATCH(n:RSTNode) OPTIONAL MATCH (n) -[r] - () DELETE n, r");
                    tran.Success();                                        
                }
                using (var session = driver.Session())
                {
                    var tran = session.BeginTransaction();
                    tran.Run("MATCH(n:RSTEdu) OPTIONAL MATCH (n) -[r] - () DELETE n, r");
                    tran.Success();
                }                
            }
        }
        public void SaveDocumentRST(RST.RSTDocument Document)
        {
            using (var client = this.CreateClient())
            {
                client.Connect();
                
                client.Cypher.Match("(d:Document)", "(r:RSTNode)")
                .Where((Common.Document d) => d.name ==  Document.name)
                .AndWhere((RSTNode r) => r.name == Document.root.name)
                .CreateUnique(string.Format("(d)-[:has]->(r)"))
                .ExecuteWithoutResults();

                client.Cypher.Create("(d:RSTNode {newObject})")
                   .WithParam("newObject", Document.root)
                   .ExecuteWithoutResults();

                this.SaveNodeRST(client , Document.root);
            }
        }
        private void SaveNodeRST(IGraphClient client, RST.RSTNode Parent)
        {
            if (Parent.edu != null)
            {

                client.Cypher.Create("(d:RSTEdu {newObject})")
                   .WithParam("newObject", Parent.edu)
                   .ExecuteWithoutResults();

                client.Cypher.Match("(n:RSTNode)", "(e:RSTEdu)")
                .Where((RSTNode n) => n.name == Parent.name)
                .AndWhere((RSTEdu e) => e.name == Parent.edu.name)
                .CreateUnique(string.Format("(n)-[:has]->(e)"))
                .ExecuteWithoutResults();


                client.Cypher.Match("(s:Sentence)", "(e:RSTEdu)")
                .Where((Common.Sentence s) => s.name == Parent.edu.sentence)
                .AndWhere((RSTEdu e) => e.name == Parent.edu.name)
                .CreateUnique(string.Format("(e)-[:sentence]->(s)"))
                .ExecuteWithoutResults();
                
                foreach (var token in Parent.edu.tokens)
                {
                    client.Cypher
                    .Match("(s:Sentence)-[:has]->(t:Token)")
                    .Where((Common.Sentence s) => s.name == Parent.edu.sentence)
                    .AndWhere((Common.Token t) => t.name == token.name && t.lemma == token.lemma)
                    .Set("t.rstweight = {rstweight}")
                    .Set("t.eduid = {eduid}")
                    .WithParam("rstweight", token.rstweight)
                    .WithParam("eduid", token.eduid)
                    .ExecuteWithoutResults();
                }
                return;
            }

            foreach (var child in Parent.children)
            {
                client.Cypher.Create("(d:RSTNode {newObject})")
               .WithParam("newObject", child)
               .ExecuteWithoutResults();

                client.Cypher.Match("(r:RSTNode)", "(r1:RSTNode)")
                       .Where((RSTNode r) => r.name == Parent.name)
                       .AndWhere((RSTNode r1) => r1.name == child.name)
                       .CreateUnique(string.Format("(r)-[rr:RSTRelation {{ kind: '{0}'}}]->(r1)", child.nucleus ? "nucleus" : "satellite"))
                       .ExecuteWithoutResults();

                SaveNodeRST(client, child); 
            }

            

        }
        #endregion
        #region AMR
        public void DeleteAllAMR()
        {
            using (var driver = this.CreateDriver())
            {
                using (var session = driver.Session())
                {
                    var tran = session.BeginTransaction();
                    tran.Run("MATCH(n:AMRNode) OPTIONAL MATCH (n) -[r] - () DELETE n, r");
                    tran.Success();
                }
                using (var session = driver.Session())
                {
                    var tran = session.BeginTransaction();
                    tran.Run("MATCH(n:AMRGraph) OPTIONAL MATCH (n) -[r] - () DELETE n, r");
                    tran.Success();
                }
            }
        }
        public void SaveAMR(AMR.AMRDocument Document)
        {
            using (var client = this.CreateClient())
            {
                client.Connect();
                
                foreach (var graph in Document.Graphs)
                {
                    client.Cypher.Create("(a:AMRGraph {newObject})")
                   .WithParam("newObject", graph)
                   .ExecuteWithoutResults();

                    
                    client.Cypher.Match("(a:AMRGraph)", "(s:RSTEdu)")
                        .Where((AMR.AMRGraph a) => a.name == graph.name)
                        .AndWhere((RSTEdu s) => s.name == graph.name)
                        .CreateUnique(string.Format("(s)-[:hasedu]->(a)"))
                        .ExecuteWithoutResults();

                    client.Cypher.Match("(a:AMRGraph)", "(s:Sentence)")
                        .Where((AMR.AMRGraph a) => a.name == graph.name)
                        .AndWhere((Common.Sentence s) => s.name == graph.name)
                        .CreateUnique(string.Format("(s)-[:amr]->(a)"))
                        .ExecuteWithoutResults();

                    foreach (var node in graph.Nodes)
                    {
                        client.Cypher.Create("(n:AMRNode {newObject})")
                        .WithParam("newObject", node)
                        .ExecuteWithoutResults();

                        client.Cypher.Match("(a:AMRGraph)", "(n:AMRNode)")
                        .Where((AMR.AMRGraph a) => a.name == graph.name)
                        .AndWhere((AMR.AMRNode n) => n.id == node.id)
                        .CreateUnique(string.Format("(a)-[:has]->(n)"))
                        .ExecuteWithoutResults();
                    }
                    foreach (var relation in graph.Relations)
                    {
                        var headnode = graph.Nodes.Where(c => c.name == relation.Head).First();
                        var tailnode = graph.Nodes.Where(c => c.name == relation.Tail).First();
                        client.Cypher.Match("(a:AMRNode)", "(n:AMRNode)")
                        .Where((AMR.AMRNode a) => a.id == headnode.id)
                        .AndWhere((AMR.AMRNode n) => n.id == tailnode.id)
                        .CreateUnique(string.Format("(a)-[r:amrrelation {{ kind: '{0}', description: '{1}', f:'{2}', vntheta:'{3}' }} ]->(n)", relation.label, relation.description, relation.f, relation.vntheta ))
                        .ExecuteWithoutResults();
                    }
                }                
            }
        }
        #endregion

        #region conceptual graph
        public void DeleteAllCG()
        {
            using (var driver = this.CreateDriver())
            {
                using (var session = driver.Session())
                {
                    var tran = session.BeginTransaction();
                    tran.Run("MATCH(n:CGNode) OPTIONAL MATCH (n) -[r] - () DELETE n, r");
                    tran.Success();
                }
                using (var session = driver.Session())
                {
                    var tran = session.BeginTransaction();
                    tran.Run("MATCH(n:CGGraph) OPTIONAL MATCH (n) -[r] - () DELETE n, r");
                    tran.Success();
                }
            }
        }
        public void SaveCG(CGGraph Graph)
        {
            using (var client = this.CreateClient())
            {
                client.Connect();

                client.Cypher.Create("(a:CGGraph {newObject})")
                  .WithParam("newObject", Graph)
                  .ExecuteWithoutResults();

                client.Cypher.Match("(a:CGGraph)", "(d:Document)")
                     .Where((CGGraph a) => a.name == Graph.name)
                     .AndWhere((Common.Document d) => d.name == Graph.name)
                     .CreateUnique(string.Format("(d)-[:cg]->(a)"))
                     .ExecuteWithoutResults();

                foreach (var node in Graph.Nodes)
                {
                    client.Cypher.Create("(n:CGNode {newObject})")
                        .WithParam("newObject", node)
                        .ExecuteWithoutResults();

                    client.Cypher.Match("(a:CGGraph)", "(n:CGNode)")
                       .Where((CGGraph a) => a.name == Graph.name)
                       .AndWhere((CGNode n) => n.id == node.id)
                       .CreateUnique(string.Format("(a)-[:has]->(n)"))
                       .ExecuteWithoutResults();
                }
                foreach (var relation in Graph.Relations)
                {                    
                    client.Cypher.Match("(a:CGNode)", "(n:CGNode)")
                    .Where((CGNode a) => a.id == relation.Head)
                    .AndWhere((CGNode n) => n.id == relation.Tail)
                    .CreateUnique(string.Format("(a)-[r:gcrelation {{ kind: '{0}', description: '{1}', f:'{2}', vntheta:'{3}' }} ]->(n)", relation.label, relation.description, relation.f, relation.vntheta))
                    .ExecuteWithoutResults();
                }
            }
        }
        #endregion
    }
}
