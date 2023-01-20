package org.cjoakim.cosmos.altgraph.data.common.util;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;

import java.util.HashMap;

/**
 * Instances of this class can be used for counting/incrementing named values.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
public class Counter {

    private HashMap<String, Long> data = new HashMap<String, Long>();

    public Counter() {

        super();
    }

    public void increment(String key) {

        if (key != null) {
            if (data.containsKey(key)) {
                long currValue = data.get(key);
                data.put(key, currValue + 1);
            } else {
                data.put(key, 1l);
            }
        }
    }

    @JsonIgnore
    public String asJson(boolean pretty) throws Exception {

        try {
            ObjectMapper mapper = new ObjectMapper();
            if (pretty) {
                return mapper.writerWithDefaultPrettyPrinter().writeValueAsString(this.data);
            } else {
                return mapper.writeValueAsString(this.data);
            }
        } catch (JsonProcessingException e) {
            e.printStackTrace();
            return null;
        }
    }
}
