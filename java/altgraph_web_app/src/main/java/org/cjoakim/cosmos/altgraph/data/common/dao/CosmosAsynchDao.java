package org.cjoakim.cosmos.altgraph.data.common.dao;

import com.azure.cosmos.*;
import com.azure.cosmos.models.CosmosBulkOperations;
import com.azure.cosmos.models.CosmosItemOperation;
import com.azure.cosmos.models.PartitionKey;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.DataAppConfiguration;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.SeedDocument;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Movie;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Person;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.SmallTriple;
import reactor.core.publisher.Flux;

import java.util.ArrayList;

/**
 * This is a Data Access Object (DAO) which uses the Cosmos DB SDK for Java
 * rather than Spring Data.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Slf4j
public class CosmosAsynchDao {

    private CosmosAsyncClient client;
    private CosmosAsyncDatabase database;
    private CosmosAsyncContainer container;

    private String uri;
    private String key;
    private String currentDbName;
    private String currentContainerName = "";

    boolean verbose;

    public CosmosAsynchDao() {

        super();
    }

    public CosmosAsyncClient initialize(
            String uri, String key, String dbName, boolean verbose) {

        this.uri = uri;
        this.key = key;
        this.currentDbName = dbName;

        if (verbose) {
            log.warn("uri:     " + uri);
            log.warn("key:     " + key);
            log.warn("dbName:  " + dbName);
        }

        if (client == null) {
            ArrayList<String> prefRegions = DataAppConfiguration.getPreferredRegions();
            if (prefRegions.size() > 0) {
                client = new CosmosClientBuilder()
                        .endpoint(uri)
                        .key(key)
                        .preferredRegions(prefRegions)
                        .consistencyLevel(ConsistencyLevel.SESSION)
                        .contentResponseOnWriteEnabled(true)
                        .buildAsyncClient();
            }
            else {
                client = new CosmosClientBuilder()
                        .endpoint(uri)
                        .key(key)
                        .consistencyLevel(ConsistencyLevel.SESSION)
                        .contentResponseOnWriteEnabled(true)
                        .buildAsyncClient();
            }
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

    public String getCurrentContainerName() {

        return currentContainerName;
    }

    public void setCurrentContainer(String c) {

        if (this.currentContainerName.equalsIgnoreCase(c)) {
            return;
        } else {
            container = database.getContainer(c);
            this.currentContainerName = c;
        }
    }

    public void bulkLoadMovies(Flux<Movie> movies) {

        Flux<CosmosItemOperation> cosmosItemOperations = movies.map(
                movie -> CosmosBulkOperations.getCreateItemOperation(
                        movie, new PartitionKey(movie.getPk())));
        container.executeBulkOperations(cosmosItemOperations).blockLast();
    }

    public void bulkLoadPeople(Flux<Person> people) {

        Flux<CosmosItemOperation> cosmosItemOperations = people.map(
                person -> CosmosBulkOperations.getCreateItemOperation(
                        person, new PartitionKey(person.getPk())));
        container.executeBulkOperations(cosmosItemOperations).blockLast();
    }

    public void bulkLoadSmallTriples(Flux<SmallTriple> triples) {

        Flux<CosmosItemOperation> cosmosItemOperations = triples.map(
                triple -> CosmosBulkOperations.getCreateItemOperation(
                        triple, new PartitionKey(triple.getPk())));
        container.executeBulkOperations(cosmosItemOperations).blockLast();
    }

    public void bulkLoadIndexDocuments(Flux<SeedDocument> idxDocs) {

        Flux<CosmosItemOperation> cosmosItemOperations = idxDocs.map(
                idxDoc -> CosmosBulkOperations.getCreateItemOperation(
                        idxDoc, new PartitionKey(idxDoc.getPk())));
        container.executeBulkOperations(cosmosItemOperations).blockLast();
    }
}
