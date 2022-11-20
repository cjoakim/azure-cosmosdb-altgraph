package org.cjoakim.cosmos.altgraph.data.common.dao;

import com.azure.cosmos.CosmosClient;
import com.azure.cosmos.CosmosClientBuilder;
import com.azure.cosmos.CosmosContainer;
import com.azure.cosmos.CosmosDatabase;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.DataAppConfiguration;
import org.springframework.context.annotation.Configuration;

/**
 * This is a Data Access Object (DAO) which uses the CosmosDB SDK for Java
 * rather than Spring Data.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Configuration
@Slf4j
public class CosmosSynchDaoOther {

    private CosmosClient client;
    private CosmosDatabase database;
    private CosmosContainer container;

    private String uri;
    private String key;
    private String currentDbName;
    private String currentContainerName = "";

    boolean verbose;

    public CosmosSynchDaoOther() {

        super();
    }

    public CosmosClient initialize(String uri, String key, String dbName, boolean verbose) {

        this.uri = uri;
        this.key = key;
        this.currentDbName = dbName;

        if (verbose) {
            log.warn("uri:     " + uri);
            log.warn("key:     " + key);
            log.warn("dbName:  " + dbName);
            log.warn("regions: " + DataAppConfiguration.getInstance().getPreferredRegions());
        }

        if (client == null) {
            client = new CosmosClientBuilder()
                    .endpoint(uri)
                    .key(key)
                    .preferredRegions(DataAppConfiguration.getInstance().getPreferredRegions())
                    .buildClient();
            log.warn("client: " + client);

            database = client.getDatabase(this.currentDbName);
            log.warn("client connected to database Id: " + database.getId());
        }
        return client;
    }

    public void close() {

        if (client != null) {
            log.warn("closing...");
            client.close();
            log.warn("closed");
        }
    }

    public void setCurrentContainer(String c) {

        if (this.currentContainerName.equalsIgnoreCase(c)) {
            return;
        } else {
            container = database.getContainer(c);
            this.currentContainerName = c;
        }
    }

//    public TelemetryQueryResults getAllTelemetry() {
//
//        String sql = "select * from c offset 0 limit 3000";
//        int    pageSize = 100;
//        String continuationToken = null;
//        CosmosQueryRequestOptions queryOptions = new CosmosQueryRequestOptions();
//        TelemetryQueryResults resultsStruct = new TelemetryQueryResults(sql);
//        resultsStruct.start();
//        // Execute the SQL query and iterate the paginated result set,
//        // collecting the documents and total RU charge.
//        do {
//            Iterable<FeedResponse<TelemetryEvent>> feedResponseIterator =
//                    container.queryItems(sql, queryOptions, TelemetryEvent.class)
//                            .iterableByPage(continuationToken, pageSize);
//
//            for (FeedResponse<TelemetryEvent> page : feedResponseIterator) {
//                for (TelemetryEvent doc : page.getResults()) {
//                    resultsStruct.addDocument(doc);
//                }
//                resultsStruct.addRequestCharge(page.getRequestCharge());
//                resultsStruct.incrementPageCount();
//                continuationToken = page.getContinuationToken();
//            }
//        }
//        while (continuationToken != null);
//        resultsStruct.stop();
//        return resultsStruct;
//    }

//    public TelemetryQueryResults countAllTelemetry() {
//
//        String sql = "select count(1) as count from c";
//        int    pageSize = 100;
//        String continuationToken = null;
//        CosmosQueryRequestOptions queryOptions = new CosmosQueryRequestOptions();
//        TelemetryQueryResults resultsStruct = new TelemetryQueryResults(sql);
//        resultsStruct.start();
//
//        Iterable<FeedResponse<JsonNode>> feedResponseIterator =
//                container.queryItems(sql, queryOptions, JsonNode.class)
//                        .iterableByPage(continuationToken, 1);
//
//        // only one result in one page is expected here
//        for (FeedResponse<JsonNode> page : feedResponseIterator) {
//            for (JsonNode node : page.getResults()) {
//                log.warn("Count: " + node.toString());  // Count: {"count":1000} or Count: {"$1":1000}
//                long count = Long.parseLong(node.get("count").asText());
//                resultsStruct.setDocumentCount(count);
//            }
//            resultsStruct.addRequestCharge(page.getRequestCharge());
//            resultsStruct.incrementPageCount();
//        }
//        resultsStruct.stop();
//        return resultsStruct;
//    }
}
