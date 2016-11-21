﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JarvisRDFToNEO4J
{
    public class RSTTree
    {
        public string document { get; set; }
        public RSTTree(string document)
        {
            this.document = document;
        }        
        #region Relation score
        public Dictionary<string, double> Relations = new Dictionary<string, double>() {

            //main importantes
            { "list", 0.8d },//Reduce-NN-list
            { "reason", 0.8d },//Reduce-SN-reason "reason", //Reduce-NS-reason "reason", //Reduce-NN-reason                               
            { "antithesis", 0.8d }, //'Reduce-NS-antithesis' Reduce-SN-antithesis
            { "cause", 0.8d }, //'Reduce-NS-cause' Reduce-NN-cause Reduce-SN-cause                
            { "concession", 0.8d }, //'Reduce-SN-concession' Reduce-NS-concession
            { "condition", 0.8d }, //Reduce-SN-condition "condition", //Reduce-NS-condition
            { "contrast", 0.8d }, //Reduce-NN-contrast
            { "disjunction", 0.8d }, //Reduce-NN-disjunction
            { "inverted", 0.8d }, //Reduce-NN-inverted                
            { "manner", 0.8d }, //Reduce-NS-manner "manner", //Reduce-SN-manner
            { "otherwise", 0.8d }, //Reduce-SN-otherwise "otherwise", //Reduce-NS-otherwise
            { "problem", 0.8d }, //'Reduce-NN-problem' "problem", //Reduce-SN-problem "problem", //Reduce-NS-problem                                
            { "purpose", 0.8d }, //Reduce-SN-purpose "purpose", //Reduce-NS-purpose
            { "question" , 0.8d}, //Reduce-SN-question "question", //Reduce-NN-question Reduce-NS-question
            { "statement", 0.8d }, //Reduce-SN-statement "statement", //Reduce-NS-statement "statement", //Reduce-NN-statement                                
            { "result", 0.8d }, //Reduce-NS-result Reduce-SN-result
            { "same_unit", 0.8d}, //Reduce-NN-same_unit                
            { "sequence", 0.8d }, //Reduce-NN-sequence
            { "topic", 0.8d }, //Reduce-NS-topic "topic", //"Reduce-NN-topic", 

            // importantes
            { "comparison", 0.6d }, //'Reduce-NS-comparison' Reduce-NN-comparison "comparison", //Reduce-SN-comparison
            { "enablement", 0.6d }, //Reduce-SN-enablement "enablement", //Reduce-NS-enablement
            { "evaluation", 0.6d } , //Reduce-NN-evaluation "evaluation", //Reduce-NS-evaluation Reduce-SN-evaluation
            { "means", 0.6d }, //Reduce-NS-means "means", //Reduce-SN-means
            { "preference", 0.6d }, //Reduce-SN-preference "preference", //Reduce-NS-preference
            //normal
            { "analogy", 0.4d }, //Reduce-SN-analogy "analogy", //Reduce-NS-analogy "analogy", //Reduce-NN-analogy                
            { "attribution", 0.4d },//Reduce-SN-attribution Reduce-NS-attribution
            { "comment", 0.4d }, //Reduce-SN-comment "comment", //Reduce-NS-comment "comment", //Reduce-NN-comment
            { "conclusion", 0.4d }, //Reduce-NS-conclusion "conclusion", //Reduce-SN-conclusion
            { "consequence", 0.4d }, //'Reduce-SN-consequence' "consequence", //Reduce-NS-consequence "consequence", //Reduce-NN-consequence
            { "contingency", 0.4d }, //Reduce-SN-contingency Reduce-NS-contingency                
            { "evidence", 0.4d }, //Reduce-NS-evidence "evidence", //Reduce-SN-evidence                
            { "explanation", 0.4d }, //Reduce-NS-explanation 'Reduce-SN-explanation'
            { "hypothetical", 0.4d }, //Reduce-NS-hypothetical "hypothetical", //Reduce-SN-hypothetical
            { "interpretation" , 0.4d}, //Reduce-SN-interpretation "interpretation", //Reduce-NS-interpretation "interpretation" //Reduce-NN-interpretation
            { "restatement", 0.4d }, //Reduce-NS-restatement "restatement", //Reduce-SN-restatement
            { "textualorganization", 0.4d }, //Reduce-NN-textualorganization                
            { "summary", 0.4d }, //Reduce-NS-summary "summary",//Reduce-SN-summary 
            { "proportion", 0.4d }, //Reduce-NN-proportion                
            { "rhetorical", 0.4d }, //Reduce-SN-rhetorical "rhetorical", //Reduce-NS-rhetorical                
            //-- importante 
            { "background", 0.2d }, //Reduce-SN-background Reduce-NS-background
            { "circumstance", 0.2d }, //Reduce-SN-circumstance "circumstance", //Reduce-NS-circumstance
            { "temporal", 0.2d },//Reduce-NS-temporal Reduce-SN-temporal "temporal", //Reduce-NN-temporal                                
            { "example", 0.2d }, //Reduce-SN-example "example", //Reduce-NS-example                
            { "definition", 0.2d }, //Reduce-NS-definition                                                
            { "elaboration", 0.2d }, //Reduce-SN-elaboration "elaboration", //Reduce-NS-elaboration                               
        };
        #endregion
        public RSTNode Root { get; set; }
        public void Load(string Path)
        {
            var reader = XElement.Load(Path);
            var tokensquery = from c in reader.Elements("tokens").Elements("token")
                              select c;
            var tokens = new List<RSTWord>();
            foreach (var item in tokensquery)
            {
                var tk = new RSTWord();
                tk.id = int.Parse(item.Attribute("id").Value);
                tk.Text = item.Attribute("word").Value;
                tk.eduid = int.Parse(item.Attribute("eduidx").Value);
                tk.sentenceid = int.Parse(item.Attribute("sidx").Value);
                tokens.Add(tk);
            }
            var query = from c in reader.Elements("rsttree").Elements("node")
                        select c;
            var treebankstr = (from c in reader.Elements("rstview") select c).First().Value;
            var input = new java.io.StringReader(treebankstr);
            var treeReader = new edu.stanford.nlp.trees.PennTreeReader(input);
            this.Root = new RSTNode();
            Root.Load(treeReader.readTree(), tokens);
        }        
        public void EvaluateODonell()
        {
            this.Root.Weight = 1;
            EvaluateODonell(this.Root);
        }
        public void EvaluateODonell(RSTNode Parent)
        {
            if (Parent.leaf)
            {
                return;
            }
            //evaluate children
            var Weight = this.Relations[Parent.relation];
            foreach (var child in Parent.Children)
            {
                if (child.type == RSTNodeType.Nucleus)
                {
                    child.Weight = Parent.Weight;
                }
                else
                {
                    child.Weight = Parent.Weight * Weight;
                }
            }
            foreach (var item in Parent.Children)
            {
                EvaluateODonell(item);
            }
        }
        private void TransformList(RSTNode node, List<RSTNode> state)
        {
            state.Add(node);
            foreach (var item in node.Children)
            {
                TransformList(item, state);                 
            }
        }
        public List<RSTNode> Summarize()
        {
            List<RSTNode> state = new List<RSTNode>();
            TransformList(this.Root, state);
            var query = (from c in state
                        where c.leaf == true
                        select c).OrderByDescending(d=>d.Weight);
            return query.ToList();
        }
    }
}