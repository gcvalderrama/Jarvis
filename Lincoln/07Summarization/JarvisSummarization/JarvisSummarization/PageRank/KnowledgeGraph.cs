using edu.stanford.nlp.ling;
using edu.stanford.nlp.trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.PageRank
{
    public class PRKnowledgeGraph
    {
        public int Size { get; set; }
        private IEnumerable<CG.CGNode> vertexs;
        public double[][] matrix;

        public double GetRSTWeight(int pos)
        {
            var result = this.vertexs.Where(c => c.id == pos).SingleOrDefault();
            if (result != null)
                return result.rstweight;
            else
                return 0;
        }
        /**
         * This method builds the knowledge graph 
         * @param words
         * @param dependencies
         */
        public void create(CG.CGGraph cggraph)
        {
            this.Size = cggraph.Nodes.Max(c => c.id) + 1; //start in 0 
            this.vertexs = cggraph.Nodes;
            initMatrix();
            FillMatrix(cggraph.Relations);
        }

        public void FillMatrix(IEnumerable<CG.CGRelation> Relations )
        {
            foreach (var item in Relations)
            {
                this.matrix[item.Head][item.Tail]= 1;
                this.matrix[item.Tail][item.Head] = 1;
            }
        }   

        /**
         * This method initializes the graph's matrix
         */
        private void initMatrix()
        {
            matrix = new double[this.Size][];
            for (int i = 0; i < this.Size; i++)
            { 
                matrix[i] = new double[this.Size];
                for (int j = 0; j < this.Size; j++)
                    matrix[i][j] = 0;
            }
        }
        
        
        /**
         * This method returns the list of vertexs
         * @return
         */
        public IEnumerable<CG.CGNode> getVertexs()
        {
            return vertexs;
        }

    }
}
