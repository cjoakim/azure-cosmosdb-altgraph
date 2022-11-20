package org.cjoakim.cosmos.altgraph.data.common.model.npm;

import com.azure.spring.data.cosmos.core.mapping.Container;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import lombok.Data;
import lombok.EqualsAndHashCode;

import java.util.HashMap;

/**
 * Instances of this class represent a NPM (Node.js Package Manager) Library.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@Container(containerName = "npm_graph")
@JsonIgnoreProperties(ignoreUnknown = true)
@EqualsAndHashCode(callSuper = false)
public class Library extends NpmDocument {

    public Library() {
        super();
    }

    private String name;
    private String desc;
    private String[] keywords;
    private HashMap<String, String> dependencies;
    private HashMap<String, String> devDependencies;
    private String author;
    private String[] maintainers;
    private String version;
    private String[] versions;
    private String homepage;

    private int library_age_days;
    private int version_age_days;

    @Override
    public void intern() {

        baseIntern();
    }

}
