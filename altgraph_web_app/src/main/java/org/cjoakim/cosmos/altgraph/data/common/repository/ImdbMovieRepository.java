package org.cjoakim.cosmos.altgraph.data.common.repository;

import com.azure.spring.data.cosmos.repository.CosmosRepository;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Movie;
import org.springframework.stereotype.Component;
import org.springframework.stereotype.Repository;

/**
 * This class is a Spring Data Repository for Cosmos DB IMDb Movie documents.
 * Chris Joakim, Microsoft, November 2022
 */

@Component
@Repository
public interface ImdbMovieRepository extends CosmosRepository<Movie, String> {

    Iterable<Movie> findByIdAndPk(String id, String pk);
}
