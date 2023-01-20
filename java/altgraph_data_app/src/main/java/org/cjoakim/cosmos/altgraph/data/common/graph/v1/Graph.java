package org.cjoakim.cosmos.altgraph.data.common.graph.v1;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;
import lombok.extern.slf4j.Slf4j;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Instances of this class represent an in-memory Graph from a given root node.
 * The Graph is created in-memory by class GraphBuilder from the set of triples;
 * a TripleQueryStruct.  This implementation does NOT use the org.jgrapht library.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@Slf4j
@JsonIgnoreProperties({"startTime", "endTime", "currentKeys"})
public class Graph {

    private String rootKey;
    private HashMap<String, GraphNode> graphMap;

    private long startTime = System.currentTimeMillis();
    private long endTime;
    private long elapsedMs;

    public Graph() {

        super();
        startTime = System.currentTimeMillis();
        graphMap = new HashMap<String, GraphNode>();
    }

    public void setRootNode(String key) {

        rootKey = key;
        GraphNode node = new GraphNode(true, key);
        graphMap.put(key, node);
    }

    public int updateForLibrary(String subjectKey, String objectKey, String predicate) {

        int changeCount = 0;
        GraphNode subjectNode = null;
        GraphNode objectNode = null;

        if (graphMap.containsKey(objectKey)) {
            objectNode = graphMap.get(objectKey);
        } else {
            objectNode = createNewNode(objectKey);
            changeCount++;
        }

        subjectNode = graphMap.get(subjectKey);
        int addAdjResult = subjectNode.addAdjacent(objectNode, predicate);
        changeCount = changeCount + addAdjResult;

        return changeCount;
    }

    public int updateForAuthor(String subjectKey, String objectKey, String predicate) {

        int changeCount = 0;
        GraphNode subjectNode = null;
        GraphNode objectNode = null;

        if (graphMap.containsKey(objectKey)) {
            objectNode = graphMap.get(objectKey);
        } else {
            objectNode = createNewNode(objectKey);
            changeCount++;
        }

        subjectNode = graphMap.get(subjectKey);
        int addAdjResult = subjectNode.addAdjacent(objectNode, predicate);
        changeCount = changeCount + addAdjResult;

        return changeCount;
    }

    public GraphNode createNewNode(String key) {

        GraphNode node = new GraphNode(false, key);
        graphMap.put(key, node);
        return node;
    }

    public ArrayList<String> getCurrentKeys() {

        ArrayList<String> values = new ArrayList<String>();
        Object[] objArray = graphMap.keySet().toArray();
        for (int i = 0; i < objArray.length; i++) {
            values.add(objArray[i].toString());
        }
        return values;
    }

    public void addNode(String key) {

    }

    public long finish() {
        endTime = System.currentTimeMillis();
        elapsedMs = endTime - startTime;
        log.warn("finish() elapsedMs: " + elapsedMs);
        return elapsedMs;
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
