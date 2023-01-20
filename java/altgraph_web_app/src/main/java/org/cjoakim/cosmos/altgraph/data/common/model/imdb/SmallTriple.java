package org.cjoakim.cosmos.altgraph.data.common.model.imdb;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import lombok.Data;
import lombok.EqualsAndHashCode;
import org.cjoakim.cosmos.altgraph.data.common.model.AbstractDocument;

import java.util.ArrayList;

/**
 * Instances of this class represent an RDF-like Triple - with subject, predicate, object.
 * The original implementation of class Triple has more attributes; this one is slimmer
 * for memory-consumption purposes.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonIgnoreProperties(ignoreUnknown = true)
public class SmallTriple extends AbstractDocument {

    private String subjectType;
    private String subjectIdPk;
    private ArrayList<String> subjectTags = null;
    private String predicate;
    private String objectType;
    private String objectIdPk;
    private ArrayList<String> objectTags = null;

    public SmallTriple() {

        super();
    }

    @Override
    public void scrubValues() {

    }

    @Override
    public void intern() {

        baseIntern();

        if (subjectType != null) {
            subjectType = subjectType.intern();
        }
        if (subjectIdPk != null) {
            subjectIdPk = subjectIdPk.intern();
        }
        if (predicate != null) {
            predicate = predicate.intern();
        }
        if (objectType != null) {
            objectType = objectType.intern();
        }
        if (objectIdPk != null) {
            objectIdPk = objectIdPk.intern();
        }

        if (subjectTags != null) {
            ArrayList<String> newSubjectTags = new ArrayList<String>();
            for (int i = 0; i < subjectTags.size(); i++) {
                newSubjectTags.add(subjectTags.get(i).intern());
            }
            subjectTags = newSubjectTags;
        }

        if (objectTags != null) {
            ArrayList<String> newObjectTags = new ArrayList<String>();
            for (int i = 0; i < objectTags.size(); i++) {
                newObjectTags.add(objectTags.get(i).intern());
            }
            objectTags = newObjectTags;
        }
    }
}
