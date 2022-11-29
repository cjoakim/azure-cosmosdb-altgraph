package org.cjoakim.cosmos.altgraph.data.common.graph.v1;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import java.util.HashMap;

/**
 * Instances of this class represent a node within an in-memory Graph.
 * This implementation does NOT use the org.jgrapht library.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@Slf4j
@NoArgsConstructor
@JsonIgnoreProperties({"addAdjacent"})
public class GraphNode {

    private String tripleKey;

    private int adjacentNodeCount = 0;

    private HashMap<String, String> adjacentNodes = new HashMap<String, String>();

    public GraphNode(String key) {
        super();
        this.tripleKey = key;
    }

    public GraphNode(boolean root, String key) {
        super();
        this.tripleKey = key;
    }

    public int addAdjacent(GraphNode neighbor, String predicate) {

        if (this.adjacentNodes.containsKey(neighbor.getTripleKey())) {
            //log.warn("addAdjacent()_present: " + neighbor.getTripleKey() + " to " + this.getTripleKey());
            return 0;
        } else {
            //log.warn("addAdjacent()_adding:  " + neighbor.getTripleKey() + " to " + this.getTripleKey());
            adjacentNodes.put(neighbor.getTripleKey(), predicate);
            adjacentNodeCount = adjacentNodes.size();
            return 1;
        }
    }

    public String asJson(boolean pretty) throws Exception {
        try {
            ObjectMapper mapper = new ObjectMapper();
            if (pretty) {
                return mapper.writerWithDefaultPrettyPrinter().writeValueAsString(this);
            } else {
                return mapper.writeValueAsString(this);
            }
        } catch (JsonProcessingException e) {
            return null;
        }
    }
}
