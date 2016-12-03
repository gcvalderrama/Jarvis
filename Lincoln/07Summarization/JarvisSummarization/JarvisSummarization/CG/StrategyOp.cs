using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class StrategyOperatorAndOr
    {
        private CGGraph graph;
        public StrategyOperatorAndOr(CGGraph graph)
        {
            this.graph = graph;
        }
        private void CleanStart()
        {
            List<CGRelation> relations_deletes = new List<CGRelation>();
            List<CGRelation> relations_news = new List<CGRelation>();
            //start and or 
            foreach (var relation in this.graph.Relations)
            {
                var head = this.graph.Nodes.Where(c => c.id == relation.Head).First();
                var in_rels = this.graph.Relations.Where(c => c.Tail == head.id).Count();

                if (in_rels == 0 && (head.nosuffix == "and" || head.nosuffix == "or"))
                {
                    var out_rels = this.graph.Relations.Where(c => c.Head == head.id);
                    relations_deletes.AddRange(out_rels);
                }
            }
            foreach (var item in relations_deletes)
            {
                this.graph.RemoveRelation(item);
            }
        }
        private void CleanInternal(List<CGRelation> relations_deletes, List<CGRelation> relations_news)
        {
            foreach (var item in relations_news)
            {
                this.graph.AddRelation(item);

            }
            foreach (var item in relations_deletes)
            {
                this.graph.RemoveRelation(item);
            }

            relations_news = new List<CGRelation>();
            relations_deletes = new List<CGRelation>();

            foreach (var relation in this.graph.Relations)
            {
                var head = this.graph.Nodes.Where(c => c.id == relation.Head).First();
                var tail = this.graph.Nodes.Where(c => c.id == relation.Tail).First();

                if (tail.nosuffix == "and" || tail.nosuffix == "or")
                {
                    var rels = this.graph.Relations.Where(c => c.Head == tail.id && c.label.StartsWith("op")).ToList();
                    foreach (var item in rels)
                    {
                        var target = this.graph.Nodes.Where(c => c.id == item.Tail).First();
                        var newrel = relation.Clone();
                        newrel.Tail = item.Tail;
                        relations_news.Add(newrel);
                        relations_deletes.Add(item);
                    }
                    relations_deletes.Add(relation);
                    this.CleanInternal(relations_deletes, relations_news);
                    break;
                }
            }

        }
        private void SetSemanticRole()
        {
            foreach (var item in this.graph.Relations.Where(c => c.label.StartsWith("op")))
            {
                item.description = item.label;
                item.f = "op";
                item.conceptualrole = "op";
            }
        }

        public void Execute()
        {
            //this.CleanStart();
            //this.CleanInternal(new List<CGRelation>(), new List<CGRelation>());
            this.SetSemanticRole();
        }

    }
}
