package org.cjoakim.cosmos.altgraph.data.common.graph.v1;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Triple;

import java.util.ArrayList;

/**
 * This is the primary data structure in AltGraph V1.  It contains a set of RDF-like "Triples"
 * that describe the structure of a business graph.  The Triples can be read efficiently from
 * Cosmos DB as they are small documents and reside in the same logical partition.
 * <p>
 * They can be kept in-memory, and/or cached to either local disk or Redis cache.  The Triples can be
 * navigated/traversed in-memory in an orders-of-magnitide faster manner than reading from a database.
 * <p>
 * See file data/data/samples/TripleQueryStruct.json in this GitHub repository; it is a JSON
 * representation of this data structure.
 * <p>
 * This implementation does NOT use the org.jgrapht library.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@NoArgsConstructor
@Slf4j
@JsonIgnoreProperties({"startTime", "endTime"})
public class TripleQueryStruct {

    private String structType = this.getClass().getName();
    private String containerName;
    private String sql;
    private long startTime = System.currentTimeMillis();
    private long endTime;
    private long elapsedMs;
    private long pageCount;
    private double requestCharge;

    private long documentCount;
    ArrayList<Triple> documents = new ArrayList<Triple>();

    public void reset(boolean newList) {
        if (newList) {
            documents = new ArrayList<Triple>();
        }
        startTime = System.currentTimeMillis();
    }

    public void incrementPageCount() {
        pageCount++;
    }

    public void incrementRuCharge(double ru) {
        requestCharge = requestCharge + ru;
    }

    public void addDocument(Triple t) {
        documents.add(t);
    }

    public long start() {
        startTime = System.currentTimeMillis();
        return startTime;
    }

    public long stop() {
        endTime = System.currentTimeMillis();
        elapsedMs = endTime - startTime;
        documentCount = documents.size();
        return elapsedMs;
    }

    public Triple find(String id, String pk, String tenant) {

        for (int i = 0; i < documents.size(); i++) {
            Triple t = documents.get(i);
            if (t.getId().equals(id)) {

                if (t.getTenant().equals(tenant)) {
                    return t;
                }
            }
        }
        return null;
    }

    public String asJson(boolean pretty) throws Exception {

        try {
            ObjectMapper mapper = new ObjectMapper();
            if (pretty) {
                return mapper.writerWithDefaultPrettyPrinter().writeValueAsString(this);
            } else {
                return mapper.writeValueAsString(this);
            }
        } catch (JsonProcessingException e) {
            return null;
        }
    }
}
