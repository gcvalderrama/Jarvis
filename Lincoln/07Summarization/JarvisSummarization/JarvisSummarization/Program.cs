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
        private static string RSTSummaryDir = @"D:\Tesis2016\Jarvis\Lincoln\03RST\Summaries\";
        private static string AMRDir = @"D:/Tesis2016/Jarvis/Lincoln/05AMRParsing/Output/";
        private static string AMRSanityDir = @"D:/Tesis2016/Jarvis/Lincoln/05AMRParsing/Sanity/";
        private static string ConceptualGraphDir = @"D:/Tesis2016/Jarvis/Lincoln/07ConceptualGraph/Summaries";
        private static string ConceptualGraphNoRSTDir = @"D:/Tesis2016/Jarvis/Lincoln/07ConceptualGraph/SummariesNonRST";
        private static string ConceptualLogGraphDir = @"D:/Tesis2016/Jarvis/Lincoln/07ConceptualGraph/Logs";
        private static string ConceptualLogGraphNoRSTDir = @"D:/Tesis2016/Jarvis/Lincoln/07ConceptualGraph/LogsNoRst";
        static void Main(string[] args)
        {

            if (!Directory.Exists(RSTSanityDir)) Directory.CreateDirectory(RSTSanityDir);
            if (!Directory.Exists(AMRSanityDir)) Directory.CreateDirectory(AMRSanityDir);
            if (!Directory.Exists(RSTSummaryDir)) Directory.CreateDirectory(RSTSummaryDir);
            if (!Directory.Exists(ConceptualGraphDir)) Directory.CreateDirectory(ConceptualGraphDir);
            if (!Directory.Exists(ConceptualLogGraphDir)) Directory.CreateDirectory(ConceptualLogGraphDir);
            if (!Directory.Exists(ConceptualGraphNoRSTDir)) Directory.CreateDirectory(ConceptualGraphNoRSTDir);
            if (!Directory.Exists(ConceptualLogGraphNoRSTDir)) Directory.CreateDirectory(ConceptualLogGraphNoRSTDir);
            

            NEO.NEOManager manager = new NEO.NEOManager();
            
            var files = Directory.GetFiles(InputDir, "*.txt");
            bool neo = false;
            if(neo) manager.DeleteAllNodes();
            foreach (var file in files)
            {
                Console.WriteLine("============================File with " + Path.GetFileNameWithoutExtension(file));
                POS.POSReader posreader = new POS.POSReader();
                var document = posreader.Load(Path.Combine(SintacticDir, Path.GetFileNameWithoutExtension(file) + ".xml"));

                if ( neo ) manager.SaveDocument(document);

                var sanity = SanityXml.Sanity(File.ReadAllText(Path.Combine(RSTDir, Path.GetFileNameWithoutExtension(file) + ".txt.xml.jarvis")));
                File.WriteAllText(Path.Combine(RSTSanityDir, Path.GetFileNameWithoutExtension(file) + ".txt.xml.jarvis"), sanity);
                               


                RST.RSTReader rstreader = new RST.RSTReader();
                var rstdocument = rstreader.ReadDocument(Path.Combine(RSTSanityDir, Path.GetFileNameWithoutExtension(file) + ".txt.xml.jarvis"), Path.GetFileNameWithoutExtension(file));
                rstdocument.EvaluateODonell(false);
                File.WriteAllText(Path.Combine(RSTSummaryDir, Path.GetFileNameWithoutExtension(file) + ".txt"), rstdocument.SummaryLemma());

                if (neo) manager.DeleteAllRST();
                if (neo) manager.SaveDocumentRST(rstdocument);
                                
                var rstdummydocument = rstreader.ReadDocument(Path.Combine(RSTSanityDir, Path.GetFileNameWithoutExtension(file) + ".txt.xml.jarvis"), Path.GetFileNameWithoutExtension(file));
                rstdummydocument.EvaluateODonell(true);
                
                var sanityAMR = SanityXml.Sanity(File.ReadAllText(Path.Combine(AMRDir, Path.GetFileNameWithoutExtension(file) + ".txt.all.basic-abt-brown-verb.parsed")));
                File.WriteAllText(Path.Combine(AMRSanityDir, Path.GetFileNameWithoutExtension(file) + ".txt.all.basic-abt-brown-verb.parsed"), sanityAMR);

                var reader = new AMR.AMRReader();                
                var amrdoc = reader.ReadXML(Path.Combine(AMRSanityDir, Path.GetFileNameWithoutExtension(file)+ ".txt.all.basic-abt-brown-verb.parsed"));
                amrdoc.LoadRSTInformation(rstdocument);

                var amrnorstdoc = reader.ReadXML(Path.Combine(AMRSanityDir, Path.GetFileNameWithoutExtension(file) + ".txt.all.basic-abt-brown-verb.parsed"));

                if (neo) manager.DeleteAllAMR();
                if (neo) manager.SaveAMR(amrdoc);

                CGGraph cgraph = new CGGraph(Path.GetFileNameWithoutExtension(file), @"D:\Tesis2016\Propbank\frames", document.NumberOfWords);
                cgraph.ReadAMR(amrdoc);
                cgraph.Digest();

                if (neo) manager.DeleteAllCG();
                if (neo) manager.SaveCG(cgraph);               
                
                File.WriteAllText(Path.Combine(ConceptualGraphDir, Path.GetFileNameWithoutExtension(file) + ".txt"), cgraph.Summary());
                File.WriteAllText(Path.Combine(ConceptualLogGraphDir, Path.GetFileNameWithoutExtension(file) + ".txt"), cgraph.GenerateInformativeAspectsv2());

                CGGraph cgraphnorst = new CGGraph(Path.GetFileNameWithoutExtension(file), @"D:\Tesis2016\Propbank\frames", document.NumberOfWords);
                cgraphnorst.ReadAMR(amrnorstdoc);
                cgraphnorst.Digest();
                File.WriteAllText(Path.Combine(ConceptualGraphNoRSTDir, Path.GetFileNameWithoutExtension(file) + ".txt"), cgraphnorst.Summary());
                File.WriteAllText(Path.Combine(ConceptualLogGraphNoRSTDir, Path.GetFileNameWithoutExtension(file) + ".txt"), cgraphnorst.GenerateInformativeAspectsv2());
                
                Console.WriteLine("========================================================================");
            }
            Console.ReadLine(); 

        }
    }
}
