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
        public AMRDocument ReadXML(string path, string propbankpath)
        {
            AMRDocument document = new AMRDocument(propbankpath);
            var str = File.ReadAllText(path);
            var amrelement = XElement.Parse(str);
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
    }
}
