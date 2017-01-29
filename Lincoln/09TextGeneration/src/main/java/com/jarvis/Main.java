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


        public CGVerb()
        {
            this.VerbAttributes = new ArrayList<CGVerbTerm>();
            this.Agents = new ArrayList<ArrayList<CGVerbTerm>>();
            this.Patients = new ArrayList<ArrayList<CGVerbTerm>>();
            this.Themes  = new ArrayList<ArrayList<CGVerbTerm>>();
            this.Goal = new ArrayList<ArrayList<CGVerbTerm>>();
        }

    }

    public static void SimpleNLGGenerator()  throws IOException {
        String inputDir = "D:/Tesis2016/Jarvis/Final/Test/07NLGMetadata/";

        String outputDir =  "D:/Tesis2016/Jarvis/Final/Test/08NLGSummaries/";

        File folder = new File(inputDir);
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

                    CoordinatedPhraseElement coordinateSubject = nlgFactory.createCoordinatedPhrase();

                    for (ArrayList<CGVerbTerm> tmpArray: verb.Agents ) {
                        StringBuilder sbagents = new StringBuilder();
                        for (CGVerbTerm tmp: tmpArray ) {
                            sbagents.append(tmp.Node.nosuffix + " ");
                        }
                        NPPhraseSpec subagent = nlgFactory.createNounPhrase(sbagents.toString());
                        coordinateSubject.addCoordinate(subagent);
                    }

                    p.setSubject(coordinateSubject);

                    p.setVerb( verb.Verb.nosuffix);

                    CoordinatedPhraseElement coordinateIndirectObject = nlgFactory.createCoordinatedPhrase();

                    for (ArrayList<CGVerbTerm> tmpArray: verb.Patients ) {
                        StringBuilder sbpatients = new StringBuilder();
                        for (CGVerbTerm tmp: tmpArray ) {
                            sbpatients.append(tmp.Node.nosuffix + " ");
                        }
                        NPPhraseSpec subpatient = nlgFactory.createNounPhrase(sbpatients.toString());
                        coordinateIndirectObject.addCoordinate(subpatient);
                    }

                    p.setIndirectObject(coordinateIndirectObject);

                    CoordinatedPhraseElement coordinateObject = nlgFactory.createCoordinatedPhrase();

                    for (ArrayList<CGVerbTerm> tmpArray: verb.Themes ) {
                        StringBuilder sbthemes = new StringBuilder();
                        //sbthemes.append(" related with ");
                        for (CGVerbTerm tmp: tmpArray ) {
                            sbthemes.append(tmp.Node.nosuffix + " ");
                        }
                        NPPhraseSpec subtheme = nlgFactory.createNounPhrase(sbthemes.toString());
                        coordinateObject.addCoordinate(subtheme);
                    }
                    for (ArrayList<CGVerbTerm> tmpArray: verb.Goal ) {
                        StringBuilder sbgoal = new StringBuilder();
                        sbgoal.append(" with objective of ");
                        for (CGVerbTerm tmp: tmpArray ) {
                            sbgoal.append(tmp.Node.nosuffix + " ");
                        }
                        NPPhraseSpec subgoal = nlgFactory.createNounPhrase(sbgoal.toString());
                        coordinateObject.addCoordinate(subgoal);
                    }
                    for (ArrayList<CGVerbTerm> tmpArray: verb.Attributes ) {
                        StringBuilder sbattributes = new StringBuilder();
                        sbattributes.append(" with attribute of ");
                        for (CGVerbTerm tmp: tmpArray ) {
                            sbattributes.append(tmp.Node.nosuffix + " ");
                        }
                        NPPhraseSpec subattribute = nlgFactory.createNounPhrase(sbattributes.toString());
                        coordinateObject.addCoordinate(subattribute);
                    }

                    p.setObject(coordinateObject);
                   // c.addCoordinate(p);
                    String output =  realiser.realiseSentence(p);
                    sb.append(output);
                    sb.append(System.getProperty("line.separator"));
                }
                //String output =  realiser.realiseSentence(c);
                //sb.append(output);
                String ta = outputDir + file.getName();
                //System.out.println(ta);
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

    public static void main(String[] args) throws IOException {

        SimpleNLGGenerator();
        /*
        Lexicon lexicon = Lexicon.getDefaultLexicon();
        NLGFactory nlgFactory = new NLGFactory(lexicon);
        Realiser realiser = new Realiser(lexicon);
        SPhraseSpec p = nlgFactory.createClause();
        p = nlgFactory.createClause();
        p.setFeature(Feature.TENSE, Tense.PAST);

        NPPhraseSpec subjectA = nlgFactory.createNounPhrase("Mery");
        NPPhraseSpec subjectB = nlgFactory.createNounPhrase("Helen");
        NPPhraseSpec subjectC = nlgFactory.createNounPhrase("Alisson");

        CoordinatedPhraseElement coord = nlgFactory.createCoordinatedPhrase();
        coord.addCoordinate(subjectA);
        //coord.addCoordinate(subjectB);
        //coord.addCoordinate(subjectC);



        p.setSubject(coord);


        //NPPhraseSpec subject = nlgFactory.createNounPhrase("father");
        VPPhraseSpec verb = nlgFactory.createVerbPhrase("bring");

        AdjPhraseSpec adj = nlgFactory.createAdjectivePhrase("nation");
        //p.setSubject(subject);
        p.setVerb(verb);

        p.setObject(adj);

        //p.addComplement("continent");
        String output =  realiser.realiseSentence(p);
        System.out.println(output);

        //Father dedicates thing.
        //Father brings nation for this continent.
*/
    }
}

