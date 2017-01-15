using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisSummarization.NLG
{
    public class NLGSentence
    {
        public List<string> Agents { get; set; }
        public List<string> Verb { get; set; }
        public List<string> Patients { get; set; }
        public List<string> Themes { get; set; }
        


    }
}
