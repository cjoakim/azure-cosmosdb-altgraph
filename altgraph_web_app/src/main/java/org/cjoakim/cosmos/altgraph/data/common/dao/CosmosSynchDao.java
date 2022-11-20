package org.cjoakim.cosmos.altgraph.data.common.dao;

import com.azure.cosmos.CosmosClient;
import com.azure.cosmos.CosmosClientBuilder;
import com.azure.cosmos.CosmosContainer;
import com.azure.cosmos.CosmosDatabase;
import com.azure.cosmos.models.CosmosQueryRequestOptions;
import com.azure.cosmos.models.FeedResponse;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.TripleQueryStruct;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Triple;

import java.util.ArrayList;

/**
 * This is a Data Access Object (DAO) which uses the CosmosDB SDK for Java
 * rather than Spring Data.  This class isn't used in the web application,
 * it is just for ad-hoc and exploratory purposes.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Slf4j
public class CosmosSynchDao {

    private CosmosClient client;
    private CosmosDatabase database;
    private CosmosContainer container;

    private String uri;
    private String key;
    private String currentDbName;
    private String currentContainerName = "";

    public CosmosSynchDao() {

        super();
    }

    public CosmosClient initialize(String uri, String key, String dbName) {

        this.uri = uri;
        this.key = key;
        this.currentDbName = dbName;

        if (client == null) {
            log.warn("getClient, uri: " + uri + " key: " + key + " currentDbName: " + currentDbName);

            client = new CosmosClientBuilder()
                    .endpoint(uri)
                    .key(key)
                    .buildClient();
            log.warn("client: " + client);

            database = client.getDatabase(this.currentDbName);
            log.warn("database Id: " + database.getId());
        }
        return client;
    }

    public TripleQueryStruct queryTriples(TripleQueryStruct struct) {

        CosmosQueryRequestOptions queryOptions = new CosmosQueryRequestOptions();
        String continuationToken = null;
        int pageSize = 10000;
        struct.reset(false);
        setCurrentContainer(struct.getContainerName());
        struct.reset(true);

        do {
            Iterable<FeedResponse<Triple>> feedResponseIterator =
                    container.queryItems(struct.getSql(), queryOptions, Triple.class)
                            .iterableByPage(continuationToken, pageSize);

            for (FeedResponse<Triple> page : feedResponseIterator) {
                for (Triple doc : page.getResults()) {
                    struct.addDocument(doc);
                }
                struct.incrementPageCount();
                double charge = page.getRequestCharge();
                struct.incrementRuCharge(charge);
                log.warn("pageNum: " + struct.getPageCount() + ", page.getRequestCharge(): " + charge);
                continuationToken = page.getContinuationToken();
            }
        }
        while (continuationToken != null);
        struct.stop();
        return struct;
    }

    public ArrayList<Triple> getTriplesV2() {

        ArrayList<Triple> documents = new ArrayList<>();
        setCurrentContainer("graph");
        String sql = "select * from c where c.doctype = 'triple' offset 0 limit 1000";
        int pageSize = 100;
        int currentPageNumber = 1;
        int documentNumber = 0;
        String continuationToken = null;

        double requestCharge = 0.0;
        CosmosQueryRequestOptions queryOptions = new CosmosQueryRequestOptions();

        // First iteration (continuationToken = null): Receive a batch of query response pages
        // Subsequent iterations (continuationToken != null): Receive subsequent batch of query response pages, with continuationToken indicating where the previous iteration left off
        do {
            Iterable<FeedResponse<Triple>> feedResponseIterator =
                    container.queryItems(sql, queryOptions, Triple.class)
                            .iterableByPage(continuationToken, pageSize);

            for (FeedResponse<Triple> page : feedResponseIterator) {
                for (Triple doc : page.getResults()) {
                    documents.add(doc);
                }
                requestCharge += page.getRequestCharge();
                continuationToken = page.getContinuationToken();
                currentPageNumber++;
            }
        }
        while (continuationToken != null);

        return documents;
    }

    public void close() {

        if (client != null) {
            log.warn("closing...");
            client.close();
            log.warn("closed");
        }
    }

    private void setCurrentContainer(String c) {

        if (this.currentContainerName.equalsIgnoreCase(c)) {
            return;
        } else {
            container = database.getContainer(c);
            this.currentContainerName = c;
        }
    }

}
