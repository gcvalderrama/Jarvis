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
        private static string ManualSummaryDir = @"D:\Tesis2016\Jarvis\Lincoln\LAB\ManualSummaries";
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


        public static void RSTSummary(string InputDir, string RSTInputDir, string RSTSummaryDir)
        {
            var files = Directory.GetFiles(InputDir, "*.txt");            
            foreach (var file in files)
            {
                var rstfile = Path.Combine(RSTInputDir, Path.GetFileNameWithoutExtension(file) + ".txt.xml.jarvis");
                
                if (File.Exists(rstfile))
                {                    
                    var sanity = SanityXml.Sanity(File.ReadAllText(rstfile));
                    RST.RSTReader rstreader = new RST.RSTReader();
                    var rstdocument = rstreader.ReadDocumentContent(sanity, Path.GetFileNameWithoutExtension(file));
                    rstdocument.EvaluateODonell(false);
                    File.WriteAllText(Path.Combine(RSTSummaryDir, Path.GetFileNameWithoutExtension(file) + ".txt"), rstdocument.SummaryLemma());                    
                }                
                else
                {
                    Console.WriteLine("not found" + file);
                }
                var manualsummary = Path.Combine(ManualSummaryDir, Path.GetFileNameWithoutExtension(file) + ".txt");
                if (!File.Exists(manualsummary))
                {
                    Console.WriteLine("manual summary not found" + manualsummary);
                }
            }
            Console.ReadLine();
        }

        public static void ConceptualSummary(string InputDir, string AmrDir, string OutputDir)
        {
            var files = Directory.GetFiles(InputDir, "*.txt");
            foreach (var file in files)
            {

                var amrfile = Path.Combine(AmrDir, Path.GetFileNameWithoutExtension(file) + ".txt.all.basic-abt-brown-verb.parsed");

                if (File.Exists(amrfile))
                {
                    var sanityAMR = SanityXml.Sanity(File.ReadAllText(amrfile));
                    var reader = new AMR.AMRReader();
                    var amrdoc = reader.ReadContent(sanityAMR);
                    amrdoc.LoadDummyRSTInformation();                    
                    CGGraph cgraph = new CGGraph(Path.GetFileNameWithoutExtension(file), @"D:\Tesis2016\Propbank\frames", amrdoc.Graphs.Sum(c => c.Nodes.Count));
                    cgraph.ReadAMR(amrdoc);
                    cgraph.Digest();
                    File.WriteAllText(Path.Combine(OutputDir, Path.GetFileNameWithoutExtension(file) + ".txt"), cgraph.Summary());
                }
                
            }
            Console.ReadLine();
        }
        public static void ConceptualRSTSummary(string InputDir, string RSTDir2, string AmrDir, string ConceptualOutputDir,  string OutputDir)
        {
            var files = Directory.GetFiles(InputDir, "*.txt");
            foreach (var file in files)
            {
                var amrfile = Path.Combine(AmrDir, Path.GetFileNameWithoutExtension(file) + ".txt.all.basic-abt-brown-verb.parsed");

                if (File.Exists(amrfile))
                {
                    var sanity = SanityXml.Sanity(File.ReadAllText(Path.Combine(RSTDir2, Path.GetFileNameWithoutExtension(file) + ".txt.xml.jarvis")));

                    RST.RSTReader rstreader = new RST.RSTReader();
                    var rstdocument = rstreader.ReadDocumentContent(sanity, Path.GetFileNameWithoutExtension(file));
                    rstdocument.EvaluateODonell(false);
                    var sanityAMR = SanityXml.Sanity(File.ReadAllText(amrfile));
                    var reader = new AMR.AMRReader();
                    var amrdoc = reader.ReadContent(sanityAMR);
                    amrdoc.LoadRSTInformation(rstdocument);
                    CGGraph cgraph = new CGGraph(Path.GetFileNameWithoutExtension(file), @"D:\Tesis2016\Propbank\frames", amrdoc.Graphs.Sum(c => c.Nodes.Count));
                    cgraph.ReadAMR(amrdoc);
                    cgraph.Digest();

                    if (!string.IsNullOrWhiteSpace(ConceptualOutputDir))
                    {
                        File.WriteAllText(Path.Combine(ConceptualOutputDir, Path.GetFileNameWithoutExtension(file) + ".json"), cgraph.GenerateJSON());                        
                    }
                    File.WriteAllText(Path.Combine(OutputDir, Path.GetFileNameWithoutExtension(file) + ".txt"), cgraph.Summary());
                }
            }
            Console.ReadLine();
        }


        public static void ConceptualRSTNLG(string InputDir, string AmrDir, string OutputDir)
        {
            var files = Directory.GetFiles(InputDir, "*.txt");
            foreach (var file in files)
            {
                var amrfile = Path.Combine(AmrDir, Path.GetFileNameWithoutExtension(file) + ".txt.all.basic-abt-brown-verb.parsed");
                if (File.Exists(amrfile))
                {
                    var sanity = SanityXml.Sanity(File.ReadAllText(Path.Combine(RSTDir, Path.GetFileNameWithoutExtension(file) + ".txt.xml.jarvis")));

                    RST.RSTReader rstreader = new RST.RSTReader();
                    var rstdocument = rstreader.ReadDocumentContent(sanity, Path.GetFileNameWithoutExtension(file));
                    rstdocument.EvaluateODonell(false);
                    var sanityAMR = SanityXml.Sanity(File.ReadAllText(amrfile));
                    var reader = new AMR.AMRReader();
                    var amrdoc = reader.ReadContent(sanityAMR);
                    amrdoc.LoadRSTInformation(rstdocument);
                    CGGraph cgraph = new CGGraph(Path.GetFileNameWithoutExtension(file), @"D:\Tesis2016\Propbank\frames", amrdoc.Graphs.Sum(c => c.Nodes.Count));
                    cgraph.ReadAMR(amrdoc);
                    cgraph.Digest();
                    File.WriteAllText(Path.Combine(OutputDir, Path.GetFileNameWithoutExtension(file) + ".txt"), cgraph.GenerateMetadataNLG());
                }
            }
            Console.ReadLine();
        }

        public static void All()
        {
            NEO.NEOManager manager = new NEO.NEOManager();

            var files = Directory.GetFiles(InputDir, "*.txt");
            bool neo = false;
            if (neo) manager.DeleteAllNodes();
            foreach (var file in files)
            {
                Console.WriteLine("============================File with " + Path.GetFileNameWithoutExtension(file));
                POS.POSReader posreader = new POS.POSReader();
                var document = posreader.Load(Path.Combine(SintacticDir, Path.GetFileNameWithoutExtension(file) + ".xml"));

                if (neo) manager.SaveDocument(document);

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
                var amrdoc = reader.ReadXML(Path.Combine(AMRSanityDir, Path.GetFileNameWithoutExtension(file) + ".txt.all.basic-abt-brown-verb.parsed"));
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
        static void ClearDirectory(string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var files = Directory.GetFiles(dir);
            foreach (var item in files)
            {
                File.Delete(item);
            }
        }
        
        static void Main(string[] args)
        {           

            //rst summaries
            var rstSummariesDir = @"D:\Tesis2016\Jarvis\Lincoln\LAB\RSTSummaries";
            string RstInputDIr = @"D:\Tesis2016\Jarvis\Lincoln\03RST\Input\";
            //ClearDirectory(rstSummariesDir); 
            //RSTSummary(ManualSummaryDir, RstInputDIr, rstSummariesDir);
                        
            var amrInputDir = @"D:\Tesis2016\Jarvis\Lincoln\05AMRParsing\OutputClean2";
            var ConceptualSummaryDir = @"D:\Tesis2016\Jarvis\Lincoln\LAB\ConceptualSummariesClean";
            //ClearDirectory(ConceptualSummaryDir);
            //ConceptualSummary(ManualSummaryDir, amrInputDir, ConceptualSummaryDir);

            //var ConceptualSummaryDir2 = @"D:\Tesis2016\Jarvis\Lincoln\LAB\ConceptualSummariesCleanRST";
            //ClearDirectory(ConceptualSummaryDir2);
            //ConceptualRSTSummary(@"D:\Tesis2016\Jarvis\Final\Training\09 Manual Summaries",
            //    @"D:\Tesis2016\Jarvis\Final\Training\04 RST No Document Expantion",
            //    @"D:\Tesis2016\Jarvis\Final\Training\06 AMR XML No Document Expantion", 
            //    ConceptualSummaryDir2);


            var ConceptualSummaryDir2 = @"D:\Tesis2016\Jarvis\Lincoln\LAB\ConceptualSummariesNONERV2";
            var ConceptualSummaryJSONDir = @"D:\Tesis2016\Jarvis\Lincoln\LAB\ConceptualSummariesJSONNONERV2";
            ClearDirectory(ConceptualSummaryDir2);
            ClearDirectory(ConceptualSummaryJSONDir);
            ConceptualRSTSummary(@"D:\Tesis2016\Jarvis\Final\Training\09 Manual Summaries",
                @"D:\Tesis2016\Jarvis\Final\Training\04 RSTNOEXPANTIONV2",
                @"D:\Tesis2016\Jarvis\Final\Training\05 AMRNOEXPANTIONXMLV2",
                ConceptualSummaryJSONDir,
                ConceptualSummaryDir2);



            var AMRRSTInputDir = @"D:\Tesis2016\Jarvis\Lincoln\05AMRParsing\Output2";
            var AMRRSTOutputDir = @"D:\Tesis2016\Jarvis\Lincoln\LAB\AMRRSTSummaries65V70";
            //ClearDirectory(AMRRSTOutputDir);
            //ConceptualRSTSummary(ManualSummaryDir, AMRRSTInputDir, AMRRSTOutputDir);



            var AMRRSTInputNLGDir = @"D:\Tesis2016\Jarvis\Lincoln\05AMRParsing\Output2";
            var AMRRSTOutputNLGDir = @"D:\Tesis2016\Jarvis\Lincoln\LAB\NLG";
            //ClearDirectory(AMRRSTOutputNLGDir);
            //ConceptualRSTNLG(ManualSummaryDir, AMRRSTInputNLGDir, AMRRSTOutputNLGDir);
        }
    }
}
