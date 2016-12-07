using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class StrategySolveOfRelations
    {
        private CGGraph graph;
        public StrategySolveOfRelations(CGGraph graph)
        {
            this.graph = graph;
        }

        public void Execute()
        {
            var relations_ARG0 = from c in graph.Relations.Where(c => c.label.Contains("ARG0-of"))
                            select c;
            foreach (var item in relations_ARG0)
            {
                if (this.graph.IsLeaf(item.Tail))
                {
                    item.description = item.label;
                    item.f = "mod-of";
                    item.conceptualrole = "mod-of";
                }
                else
                {
                    item.description = item.label;
                    item.f = "agent-of";
                    item.conceptualrole = "agent-of";
                }
                
            }
            var relations_ARG1 = from c in graph.Relations.Where(c => c.label.Contains("ARG1-of"))
                                 select c;
            foreach (var item in relations_ARG1)
            {
                if (this.graph.IsLeaf(item.Tail))
                {
                    item.description = item.label;
                    item.f = "mod-of";
                    item.conceptualrole = "mod-of";
                }
                else
                {
                    item.description = item.label;
                    item.f = "patient-of";
                    item.conceptualrole = "patient-of";
                }
                
            }
            var relations_ARG2 = from c in graph.Relations.Where(c => c.label.Contains("ARG2-of"))
                                 select c;
            foreach (var item in relations_ARG2)
            {
                if (this.graph.IsLeaf(item.Tail))
                {
                    item.description = item.label;
                    item.f = "mod-of";
                    item.conceptualrole = "mod-of";
                }
                else
                {
                    item.description = item.label;
                    item.f = "theme-of";
                    item.conceptualrole = "theme-of";
                }
                
            }
        }

        public void Execute(CGGraph graph)
        {
            List<CGRelation> deletes = new List<CGRelation>(); 

            var relations = from c in graph.Relations.Where(c => c.label.Contains("-of"))
                            select c;

            foreach (var item in relations)
            {
                var head = graph.Nodes.Where(c => c.id == item.Head).First();
                var tail = graph.Nodes.Where(c => c.id == item.Tail).First();
                if (!tail.text.Contains("-0"))
                {
                    //deletes.Add(item);
                }
                else {
                    item.log += string.Format("OF relation {0};", item.label);
                    item.label = item.label.Replace("-of", "");
                    var tmp = item.Head;
                    item.Head = item.Tail;
                    item.Tail = tmp;
                }                
            }
            foreach (var item in deletes)
            {
                graph.RemoveRelation(item); 
            }
        }
    }
}
