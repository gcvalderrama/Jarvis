using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{

    
    public class StrategyExample
    {
        private CGGraph graph;

        public StrategyExample(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("example"))
                {
                    item.description = item.label;
                    item.f = item.label;
                    item.conceptualrole = item.label;
                }
            }
        }
    }
    public class StrategyTopic
    {
        private CGGraph graph;

        public StrategyTopic(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("topic"))
                {
                    item.description = item.label;
                    item.f = item.label;
                    item.conceptualrole = item.label;
                }
            }
        }
    }
    
    public class StrategyDirection
    {
        private CGGraph graph;

        public StrategyDirection(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("direction"))
                {
                    item.description = item.label;
                    item.f = item.label;
                    item.conceptualrole = item.label;
                }
            }
        }
    }
    public class StrategySource
    {
        private CGGraph graph;

        public StrategySource(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("source"))
                {
                    item.description = item.label;
                    item.f = item.label;
                    item.conceptualrole = item.label;
                }
            }
        }
    }
    public class StrategyComparedTo
    {
        private CGGraph graph;

        public StrategyComparedTo(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("compared-to"))
                {
                    item.description = item.label;
                    item.f = item.label;
                    item.conceptualrole = item.label;
                }
            }
        }
    }
    public class StrategyDegree
    {        
        private CGGraph graph;

        public StrategyDegree(CGGraph graph)
        {
            this.graph = graph;
        }
        public void ExecuteAdd()
        {            
            foreach (var item in this.graph.Relations)
            {
                if (item.label.StartsWith("degree"))
                {
                    item.description = item.label;
                    item.f = item.label;
                    item.conceptualrole = item.label; 
                }
            }            
        }
        public void Execute()
        {            
            foreach (var item in this.graph.Relations.ToList())
            {   
                if (item.label.StartsWith("degree"))
                {
                    var node = this.graph.Nodes.Where(c => c.id == item.Tail).Single();
                    this.graph.RecursiveRemoveSubGraph(node); 
                }                
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
                graph.RemoveRelation(item);
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
                graph.RemoveRelation(item);
            }
        }
    }
    
    public class StrategyWeekday
    {
        private CGGraph graph;
        public StrategyWeekday(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label == "weekday")
                {
                    deletes.Add(item);                    
                }
            }
            foreach (var item in deletes)
            {
                this.graph.RemoveRelation(item);
            }
        }
    }
    public class StrategyBeneficiary
    {
        private CGGraph graph;
        public StrategyBeneficiary(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label == "beneficiary")
                {
                    item.f = item.label;
                    item.description = item.label;
                    item.conceptualrole = item.label;
                }
            }
        }
    }
    public class StrategyCondition
    {
        private CGGraph graph;
        public StrategyCondition(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label == "condition")
                {
                    item.f = item.label;
                    item.description = item.label;
                    item.conceptualrole = item.label;
                }
            }
        }
    }

    public class StrategyPoss
    {
        private CGGraph graph;
        public StrategyPoss(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label == "poss")
                {
                    item.f = item.label;
                    item.description = item.label;
                    item.conceptualrole = item.label;
                }
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
            foreach (var item in this.graph.Relations.ToList())
            {
                if (item.label == "quant")
                {
                    var node = this.graph.Nodes.Where(c => c.id == item.Tail).Single();
                    this.graph.RecursiveRemoveSubGraph(node);
                }
            }
            foreach (var item in deletes)
            {
                graph.RemoveRelation(item);
            }
        }
    }
    
    public class StrategyValue
    {
        private CGGraph graph;
        public StrategyValue(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label == "value")
                    deletes.Add(item);
            }
            foreach (var item in deletes)
            {
                graph.RemoveRelation(item);
            }
        }
    }
    public class StrategyX
    {
        private CGGraph graph;
        public StrategyX(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            List<CGRelation> deletes = new List<CGRelation>();
            foreach (var item in this.graph.Relations)
            {
                if (item.label == "x")
                    deletes.Add(item);
            }
            foreach (var item in deletes)
            {
                graph.RemoveRelation(item);
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
            foreach (var item in this.graph.Relations.ToList())
            {
                if (item.label == "time")
                {
                    var node = this.graph.Nodes.Where(c => c.id == item.Tail).Single();
                    this.graph.RecursiveRemoveSubGraph(node);
                }
            }
            
        }
    }
}
