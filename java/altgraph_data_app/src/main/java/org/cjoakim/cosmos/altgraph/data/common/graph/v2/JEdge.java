package org.cjoakim.cosmos.altgraph.data.common.graph.v2;

import lombok.Data;
import lombok.EqualsAndHashCode;
import org.jgrapht.graph.DefaultEdge;

/**
 * Sample Custom Edge class, containing a source, target, and label.
 * This class is NOT actually used in this project; it is only an example.
 * <p>
 * See https://jgrapht.org/guide/LabeledEdges
 * See https://github.com/jgrapht/jgrapht/blob/master/jgrapht-core/src/main/java/org/jgrapht/graph/DefaultEdge.java
 * See https://github.com/jgrapht/jgrapht/blob/master/jgrapht-core/src/main/java/org/jgrapht/graph/IntrusiveEdge.java
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
public class JEdge extends DefaultEdge {

    public JEdge() {
        super();
    }

    public String s() {
        return (String) this.getSource();
    }

    public String t() {
        return (String) this.getTarget();
    }
}
