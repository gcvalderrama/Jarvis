using JarvisSummarization.AMR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordNet = LAIR.ResourceAPIs.WordNet;
namespace JarvisSummarization.CG
{
    public class CGGraph
    {
        [JsonIgnore]
        public WordNet.WordNetEngine _wordNetEngine = new WordNet.WordNetEngine(@"D:\Tesis2016\WordnetAPI\resources\", false);

        public string name { get; set; }
        public int numnodes { get {
                return this.Nodes.Count;
            } }
        public int numrelations {
            get {
                return this.Relations.Count;  
            }
        }
        [JsonIgnore]
        public List<CGNode> Nodes { get; set; }
        [JsonIgnore]
        public List<CGRelation> Relations { get; set; }
        public CGGraph(string name)
        {
            this.name = name; 
            this.Nodes = new List<CGNode>();
            this.Relations = new List<CGRelation>();
        }
        private void ProcessARGOf()
        {            
                var relations = from c in this.Relations.Where(c => c.label.Contains("ARG") && c.label.Contains("-of"))
                                select c;

                foreach (var item in relations)
                {
                    //we can invert if the tail is a verb
                    var head = this.Nodes.Where(c => c.id == item.Head).First();
                    var tail = this.Nodes.Where(c => c.id == item.Tail).First();
                    item.label = item.label.Replace("-of", "");
                    var tmp = item.Head;
                    item.Head = item.Tail;
                    item.Tail = tmp;
                }
            
        }
        private void PruneRelationErrors()
        {            
            var deletes = new List<CGRelation>();
            foreach (var item in this.Relations.Where(c => c.label.Contains("ARG")))
            {
                var head = this.Nodes.Where(c => c.id == item.Head).First();
                var tail = this.Nodes.Where(c => c.id == item.Tail).First();
                if (item.label.Contains("-of"))
                {
                    if (tail.kind != "verb")
                    {
                        deletes.Add(item);
                    }
                }
                else
                {
                    if (head.kind != "verb")
                    {
                        deletes.Add(item);
                    }
                }
            }
            foreach (var item in deletes)
            {
                this.Relations.Remove(item);
            }            
        }
        private void SplitSemanticRole()
        {            
            var query = from c in this.Relations.Where(c => c.label.Contains("ARG"))
                        group c by c.Tail into g
                        where g.Count() > 1 && g.Select(c => c.f).Distinct().Count() > 1
                        select g;
            foreach (var rel in query)
            {
                var reloriginal = rel.ElementAt(0);
                var nodeoriginal = this.Nodes.Where(c => c.id == reloriginal.Tail).First();
                var outrelations = this.Relations.Where(c => c.Head == nodeoriginal.id).ToList();
                for (int i = 1; i < rel.Count(); i++)
                {
                    var clone = nodeoriginal.Clone(this.Nodes.Max(c => c.id) + 1);
                    var prelation = rel.ElementAt(i);
                    prelation.Tail = clone.id;
                    this.Nodes.Add(clone);
                    foreach (var item in outrelations)
                    {
                        var clonerel = item.Clone();
                        clonerel.Head = clone.id;
                        this.Relations.Add(clonerel);
                    }
                }
            }
            
        }
        
        public void AssignSemanticRole()
        {
            foreach (var relation in this.Relations)
            {
                var node = this.Nodes.Where(c => c.id == relation.Tail).First();
                node.semanticrole = string.IsNullOrWhiteSpace(relation.f) ? relation.label : relation.f;
            }
        }

        private Dictionary<string, WordNet.SynSet> SynSets = new Dictionary<string, WordNet.SynSet>();
        public void LoadWordnet()
        {
            foreach (var item in this.Nodes.Where(c => c.semanticrole == "ppt" || c.semanticrole == "pag").Select(c => c.nosuffix).Distinct())
            {
                WordNet.SynSet synset = _wordNetEngine.GetMostCommonSynSet(item, WordNet.WordNetEngine.POS.Noun);
                
                SynSets.Add(item, synset); 
            }
        }
        public void CollapseAgents()
        {
            var sameagents = from c in this.Nodes where c.semanticrole == "pag" group c by c.nosuffix into g select g;
            GraphFusion(sameagents);
        }
        private void GraphFusion(IEnumerable<IGrouping<string, CGNode>> groups)
        {
            foreach (var g in groups)
            {
                //survivor
                List<CGNode> deletes = new List<CGNode>();
                var max = g.OrderByDescending(c => c.rstweight).First();
                foreach (var node in g)
                {
                    if (node.id == max.id) continue;
                    deletes.Add(node);
                    var inrelations = from c in this.Relations where c.Tail == node.id select c;
                    foreach (var item in inrelations)
                    {
                        item.Tail = max.id;
                    }
                    var outrelations = from c in this.Relations where c.Head == node.id select c;
                    foreach (var item in outrelations)
                    {
                        item.Head = max.id;
                    }
                }
                foreach (var item in deletes)
                {
                    this.Nodes.Remove(item);
                }
            }

        }
        private void CollapsePatientTheme()
        {
            var samethemes = from c in this.Nodes where c.semanticrole == "ppt" group c by c.nosuffix into g select g;
            GraphFusion(samethemes);
        }
        //http://es.slideshare.net/underx/semantic-roles
        private void CollapseGOL()
        {
            var samethemes = from c in this.Nodes where c.semanticrole == "gol" group c by c.nosuffix into g select g;
            GraphFusion(samethemes);
        }
        public void AssignWornet()
        {
            //agents 
        }
        public void Digest()
        {
            this.ProcessARGOf();
            this.PruneRelationErrors();
            this.SplitSemanticRole(); //we can not have a token with two semantic role assign
            this.AssignSemanticRole();
            this.LoadWordnet();
            //this.CollapseAgents();
            //this.CollapsePatientTheme();


        }
        public void ReadAMR(AMRDocument Document)
        {
            foreach (var gr in Document.Graphs)
            {
                foreach (var node in gr.Nodes)
                {

                    this.Nodes.Add(new CGNode(node, gr.name));
                }
                //transform relations 
                foreach (var relation in gr.Relations)
                {
                    var head = gr.Nodes.Where(c => c.name == relation.Head).First();
                    var tail = gr.Nodes.Where(c => c.name == relation.Tail).First();
                    relation.Head = head.id;
                    relation.Tail = tail.id;

                    var rel = new CGRelation(relation);
                    this.Relations.Add(rel);
                }
            }
        }

    }
}
