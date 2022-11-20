package org.cjoakim.cosmos.altgraph.data.processor;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.cjoakim.cosmos.altgraph.data.common.ImdbConstants;
import org.cjoakim.cosmos.altgraph.data.common.io.JsonLoader;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.IndexDocument;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Movie;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Person;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.SmallTriple;
import org.cjoakim.cosmos.altgraph.data.common.util.MemoryStats;

import java.io.FileOutputStream;
import java.io.OutputStreamWriter;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;

/**
 * Base class for all console-app processes delegated from the main entrypoint class.
 *
 * Chris Joakim, Microsoft, November 2022
 */

public abstract class AbstractConsoleAppProcess implements DataConstants, ImdbConstants {

    public abstract void process() throws Exception;

    private ArrayList<String> memoryStatsList = new ArrayList<String>();

    protected HashMap<String, Movie> readMovieDocuments(boolean intern) throws Exception {

        checkMemory(true, true, "readMovieDocuments start");
        HashMap<String, Movie> movies = new HashMap<String, Movie>();
        JsonLoader loader = new JsonLoader();
        long unparsableLines = loader.readMovieDocuments(movies, intern);
        sysout("readMovieDocuments, documents: " + movies.size() + ", unparsableLines: " + unparsableLines);
        checkMemory(true, true, "readMovieDocuments finish");
        return movies;
    }

    protected ArrayList<Movie> readMovieDocumentsAsList(boolean intern) throws Exception {

        HashMap<String, Movie> hash = readMovieDocuments(intern);
        ArrayList<Movie> list = new ArrayList<Movie>();
        Iterator<String> it = hash.keySet().iterator();
        while (it.hasNext()) {
            String key = it.next();
            list.add(hash.get(key));
        }
        return list;
    }

    protected HashMap<String, Person> readPeopleDocuments(boolean intern) throws Exception {

        checkMemory(true, true, "readPeopleDocuments start");
        HashMap<String, Person> people = new HashMap<String, Person>();
        JsonLoader loader = new JsonLoader();
        long unparsableLines = loader.readPeopleDocuments(people, intern);
        sysout("readPeopleDocuments, documents: " + people.size() + ", unparsableLines: " + unparsableLines);
        checkMemory(true, true, "readPeopleDocuments finish");
        return people;
    }

    protected ArrayList<Person> readPeopleDocumentsAsList(boolean intern) throws Exception {

        HashMap<String, Person> hash = readPeopleDocuments(intern);
        ArrayList<Person> list = new ArrayList<Person>();
        Iterator<String> it = hash.keySet().iterator();
        while (it.hasNext()) {
            String key = it.next();
            list.add(hash.get(key));
        }
        return list;
    }

    protected ArrayList<SmallTriple> readSmallTripleDocumentsAsList(boolean intern) throws Exception {

        checkMemory(true, true, "readSmallTripleDocumentsAsList start");
        ArrayList<SmallTriple> objects = new ArrayList<SmallTriple>();
        JsonLoader loader = new JsonLoader();
        long unparsableLines = loader.readSmallTripleDocuments(objects, intern);
        sysout("readSmallTripleDocumentsAsList, documents: " + objects.size() + ", unparsableLines: " + unparsableLines);
        checkMemory(true, true, "readSmallTripleDocumentsAsList finish");
        return objects;
    }

    protected ArrayList<IndexDocument> readIndexDocumentsAsList(String path, boolean intern) throws Exception {

        checkMemory(true, true, "readIndexDocumentsAsList start");
        ArrayList<IndexDocument> objects = new ArrayList<IndexDocument>();
        JsonLoader loader = new JsonLoader();
        long unparsableLines = loader.readIndexDocuments(path, objects, intern);
        sysout("readIndexDocumentsAsList, documents: " + objects.size() + ", unparsableLines: " + unparsableLines);
        checkMemory(true, true, "readIndexDocumentsAsList finish");
        return objects;
    }

