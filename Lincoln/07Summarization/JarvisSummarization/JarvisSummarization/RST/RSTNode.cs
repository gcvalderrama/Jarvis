using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.RST
{
    public class RSTNode
    {
        private string _relation;

        public string relation
        {
            get {
                if (string.IsNullOrWhiteSpace(this._relation))
                {
                    return string.Format("{0}-{1}", "EDU", this.edu.name);
                }
                else
                    return _relation;
            }
            set { _relation = value; }
        }

        public string name { get; set; }
        public bool nucleus { get; set; }
        public string form { get; set; }
        
        public double weight { get; set; }
        
        [Newtonsoft.Json.JsonIgnore]
        public RSTEdu edu { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public List<RSTNode> children { get; set; }
        
        public RSTNode()
        {
            this.name = Guid.NewGuid().ToString(); 
            this.children = new List<RSTNode>();
        }
        public void Load(edu.stanford.nlp.trees.Tree treenode, List<Common.Token> Tokens)
        {
            //leaf node
            if (treenode.value() == "EDU")
            {                                
                this.edu = new RSTEdu();
                this.edu.name = int.Parse(treenode.firstChild().value());
                this.edu.tokens = Tokens.Where(c => c.eduid == this.edu.name).ToList();
                this.edu.sentence = this.edu.tokens.Select(c => c.sentence).Distinct().Single();
            }
            else
            {
                var str = treenode.nodeString();
                string[] parts = str.Split('-');
                this.relation = parts[1];                
                this.form = parts[0];
                string ids = parts[0];
                var children = treenode.children();
                if (children.Count() > 2) throw new ApplicationException("error not supported scenario");
                for (int i = 0; i < ids.Length; i++)
                {
                    var child = children[i];
                    var rstnode = new RSTNode();

                    rstnode.Load(child, Tokens);
                    rstnode.nucleus = ids[i] == 'N';
                    this.children.Add(rstnode);                    
                }                
            }
        }
    }
}
