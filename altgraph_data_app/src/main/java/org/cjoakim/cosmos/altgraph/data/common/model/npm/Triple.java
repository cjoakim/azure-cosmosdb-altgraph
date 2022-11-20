package org.cjoakim.cosmos.altgraph.data.common.model.npm;

import com.azure.spring.data.cosmos.core.mapping.Container;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import lombok.Data;
import lombok.EqualsAndHashCode;
import lombok.NoArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;

import java.util.ArrayList;


/**
 * Instances of this class represent the conceptual equivalent of an RDF Triplestore "Triple".
 * RDF triples contain a subject, predicate, and object.  Similarly, this class contains
 * several subject, predicate, and object attributes to enable the efficient representation
 * and navigation of the graph.  The size of the Triple object is designed to be as small
 * as possible to enable fast and efficient in-memory processing.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@NoArgsConstructor
@Slf4j
@Container(containerName = "npm_graph")
@JsonIgnoreProperties(ignoreUnknown = true)
@JsonInclude(JsonInclude.Include.NON_NULL)
public class Triple extends NpmDocument implements DataConstants {

    private String tenant;
    private String lob;
    private String doctype;
    private String subjectType;
    private String subjectLabel;
    private String subjectId;
    private String subjectPk;
    private String subjectKey;

    private ArrayList<String> subjectTags = new ArrayList<String>();

    private String predicate;

    private String objectType;
    private String objectLabel;
    private String objectId;
    private String objectPk;
    private String objectKey;

    private ArrayList<String> objectTags = new ArrayList<String>();

    @Override
    public void intern() {

        baseIntern();
    }

    public void setKeyFields() {
        subjectKey = subjectType + "^" + subjectLabel + "^" + subjectId + "^" + subjectPk;
        objectKey = objectType + "^" + objectLabel + "^" + objectId + "^" + objectPk;
    }

    public void addSubjectTag(String tag) {

        if (tag != null) {
            subjectTags.add(tag.trim());
        }
    }

    public void addObjectTag(String tag) {

        if (tag != null) {
            objectTags.add(tag.trim());
        }
    }

}
