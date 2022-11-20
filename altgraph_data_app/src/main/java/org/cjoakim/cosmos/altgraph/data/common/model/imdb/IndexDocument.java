package org.cjoakim.cosmos.altgraph.data.common.model.imdb;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import lombok.Data;
import lombok.EqualsAndHashCode;
import org.cjoakim.cosmos.altgraph.data.common.model.AbstractDocument;

import java.util.ArrayList;
import java.util.UUID;

/**
 * Instances of this class represent an index, or pointer, to another document - the target document.
 * All of these IndexDocuments have the same partition key for their respective target.
 * For example, IndexDocuments with pk of "movie_index" point to corresponding Movie documents.
 * <p>
 * Chris Joakim, November, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonIgnoreProperties(ignoreUnknown = true)
public class IndexDocument extends AbstractDocument {

    private String targetId;
    private String targetPk;
    private String targetDoctype;
    private ArrayList<String> adjacentVertices = new ArrayList<String>();

    public IndexDocument() {

        super();
    }

    public IndexDocument(String pkDoctype, String tgtId, String tgtPk) {

        this();
        this.id = UUID.randomUUID().toString();
        this.pk = pkDoctype;
        this.doctype = pkDoctype;
        this.targetId = tgtId;
        this.targetPk = tgtPk;
    }

    @Override
    public void scrubValues() {

    }

    public void addAdjacentVertex(String idPk) {

        if (idPk != null) {
            adjacentVertices.add(idPk.intern());
        }
    }

    @Override
    public void intern() {

        baseIntern();
        if (targetId != null) {
            targetId = targetId.intern();
        }
        if (targetPk != null) {
            targetPk = targetPk.intern();
        }
    }
}
