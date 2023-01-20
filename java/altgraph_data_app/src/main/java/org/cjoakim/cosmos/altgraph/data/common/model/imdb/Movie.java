package org.cjoakim.cosmos.altgraph.data.common.model.imdb;

import com.azure.spring.data.cosmos.core.mapping.Container;
import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import lombok.Data;
import lombok.EqualsAndHashCode;
import org.cjoakim.cosmos.altgraph.data.common.model.AbstractDocument;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.Iterator;

/**
 * Instances of this class represent an IMDb Movie.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@Container(containerName = "imdb_graph")
@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonIgnoreProperties(value = {"isAdult", "isFootloose"}, ignoreUnknown = true)
public class Movie extends AbstractDocument {

    private static int minMinutes;

    private String tconst;
    private String titleType;
    private String title;
    private ArrayList<String> titleWords;
    private String isAdult;
    private int year;
    private int minutes;

    public static void setMinMinutes(int min) {
        minMinutes = min;
    }

    private HashSet<String> people = new HashSet<String>();

    public Movie() {

        super();
    }

    public Movie(String[] lineTokens) {

        this();
        if (lineTokens != null) {
            if (lineTokens.length > 8) {
                doctype = "movie";
                tconst = lineTokens[0].strip();
                titleType = lineTokens[1].strip();
                title = lineTokens[2].strip();
                isAdult = lineTokens[4].strip();
                people = new HashSet<String>();
                try {
                    year = Integer.parseInt(lineTokens[5].strip());
                    minutes = Integer.parseInt(lineTokens[7].strip());
                } catch (NumberFormatException e) {
                    // ignore
                }
            }
        }
    }

    @Override
    public void scrubValues() {

    }

    @Override
    public void intern() {

        baseIntern();

        if (tconst != null) {
            tconst = tconst.intern();
        }
        if (titleType != null) {
            titleType = titleType.intern();
        }
        if (title != null) {
            title = title.intern();
        }
        if (isAdult != null) {
            isAdult = isAdult.intern();
        }

        if (title != null) {
            titleWords = new ArrayList<String>();
            String[] tokens = title.split("\\s+");
            for (String token : tokens) {
                if (token.length() > 0) {
                    titleWords.add((token.intern()));
                }
            }
            title = null;
        }

        if (people != null) {
            HashSet<String> newSet = new HashSet<String>();
            Iterator<String> it = people.iterator();
            while (it.hasNext()) {
                String s = it.next();
                if (s.trim().length() > 0) {
                    newSet.add(s.intern());
                }
            }
            people = newSet;
        }
    }

    public void addPerson(String nconst) {

        if (tconst != null) {
            people.add(nconst.strip().toLowerCase());
        }
    }

    public String titleWordsJoined() {

        if (titleWords != null) {
            return String.join(" ", titleWords);
        }
        return null;
    }

    @JsonIgnore
    public boolean include(int y) {

        if (year < y) {
            return false;
        }
        if (minutes < minMinutes) {
            return false;
        }
        if (isAdult == null) {
            return false;
        }
        if (isAdult.equals("1")) {
            return false;
        }
        if (!titleType.equals("movie")) {
            return false;
        }
        return true;
    }

}
