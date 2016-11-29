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
                return this._Relations.Count;  
            }
        }
        [JsonIgnore]
        public List<CGNode> Nodes { get; set; }

        private List<CGRelation> _Relations;

        [JsonIgnore]
        public IEnumerable<CGRelation> Relations { get {
                return this._Relations;
            } }

        [JsonIgnore]
        public List<CGInformativeAspect> InformativeAspects { get; protected set; }

        public void AddRelation(CGRelation relation)
        {
            var exists = this._Relations.Where(c => c.Head == relation.Head && c.Tail == relation.Tail && c.f == relation.f).FirstOrDefault();
            if (exists != null)
                exists.ocurrences += 1;
            else
                this._Relations.Add(relation);
        }
        public void RemoveRelation(CGRelation relation)
        {
            this._Relations.Remove(relation); 
        }
        public CGGraph(string name, string propbankPath)
        {
            this.propbankPath = propbankPath;
            this.name = name; 
            this.Nodes = new List<CGNode>();
            this._Relations = new List<CGRelation>();
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
            //important last
            new StrategyOp(this).Execute();
            //invert of relation
            new StrategySolveOfRelations().Execute(this);
            
            new StrategyNameFusion(this).Execute(); 

            new StrategyDegree(this).Execute(); 
            new StrategyPolarity(this).Execute(); 
            
            new StrategyUnit(this).Execute();
            new StrategyPoss(this).Execute();
            new StrategyCondition(this).Execute();
            new StrategyBeneficiary(this).Execute();
            new StrategyWeekday(this).Execute(); 
            new StrategyQuant(this).Execute();
            new StrategyValue(this).Execute();
            new StrategyTime(this).Execute();
            new StrategyLocation(this).Execute();
            new StrategyMod(this).Execute();
            new StrategyDomain(this).Execute();
            new StrategyDuration(this).Execute();
            new StrategyPurpose(this).Execute();
            new StrategyPart(this).Execute();
            new StrategyManner(this).Execute();
            new StrategyInstrument(this).Execute();  
            new StrategySolveNullNodes().Execute(this);
            new StrategySolveNullEdgeRelations().Execute(this);
            
            
            new StrategyAssignSemanticRelation(this).Execute();
            //new StrategySplitSemanticRole().Execute(this); //we can not have a token with two semantic role assign
            new StrategyAssignSemanticRole(this).Execute();
            new StrategySynonym().Execute(this);
            new StrategySolveSemantics(this).Execute();
            new StrategyCompressGraph(this).Execute();
            new StrategyPageRank(this).Execute(); 
            
        }


        private List<CGNode> GetModifiers(IEnumerable<CGRelation> in_relations)
        {
            List<CGNode> result = new List<CGNode>();
            foreach (var item in in_relations)
            {
                var tmp_relations = this.Relations.Where(c => c.Tail == item.Head).Count();
                if (tmp_relations  == 0)
                {
                    var target = this.Nodes.Where(c => c.id == item.Head).First();
                    result.Add(target);                    
                }
            }
            return result; 
        }

        public CGInformativeAspect Navigate(CGNode who)
        {
            CGInformativeAspect Aspect = new CGInformativeAspect();
            
            var modifiers = this.GetModifiers(this.Relations.Where(c => c.Tail == who.id));
            Aspect.Who.Items.Add(new Tuple<CGNode, IEnumerable<CGNode>>(who, modifiers));

            var in_pag_verb = this.Relations.Where(c => c.Tail == who.id && c.f == "pag").ToList();
            foreach (var item in in_pag_verb)
            {
                var verb = this.Nodes.Where(c => c.id == item.Head).First();
                modifiers = this.GetModifiers(this.Relations.Where(c => c.Tail == item.Tail));
                Aspect.What.Items.Add(new Tuple<CGNode, IEnumerable<CGNode>>(verb, modifiers));
            }
            
            //Aspect.What.Principal = verb;
            //Aspect.What.modifiers = this.GetModifiers(
            //    this.Relations.Where(c => c.Tail == verb.id && c.f == "ppt").ToList());

            ////agents

            //IEnumerable<CGRelation> in_agents_relations;
            //if (verb.IsPatientVerb)
            //{
            //    in_agents_relations = this.Relations.Where(c => c.Head == verb.id && c.f == "ppt").ToList();
            //}
            //else
            //{
            //    in_agents_relations =  this.Relations.Where(c => c.Head == verb.id && c.f == "pag").ToList();
            //}                

            //foreach (var item in in_agents_relations)
            //{
            //    var target = this.Nodes.Where(c => c.id == item.Tail).First();
            //    var modifiers = this.GetModifiers(this.Relations.Where(c => c.Head != item.Head && c.Tail == item.Tail && c.f == "ppt"));
            //    Aspect.Who.Items.Add(new Tuple<CGNode, IEnumerable<CGNode>>( target,  modifiers ));
            //}
            ////patients
            //if (!verb.IsPatientVerb)
            //{
            //    var in_patients_relations = this.Relations.Where(c => c.Head == verb.id && (c.f == "ppt")).ToList();
            //    foreach (var item in in_patients_relations)
            //    {
            //        var target = this.Nodes.Where(c => c.id == item.Tail).First();
            //        var modifiers = this.GetModifiers(this.Relations.Where(c => c.Head != item.Head && c.Tail == item.Tail && c.f == "ppt"));
            //        Aspect.Who_affected.Items.Add(new Tuple<CGNode, IEnumerable<CGNode>>(target, modifiers));
            //    }
            //}
            ////goals
            //var in_gols_relations = this.Relations.Where(c => c.Head == verb.id && (c.f == "gol")).ToList();
            //foreach (var item in in_gols_relations)
            //{
            //    var target = this.Nodes.Where(c => c.id == item.Tail).First();
            //    var modifiers = this.GetModifiers(this.Relations.Where(c => c.Head != item.Head && c.Tail == item.Tail && c.f == "ppt"));
            //    Aspect.Why.Items.Add(new Tuple<CGNode, IEnumerable<CGNode>>(target, modifiers));
            //}

            return Aspect;
        }

        public void GenerateInformativeAspects()
        {
            var rels = this.Nodes.Where(c => c.semanticroles.Contains("pag")).OrderByDescending(c=>c.pagerank).ToList();
            foreach (var item in rels)
            {                   
                var aspect = this.Navigate(item);
                aspect.name = this.InformativeAspects.Count + 1;
                this.InformativeAspects.Add( aspect );
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
                    this.AddRelation(rel);
                }
            }
        }

    }
}
