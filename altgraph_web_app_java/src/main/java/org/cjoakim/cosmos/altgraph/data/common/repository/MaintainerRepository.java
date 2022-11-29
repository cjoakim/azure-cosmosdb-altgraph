package org.cjoakim.cosmos.altgraph.data.common.repository;

import com.azure.spring.data.cosmos.repository.CosmosRepository;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Maintainer;
import org.springframework.stereotype.Component;
import org.springframework.stereotype.Repository;

/**
 * This class is a Spring Data Repository for CosmosDB NPM Maintainer documents.
 * Chris Joakim, Microsoft, November 2022
 */

@Component
@Repository
public interface MaintainerRepository extends CosmosRepository<Maintainer, String> {

}
