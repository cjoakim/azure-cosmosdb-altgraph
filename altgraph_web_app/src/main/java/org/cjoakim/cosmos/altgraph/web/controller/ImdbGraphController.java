package org.cjoakim.cosmos.altgraph.web.controller;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.DataAppConfiguration;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.cjoakim.cosmos.altgraph.data.common.cache.Cache;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.JGraph;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.JRank;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.JStarNetwork;
import org.cjoakim.cosmos.altgraph.data.common.graph.v2.struct.EdgesStruct;
import org.cjoakim.cosmos.altgraph.data.common.io.FileUtil;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Movie;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Person;
import org.cjoakim.cosmos.altgraph.data.common.repository.ImdbMovieRepository;
import org.cjoakim.cosmos.altgraph.data.common.repository.ImdbPersonRepository;
import org.cjoakim.cosmos.altgraph.data.common.util.MemoryStats;
import org.cjoakim.cosmos.altgraph.web.controller.struct.GraphStatsStruct;
import org.cjoakim.cosmos.altgraph.web.forms.ImdbGraphForm;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

import javax.servlet.http.HttpSession;
import java.util.ArrayList;

/**
 * This is the Controller vor the IMDB views in this web application.
 * Chris Joakim, Microsoft, November 2022
 */

@Slf4j
@Controller
public class ImdbGraphController implements DataConstants {
    private JGraph jgraph;

    private FileUtil fileUtil = new FileUtil();

    ImdbMovieRepository movieRepository = null;
    ImdbPersonRepository personRepository = null;

    @Autowired
    public ImdbGraphController(ImdbMovieRepository mr, ImdbPersonRepository pr) {
        super();
        movieRepository  = mr;
        personRepository = pr;

        String source = DataAppConfiguration.getInstance().imdbGraphSource;
        jgraph = new JGraph(DataConstants.GRAPH_DOMAIN_IMDB, source);
        int[] counts = jgraph.getVertexAndEdgeCounts();
        log.warn("jgraph vertices: " + counts[0]);
        log.warn("jgraph edges:    " + counts[1]);
    }

    @GetMapping(value = "/show_imdb_graph_form")
    public String showImdbGraphForm(HttpSession session, Model model) {
        ImdbGraphForm formObject = new ImdbGraphForm();
        formObject.setValue1("");
        formObject.setValue2("");
        formObject.setElapsedMs("");
        formObject.setSessionId(session.getId());
        model.addAttribute("formObject", formObject);
        return "imdb_search";
    }

