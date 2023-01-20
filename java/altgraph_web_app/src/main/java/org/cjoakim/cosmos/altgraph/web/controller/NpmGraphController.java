package org.cjoakim.cosmos.altgraph.web.controller;

import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.cjoakim.cosmos.altgraph.data.common.cache.Cache;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.D3CsvBuilder;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.Graph;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.GraphBuilder;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.TripleQueryStruct;
import org.cjoakim.cosmos.altgraph.data.common.io.FileUtil;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Author;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Library;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Triple;
import org.cjoakim.cosmos.altgraph.data.common.repository.AuthorRepository;
import org.cjoakim.cosmos.altgraph.data.common.repository.LibraryRepository;
import org.cjoakim.cosmos.altgraph.data.common.repository.ResponseDiagnosticsProcessorImpl;
import org.cjoakim.cosmos.altgraph.data.common.repository.TripleRepository;
import org.cjoakim.cosmos.altgraph.web.forms.NpmGraphForm;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;
import java.util.Iterator;

/**
 * This is the Controller vor the NPM views in this web application.
 * Chris Joakim, Microsoft, November 2022
 */

@Slf4j
@Controller
public class NpmGraphController implements DataConstants {
    private LibraryRepository libraryRepository = null;

    private AuthorRepository authorRepository = null;
    private TripleRepository tripleRepository = null;
    private Cache cache;
    private FileUtil fileUtil = new FileUtil();

    @Autowired
    public NpmGraphController(LibraryRepository lr, AuthorRepository ar, TripleRepository tr, Cache c) {
        super();
        libraryRepository = lr;
        authorRepository = ar;
        tripleRepository = tr;
        cache = c;
    }

    @GetMapping(value = "/show_npm_graph_form")
    public String showNpmGraphForm(HttpSession session, Model model) {
        NpmGraphForm formObject = new NpmGraphForm();
        formObject.setSubjectName("");
        formObject.setGraphDepth("");
        formObject.setElapsedMs("");
        formObject.setCacheOpts("");
        formObject.setSessionId(session.getId());
        model.addAttribute("formObject", formObject);
        return "npm_search";
    }

