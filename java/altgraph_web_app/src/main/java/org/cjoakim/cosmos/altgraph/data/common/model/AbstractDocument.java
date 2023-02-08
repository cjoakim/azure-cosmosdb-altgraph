package org.cjoakim.cosmos.altgraph.data.common.model;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.springframework.data.annotation.Version;

import java.util.Random;
import java.util.UUID;


/**
 * This is the abstract superclass for the several model classes - Movie, Person, Triple, etc.
 * It has variables and methods for Cosmos DB id and pk attributes, and also JSON serialization.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Data
@JsonInclude(JsonInclude.Include.NON_NULL)
public abstract class AbstractDocument implements DataConstants {

    private static Random rnd = new Random();
    protected String id;
    protected String pk;

    protected String g;  // group, or load-group; a random value from 0 to 99.  A String for intern().
    protected String doctype;

    @Version
    String _etag;  // don't set this to null, use an empty String when loading the DB

    public abstract void scrubValues();

    public abstract void intern();

    public void baseIntern() {

        if (id != null) {
            id = id.intern();
        }
        if (pk != null) {
            pk = pk.intern();
        }
        if (g != null) {
            g = g.intern();
        }
        if (doctype != null) {
            doctype = doctype.intern();
        }
    }

    /**
     * The given idPk value is assumed to be unique (as in the IMDb person and movie constants).
     *
     * @param idPk
     */
    public void setCosmosDbCoordinateAttributes(String idPk, String doctype) {

        if ((idPk == null) || (idPk.trim().equals(""))) {
            this.id = UUID.randomUUID().toString();
            this.pk = "orphan";
            this.g = "" + rnd.nextInt(GROUP_MAX); // range 0 to 99
            return;
        }
        this.id = idPk;
        this.pk = idPk;
        this.doctype = "" + doctype;
        this.g = "" + rnd.nextInt(GROUP_MAX); // range 0 to 99
    }

    public void setCosmosDbSmallTripleCoordinateAttributes() {

        this.id = UUID.randomUUID().toString();
        this.pk = DOCTYPE_SMALL_TRIPLE;
        this.doctype = DOCTYPE_SMALL_TRIPLE;
        this.g = "" + rnd.nextInt(GROUP_MAX); // range 0 to 99
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
}
