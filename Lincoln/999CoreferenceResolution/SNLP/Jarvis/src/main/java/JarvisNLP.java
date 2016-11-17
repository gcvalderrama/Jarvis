import edu.stanford.nlp.hcoref.CorefCoreAnnotations;
import edu.stanford.nlp.hcoref.data.CorefChain;
import edu.stanford.nlp.hcoref.data.Mention;
import edu.stanford.nlp.ling.CoreAnnotations;
import edu.stanford.nlp.pipeline.Annotation;
import edu.stanford.nlp.pipeline.StanfordCoreNLP;
import edu.stanford.nlp.util.CoreMap;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Properties;

/**
 * Created by greg on 10/19/2016.
 */
public class JarvisNLP{

    public List<String> GetSentences(Annotation document)
    {
        List<String> result = new ArrayList<String>();
        Properties props = new Properties();
        props.setProperty("annotators", "tokenize, ssplit");
        StanfordCoreNLP pipeline = new StanfordCoreNLP(props);
        pipeline.annotate(document);
        List<CoreMap> sentences = document.get(CoreAnnotations.SentencesAnnotation.class);
        for(CoreMap sentence: sentences) {
            String item = sentence.toString();
            result.add(item);
        }
        return result;
    }

    public void CoreferenceResolution(Annotation document) throws IOException {
        //http://stanfordnlp.github.io/CoreNLP/coref.html
        //java -Xmx5g -cp stanford-corenlp-3.6.0.jar:stanford-corenlp-models-3.6.0.jar:* edu.stanford.nlp.pipeline.StanfordCoreNLP
        // -annotators tokenize,ssplit,pos,lemma,ner,parse,mention,coref -file example_file.txt
        //props.setProperty("annotators", "tokenize,ssplit,pos,lemma,ner,parse,relation,sentiment,mention,dcoref");
        Properties props = new Properties();
        props.setProperty("annotators", "tokenize,ssplit,pos,lemma,ner,parse,relation,,mention,coref");
        StanfordCoreNLP pipeline = new StanfordCoreNLP(props);
        pipeline.annotate(document);
        FileOutputStream os = new FileOutputStream(new File("coreference_output.xml"));
        pipeline.xmlPrint(document, os);
        /*
        System.out.println("---");
        System.out.println("coref chains");
        Collection<CorefChain> values= document.get(CorefCoreAnnotations.CorefChainAnnotation.class).values();
        for (CorefChain cc : values) {
            System.out.println("\t"+cc);
        }
        for (CoreMap sentence : document.get(CoreAnnotations.SentencesAnnotation.class)) {
            System.out.println("---");
            System.out.println("mentions");
            for (Mention m : sentence.get(CorefCoreAnnotations.CorefMentionsAnnotation.class)) {
                int start = m.startIndex;
                int end = m.endIndex;
                String relation =  m.getRelation();
                String mention = m.mentionType.toString();
                String person = m.person.toString();

                String headString =  m.headString;
                String headWord =  m.headWord.toString();

                System.out.println("\t"+ String.format("%s %s %s %s %s %s %s", start   , end   , relation, mention,
                        person, headString, headWord));
            }
        }
        */
    }

    public void ApplyPipeline(Annotation document)
    {

        Properties props = new Properties();
//        props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, parse, dcoref");
        props.setProperty("annotators", "tokenize, ssplit, pos, lemma, parse");
        //props.setProperty("parse.model", "en");
        //props.setProperty("tokenize.language", "en");

        StanfordCoreNLP pipeline = new StanfordCoreNLP(props);
        pipeline.annotate(document);
    }

}
