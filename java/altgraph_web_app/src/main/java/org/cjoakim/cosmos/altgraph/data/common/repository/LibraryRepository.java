package org.cjoakim.cosmos.altgraph.data.common.repository;

import com.azure.spring.data.cosmos.repository.CosmosRepository;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Library;
import org.springframework.stereotype.Component;
import org.springframework.stereotype.Repository;

/**
 * This class is a Spring Data Repository for Cosmos DB NPM Library documents.
 * Chris Joakim, Microsoft, November 2022
 */

@Component
@Repository
public interface LibraryRepository extends CosmosRepository<Library, String> {

    Iterable<Library> findByPkAndTenant(String pk, String tenant);

    Iterable<Library> findByPkAndTenantAndDoctype(String pk, String tenant, String doctype);

}
