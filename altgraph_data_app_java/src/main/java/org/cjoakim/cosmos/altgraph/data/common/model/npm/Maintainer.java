package org.cjoakim.cosmos.altgraph.data.common.model.npm;

import com.azure.spring.data.cosmos.core.mapping.Container;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import lombok.Data;
import lombok.EqualsAndHashCode;

/**
 * Instances of this class represent a NPM (Node.js Package Manager) library Maintainer.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@Container(containerName = "npm_graph")
@JsonIgnoreProperties(ignoreUnknown = true)
public class Maintainer extends NpmDocument {

    public Maintainer() {
        super();
    }

    @Override
    public void intern() {

        baseIntern();
    }
}
