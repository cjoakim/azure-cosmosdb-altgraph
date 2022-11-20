package org.cjoakim.cosmos.altgraph.data.processor;

import lombok.Data;
import lombok.EqualsAndHashCode;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.IndexDocument;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Movie;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Person;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Principal;

import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.io.OutputStreamWriter;
import java.nio.charset.StandardCharsets;
import java.util.*;

/**
 * Batch process to "wrangle" the raw IMDb data into correlated movies.json and people.json
 * files.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@Slf4j
public class ImdbRawDataWranglerProcess extends AbstractConsoleAppProcess {

    private static int DISPLAY_BATCH_SIZE = 100000;
    int minYear;
    int minMinutes;

    long totalMovieCount = 0;
    long totalPersonCount = 0;
    long totalPrincipalCount = 0;
    long titleToPersonCount = 0;
    long startMs = 0;
    private HashMap<String, Movie> movies = new HashMap<String, Movie>();  // key is tconst
    private HashSet<String> principalSet = new HashSet<String>();
    private HashMap<String, Person> people = new HashMap<String, Person>();  // key is nconst

    public void process() throws Exception {

        Movie.setMinMinutes(minMinutes);

        startMs = System.currentTimeMillis();
        checkMemory(true, true, "start");

        readFilterMovies(minYear);
        checkMemory(true, true, "after readFilterMovies");

        readIdentifyPrincipalsInMovies();
        checkMemory(true, true, "after readIdentifyPrincipalsInMovies");

        readNamesOfPrincipals();
        checkMemory(true, true, "after readNamesOfPrincipals");

        associateTitlesToPeople();
        checkMemory(true, true, "after associateTitlesToPeople");

        writeMovies();
        checkMemory(true, true, "after writeMovies");

        writePeople();
        checkMemory(true, true, "after writePeople");

        createWriteMovieIndex();
        checkMemory(true, true, "after createWriteMovieIndex");

        checkMemory(true, true, "finish");
        displayEojTotals();
    }

    private void readFilterMovies(int minYear) throws Exception {

        String path = IMDB_RAW_TITLE_BASICS_FILE;
        sysout("readFilterMovies; path: " + path);

        InputStream is = new FileInputStream(path);
        long lineNumber = 0;

        Scanner sc = new Scanner(is, StandardCharsets.UTF_8.name());
        try {
            while (sc.hasNextLine()) {
                lineNumber++;
                String line = sc.nextLine();
                String[] lineTokens = line.split("\\t");
                if (lineNumber < 2) {
                    printHeaderRow(path, line, lineTokens);
                } else {
                    Movie movie = new Movie(lineTokens);
                    totalMovieCount++;
                    if (movie.include(minYear)) {
                        movie.scrubValues();
                        movie.setCosmosDbCoordinateAttributes(movie.getTconst(), DOCTYPE_MOVIE);
                        movies.put(movie.getTconst(), movie);
                    }
                }
            }
        } catch (Throwable t) {
            t.printStackTrace();
        } finally {
            sc.close();
        }
    }

    private void readIdentifyPrincipalsInMovies() throws Exception {

        String path = IMDB_RAW_TITLE_PRINCIPALS_FILE;
        sysout("readIdentifyPrincipalsInMovies; path: " + path);

        InputStream is = new FileInputStream(path);
        long lineNumber = 0;

        Scanner sc = new Scanner(is, StandardCharsets.UTF_8.name());
        try {
            while (sc.hasNextLine()) {
                lineNumber++;
                String line = sc.nextLine();
                String[] lineTokens = line.split("\\t");
                if (lineNumber < 2) {
                    printHeaderRow(path, line, lineTokens);
                } else {
                    Principal principal = new Principal(lineTokens);
                    principal.setDoctype(DOCTYPE_PRINCIPAL);
                    totalPrincipalCount++;
                    if (principal.isValid()) {
                        if (principal.hasNecessaryRole()) {  // actress,actor,director
                            if (movies.containsKey(principal.getTconst())) {
                                principalSet.add(principal.getNconst());
                            }
                        }
                    }
                }
            }
        } catch (Throwable t) {
            t.printStackTrace();
        } finally {
            sc.close();
        }
    }

    private void readNamesOfPrincipals() throws Exception {

        String path = IMDB_RAW_NAME_BASICS_FILE;
        sysout("readNamesOfPrincipals; path: " + path);

        InputStream is = new FileInputStream(path);
        long lineNumber = 0;

        Scanner sc = new Scanner(is, StandardCharsets.UTF_8.name());
        try {
            while (sc.hasNextLine()) {
                lineNumber++;
                String line = sc.nextLine();
                String[] lineTokens = line.split("\\t");
                if (lineNumber < 2) {
                    printHeaderRow(path, line, lineTokens);
                } else {
                    Person person = new Person(lineTokens);
                    totalPersonCount++;
                    if (person.isValid()) {
                        if (principalSet.contains(person.getNconst())) {
                            person.scrubValues();
                            person.setCosmosDbCoordinateAttributes(person.getNconst(), DOCTYPE_PERSON);
                            people.put(person.getNconst(), person);
                        }
                    }
                }
            }
        } catch (Throwable t) {
            t.printStackTrace();
        } finally {
            sc.close();
        }
    }

    private void writeMovies() throws Exception {

        String path = IMDB_MOVIES_DOCUMENTS_FILE;
        sysout("writeMovies to " + path);

        try (FileOutputStream out = new FileOutputStream(path);
             OutputStreamWriter writer = new OutputStreamWriter(out)) {

            Iterator<String> iterator = movies.keySet().iterator();
            while (iterator.hasNext()) {
                String key = iterator.next();
                Movie movie = movies.get(key);
                String json = movie.asJson(false).trim();
                if (json.startsWith("{")) {
                    if (json.endsWith("}")) {
                        writer.write(json);
                        writer.write(System.lineSeparator());
                    }
                }
            }
        }
    }

    private void writePeople() throws Exception {

        String path = IMDB_PEOPLE_DOCUMENTS_FILE;
        sysout("writePeople to " + path);

        try (FileOutputStream out = new FileOutputStream(path);
             OutputStreamWriter writer = new OutputStreamWriter(out)) {

            Iterator<String> iterator = people.keySet().iterator();
            while (iterator.hasNext()) {
                String key = iterator.next();
                Person person = people.get(key);
                String json = person.asJson(false).trim();
                if (json.startsWith("{")) {
                    if (json.endsWith("}")) {
                        writer.write(json);
                        writer.write(System.lineSeparator());
                    }
                }
            }
        }
    }

    private void associateTitlesToPeople() throws Exception {

        String path = IMDB_RAW_TITLE_PRINCIPALS_FILE;
        sysout("associateTitlesToPeople; path: " + path);

        InputStream is = new FileInputStream(path);
        long lineNumber = 0;
        Scanner sc = new Scanner(is, StandardCharsets.UTF_8.name());
        try {
            while (sc.hasNextLine()) {
                String line = sc.nextLine();
                lineNumber++;
                if (lineNumber > 1) {
                    String[] lineTokens = line.split("\\t");
                    Principal principal = new Principal(lineTokens);
                    if (principal.isValid()) {
                        String nconst = principal.getNconst();
                        if (people.containsKey(nconst)) {
                            String tconst = principal.getTconst();
                            if (movies.containsKey(tconst)) {
                                Person person = people.get(nconst);
                                Movie movie = movies.get(tconst);
                                movie.addPerson(nconst);
                                person.addTitle(tconst);
                                titleToPersonCount++;
                            }
                        }
                    }
                }
            }
        } catch (Throwable t) {
            t.printStackTrace();
        } finally {
            sc.close();
        }
    }

    private void createWriteMovieIndex() throws Exception {

        String path = IMDB_MOVIES_INDEX_FILE;
        ArrayList<IndexDocument> indexDocs = new ArrayList<IndexDocument>();
        Iterator<String> moviesIt = movies.keySet().iterator();
        while (moviesIt.hasNext()) {
            String key = moviesIt.next();
            Movie movie = movies.get(key);
            IndexDocument idxDoc = new IndexDocument(
                    DOCTYPE_MOVIE_IDX, movie.getId(), movie.getPk());

            Iterator<String> peopleIt = movie.getPeople().iterator();
            while (peopleIt.hasNext()) {
                idxDoc.addAdjacentVertex(peopleIt.next());
            }
            idxDoc.intern();
            indexDocs.add(idxDoc);
        }

        try (FileOutputStream out = new FileOutputStream(path);
             OutputStreamWriter writer = new OutputStreamWriter(out)) {
            for (int i = 0; i < indexDocs.size(); i++) {
                IndexDocument idxDoc = indexDocs.get(i);
                String json = idxDoc.asJson(false).trim();
                if (json.startsWith("{")) {
                    if (json.endsWith("}")) {
                        writer.write(json);
                        writer.write(System.lineSeparator());
                    }
                }
            }
        }
        sysout("createWriteMovieIndex, file written: " + path + "  docs: " + indexDocs.size());
    }

    private void displayEojTotals() {

        double elapsedMs = System.currentTimeMillis() - startMs;
        double elapsedSec = elapsedMs / 1000.0;
        double elapsedMin = elapsedSec / 60.0;

        sysout("");
        sysout("EOJ Totals:");
        sysout("  minYear:                  " + minYear);
        sysout("  minMinutes:               " + minMinutes);
        sysout("  totalMovieCount:          " + totalMovieCount);
        sysout("  includedMovieCount:       " + movies.size());
        sysout("  totalPrincipalCount:      " + totalPrincipalCount);
        sysout("  includedPrincipalCount:   " + principalSet.size());
        sysout("  totalPersonCount:         " + totalPersonCount);
        sysout("  includedPersonCount:      " + people.size());
        sysout("  included Movies + People: " + (people.size() + movies.size()));
        sysout("  titleToPersonCount:       " + titleToPersonCount);
        sysout("  elapsedMs:                " + elapsedMs);
        sysout("  elapsedMin:               " + elapsedMin);
    }

    private void printHeaderRow(String path, String line, String[] lineTokens) {

        sysout("path:   " + path);
        sysout("header: " + line);
        for (int t = 0; t < lineTokens.length; t++) {
            sysout("token: " + t + " -> " + lineTokens[t]);
        }
    }
}
