using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WordNet = LAIR.ResourceAPIs.WordNet;
namespace JarvisSummarization.AMR
{
    public class AMRDocument
    {
        public List<AMRGraph> Graphs { get; set; }
                
        public AMRDocument()
        {            
            this.Graphs = new List<AMRGraph>(); 
        }
        
        public void LoadRSTInformation(RST.RSTDocument Document)
        {
            foreach (var graph in this.Graphs)
            {
                
                var weight = Document.Tokens.Where(c => c.eduid == graph.name).Max(c => c.rstweight);
                foreach (var node in graph.Nodes)
                {                    
                    node.rstweight = weight;                    
                }
            }            
        }
    }
}
