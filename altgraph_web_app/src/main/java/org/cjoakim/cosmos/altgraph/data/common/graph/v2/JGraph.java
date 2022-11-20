package org.cjoakim.cosmos.altgraph.data.common.graph.v2;

import lombok.Data;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.cjoakim.cosmos.altgraph.data.common.ImdbConstants;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct.EdgeStruct;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct.EdgesStruct;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct.VertexValueStruct;
import org.jgrapht.GraphPath;
import org.jgrapht.alg.scoring.KatzCentrality;
import org.jgrapht.alg.scoring.PageRank;
import org.jgrapht.alg.shortestpath.DijkstraShortestPath;
import org.jgrapht.graph.DefaultEdge;

import java.util.*;

/**
 * This class wrappers a JGraphT org.jgrapht.Graph object, and implements
 * graph traversal and lookup methods.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@Slf4j
public class JGraph implements DataConstants, ImdbConstants {

    private String domain;
    private String source;

    private Date refreshDate;
    private long refreshMs;

    private org.jgrapht.Graph<String, DefaultEdge> graph;

    public JGraph(String domain, String source) {
        super();
        this.domain = domain;
        this.source = source;
        refresh();
    }

    public int[] getVertexAndEdgeCounts() {
        int[] counts = new int[2];
        counts[0] = graph.vertexSet().size();
        counts[1] = graph.edgeSet().size();
        return counts;
    }

    /**
     * Find the shortest path with the DijkstraShortestPath class in JGraphT.
     */
    public GraphPath<String, DefaultEdge> getShortestPath(String v1, String v2) {
        log.warn("getShortestPath, v1: " + v1 + " to v2: " + v2);
        long start = System.currentTimeMillis();
        if (!isVertexPresent(v1)) {
            return null;
        }
        if (!isVertexPresent(v2)) {
            return null;
        }
        GraphPath<String, DefaultEdge> path =
                DijkstraShortestPath.findPathBetween(graph, v1, v2);
        long elapsed = System.currentTimeMillis() - start;

        if (path == null) {
            log.warn("path is null");
        } else {
            log.warn("elapsed milliseconds: " + elapsed);
            log.warn("path getLength:       " + path.getLength());
            log.warn("path getStartVertex:  " + path.getStartVertex());
            log.warn("path getEndVertex:    " + path.getEndVertex());
        }
        return path;
    }


    public EdgesStruct getShortestPathAsEdgesStruct(String v1, String v2) {

        long startMs = System.currentTimeMillis();
        GraphPath<String, DefaultEdge> path = getShortestPath(v1, v2);

        if (path != null) {
            EdgesStruct edgesStruct = new EdgesStruct();
            edgesStruct.setElapsedMs(System.currentTimeMillis() - startMs);
            edgesStruct.setVertex1(v1);
            edgesStruct.setVertex2(v2);
            for (DefaultEdge e : path.getEdgeList()) {
                EdgeStruct edgeStruct = parseDefaultEdge(e);
                edgesStruct.addEdge(edgeStruct);
            }
            return edgesStruct;
        } else {
            return null;
        }
    }

    public Set<DefaultEdge> edgesOf(String v) {
        if (isVertexPresent(v)) {
            return graph.edgesOf(v);
        }
        return null;
    }

    public JStarNetwork starNetworkFor(String rootVertex, int degrees) {
        JStarNetwork star = null;
        if (isVertexPresent(rootVertex)) {
            star = new JStarNetwork(rootVertex, degrees);

            for (int d = 1; d <= degrees; d++) {
                ArrayList<String> unvisitedList = star.getUnvisitedList();
                star.resetUnvisitedSet();

                log.warn("networkFor, unvisitedList size: " + unvisitedList.size() + " degree: " + d);
                for (int i = 0; i < unvisitedList.size(); i++) {
                    String v = unvisitedList.get(i);
                    Set<DefaultEdge> edges = graph.edgesOf(v);
                    star.addOutEdgesFor(v, edges, d);
                }
            }
        }
        star.finish();
        return star;
    }

    public Set<DefaultEdge> incomingEdgesOf(String v) {
        if (isVertexPresent(v)) {
            return graph.incomingEdgesOf(v);
        }
        return null;
    }

    public int degreeOf(String v) {
        if (isVertexPresent(v)) {
            return graph.degreeOf(v);
        }
        return -1;
    }

    public int inDegreeOf(String v) {
        if (isVertexPresent(v)) {
            return graph.inDegreeOf(v);
        }
        return -1;
    }

    public Double pageRankForVertex(String v) {
        if (isVertexPresent(v)) {
            PageRank pr = new PageRank(graph);
            return pr.getVertexScore(v);
        }
        return -1.0;
    }

    public Map pageRankForAll() {
        PageRank pr = new PageRank(graph);
        return pr.getScores();
    }

    public ArrayList<JRank> sortedPageRanks(int maxCount) {
        ArrayList<JRank> ranks = new ArrayList<JRank>();
        VertexValueStruct vvStruct = new VertexValueStruct();
        Map<String, Double> scores = this.pageRankForAll();
        Iterator<String> prAllIt = scores.keySet().iterator();
        while (prAllIt.hasNext()) {
            String vertex = prAllIt.next();
            Double value = scores.get(vertex);
            vvStruct.addRank(vertex, value);
        }
        vvStruct.sort();
        for (int i = 0; i < maxCount; i++) {
            ranks.add(vvStruct.getRank(i));
        }
        return ranks;
    }

    public Double centralityOfVertex(String v) {
        if (isVertexPresent(v)) {
            KatzCentrality kc = new KatzCentrality(graph);
            return kc.getVertexScore(v);
        }
        return -1.0;
    }

    public Map centralityRankAll() {
        KatzCentrality kc = new KatzCentrality(graph);
        return kc.getScores();
    }

    public void refresh() {
        long t1 = System.currentTimeMillis();
        org.jgrapht.Graph<String, DefaultEdge> newGraph = null;
        JGraphBuilder builder = new JGraphBuilder(source);
        log.warn("JGraph refresh(), domain: " + domain);

        try {
            if (domain.equalsIgnoreCase(GRAPH_DOMAIN_IMDB)) {
                newGraph = builder.buildImdbGraph();
                if (newGraph != null) {
                    refreshMs = System.currentTimeMillis() - t1;
                    refreshDate = new Date();
                    log.warn("JGraph refresh() - replacing graph with newGraph, elapsed ms: " + refreshMs);
                    graph = newGraph;
                }
            }
        } catch (Exception ex) {
            ex.printStackTrace();
        }
    }

    public boolean isVertexPresent(String v) {
        if (v != null) {
            return graph.containsVertex(v);
        }
        return false;
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
}
