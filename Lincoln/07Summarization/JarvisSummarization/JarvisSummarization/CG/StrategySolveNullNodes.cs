using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class StrategySolveNullNodes
    {
        public void Execute(CGGraph graph)
        {
            List<CGNode> deletes = new List<CGNode>();
            foreach (var node in graph.Nodes)
            {
                if (node.text == "null_tag" || node.text == "multi-sentence")
                {
                    deletes.Add(node);
                    foreach (var inR in graph.Relations.Where(c=>c.Head == node.id).ToList())
                    {
                        graph.Relations.Remove(inR);
                    }
                    foreach (var outR in graph.Relations.Where(c => c.Tail == node.id).ToList())
                    {
                        graph.Relations.Remove(outR);
                    }
                }
            }
            foreach (var item in deletes)
            {
                graph.Nodes.Remove(item);
            }            
        }
    }
}
