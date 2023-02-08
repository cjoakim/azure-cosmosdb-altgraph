package org.cjoakim.cosmos.altgraph.data.processor;

import lombok.NoArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.Graph;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.GraphBuilder;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.GraphNode;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.TripleQueryStruct;
import org.cjoakim.cosmos.altgraph.data.common.io.FileUtil;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Author;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Library;
import org.springframework.stereotype.Component;

/**
 * Instances of this class are used for ad-hoc testing and development
 * of Graph generation functionality within the (batch) DataCommandLineApp,
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */
@Component
@NoArgsConstructor
@Slf4j
public class GraphProcessor extends AbstractConsoleAppProcess {

    public void process() throws Exception {

        FileUtil fu = new FileUtil();
        TripleQueryStruct struct = fu.readTripleQueryStruct(TRIPLE_QUERY_STRUCT_FILE);
        //log.warn(struct.asJson(true));
        log.warn("TripleQueryStruct documents size: " + struct.getDocuments().size());

        // First build a Library Graph
        Library rootLibrary = readLibraryTediousFixture();
        log.warn("rootLibrary: " + rootLibrary.asJson(true));
        GraphBuilder builder = new GraphBuilder(rootLibrary, struct);
        Graph graph = builder.buildLibraryGraph(50);
        fu.writeJson(graph, NPM_LIBRARY_GRAPH_JSON_FILE, true, true);
        log.warn("Graph root lib dependencies is correct: " + verifyGraph(rootLibrary, graph));

        // First build an Author Graph
        Author author = readAuthorTjHolowaychuk();
        log.warn("author: " + author.asJson(true));
        builder = new GraphBuilder(author, struct);
        graph = builder.buildAuthorGraph(author);
        fu.writeJson(graph, NPM_AUTHOR_GRAPH_JSON_FILE, true, true);
    }

    private Library readLibraryExpressFixture() throws Exception {

        return new FileUtil().readLibrary("data/refined/express.json");
    }

    private Library readLibraryTediousFixture() throws Exception {

        return new FileUtil().readLibrary("data/refined/tedious.json");
    }

    private Author readAuthorTjHolowaychuk() throws Exception {

        return new FileUtil().readAuthor("data/refined/author-tj-holowaychuk.json");
    }

    /**
     * Compare the set of dependencies in the given Library document vs the set
     * of dependencies calculated in the Graph, as created by class GraphBuilder.
     */
    private boolean verifyGraph(Library lib, Graph graph) throws Exception {
        int expectedLibsFound = 0;

        log.warn("verifyGraph, Library: " + lib.asJson(true));

        String rootKey = lib.getGraphKey();
        log.warn("verifyGraph, rootKey: " + rootKey);

        GraphNode rootNode = graph.getGraphMap().get(rootKey);
        log.warn("verifyGraph, Graph rootNode: " + rootNode.asJson(true));

        Object[] libKeys = lib.getDependencies().keySet().toArray();
        Object[] nodeKeys = rootNode.getAdjacentNodes().keySet().toArray();

        for (int i = 0; i < libKeys.length; i++) {
            String libName = (String) libKeys[i];
            String expectedKeyPrefix = "library^" + libName + "^"; // library^depd^bb9c0d1f-52b0-470c-abe4-1681a03b8aa3^depd

            for (int nk = 0; nk < nodeKeys.length; nk++) {
                String nodeDepLibKey = (String) nodeKeys[nk];
                if (nodeDepLibKey.startsWith(expectedKeyPrefix)) {
                    expectedLibsFound++;
                    log.warn("verifyGraph, found: " + expectedKeyPrefix);
                }
            }
        }
        log.warn("verifyGraph, Library dep count:   " + libKeys.length);
        log.warn("verifyGraph, GraphNode dep count: " + nodeKeys.length);
        log.warn("verifyGraph, expectedLibsFound:   " + expectedLibsFound);
        if (libKeys.length == expectedLibsFound) {
            return true;
        }
        return false;
    }
}
