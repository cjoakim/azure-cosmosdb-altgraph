package org.cjoakim.cosmos.altgraph.data.processor;

import lombok.Data;
import lombok.EqualsAndHashCode;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.DataAppConfiguration;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.cjoakim.cosmos.altgraph.data.common.ImdbConstants;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.JGraph;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.JStarNetwork;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct.DegreeStruct;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct.EdgeStruct;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct.EdgesStruct;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct.VertexValueStruct;
import org.cjoakim.cosmos.altgraph.data.common.io.FileUtil;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Movie;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Person;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.SmallTriple;
import org.cjoakim.cosmos.altgraph.data.common.util.MemoryStats;
import org.jgrapht.graph.DefaultEdge;
import org.jgrapht.traverse.DepthFirstIterator;

import java.util.*;

/**
 * Batch process to explore Graph functionality, performance, and JVM memory utilization
 * using the JGraphT library.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@Slf4j
public class ImdbJgraphtGraphTraversalProcess extends AbstractConsoleAppProcess {

    private static int DISPLAY_BATCH_SIZE = 20000;

    private String traversalType;
    private String nconst1;
    private String nconst2;
    private boolean directed;
    private String source;

    long startMs = 0;
    long tripleLinesRead = 0;
    long unparsableTriples = 0;

    long moviesCount = 0;
    long peopleCount = 0;
    long triplesCount = 0;

    private ArrayList<SmallTriple> triples = new ArrayList<SmallTriple>();
    private HashMap<String, Movie> movies = new HashMap<String, Movie>();
    private HashMap<String, Person> people = new HashMap<String, Person>();

    private HashMap<String, String> idMap = new HashMap<String, String>();
    private ArrayList<String> memoryStatsList = new ArrayList<String>();
    private JGraph jgraph;

    public void process() throws Exception {
        checkMemory(true, false, "beginning of process()");
        startMs = System.currentTimeMillis();
        if (true) {
            switch (this.traversalType) {
                case "traverse":
                    traverse();
                    break;
                default:
                    sysout("unknown traversalType: " + traversalType);
            }
        }
    }

    private void traverse() throws Exception {
        MemoryStats.setShouldRecordHistory(true);

        checkMemory(true, false, "traverse start");
        String source = DataAppConfiguration.getInstance().imdbGraphSource;
        jgraph = new JGraph(DataConstants.GRAPH_DOMAIN_IMDB, source);
        int[] counts = jgraph.getVertexAndEdgeCounts();
        checkMemory(true, false,
                "traverse after jgraph.refresh() - vertices: " + counts[0] + ", edges: " + counts[1]);

        exploreShortestPath();
        exploreDegree();
        exploreEdges();
        explorePageRankVertex();
        explorePageRankAll();
        exploreCentralityVertex();
        exploreCentralityAll();
        exploreStarNetwork();

        MemoryStats.writeHistory("jgrapht_traverse");
    }

    private void exploreShortestPath() {
        try {
            EdgesStruct edgesStruct =
                    jgraph.getShortestPathAsEdgesStruct(
                            PERSON_KEVIN_BACON,
                            PERSON_CHARLOTTE_RAMPLING);
            FileUtil util = new FileUtil();
            util.writeJson(edgesStruct, "out/_shortestPath.json", true, true);

            checkMemory(true, false, "exploreShortestPath finish");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void exploreDegree() {
        try {
            FileUtil util = new FileUtil();
            DegreeStruct struct = new DegreeStruct();
            struct.setVertex(PERSON_KEVIN_BACON);
            long startMs = System.currentTimeMillis();
            struct.setDegree(jgraph.degreeOf(PERSON_KEVIN_BACON));
            struct.setInDegree(jgraph.inDegreeOf(PERSON_KEVIN_BACON));
            struct.setElapsedMs(System.currentTimeMillis() - startMs);
            util.writeJson(struct, "out/_degreeKevinBacon.json", true, true);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void exploreEdges() {
        try {
            FileUtil util = new FileUtil();
            EdgesStruct edgesStruct = new EdgesStruct();
            edgesStruct.setVertex1(PERSON_KEVIN_BACON);
            long startMs = System.currentTimeMillis();
            Set<DefaultEdge> edges = jgraph.edgesOf(PERSON_KEVIN_BACON);

            Iterator<DefaultEdge> it = edges.iterator();
            while (it.hasNext()) {
                DefaultEdge de = it.next();
                edgesStruct.addEdge(parseDefaultEdge(de));
            }
            edgesStruct.setElapsedMs(System.currentTimeMillis() - startMs);
            util.writeJson(edgesStruct, "out/_edgesOfKevinBacon.json", true, true);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void explorePageRankVertex() {
        try {
            FileUtil util = new FileUtil();
            VertexValueStruct vvStruct = new VertexValueStruct();
            vvStruct.setFunction("PageRank");
            long startMs = System.currentTimeMillis();
            vvStruct.addRank(PERSON_KEVIN_BACON, jgraph.pageRankForVertex(PERSON_KEVIN_BACON));
            vvStruct.addRank(PERSON_LORI_SINGER, jgraph.pageRankForVertex(PERSON_LORI_SINGER));
            vvStruct.addRank(PERSON_CHARLOTTE_RAMPLING, jgraph.pageRankForVertex(PERSON_CHARLOTTE_RAMPLING));
            vvStruct.addRank(PERSON_JULIA_ROBERTS, jgraph.pageRankForVertex(PERSON_JULIA_ROBERTS));
            vvStruct.addRank(MOVIE_FOOTLOOSE, jgraph.pageRankForVertex(MOVIE_FOOTLOOSE));
            vvStruct.setElapsedMs(System.currentTimeMillis() - startMs);
            vvStruct.sort();
            util.writeJson(vvStruct, "out/_pageRanksSelected.json", true, true);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void explorePageRankAll() {
        try {
            FileUtil util = new FileUtil();
            VertexValueStruct vvStruct = new VertexValueStruct();
            vvStruct.setFunction("PageRank");
            long startMs = System.currentTimeMillis();
            Map<String, Double> scores = jgraph.pageRankForAll();
            Iterator<String> prAllIt = scores.keySet().iterator();
            while (prAllIt.hasNext()) {
                String vertex = prAllIt.next();
                Double value = scores.get(vertex);
                vvStruct.addRank(vertex, value);
            }
            vvStruct.setElapsedMs(System.currentTimeMillis() - startMs);
            vvStruct.sort();
            util.writeJson(vvStruct, "out/_pageRanksAll.json", true, true);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void exploreCentralityVertex() {
        try {
            FileUtil util = new FileUtil();
            VertexValueStruct vvStruct = new VertexValueStruct();
            vvStruct.setFunction("Centrality");
            long startMs = System.currentTimeMillis();
            vvStruct.addRank(PERSON_KEVIN_BACON, jgraph.centralityOfVertex(PERSON_KEVIN_BACON));
            vvStruct.addRank(PERSON_LORI_SINGER, jgraph.centralityOfVertex(PERSON_LORI_SINGER));
            vvStruct.addRank(PERSON_CHARLOTTE_RAMPLING, jgraph.centralityOfVertex(PERSON_CHARLOTTE_RAMPLING));
            vvStruct.addRank(PERSON_JULIA_ROBERTS, jgraph.centralityOfVertex(PERSON_JULIA_ROBERTS));
            vvStruct.addRank(MOVIE_FOOTLOOSE, jgraph.centralityOfVertex(MOVIE_FOOTLOOSE));
            vvStruct.setElapsedMs(System.currentTimeMillis() - startMs);
            vvStruct.sort();
            util.writeJson(vvStruct, "out/_centralityRanksSelected.json", true, true);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void exploreCentralityAll() {
        try {
            FileUtil util = new FileUtil();
            VertexValueStruct vvStruct = new VertexValueStruct();
            vvStruct.setFunction("Centrality");
            long startMs = System.currentTimeMillis();
            Map<String, Double> scores = jgraph.centralityRankAll();
            Iterator<String> prAllIt = scores.keySet().iterator();
            while (prAllIt.hasNext()) {
                String vertex = prAllIt.next();
                Double value = scores.get(vertex);
                vvStruct.addRank(vertex, value);
            }
            vvStruct.setElapsedMs(System.currentTimeMillis() - startMs);
            vvStruct.sort();
            util.writeJson(vvStruct, "out/_centralityRanksAll.json", true, true);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void exploreStarNetwork() {
        try {
            FileUtil util = new FileUtil();
            int[] degrees = { 1, 2, 3, 4 };
            for (int i = 0; i < degrees.length; i++) {
                int degree = degrees[i];
                JStarNetwork star = jgraph.starNetworkFor(ImdbConstants.PERSON_KEVIN_BACON, degree);
                EdgesStruct es = star.asEdgesStruct();
                String outfile = "out/_star_network_" + degree + ".json";
                util.writeJson(es, outfile, true, true);
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    private EdgeStruct parseDefaultEdge(DefaultEdge e) {

        if (e != null) {
            String[] tokens = e.toString().split(":");
            if (tokens.length == 2) {
                EdgeStruct struct = new EdgeStruct();
                struct.setV1Value(tokens[0].replace('(', ' ').trim().intern());
                struct.setV2Value(tokens[1].replace(')', ' ').trim().intern());
                return struct;
            }
        }
        return null;
    }

    private String lookupImdbName(String imdbConst) {
        if (imdbConst != null) {
            if (movies.containsKey(imdbConst)) {
                Movie m = movies.get(imdbConst);
                return m.titleWordsJoined();
            }
            if (people.containsKey(imdbConst)) {
                Person p = people.get(imdbConst);
                return p.primaryNameWordsJoined();
            }
        }
        return "?".intern();
    }
}