    @PostMapping("/post_npm_graph_form")
    public String postNpmGraphSearchForm(HttpSession session, @ModelAttribute NpmGraphForm formObject, Model model) {
        log.warn("formObject, getSubjectName:      " + formObject.getSubjectName());
        log.warn("formObject, getDepthAsInt:       " + formObject.getDepthAsInt());
        log.warn("formObject, getCacheOpts:        " + formObject.getCacheOpts());
        log.warn("formObject, useCachedLibrary:    " + useCachedLibrary(formObject.getCacheOpts()));
        log.warn("formObject, useCachedTriples:    " + useCachedTriples(formObject.getCacheOpts()));
        log.warn("formObject, getSessionId (form): " + formObject.getSessionId());
        log.warn("formObject, getSessionId (http): " + session.getId());

        long startMs = System.currentTimeMillis();
        try {
            log.warn(formObject.asJson(true));

            if (formObject.isAuthorCheckbox()) {
                handleAuthorSearch(session, formObject);
            } else {
                handleLibrarySearch(session, formObject);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        formObject.setElapsedMs("" + (System.currentTimeMillis() - startMs) + " ms");
        model.addAttribute("formObject", formObject);
        return "npm_graph";
    }

    @GetMapping(value = "/npm_nodes_csv", produces = "text/csv")
    public void npmNodesCsv(HttpSession session, HttpServletResponse response) throws Exception {
        log.warn("nodesCsv, session Id: " + session.getId());
        String csv = readCsv("data/graph/nodes.csv");
        response.setContentType("text/plain; charset=utf-8");
        response.addHeader("Cache-Control", "max-age=0, must-revalidate, no-transform");
        response.getWriter().print(csv);
    }

    @GetMapping(value = "/npm_edges_csv", produces = "text/csv")
    public void npmEdgesCsv(HttpSession session, HttpServletResponse response) throws Exception {
        log.warn("edgesCsv, session Id: " + session.getId());
        String csv = readCsv("data/graph/edges.csv");
        response.setContentType("text/plain; charset=utf-8");
        response.addHeader("Cache-Control", "max-age=0, must-revalidate, no-transform");
        response.getWriter().print(csv);
    }

    @RequestMapping(value = "/get_npm_library/{libraryName}", method = RequestMethod.GET, produces = "application/json")
    @ResponseBody
    public String getNpmLibraryAsJson(HttpSession session, @PathVariable("libraryName") String libraryName) {
        log.warn("getNpmLibraryAsJson, libraryName: " + libraryName);

        Library library = readLibrary(libraryName, session.getId(), false);
        if (library != null) {
            try {
                return library.asJson(true);
            } catch (Exception e) {
                e.printStackTrace();
                return "{}";
            }
        } else {
            return "{}";
        }
    }

    private void handleLibrarySearch(HttpSession session, NpmGraphForm formObject) {

        try {
            String libName = formObject.getSubjectName();
            Library library = readLibrary(libName, session.getId(), useCachedLibrary(formObject.getCacheOpts()));

            if (library != null) {
                TripleQueryStruct struct = readTriples(
                        useCachedTriples(formObject.getCacheOpts()),
                        formObject.getSessionId());

                GraphBuilder graphBuilder = new GraphBuilder(library, struct);
                Graph graph = graphBuilder.buildLibraryGraph(formObject.getDepthAsInt());

                D3CsvBuilder d3CsvBuilder = new D3CsvBuilder(graph);
                d3CsvBuilder.buildBillOfMaterialCsv(session.getId(), formObject.getDepthAsInt());
                d3CsvBuilder.finish();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void handleAuthorSearch(HttpSession session, NpmGraphForm formObject) {

        try {
            formObject.setGraphDepth("0");
            String libName = formObject.getSubjectName();
            Library library = readLibrary(libName, session.getId(), useCachedLibrary(formObject.getCacheOpts()));
            Author author = readAuthorByLabel(library.getAuthor(), session.getId(), useCachedLibrary(formObject.getCacheOpts()));
            log.warn("handleAuthorSearch, libName: " + libName + ", author: " + library.getAuthor());

            if (author != null) {
                TripleQueryStruct struct = readTriples(
                        useCachedTriples(formObject.getCacheOpts()),
                        formObject.getSessionId());
                GraphBuilder graphBuilder = new GraphBuilder(author, struct);
                Graph graph = graphBuilder.buildAuthorGraph(author);
                D3CsvBuilder d3CsvBuilder = new D3CsvBuilder(graph);
                d3CsvBuilder.buildBillOfMaterialCsv(session.getId(), formObject.getDepthAsInt());
                d3CsvBuilder.finish();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }


    private Library readLibrary(String libName, String sessionId, boolean useCache) {

        log.warn("readLibrary, libName: " + libName + ", useCache: " + useCache);
        Library lib = null;

        if (useCache) {
            lib = cache.getLibrary(libName);
            if (lib != null) {
                return lib;
            }
        }

        log.warn("readLibrary, reading DB");
        Iterable<Library> iterable = libraryRepository.findByPkAndTenantAndDoctype(
                libName, DEFAULT_TENANT, "library");
        Iterator<Library> it = iterable.iterator();
        while (it.hasNext()) {
            lib = it.next();
            lib.setGraphKey(lib.calculateGraphKey());
            log.warn("readLibrary, saving to cache: " + libName);
            try {
                cache.putLibrary(lib);
                log.warn("readLibrary, saved to cache: " + libName);
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        log.warn("readLibrary, last_request_charge: " + ResponseDiagnosticsProcessorImpl.getLastRequestCharge());
        return lib;
    }

    private Author readAuthorByLabel(String label, String sessionId, boolean useCache) {

        Author author = null;
        Iterable<Author> iterable = authorRepository.findByLabel(label);
        Iterator<Author> it = iterable.iterator();
        while (it.hasNext()) {
            author = it.next();
        }
        log.warn("readAuthorByLabel, last_request_charge: " + ResponseDiagnosticsProcessorImpl.getLastRequestCharge());
        return author;
    }

    private TripleQueryStruct readTriples(boolean useCache, String sessionId) throws Exception {

        String cacheFilename = getTripleQueryStructCacheFilename(sessionId);
        log.warn("readTriples, useCache: " + useCache + ", cacheFilename: " + cacheFilename);

        if (useCache) {
            log.warn("readTriples, reading cached file " + cacheFilename);
            TripleQueryStruct struct = cache.getTriples();
            if (struct != null) {
                log.warn("readTriples, returning cached");
                return struct;
            }
        }

        log.warn("readTriples, reading DB");
        String lob = LOB_NPM_LIBRARIES;
        String subject = "library";
        TripleQueryStruct struct = new TripleQueryStruct();
        struct.setSql("dynamic");
        struct.start();

        String pk = "triple|" + DEFAULT_TENANT; // "pk": "triple|123"'
        Iterable<Triple> iterable = tripleRepository.getByPkLobAndSubjects(pk, lob, subject, subject);
        Iterator<Triple> it = iterable.iterator();
        while (it.hasNext()) {
            Triple t = it.next();
            t.setKeyFields();
            struct.addDocument(t);
        }
        struct.stop();
        try {
            log.warn("readTriples, saving to cache");
            cache.putTriples(struct);
            log.warn("readTriples, saved to cache");
        } catch (Exception e) {
            e.printStackTrace();
        }
        log.warn("readTriples, last_request_charge: " + ResponseDiagnosticsProcessorImpl.getLastRequestCharge());
        return struct;
    }


    private String readCsv(String path) {

        return fileUtil.readUnicode(path);
    }

    private boolean useCachedLibrary(String cacheOpts) {

        if (cacheOpts == null) {
            return false;
        }
        return cacheOpts.toUpperCase().contains("L");
    }

    private boolean useCachedTriples(String cacheOpts) {

        if (cacheOpts == null) {
            return false;
        }
        return cacheOpts.toUpperCase().contains("T");
    }

    private String getLibraryCacheFilename(String libName, String sessionId) {

        // TODO - implement session-specific logic
        return "data/cache/" + libName + ".json";
    }

    private String getTripleQueryStructCacheFilename(String sessionId) {

        // TODO - implement session-specific logic
        return "data/cache/TripleQueryStruct.json";
    }
}
