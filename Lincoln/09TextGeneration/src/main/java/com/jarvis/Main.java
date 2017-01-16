package com.jarvis;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import simplenlg.framework.*;
import simplenlg.lexicon.*;
import simplenlg.realiser.english.*;
import simplenlg.phrasespec.*;
import simplenlg.features.*;
import sun.misc.IOUtils;

import java.io.*;
import java.lang.reflect.Type;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;
import java.util.logging.ConsoleHandler;


public class Main {
    public class CGNode
    {
        public int id;

        public String text ;
        public String label ;
        public String nosuffix ;
        public boolean IsConcept ;
        public boolean IsDataEntity ;
        public boolean IsTemporalQuantity;
        public boolean IsMonetaryQuantity ;
        public double rstweight ;
        public double pagerank ;
        public int  sentenceid ;
        public String Entity ;
        public String description;
    }
    public class CGRelation
    {
        public int Head ;
        public int Tail ;
        public String label ;
        public String vncls ;
        public String vntheta ;
        public String description;
        public String f;
        public String log ;
        public int ocurrences ;
        public String conceptualrole ;
    }
    public class CGVerbTerm
    {
        public CGNode Node ;
        public CGRelation Relation ;
        public int Level ;
    }
    public class CGVerb
    {
        public CGNode Verb;
        public ArrayList<CGVerbTerm> VerbAttributes;
        public ArrayList<ArrayList<CGVerbTerm>> Agents;
        public ArrayList<ArrayList<CGVerbTerm>> Patients;
        public ArrayList<ArrayList<CGVerbTerm>> Themes;
        public ArrayList<ArrayList<CGVerbTerm>> Goal;
        public ArrayList<ArrayList<CGVerbTerm>> Attributes;
    }

    public static void main(String[] args) throws IOException {

        File folder = new File("D:/Tesis2016/Jarvis/Lincoln/LAB/NLG");
        File[] listOfFiles = folder.listFiles();

        Lexicon lexicon = Lexicon.getDefaultLexicon();
        NLGFactory nlgFactory = new NLGFactory(lexicon);
        Realiser realiser = new Realiser(lexicon);

        for (File file : listOfFiles) {
            if (file.isFile()) {


                String content;
                Path t = Paths.get(file.getAbsoluteFile().toURI());
                content = new String(Files.readAllBytes(t));

                Gson g = new Gson();


                Type listType = new TypeToken<ArrayList<CGVerb>>(){}.getType();

                ArrayList<CGVerb> terms = g.fromJson(content, listType);
                CoordinatedPhraseElement c = nlgFactory.createCoordinatedPhrase();


                StringBuilder sb = new StringBuilder();

                for ( CGVerb verb :terms ) {

                    SPhraseSpec p = nlgFactory.createClause();
                    p.setFeature(Feature.TENSE, Tense.PAST);

                    for (ArrayList<CGVerbTerm> tmpArray: verb.Agents ) {
                        for (CGVerbTerm tmp: tmpArray ) {
                            p.setSubject(tmp.Node.nosuffix);
                        }
                    }

                    p.setVerb( verb.Verb.nosuffix);

                    for (ArrayList<CGVerbTerm> tmpArray: verb.Patients ) {
                        for (CGVerbTerm tmp: tmpArray ) {
                            p.setObject(tmp.Node.nosuffix);
                        }
                    }
                    for (ArrayList<CGVerbTerm> tmpArray: verb.Themes ) {
                        p.setObject("about");
                        for (CGVerbTerm tmp: tmpArray ) {
                            p.setObject(tmp.Node.nosuffix);
                        }
                    }
                    for (ArrayList<CGVerbTerm> tmpArray: verb.Goal ) {
                        p.setObject("target");
                        for (CGVerbTerm tmp: tmpArray ) {
                            p.setObject(tmp.Node.nosuffix);
                        }
                    }
                    for (ArrayList<CGVerbTerm> tmpArray: verb.Attributes ) {
                        p.setObject("with");
                        for (CGVerbTerm tmp: tmpArray ) {
                            p.setObject(tmp.Node.nosuffix);
                        }
                    }
                    c.addCoordinate(p);
                    String output =  realiser.realiseSentence(p);
                    sb.append(output);
                    sb.append(System.getProperty("line.separator"));
                }
                //String output =  realiser.realiseSentence(c);
                //sb.append(output);
                String ta = "D:/Tesis2016/Jarvis/Lincoln/LAB/NLGSummarySentence/" + file.getName();
                System.out.println(ta);
                Writer writer = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(ta), "utf-8"));
                writer.write(sb.toString());
                writer.close();
                //Files.write(file, lines, Charset.forName("UTF-8"), StandardOpenOption.APPEND);
            }
        }
/*


        //String output = realiser.realiseSentence(s1);

        SPhraseSpec p = nlgFactory.createClause();
        p.setFeature(Feature.TENSE, Tense.PRESENT);
        p.setSubject("father");
        p.setVerb("dedicate");
        p.setObject("thing");
        String output =  realiser.realiseSentence(p);
        System.out.println(output);


        p = nlgFactory.createClause();
        p.setFeature(Feature.TENSE, Tense.PRESENT);
        NPPhraseSpec subject = nlgFactory.createNounPhrase("father");
        VPPhraseSpec verb = nlgFactory.createVerbPhrase("bring");
        AdjPhraseSpec adj = nlgFactory.createAdjectivePhrase("nation");
        p.setSubject(subject);
        p.setVerb(verb);
        p.setObject(adj);
        p.addComplement("continent");
        output =  realiser.realiseSentence(p);
        System.out.println(output);
*/
        //Father dedicates thing.
        //Father brings nation for this continent.

    }
}

