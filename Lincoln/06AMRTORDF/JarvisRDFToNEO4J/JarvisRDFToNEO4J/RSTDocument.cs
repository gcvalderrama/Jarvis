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
    public class RSTNode {

        public string name { get; set; }
        public string parent { get; set; }
        public bool leaf { get; set; }
        public string type { get; set; }
        public string relation { get; set; }
        public string form { get; set; }
        public string edu { get; set; }
        public int EduSentence { get {                
                return int.Parse(this.edu.Split(',')[0].Replace("(", "")); 
         } }
        public void Load(XElement Node)
        {
            this.name = "rst" + Node.Attribute("id").Value;
            this.parent = "rst" + Node.Attribute("parentid").Value;
            this.relation = Node.Attribute("relation").Value;
            this.leaf = bool.Parse(Node.Attribute("leaf").Value);
            this.type = Node.Attribute("type").Value;
            this.edu = Node.Attribute("edu").Value;
        }

    }
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
        public void CreateNode(RSTNode Node, IGraphClient client)
        {
            client.Cypher.Merge("(d:RSTNode { name : {name}})").OnCreate()
                .Set("d = {newObject}")
                .WithParams(
                    new
                    {
                        name = Node.name,
                        newObject = new { Node.name, Node.type, Node.leaf }
                    }
                ).ExecuteWithoutResults();
        }
        public void CreateRelation(RSTNode Parent, RSTNode Child, IGraphClient client)
        {
            client.Cypher
                .Match("(a:RSTNode)", "(b:RSTNode)")
                .Where((RSTNode a) => a.name == Parent.name)
                .AndWhere((RSTNode b) => b.name == Child.name)
                .CreateUnique(string.Format("(a)-[:{0}]->(b)", Child.relation))
                .ExecuteWithoutResults();
        }
        public void CreateRelationEDUSentence(RSTNode Node, IGraphClient client)
        {
            client.Cypher
                .Match("(a:RSTNode)", "(b:EduSentence)")
                .Where((RSTNode a) => a.name == Node.name)
                .AndWhere((EduSentence b) => b.Id == Node.EduSentence)
                .CreateUnique(string.Format("(a)-[:HAS]->(b)"))
                .ExecuteWithoutResults();
        }
        public void Load(string Path)
        {
            this.DeleteAllNodes();
            var reader = XElement.Load(Path);
            var query = from c in reader.Elements("rsttree").Elements("node")
                        select c;

            using (var client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "Pa$$w0rd"))
            {
                client.Connect();
                List<RSTNode> historic = new List<RSTNode>();  
                foreach (var item in query)
                {
                    var node = new RSTNode();
                    node.Load(item);
                    this.CreateNode(node, client);
                    historic.Add(node); 
                }
                foreach (var item in historic.Where(c=>c.parent != "rst-1" ))
                {                    
                    var parent = historic.Where(c => c.name == item.parent).First();
                    this.CreateRelation(parent, item, client);
                    if (item.leaf)
                    {
                        this.CreateRelationEDUSentence(item, client); 
                    }
                }
            }            
        }
    }
}
