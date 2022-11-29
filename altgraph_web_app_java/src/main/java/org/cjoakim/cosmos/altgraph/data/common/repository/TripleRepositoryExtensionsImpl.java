package org.cjoakim.cosmos.altgraph.data.common.repository;

import com.azure.spring.data.cosmos.core.CosmosTemplate;
import com.azure.spring.data.cosmos.core.query.CosmosQuery;
import com.azure.spring.data.cosmos.core.query.Criteria;
import com.azure.spring.data.cosmos.core.query.CriteriaType;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.DataAppConfiguration;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Triple;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.repository.query.parser.Part;

import java.util.ArrayList;
import java.util.Collections;

/**
 * This class implements the TripleRepositoryExtensions interface and enhances TripleRepository
 * which extends the "out of the box" CosmosRepository<Triple, String> from the CosmosDB Spring Data SDK.
 * <p>
 * This demonstrates how to leverage more of the power of the CosmosDB SQL syntax, by using
 * "Criteria" objects and an Autowired "CosmosTemplate" object.
 * <p>
 * See class TripleRepositoryExtensionsImpl in this package, which implements this interface.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Slf4j
public class TripleRepositoryExtensionsImpl implements TripleRepositoryExtensions {
    private CosmosTemplate template;

    @Autowired
    public TripleRepositoryExtensionsImpl(CosmosTemplate t) {
        super();
        this.template = t;
        log.warn("TripleRepositoryExtensionsImpl constructor, template: " + this.template);
    }

    public Iterable<Triple> findByTenantAndLobAndSubjectLabelsIn(String tenant, String lob, ArrayList<String> values) {

        String containerName = DataAppConfiguration.getCosmosContainerName();
        String pk = "triple|" + tenant;

        Criteria criteria1 = Criteria.getInstance(
                CriteriaType.IS_EQUAL, "pk", Collections.singletonList(pk),
                Part.IgnoreCaseType.NEVER);

        Criteria criteria2 = Criteria.getInstance(
                CriteriaType.IN, "subjectLabel", Collections.singletonList(values),
                Part.IgnoreCaseType.NEVER);

        Criteria allCriteria = Criteria.getInstance(CriteriaType.AND, criteria1, criteria2);

        CosmosQuery query = new CosmosQuery(allCriteria);

        return template.find(query, Triple.class, containerName);
    }
}
