package org.cjoakim.cosmos.altgraph.data.common.graph.v2;

import lombok.Data;
import org.jgrapht.graph.DefaultEdge;

import java.util.HashMap;
import java.util.Set;

/**
 * Instances of this class represents a "degree" as in the 6-degrees of Kevin Bacon)
 * in a JStarNetwork from a given root vertex.
 * <p>
 * The underlying data structure is a hash keyed by vertices, with the corresponding
 * value being the outgoing edges of the vertex.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
public class JStarDegree {

    private int degree;
    private HashMap<String, Set<DefaultEdge>> outgoingEdgesMap;

    public JStarDegree(int degree) {
        this.degree = degree;
        outgoingEdgesMap = new HashMap<String, Set<DefaultEdge>>();
    }

    public void add(String vertex, Set<DefaultEdge> outEdges) {
        outgoingEdgesMap.put(vertex, outEdges);
    }
}
