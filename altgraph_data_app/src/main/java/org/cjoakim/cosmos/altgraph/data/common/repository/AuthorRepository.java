package org.cjoakim.cosmos.altgraph.data.common.repository;

import com.azure.spring.data.cosmos.repository.CosmosRepository;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Author;
import org.springframework.stereotype.Component;
import org.springframework.stereotype.Repository;

/**
 * This class is a Spring Data Repository for CosmosDB NPM Author documents.
 * Chris Joakim, Microsoft, November 2022
 */

@Component
@Repository
public interface AuthorRepository extends CosmosRepository<Author, String> {

    Iterable<Author> findByLabel(String label);
}
