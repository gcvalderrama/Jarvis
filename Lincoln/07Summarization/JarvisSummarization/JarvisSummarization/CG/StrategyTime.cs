using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class StrategyDegree
    {        
        private CGGraph graph;

        public StrategyDegree(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("degree"))
                    deletes.Add(item);
            }
            foreach (var item in deletes)
            {
                graph.Relations.Remove(item);
            }
        }
    }
    public class StrategyPolarity
    {
        private CGGraph graph;

        public StrategyPolarity(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("polarity"))
                    deletes.Add(item);
            }
            foreach (var item in deletes)
            {
                graph.Relations.Remove(item);
            }
        }
    }
    public class StrategyOp
    {
        private CGGraph graph;
        public StrategyOp(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("op"))
                    deletes.Add(item);
            }
            foreach (var item in deletes)
            {
                graph.Relations.Remove(item);
            }
        }
    }
    public class StrategyUnit
    {
        private CGGraph graph;
        public StrategyUnit(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label == "unit")
                    deletes.Add(item);
            }
            foreach (var item in deletes)
            {
                graph.Relations.Remove(item);
            }
        }
    }
    public class StrategyQuant
    {
        private CGGraph graph;
        public StrategyQuant(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label == "quant")
                    deletes.Add(item);
            }
            foreach (var item in deletes)
            {
                graph.Relations.Remove(item);
            }
        }
    }
    public class StrategyTime
    {
        private CGGraph graph;
        public StrategyTime(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label == "time")
                    deletes.Add(item);
            }
            foreach (var item in deletes)
            {
                graph.Relations.Remove(item); 
            }
        }
    }
}
