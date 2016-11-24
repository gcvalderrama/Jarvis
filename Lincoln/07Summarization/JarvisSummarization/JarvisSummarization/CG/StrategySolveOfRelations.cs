using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class StrategySolveOfRelations
    {
        public void Execute(CGGraph graph)
        {
            var relations = from c in graph.Relations.Where(c => c.label.Contains("-of"))
                            select c;

            foreach (var item in relations)
            {
                var head = graph.Nodes.Where(c => c.id == item.Head).First();
                var tail = graph.Nodes.Where(c => c.id == item.Tail).First();
                item.log += string.Format("OF relation {0};", item.label);
                item.label = item.label.Replace("-of", "");
                var tmp = item.Head;
                item.Head = item.Tail;
                item.Tail = tmp;
            }
        }
    }
}
