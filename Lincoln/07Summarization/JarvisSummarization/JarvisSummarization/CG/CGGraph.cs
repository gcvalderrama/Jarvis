using JarvisSummarization.AMR;
using JarvisSummarization.CG.NLG;
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
        public int NumberOfFusions { get; set; }
        public int NumberOfWords { get; set; }

        public string log { get; set; }

        public int numnodes { get {
                return this.Nodes.Count();
            } }
        public int numrelations {
            get {
                return this._Relations.Count;  
            }
        }
        private List<CGNode> _Nodes = new List<CGNode>();

        [JsonIgnore]
        public IEnumerable<CGNode> Nodes { get {
                return this._Nodes;
            }}

        private List<CGRelation> _Relations;

        [JsonIgnore]
        public IEnumerable<CGRelation> Relations { get {
                return this._Relations;
            } }

        [JsonIgnore]
        public List<CGSentence> CGSentences { get; protected set; }

        public void AddRelation(CGRelation relation)
        {
            var exists = this._Relations.Where(c => c.Head == relation.Head && c.Tail == relation.Tail && c.f == relation.f).FirstOrDefault();
            if (exists != null)
                exists.ocurrences += 1;
            else
                this._Relations.Add(relation);
        }
        public void AddNode(CGNode Node)
        {
            this._Nodes.Add(Node);
        }
        public void RemoveNode(CGNode Node)
        {
            var in_rels = this.Relations.Where(c => c.Tail == Node.id).ToList();
            var out_rels = this.Relations.Where(c => c.Head == Node.id).ToList();

            
            foreach (var item in in_rels)
            {
                this.RemoveRelation(item);
            }
            foreach (var item in out_rels)
            {
                this.RemoveRelation(item);
            }
            this._Nodes.Remove(Node); 
        }
        public void RemoveRelation(CGRelation relation)
        {
            this._Relations.Remove(relation); 
        }
        

        

        public CGGraph(string name, string propbankPath, int numberofwords)
        {
            this.propbankPath = propbankPath;
            this.name = name;
            this.NumberOfWords = numberofwords; 
            this._Relations = new List<CGRelation>();
            this.CGSentences = new List<CGSentence>(); 
        }


        public string Stadistics()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("========================Statistics======================");
            sb.AppendLine(string.Format("number of nodes : {0}", this.Nodes.Count()));
            sb.AppendLine(string.Format("number of edges : {0}", this.Relations.Count()));
            sb.AppendLine(string.Format("number of Fusions : {0}", this.NumberOfFusions));

            var verbs = this.Nodes.Where(c => c.semanticroles.Contains("verb"));
            var nonverbs = this.Nodes.Where(c => !c.semanticroles.Contains("verb"));

            sb.AppendLine("==>verb conceptual relations");
            var conceptual_relations = verbs.SelectMany(c => this.GetRelations(c.id).Select(d => d.conceptualrole)).Distinct();
            sb.AppendLine(string.Join(",", conceptual_relations));

            sb.AppendLine("<==");
            sb.AppendLine("==>non verb conceptual relations");
            var nonconceptual_relations = nonverbs.SelectMany(c => this.GetRelations(c.id).Select(d => d.conceptualrole)).Distinct();
            sb.AppendLine(string.Join(",", nonconceptual_relations));
            return sb.ToString(); 

        }


        //http://es.slideshare.net/underx/semantic-roles        

        public IEnumerable<CGRelation> GetOutRelations(CGNode Node)
        {
            var relations = this.Relations.Where(c => c.Head == Node.id).ToList();
            return relations;
        }
        public IEnumerable<CGRelation> GetOutRelationsByConceptualRole(CGNode Node, params string[] Labels)
        {
            var relations = this.Relations.Where(c => c.Head == Node.id && 
                    Labels.Contains(c.conceptualrole) ).ToList();
            return relations;
        }
        public IEnumerable<CGRelation> GetRelations(int id)
        {
            var relations = this.Relations.Where(c => c.Head == id || c.Tail == id).ToList();
            return relations;  
        }
        public  IEnumerable<CGRelation> GetInRelations(CGNode Node)
        {
            var relations = this.Relations.Where(c => c.Tail == Node.id).ToList();
            return relations;
        }
        public IEnumerable<CGRelation> GetInRelationsByConceptualRole(CGNode Node, params string[] Labels)
        {
            var relations = this.Relations.Where(c => c.Tail == Node.id &&
                    Labels.Contains(c.conceptualrole)).ToList();
            return relations;
        }
        public bool IsLeaf(CGNode Node)
        {
            return this.GetOutRelations(Node).Count() == 0;
        }
        public bool IsLeaf(int id)
        {
            var node = this.Nodes.Where(c => c.id == id).Single();
            return this.IsLeaf(node);
        }
        public List<CGNode> GetChildren(CGNode Node)
        {
            var rels = this.GetOutRelations(Node);
            var result = new List<CGNode>();
            foreach (var item in rels)
            {
                var it = this.Nodes.Where(c => c.id == item.Tail).Single();
                result.Add(it);
            }
            return result; 
        }
        public List<CGNode> GetChildrenByConceptualRole(CGNode Node, params string[] labels)
        {
            var rels = this.GetOutRelationsByConceptualRole(Node, labels);
            var result = new List<CGNode>();
            foreach (var item in rels)
            {
                var it = this.Nodes.Where(c => c.id == item.Tail).Single();
                result.Add(it);
            }
            return result;
        }
        public List<CGNode> GetParentsByConceptualRole(CGNode Node, params string[] labels)
        {
            var rels = this.GetInRelationsByConceptualRole(Node, labels);
            var result = new List<CGNode>();
            foreach (var item in rels)
            {
                var it = this.Nodes.Where(c => c.id == item.Head).Single();
                result.Add(it);
            }
            return result;
        }

        public void FusionNodes(CGNode Root, List<CGNode> Target)
        {
            List<CGNode> deletes = new List<CGNode>();
            List<CGRelation> relations_deletes = new List<CGRelation>();
            List<CGRelation> relations_news = new List<CGRelation>();
            foreach (var node in Target)
            {
                if (node.id == Root.id) continue; 

                this.NumberOfFusions += 1;
                Root.AddFusionNode(node);

                deletes.Add(node);
                var inrelations = (from c in this.Relations where c.Tail == node.id select c).ToList();
                foreach (var item in inrelations)
                {
                    var n = item.Clone();
                    n.Tail = Root.id;
                    relations_deletes.Add(item);
                    if (deletes.Where(c => c.id == n.Head).Count() == 0)
                    {
                        relations_news.Add(n);
                    }
                }
                var outrelations = (from c in this.Relations where c.Head == node.id select c).ToList();
                foreach (var item in outrelations)
                {
                    var n = item.Clone();
                    n.Head = Root.id;
                    relations_deletes.Add(item);
                    if (deletes.Where(c => c.id == n.Tail).Count() == 0)
                    {
                        relations_news.Add(n);
                    }
                }                
                foreach (var item in relations_deletes)
                {
                    this.RemoveRelation(item);
                }
                foreach (var item in relations_news)
                {
                    this.AddRelation(item);
                }
                foreach (var item in deletes)
                {
                    this.RemoveNode(item);
                }
            }

        }

        public void Digest()
        {
            //new StrategyStopWords(this).Execute();
            //convert  possible to can verb 
            new StrategyPossibleToVerb(this).Execute();
            //important last
            //new StrategyOperatorAndOr(this).Execute();            

            //si tenemos un verbo que no es of- y que es el ultimo debe ser un concepto
            
            //leaf mod must be fusion
            

            new StrategyEntity(this).Execute();
            new StrategyPersonFusion(this).Execute();
            new StrategyDataEntity(this).Execute();
            new StrategyTemporalQuantity(this).Execute();
            new StrategyMonetaryQuantity(this).Execute();
            new StrategyVerbToConcept(this).Execute();
            
            new StrategyMod(this).Execute();
            new StrategyMod(this).Execute();
            new StrategyMod(this).Execute();

            //new StrategyDependencies(this).Execute();
            //new StrategyX(this).Execute();

            //invert of relation
            new StrategySolveOfRelations(this).Execute();

            //no polarity
            //new StrategyPolarity(this).Execute();
            //new StrategySource(this).Execute();
            //new StrategyDirection(this).Execute();  
            //no unit
            //new StrategyUnit(this).Execute();
            //new StrategyPoss(this).Execute();
            //new StrategyCondition(this).Execute();
            //new StrategyBeneficiary(this).Execute();
                        
            //new StrategyValue(this).Execute();


            //new StrategyExample(this).Execute(); 
            //new StrategyTopic(this).Execute(); 
            //new StrategyComparedTo(this).Execute(); 
            //new StrategyRange(this).Execute();
            //new StrategyPrep(this).Execute(); 
            //new StrategyLocation(this).Execute();
            
            //new StrategyDomain(this).Execute();
            //
            //new StrategyPurpose(this).Execute();
            //new StrategyPart(this).Execute();
            //new StrategyManner(this).Execute();
            //new StrategyInstrument(this).Execute();

            //deletes 
            //new StrategyDuration(this).Execute();
            //new StrategyFrequency(this).Execute();
            //new StrategyDegree(this).Execute();
            //new StrategyTime(this).Execute();
            //new StrategyQuant(this).Execute();
            new StrategySolveNullNodes().Execute(this);
            new StrategySolveNullEdgeRelations().Execute(this);
            //new StrategyWeekday(this).Execute();
            
            //solve semantics
            //new StrategySolveSemantics(this).Execute();            
            new StrategyAssignSemanticRelation(this).Execute();
            //new StrategySplitSemanticRole().Execute(this); //we can not have a token with two semantic role assign
            new StrategyAssignSemanticRole(this).Execute();

            new StrategySynonym(this).Execute();


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

        public void RemoveSubGraph(CGNode Start)
        {
            this.RecursiveRemoveSubGraph(Start);
        }
        //metodo no terminado cuidado
        public void RecursiveRemoveSubGraph(CGNode Target)
        {
            var nodes = new List<CGNode>();
            var in_relations = this.GetInRelations(Target);            
            foreach (var item in in_relations)
            {
                var head = this.Nodes.Where(c => c.id == item.Head).Single();
                nodes.Add(head);
                this.RemoveRelation(item);                
            }
            var out_relations = this.GetOutRelations(Target);
            foreach (var item in out_relations)
            {
                var tail = this.Nodes.Where(c => c.id == item.Tail).Single();
                nodes.Add(tail);
                this.RemoveRelation(item);
            }
            foreach (var item in out_relations)
            {
                var tail = this.Nodes.Where(c => c.id == item.Tail).Single();
                this.RecursiveRemoveSubGraph(tail);
            }
        }
        public List<CGNode> FindByTail(int Tail)
        {
            var result = new List<CGNode>();  
            var relations = this.Relations.Where(c => c.Tail == Tail);

            foreach (var item in relations)
            {
                var head = this.Nodes.Where(c => c.id == item.Head).Single();
                result.Add(head);  
            }
            return result; 
        }
        public List<CGNode> FindByHead(int Head, string conceptualrole)
        {
            var result = new List<CGNode>();
            var relations = this.Relations.Where(c => c.Head == Head && c.conceptualrole == conceptualrole);

            foreach (var item in relations)
            {
                var tail = this.Nodes.Where(c => c.id == item.Tail).Single();
                result.Add(tail);
            }
            return result;
        }
        public CGExpression BuildSubjectExpression(CGNode target, string role)
        {
            CGExpression expression = new CGExpression(target, role);
            expression.Mods = this.FindByHead(target.id, "mod");
            expression.Locations = this.FindByHead(target.id, "location");
            expression.Degree = this.FindByHead(target.id, "degree");
            expression.Manner = this.FindByHead(target.id, "manner");
            expression.Poss = this.FindByHead(target.id, "poss");

            var ins = this.Relations.Where(c => c.Tail == target.id);
            foreach (var item in ins)
            {
                var head = this.Nodes.Where(c => c.id == item.Head).Single(); 
                if (item.conceptualrole != "agent" 
                    && item.conceptualrole != "theme"
                    && item.conceptualrole != "source"
                    && item.conceptualrole != "result"
                    && item.conceptualrole != "experiencer"
                    && item.conceptualrole != "patient" 
                    && item.conceptualrole != "manner"
                    && item.conceptualrole != "mod"
                    && item.conceptualrole != "location"
                    && item.conceptualrole != "attribute"
                    && item.conceptualrole != "prep-to"
                    && item.conceptualrole != "asset"
                    && item.conceptualrole != "co-patient"
                    && item.conceptualrole != "poss"
                    && item.conceptualrole != "op"
                    && item.conceptualrole != "pivot"
                    && item.conceptualrole != "topic"
                    && item.conceptualrole != "goal"
                    && item.conceptualrole != "compared-to"
                    )                    
                    throw new ApplicationException("delete");

            }
            var outs = this.Relations.Where(c => c.Head == target.id);
            foreach (var item in outs)
            {
                if (item.conceptualrole != "mod" &&
                    item.conceptualrole != "poss" &&
                    item.conceptualrole != "location" &&
                    item.conceptualrole != "asset")
                    throw new ApplicationException("delete");
            }

            return expression;
        }
        public CGExpression BuildPredicateExpression(CGNode target, string role)
        {
            CGExpression expression = new CGExpression(target, role);
            expression.Mods = this.FindByHead(target.id, "mod");
            expression.Locations = this.FindByHead(target.id, "location");
            expression.Degree = this.FindByHead(target.id, "degree");
            expression.Manner = this.FindByHead(target.id, "manner");
            expression.Poss = this.FindByHead(target.id, "poss");

            var ins = this.Relations.Where(c => c.Tail == target.id);
            foreach (var item in ins)
            {
                var head = this.Nodes.Where(c => c.id == item.Head).Single();
                if (item.conceptualrole == "patient")
                {
                    var tmp = this.Relations.Where(c => c.Tail == item.Head).Count();
                    if (tmp == 0)
                    {
                        expression.Adverbs.Add(head);
                    }
                }
                else if (item.conceptualrole != "theme" &&
                    item.conceptualrole != "agent" &&
                    item.conceptualrole != "destination" &&
                    item.conceptualrole != "attribute" &&
                    item.conceptualrole != "purpose" &&
                    item.conceptualrole != "result" &&
                    item.conceptualrole != "source" &&
                    item.conceptualrole != "co-patient" &&
                    item.conceptualrole != "experiencer" &&
                    item.conceptualrole != "theme" &&
                    item.conceptualrole != "goal" &&
                    item.conceptualrole != "asset" &&
                    item.conceptualrole != "instrument" &&
                    item.conceptualrole != "location" &&
                    item.conceptualrole != "mod" &&
                    item.conceptualrole != "prep-to" &&
                    item.conceptualrole != "op" &&
                    item.conceptualrole != "poss" &&
                    item.conceptualrole != "pivot" &&
                    item.conceptualrole != "topic" &&
                    item.conceptualrole != "stimulus" &&
                    item.conceptualrole != "co-agent" &&
                    item.conceptualrole != "product" &&
                    item.conceptualrole != "manner" &&
                    item.conceptualrole != "prep-instead" &&                    
                    item.conceptualrole != "compared-to" 
                    )
                    throw new ApplicationException("delete");

            }
            var outs = this.Relations.Where(c => c.Head == target.id);
            foreach (var item in outs)
            {
                if (item.conceptualrole != "mod" &&
                    item.conceptualrole != "poss" &&
                    item.conceptualrole != "degree" &&
                    item.conceptualrole != "manner" &&
                    item.conceptualrole != "location"&& 
                    item.conceptualrole != "agent" &&
                    item.conceptualrole != "destination" &&
                    item.conceptualrole != "purpose" &&
                    item.conceptualrole != "result" &&
                    item.conceptualrole != "patient" &&
                    item.conceptualrole != "co-patient" &&
                    item.conceptualrole != "experiencer" &&
                    item.conceptualrole != "theme" &&
                    item.conceptualrole != "goal" &&
                    item.conceptualrole != "instrument" &&
                    item.conceptualrole != "topic" &&
                    item.conceptualrole != "prep-to" &&
                    item.conceptualrole != "prep-instead" &&
                    item.conceptualrole != "op")
                    throw new ApplicationException("delete");
            }

            return expression;
        }
        public CGExpression BuildVerbExpression(CGNode target, string role, CGNode previous)
        {
            CGExpression expression = new CGExpression(target, role);
            expression.Mods = this.FindByHead(target.id, "mod");
            expression.Locations = this.FindByHead(target.id, "location");            
            expression.Degree = this.FindByHead(target.id, "degree");
            expression.Manner = this.FindByHead(target.id, "manner");
            var ins = this.Relations.Where(c => c.Tail == target.id &&
                        (previous != null && c.Head != previous.id));
            foreach (var item in ins)
            {
                var head = this.Nodes.Where(c => c.id == item.Head).Single();
                if (item.conceptualrole == "patient")
                {
                    var tmp = this.Relations.Where(c => c.Tail == item.Head ).Count();
                    if (tmp == 0)
                    {
                        expression.Adverbs.Add(head);
                    }
                }
                else if (item.conceptualrole != "mod" &&
                   item.conceptualrole != "location" &&
                   item.conceptualrole != "degree" &&
                   item.conceptualrole != "manner" &&
                   item.conceptualrole != "agent" &&
                   item.conceptualrole != "destination" &&
                   item.conceptualrole != "purpose" &&
                   item.conceptualrole != "result" &&                   
                   item.conceptualrole != "co-patient" &&
                   item.conceptualrole != "experiencer" &&
                   item.conceptualrole != "theme" &&
                   item.conceptualrole != "goal" &&
                   item.conceptualrole != "instrument" &&
                   item.conceptualrole != "op")
                    throw new ApplicationException("delete");
            }
            var outs = this.Relations.Where(c => c.Head == target.id);
            foreach (var item in outs)
            {
                var tail = this.Nodes.Where(c => c.id == item.Tail).Single();
                if (item.conceptualrole != "mod" &&
                    item.conceptualrole != "location" &&
                    item.conceptualrole != "degree" &&
                    item.conceptualrole != "manner" &&
                    item.conceptualrole != "agent" &&
                    item.conceptualrole != "destination" &&
                    item.conceptualrole != "stimulus" &&                    
                    item.conceptualrole != "purpose" &&
                    item.conceptualrole != "result" &&
                    item.conceptualrole != "patient" &&
                    item.conceptualrole != "co-patient" &&
                    item.conceptualrole != "experiencer" &&
                    item.conceptualrole != "theme" && 
                    item.conceptualrole != "goal" &&
                    item.conceptualrole != "source" &&
                    item.conceptualrole != "topic" &&
                    item.conceptualrole != "asset" &&
                    item.conceptualrole != "instrument" &&
                    item.conceptualrole != "op" &&
                    item.conceptualrole != "attribute" &&
                    item.conceptualrole != "prep-under" &&
                    item.conceptualrole != "predicate" &&
                    item.conceptualrole != "end")
                    throw new ApplicationException("delete");
            }
            return expression;
        }
        public CGExpression BuildComplexExpression(CGNode verb, string role, CGNode previous)
        {
            var verbexpression = this.BuildVerbExpression(verb, role, previous);
            CGComplexExpression expression = new CGComplexExpression(verbexpression, role);            
                                    
            var patients = this.FindByHead(verb.id, "patient");
            foreach (var item in patients)
            {
                if (!item.semanticroles.Contains("verb"))
                    expression.Patient.Items.Add(this.BuildPredicateExpression(item, "patient"));
            }
            var experiencers = this.FindByHead(verb.id, "experiencer");
            foreach (var item in experiencers)
            {
                if (!item.semanticroles.Contains("verb"))
                    expression.Patient.Items.Add(this.BuildPredicateExpression(item, "patient"));
            }
            var copatients = this.FindByHead(verb.id, "co-patient");
            foreach (var item in copatients)
            {
                if (!item.semanticroles.Contains("verb"))
                    expression.Patient.Items.Add(this.BuildPredicateExpression(item, "patient"));
            }
            var results = this.FindByHead(verb.id, "result");
            foreach (var item in results)
            {
                if (!item.semanticroles.Contains("verb"))
                    expression.Result.Items.Add(this.BuildPredicateExpression(item, "result"));
            }
            return expression;
        }       
        
        
        public void GenerateInformation()
        {
            Console.WriteLine("Concepts");
            foreach (var item in this.Nodes.Where(c => c.IsConcept).OrderByDescending(c => c.pagerank))
            {
                Console.WriteLine(string.Format("{0}:{1}-{2}", item.id, string.Join(",", item.semanticroles), item.text));
            }
            Console.WriteLine("Entities");
            foreach (var item in this.Nodes.Where(c => c.IsEntity).OrderByDescending(c => c.pagerank))
            {
                Console.WriteLine(string.Format("{0}:{1}-{2}", item.id, string.Join(",", item.semanticroles), item.text));
            }
            Console.WriteLine("Agents");
            foreach (var item in this.Nodes.Where(c => c.semanticroles.Contains("agent")).OrderByDescending(c => c.pagerank))
            {
                Console.WriteLine(string.Format("{0}:{1}-{2}", item.id, string.Join(",", item.semanticroles), item.text));
            }
            Console.WriteLine("Verbs");

            foreach (var item in this.Nodes.Where(c=>c.semanticroles.Contains("verb")).OrderByDescending(c=>c.pagerank))
            {
                Console.WriteLine(string.Format("{0}:{1}-{2}", item.id, string.Join(",", item.semanticroles), item.text));
            }

            Console.WriteLine("Themes");

            foreach (var item in this.Nodes.Where(c => c.semanticroles.Contains("theme")).OrderByDescending(c => c.pagerank))
            {
                Console.WriteLine(string.Format("{0}:{1}-{2}", item.id, string.Join(",", item.semanticroles), item.text));
            }

            Console.WriteLine("Goal");

            foreach (var item in this.Nodes.Where(c => c.semanticroles.Contains("goal")).OrderByDescending(c => c.pagerank))
            {
                Console.WriteLine(string.Format("{0}:{1}-{2}", item.id, string.Join(",", item.semanticroles), item.text));
            }


        }
        public string Summary()
        {
            StringBuilder sb = new StringBuilder();

            var result = new List<CGVerb>();            

            var vers = this.Nodes.Where(c => c.semanticroles.Contains("verb")).OrderByDescending(c => c.pagerank).ToList();

            foreach (var verb in vers)
            {
                var cgverb = new CGVerb(verb);
                cgverb.GenerateVerbs(this);
                cgverb.GenerateAgents(this);
                cgverb.GeneratePatients(this);
                cgverb.GenerateThemes(this);
                cgverb.GenerateGoal(this);
                cgverb.GenerateAttribute(this);
                result.Add(cgverb);
            }
            int words = 0;
            foreach (var item in result.OrderByDescending(c => c.Rank))
            {
                words += item.Words;
                sb.AppendLine(item.SummaryLemme());                
                if (words > 100)
                {
                    break;
                }
            }
            return sb.ToString();
        }
        public string GenerateInformativeAspectsv2()
        {
            StringBuilder sb = new StringBuilder(); 
            var result = new List<CGVerb>();

            var noverbs = new List<string>() {  };

            var vers = this.Nodes.Where(c => 
                !noverbs.Contains(c.nosuffix) &&
                c.semanticroles.Contains("verb")).OrderByDescending(c => c.pagerank).ToList();
            foreach (var verb in vers)
            {
                var cgverb = new CGVerb(verb);
                cgverb.GenerateVerbs(this);
                cgverb.GenerateAgents(this);
                cgverb.GeneratePatients(this);
                cgverb.GenerateThemes(this);
                cgverb.GenerateGoal(this); 
                result.Add(cgverb);
            }
            int words = 0;
            foreach (var item in result.OrderByDescending(c=>c.Rank))
            {
                words += item.Words;
                sb.AppendLine(item.Log(false));                
                if ( words > 100)
                {
                    break;
                }
            }
            return sb.ToString();
        }
                
        public void ReadAMR(AMRDocument Document)
        {
            foreach (var gr in Document.Graphs)
            {
                for (int i = 0; i < gr.Nodes.Count; i++)
                {
                    var node = gr.Nodes.ElementAt(i);
                    var g = new CGNode(node, gr.name);
                    if (i == 0)
                    {
                        g.AddSemanticRole("root");
                    }
                    this.AddNode(g);
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
