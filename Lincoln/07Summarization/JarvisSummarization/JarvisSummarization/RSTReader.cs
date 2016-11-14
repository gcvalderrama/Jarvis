using JarvisSummarization.NEO;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization
{
    public class RSTReader
    {

        public RSTTree ReadRSTTree()
        {
            RSTTree tree = new RSTTree();  
            using (var client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "Pa$$w0rd"))
            {
                client.Connect();

                var result =  client.Cypher.Match("(r:RSTNode)")
                .Where((NEORSTNode r) => r.name == "rst0")
                .Return(r => r.As<NEORSTNode>())
                .Results.First();
                tree.Root = new RSTNode();
                tree.Root.Load(result);

                LoadChilds(tree.Root, client);
            }
            return tree;  
        }

        public void LoadChilds(RSTNode Parent, IGraphClient client) {

            var results = client.Cypher.Match("(a:RSTNode)-[r:rstrelation]->(b:RSTNode)")
                .Where((NEORSTNode a) => a.name == Parent.name)
                .Return( (a, r, b) => new { relation = r.As<NEORSTRelation>(), node = b.As<NEORSTNode>() })
                .Results;

            foreach (var item in results)
            {
                RSTNode node = new RSTNode();
                node.Load(item.node, item.relation);                
                Parent.Children.Add(node);
                LoadChilds(node, client); 
            }            
        }
    }
}
