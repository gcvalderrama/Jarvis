//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace JarvisSummarization.CG
//{
//    class StrategySplitSemanticRole
//    {
//        //we need to guarantee that any node has one possible role
//        public void Execute(CGGraph graph)
//        {
//            var query = from c in graph.Relations.Where(c => c.label.Contains("ARG"))
//                        group c by c.Tail into g
//                        where g.Count() > 1 && g.Select(c => c.f).Distinct().Count() > 1
//                        select g;
//            foreach (var rel in query)
//            {
//                var reloriginal = rel.ElementAt(0);
//                var nodeoriginal = graph.Nodes.Where(c => c.id == reloriginal.Tail).First();
//                var outrelations = graph.Relations.Where(c => c.Head == nodeoriginal.id).ToList();
//                for (int i = 1; i < rel.Count(); i++)
//                {
//                    var clone = nodeoriginal.Clone(graph.Nodes.Max(c => c.id) + 1);
//                    var prelation = rel.ElementAt(i);
//                    prelation.Tail = clone.id;
//                    graph.Nodes.Add(clone);
//                    foreach (var item in outrelations)
//                    {
//                        var clonerel = item.Clone();
//                        clonerel.Head = clone.id;
//                        graph.Relations.Add(clonerel);
//                    }
//                }
//            }

//        }
//    }
//}
