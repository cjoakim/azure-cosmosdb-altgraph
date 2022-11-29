package org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.JRank;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.JRankComparator;

import java.util.ArrayList;
import java.util.Collections;

@Data
public class VertexValueStruct {

    private long elapsedMs = -1;
    private String doctype = null;
    private String function = null;

    //private HashMap<String, Double> ranks;
    private ArrayList<JRank> ranks;

    public VertexValueStruct() {
        super();
        ranks = new ArrayList<JRank>();
        setDoctype("VertexValueStruct");
    }

    public void addRank(String vertex, Double value) {
        if (vertex != null) {
            ranks.add(new JRank(vertex, value));
        }
    }

    public JRank getRank(int idx) {
        return ranks.get(idx);
    }

    public void sort() {
        Collections.sort(ranks, new JRankComparator());
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
