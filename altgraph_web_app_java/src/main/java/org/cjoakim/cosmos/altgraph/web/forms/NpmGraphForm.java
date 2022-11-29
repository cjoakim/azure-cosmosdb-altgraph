package org.cjoakim.cosmos.altgraph.web.forms;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;
import lombok.extern.slf4j.Slf4j;

/**
 * Instances of this class represent the several fields of the Graph HTML FORM
 * that are HTTP POSTed to the NpmGraphController.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@Slf4j
public class NpmGraphForm {

    public static final int DEFAULT_DEPTH = 1;

    private String subjectName;

    private boolean authorCheckbox;

    private String graphDepth;

    private String cacheOpts;

    private String elapsedMs;

    private String sessionId;

    public int getDepthAsInt() {

        try {
            return Integer.parseInt(graphDepth);
        } catch (NumberFormatException e) {
            log.error("non-integer depth value: " + graphDepth + ".  returning the default");
            return DEFAULT_DEPTH;
        }
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
