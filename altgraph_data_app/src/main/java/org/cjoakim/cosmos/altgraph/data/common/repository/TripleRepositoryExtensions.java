package org.cjoakim.cosmos.altgraph.data.common.repository;


import org.cjoakim.cosmos.altgraph.data.common.model.npm.Triple;

import java.util.ArrayList;

/**
 * This interface was created to extend the TripleRepository, which in turn extends
 * CosmosRepository<Triple, String> from the Cosmos DB Spring Data SDK.
 * <p>
 * This demonstrates how to leverage more of the power of the Cosmos DB SQL syntax, by using
 * "Criteria" objects and an Autowired "CosmosTemplate" object.
 * <p>
 * See class TripleRepositoryExtensionsImpl in this package, which implements this interface.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */
public interface TripleRepositoryExtensions {
    public Iterable<Triple> findByTenantAndLobAndSubjectLabelsIn(String tenant, String lob, ArrayList<String> values);
}
