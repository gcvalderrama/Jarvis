using JarvisSummarization.CG;
using JarvisSummarization.PageRank;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization
{
    class Program
    {
        static void Main(string[] args)
        {

            NEO.NEOManager manager = new NEO.NEOManager();
            //manager.DeleteAllNodes();

            POS.POSReader posreader = new POS.POSReader();
            var inputpath = @"D:\Tesis2016\Jarvis\Lincoln\02SintacticAnalysis\Output\WSJ900402-0112.xml";
            //var inputpath = @"D:\Tesis2016\Jarvis\Lincoln\02SintacticAnalysis\Output\lincon.xml";
            
            var document = posreader.Load(inputpath);
            //manager.SaveDocument(document);

            RST.RSTReader rstreader = new RST.RSTReader();

            //var rstdocument =  rstreader.ReadDocument(@"D:\Tesis2016\Jarvis\Lincoln\03RST\Input\lincon.txt.xml.jarvis", Path.GetFileNameWithoutExtension(inputpath));

            var rstdocument = rstreader.ReadDocument(@"D:\Tesis2016\Jarvis\Lincoln\03RST\Input\WSJ9004020112.txt.xml.jarvis", Path.GetFileNameWithoutExtension(inputpath));

            rstdocument.EvaluateODonell();

            

            Console.WriteLine( rstdocument.Summarize());

            //manager.DeleteAllRST();
            //manager.SaveDocumentRST(rstdocument);


            var reader = new AMR.AMRReader();            
            var amrdoc = reader.ReadXML(@"D:/Tesis2016/Jarvis/Lincoln/05AMRParsing/Output/amr-graph.xml");
                        
            amrdoc.LoadRSTInformation(rstdocument);            

            //manager.DeleteAllAMR();
            //manager.SaveAMR(amrdoc);

            CGGraph cgraph = new CGGraph("lincon", @"D:\Tesis2016\Propbank\frames", document.NumberOfWords);
            cgraph.ReadAMR(amrdoc);
            cgraph.Digest();
            cgraph.Validate();
            //manager.DeleteAllCG();
            //manager.SaveCG(cgraph);


            Console.WriteLine(cgraph.Stadistics());

            cgraph.GenerateInformativeAspectsv2();


            var connectors = new List<CGRelation>();
            foreach (var verb in cgraph.Nodes.Where(c=>c.semanticroles.Contains("verb")))
            {
                var relations = cgraph.Relations.Where(c => c.Head == verb.id || c.Tail == verb.id);
                connectors.AddRange(relations); 
            }

            //foreach (var item in connectors.Select(c=>c.conceptualrole).Distinct())
            //{
            //    //var head = cgraph.Nodes.Where(c => c.id == item.Head).Single(); 

            //    Console.WriteLine(item);
                

            //}
            
            //cgraph.GenerateInformation();
            
            



            //RSTReader reader = new RSTReader();
            //var tree =  reader.ReadRSTTree();
            //tree.Reduce();
            //Console.WriteLine(tree.Root.text);

            Console.ReadLine(); 

        }
    }
}
