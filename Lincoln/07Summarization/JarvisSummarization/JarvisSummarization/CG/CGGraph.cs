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

        public int NumberOfWords { get; set; }


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
        
      
        //http://es.slideshare.net/underx/semantic-roles        
        

        public void Digest()
        {
            //new StrategyStopWords(this).Execute();
            //convert  possible to can verb 
            new StrategyPossibleToVerb(this).Execute(); 
            //important last
            new StrategyOperatorAndOr(this).Execute();            
            //invert of relation
            new StrategySolveOfRelations().Execute(this);            
            //si tenemos un verbo que no es of- y que es el ultimo debe ser un concepto
            new StrategyVerbToConcept(this).Execute();                       
            
            new StrategyNameFusion(this).Execute();
            new StrategyX(this).Execute();
            
            //no polarity
            new StrategyPolarity(this).Execute();
            new StrategySource(this).Execute();
            new StrategyDirection(this).Execute();  
            //no unit
            new StrategyUnit(this).Execute();
            new StrategyPoss(this).Execute();
            new StrategyCondition(this).Execute();
            new StrategyBeneficiary(this).Execute();
            
            new StrategyQuant(this).Execute();
            new StrategyValue(this).Execute();
            
            new StrategyTime(this).Execute();
            new StrategyExample(this).Execute(); 
            new StrategyTopic(this).Execute(); 
            new StrategyComparedTo(this).Execute(); 
            new StrategyRange(this).Execute();
            new StrategyPrep(this).Execute(); 
            new StrategyLocation(this).Execute();
            new StrategyMod(this).Execute();
            new StrategyDomain(this).Execute();
            new StrategyDuration(this).Execute();
            new StrategyPurpose(this).Execute();
            new StrategyPart(this).Execute();
            new StrategyManner(this).Execute();
            new StrategyInstrument(this).Execute();

            //deletes 
            new StrategyFrequency(this).Execute();
            new StrategyDegree(this).Execute();
            new StrategySolveNullNodes().Execute(this);
            new StrategySolveNullEdgeRelations().Execute(this);
            new StrategyWeekday(this).Execute();
            //solve semantics
            new StrategySolveSemantics(this).Execute();            
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
        
        public void Navigate(CGSentence sentence, CGNode verb)
        {
            sentence.Verb = new CGVerb() { Verb0 = this.BuildVerbExpression(verb, "rel", null) };

            var agents = this.FindByHead(verb.id, "agent");

            foreach (var item in agents)
            {
                sentence.Subject.Items.Add(this.BuildSubjectExpression(item, "agent"));
            }

            var patients = this.FindByHead(verb.id, "patient");            
            foreach (var item in patients)
            {
                if (item.semanticroles.Contains("verb"))
                {
                    sentence.Patient.Items.Add(this.BuildComplexExpression(item, "patient", verb));
                }
                else
                {
                    sentence.Patient.Items.Add(this.BuildPredicateExpression(item, "patient"));
                }
                
            }
            var experiencers = this.FindByHead(verb.id, "experiencer");
            foreach (var item in experiencers)
            {
                sentence.Patient.Items.Add(this.BuildPredicateExpression(item, "patient"));
            }
            var copatients = this.FindByHead(verb.id, "co-patient");
            foreach (var item in copatients)
            {
                sentence.Patient.Items.Add(this.BuildPredicateExpression(item, "patient"));
            }
            var goals = this.FindByHead(verb.id, "goal");
            foreach (var item in goals)
            {
                if (item.semanticroles.Contains("verb"))
                {
                    sentence.Goal.Items.Add(this.BuildComplexExpression(item, "goal", verb));
                }
                else {
                    sentence.Goal.Items.Add(this.BuildPredicateExpression(item, "goal"));
                }
            }
            var results = this.FindByHead(verb.id, "result");
            foreach (var item in results)
            {
                sentence.Goal.Items.Add(this.BuildPredicateExpression(item, "goal"));
            }
            var destinations = this.FindByHead(verb.id, "destination");
            foreach (var item in destinations)
            {
                sentence.Destination.Items.Add(this.BuildPredicateExpression(item, "destination"));
            }
            var themes = this.FindByHead(verb.id, "theme");
            foreach (var item in themes)
            {
                if (item.semanticroles.Contains("verb"))
                {
                    sentence.Theme.Items.Add(this.BuildComplexExpression(item, "theme", verb));
                }
                else
                {
                    sentence.Theme.Items.Add(this.BuildPredicateExpression(item, "theme"));
                }                
            }            
        }

        public void Validate()
        {
            var relations = this.Relations.Where(c => c.conceptualrole == "mod");
            var agents = new List<CGNode>();
            foreach (var item in relations)
            {
                //var head = this.Nodes.Where(c => c.id == item.Head).Single();
                //var tail = this.Nodes.Where(c => c.id == item.Tail).Single();
                //if (tail.semanticroles.Contains("verb"))
                //{
                //    throw new Exception("not valid");
                //}
            }

        }

        public void GenerateInformation()
        {
            Console.WriteLine("Concepts");
            foreach (var item in this.Nodes.Where(c=>c.semanticroles.Contains("concept")))
            {
                Console.WriteLine(item.text);
            }
            Console.WriteLine("Relations");
            foreach (var item in this.Relations.Select(c=>c.conceptualrole).Distinct())
            {
             //   Console.WriteLine(item);
            }
            Console.WriteLine("Verbs");

            foreach (var item in this.Nodes.Where(c=>c.semanticroles.Contains("verb")).OrderByDescending(c=>c.pagerank))
            {
                Console.WriteLine(item.pagerank + "  " + item.text);
            }

            Console.WriteLine("Muti Verbs");
            Console.WriteLine("=====================================================");

            foreach (var item in this.Relations)
            {
                var head = this.Nodes.Where(c => c.id == item.Head).Single();
                var tail = this.Nodes.Where(c => c.id == item.Tail).Single();
                if (head.semanticroles.Contains("verb") && tail.semanticroles.Contains("verb"))
                {                    
                    
                    Console.WriteLine(head.text + " - " +  item.conceptualrole + " : "  +item.label +" -> " + tail.text);

                    var outs = this.Relations.Where(c => c.Head == head.id);
                    foreach (var rel in outs)
                    {
                        var outtail = this.Nodes.Where(c => c.id == rel.Tail).Single();
                        Console.WriteLine("     " + rel.conceptualrole + " : " + rel.label + " = " + outtail.semanticroles.Single()+ " > " + outtail.text);
                    }
                }
            }
            Console.WriteLine("=====================================================");
            foreach (var item in this.Nodes.Where(c => c.semanticroles.Contains("verb")).OrderByDescending(c => c.pagerank))
            {
                Console.WriteLine(item.pagerank + "  " + item.text);
            }

            Console.WriteLine("Agents");


            var agents_relations = this.Relations.Where(c => c.conceptualrole == "agent" ).Select(c=>c.Tail).Distinct();
            var agents = new List<CGNode>();
            foreach (var item in agents_relations)
            {
                var tail = this.Nodes.Where(c => c.id == item).Single();
                agents.Add(tail); 
            }

            foreach (var item in agents.OrderByDescending(c=>c.pagerank))
            {
                Console.WriteLine(item.pagerank +"  "+ item.text); 
            }

            Console.WriteLine("themes");
            
            var theme_relations = this.Relations.Where(c => c.conceptualrole == "theme").Select(c => c.Tail).Distinct();
            var themes = new List<CGNode>();
            foreach (var item in theme_relations)
            {
                var tail = this.Nodes.Where(c => c.id == item).Single();
                themes.Add(tail);
            }

            foreach (var item in themes.OrderByDescending(c => c.pagerank))
            {
                Console.WriteLine(item.pagerank + "  " + item.text);
            }

        }
        public void GenerateInformativeAspectsv2()
        {
            var verbs = this.Nodes.Where(c => c.semanticroles.Contains("verb")).OrderByDescending(c => c.pagerank).ToList();
            foreach (var verb in verbs.Where(c=>c.IsPatientVerb))
            {
                var out_relations = this.Relations.Where(c => c.Head == verb.id);
                foreach (var out_rel in out_relations)
                {
                    var tail = this.Nodes.Where(c => c.id == out_rel.Tail).Single();
                    Console.WriteLine( verb.id.ToString() + ":" +verb.text + " - " + out_rel.conceptualrole + " -> " + tail.semanticroles.Single() + ":"+ tail.text);
                }



             
            }
        }
        public void GenerateInformativeAspects()
        {
            var rels = this.Nodes.Where(c => c.semanticroles.Contains("verb")).OrderByDescending(c=>c.pagerank).ToList();
            foreach (var item in rels)
            {
                var aspect = new CGSentence();
                this.Navigate(aspect,item);
                aspect.name = this.CGSentences.Count + 1;                
                this.CGSentences.Add( aspect );
            }
        }
        public void Summary30()
        {
            var numberofterms = this.NumberOfWords * 0.3;            
            
        }
        public void ReadAMR(AMRDocument Document)
        {
            foreach (var gr in Document.Graphs)
            {
                foreach (var node in gr.Nodes)
                {
                    this.AddNode(new CGNode(node, gr.name));
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