    protected MemoryStats checkMemory(boolean doGc, boolean display, String note) {
        if (doGc) {
            System.gc();
        }
        MemoryStats ms = new MemoryStats(note);
        if (display) {
            try {
                sysout(ms.asDelimitedHeaderLine("|"));
                sysout(ms.asDelimitedDataLine("|"));
            } catch (Exception e) {
                sysout("error serializing MemoryStats to JSON");
            }
        }
        return ms;
    }

    @JsonIgnore
    public String asJson(Object obj, boolean pretty) throws Exception {

        try {
            ObjectMapper mapper = new ObjectMapper();
            if (pretty) {
                return mapper.writerWithDefaultPrettyPrinter().writeValueAsString(obj);
            } else {
                return mapper.writeValueAsString(this);
            }
        } catch (JsonProcessingException e) {
            e.printStackTrace();
            return null;
        }
    }

    protected void writeMoviesOfInterest(HashMap<String, Movie> movies) {

        String path = IMDB_MOVIES_OF_INTEREST_FILE;
        sysout("writeMoviesOfInterest to " + path);
        try {
            try (FileOutputStream out = new FileOutputStream(path);
                 OutputStreamWriter writer = new OutputStreamWriter(out)) {
                ObjectMapper mapper = new ObjectMapper();
                for (int i = 0; i < MOVIES_OF_INTEREST.length; i++) {
                    String key = MOVIES_OF_INTEREST[i];
                    Movie m = movies.get(key);
                    writer.write(m.asJson(true));
                    writer.write(System.lineSeparator());
                    writer.write("---");
                    writer.write(System.lineSeparator());
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    protected void writePeopleOfInterest(HashMap<String, Person> people) {

        String path = IMDB_PEOPLE_OF_INTEREST_FILE;
        sysout("writePeopleOfInterest to " + path);
        try {
            try (FileOutputStream out = new FileOutputStream(path);
                 OutputStreamWriter writer = new OutputStreamWriter(out)) {
                ObjectMapper mapper = new ObjectMapper();
                for (int i = 0; i < PEOPLE_OF_INTEREST.length; i++) {
                    String key = PEOPLE_OF_INTEREST[i];
                    Person p = people.get(key);
                    writer.write(p.asJson(true));
                    writer.write(System.lineSeparator());
                    writer.write("---");
                    writer.write(System.lineSeparator());
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }


    protected void writeMoviesMapFile(HashMap<String, Movie> movies) throws Exception {

        HashMap<String, String> map = new HashMap<String, String>();
        Iterator<String> it = movies.keySet().iterator();
        while (it.hasNext()) {
            String key = it.next();
            Movie m = movies.get(key);
            map.put(key, m.titleWordsJoined());
        }
        String path = IMDB_MOVIES_MAP_FILE;
        sysout("writeMoviesMapFile to " + path);

        try (FileOutputStream out = new FileOutputStream(path);
             OutputStreamWriter writer = new OutputStreamWriter(out)) {
            ObjectMapper mapper = new ObjectMapper();
            writer.write(mapper.writerWithDefaultPrettyPrinter().writeValueAsString(map));
        }
        movies = null;
    }

    protected void writePeopleMapFile(HashMap<String, Person> people) throws Exception {

        HashMap<String, String> map = new HashMap<String, String>();
        Iterator<String> it = people.keySet().iterator();
        while (it.hasNext()) {
            String key = it.next();
            Person p = people.get(key);
            map.put(key, p.primaryNameWordsJoined());
        }
        String path = IMDB_PEOPLE_MAP_FILE;
        sysout("writePeopleMapFile to " + path);

        try (FileOutputStream out = new FileOutputStream(path);
             OutputStreamWriter writer = new OutputStreamWriter(out)) {
            ObjectMapper mapper = new ObjectMapper();
            writer.write(mapper.writerWithDefaultPrettyPrinter().writeValueAsString(map));
        }
        people = null;
    }

    public String utf8String(String s) {
        // see https://www.baeldung.com/java-string-encode-utf-8
        byte[] bytes = s.getBytes(StandardCharsets.UTF_8);
        return new String(bytes, StandardCharsets.UTF_8);
    }

    protected void sysout(String s) {
        System.out.println(s);
    }

}
