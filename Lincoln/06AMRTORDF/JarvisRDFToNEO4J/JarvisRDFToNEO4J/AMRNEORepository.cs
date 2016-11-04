using Neo4j.Driver.V1;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisRDFToNEO4J
{
    public class AMRNEORepository
    {
        public void DeleteAllNodes()
        {            
            using (var driver = GraphDatabase.Driver("bolt://localhost:7687", Neo4j.Driver.V1.AuthTokens.Basic("neo4j", "Pa$$w0rd")))
            {
                using (var session = driver.Session())
                {
                    var tran = session.BeginTransaction();
                    tran.Run("MATCH(n) OPTIONAL MATCH (n) -[r] - () DELETE n, r");
                    tran.Success();
                }
            }
        }
        
        public void SaveDocument(Document document)
        {
            this.DeleteAllNodes();

            using (var client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "Pa$$w0rd"))
            {
                client.Connect();
                var docs =  client.Cypher.Create("(d:Document {newObject})")
                    .WithParam("newObject", new { Id = document.Id })
                    .Return(d => d.As<Document>())
                    .Results.First();
                
                foreach (var edu in document.EDUSentences.OrderBy(c=>c.Id))
                {
                    client.Cypher.Create("(d:EduSentence {newObject})")
                    .WithParam("newObject", new { Id = edu.Id, edu.value }).ExecuteWithoutResults();

                    client.Cypher.Match("(doc:Document)", "(edunode:EduSentence)")
                    .Where((Document doc) => doc.Id == docs.Id)
                    .AndWhere((EduSentence edunode) => edunode.Id == edu.Id)
                    .Create("(doc)-[:HAS]->(edunode)").ExecuteWithoutResults();
                                        
                    SaveNode(client, edu.Root, null,  edu);

                    CreateRootRelation(client, edu, edu.Root);

                }
            }             
        }

        private void CreateNode(IGraphClient client, AmrNode Node)
        {
            client.Cypher.Merge("(d:AMRNode { Id : {id}})").OnCreate()
                .Set("d = {newObject}")
                .WithParams(
                    new
                    {
                        id = Node.Id,
                        newObject = new { Id = Node.Id, Name = Node.Name, Node.PropBank, Node.PropBankName, Node.Description, Node.AmrType }
                    }
                ).ExecuteWithoutResults();
        }
        private void CreateRootRelation(IGraphClient client, EduSentence sentence, AmrNode Root)
        {
            client.Cypher.Match("(edu:EduSentence)", "(node:AMRNode)")
                    .Where((EduSentence edu) => edu.Id == sentence.Id)
                    .AndWhere((AmrNode node) => node.Id == Root.Id)
                    .Create("(edu)-[:Root]->(node)").ExecuteWithoutResults();
        }

        public void SaveNode(IGraphClient client, AmrNode Node, AmrNode Parent, EduSentence Sentence)
        {

            this.CreateNode(client, Node); 


            client.Cypher.Match("(node:AMRNode)", "(edunode:EduSentence)")
            .Where((AmrNode node) => node.Id == Node.Id)
            .AndWhere((EduSentence edunode) => edunode.Id == Sentence.Id)
            .Create("(edunode)-[:HAS]->(node)").ExecuteWithoutResults();

            if (Parent != null)
            {
                if (Node.Direction)
                {
                    client.Cypher.Match("(node:AMRNode)", "(child:AMRNode)")
                    .Where((AmrNode node) => node.Id == Parent.Id)
                    .AndWhere((AmrNode child) => child.Id == Node.Id)
                    .Create(string.Format("(node)-[r:wordrel {{name:'{0}',inverse:{1}, url:'{2}', argname: '{3}' }}]->(child)", Node.RelationName, !Node.Direction, Node.Relation, CalculateRelationArgName(Parent, Node))).ExecuteWithoutResults();
                }
                else
                {
                    client.Cypher.Match("(node:AMRNode)", "(child:AMRNode)")
                    .Where((AmrNode node) => node.Id == Parent.Id)
                    .AndWhere((AmrNode child) => child.Id == Node.Id)
                    .Create(string.Format("(child)-[r:wordrel {{name:'{0}',inverse:{1}, url:'{2}', argname: '{3}' }}]->(node)", Node.RelationName, !Node.Direction, Node.Relation, CalculateRelationArgName(Parent, Node))).ExecuteWithoutResults();
                }                
            }

            foreach (var child in Node.Nodes)
            {
                this.SaveNode(client, child, Node, Sentence);
            }
        }
        public string CalculateRelationArgName(AmrNode Parent, AmrNode Child)
        {

            if (Child.Relation.ToString().StartsWith("http://amr.isi.edu/rdf/amr-terms"))
            {
                return Child.Relation.Fragment.Replace("#", "");
            }
            else if (Child.Direction)
            {
                return Child.Relation.ToString().Replace(Parent.PropBank.ToString(), "").Replace(".", "");
            }
            else
            {
                return Child.Relation.ToString().Replace(Child.PropBank.ToString(), "OF-").Replace(".", "");
            }
        }
    }
    
}
