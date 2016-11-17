using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis
{
    public class Sentence
    {
        public int Id;
        public LinkedList<Token> Tokens = new LinkedList<Token>();
    }
}
