package org.cjoakim.cosmos.altgraph.data.processor;

import lombok.Data;
import lombok.EqualsAndHashCode;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Movie;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Person;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.SmallTriple;

import java.io.FileOutputStream;
import java.io.OutputStreamWriter;
import java.util.HashMap;
import java.util.Iterator;

/**
 * Batch process to create the triples.json file from movies.json and people.json.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@Slf4j
public class ImdbTripleBuilderProcess extends AbstractConsoleAppProcess {

    private static int DISPLAY_BATCH_SIZE = 20000;
    long tripleLinesWritten = 0;
    long startMs = 0;
    private HashMap<String, Movie> movies = new HashMap<String, Movie>();
    private HashMap<String, Person> people = new HashMap<String, Person>();

    public void process() throws Exception {

        startMs = System.currentTimeMillis();
        movies = readMovieDocuments(false);
        people = readPeopleDocuments(false);

        FileOutputStream fos = new FileOutputStream(IMDB_SMALL_TRIPLES_DOCUMENTS_FILE);
        OutputStreamWriter writer = new OutputStreamWriter(fos);

        Iterator<String> peopleIterator = people.keySet().iterator();
        while (peopleIterator.hasNext()) {
            String nconst = peopleIterator.next();
            Person person = people.get(nconst);
            if (person != null) {
                Iterator titleIterator = person.getTitles().iterator();
                while (titleIterator.hasNext()) {
                    String movieTconst = (String) titleIterator.next();

                    if (movies.containsKey(movieTconst)) {
                        Movie movie = movies.get(movieTconst);

                        // Create the person-in-movie Triple
                        SmallTriple t = new SmallTriple();
                        t.setCosmosDbSmallTripleCoordinateAttributes();
                        t.setSubjectType(person.getDoctype());
                        t.setSubjectIdPk(person.getNconst());
                        t.setPredicate(PREDICATE_IN_MOVIE);
                        t.setObjectType(movie.getDoctype());
                        t.setObjectIdPk(movie.getTconst());
                        String json = t.asJson(false);
                        String utf8 = utf8String(json).trim();
                        if (utf8.startsWith("{")) {
                            if (utf8.endsWith("}")) {
                                writer.write(utf8);
                                writer.write(System.lineSeparator());
                                tripleLinesWritten++;
                            }
                        }

                        // Create the movie-has-person Triple
                        t = new SmallTriple();
                        t.setCosmosDbSmallTripleCoordinateAttributes();
                        t.setSubjectType(movie.getDoctype());
                        t.setSubjectIdPk(movie.getTconst());
                        t.setPredicate(PREDICATE_HAS_PERSON);
                        t.setObjectType(person.getDoctype());
                        t.setObjectIdPk(person.getNconst());

                        json = t.asJson(false);
                        utf8 = utf8String(json).trim();
                        if (utf8.startsWith("{")) {
                            if (utf8.endsWith("}")) {
                                writer.write(utf8);
                                writer.write(System.lineSeparator());
                                tripleLinesWritten++;
                            }
                        }
                    }
                }
            }
        }
        displayEojTotals();
    }

    private void displayEojTotals() {

        double elapsedMs = System.currentTimeMillis() - startMs;
        double elapsedSec = elapsedMs / 1000.0;
        double elapsedMin = elapsedSec / 60.0;

        sysout("");
        sysout("EOJ Totals:");
        sysout("  movies size:            " + movies.size());
        sysout("  people size:            " + people.size());
        sysout("  tripleLinesWritten:     " + tripleLinesWritten);
        sysout("  elapsedMs:              " + elapsedMs);
        sysout("  elapsedMin:             " + elapsedMin);
    }
}
