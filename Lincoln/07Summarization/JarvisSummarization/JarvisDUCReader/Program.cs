using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisDUCReader
{
    class SanityXml
    {
        public static string Sanity(string input)
        {
            return input.Replace("&", "&amp;").Replace("%", "&#37;");
        }
        public static string Sanity(string filename, string input)
        {
            if (filename.StartsWith("FBI"))
            {
                input = input.Replace("P=100", "P='100'")
                    .Replace("P=101", "P='101'")
                    .Replace("P=102", "P='102'")
                    .Replace("P=103", "P='103'")
                    .Replace("P=104", "P='104'")
                    .Replace("P=105", "P='105'")
                    .Replace("P=106", "P='106'")
                    .Replace("P=107", "P='107'");
                return Sanity(input); 
            }
            else
                return Sanity(input); 
        }
    }
    class Program
    {
        private static string DUC_Processed_Text = @"D:\Tesis2016\DUC2001_Processed\Texts";
        private static string DUC_Processed_Summaries = @"D:\Tesis2016\DUC2001_Processed\Summaries";
        public static void ProcessTraining()
        {
            var directories = Directory.GetDirectories(@"D:\Tesis2016\DUC2001_Summarization_Documents\data\training");

            foreach (var directory in directories)
            {
                Dictionary<string, string> Files = new Dictionary<string, string>();

                var subdirs = Directory.GetDirectories(directory);
                var textDir = subdirs.Where(c => c.EndsWith("docs")).First();

                foreach (var file in Directory.GetFiles(textDir))
                {
                    var sb = new StringBuilder();
                    var file_name = Path.GetFileNameWithoutExtension(file);
                    var element = XElement.Parse(SanityXml.Sanity(file_name, File.ReadAllText(file)));
                    if (file_name.StartsWith("SJMN"))
                    {
                        var LEADPARA = (from c in element.Elements("LEADPARA")
                                        select c).First();
                        var TEXT = (from c in element.Elements("TEXT")
                                    select c).First();

                        sb.AppendLine(LEADPARA.Value);
                        sb.AppendLine(TEXT.Value);
                    }
                    else if (file_name.StartsWith("WSJ"))
                    {
                        var TEXT = (from c in element.Elements("TEXT")
                                    select c).First();
                        sb.AppendLine(TEXT.Value);
                    }
                    else if (file_name.StartsWith("FBI"))
                    {
                        var TEXT = (from c in element.Elements("TEXT")
                                    select c).First();

                        sb.AppendLine(TEXT.Value);
                    }
                    else if (file_name.StartsWith("AP"))
                    {
                        var TEXT = (from c in element.Elements("TEXT")
                                    select c);

                        foreach (var item in TEXT)
                        {
                            sb.AppendLine(item.Value);
                        }                        
                    }
                    else if (file_name.StartsWith("LA"))
                    {
                        var TEXT = (from c in element.Elements("TEXT")
                                    select c).First();

                        sb.AppendLine(TEXT.Value.Replace("<P>", "").Replace("</P>", ""));
                    }
                    else if (file_name.StartsWith("FT"))
                    {
                        var TEXT = (from c in element.Elements("TEXT")
                                    select c).First();

                        sb.AppendLine(TEXT.Value);
                    }
                    else
                    {
                        throw new Exception("no pattern");
                    }

                    Files[file_name] = file;
                    File.WriteAllText(Path.Combine(DUC_Processed_Text, file_name) + ".txt", sb.ToString().TrimStart().TrimEnd());
                }
                var sumDir = subdirs.Where(c => !c.EndsWith("docs")).First();

                foreach (var file in Directory.GetFiles(sumDir).Where(c => c.EndsWith("perdocs")))
                {
                    var str = "<roo>" + File.ReadAllText(file) + "</roo>";
                    var element = XElement.Parse(SanityXml.Sanity(str));

                    foreach (var item in Files.Keys)
                    {
                        var sum = (from c in element.Elements("SUM")
                                   where c.Attribute("DOCREF").Value == item
                                   select c).FirstOrDefault();

                        if (sum == null)
                        {
                            File.Delete(Path.Combine(DUC_Processed_Text, item));
                        }
                        else
                        {
                            File.WriteAllText(Path.Combine(DUC_Processed_Summaries, item) + ".txt", sum.Value.TrimStart().TrimEnd());
                        }
                    }                    
                }
            }
        }   
        static void Main(string[] args)
        {
            ProcessTraining();
        }
    }
}
