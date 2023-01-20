package org.cjoakim.cosmos.altgraph.data.common.graph.v2;

import lombok.Data;

/**
 * Instances of this class are sortable results of JGraphT PageRank calls.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
public class JRank {

    private String key;
    private Double value;

    public JRank(String key, Double value) {
        super();
        this.key = key;
        this.value = value;
    }

    public int compareTo(Object other) {

        JRank otherInstance = (JRank) other;

        if (this.getValue() > otherInstance.getValue()) {
            return -1;
        } else if (this.getValue() < otherInstance.getValue()) {
            return 1;
        } else {
            return 0;
        }
    }
}
