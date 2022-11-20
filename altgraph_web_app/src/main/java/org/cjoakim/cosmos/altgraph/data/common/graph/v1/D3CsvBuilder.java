package org.cjoakim.cosmos.altgraph.data.common.graph.v1;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.cjoakim.cosmos.altgraph.data.common.io.FileUtil;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;

/**
 * Instances of this class are created in the Web Application to create
 * the two necessary CSV files for D3.js to create a Graph visualization with,
 * from the given Graph object.  Used in the NPM graph.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@Slf4j
@JsonInclude(JsonInclude.Include.NON_NULL)
public class D3CsvBuilder implements DataConstants {

    private Graph graph;
    private ArrayList<String> nodesCsvLines = new ArrayList<String>();
    private ArrayList<String> edgeCsvLines = new ArrayList<String>();

    private HashMap<String, String> collectedNodesHash = new HashMap<String, String>();
    private int collectedNodesCount;
    private HashMap<String, String> collectedEdgesHash = new HashMap<String, String>();
    private int collectedEdgesCount;

    private String nodesCsvFile = null;
    private String edgesCsvFile = null;

    int iterationCount = 0;

    public D3CsvBuilder(Graph g) {
        super();
        graph = g;
        nodesCsvFile = NPM_GRAPH_NODES_CSV_FILE;
        edgesCsvFile = NPM_GRAPH_EDGES_CSV_FILE;
    }

    public void buildBillOfMaterialCsv(String sessionId, int depth) throws Exception {

        collectDataFromGraph(depth);
        buildNodesCsv();
        buildEdgesCsv();
        writeCsvFiles();
    }

    private void collectDataFromGraph(int depth) {

        boolean continueToCollect = true;
        String rootKey = graph.getRootKey();
        log.warn("collectLibrariesDataFromGraph rootKey: " + rootKey);

        String rootLib = extractNameFromKey(rootKey);
        log.warn("collectLibrariesDataFromGraph rootLib: " + rootLib);

        String rootEdgeKey = edgeKey(rootLib, rootLib);
        log.warn("collectLibrariesDataFromGraph rootHashKey: " + rootEdgeKey);

        collectedNodesHash.put(rootKey, "pending");
        collectedEdgesHash.put(rootEdgeKey, "");

        while (continueToCollect) {
            iterationCount++;
            Object[] currentKeys = collectedNodesHash.keySet().toArray();

            for (int i = 0; i < currentKeys.length; i++) {
                String currKey = (String) currentKeys[i];
                String currVal = collectedNodesHash.get(currKey);
                String currLib = extractNameFromKey(currKey);

                if (currVal.equalsIgnoreCase("pending")) {
                    GraphNode node = graph.getGraphMap().get(currKey);
                    Object[] dependencyKeys = node.getAdjacentNodes().keySet().toArray();
                    for (int d = 0; d < dependencyKeys.length; d++) {
                        String depKey = (String) dependencyKeys[d];
                        if (depKey.startsWith("library")) {
                            if (collectedNodesHash.containsKey(depKey)) {
                                //log.warn("already in collectedNodesHash: " + depKey);
                            } else {
                                collectedNodesHash.put(depKey, "pending");
                                String depLib = extractNameFromKey(depKey);
                                String depEdgeKey = edgeKey(currLib, depLib);
                                collectedEdgesHash.put(depEdgeKey, "");
                            }
                        }
                    }
                    collectedNodesHash.put(currKey, "processed");
                }
            }

            // terminate the while-loop as necessary
            if (iterationCount == depth) {
                continueToCollect = false;
                //log.error("buildBillOfMaterialCsv while loop terminating at depth: " + iterationCount);
            }
            if (iterationCount >= 99) {
                continueToCollect = false;  // possible infinite loop, eject!
                log.error("buildBillOfMaterialCsv while loop bailing out at iterationCount " + iterationCount);
            }
        }
    }

    private void buildNodesCsv() {
        nodesCsvLines.add("name,type,adjCount");  // header row
        ArrayList<String> keys = sortedArray(collectedNodesHash.keySet().toArray());
        for (int i = 0; i < keys.size(); i++) {
            String key = keys.get(i);
            String libName = extractNameFromKey(key);
            GraphNode node = graph.getGraphMap().get(key);
            nodesCsvLines.add(libName + ",library," + node.getAdjacentNodes().size());
        }
        log.warn("buildNodesCsv count: " + nodesCsvLines.size());
    }

    private void buildEdgesCsv() {
        edgeCsvLines.add("source,target,weight");  // header row
        ArrayList<String> keys = sortedArray(collectedEdgesHash.keySet().toArray());
        for (int i = 0; i < keys.size(); i++) {
            String key = keys.get(i);
            String[] tokens = key.split(" ");
            edgeCsvLines.add(tokens[0] + "," + tokens[1] + ",1");
        }
        log.warn("buildEdgesCsv count: " + edgeCsvLines.size());
    }

    private ArrayList<String> sortedArray(Object[] array) {

        ArrayList<String> strings = new ArrayList<>();
        for (int i = 0; i < array.length; i++) {
            strings.add((String) array[i]);
        }
        Collections.sort(strings);
        return strings;
    }

    private String edgeKey(String parentLibName, String childLibName) {

        return parentLibName + " " + childLibName;
    }

    private String extractNameFromKey(String key) {
        //String name = key.replace("^", " ").split(" ")[1];
        String name = key.replace("^", ":").split(":")[1].replace(" ", "_");

        // log.warn("extractNameFromKey: " + key + " -> " + name);
        // library^express^bf8cff83-5f7c-4995-8484-d2f405bcbce7^express -> express
        // author^TJ Holowaychuk <tj@vision-media.ca>^54dff427-35de-4a13-bcad-b3e4124b303a^TJ Holowaychuk <tj@vision-media.ca> -> TJ Holowaychuk <tj@vision-media.ca>

        return name;
    }

    private void writeCsvFiles() throws Exception {

        // GRAPH_NODES_CSV_FILE
        FileUtil fu = new FileUtil();
        fu.writeLines(nodesCsvFile, nodesCsvLines, true);
        fu.writeLines(edgesCsvFile, edgeCsvLines, true);
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

    public void finish() {

        this.graph = null;
        collectedNodesCount = collectedNodesHash.size();
        collectedEdgesCount = collectedEdgesHash.size();
    }
}
