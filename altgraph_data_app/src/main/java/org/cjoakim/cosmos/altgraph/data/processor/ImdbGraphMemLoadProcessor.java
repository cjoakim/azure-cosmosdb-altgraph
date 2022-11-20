package org.cjoakim.cosmos.altgraph.data.processor;

import lombok.Data;
import lombok.EqualsAndHashCode;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.DataAppConfiguration;
import org.cjoakim.cosmos.altgraph.data.common.dao.CosmosAsynchDao;
import org.cjoakim.cosmos.altgraph.data.common.util.MemoryStats;
import org.jgrapht.graph.DefaultEdge;
import org.springframework.beans.factory.annotation.Value;

/**
 * Batch process to explore Graph functionality, performance, and JVM memory utilization.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@Slf4j
public class ImdbGraphMemLoadProcessor extends AbstractConsoleAppProcess {

    private static int DISPLAY_BATCH_SIZE = 10000;

    private String graphType;
    private String container;
    org.jgrapht.Graph<String, DefaultEdge> graph;

    long startMs = 0;
    long indexDocumentsRead = 0;

    @Value("${spring.cloud.azure.cosmos.endpoint}")
    public String uri;
    @Value("${spring.cloud.azure.cosmos.key}")
    public String key;
    @Value("${spring.cloud.azure.cosmos.database}")
    private String dbName;


    public void process() throws Exception {

        checkMemory(true, true, "beginning of process()");
        startMs = System.currentTimeMillis();

        if (true) {
            switch (graphType) {
                case "imdb":
                    loadImdbGraph();
                    break;
                default:
                    sysout("unknown graphType: " + graphType);
            }
        }
    }

    private void loadImdbGraph() throws Exception {

        MemoryStats.setShouldRecordHistory(true);
        checkMemory(true, true, "loadImdbGraph start");
        boolean verbose = DataAppConfiguration.booleanArg(VERBOSE_FLAG);

        CosmosAsynchDao dao = new CosmosAsynchDao();
        dao.initialize(uri, key, dbName, verbose);
        dao.setCurrentContainer(container);

        double elapsedMs = System.currentTimeMillis() - startMs;
        double elapsedSec = elapsedMs / 1000.0;
        double elapsedMin = elapsedSec / 60.0;

        sysout("");
        sysout("EOJ Totals:");
        sysout("  indexDocumentsRead:  " + indexDocumentsRead);
        sysout("  elapsedMs:           " + elapsedMs);
        sysout("  elapsedMin:          " + elapsedMin);

        MemoryStats.writeHistory("loadImdbGraph");
    }

    /**
     * Explore the JGraphT library.  See https://jgrapht.org/guide/HelloJGraphT
     */
//    private void jgraphtExperiment() {
//
//        org.jgrapht.Graph<String, DefaultEdge> g = new SimpleGraph<>(DefaultEdge.class);
//
//        // Movie Vertices
//        Iterator<String> moviesIt = movies.keySet().iterator();
//        long movieNodesCreated = 0;
//        while (moviesIt.hasNext()) {
//            String tconst = moviesIt.next();
//            g.addVertex(tconst);
//            movieNodesCreated++;
//        }
//        checkMemory(true, true, "after creating jgrapht movie vertices - count: " + movieNodesCreated);
//
//        // People Vertices
//        Iterator<String> peopleIt = people.keySet().iterator();
//        long peopleNodesCreated = 0;
//        while (peopleIt.hasNext()) {
//            String nconst = peopleIt.next();
//            g.addVertex(nconst);
//            peopleNodesCreated++;
//        }
//        checkMemory(true, true, "after creating jgrapht people vertices - count: " + peopleNodesCreated);
//
//        // Edges: person-to-movie, and movie-to-person
//        peopleIt = people.keySet().iterator();
//        long edgesCreated = 0;
//        while (peopleIt.hasNext()) {
//            String nconst = peopleIt.next();
//            Person person = people.get(nconst);
//            Iterator<String> personMoviesIt = person.getTitles().iterator();
//            while (personMoviesIt.hasNext()) {
//                String tconst = personMoviesIt.next();
//                g.addEdge(nconst, tconst);  // person-to-movie
//                edgesCreated++;
//                g.addEdge(tconst, nconst);  // movie-to-person
//                edgesCreated++;
//            }
//        }
//        checkMemory(true, true, "after creating jgrapht edges - count: " + edgesCreated);
//
//        // Find the shortest path with the DijkstraShortestPath class in JGraphT
//        String sourceVertex = PERSON_KEVIN_BACON;
//        String targetVertex = PERSON_CHARLOTTE_RAMPLING;
//        sysout("jgrapht finding path from " + sourceVertex + " to " + targetVertex);
//        long start = System.currentTimeMillis();
//        GraphPath<String, DefaultEdge> path = DijkstraShortestPath.findPathBetween(g, sourceVertex, targetVertex);
//        long elapsed = System.currentTimeMillis() - start;
//
//        if (path == null) {
//            sysout("path is null");
//        }
//        else {
//            sysout("elapsed milliseconds: " + elapsed);
//            sysout("path getLength:       " + path.getLength());
//            sysout("path getStartVertex:  " + path.getStartVertex());
//            sysout("path getEndVertex:    " + path.getEndVertex());
//            for (DefaultEdge e : path.getEdgeList()) {
//                String[] array = parseDefaultEdge(e);
//                String name0 = lookupImdbName(array[0]);
//                String name1 = lookupImdbName(array[1]);
//                StringBuffer sb = new StringBuffer();
//                sb.append("EDGE: " + array[0] + " " + name0 + " -> ");
//                sb.append("" + array[1] + " " + name1);
//                sysout(sb.toString());
//            }
//            for (String vertex : path.getVertexList()) {
//                sysout("vertex: " + vertex);
//            }
//        }

//        jgrapht finding path from nm0000102 to nm0001648
//        path getLength:      4
//        path getStartVertex: nm0000102
//        path getEndVertex:   nm0001648
//        EDGE: nm0000102 Kevin Bacon -> tt0164052 Hollow Man
//        EDGE: nm0000682 Paul Verhoeven -> tt0164052 Hollow Man
//        EDGE: nm0000682 Paul Verhoeven -> tt6823148 Benedetta
//        EDGE: nm0001648 Charlotte Rampling -> tt6823148 Benedetta
//    }

}
