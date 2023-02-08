package org.cjoakim.cosmos.altgraph.data.common.model.imdb;

import com.fasterxml.jackson.annotation.JsonInclude;
import lombok.Data;
import lombok.EqualsAndHashCode;
import org.cjoakim.cosmos.altgraph.data.common.model.AbstractDocument;

/**
 * Instances of this class represent an IMDb Principal in a Movie.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@JsonInclude(JsonInclude.Include.NON_NULL)
public class Principal extends AbstractDocument {
    private String tconst;
    private String nconst;
    private String category;

    public Principal() {

        super();
    }

    public Principal(String[] lineTokens) {

        this();
        if (lineTokens != null) {
            if (lineTokens.length > 5) {
                doctype = "principal";
                tconst = lineTokens[0].strip().toLowerCase();
                nconst = lineTokens[2].strip().toLowerCase();
                category = lineTokens[3].strip().toLowerCase();
            }
        }
    }

    public boolean isValid() {

        if (tconst == null) {
            return false;
        }
        if (nconst == null) {
            return false;
        }
        if (tconst.length() < 8) { // tt7600874
            return false;
        }
        if (nconst.length() < 8) { // tt7600874
            return false;
        }
        return true;
    }

    @Override
    public void scrubValues() {

    }

    @Override
    public void intern() {

        baseIntern();

        if (tconst != null) {
            tconst = tconst.intern();
        }
        if (nconst != null) {
            nconst = nconst.intern();
        }
        if (category != null) {
            category = category.intern();
        }
    }

    public boolean hasNecessaryRole() {

        if (category == null) {
            return false;
        }
        if (category.contains("actress")) {
            return true;
        }
        if (category.contains("actor")) {
            return true;
        }
        if (category.contains("director")) {
            return true;
        }
        return false;
    }
}
