package org.cjoakim.cosmos.altgraph.data.common.repository;

import com.azure.spring.data.cosmos.repository.CosmosRepository;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Person;
import org.springframework.stereotype.Component;
import org.springframework.stereotype.Repository;

/**
 * This class is a Spring Data Repository for CosmosDB IMDb Person documents.
 * Chris Joakim, Microsoft, November 2022
 */

@Component
@Repository
public interface ImdbPersonRepository extends CosmosRepository<Person, String> {

    Iterable<Person> findByIdAndPk(String id, String pk);
}
