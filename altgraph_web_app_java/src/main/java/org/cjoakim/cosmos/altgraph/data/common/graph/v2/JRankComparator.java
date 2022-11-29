package org.cjoakim.cosmos.altgraph.data.common.graph.v2;

import java.util.Comparator;

/**
 * Comparator for sorting JRank objects.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */
public class JRankComparator implements Comparator {

    public int compare(Object obj1, Object obj2) {
        JRank pr1 = (JRank) obj1;
        JRank pr2 = (JRank) obj2;
        return pr1.compareTo(pr2);
    }
}
