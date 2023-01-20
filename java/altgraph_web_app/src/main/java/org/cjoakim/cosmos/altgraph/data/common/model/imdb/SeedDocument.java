package org.cjoakim.cosmos.altgraph.data.common.model.imdb;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import lombok.Data;
import lombok.EqualsAndHashCode;
import org.cjoakim.cosmos.altgraph.data.common.model.AbstractDocument;

import java.util.ArrayList;
import java.util.UUID;

/**
 * Instances of this class represent "seed data" that the in-memory JGraphT is built from.
 * All of these SeedDocuments have the same partition key for their respective target.
 * For example, SeedDocuments with pk of "movie_seed" (DOCTYPE_MOVIE_SEED) correspond
 * to IMDb Movie documents.
 *
 * See method 'loadImdbGraphFromCosmos' of class JGraphBuilder where these documents
 * are read from Cosmos DB to load the in-memory graph.
 *
 * Chris Joakim, November, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonIgnoreProperties(ignoreUnknown = true)
public class SeedDocument extends AbstractDocument {

    private String targetId;
    private String targetPk;
    private String targetDoctype;
    private ArrayList<String> adjacentVertices = new ArrayList<String>();

    public SeedDocument() {

        super();
    }

    public SeedDocument(String pkDoctype, String tgtId, String tgtPk) {

        this();
        this.id = UUID.randomUUID().toString();
        this.pk = pkDoctype;          // note that doctype and pk (partition key) are the same value
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
