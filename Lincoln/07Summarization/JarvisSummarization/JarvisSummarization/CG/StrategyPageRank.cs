using JarvisSummarization.PageRank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.CG
{
    public class StrategyPageRank
    {
        CGGraph graph;
        public StrategyPageRank(CGGraph graph)
        {
            this.graph = graph;
        }
        public void Execute()
        {
            PRKnowledgeGraph prg = new PRKnowledgeGraph();
            prg.create(this.graph);
            PageRank.PageRank prank = new PageRank.PageRank(prg);
            prank.execute();
        }
    }
}
