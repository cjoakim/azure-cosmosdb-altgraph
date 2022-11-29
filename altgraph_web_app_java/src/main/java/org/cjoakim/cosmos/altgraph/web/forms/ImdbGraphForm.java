package org.cjoakim.cosmos.altgraph.web.forms;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.ImdbConstants;

/**
 * Instances of this class represent the several fields of the Graph HTML FORM
 * that are HTTP POSTed to the NpmGraphController.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@Slf4j
public class ImdbGraphForm implements ImdbConstants {

    public static final int DEFAULT_DEPTH = 1;

    private String formFunction;

    private String value1;
    private String value2;

    private String elapsedMs;

    private String sessionId;

    public void translateShortcutValues() {

        if (value1.equalsIgnoreCase("kb")) {
            value1 = PERSON_KEVIN_BACON;
        }
        if (value1.equalsIgnoreCase("cr")) {
            value1 = PERSON_CHARLOTTE_RAMPLING;
        }
        if (value1.equalsIgnoreCase("jr")) {
            value1 = PERSON_JULIA_ROBERTS;
        }
        if (value1.equalsIgnoreCase("jl")) {
            value1 = PERSON_JENNIFER_LAWRENCE;
        }
        if (value1.equalsIgnoreCase("fl")) {
            value1 = MOVIE_FOOTLOOSE;
        }

        //

        if (value2.equalsIgnoreCase("kb")) {
            value2 = PERSON_KEVIN_BACON;
        }
        if (value2.equalsIgnoreCase("cr")) {
            value2 = PERSON_CHARLOTTE_RAMPLING;
        }
        if (value2.equalsIgnoreCase("jr")) {
            value2 = PERSON_JULIA_ROBERTS;
        }
        if (value2.equalsIgnoreCase("jl")) {
            value2 = PERSON_JENNIFER_LAWRENCE;
        }
        if (value2.equalsIgnoreCase("fl")) {
            value2 = MOVIE_FOOTLOOSE;
        }
    }

    public boolean isValue1AnInteger() {

        try {
            Integer.parseInt(getValue1());
            return true;
        } catch (NumberFormatException e) {
            return false;
        }
    }

    public boolean isValue2AnInteger() {

        try {
            Integer.parseInt(getValue2());
            return true;
        } catch (NumberFormatException e) {
            return false;
        }
    }

    public String reloadFlag() {
        if (getValue1().toLowerCase().contains("reload")) {
            return "reload";
        }
        return "no";
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
