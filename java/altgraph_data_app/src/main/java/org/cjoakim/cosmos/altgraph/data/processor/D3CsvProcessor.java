package org.cjoakim.cosmos.altgraph.data.processor;

import lombok.NoArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.D3CsvBuilder;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.Graph;
import org.cjoakim.cosmos.altgraph.data.common.io.FileUtil;
import org.springframework.stereotype.Component;

/**
 * Instances of this class are used for ad-hoc testing and development
 * of D3 CSV generation functionality within the (batch) DataCommandLineApp,
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Component
@NoArgsConstructor
@Slf4j
public class D3CsvProcessor extends AbstractConsoleAppProcess {

    public void process() throws Exception {

        // First build CSVs for Library Graph with default filenames
        Graph graph = readCapturedLibraryGraph();
        log.warn("graph: " + graph.asJson(true));
        String sessionId = "123"; // + System.currentTimeMillis();
        D3CsvBuilder builder = new D3CsvBuilder(graph);
        builder.buildBillOfMaterialCsv(sessionId, 2);
        builder.finish();

        // Next build CSVs for Library Graph with override filenames
        graph = readCapturedLibraryGraph();
        log.warn("graph: " + graph.asJson(true));
        builder = new D3CsvBuilder(graph);
        builder.setNodesCsvFile("data/graph/library_nodes.csv");
        builder.setEdgesCsvFile("data/graph/library_edges.csv");
        builder.buildBillOfMaterialCsv(sessionId, 2);
        builder.finish();

        // Next build CSVs for an Author Graph  with override filenames
        graph = readCapturedAuthorGraph();
        log.warn("graph: " + graph.asJson(true));
        builder = new D3CsvBuilder(graph);
        builder.setNodesCsvFile("data/graph/author_nodes.csv");
        builder.setEdgesCsvFile("data/graph/author_edges.csv");
        builder.buildBillOfMaterialCsv(sessionId, 2);
        builder.finish();
    }

    private Graph readCapturedLibraryGraph() throws Exception {

        return new FileUtil().readGraph("data/graph/library_graph.json");
    }

    private Graph readCapturedAuthorGraph() throws Exception {

        return new FileUtil().readGraph("data/graph/author_graph.json");
    }

}
