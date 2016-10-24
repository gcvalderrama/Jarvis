using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace Jarvis
{
    class Program
    {
        private static string PathRoot = @"D:\Tesis2016\Jarvis\Lincoln\03RST\Output";
        public static void generateEDUS(XElement root)
        {

            var files = Directory.GetFiles(PathRoot, "*.txt");
            foreach (var item in files)
            {
                File.Delete(item); 
            }

            var query = from c in root.Elements("tokens").Elements("token")
                        select c;
            int index = 1;
            StringBuilder sb = new StringBuilder(); 
            foreach (var token in query)
            {
                var eduid = int.Parse(token.Attribute("eduidx").Value);
                if (index != eduid)
                {
                    File.WriteAllText(Path.Combine(PathRoot, string.Format("{0}_edu.txt", index)), sb.ToString());
                    sb = new StringBuilder();
                    index++;
                }
                sb.Append(token.Attribute("word").Value + " ");
            }
            File.WriteAllText(Path.Combine(PathRoot, string.Format("{0}_edu.txt", index )), sb.ToString());
            sb = new StringBuilder();
        }

        static void Main(string[] args)
        {
            var root = XElement.Load(@"D:\Tesis2016\Jarvis\Lincoln\03RST\Output\rst.xml");
            generateEDUS(root);
        }
    }
}
