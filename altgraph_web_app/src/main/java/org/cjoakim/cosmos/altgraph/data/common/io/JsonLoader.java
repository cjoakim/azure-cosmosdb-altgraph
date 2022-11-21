package org.cjoakim.cosmos.altgraph.data.common.io;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.SeedDocument;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Movie;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Person;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.SmallTriple;

import java.io.FileInputStream;
import java.io.InputStream;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Scanner;

/**
 * Utility class to efficiently load the Movies, People, and Triples JSON files.
 * Optionally invokes intern() on each created instance.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

public class JsonLoader implements DataConstants {

    public long readMovieDocuments(HashMap<String, Movie> movies, boolean intern) throws Exception {

        InputStream is = new FileInputStream(IMDB_MOVIES_DOCUMENTS_FILE);
        ObjectMapper mapper = new ObjectMapper();
        long unparsableLines = 0;

        Scanner sc = new Scanner(is, StandardCharsets.UTF_8.name());
        try {
            while (sc.hasNextLine()) {
                String line = sc.nextLine().trim();
                Movie movie = parseMovie(mapper, line);
                if (movie != null) {
                    if (intern) {
                        movie.intern();
                    }
                    movies.put(movie.getTconst(), movie);
                } else {
                    unparsableLines++;
                    sysout("readMovieDocuments unparsable line: " + line);
                }
            }
        } catch (Throwable t) {
            t.printStackTrace();
        } finally {
            sc.close();
        }
        return unparsableLines;
    }

    public long readPeopleDocuments(HashMap<String, Person> people, boolean intern) throws Exception {

        InputStream is = new FileInputStream(IMDB_PEOPLE_DOCUMENTS_FILE);
        ObjectMapper mapper = new ObjectMapper();
        long unparsableLines = 0;

        Scanner sc = new Scanner(is, StandardCharsets.UTF_8.name());
        try {
            while (sc.hasNextLine()) {
                String line = sc.nextLine().trim();
                Person person = parsePerson(mapper, line);
                if (person != null) {
                    if (intern) {
                        person.intern();
                    }
                    people.put(person.getNconst(), person);
                } else {
                    unparsableLines++;
                    sysout("readPeopleDocuments unparsable line: " + line);
                }
            }
        } catch (Throwable t) {
            t.printStackTrace();
        } finally {
            sc.close();
        }
        return unparsableLines;
    }

    public long readSmallTripleDocuments(ArrayList<SmallTriple> triples, boolean intern) throws Exception {

        InputStream is = new FileInputStream(IMDB_SMALL_TRIPLES_DOCUMENTS_FILE);
        ObjectMapper mapper = new ObjectMapper();
        long unparsableLines = 0;

        Scanner sc = new Scanner(is, StandardCharsets.UTF_8.name());
        try {
            while (sc.hasNextLine()) {
                String line = sc.nextLine().trim();
                SmallTriple triple = parseTriple(mapper, line);
                if (triple != null) {
                    if (intern) {
                        triple.intern();
                    }
                    triples.add(triple);
                } else {
                    unparsableLines++;
                    sysout("readTripleDocuments unparsable line: " + line);
                }
            }
        } catch (Throwable t) {
            t.printStackTrace();
        } finally {
            sc.close();
        }
        return unparsableLines;
    }

    public long readIndexDocuments(
            String path, ArrayList<SeedDocument> list, boolean intern) throws Exception {

        InputStream is = new FileInputStream(path);
        ObjectMapper mapper = new ObjectMapper();
        long unparsableLines = 0;

        Scanner sc = new Scanner(is, StandardCharsets.UTF_8.name());
        try {
            while (sc.hasNextLine()) {
                String line = sc.nextLine().trim();
                SeedDocument idxDoc = parseIndexDocument(mapper, line);
                if (idxDoc != null) {
                    if (intern) {
                        idxDoc.intern();
                    }
                    list.add(idxDoc);
                } else {
                    unparsableLines++;
                    sysout("readIndexDocuments unparsable line: " + line);
                }
            }
        } catch (Throwable t) {
            t.printStackTrace();
        } finally {
            sc.close();
        }
        return unparsableLines;
    }

    private SmallTriple parseTriple(ObjectMapper mapper, String line) {

        try {
            return mapper.readValue(line, SmallTriple.class);
        } catch (JsonProcessingException e) {
            return null;
        }
    }

    private Movie parseMovie(ObjectMapper mapper, String line) {

        try {
            return mapper.readValue(line, Movie.class);
        } catch (JsonProcessingException e) {
            return null;
        }
    }

    private Person parsePerson(ObjectMapper mapper, String line) {

        try {
            return mapper.readValue(line, Person.class);
        } catch (JsonProcessingException e) {
            return null;
        }
    }

    private SeedDocument parseIndexDocument(ObjectMapper mapper, String line) {

        try {
            return mapper.readValue(line, SeedDocument.class);
        } catch (JsonProcessingException e) {
            return null;
        }
    }

    private void sysout(String s) {
        System.out.println(s);
    }
}
