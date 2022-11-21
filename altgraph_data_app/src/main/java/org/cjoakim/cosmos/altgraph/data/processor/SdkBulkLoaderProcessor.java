package org.cjoakim.cosmos.altgraph.data.processor;

import lombok.Data;
import lombok.EqualsAndHashCode;
import lombok.NoArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.DataAppConfiguration;
import org.cjoakim.cosmos.altgraph.data.common.dao.CosmosAsynchDao;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.SeedDocument;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Movie;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Person;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.SmallTriple;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;
import reactor.core.publisher.Flux;

import java.util.ArrayList;

/**
 * Execute CosmosDB bulk loading with the SDK and Asynch methods.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Component
@Data
@EqualsAndHashCode(callSuper = false)
@NoArgsConstructor
@Slf4j
public class SdkBulkLoaderProcessor extends AbstractConsoleAppProcess {
    private String container;
    private long skipCount = 0;

    private long outputDocCount = 0;
    private long maxRecords = Long.MAX_VALUE;
    private String infile;
    private String loadType;

    @Value("${spring.cloud.azure.cosmos.endpoint}")
    public String uri;
    @Value("${spring.cloud.azure.cosmos.key}")
    public String key;
    @Value("${spring.cloud.azure.cosmos.database}")
    private String dbName;

    private CosmosAsynchDao dao = new CosmosAsynchDao();

    private boolean doWrites = false;
    private boolean verbose = false;

    public void process() throws Exception {

        try {
            doWrites = DataAppConfiguration.booleanArg(DO_WRITES_FLAG);
            verbose = DataAppConfiguration.booleanArg(VERBOSE_FLAG);

            log.warn("process, skipCount:  " + skipCount);
            log.warn("process, maxRecords: " + maxRecords);
            log.warn("process, loadType:   " + loadType);
            log.warn("process, infile:     " + infile);
            log.warn("process, doWrites:   " + doWrites);
            log.warn("process, dbName:     " + dbName);
            log.warn("process, container:  " + container);

            dao.initialize(uri, key, dbName, verbose);
            dao.setCurrentContainer(container);

            if (loadType.equalsIgnoreCase("imdb_bulk_load_movies")) {
                bulkLoadImdbMovies();
            } else if (loadType.equalsIgnoreCase("imdb_bulk_load_people")) {
                bulkLoadImdbPeople();
            } else if (loadType.equalsIgnoreCase("imdb_bulk_load_small_triples")) {
                bulkLoadImdbSmallTriples();
            } else if (loadType.equalsIgnoreCase("imdb_bulk_load_movies_seed")) {
                bulkLoadImdbMovieSeed();
            }
        } finally {
            dao.close();
        }
    }

    private void bulkLoadImdbMovies() throws Exception {

        log.warn("bulkLoadImdbMovies...");
        ArrayList<Movie> movies = readMovieDocumentsAsList(true);
        log.warn("movies read from disk: " + movies.size());
        long startMs = System.currentTimeMillis();
        dao.bulkLoadMovies(Flux.fromIterable(movies));
        long elapsedMs = System.currentTimeMillis() - startMs;
        log.warn("bulkLoadImdbMovies complete in: " + elapsedMs + "ms");
    }

    private void bulkLoadImdbPeople() throws Exception {

        log.warn("bulkLoadImdbPeople...");
        ArrayList<Person> people = readPeopleDocumentsAsList(true);
        log.warn("people read from disk: " + people.size());
        long startMs = System.currentTimeMillis();
        dao.bulkLoadPeople(Flux.fromIterable(people));
        long elapsedMs = System.currentTimeMillis() - startMs;
        log.warn("bulkLoadImdbPeople complete in: " + elapsedMs + "ms");
    }

    private void bulkLoadImdbSmallTriples() throws Exception {

        log.warn("bulkLoadImdbSmallTriples...");
        ArrayList<SmallTriple> triples = readSmallTripleDocumentsAsList(true);
        log.warn("triples read from disk: " + triples.size());
        long startMs = System.currentTimeMillis();
        dao.bulkLoadSmallTriples(Flux.fromIterable(triples));
        long elapsedMs = System.currentTimeMillis() - startMs;
        log.warn("bulkLoadImdbSmallTriples complete in: " + elapsedMs + "ms");
    }

    private void bulkLoadImdbMovieSeed() throws Exception {

        log.warn("bulkLoadImdbMovieSeed...");
        ArrayList<SeedDocument> idxDocs = readIndexDocumentsAsList(IMDB_MOVIES_SEED_FILE, true);
        log.warn("idxDocs read from disk: " + idxDocs.size());
        long startMs = System.currentTimeMillis();
        dao.bulkLoadIndexDocuments(Flux.fromIterable(idxDocs));
        long elapsedMs = System.currentTimeMillis() - startMs;
        log.warn("bulkLoadImdbMovieSeed complete in: " + elapsedMs + "ms");
    }
}
