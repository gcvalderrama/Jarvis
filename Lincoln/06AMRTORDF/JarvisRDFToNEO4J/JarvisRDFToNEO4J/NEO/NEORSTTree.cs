using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisRDFToNEO4J.NEO
{
    public class NEORSTTree
    {
        public NEORSTNode Root { get; set; }
        public string document { get; set; }
        public NEORSTTree() { }
        public NEORSTTree(RSTTree tree) {
            this.document = tree.document;
            this.Root = new NEORSTNode(tree.Root);
            this.GenerateIds(); 
        }
        public void GenerateIds()
        {
            List<NEORSTNode> tmp = new List<NEORSTNode>();
            this.TranformList(this.Root,tmp);
            int id = 0; 
            foreach (var item in tmp)
            {
                item.name = string.Format("{0}-{1}", this.document, id);
                id++;
            }
        }
        private void TranformList(NEORSTNode Parent, List<NEORSTNode> state)
        {
            state.Add(Parent);
            foreach (var relation in Parent.Relations)
            {
                TranformList(relation.Child, state);                 
            }            
        }
    }
}
