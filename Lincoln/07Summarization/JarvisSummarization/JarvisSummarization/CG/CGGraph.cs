using JarvisSummarization.AMR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisSummarization.CG
{
    public class CGGraph
    {
        
        private string propbankPath;
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

        [JsonIgnore]
        public List<CGInformativeAspect> InformativeAspects { get; protected set; }
        

        public CGGraph(string name, string propbankPath)
        {
            this.propbankPath = propbankPath;
            this.name = name; 
            this.Nodes = new List<CGNode>();
            this.Relations = new List<CGRelation>();
            this.InformativeAspects = new List<CGInformativeAspect>(); 
        }
        
        private void PruneRelationErrors()
        {            
            //var deletes = new List<CGRelation>();
            //foreach (var item in this.Relations.Where(c => c.label.Contains("ARG")))
            //{
            //    var head = this.Nodes.Where(c => c.id == item.Head).First();
            //    var tail = this.Nodes.Where(c => c.id == item.Tail).First();
            //    if (item.label.Contains("-of"))
            //    {
            //        if (tail.kind != "verb")
            //        {
            //            deletes.Add(item);
            //        }
            //    }
            //    else
            //    {
            //        if (head.kind != "verb")
            //        {
            //            deletes.Add(item);
            //        }
            //    }
            //}
            //foreach (var item in deletes)
            //{
            //    this.Relations.Remove(item);
            //}            
        }   
        
        //http://es.slideshare.net/underx/semantic-roles        
        
        public void Digest()
        {
            new StrategyNameFusion(this).Execute(); 
            new StrategyDegree(this).Execute(); 
            new StrategyPolarity(this).Execute(); 
            new StrategyOp(this).Execute(); 
            new StrategyUnit(this).Execute(); 
            new StrategyQuant(this).Execute(); 
            new StrategyTime(this).Execute();
            new StrategyLocation(this).Execute();
            new StrategyMod(this).Execute();
            new StrategyDomain(this).Execute();
            new StrategyDuration(this).Execute();

            new StrategySolveNullNodes().Execute(this);
            new StrategySolveNullEdgeRelations().Execute(this);
            new StrategyCompressGraph(this).Execute(); 

            new StrategySolveOfRelations().Execute(this);
            new StrategyAssignSemanticRelation(this).Execute();
            //new StrategySplitSemanticRole().Execute(this); //we can not have a token with two semantic role assign
            new StrategyAssignSemanticRole().Execute(this);
            new StrategySynonym().Execute(this);
            new StrategyPageRank(this).Execute(); 
        }

        

        public void GenerateInformativeAspects()
        {
            var rels = this.Nodes.Where(c => c.semanticroles.Contains("rel")).OrderByDescending(c=>c.pagerank).ToList();
            foreach (var item in rels)
            {
                var pos_agents = this.Relations.Where(c => c.Head == item.id && c.f == "pag").Select(c=>c.Tail);
                var pos_patients = this.Relations.Where(c => c.Head == item.id && c.f == "ppt").Select(c => c.Tail);
                var pos_gols = this.Relations.Where(c => c.Head == item.id && c.f == "gol").Select(c => c.Tail);
                var aspect = new CGInformativeAspect()
                {
                    Who = this.Nodes.Where(c => pos_agents.Contains(c.id)).ToList(),
                    What = new List<CGNode>() { item },
                    Who_affected = this.Nodes.Where(c => pos_patients.Contains(c.id)).ToList(),
                    Why = this.Nodes.Where(c => pos_gols.Contains(c.id)).ToList()
                };
                aspect.name = this.InformativeAspects.Count + 1;
                this.InformativeAspects.Add(aspect);
            }
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
