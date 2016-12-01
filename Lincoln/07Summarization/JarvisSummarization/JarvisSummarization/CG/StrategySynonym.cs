using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordNet = LAIR.ResourceAPIs.WordNet;
namespace JarvisSummarization.CG
{
    public class StrategySynonym
    {
        private CGGraph graph;

        private WordNet.WordNetEngine _wordNetEngine;

        private WordNet.WordNetSimilarityModel _semanticSimilarityModel;


        public StrategySynonym(CGGraph graph)
        {
            this.graph = graph;
            _wordNetEngine = new WordNet.WordNetEngine(@"D:\Tesis2016\WordnetAPI\resources\", false);
            _semanticSimilarityModel = new LAIR.ResourceAPIs.WordNet.WordNetSimilarityModel(_wordNetEngine);
        }
        private Dictionary<string, WordNet.SynSet> SynSets = new Dictionary<string, WordNet.SynSet>();
        private void GraphFusion(CGGraph graph, IEnumerable<IGrouping<string, CGNode>> groups)
        {
            foreach (var g in groups)
            {
                //survivor
                List<CGNode> deletes = new List<CGNode>();
                List<CGRelation> relations_deletes = new List<CGRelation>();
                List<CGRelation> relations_news = new List<CGRelation>();

                var max = g.OrderByDescending(c => c.rstweight).First();
                foreach (var node in g)
                {
                    if (node.id == max.id) continue;
                    max.AddFusionNode(node); 
                    deletes.Add(node);
                    
                }
                foreach (var node in g)
                {
                    if (node.id == max.id) continue;
                    var inrelations = from c in graph.Relations where c.Tail == node.id select c;
                    foreach (var item in inrelations)
                    {
                        var n = item.Clone();
                        n.Tail = max.id;
                        relations_deletes.Add(item);

                        if (deletes.Where(c => c.id == n.Head).Count() == 0)
                        {
                            relations_news.Add(n);
                        }
                    }
                    var outrelations = from c in graph.Relations where c.Head == node.id select c;
                    foreach (var item in outrelations)
                    {
                        var n = item.Clone();
                        n.Head = max.id;
                        relations_deletes.Add(item);
                        if (deletes.Where(c => c.id == n.Tail).Count() == 0)
                        {
                            relations_news.Add(n);
                        }
                    }
                }
                foreach (var item in relations_deletes)
                {
                    graph.RemoveRelation(item);
                }
                foreach (var item in relations_news)
                {
                    graph.AddRelation(item); 
                }
                
                foreach (var item in deletes)
                {
                    graph.RemoveNode(item);
                }
            }
        }
     
        public void FusionConcepts()
        {
            var nodes = this.graph.Nodes.Where(c => c.semanticroles.Count() == 1
                && (c.semanticroles.Contains("concept"))
                && !c.semanticroles.Contains("rel")).ToList();

            WordNet.WordNetSimilarityModel.Strategy strategy = WordNet.WordNetSimilarityModel.Strategy.WuPalmer1994MostCommon;

            foreach (var current in nodes.OrderByDescending(c => c.rstweight))
            {
                var currentset = this.SynSets[current.nosuffix];

                if (currentset == null)
                {
                    current.hypernym = current.nosuffix;
                    continue;
                }

                //este ciclo incluira el nodo mismo
                foreach (var node in nodes.Where(c => string.IsNullOrWhiteSpace(c.hypernym)))
                {
                    var nodeset = this.SynSets[node.nosuffix];
                    if (nodeset == null) continue;

                    float sim = _semanticSimilarityModel.GetSimilarity(currentset, nodeset, strategy, WordNet.WordNetEngine.SynSetRelation.Hypernym);
                    if (sim > 0.9)
                    {
                        node.hypernym = current.nosuffix;
                    }
                }
            }
            this.GraphFusion(graph, nodes.GroupBy(c => c.hypernym));

        }
        public void Execute()
        {
            var nodes = this.graph.Nodes.Where(c => c.semanticroles.Count() == 1
                && (c.semanticroles.Contains("concept"))
                && !c.semanticroles.Contains("rel")).ToList();

            foreach (var item in nodes.Select(c => c.nosuffix).Distinct())
            {
                WordNet.SynSet synset = _wordNetEngine.GetMostCommonSynSet(item, WordNet.WordNetEngine.POS.Noun);
                SynSets.Add(item, synset);
            }
            this.FusionConcepts();
            
        }
    }
}
