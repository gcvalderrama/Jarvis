using JarvisSummarization.CG;
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
            var inputpath = @"D:\Tesis2016\Jarvis\Lincoln\02POS\Output\lincon.xml";
            //var document = posreader.Load(inputpath);
            //manager.SaveDocument(document);

            RST.RSTReader rstreader = new RST.RSTReader();

            var rstdocument =  
                rstreader.ReadDocument(@"D:\Tesis2016\Jarvis\Lincoln\03RST\Input\lincon.txt.xml.jarvis", 
                Path.GetFileNameWithoutExtension(inputpath));

            rstdocument.EvaluateODonell();

            ////Console.WriteLine( rstdocument.Summarize());

            //manager.DeleteAllRST();
            //manager.SaveDocumentRST(rstdocument);


            var reader = new AMR.AMRReader();
            
            var amrdoc = reader.ReadXML(@"D:\Tesis2016\Jarvis\Lincoln\05AMRParsing\Output\amr-graph.xml",
                @"D:\Tesis2016\Propbank\frames");
                        
            amrdoc.LoadRSTInformation(rstdocument);

            amrdoc.Digest(); 

            //manager.DeleteAllAMR();

            //manager.SaveAMR(amrdoc);

            CGGraph cgraph = new CGGraph("lincon");
            cgraph.ReadAMR(amrdoc);
            cgraph.Digest();

            manager.DeleteAllCG(); 
            manager.SaveCG(cgraph); 

            //RSTReader reader = new RSTReader();

            //var tree =  reader.ReadRSTTree();

            //tree.Reduce();


            //Console.WriteLine(tree.Root.text);

            Console.ReadLine(); 

        }
    }
}
