package org.cjoakim.cosmos.altgraph.data.common.model.npm;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import lombok.Data;
import lombok.EqualsAndHashCode;
import lombok.NoArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.model.AbstractDocument;

/**
 * This is superclass of classes Author, Library, and Maintainer in the NPM BOM graph.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */
@Data
@EqualsAndHashCode(callSuper = false)
@NoArgsConstructor
@Slf4j
@JsonIgnoreProperties(value = {"cacheKey", "graphKey", "_etag"}, ignoreUnknown = true)
public abstract class NpmDocument extends AbstractDocument {

    private String label;
    private String tenant;
    private String lob;
    private String cacheKey;
    private String graphKey;

    public void populateCacheKey() {

        cacheKey = "" + doctype + "|" + label;
    }

    public void scrubValues() {
        // TODO - implement in subclasses
    }

    public void intern() {
        // TODO - implement in subclasses
    }

    public String calculateGraphKey() {

        StringBuffer sb = new StringBuffer();
        sb.append(this.getDoctype());
        sb.append("^");
        sb.append(this.getLabel());
        sb.append("^");
        sb.append(this.getId());
        sb.append("^");
        sb.append(this.getPk());
        graphKey = sb.toString();
        return graphKey;
    }
}
