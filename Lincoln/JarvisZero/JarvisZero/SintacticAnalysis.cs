using edu.stanford.nlp.pipeline;
using java.io;
using java.util;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisZero
{
    public class SintacticAnalysis
    {
        public Annotation ReadAnnotation(string Path)
        {
            var str =  System.IO.File.ReadAllText(Path);
            return new Annotation(str);  
        }
        public void demo()
        {

        }
        public void Analysis(string path)
        {






            var document = this.ReadAnnotation(path);
            

            Properties props = new Properties();
            //props.setProperty("annotators", "tokenize,ssplit,pos,lemma,ner,parse,relation,,mention,coref");


            
            //var modelsDirectory = jarRoot ;

            // Loading POS Tagger

            //String modPath = @"D:\Tesis2016\Jarvis\Lincoln\Models\";
            //props.put("pos.model", modPath + @"pos-tagger\english-bidirectional-distsim.tagger");
            //props.put("ner.model", modPath + "ner/english.all.3class.distsim.crf.ser.gz");
            //props.put("parse.model", modPath + "lexparser/englishPCFG.ser.gz");
            //props.put("annotators", "tokenize, ssplit, pos, lemma, ner, parse, dcoref");
            //props.put("sutime.binders", "0");
            //props.put("sutime.rules", modPath + "sutime/defs.sutime.txt, " + modPath + "sutime/english.sutime.txt");
            //props.put("dcoref.demonym", modPath + "dcoref/demonyms.txt");
            //props.put("dcoref.states", modPath + "dcoref/state-abbreviations.txt");
            //props.put("dcoref.animate", modPath + "dcoref/animate.unigrams.txt");
            //props.put("dcoref.inanimate", modPath + "dcoref/inanimate.unigrams.txt");
            //props.put("dcoref.big.gender.number", modPath + "dcoref/gender.data.gz");
            //props.put("dcoref.countries", modPath + "dcoref/countries");
            //props.put("dcoref.states.provinces", modPath + "dcoref/statesandprovinces");
            //props.put("dcoref.singleton.model", modPath + "dcoref/singleton.predictor.ser");
            //props.put("ner.useSUTime", "0");

            props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, parse, dcoref");
            props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, parse, dcoref");
            props.setProperty("sutime.binders", "0");                        
            props.setProperty("ner.useSUTime", "false");

            var jarRoot = @"D:\Tesis2016\Jarvis\Lincoln\Models";
            var curDir = Environment.CurrentDirectory;
            System.IO.Directory.SetCurrentDirectory(jarRoot);
            var pipeline = new StanfordCoreNLP(props);            
            pipeline.annotate(document);
            System.IO.Directory.SetCurrentDirectory(curDir);
            FileOutputStream os = new FileOutputStream(new File("coreference_output.xml"));
            pipeline.xmlPrint(document, os);
        }
    }
}
