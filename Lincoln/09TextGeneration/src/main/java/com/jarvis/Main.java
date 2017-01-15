package com.jarvis;
import simplenlg.framework.*;
import simplenlg.lexicon.*;
import simplenlg.realiser.english.*;
import simplenlg.phrasespec.*;
import simplenlg.features.*;
import sun.misc.IOUtils;
import java.io.FileInputStream;
import java.io.FileReader;
import java.io.IOException;
import java.util.Scanner;

public class Main {

    public static void main(String[] args) throws IOException {


        Scanner in = new Scanner(new FileReader("filename.txt"));

        

        Lexicon lexicon = Lexicon.getDefaultLexicon();
        NLGFactory nlgFactory = new NLGFactory(lexicon);
        Realiser realiser = new Realiser(lexicon);

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

        //Father dedicates thing.
        //Father brings nation for this continent.

    }
}

