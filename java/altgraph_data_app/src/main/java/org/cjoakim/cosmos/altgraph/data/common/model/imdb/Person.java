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
 * Instances of this class represent an IMDb Person.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@EqualsAndHashCode(callSuper = false)
@Container(containerName = "imdb_graph")
@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonIgnoreProperties(value = {"isValid", "scrubValues", "isActorActressDirector", "addTitle"}, ignoreUnknown = true)
public class Person extends AbstractDocument {
    private String nconst;
    private String primaryName;
    private ArrayList<String> primaryNameWords;
    private String birthYear;
    private String deathYear;
    private String primaryProfession;
    private ArrayList<String> primaryProfessionWords;
    private HashSet<String> titles = new HashSet<String>();
    private String titleCount = "0";

    public Person() {

        super();
    }

    public Person(String[] lineTokens) {

        this();
        if (lineTokens != null) {
            if (lineTokens.length > 5) {
                doctype = "person";
                nconst = lineTokens[0].strip();
                primaryName = lineTokens[1].strip();
                birthYear = lineTokens[2].strip();
                deathYear = lineTokens[3].strip();
                primaryProfession = lineTokens[4].strip();
                titles = new HashSet<String>();

//                token: 0 -> nconst
//                token: 1 -> primaryName
//                token: 2 -> birthYear
//                token: 3 -> deathYear
//                token: 4 -> primaryProfession
//                token: 5 -> knownForTitles
//                lineNumber: 2>>> nm0000001      Fred Astaire    1899    1987    soundtrack,actor,miscellaneous  tt0031983,tt0050419,tt0053137,tt0072308 <<<< tokens:6
//                token: 0 -> nm0000001
//                token: 1 -> Fred Astaire
//                token: 2 -> 1899
//                token: 3 -> 1987
//                token: 4 -> soundtrack,actor,miscellaneous
//                token: 5 -> tt0031983,tt0050419,tt0053137,tt0072308
            }
        }
    }

    @JsonIgnore
    public boolean isValid() {

        if (nconst == null) {
            return false;
        }
        if (primaryName == null) {
            return false;
        }
        if (nconst.length() < 8) {
            return false;
        }
        if (primaryName.equals("\\\\N")) {
            return false;
        }
        if (primaryName.length() < 4) {
            return false;
        }
        return true;
    }

    @Override
    public void scrubValues() {

        if (isValid()) {
            if (birthYear.contains("\\N")) {
                birthYear = "";
            }
            if (deathYear.contains("\\N")) {
                deathYear = "";
            }
            if (primaryProfession.contains("\\N")) {  // the raw data contains lowercase valid values
                primaryName = "";
            }
        }
    }

    @Override
    public void intern() {

        baseIntern();

        if (nconst != null) {
            nconst = nconst.intern();
        }
        if (primaryName != null) {
            primaryName = primaryName.intern();
        }
        if (birthYear != null) {
            birthYear = birthYear.intern();
        }
        if (deathYear != null) {
            deathYear = deathYear.intern();
        }
        if (primaryProfession != null) {
            primaryProfession = primaryProfession.intern();
        }
        if (titleCount != null) {
            titleCount = titleCount.intern();
        }

        if (titles != null) {
            HashSet<String> newSet = new HashSet<String>();
            Iterator<String> it = titles.iterator();
            while (it.hasNext()) {
                String s = it.next();
                if (s.trim().length() > 0) {
                    newSet.add(s.intern());
                }
            }
            titles = newSet;
        }

        if (primaryName != null) {
            primaryNameWords = new ArrayList<String>();
            String[] tokens = primaryName.split("\\s+");
            for (String token : tokens) {
                if (token.length() > 0) {
                    primaryNameWords.add((token.intern()));
                }
            }
            primaryName = null;
        }

        if (primaryProfession != null) {
            primaryProfessionWords = new ArrayList<String>();
            String[] tokens = primaryProfession.split("\\,+");
            for (String token : tokens) {
                if (token.length() > 0) {
                    primaryProfessionWords.add((token.intern()));
                }
            }
            primaryProfession = null;
        }
    }

    public String primaryNameWordsJoined() {

        if (primaryNameWords != null) {
            return String.join(" ", primaryNameWords);
        }
        return null;
    }

    @JsonIgnore
    public boolean isActorActressDirector() {

        if (primaryProfession != null) {
            if (primaryProfession.contains("actor")) {
                return true;
            }
            if (primaryProfession.contains("actress")) {
                return true;
            }
            if (primaryProfession.contains("director")) {
                return true;
            }
        }
        return false;
    }

    @JsonIgnore
    public String getShortInfo() {

        return "" + nconst + "," + primaryName;
    }

    public void addTitle(String tconst) {

        if (tconst != null) {
            titles.add(tconst.strip().toLowerCase());
            titleCount = "" + titles.size();
        }
    }
}
