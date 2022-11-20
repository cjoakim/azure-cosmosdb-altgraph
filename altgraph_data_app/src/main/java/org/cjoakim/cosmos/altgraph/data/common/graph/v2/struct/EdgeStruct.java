package org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;

@Data
@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonIgnoreProperties(value = {"isValid"}, ignoreUnknown = true)
public class EdgeStruct {

    private int seq;
    private int level;
    private String v1Value;
    private String v1Name;
    private String v2Value;
    private String v2Name;

    public EdgeStruct() {
        super();
    }

    public EdgeStruct(String v1, String v2) {
        this();
        this.v1Value = v1;
        this.v2Value = v2;
    }

    public boolean isValid() {
        if (v1Value == null) {
            return false;
        }
        if (v2Value == null) {
            return false;
        }
        if (v1Value.length() < 1) {
            return false;
        }
        if (v2Value.length() < 1) {
            return false;
        }
        return true;
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
