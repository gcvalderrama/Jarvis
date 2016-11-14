using Neo4j.Driver.V1;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisRDFToNEO4J
{
 
    public class RSTDocumentRepository
    {
        public void DeleteAllNodes()
        {
            using (var driver = GraphDatabase.Driver("bolt://localhost:7687", Neo4j.Driver.V1.AuthTokens.Basic("neo4j", "Pa$$w0rd")))
            {
                using (var session = driver.Session())
                {
                    var tran = session.BeginTransaction();
                    tran.Run("MATCH(n:RSTNode) OPTIONAL MATCH (n) -[r] - () DELETE n, r");
                    tran.Success();
                }
            }
        }
        public void CreateNode(NEO.NEORSTNode Node, IGraphClient client)
        {
            client.Cypher.Merge("(d:RSTNode { name : {name}})").OnCreate()
                .Set("d = {newObject}")
                .WithParams(
                    new
                    {
                        name = Node.name,
                        newObject = Node
                    }
                ).ExecuteWithoutResults();
        }
        public void CreateRelation(NEO.NEORSTRelation Relation, NEO.NEORSTNode Parent, NEO.NEORSTNode Child, IGraphClient client)
        {
            client.Cypher
                .Match("(a:RSTNode)", "(b:RSTNode)")
                .Where((RSTNode a) => a.name == Parent.name)
                .AndWhere((RSTNode b) => b.name == Child.name)
                .CreateUnique(string.Format("(a)-[r:RSTRelation {{ kind: '{0}'}}]->(b)", Relation.relation ))
                .ExecuteWithoutResults();

            
        }
        public void CreateRelationEDUSentence(RSTNode Node, IGraphClient client)
        {
            //client.Cypher
            //    .Match("(a:RSTNode)", "(b:EduSentence)")
            //    .Where((RSTNode a) => a.name == Node.name)
            //    .AndWhere((EduSentence b) => b.Id == Node.EduSentence)
            //    .CreateUnique(string.Format("(a)-[:HAS]->(b)"))
            //    .ExecuteWithoutResults();
        }

        private void InternalSave(NEO.NEORSTNode Node, IGraphClient Client)
        {
            this.CreateNode(Node, Client);
            foreach (var rel in Node.Relations)
            {
                InternalSave(rel.Child, Client);
                this.CreateRelation(rel, Node, rel.Child, Client);
            }
        }
        public void Save(RSTTree tree)
        {
            using (var client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "Pa$$w0rd"))
            {
                client.Connect();

                NEO.NEORSTTree target = new NEO.NEORSTTree(tree);

                InternalSave(target.Root, client);

                //List<RSTNode> historic = new List<RSTNode>();
                //foreach (var item in query)
                //{
                //    var node = new RSTNode();
                //    node.Load(item, tokens);
                //    this.CreateNode(node, client);
                //    historic.Add(node);
                //}
                //foreach (var item in historic.Where(c => c.parent != "rst-1"))
                //{
                //    var parent = historic.Where(c => c.name == item.parent).First();
                //    this.CreateRelation(parent, item, client);
                //    if (item.leaf)
                //    {
                //        this.CreateRelationEDUSentence(item, client);
                //    }
                //}
            }
        }
    }
}
