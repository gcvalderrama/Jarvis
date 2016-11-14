using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization
{
    class Program
    {
        static void Main(string[] args)
        {
            RSTReader reader = new RSTReader();

            var tree =  reader.ReadRSTTree();
                        
            tree.Reduce();
            

            Console.WriteLine(tree.Root.text);

            Console.ReadLine(); 

        }
    }
}
