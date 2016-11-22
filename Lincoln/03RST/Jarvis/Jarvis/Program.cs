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
        public static IEnumerable<string> generateEDUS(XElement root)
        {
            List<string> punc_special = new List<string>() { ",", ".", ";", "?", "%" };
            List<string> numeric_special = new List<string>() { "$" };
            var query = from c in root.Elements("tokens").Elements("token")
                        select c;
            int index = 1;         
            List<LinkedList<string>> bag = new List<LinkedList<string>>();
            LinkedList<string> iter = new LinkedList<string>(); 
            foreach (var token in query)
            {
                var eduid = int.Parse(token.Attribute("eduidx").Value);
                if (index != eduid)
                {
                    bag.Add(iter);
                    iter = new LinkedList<string>();
                    index++;
                }
                iter.AddLast(token.Attribute("word").Value);
            }
            bag.Add(iter);

            List<string> result = new List<string>();
            
            foreach (var item in bag)
            {
                var sb = new StringBuilder();
                for (LinkedListNode<string> it = item.First; it != null;)
                {
                    var past = it.Previous;
                    var next = it.Next;

                    var word = it.Value;

                    if (word == "-LRB-")
                        word = "(";
                    if (word == "-RRB-")
                        word = ")";

                    if (next != null && punc_special.Contains(next.Value))
                    {
                        sb.Append(word);
                    }
                    else if (numeric_special.Contains(word))
                    {
                        sb.Append(word);
                    }
                    else if (next != null && next.Value.StartsWith("'"))
                    {
                        sb.Append(word);
                    }
                    else
                    {
                        sb.Append(word + " ");
                    }
                    it = next;
                }

                result.Add(sb.ToString());
            }
            return result; 
            
        }

        static void Main(string[] args)
        {
            var outputpath = @"D:\Tesis2016\Jarvis\Lincoln\03RST\Output";
            var files = Directory.GetFiles(outputpath, " *.txt");
            foreach (var item in files)
            {
                File.Delete(item);
            }
            files = Directory.GetFiles(@"D:\Tesis2016\Jarvis\Lincoln\03RST\Input", "*.jarvis");
            foreach (var item in files)
            {
                var root = XElement.Load(item);
                var result =  generateEDUS(root);
                File.WriteAllLines(Path.Combine(outputpath, Path.GetFileNameWithoutExtension(item)) + ".txt", 
                    result.ToArray());
            }
        }
    }
}
