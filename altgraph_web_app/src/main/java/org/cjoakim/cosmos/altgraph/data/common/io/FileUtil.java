package org.cjoakim.cosmos.altgraph.data.common.io;

import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.Graph;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.TripleQueryStruct;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Author;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Library;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Scanner;

/**
 * Instances of this class are used to perform all local disk IO for this application.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Slf4j
public class FileUtil {

    public FileUtil() {

        super();
    }

    public String readUnicode(String filename) {

        Path path = Paths.get(filename);
        StringBuffer sb = new StringBuffer();

        // Java 8, default UTF-8
        try (BufferedReader reader = Files.newBufferedReader(path)) {
            String str;
            while ((str = reader.readLine()) != null) {
                sb.append(str);
                sb.append("\n");
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        return sb.toString();
    }

    public List<String> readLines(String infile) throws IOException {

        List<String> lines = new ArrayList<String>();
        File file = new File(infile);
        Scanner sc = new Scanner(file);
        while (sc.hasNextLine()) {
            lines.add(sc.nextLine().trim());
        }
        return lines;
    }

    public Map<String, Object> readJsonMap(String infile) throws Exception {

        ObjectMapper mapper = new ObjectMapper();
        return mapper.readValue(Paths.get(infile).toFile(), Map.class);
    }

    public TripleQueryStruct readTripleQueryStruct(String infile) throws Exception {

        ObjectMapper mapper = new ObjectMapper();
        return mapper.readValue(Paths.get(infile).toFile(), TripleQueryStruct.class);
    }

    public Library readLibrary(String infile) throws Exception {

        ObjectMapper mapper = new ObjectMapper();
        return mapper.readValue(Paths.get(infile).toFile(), Library.class);
    }

    public Author readAuthor(String infile) throws Exception {

        ObjectMapper mapper = new ObjectMapper();
        return mapper.readValue(Paths.get(infile).toFile(), Author.class);
    }

    public Graph readGraph(String infile) throws Exception {

        ObjectMapper mapper = new ObjectMapper();
        return mapper.readValue(Paths.get(infile).toFile(), Graph.class);
    }

    public void writeJson(Object obj, String outfile, boolean pretty, boolean verbose) throws Exception {

        ObjectMapper mapper = new ObjectMapper();
        String json = null;
        if (pretty) {
            json = mapper.writerWithDefaultPrettyPrinter().writeValueAsString(obj);
            String utf8Str = new String(json.getBytes("UTF8"));
            writeTextFile(outfile, utf8Str, verbose);
        } else {
            json = mapper.writeValueAsString(obj);
            String utf8Str = new String(json.getBytes("UTF8"));
            writeTextFile(outfile, utf8Str, verbose);
            if (verbose) {
                log.warn("file written: " + outfile);
            }
        }
    }

    public void writeTextFile(String outfile, String text, boolean verbose) throws Exception {

        FileWriter fw = null;
        try {
            fw = new FileWriter(outfile);
            fw.write(text);
            if (verbose) {
                log.warn("file written: " + outfile);
            }
        } catch (IOException e) {
            e.printStackTrace();
            throw e;
        } finally {
            if (fw != null) {
                fw.close();
            }
        }
    }

    public void writeLines(String outfile, ArrayList<String> lines, boolean verbose) throws Exception {

        FileWriter fw = null;
        try {
            fw = new FileWriter(outfile);
            for (int i = 0; i < lines.size(); i++) {
                fw.write(lines.get(i));
                fw.write(System.lineSeparator());
            }

            if (verbose) {
                log.warn("file written: " + outfile);
            }
        } catch (IOException e) {
            e.printStackTrace();
            throw e;
        } finally {
            if (fw != null) {
                fw.close();
            }
        }
    }

    public String baseName(File f) {

        return f.getName();
    }

    public String baseNameNoSuffix(File f) {

        return baseName(f).split("\\.")[0];
    }

    public String immediateParentDir(File f) {

        return new File(f.getParent()).getName().toString();
    }
}

