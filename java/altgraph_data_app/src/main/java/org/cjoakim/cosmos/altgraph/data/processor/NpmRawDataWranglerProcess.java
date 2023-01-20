package org.cjoakim.cosmos.altgraph.data.processor;

import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectReader;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.DataAppConfiguration;
import org.cjoakim.cosmos.altgraph.data.common.io.FileUtil;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.*;

import java.nio.file.Paths;
import java.util.*;

/**
 * An instance of this class is created and executed from the DataCommandLineApp main class
 * to transform the raw data into NpmDocument and Triple documents, and persist these to local disk.
 * <p>
 * The raw data file being parsed is the same file as in this original BOM Graph demo repo:
 * https://github.com/Azure-Samples/azure-cosmos-db-graph-npm-bom-sample
 * <p>
 * Inputs:
 * - data/npm/aggregated_libraries.json
 * Outputs:
 * - data/refined/libraries.json
 * - data/refined/authors.json
 * - data/refined/maintainers.json
 * - data/refined/triples.json
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Slf4j
public class NpmRawDataWranglerProcess extends AbstractConsoleAppProcess {

    // Instance variables:
    private List<Library> rawLibrariesList = null;
    private HashMap<String, NpmDocument> entities = new HashMap<String, NpmDocument>();
    private ArrayList<Triple> triples = new ArrayList<Triple>();

    private long entityCacheHits = 0;
    private long entityCacheMisses = 0;
    private String tenant = null;

    public NpmRawDataWranglerProcess() {

        super();
        tenant = DataAppConfiguration.getTenant();
    }

    public void process() throws Exception {
        log.warn("process...");

        rawLibrariesList = readRawLibraryData();
        log.warn("rawLibrariesList count: " + rawLibrariesList.size());
        // displayParsedRawLibraryData();

        createEntities();
        createTriples();

        log.warn("entityCacheHits:   " + entityCacheHits);
        log.warn("entityCacheMisses: " + entityCacheMisses);

        persistEntitiesToDisk();
        persistTriplesToDisk();

        return;
    }

    private void displayParsedRawLibraryData() throws Exception {

        log.warn("displayParsedRawLibraryData count: " + rawLibrariesList.size());
        for (int i = 0; i < rawLibrariesList.size(); i++) {
            Library lib = rawLibrariesList.get(i);
            log.warn(lib.asJson(true));
        }
        log.warn("displayParsedRawLibraryData count: " + rawLibrariesList.size());
    }

    private void createEntities() {

        createLibraryEntities();
        createAuthorEntities();
        createMaintainerEntities();
    }

    private void createLibraryEntities() {

        for (int i = 0; i < rawLibrariesList.size(); i++) {
            Library lib = rawLibrariesList.get(i);
            lib.setDoctype(DOCTYPE_LIBRARY);
            lib.setLabel(lib.getName());
            lib.setPk(lib.getName());
            lib.setId(UUID.randomUUID().toString());
            lib.setTenant(tenant);
            lib.setLob(LOB_NPM_LIBRARIES);
            lib.populateCacheKey();
            addEntity(lib);
        }
    }

    private void createAuthorEntities() {

        for (int i = 0; i < rawLibrariesList.size(); i++) {
            Library lib = rawLibrariesList.get(i);
            String name = ("" + lib.getAuthor()).trim();
            Author a = new Author();
            a.setDoctype(DOCTYPE_AUTHOR);
            a.setLabel(name);
            a.setPk(name);
            a.setId(UUID.randomUUID().toString());
            a.setTenant(tenant);
            a.setLob(LOB_NPM_LIBRARIES);
            a.populateCacheKey();
            addEntity(a);
        }
    }

    private void createMaintainerEntities() {

        for (int i = 0; i < rawLibrariesList.size(); i++) {
            Library lib = rawLibrariesList.get(i);
            if (lib.getMaintainers() != null) {
                for (int m = 0; m < lib.getMaintainers().length; m++) {
                    String mValue = lib.getMaintainers()[m].trim();
                    Maintainer maintainer = new Maintainer();
                    maintainer.setDoctype(DOCTYPE_MAINTAINER);
                    maintainer.setLabel(mValue);
                    maintainer.setPk(mValue);
                    maintainer.setId(UUID.randomUUID().toString());
                    maintainer.setTenant(tenant);
                    maintainer.setLob(LOB_NPM_LIBRARIES);
                    maintainer.populateCacheKey();
                    addEntity(maintainer);
                }
            }
        }
    }

    private void createTriples() {

        if (DataAppConfiguration.isVerbose()) {
            displayEntityCacheKeys();
        }
        createLibraryLibraryTriples();
        createLibraryAuthorTriples();
        createLibraryMaintainerTriples();
    }

    private void displayEntityCacheKeys() {

        ArrayList<String> values = new ArrayList<String>();

        Object[] keys = entities.keySet().toArray();
        log.warn("displayEntityCacheKeys, count: " + keys.length);

        for (int i = 0; i < keys.length; i++) {
            values.add(keys[i].toString().trim());
        }
        Collections.sort(values);
        for (int i = 0; i < values.size(); i++) {
            log.warn("displayEntityCacheKeys: ^" + values.get(i) + "^");
        }
        log.warn("displayEntityCacheKeys, count: " + keys.length);
    }

    private void createLibraryLibraryTriples() {

        for (int i = 0; i < rawLibrariesList.size(); i++) {
            Library lib = rawLibrariesList.get(i);
            if (lib.getDependencies() != null) {
                NpmDocument entity1 = lookupCachedEntity(lib);
                if (entity1 != null) {
                    Object[] depNames = lib.getDependencies().keySet().toArray();
                    for (int d = 0; d < depNames.length; d++) {
                        String depKey = ("library|" + depNames[d]).trim();
                        NpmDocument entity2 = lookupCachedEntity(depKey);
                        if (entity2 != null) {
                            buildTriple(entity1, "uses_lib", entity2);
                            buildTriple(entity2, "used_in_lib", entity1);
                        } else {
                            log.error("createLibraryLibraryTriples entity2 not_found; " + depKey);
                        }
                    }
                } else {
                    log.error("createLibraryLibraryTriples entity1 not_found; " + lib.getCacheKey());
                }
            }
        }
    }

    private void createLibraryAuthorTriples() {

        for (int i = 0; i < rawLibrariesList.size(); i++) {
            Library lib = rawLibrariesList.get(i);
            if (lib.getDependencies() != null) {
                NpmDocument entity1 = lookupCachedEntity(lib);
                if (entity1 != null) {
                    if (lib.getAuthor() != null) {
                        String depKey = "author|" + lib.getAuthor().trim();
                        NpmDocument entity2 = lookupCachedEntity(depKey);
                        if (entity2 != null) {
                            buildTriple(entity1, "authored_by", entity2);
                            buildTriple(entity2, "author_of", entity1);
                        } else {
                            log.error("createLibraryAuthorTriples entity2 not_found; " + depKey);
                        }
                    }
                }
            } else {
                log.error("createLibraryAuthorTriples entity1 not_found; " + lib.getCacheKey());
            }
        }
    }

    private void createLibraryMaintainerTriples() {

        for (int i = 0; i < rawLibrariesList.size(); i++) {
            Library lib = rawLibrariesList.get(i);
            if (lib.getDependencies() != null) {
                NpmDocument entity1 = lookupCachedEntity(lib);
                if (entity1 != null) {
                    Object[] depNames = lib.getDependencies().keySet().toArray();
                    String[] maintainers = lib.getMaintainers();
                    for (int m = 0; m < maintainers.length; m++) {
                        if (maintainers[m].trim().length() > 0) {
                            String mKey = ("maintainer|" + maintainers[m]).trim();
                            NpmDocument entity2 = lookupCachedEntity(mKey);
                            if (entity2 != null) {
                                buildTriple(entity1, "maintained_by", entity2);
                                buildTriple(entity2, "maintains_lib", entity1);
                            } else {
                                log.error("createLibraryMaintainerTriples entity2 not_found; " + mKey);
                            }
                        }
                    }
                } else {
                    log.error("createLibraryMaintainerTriples entity1 not_found; " + lib.getCacheKey());
                }
            }
        }
    }

    private NpmDocument lookupCachedEntity(NpmDocument e) {

        return lookupCachedEntity(e.getCacheKey());
    }

    private NpmDocument lookupCachedEntity(String cacheKey) {

        if (entities.containsKey(cacheKey)) {
            if (DataAppConfiguration.isVerbose()) {
                log.warn("lookupCachedEntity ok: " + cacheKey);
            }
            NpmDocument e = entities.get(cacheKey);
            if (e != null) {
                entityCacheHits++;
            } else {
                entityCacheMisses++;
            }
            return e;
        } else {
            entityCacheMisses++;
            log.error("lookupCachedEntity not_found: " + cacheKey);
            return null;
        }
    }

    private void buildTriple(NpmDocument e1, String predicate, NpmDocument e2) {

        Triple t = new Triple();

        t.setSubjectType(e1.getDoctype());
        t.setSubjectLabel(e1.getLabel());
        t.setSubjectId(e1.getId());
        t.setSubjectPk(e1.getPk());

        t.setPredicate(predicate);

        t.setObjectType(e2.getDoctype());
        t.setObjectLabel(e2.getLabel());
        t.setObjectId(e2.getId());
        t.setObjectPk(e2.getPk());

        t.setPk(DOCTYPE_TRIPLE + "|" + tenant);
        t.setId(UUID.randomUUID().toString());
        t.setDoctype(DOCTYPE_TRIPLE);
        t.setTenant(tenant);
        t.setLob(LOB_NPM_LIBRARIES);

        // set subjectTags
        if (t.getSubjectType().equals("library")) {
            String entityKey = "library|" + t.getSubjectPk();  // library|tcx-js
            NpmDocument e = this.entities.get(entityKey);
            if (e != null) {
                t.setSubjectTags(getLibraryTags(e));
            } else {
                log.warn("buildTriple - miss on subjectKey: " + entityKey);
            }
        }

        // set objectTags
        if (t.getObjectType().equals("library")) {
            String entityKey = "library|" + t.getObjectPk();  // library|tcx-js
            NpmDocument e = this.entities.get(entityKey);
            if (e != null) {
                t.setObjectTags(getLibraryTags(e));
            } else {
                log.warn("buildTriple - miss on objectKey: " + entityKey);
            }
        }

        triples.add(t);
    }

    private ArrayList<String> getLibraryTags(NpmDocument e) {
        ArrayList<String> list = new ArrayList<String>();
        try {
            Library lib = (Library) e;
            if (lib != null) {
                if (lib.getAuthor() != null) {
                    String author = lib.getAuthor().trim();
                    if (author.length() > 0) {
                        list.add("author|" + lib.getAuthor().trim());
                    }
                }
                if (lib.getMaintainers() != null) {
                    for (int m = 0; m < lib.getMaintainers().length; m++) {
                        String maintainer = lib.getMaintainers()[m].trim();
                        if (maintainer.length() > 0) {
                            list.add("maintainer|" + maintainer);
                        }
                    }
                }
            }
        } catch (Exception ex) {
            ex.printStackTrace();
        }
        return list;
    }

    private void addEntity(NpmDocument e) {

        String key = e.getCacheKey();

        if (DataAppConfiguration.isVerbose()) {
            if (entities.containsKey(key)) {
                log.warn("addEntity_dup " + key);
            } else {
                log.warn("addEntity_new " + key);
            }
        }
        entities.put(e.getCacheKey(), e);
    }

    public List<Library> readRawLibraryData() throws Exception {

        ObjectMapper objectMapper = new ObjectMapper();
        ObjectReader objectReader = objectMapper.reader().forType(new TypeReference<List<Library>>() {
        });

        String infile = NPM_RAW_LIBRARIES_FILE;
        return objectReader.readValue(Paths.get(infile).toFile());
    }

    /**
     * Write the Entities to a JSON file on local disk.
     */
    private void persistEntitiesToDisk() throws Exception {

        // First get a sorted list of the NpmDocument keys
        ArrayList<String> keysList = new ArrayList<String>();
        Object[] keyValues = entities.keySet().toArray();
        for (int i = 0; i < keyValues.length; i++) {
            keysList.add((String) keyValues[i]);
        }
        Collections.sort(keysList);

        FileUtil fu = new FileUtil();

        // Collect and write the entity-specific files

        ArrayList<NpmDocument> libraries = filterByEntityType(keysList, DOCTYPE_LIBRARY);
        fu.writeJson(libraries, NPM_LIBRARIES_FILE, true, true);

        ArrayList<NpmDocument> authors = filterByEntityType(keysList, DOCTYPE_AUTHOR);
        fu.writeJson(authors, NPM_AUTHORS_FILE, true, true);

        ArrayList<NpmDocument> maintainers = filterByEntityType(keysList, DOCTYPE_MAINTAINER);
        fu.writeJson(maintainers, NPM_MAINTAINERS_FILE, true, true);
    }

    private ArrayList<NpmDocument> filterByEntityType(ArrayList<String> keysList, String type) {

        ArrayList<NpmDocument> filteredEntities = new ArrayList<NpmDocument>();

        for (int i = 0; i < keysList.size(); i++) {
            String key = keysList.get(i);
            if (key.startsWith(type)) {
                filteredEntities.add(entities.get(key));
            }
        }
        log.warn("filterByEntityType: " + filteredEntities.size() + " of type: " + type);
        return filteredEntities;
    }

    /**
     * Write the Triples to a JSON file on local disk.
     */
    private void persistTriplesToDisk() throws Exception {

        FileUtil fu = new FileUtil();
        String outfile = NPM_TRIPLES_FILE;
        fu.writeJson(triples, outfile, true, true);
        log.warn("" + triples.size() + " triples written to disk");
    }

}
