using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class CGSentence
    {
        public int name { get; set; }
        public CGSubject Subject { get; set; }
        public CGVerb Verb { get; set; }
        public CGPredicate Patient { get; set;}
        public CGPredicate Goal { get; set; }
        public CGPredicate Destination { get; set; }
        public CGPredicate Theme { get; set; }
        public CGSentence()
        {
            this.Subject = new CGSubject();
            this.Verb = new CGVerb();
            this.Patient = new CGPredicate();
            this.Goal = new CGPredicate();
            this.Destination = new CGPredicate();
            this.Theme = new CGPredicate();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("==========================================================================");
            sb.AppendLine("Subject");
            foreach (var item in this.Subject.Items)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine("Verb");
            sb.AppendLine(this.Verb.ToString());
            sb.AppendLine("Patients");
            foreach (var item in this.Patient.Items)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine("Theme");
            foreach (var item in this.Theme.Items)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine("Goals");
            foreach (var item in this.Goal.Items)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine("Destinations");
            foreach (var item in this.Destination.Items)
            {
                sb.AppendLine(item.ToString());
            }

            //sb.AppendLine(string.Format("weight {0}", this.weight));
            sb.AppendLine("==========================================================================");
            return sb.ToString();
        }


    }
    public class CGExpression
    {
        public List<CGNode> Adverbs { get; set; }
        public List<CGNode> Mods { get; set; }
        public List<CGNode> Locations { get; set; }
        public List<CGNode> Degree { get; set; }
        public List<CGNode> Manner { get; set; }

        public CGNode Node { get; set; }
        public string role { get; set; }
        public CGExpression(CGNode node, string role)
        {
            this.Mods = new List<CGNode>();
            this.Locations = new List<CGNode>();        
            this.Adverbs = new List<CGNode>(); 
            this.Manner = new List<CGNode>();
            this.Node = node; this.role = role;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("ID  {0} Text :{1}", this.Node.id, this.Node.text));
            foreach (var item in this.Mods)
            {
                sb.AppendLine("mod " + item.nosuffix);
            }
            foreach (var item in this.Adverbs)
            {
                sb.AppendLine("adv " + item.nosuffix);
            }
            foreach (var item in this.Locations)
            {
                sb.AppendLine("location " + item.nosuffix);
            }
            return sb.ToString();
        }
    }
    public class CGSubject
    {
        public List<CGExpression> Items { get; set; }
        public CGSubject()
        {
            this.Items = new List<CGExpression>();  
        }
        
    }
    public class CGVerb
    {
        public CGExpression Verb0 { get; set; }
        public override string ToString()
        {
            return this.Verb0.ToString();
        }
    }
    
    public class CGPredicate
    {
        public List<CGExpression> Items { get; set; }
        public CGPredicate()
        {
            this.Items = new List<CGExpression>();
        }
    }

 
}
