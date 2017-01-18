using Jarvis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateDocsFromSintacticParser
{
    public class WordHelper
    {
        public static string ProcessWord(Sentence sentence, int tokenid)
        {
            StringBuilder sb = new StringBuilder();
            Token next = null;
            var word = sentence.Tokens.ElementAt(tokenid);
            if (tokenid + 1 < sentence.Tokens.Count)
            {
                next = sentence.Tokens.ElementAt(tokenid + 1);
            }
            if (word.Word == "-LRB-")
            {
                sb.Append("(");
            }
            else if (word.Word == "-RRB-")
            {
                sb.Append(")");
            }
            else if (string.Compare(word.Word, "'s", true) == 0 && (word.POS == "VBP" || word.POS == "VBZ"))
            {
                sb.Append("is ");
            }
            else if (string.Compare(word.Word, "'re", true) == 0 && word.POS == "VBP")
            {
                sb.Append("are ");
            }
            else if (string.Compare(word.Word, "'m", true) == 0 && word.POS == "VBP")
            {
                sb.Append("am ");
            }
            else if (word.Word == "'ve" && (word.POS == "VBP" || word.POS == "VB"))
            {
                sb.Append("have ");
            }
            else if (word.Word == "'ll" && word.POS == "MD")
            {
                sb.Append("will ");
            }
            else if (word.Word == "'d" && word.POS == "MD")
            {
                sb.Append("would ");
            }

            else if (word.Word == "n't" && word.POS == "RB")
            {
                sb.Append("not ");
            }
            else if (word.Word == "'10s" || word.Word == "'20s" || word.Word == "'30s" || word.Word == "'40s" ||
                word.Word == "'50s" || word.Word == "'60s" || word.Word == "'70s" || word.Word == "'80s" ||
                word.Word == "'90s")
            {
                sb.Append(word.Word.Replace("'", "") + " ");
            }
            else if (word.Word == "." || word.Word == "`" || word.Word == "'" || word.Word == "''" || word.Word == "``"
                || word.Word == "-RRB-" || word.Word == "-LRB-" || word.Word == "--")
            {

            }
            else
            {
                if (next != null)
                {
                    if (((next.Word == "'s" || next.Word == "'") && next.POS == "POS"))
                    {
                        sb.Append(word.Word);
                    }
                    else
                    {
                        sb.Append(word.Word + " ");
                    }
                }
                else
                {
                    sb.Append(word.Word + " ");
                }
            }
            return sb.ToString();
        }
    }
}
