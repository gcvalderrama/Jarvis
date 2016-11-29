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
                var tokens = Document.Tokens.Where(c => c.sentence == graph.name)
                    .OrderBy(c => c.sentencepos).ToList();
                foreach (var node in graph.Nodes)
                {
                    var token = tokens.Where(c => c.lemma == node.nosuffix)
                        .OrderByDescending(c => c.rstweight).FirstOrDefault();
                    if (token != null)
                    {
                        node.rstweight = token.rstweight;
                    }
                }
            }
        }
    }
}