    @PostMapping("/post_imdb_graph_form")
    public String postImdbGraphSearchForm(HttpSession session, @ModelAttribute ImdbGraphForm formObject, Model model) {
        formObject.translateShortcutValues();
        log.warn("formObject, getFormFunction:     " + formObject.getFormFunction());
        log.warn("formObject, getValue1:           " + formObject.getValue1());
        log.warn("formObject, getValue2:           " + formObject.getValue2());
        log.warn("formObject, getSessionId (form): " + formObject.getSessionId());
        log.warn("formObject, getSessionId (http): " + session.getId());

        String formFunctionAndView = formObject.getFormFunction();
        long startMs = System.currentTimeMillis();
        try {
            switch (formFunctionAndView) {
                case "imdb_stats":
                    break;
                case "imdb_centrality":
                    break;
                case "imdb_page_rank":
                    if (!formObject.isValue1AnInteger()) {
                        formObject.setValue1("100");
                    }
                    break;
                case "imdb_shortest_path":
                    break;
                case "imdb_network":
                    if (!formObject.isValue2AnInteger()) {
                        formObject.setValue2("1");
                    }
                    break;
                default:
                    break;
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

        formObject.setElapsedMs("" + (System.currentTimeMillis() - startMs) + " ms");
        model.addAttribute("formObject", formObject);
        return formFunctionAndView;
    }

    /**
     * curl http://localhost:8080/graph_stats
     */
    @RequestMapping(value = "/graph_stats/{flag}", method = RequestMethod.GET, produces = "application/json")
    public ResponseEntity<GraphStatsStruct> getGraphStats(
            HttpSession session,
            @PathVariable("flag") String flag) {

        long startMs = System.currentTimeMillis();
        MemoryStats memStats = new MemoryStats("");
        GraphStatsStruct struct = new GraphStatsStruct();
        if (flag.equalsIgnoreCase("reload")) {
            jgraph.refresh();
        }
        int[] counts = jgraph.getVertexAndEdgeCounts();
        struct.setVertexCount(counts[0]);
        struct.setEdgeCount(counts[1]);
        struct.setEpoch(memStats.getEpoch());
        struct.setTotalMb(memStats.getTotalMb());
        struct.setFreeMb(memStats.getFreeMb());
        struct.setMaxMb(memStats.getMaxMb());
        struct.setPctFree(memStats.getPctFree());
        struct.setElapsedMs(System.currentTimeMillis() - startMs);
        struct.setRefreshDate(jgraph.getRefreshDate());
        struct.setRefreshMs(jgraph.getRefreshMs());
        struct.setRefreshSource(jgraph.getSource());

        HttpHeaders headers = new HttpHeaders();
        ResponseEntity<GraphStatsStruct> entity =
                new ResponseEntity<>(struct, headers, HttpStatus.OK);
        return entity;
    }

    /**
     * curl http://localhost:8080/get_shortest_path/nm0000102/nm0001648
     */
    @RequestMapping(value = "/get_shortest_path/{v1}/{v2}", method = RequestMethod.GET, produces = "application/json")
    @ResponseBody
    public ResponseEntity<EdgesStruct> getShortestPath(
            HttpSession session,
            @PathVariable("v1") String v1,
            @PathVariable("v2") String v2) {
        log.warn("getShortestPath, v1: " + v1 + ", v2: " + v2);

        EdgesStruct struct =
                jgraph.getShortestPathAsEdgesStruct(v1, v2);

        HttpHeaders headers = new HttpHeaders();
        ResponseEntity<EdgesStruct> entity =
                new ResponseEntity<>(struct, headers, HttpStatus.OK);
        return entity;
    }

    /**
     * curl http://localhost:8080/get_page_ranks/10
     */
    @RequestMapping(value = "/get_page_ranks/{count}", method = RequestMethod.GET, produces = "application/json")
    @ResponseBody
    public ResponseEntity<ArrayList<JRank>> getPageRanks(
            HttpSession session,
            @PathVariable("count") int count) {
        log.warn("getPageRanks, count: " + count);

        ArrayList<JRank> ranks = jgraph.sortedPageRanks(count);

        HttpHeaders headers = new HttpHeaders();
        ResponseEntity<ArrayList<JRank>> entity =
                new ResponseEntity<>(ranks, headers, HttpStatus.OK);
        return entity;
    }

    /**
     * curl http://localhost:8080/get_star_network/nm0000102/2
     */
    @RequestMapping(value = "/get_star_network/{vertex}/{degree}", method = RequestMethod.GET, produces = "application/json")
    @ResponseBody
    public ResponseEntity<EdgesStruct> getStarNetwork(
            HttpSession session,
            @PathVariable("vertex") String vertex,
            @PathVariable("degree") int degree) {
        log.warn("getStarNetwork, vertex: " + vertex + ", degree: " + degree);

        JStarNetwork star = jgraph.starNetworkFor(vertex, degree);
        EdgesStruct es = star.asEdgesStruct();

        HttpHeaders headers = new HttpHeaders();
        ResponseEntity<EdgesStruct> entity =
                new ResponseEntity<>(es, headers, HttpStatus.OK);
        return entity;
    }

    @RequestMapping(value = "/get_imdb_vertex/{imdbConst}", method = RequestMethod.GET, produces = "application/json")
    @ResponseBody
    public String getImdbVertex(HttpSession session, @PathVariable("imdbConst") String imdbConst) {
        log.warn("geImdbLibraryAsJson, imdbConst: " + imdbConst);

        try {
            if (imdbConst.startsWith("tt")) {
                Movie m = lookupMovie(imdbConst);
                if (m != null) {
                    return m.asJson(true);
                }
            } else if (imdbConst.startsWith("nm")) {
                Person p = lookupPerson(imdbConst);
                if (p != null) {
                    return p.asJson(true);
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return "{}";
    }

    private Movie lookupMovie(String imdbConst) {
        Iterable<Movie> it = movieRepository.findByIdAndPk(imdbConst, imdbConst);
        while (it.iterator().hasNext()) {
            return it.iterator().next();
        }
        return null;
    }

    private Person lookupPerson(String imdbConst) {
        Iterable<Person> it = personRepository.findByIdAndPk(imdbConst, imdbConst);
        while (it.iterator().hasNext()) {
            return it.iterator().next();
        }
        return null;
    }

    public String asJson(Object obj, boolean pretty) {

        try {
            ObjectMapper mapper = new ObjectMapper();
            if (pretty) {
                return mapper.writerWithDefaultPrettyPrinter().writeValueAsString(obj);
            } else {
                return mapper.writeValueAsString(this);
            }
        } catch (JsonProcessingException e) {
            e.printStackTrace();
            return "{}";
        }
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
