package org.cjoakim.cosmos.altgraph.data.common.util;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;

import java.io.FileOutputStream;
import java.io.OutputStreamWriter;
import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.Iterator;

/**
 * Instances of this class represent a snapshot of JVM memory at a point in time.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@Slf4j
public class MemoryStats implements DataConstants {

    // Class variables:
    private static ArrayList<MemoryStats> history = new ArrayList<MemoryStats>();
    private static boolean shouldRecordHistory = false;

    // Instance variables
    private double epoch;
    private String note;
    private double totalMb;
    private double freeMb;
    private double maxMb;
    private double pctFree;
    private double pctMax;
    private double totalPerDoc;

    public static void setShouldRecordHistory(boolean b) {
        shouldRecordHistory = b;
    }

    public static void displayHistory(String desc) throws Exception {

        sysout("MemoryStats displayHistory:");

        for (int i = 0; i < history.size(); i++) {
            sysout(history.get(i).asJson(false));
        }
    }

    public static void writeHistory(String desc) throws Exception {

        String path = MEMORY_STATS_BASE + desc + ".csv";
        sysout("write memory stats to: " + path);

        try (FileOutputStream out = new FileOutputStream(path);
             OutputStreamWriter writer = new OutputStreamWriter(out)) {

            Iterator<MemoryStats> it = history.iterator();
            int count = 0;
            while (it.hasNext()) {
                count++;
                MemoryStats ms = it.next();
                if (count == 1) {
                    writer.write(ms.asDelimitedHeaderLine(","));
                    writer.write(System.lineSeparator());
                }
                writer.write(ms.asDelimitedDataLine(","));
                writer.write(System.lineSeparator());
            }
        }
    }

    public MemoryStats(String note) {

        super();
        if (note == null) {
            this.note = "";
        } else {
            this.note = note.replace(',', ' ');
        }
        epoch = System.currentTimeMillis();
        totalMb = (double) Runtime.getRuntime().totalMemory() / MB;
        freeMb = (double) Runtime.getRuntime().freeMemory() / MB;
        maxMb = (double) Runtime.getRuntime().maxMemory() / MB;
        pctFree = freeMb / totalMb;
        pctMax = totalMb / maxMb;

        if (shouldRecordHistory) {
            history.add(this);
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

    public void display() {

        NumberFormat df = NumberFormat.getInstance();
        df.setMaximumFractionDigits(0);

        NumberFormat pf = NumberFormat.getInstance();
        df.setMaximumFractionDigits(12);

        sysout("MemoryStats:");
        sysout("  epoch:     " + df.format(epoch));
        sysout("  totalMb:   " + df.format(totalMb));
        sysout("  freeMb:    " + df.format(freeMb));
        sysout("  maxMb:     " + df.format(maxMb));
        sysout("  pctFree:   " + pf.format(pctFree));
        sysout("  pctMax:    " + pf.format(pctMax));
        sysout("  note:      " + note);
    }

    public String asDelimitedHeaderLine(String delim) {

        StringBuffer sb = new StringBuffer();
        sb.append("creationEpoch");
        sb.append(delim);
        sb.append("totalMb");
        sb.append(delim);
        sb.append("freeMb");
        sb.append(delim);
        sb.append("maxMb");
        sb.append(delim);
        sb.append("pctFree");
        sb.append(delim);
        sb.append("pctMax");
        sb.append(delim);
        sb.append("note");
        return sb.toString();
    }

    public String asDelimitedDataLine(String delim) {

        NumberFormat nf = NumberFormat.getInstance();
        nf.setMaximumFractionDigits(0);
        nf.setGroupingUsed(false);

        NumberFormat df = NumberFormat.getInstance();
        df.setMaximumFractionDigits(12);
        df.setGroupingUsed(false);

        StringBuffer sb = new StringBuffer();
        sb.append(nf.format(epoch));
        sb.append(delim);
        sb.append(nf.format(totalMb));
        sb.append(delim);
        sb.append(nf.format(freeMb));
        sb.append(delim);
        sb.append(nf.format(maxMb));
        sb.append(delim);
        sb.append(df.format(pctFree));
        sb.append(delim);
        sb.append(df.format(pctMax));
        sb.append(delim);
        sb.append(note);
        return sb.toString();
    }

    private static void sysout(String s) {
        System.out.println(s);
    }
}
