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
        private static string InputDir = @"D:\Tesis2016\Jarvis\Lincoln\00Input\Input";
        private static string SintacticDir = @"D:\Tesis2016\Jarvis\Lincoln\02SintacticAnalysis\Output\";
        private static string RSTDir = @"D:\Tesis2016\Jarvis\Lincoln\03RST\Input\";
        private static string RSTSanityDir = @"D:\Tesis2016\Jarvis\Lincoln\03RST\Sanity\";
        private static string AMRDir = @"D:/Tesis2016/Jarvis/Lincoln/05AMRParsing/Output/";
        private static string AMRSanityDir = @"D:/Tesis2016/Jarvis/Lincoln/05AMRParsing/Sanity/";
        static void Main(string[] args)
        {

            if (!Directory.Exists(RSTSanityDir)) Directory.CreateDirectory(RSTSanityDir);
            if (!Directory.Exists(AMRSanityDir)) Directory.CreateDirectory(AMRSanityDir);

            
            NEO.NEOManager manager = new NEO.NEOManager();
            //manager.DeleteAllNodes();
            var files = Directory.GetFiles(InputDir, "*.txt");
            



            foreach (var file in files)
            {
                Console.WriteLine("============================File with " + Path.GetFileNameWithoutExtension(file));
                POS.POSReader posreader = new POS.POSReader();
                var document = posreader.Load(Path.Combine(SintacticDir, Path.GetFileNameWithoutExtension(file) + ".xml"));
                //manager.SaveDocument(document);

                var sanity = SanityXml.Sanity(File.ReadAllText(Path.Combine(RSTDir, Path.GetFileNameWithoutExtension(file) + ".txt.xml.jarvis")));
                File.WriteAllText(Path.Combine(RSTSanityDir, Path.GetFileNameWithoutExtension(file) + ".txt.xml.jarvis"), sanity);

                RST.RSTReader rstreader = new RST.RSTReader();
                var rstdocument =  rstreader.ReadDocument(Path.Combine(RSTSanityDir, Path.GetFileNameWithoutExtension(file) + ".txt.xml.jarvis"), Path.GetFileNameWithoutExtension(file));
                rstdocument.EvaluateODonell();
                Console.WriteLine(rstdocument.Summarize());
                //manager.DeleteAllRST();
                //manager.SaveDocumentRST(rstdocument);

                var sanityAMR = SanityXml.Sanity(File.ReadAllText(Path.Combine(AMRDir, Path.GetFileNameWithoutExtension(file) + ".txt.all.basic-abt-brown-verb.parsed")));
                File.WriteAllText(Path.Combine(AMRSanityDir, Path.GetFileNameWithoutExtension(file) + ".txt.all.basic-abt-brown-verb.parsed"), sanityAMR);

                var reader = new AMR.AMRReader();                
                var amrdoc = reader.ReadXML(Path.Combine(AMRSanityDir, Path.GetFileNameWithoutExtension(file)+ ".txt.all.basic-abt-brown-verb.parsed"));
                amrdoc.LoadRSTInformation(rstdocument);

                //manager.DeleteAllAMR();
                //manager.SaveAMR(amrdoc);

                CGGraph cgraph = new CGGraph(Path.GetFileNameWithoutExtension(file), @"D:\Tesis2016\Propbank\frames", document.NumberOfWords);
                cgraph.ReadAMR(amrdoc);
                cgraph.Digest();
                
                //manager.DeleteAllCG();
                //manager.SaveCG(cgraph);
                Console.WriteLine(cgraph.Stadistics());
                cgraph.GenerateInformativeAspectsv2();

                Console.WriteLine("========================================================================");

            }











            Console.ReadLine(); 

        }
    }
}
