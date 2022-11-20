package org.cjoakim.cosmos.altgraph.data.common.graph.v2;

import com.azure.cosmos.*;
import com.azure.cosmos.models.CosmosQueryRequestOptions;
import com.azure.cosmos.models.FeedResponse;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.DataAppConfiguration;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.cjoakim.cosmos.altgraph.data.common.io.JsonLoader;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.IndexDocument;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Movie;
import org.cjoakim.cosmos.altgraph.data.common.util.MemoryStats;
import org.jgrapht.graph.DefaultEdge;
import org.jgrapht.graph.DirectedMultigraph;
import org.jgrapht.graph.Multigraph;

import java.util.HashMap;
import java.util.Iterator;

/**
 * This class builds/refreshes an in-memory Google Guava Graph "network" (i.e. - graph)
 * from either a disk or CosmosDB datasource.
 * <p>
 * It is invoked from the refresh() method of class JGraph.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Slf4j
public class JGraphBuilder implements DataConstants {

    private String uri;
    private String key;
    private String dbName;
    private String source = null;
    private boolean directed = false;

    public JGraphBuilder(String source) {

        super();
        this.source = source;
    }

    public org.jgrapht.Graph<String, DefaultEdge> buildImdbGraph() throws Exception {

        log.warn("buildImdbGraph, source:   " + source);
        log.warn("buildImdbGraph, directed: " + directed);

        try {
            switch (source) {
                case IMDB_GRAPH_SOURCE_DISK:
                    return loadImdbGraphFromDisk(directed);
                case IMDB_GRAPH_SOURCE_COSMOS:
                    return loadImdbGraphFromCosmos(directed);
                default:
                    log.warn("undefined graph source: " + source);
            }
        } catch (Exception ex) {
            ex.printStackTrace();
            throw ex;
        }
        return null;
    }

    /**
     * See https://jgrapht.org/guide/UserOverview#graph-structures
     */
    private org.jgrapht.Graph<String, DefaultEdge> createGraphObject(boolean directed) {

        org.jgrapht.Graph<String, DefaultEdge> graph = null;

        if (directed) {
            graph = new DirectedMultigraph<>(DefaultEdge.class);
        } else {
            graph = new Multigraph<>(DefaultEdge.class);
        }
        return graph;
    }

    private org.jgrapht.Graph<String, DefaultEdge> loadImdbGraphFromDisk(boolean directed)
            throws Exception {

        org.jgrapht.Graph<String, DefaultEdge> graph = createGraphObject(directed);

        JsonLoader jsonLoader = new JsonLoader();
        HashMap<String, Movie> moviesHash = new HashMap<String, Movie>();
        jsonLoader.readMovieDocuments(moviesHash, true);
        checkMemory(true, true, "loadImdbGraphFromDisk - after reading movies from file");

        Iterator<String> moviesIt = moviesHash.keySet().iterator();
        long movieNodesCreated = 0;
        long personNodesCreated = 0;
        long edgesCreated = 0;

        while (moviesIt.hasNext()) {
            String tconst = moviesIt.next();
            Movie movie = moviesHash.get(tconst);
            if (!graph.containsVertex(tconst)) {
                graph.addVertex(tconst);
                movieNodesCreated++;
            }

            Iterator<String> peopleIt = movie.getPeople().iterator();
            while (peopleIt.hasNext()) {
                String nconst = peopleIt.next();
                if (!graph.containsVertex(nconst)) {
                    graph.addVertex(nconst);
                    personNodesCreated++;
                }
                graph.addEdge(nconst, tconst);  // person-to-movie
                edgesCreated++;
                if (directed) {
                    // just a single edge between vertices
                } else {
                    graph.addEdge(tconst, nconst);  // movie-to-person
                    edgesCreated++;
                }
            }
        }

        checkMemory(true, true, "loadImdbGraphFromDisk - after building graph");
        log.warn("loadImdbGraphFromDisk - movieNodesCreated:  " + movieNodesCreated);
        log.warn("loadImdbGraphFromDisk - personNodesCreated: " + personNodesCreated);
        log.warn("loadImdbGraphFromDisk - edgesCreated:       " + edgesCreated);
        return graph;
    }

    private org.jgrapht.Graph<String, DefaultEdge> loadImdbGraphFromCosmos(boolean directed) {

        uri = DataAppConfiguration.getInstance().uri;
        key = DataAppConfiguration.getInstance().key;
        dbName = DataAppConfiguration.getInstance().dbName;
        source = DataAppConfiguration.getInstance().imdbGraphSource;
        directed = DataAppConfiguration.getInstance().imdbGraphDirected;

        org.jgrapht.Graph<String, DefaultEdge> graph = createGraphObject(directed);

        CosmosAsyncClient client;
        CosmosAsyncDatabase database;
        CosmosAsyncContainer container;
        double requestCharge = 0;
        long documentsRead = 0;
        long movieNodesCreated = 0;
        long personNodesCreated = 0;
        long edgesCreated = 0;
        String sql = "select * from c where c.pk = 'movie_idx'";
        int pageSize = 1000;
        String continuationToken = null;
        CosmosQueryRequestOptions queryOptions = new CosmosQueryRequestOptions();

        log.warn("uri:    " + uri);
        log.warn("key:    " + key);
        log.warn("dbName: " + dbName);

        checkMemory(true, true, "loadImdbGraphFromCosmos - start");
        long startMs = System.currentTimeMillis();

        client = new CosmosClientBuilder()
                .endpoint(uri)
                .key(key)
                .preferredRegions(DataAppConfiguration.getPreferredRegions())
                .consistencyLevel(ConsistencyLevel.SESSION)
                .contentResponseOnWriteEnabled(true)
                .buildAsyncClient();

        database = client.getDatabase(this.dbName);
        log.warn("client connected to database Id: " + database.getId());

        container = database.getContainer(IMDB_INDEX_CONTAINER);
        log.warn("container: " + container.getId());

        long dbConnectMs = System.currentTimeMillis();

        try {
            do {
                Iterable<FeedResponse<IndexDocument>> feedResponseIterator =
                        container.queryItems(sql, queryOptions, IndexDocument.class).byPage(
                                continuationToken, pageSize).toIterable();  // Convert Asynch Flux to Iterable

                for (FeedResponse<IndexDocument> page : feedResponseIterator) {
                    for (IndexDocument doc : page.getResults()) {
                        documentsRead++;
                        if ((documentsRead % 10000) == 0) {
                            log.warn("" + documentsRead + " -> " + doc.asJson(false));
                        }
                        String tconst = doc.getTargetId();
                        graph.addVertex(tconst);
                        movieNodesCreated++;

                        for (int i = 0; i < doc.getAdjacentVertices().size(); i++) {
                            String nconst = doc.getAdjacentVertices().get(i);
                            if (!graph.containsVertex(nconst)) {
                                graph.addVertex(nconst);
                                personNodesCreated++;
                            }
                            graph.addEdge(nconst, tconst);  // person-to-movie
                            edgesCreated++;

                            if (directed) {
                                // just a single edge between vertices
                            } else {
                                graph.addEdge(tconst, nconst);  // movie-to-person
                                edgesCreated++;
                            }
                        }

                    }
                    requestCharge = requestCharge + page.getRequestCharge();
                    continuationToken = page.getContinuationToken();
                }
            }
            while (continuationToken != null);
        } catch (Throwable t) {
            t.printStackTrace();
        }
        long finishMs = System.currentTimeMillis();
        long dbConnectElapsed = dbConnectMs - startMs;
        long dbReadingElapsed = finishMs - dbConnectMs;
        double dbReadingSeconds = (double) dbReadingElapsed / 1000.0;
        long totalElapsed = finishMs - startMs;

        double ruPerSec = (double) requestCharge / dbReadingSeconds;

        checkMemory(true, true, "loadImdbGraphFromCosmos - after building graph");
        log.warn("loadImdbGraphFromCosmos - documentsRead:      " + documentsRead);
        log.warn("loadImdbGraphFromCosmos - movieNodesCreated:  " + movieNodesCreated);
        log.warn("loadImdbGraphFromCosmos - personNodesCreated: " + personNodesCreated);
        log.warn("loadImdbGraphFromCosmos - edgesCreated:       " + edgesCreated);
        log.warn("loadImdbGraphFromCosmos - requestCharge:      " + requestCharge);
        log.warn("loadImdbGraphFromCosmos - ru per second:      " + ruPerSec);
        log.warn("loadImdbGraphFromCosmos - db connect ms:      " + dbConnectElapsed);
        log.warn("loadImdbGraphFromCosmos - db read ms:         " + dbReadingElapsed);
        log.warn("loadImdbGraphFromCosmos - total elapsed ms:   " + totalElapsed);
        return graph;
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

    protected void sysout(String s) {
        System.out.println(s);
    }

}
