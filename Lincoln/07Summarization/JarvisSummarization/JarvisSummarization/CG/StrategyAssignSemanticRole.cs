using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyAssignSemanticRole
    {
        public void Execute(CGGraph graph)
        {            
            foreach (var node in graph.Nodes)
            {
                var in_relations = graph.Relations.Where(c=>c.Tail == node.id);
                if (in_relations.Count() > 0)
                {
                    foreach (var rel in in_relations)
                    {
                        node.AddSemanticRole(rel.f);
                    }                    
                }
                else {
                    //caso no relaciones puede ser un verbo o agente
                    if (node.text.Contains("-0"))
                    {
                        node.AddSemanticRole("rel");
                    }
                    else {
                        node.AddSemanticRole("pag");
                    }    

                }
                //siempre si es un verbo es un rel 
                if (node.text.Contains("-0"))
                {
                    node.AddSemanticRole("rel");
                }
            }            
        }
    }
}
