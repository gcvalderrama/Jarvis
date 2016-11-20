//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace JarvisSummarization
//{
//    public class RSTTree
//    {
//        public RSTNode Root { get; set; }

          
//        public void Reduce()
//        {
//            this.Root.Weight = 1;
//            NodeReduce(this.Root); 
//        }
//        private void NodeReduce(RSTNode Parent)
//        {
//            if (Parent.leaf)
//            {
//                return;
//            }
//            //evaluate children
//            var Weight = this.Relations[Parent.relation];
//            foreach (var child in Parent.Children)
//            {                
//                if (child.type == RSTNodeType.Nucleus)
//                {
//                    child.Weight = Parent.Weight;
//                }
//                else
//                {
//                    child.Weight = Parent.Weight * Weight;
//                }
//            }
//            foreach (var item in Parent.Children)
//            {
//                NodeReduce(item); 
//            }            
//        }        
//    }
//}
