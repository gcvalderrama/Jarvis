import com.sun.corba.se.pept.transport.ReaderThread;
import edu.stanford.nlp.dcoref.Document;
import edu.stanford.nlp.pipeline.Annotation;

import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.Reader;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;

/**
 * Created by greg on 10/19/2016.
 */
public class JarvisReader {

    private String ReadText(String Path) throws IOException {
        byte[] bytes =  Files.readAllBytes(Paths.get(Path) );
        String contents = new String(bytes, StandardCharsets.UTF_8);
        return contents;
    }
    public String ReadString(String Path)throws IOException
    {
        String result = ReadText(Path);
        return result;
    }
    public Annotation ReadAnnotation(String Path)throws IOException
    {
        String result = ReadText(Path);
        return new Annotation(result);
    }
}
