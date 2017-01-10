using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisSummarization.AMR
{
    public class AMRReader
    {
        public AMRDocument ReadContent(string Content)
        {
            AMRDocument document = new AMRDocument();            
            var amrelement = XElement.Parse(Content);
            var grapselement = from c in amrelement.Elements("graph")
                               select c;
            foreach (var graphelement in grapselement)
            {
                var graph = new AMRGraph();
                graph.ReadXML(graphelement);
                document.Graphs.Add(graph);
            }
            return document;
        }

        public AMRDocument ReadXML(string path)
        {            
            var str = File.ReadAllText(path);
            return this.ReadContent(str);
        }
    }
}
