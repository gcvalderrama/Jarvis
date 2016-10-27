using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis
{
    public class Token
    {
        public int Id { get; set; }
        public int SentenceLoc { get; set; }
        public string Word { get; set; }
        public int CharacterOffsetBegin { get; set; }
        public int CharacterOffsetEnd { get; set; }
        public string POS { get; set; }
    }
}
