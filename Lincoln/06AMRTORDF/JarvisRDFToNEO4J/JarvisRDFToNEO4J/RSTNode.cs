using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisRDFToNEO4J
{
    public enum RSTNodeType
    {
        Nucleus,
        Satellite
    }
    public class RSTNode
    {
        public string name { get; set; }        
        public bool leaf { get; set; }
        public RSTNodeType type { get; set; }
        public string relation { get; set; }
        public string form { get; set; }
        public int edu { get; set; }
        public double Weight { get; set; }
        public string text { get {

                StringBuilder sb = new StringBuilder();
                foreach (var item in this.Words)
                {
                    sb.Append(item.Text + " ");
                }
                return sb.ToString();
            } }        
        public List<RSTNode> Children { get; set; }
        public List<RSTWord> Words { get; set; }
        public RSTNode()
        {
            this.Children = new List<RSTNode>();
            this.Words = new List<RSTWord>(); 
        }                

        public void Load(edu.stanford.nlp.trees.Tree treenode, List<RSTWord> Tokens)
        {
            //leaf node
            if (treenode.value() == "EDU")
            {
                this.leaf = true;
                this.edu = int.Parse(treenode.firstChild().value());
                this.Words = Tokens.Where(c => c.eduid == this.edu).ToList(); 
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

                    if (ids[i] == 'N')
                    {
                        rstnode.type = RSTNodeType.Nucleus;
                    }
                    else
                    {
                        rstnode.type = RSTNodeType.Satellite;
                    }
                    this.Children.Add(rstnode);
                }
                this.Words = new List<RSTWord>(this.Children.SelectMany(c => c.Words).ToArray());
            }
        }

       
    }
}
