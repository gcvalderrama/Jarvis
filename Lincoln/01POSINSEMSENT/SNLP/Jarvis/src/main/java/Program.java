import edu.stanford.nlp.pipeline.Annotation;

import java.io.IOException;
import java.util.List;

/**
 * Created by greg on 10/19/2016.
 */
public class Program {
    public static void main(String[] Args) throws IOException {

        String InputPath =  "D:/Tesis2016/Jarvis/Lincoln/00Input/inputlincon.txt";

        JarvisReader reader =  new JarvisReader();
        JarvisNLP nlp = new JarvisNLP();
        Annotation result =  reader.ReadAnnotation(InputPath);

        nlp.CoreferenceResolution(result);

        //List<String> sentences  = nlp.GetSentences(result.copy());

        //JarvisWriter writer = new JarvisWriter();
        //writer.WriteSentences(sentences);


    }
}
