package org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;

import java.util.ArrayList;
import java.util.Date;

@Data
@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonIgnoreProperties(ignoreUnknown = true)
public class EdgesStruct {

    private String date;
    private long elapsedMs = -1;
    private String doctype = null;
    private String vertex1;
    private String vertex2;
    private ArrayList<EdgeStruct> edges;

    public EdgesStruct() {
        super();
        date = new Date().toString(); // .toGMTString() is deprecated
        edges = new ArrayList<EdgeStruct>();
        setDoctype("EdgesStruct");
    }

    public void addEdge(EdgeStruct edge) {
        if (edge != null) {
            if (edge.isValid()) {
                edge.setSeq(edges.size() + 1);
                edges.add(edge);
            }
        }
    }

    @JsonIgnore
    public String asJson(boolean pretty) throws Exception {

        try {
            ObjectMapper mapper = new ObjectMapper();
            if (pretty) {
                return mapper.writerWithDefaultPrettyPrinter().writeValueAsString(this);
            } else {
                return mapper.writeValueAsString(this);
            }
        } catch (JsonProcessingException e) {
            e.printStackTrace();
            return null;
        }
    }
}
