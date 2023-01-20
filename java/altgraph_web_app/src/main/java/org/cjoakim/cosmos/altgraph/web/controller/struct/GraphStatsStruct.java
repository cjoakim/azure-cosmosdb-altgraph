package org.cjoakim.cosmos.altgraph.web.controller.struct;

import lombok.Data;

import java.util.Date;

/**
 * Instances of this class are serialized as JSON to pass these values to
 * the JavaScript UI.
 * Chris Joakim, Microsoft, November 2022
 */

@Data
public class GraphStatsStruct {

    private String date = new Date().toString();

    // JGraphT graph info
    private int    vertexCount;
    private int    edgeCount;
    private long   elapsedMs;
    private long   refreshMs;
    private Date   refreshDate;
    private String refreshSource;

    // JVM Memory info
    private double epoch;
    private double totalMb;
    private double freeMb;
    private double maxMb;
    private double pctFree;
}
