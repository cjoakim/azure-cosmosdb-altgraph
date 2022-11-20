package org.cjoakim.cosmos.altgraph.data;

import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Movie;
import org.cjoakim.cosmos.altgraph.data.common.model.imdb.Person;
import org.cjoakim.cosmos.altgraph.data.common.repository.AuthorRepository;
import org.cjoakim.cosmos.altgraph.data.common.repository.ImdbMovieRepository;
import org.cjoakim.cosmos.altgraph.data.common.repository.ImdbPersonRepository;
import org.cjoakim.cosmos.altgraph.data.common.repository.TripleRepository;
import org.cjoakim.cosmos.altgraph.data.processor.*;
import org.jgrapht.graph.DefaultEdge;
import org.jgrapht.graph.SimpleGraph;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.ApplicationContext;
import org.springframework.context.annotation.ComponentScan;

/**
 * This is the entry=point for this Spring Boot application.
 * It is a "console app" due to the CommandLineRunner interface.
 *
 * Chris Joakim, Microsoft, November 2022
 */

@SpringBootApplication
@Slf4j
@ComponentScan(basePackages = {"org.cjoakim.cosmos.altgraph"})
public class DataCommandLineApp implements CommandLineRunner, DataConstants {
    @Autowired
    private ApplicationContext applicationContext;
    @Autowired
    private AuthorRepository authorRepository;
    @Autowired
    private TripleRepository tripleRepository;

    @Autowired
    private ImdbMovieRepository movieRepository;
    @Autowired
    private ImdbPersonRepository personRepository;

    @Autowired
    private NpmCosmosLoaderProcessor npmCosmosLoaderProcessor;
    @Autowired
    private RepoQueryProcessor repoQueryProcessor;
    @Autowired
    private DaoQueryProcessor daoQueryProcessor;
    @Autowired
    private D3CsvProcessor d3CsvProcessor;
    @Autowired
    private CacheProcessor cacheProcessor;
    @Autowired
    private RedisPocProcessor redisPocProcessor;

    @Autowired
    private SdkBulkLoaderProcessor sdkBulkLoaderProcessor;

    ImdbRawDataWranglerProcess imdbRawDataWranglerProcess = null;
    ImdbTripleBuilderProcess imdbTripleBuilderProcess = null;
    ImdbGraphMemLoadProcessor imdbGraphMemLoadProcessor = null;

    @Autowired
    private GraphProcessor graphProcessor;

    public static void main(String[] args) {
        DataAppConfiguration.setCommandLineArgs(args);
        log.warn("main method...");
        SpringApplication.run(DataCommandLineApp.class, args);
    }

    public void run(String[] args) throws Exception {
        log.warn("run method...");
        DataAppConfiguration.setCommandLineArgs(args);
        String processType = args[0];
        AbstractConsoleAppProcess processor = null;
        log.warn("run, processType: " + processType);

        try {
            switch (processType) {
                case "npm_wrangle_raw_data":
                    processor = new NpmRawDataWranglerProcess();
                    processor.process();
                    break;
                case "npm_load_cosmos":
                    npmCosmosLoaderProcessor.process();
                    break;
                case "npm_springdata_queries":
                    repoQueryProcessor.process();
                    break;
                case "npm_dao_queries":
                    daoQueryProcessor.process();
                    break;
                case "npm_build_graph":
                    graphProcessor.process();
                    break;
                case "npm_build_d3_csv":
                    d3CsvProcessor.process();
                    break;
                case "test_cache":
                    cacheProcessor.process();
                    break;
                case "test_redis":
                    redisPocProcessor.process();
                    break;
                case "imdb_wrangle_raw_data":
                    imdbRawDataWranglerProcess = new ImdbRawDataWranglerProcess();
                    imdbRawDataWranglerProcess.setMinYear(Integer.parseInt(args[1]));
                    imdbRawDataWranglerProcess.setMinMinutes(Integer.parseInt(args[2]));
                    imdbRawDataWranglerProcess.process();
                    break;
                case "imdb_build_triples":
                    imdbTripleBuilderProcess = new ImdbTripleBuilderProcess();
                    imdbTripleBuilderProcess.process();
                    break;
                case "imdb_traversal":
                    ImdbJgraphtGraphTraversalProcess jgraphtProcess =
                            new ImdbJgraphtGraphTraversalProcess();
                    jgraphtProcess.setTraversalType("traverse");
                    jgraphtProcess.setNconst1(args[1]);
                    jgraphtProcess.setNconst2(args[2]);
                    jgraphtProcess.setDirected(Boolean.parseBoolean(args[3]));
                    jgraphtProcess.process();
                    break;
                case "imdb_bulk_load_movies":
                case "imdb_bulk_load_people":
                case "imdb_bulk_load_small_triples":
                case "imdb_bulk_load_movies_idx":
                    sdkBulkLoaderProcessor.setLoadType(processType);
                    sdkBulkLoaderProcessor.setContainer(args[1]);
                    sdkBulkLoaderProcessor.process();
                    break;
                case "imdb_person_lookup":
                    Iterable<Person> personIt = personRepository.findByIdAndPk(args[1], args[1]);
                    while (personIt.iterator().hasNext()) {
                        Person p = personIt.iterator().next();
                        log.warn(p.asJson(true));
                        break;
                    }
                    break;
                case "imdb_movie_lookup":
                    Iterable<Movie> movieIt = movieRepository.findByIdAndPk(args[1], args[1]);
                    while (movieIt.iterator().hasNext()) {
                        Movie p = movieIt.iterator().next();
                        log.warn(p.asJson(true));
                        break;
                    }
                    break;
                case "imdb_mem_load_imdb_graph":
                    org.jgrapht.Graph<String, DefaultEdge> jgraph = new SimpleGraph<>(DefaultEdge.class);
                    imdbGraphMemLoadProcessor = new ImdbGraphMemLoadProcessor();
                    imdbGraphMemLoadProcessor.setGraphType("imdb");
                    imdbGraphMemLoadProcessor.setContainer(args[1]);
                    imdbGraphMemLoadProcessor.setGraph(jgraph);
                    imdbGraphMemLoadProcessor.process();
                    break;
                default:
                    log.error("unknown CLI process name: " + processType);
            }
        } catch (Throwable t) {
            t.printStackTrace();
            throw t;
        } finally {
            log.warn("spring app exiting");
            SpringApplication.exit(this.applicationContext);
            log.warn("spring app exit completed");
            System.exit(0);
        }
    }
}
