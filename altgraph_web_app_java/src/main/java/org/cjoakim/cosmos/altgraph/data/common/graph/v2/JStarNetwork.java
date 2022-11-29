package org.cjoakim.cosmos.altgraph.data.common.graph.v2;

import lombok.Data;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.ImdbConstants;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct.EdgeStruct;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct.EdgesStruct;
import org.jgrapht.graph.DefaultEdge;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.Iterator;
import java.util.Set;

/**
 * Instances of this class represents a network that starts from one given
 * vertex, and traverses out from that root vertex one "degree of separation"
 * at a time.  The resulting network shape looks like a "star" with the root
 * vertex in the center.
 * <p>
 * Each "degree" (as in the 6-degrees of Kevin Bacon) is represented as
 * instance of JStarDegree.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@Slf4j
public class JStarNetwork implements ImdbConstants {

    private String rootVertex;
    private int depth;
    private long startMs;
    private long finishMs;

    private ArrayList<JStarDegree> degrees;
    private HashSet<String> visitedSet;
    private HashSet<String> unvisitedSet;

    public JStarNetwork(String root, int depth) {
        super();
        this.startMs = System.currentTimeMillis();
        this.rootVertex = root;
        this.depth = depth;
        this.degrees = new ArrayList<JStarDegree>();
        for (int i = 1; i <= depth; i++) {
            degrees.add(new JStarDegree(i));
        }
        this.visitedSet = new HashSet<String>();
        this.unvisitedSet = new HashSet<String>();
        unvisitedSet.add(rootVertex);
    }

    public void addOutEdgesFor(String vertex, Set<DefaultEdge> outEdges, int degree) {
        visitedSet.add(vertex);
        degrees.get(degree - 1).add(vertex, outEdges);

        Iterator<DefaultEdge> it = outEdges.iterator();
        while (it.hasNext()) {
            DefaultEdge de = it.next();
            String[] values = parseDefaultEdge(de);
            String ev1 = values[0];
            String ev2 = values[1];
            if (!visitedSet.contains(ev1)) {
                unvisitedSet.add(ev1);
            }
            if (!visitedSet.contains(ev2)) {
                unvisitedSet.add(ev2);
            }
        }
    }

    private String[] parseDefaultEdge(DefaultEdge de) {
        String[] result = new String[2];
        result[0] = "?".intern();
        result[1] = "?".intern();

        if (de != null) {
            String[] tokens = de.toString().split(":");
            if (tokens.length == 2) {
                result[0] = tokens[0].replace('(', ' ').trim().intern();
                result[1] = tokens[1].replace(')', ' ').trim().intern();
            }
        }
        return result;
    }

    public boolean containsVertex(String v) {
        if (v != null) {
            return visitedSet.contains(v);
        }
        return false;
    }

    public ArrayList<String> getUnvisitedList() {

        ArrayList<String> list = new ArrayList<String>();
        Iterator<String> it = unvisitedSet.iterator();
        while (it.hasNext()) {
            list.add(it.next());
        }
        return list;
    }

    public void resetUnvisitedSet() {
        this.unvisitedSet = new HashSet<String>();
    }

    public EdgesStruct asEdgesStruct() {
        EdgesStruct edgesStruct = new EdgesStruct();
        edgesStruct.setVertex1(rootVertex);
        edgesStruct.setVertex2("" + getDepth());
        edgesStruct.setElapsedMs(elapsedMs());

        for (int i = 0; i < this.degrees.size(); i++) {
            int level = i + 1;
            JStarDegree jsd = degrees.get(i);
            Iterator<String> vertexIt = jsd.getOutgoingEdgesMap().keySet().iterator();
            while (vertexIt.hasNext()) {
                String v = vertexIt.next();
                Set<DefaultEdge> deSet = jsd.getOutgoingEdgesMap().get(v);
                Iterator<DefaultEdge> edgeIt = deSet.iterator();
                int seq = 0;
                while (edgeIt.hasNext()) {
                    DefaultEdge de = edgeIt.next();
                    String[] vertices = parseDefaultEdge(de);
                    if (vertices.length == 2) {
                        seq++;
                        EdgeStruct es = new EdgeStruct(vertices[0], vertices[1]);
                        es.setSeq(seq);
                        es.setLevel(level);
                        edgesStruct.addEdge(es);
                    }
                }
            }
        }
        return edgesStruct;
    }

    public void display() {
        sysout("JStar Network:");
        sysout("  depth:       " + this.depth);
        sysout("  visitedSize: " + visitedSet.size());
        sysout("  elapsedMs:   " + elapsedMs());
        sysout("  charlotte:   " + visitedSet.contains(PERSON_CHARLOTTE_RAMPLING));
        sysout("  julia:       " + visitedSet.contains(PERSON_JULIA_ROBERTS));
        sysout("  lori:        " + visitedSet.contains(PERSON_LORI_SINGER));
        sysout("  cjoakim:     " + visitedSet.contains("cjoakim"));

        for (int i = 0; i < this.degrees.size(); i++) {
            JStarDegree jsd = degrees.get(i);
            sysout("  degree : " + jsd.getDegree() + " (" + jsd.getOutgoingEdgesMap().size() + ")");
            Iterator<String> vertexIt = jsd.getOutgoingEdgesMap().keySet().iterator();
            while (vertexIt.hasNext()) {
                String v = vertexIt.next();
                sysout("    " + v);
                Set<DefaultEdge> deSet = jsd.getOutgoingEdgesMap().get(v);
                Iterator<DefaultEdge> edgeIt = deSet.iterator();
                int seq = 0;
                while (edgeIt.hasNext()) {
                    seq++;
                    DefaultEdge de = edgeIt.next();
                    //sysout("      " + de);
                }
            }
        }
    }

    public void finish() {
        finishMs = System.currentTimeMillis();
    }

    public long elapsedMs() {
        return finishMs - startMs;
    }

    protected void sysout(String s) {
        System.out.println(s);
    }
}
