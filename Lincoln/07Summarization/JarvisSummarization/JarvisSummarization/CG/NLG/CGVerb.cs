using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG.NLG
{
    public class CGVerbTerm
    {
        public CGNode Node { get; set; }
        public CGRelation Relation { get; set; }
        public int Level { get; set; }
        public CGVerbTerm(CGNode target, CGRelation relation, int level)
        {
            this.Node = target;
            this.Level = level;
            this.Relation = relation;
        }
        public override string ToString()
        {            
            return string.Format(" f:{0} s:{1} id:{2} rel:{3}/ pr:{4} / le:{5} / {6} / {7}", this.Node.FusionNodes.Count() > 0, 
                this.Node.sentenceid, this.Node.id, this.Relation.conceptualrole, this.Node.pagerank, this.Level, this.Node.text, this.Node.nosuffix);
        }
    }
    public class CGVerb
    {
        public CGNode Verb { get; set; }
        public List<CGVerbTerm> VerbAttributes { get; set; }


        public List<string> VerbLoss = new List<string>();
        public List<string> AgentsLoss = new List<string>();
        public List<string> PatientsLoss = new List<string>();
        public List<string> GoalsLoss = new List<string>();
        public List<string> ThemesLoss = new List<string>();
        public List<string> AttributesLoss = new List<string>();

        public List<List<CGVerbTerm>> Agents { get; set; }
        public List<List<CGVerbTerm>> Patients { get; set; }
        public List<List<CGVerbTerm>> Themes { get; set; }
        public List<List<CGVerbTerm>> Goal { get; set; }
        public List<List<CGVerbTerm>> Attributes { get; set; }
        public int Words { get {
                return 1 + 
                    this.Agents.Sum(c=>c.Sum(d=>d.Node.NumberOfWords)) +
                    this.Patients.Sum(c => c.Sum(d => d.Node.NumberOfWords)) +
                    this.Themes.Sum(c => c.Sum(d => d.Node.NumberOfWords)) + 
                    this.Goal.Sum(c => c.Sum(d => d.Node.NumberOfWords)) + 
                    this.Attributes.Sum(c => c.Sum(d => d.Node.NumberOfWords)); 
            } }
        public double Rank {
            get {
                if (this.Agents.Count() + this.Patients.Count() + this.Themes.Count() + this.Goal.Count() == 0)
                    return 0;
                if (this.Agents.Count() != 0 && this.Patients.Count() == 0 && this.Themes.Count() == 0 && this.Goal.Count() == 0)
                    return 0;
                if (this.Agents.Count() == 0 && this.Patients.Count() != 0 && this.Themes.Count() == 0 && this.Goal.Count() == 0)
                    return 0;
                if (this.Agents.Count() == 0 && this.Patients.Count() == 0 && this.Themes.Count() != 0 && this.Goal.Count() == 0)
                    return 0;
                if (this.Agents.Count() == 0 && this.Patients.Count() == 0 && this.Themes.Count() == 0 && this.Goal.Count() != 0)
                    return 0;
                return this.Verb.pagerank + 
                    this.Agents.SelectMany(c => c).Sum(c => c.Node.pagerank) +
                    this.Patients.SelectMany( c=>c).Sum(c=>c.Node.pagerank)  +
                    this.Themes.SelectMany(c=>c).Sum(c=>c.Node.pagerank) + 
                    this.Goal.SelectMany(c=>c).Sum(c=>c.Node.pagerank)  +
                    this.Attributes.SelectMany(c=>c).Sum(c=>c.Node.pagerank);
            }
        }
        public CGVerb(CGNode verb)
        {
            this.Verb = verb;
            this.Agents = new List<List<CGVerbTerm>>();
            this.Patients = new List<List<CGVerbTerm>>();
            this.Themes = new List<List<CGVerbTerm>>();
            this.Goal = new List<List<CGVerbTerm>>();
            this.Attributes = new List<List<CGVerbTerm>>();
            this.VerbAttributes = new List<CGVerbTerm>(); 
        }
        #region Verbs
        public void GenerateVerbs(CGGraph graph)
        {
            var out_verbs = graph.GetOutRelations(this.Verb);
            foreach (var rel in out_verbs)
            {
                if (rel.conceptualrole == "mod" ||
                    rel.conceptualrole == "manner"||
                    rel.conceptualrole == "time")
                {
                    var node = graph.Nodes.Where(c => c.id == rel.Tail).Single();
                    var wrapper = new CGVerbTerm(node, rel, 0);
                    this.VerbAttributes.Add(wrapper);
                }
                else {
                    this.AttributesLoss.Add(string.Format("{0} {1} {2} {3}",Verb.id, Verb.text, rel.conceptualrole, rel.Tail));
                }                
            }
        }
        #endregion
        #region Agents 
        public void GenerateAgents(CGGraph graph)
        {

            var out_agents = graph.GetOutRelationsByConceptualRole(this.Verb, "agent", "co-agent", "experiencer", 
                "stimulus");            

            foreach (var rel in out_agents)
            {
                var node = graph.Nodes.Where(c => c.id == rel.Tail).Single();
                var list = new List<CGVerbTerm>();
                var wrapper = new CGVerbTerm(node, rel, 0);
                list.Add(wrapper);
                NavigateAgent(list, node, graph, 0);
                this.Agents.Add(list);
            }
            var in_agents = graph.GetInRelationsByConceptualRole(this.Verb, "agent-of");
            foreach (var rel in in_agents)
            {
                var node = graph.Nodes.Where(c => c.id == rel.Head).Single();
                var list = new List<CGVerbTerm>();
                var wrapper = new CGVerbTerm(node, rel, 0);
                list.Add(wrapper);
                NavigateAgent(list, node, graph, 0);
                this.Agents.Add(list);
            }
        }
        private void NavigateAgent(List<CGVerbTerm> list, CGNode Target, CGGraph graph, int level)
        {            
            var out_agents = graph.GetOutRelations(Target);            
            foreach (var rel in out_agents)
            {
                var item = graph.Nodes.Where(c => c.id == rel.Tail).Single();
                if (rel.conceptualrole == "mod" ||
                    rel.conceptualrole == "manner" ||
                    rel.conceptualrole == "op" ||
                    rel.conceptualrole == "location"||                    
                    rel.conceptualrole == "direction" ||
                    rel.conceptualrole == "example" ||
                    rel.conceptualrole == "poss" ||
                    rel.conceptualrole == "product" ||
                    rel.conceptualrole == "material" )
                    //rel.conceptualrole == "agent" ||
                    //rel.conceptualrole == "destination" ||
                    //rel.conceptualrole == "time"

                {
                    if (list.Where(c => c.Node.id == item.id).Count() == 0 && 
                        list.Where(c=>c.Node.text == item.text).Count() == 0  )
                    {
                        var wrapper = new CGVerbTerm(item, rel, level + 1);
                        list.Add(wrapper);
                        NavigateAgent(list, item, graph, level + 1);
                    }
                }
                else {
                    this.AgentsLoss.Add(string.Format("Text :{0} / conceptualroles:{1} / Item Text: {2} / Level {3}", Target.text, rel.conceptualrole, item.text, level));
                }
            }
        }
        #endregion

        #region Patients 
        public void GeneratePatients(CGGraph graph)
        {
            var out_patients = graph.GetOutRelationsByConceptualRole(this.Verb, "patient", "co-patient", 
                "recipient", "predicate");
            foreach (var rel in out_patients)
            {                
                var node = graph.Nodes.Where(c => c.id == rel.Tail).Single();
                var list = new List<CGVerbTerm>();
                var wrapper = new CGVerbTerm(node, rel, 0);
                list.Add(wrapper);
                NavigatePatient(list, node, graph, 0);                
                this.Patients.Add(list);
            }
        }
        private void NavigatePatient(List<CGVerbTerm> list, CGNode Target, CGGraph graph, int level)
        {
            var out_agents = graph.GetOutRelations(Target);
            foreach (var rel in out_agents)
            {
                var item = graph.Nodes.Where(c => c.id == rel.Tail).Single();
                if (rel.conceptualrole == "mod" ||
                    rel.conceptualrole == "manner" ||
                    rel.conceptualrole == "op" ||
                    rel.conceptualrole == "domain" ||
                    rel.conceptualrole == "poss" ||
                    rel.conceptualrole == "direction" ||
                    rel.conceptualrole == "location"
                    )
                {

                    if (list.Where(c => c.Node.id == item.id).Count() == 0 &&
                        list.Where(c => c.Node.text == item.text).Count() == 0)
                    {
                        var wrapper = new CGVerbTerm(item, rel, level + 1);
                        list.Add(wrapper);
                        NavigatePatient(list, item, graph, level + 1);
                    }
                }
                else
                {
                    this.PatientsLoss.Add(string.Format("Text :{0} / conceptualroles:{1} / Item Text: {2} / Level {3}", Target.text, rel.conceptualrole, item.text, level));
                }
            }            
        }
        #endregion
        #region Themes 
        public void GenerateThemes(CGGraph graph)
        {
            var out_themes = graph.GetOutRelationsByConceptualRole(this.Verb, "theme", "topic", "co-theme" );
            foreach (var rel in out_themes)
            {
                var node = graph.Nodes.Where(c => c.id == rel.Tail).Single();
                var list = new List<CGVerbTerm>();
                var wrapper = new CGVerbTerm(node, rel, 0);
                list.Add(wrapper);
                NavigateAgent(list, node, graph, 0);
                this.Themes.Add(list);
            }
        }
        private void NavigateTheme(List<CGVerbTerm> list, CGNode Target, CGGraph graph, int level)
        {            
            var out_agents = graph.GetOutRelations(Target);
            foreach (var rel in out_agents)
            {
                var item = graph.Nodes.Where(c => c.id == rel.Tail).Single();
                if (rel.conceptualrole == "mod" ||
                    rel.conceptualrole == "manner" ||
                    rel.conceptualrole == "op" ||
                    rel.conceptualrole == "domain" ||
                    rel.conceptualrole == "poss" ||
                    rel.conceptualrole == "direction" ||
                    rel.conceptualrole == "location" ||
                    rel.conceptualrole == "patient" ||
                    rel.conceptualrole == "purpose")
                {

                    if (list.Where(c => c.Node.id == item.id).Count() == 0 &&
                        list.Where(c => c.Node.text == item.text).Count() == 0)
                    {
                        var wrapper = new CGVerbTerm(item, rel, level + 1);
                        list.Add(wrapper);
                        NavigateTheme(list, item, graph, level + 1);
                    }
                }
                else
                {
                    this.ThemesLoss.Add(string.Format("Text :{0} / conceptualroles:{1} / Item Text: {2} / Level {3}", Target.text, rel.conceptualrole, item.text, level));
                }
            }
        }
        #endregion 

        #region Goal 
        public void GenerateGoal(CGGraph graph)
        {
            var out_themes = graph.GetOutRelationsByConceptualRole(this.Verb, "goal",  "purpose", "destination", "asset");
            foreach (var rel in out_themes)
            {
                var node = graph.Nodes.Where(c => c.id == rel.Tail).Single();
                var list = new List<CGVerbTerm>();
                var wrapper = new CGVerbTerm(node, rel, 0);
                list.Add(wrapper);
                NavigateGoal(list, node, graph, 0);
                this.Goal.Add(list);
            }
        }
        private void NavigateGoal(List<CGVerbTerm> list, CGNode Target, CGGraph graph, int level)
        {         
            var out_agents = graph.GetOutRelations(Target);
            foreach (var rel in out_agents)
            {
                var item = graph.Nodes.Where(c => c.id == rel.Tail).Single();
                if (rel.conceptualrole == "mod" ||
                    rel.conceptualrole == "manner" ||
                    rel.conceptualrole == "op" ||
                    rel.conceptualrole == "domain" ||
                    rel.conceptualrole == "poss" ||
                    rel.conceptualrole == "direction" ||
                    rel.conceptualrole == "location")
                {
                    if (list.Where(c => c.Node.id == item.id).Count() == 0 &&
                        list.Where(c => c.Node.text == item.text).Count() == 0)
                    {
                        var wrapper = new CGVerbTerm(item, rel, level + 1);
                        list.Add(wrapper);
                        NavigateGoal(list, item, graph, level + 1);
                    }
                }
                else
                {
                    this.GoalsLoss.Add(string.Format("Text :{0} / conceptualroles:{1} / Item Text: {2} / Level {3}", Target.text, rel.conceptualrole, item.text, level));
                }
            }
        }
        #endregion 
        #region Attributes 
        public void GenerateAttribute(CGGraph graph)
        {
            var out_themes = graph.GetOutRelationsByConceptualRole(this.Verb, "attribute");
            foreach (var rel in out_themes)
            {
                var node = graph.Nodes.Where(c => c.id == rel.Tail).Single();
                var list = new List<CGVerbTerm>();
                var wrapper = new CGVerbTerm(node, rel, 0);
                list.Add(wrapper);
                NavigateAttribute(list, node, graph, 0);
                this.Attributes.Add(list);
            }
        }
        private void NavigateAttribute(List<CGVerbTerm> list, CGNode Target, CGGraph graph, int level)
        {
            
            var out_agents = graph.GetOutRelations(Target);
            foreach (var rel in out_agents)
            {
                var item = graph.Nodes.Where(c => c.id == rel.Tail).Single();
                if (rel.conceptualrole == "mod" ||
                    rel.conceptualrole == "manner" ||
                    rel.conceptualrole == "op" ||
                    rel.conceptualrole == "domain" ||
                    rel.conceptualrole == "poss" ||
                    rel.conceptualrole == "direction" ||
                    rel.conceptualrole == "location")
                {
                    if (list.Where(c => c.Node.id == item.id).Count() == 0 &&
                        list.Where(c => c.Node.text == item.text).Count() == 0)
                    {
                        var wrapper = new CGVerbTerm(item, rel, level + 1);
                        list.Add(wrapper);
                        NavigateAttribute(list, item, graph, level + 1);
                    }
                }
                else
                {
                    this.AttributesLoss.Add(string.Format("Text :{0} / conceptualroles:{1} / Item Text: {2} / Level {3}", Target.text, rel.conceptualrole, item.text, level));
                }
            }
        }
        #endregion


        

        public string SummaryLemme()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var agent in this.Agents)
            {
                foreach (var item in agent)
                {
                    sb.Append(string.Format("{0} ", item.Node.nosuffix));
                }                
            }            
            //verb attributes are not valid yet
            //foreach (var item in this.VerbAttributes)
            //{
            //    sb.Append(string.Format(" {0} ", item.Node.nosuffix));
            //}
            sb.Append(string.Format(" {0} ", this.Verb.nosuffix));            
            foreach (var patient in this.Patients)
            {
                foreach (var item in patient)
                {
                    sb.Append(string.Format(" {0} ", item.Node.nosuffix));
                }
            }            
            foreach (var theme in this.Themes)
            {
                foreach (var item in theme)
                {
                    sb.Append(string.Format(" {0} ", item.Node.nosuffix));
                }
            }            
            foreach (var gol in this.Goal)
            {
                foreach (var item in gol)
                {
                    sb.Append(string.Format(" {0} ", item.Node.nosuffix));
                }
            }            
            
            foreach (var attribute in this.Attributes)
            {
                foreach (var item in attribute)
                {
                    sb.Append(string.Format(" {0} ", item.Node.nosuffix));
                }
            }
            return sb.ToString();
        }

        public string Log(bool debug)
        {
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine("===================================================");

            //sb.AppendLine(string.Format("{0} : {1} : {2}", this.Verb.id, this.Verb.text, this.Rank));

            //sb.AppendLine("Attributes==>");

            //foreach (var item in this.VerbAttributes)
            //{
            //    sb.AppendLine(item.ToString());
            //}


            //sb.AppendLine("Agents==>");

            //foreach (var agent in this.Agents)
            //{
            //    sb.AppendLine("====");

            //    foreach (var item in agent)
            //    {
            //        sb.AppendLine(item.ToString());
            //    }
            //    sb.AppendLine("====");
            //}
            

            //sb.AppendLine("Patients==>");

            //foreach (var patient in this.Patients)
            //{
            //    sb.AppendLine("====");

            //    foreach (var item in patient)
            //    {
            //        sb.AppendLine(item.ToString());
            //    }
            //    sb.AppendLine("====");
            //}
            //sb.AppendLine("Themes==>");

            //foreach (var theme in this.Themes)
            //{
            //    sb.AppendLine("====");

            //    foreach (var item in theme)
            //    {
            //        sb.AppendLine(item.ToString());
            //    }
            //    sb.AppendLine("====");
            //}
            //sb.AppendLine("Goals==>");

            //foreach (var goal in this.Goal)
            //{
            //    sb.AppendLine("====");

            //    foreach (var item in goal)
            //    {
            //        sb.AppendLine(item.ToString());
            //    }
            //    sb.AppendLine("====");
            //}
            //sb.AppendLine("Attributes==>");

            //foreach (var attribute in this.Attributes)
            //{
            //    sb.AppendLine("====");

            //    foreach (var item in attribute)
            //    {
            //        sb.AppendLine(item.ToString());
            //    }
            //    sb.AppendLine("====");
            //}

            if (debug)
            {
                sb.AppendLine("==========================  Loss Attributes ====================>");
                foreach (var item in this.AttributesLoss)
                {
                    sb.AppendLine(item);
                }
                sb.AppendLine("==========================  End Loss Attributes ====================>");
                sb.AppendLine("=================================== Loss Agents =====================>");
                foreach (var item in this.AgentsLoss)
                {
                    sb.AppendLine(item);
                }
                sb.AppendLine("=================================== End Loss Agents =====================>");
                sb.AppendLine("=================================== Loss Patients==>");
                foreach (var item in this.PatientsLoss)
                {
                    sb.AppendLine(item);
                }
                sb.AppendLine("=================================== End Loss Patients =====================>");
                sb.AppendLine("=================================== Loss Themes==>");
                foreach (var item in this.ThemesLoss)
                {
                    sb.AppendLine(item);
                }
                sb.AppendLine("=================================== End Loss Themes =====================>");
                sb.AppendLine("=================================== Loss Goals==>");
                foreach (var item in this.GoalsLoss)
                {
                    sb.AppendLine(item);
                }
                sb.AppendLine("=================================== End Loss Goals =====================>");
            }

            sb.AppendLine("===================================================");

            return sb.ToString();
        }
    }
}
