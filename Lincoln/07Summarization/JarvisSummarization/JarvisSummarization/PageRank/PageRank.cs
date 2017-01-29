using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.PageRank
{
    public class PageRank
    {
        public PRKnowledgeGraph graph;

        public double damping = 0.55f;
        private int nIterations = 50;

        public  List<double> randomSurfing;
        public double[][] transitionMatrix;
        public List<double> pr;
        /**
	 * Constructor
	 * @param graph
	 */
        public PageRank(PRKnowledgeGraph graph)
        {
            this.graph = graph;
            initPageRank();
            initTransitionMatrix();
            initRandomSurfing();
        }
        /**
	 * This method initializes the pagerank vector
	 */
        private void initPageRank()
        {
            pr = new List<double>();
            for (int i = 0; i < graph.Size; i++)
            {
                pr.Add(1d / (double)graph.Size);
            }            
        }
        /**
	 * This method initializes the transition matrix for the PageRank algorithm
	 */
        private void initTransitionMatrix()
        {
            int n = graph.Size;
            transitionMatrix = new double[n][];

            for (int i = 0; i < n; i++)
            {
                transitionMatrix[i] = new double[n];
            }

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    transitionMatrix[i][j] = 0f;

            double[] sum = new double[n];
            for (int j = 0; j < n; j++)
                for (int i = 0; i < n; i++)
                    sum[j] += graph.matrix[i][j];

            for (int j = 0; j < n; j++)
                for (int i = 0; i < n; i++)
                    if (sum[j] != 0)
                        transitionMatrix[i][j] = graph.matrix[i][j] / sum[j];
        }
        /**
	    * This method initializes the Random Surfing Vector
	    */
        private void initRandomSurfing()
        {
            randomSurfing = new List<double>();
            double sum = 0;

            for (int i = 0; i < graph.Size; i++)
            {
                sum += this.graph.GetRSTWeight(i);
            }
            for (int i = 0; i < graph.Size; i++)
            {
                double item = this.graph.GetRSTWeight(i) / sum;
                randomSurfing.Add(item);
                //randomSurfing.Add(0.1d);
            }                
            
        }
        /**
	 * This method executes the pagerank algorithm
	 */
        public void execute()
        {

            for (int z = 0; z < nIterations; z++)
            {

                //Calculating transition Matrix
                List<double> p1 = new List<double>();
                for (int i = 0; i < pr.Count; i++)
                {
                    double sumI = 0d;
                    for (int j = 0; j < pr.Count; j++)
                        sumI += transitionMatrix[i][j] * pr.ElementAt(j);
                    p1.Add(sumI * damping);
                }

                //Calculating random surfing
                List<double> p2 = new List<double>();
                foreach (double rs in randomSurfing)
                    p2.Add(rs * (1 - damping));

                for (int i = 0; i < pr.Count; i++)
                    pr[i] =  p1[i] + p2[i];
            }
        }
        /**
	 * This method returns the Knowledge Graph
	 * @return
	 */
        public PRKnowledgeGraph getGraph()
        {
            return graph;
        }

        /**
	 * 
	 * @param pos
	 * @return
	 */
        public double getPr(int pos)
        {
            return pr[pos];
        }
    }
}
