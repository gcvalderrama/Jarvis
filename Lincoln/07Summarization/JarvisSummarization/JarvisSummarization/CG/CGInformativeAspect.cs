using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    

    public class CGConcept {
        public double rank { get; protected set;   }
        public CGNode text { get; protected set; }
        public List<CGNode> mods { get; protected set; }
        public CGConcept()
        {
            this.mods = new List<CGNode>();  
        }
        public void AddText(CGNode node)
        {
            this.text = node;
            this.rank += node.pagerank;
        }
        public void AddMod(CGNode mod)
        {
            this.mods.Add(mod);
            this.rank += mod.pagerank;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("ID  {0} Text :{1}" , this.text.id,  this.text.text));
            foreach (var item in this.mods)
            {
                sb.AppendLine("modifier " + item);
            }
            return sb.ToString();
        }
    }
    public class CGInformativeAspect
    {
        public double weight { get {
                return
                    this.Agents.Sum(c => c.rank) +
                    this.What.Sum(c => c.rank) +                    
                    this.Patients.Sum(c => c.rank) +
                    this.Themes.Sum(c=>c.rank) + 
                    this.Goals.Sum(c => c.rank);
            } }        
        
        
        

        [JsonIgnore]
        public List<CGConcept> What { get; set; } // rel
        public List<CGConcept> Agents { get; set; } // rel
        public List<CGConcept> Patients { get; set; } 
        public List<CGConcept> Themes { get; protected set; }
        public List<CGConcept> Goals { get; protected set; }
        public int name { get; set; }
        public CGInformativeAspect()
        {            
            this.What = new List<CGConcept>();                        
            this.Themes = new List<CGConcept>();
            this.Goals = new List<CGConcept>();
            this.Patients = new List<CGConcept>();
            this.Agents = new List<CGConcept>();
        }

        

        //pending 
        public string Where { get; set; }        
        public string How { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("==========================================================================");
            sb.AppendLine("Agents");
            foreach (var item in this.Agents)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine("What");
            foreach (var item in this.What)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine("Patients");
            foreach (var item in this.Patients)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine("Theme");
            foreach (var item in this.Themes)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine("Goals");
            foreach (var item in this.Goals)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine(string.Format("weight {0}", this.weight) );
            sb.AppendLine("==========================================================================");
            return sb.ToString();
        }

    }
}
