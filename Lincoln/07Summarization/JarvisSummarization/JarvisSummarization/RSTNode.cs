//using JarvisSummarization.NEO;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace JarvisSummarization
//{
//    public enum RSTNodeType
//    {
//        Nucleus,
//        Satellite
//    }
//    public class RSTNode
//    {
//        public RSTNode()
//        {
//            this.Children = new List<RSTNode>(); 
//        }



//        public double Weight { get; set; }

//        public string name { get; set; }

//        public string text { get; set; }

//        public bool leaf { get; set; }

//        public RSTNodeType type { get; set; }

//        public string relation { get; set; }

//        public IList<RSTNode> Children { get; set; }

//        public void Reduce()
//        {
            
//        }
//        public void Load(NEORSTNode State, NEORSTRelation Relation = null)
//        {
//            this.name = State.name;
//            this.text = State.text;
//            this.leaf = State.leaf;
//            this.relation = State.relation;
//            if (Relation == null)
//            {
//                this.type = RSTNodeType.Nucleus;
//            }
//            else {
//                this.type   = (RSTNodeType)Enum.Parse(typeof(RSTNodeType), Relation.kind);
//            }

            
//        }
//    }
//}
