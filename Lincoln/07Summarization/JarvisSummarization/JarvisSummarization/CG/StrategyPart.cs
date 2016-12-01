using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    class StrategyFrequency
    {
        private CGGraph graph;

        public StrategyFrequency(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("frequency"))
                {
                    item.f = item.label;
                    item.description = item.label;
                    item.conceptualrole = item.label;
                }
            }
        }
    }
    class StrategyPrep
    {
        private CGGraph graph;

        public StrategyPrep(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("prep-"))
                {
                    item.f = item.label;
                    item.description = item.label;
                    item.conceptualrole = item.label;
                }
            }
            
        }
    }
    class StrategyPart
    {
        private CGGraph graph;

        public StrategyPart(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("part"))
                {
                    item.description = item.label;
                    item.f = item.label;
                    item.conceptualrole = item.label;
                }                    
            }            
        }
    }
}
