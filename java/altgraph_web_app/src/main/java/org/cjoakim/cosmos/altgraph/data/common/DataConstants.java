package org.cjoakim.cosmos.altgraph.data.common;

/**
 * This interface implements constants or hardcoded values used in the App.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */
public interface DataConstants {

    // Bytes, Storage, Memory
    public static final double KB = 1024;
    public static final double MB = 1024 * 1024;
    public static final double GB = 1024 * 1024 * 1024;
    public static final double TB = 1024 * 1024 * 1024 * 1024;

    public static final String IMDB_GRAPH_SOURCE_COSMOS = "cosmos";
    public static final String IMDB_GRAPH_SOURCE_DISK = "disk";

    public static final String IMDB_SEED_CONTAINER = "imdb_seed";
    public static final String GRAPH_DOMAIN_IMDB = "imdb";

    // Document Types
    public static final String DOCTYPE_LIBRARY = "library";
    public static final String DOCTYPE_AUTHOR = "author";
    public static final String DOCTYPE_MAINTAINER = "maintainer";
    public static final String DOCTYPE_TRIPLE = "triple";

    public static final String DOCTYPE_MOVIE = "movie";
    public static final String DOCTYPE_MOVIE_SEED = "movie_seed";
    public static final String DOCTYPE_PERSON = "person";
    public static final String DOCTYPE_PRINCIPAL = "principal";
    public static final String DOCTYPE_SMALL_TRIPLE = "sm_triple";

    // Default config values

    public static final String DEFAULT_CONTAINER = "altgraph";
    public static final String DEFAULT_TENANT = "123";
    public static final String LOB_NPM_LIBRARIES = "npm";

    public static final String CACHE_WITH_LOCAL_DISK = "disk";
    public static final String CACHE_WITH_REDIS = "redis";

    // Filenames
    public static final String NPM_RAW_LIBRARIES_FILE = "data/npm_raw/aggregated_libraries.json";
    public static final String NPM_LIBRARIES_FILE = "data/npm_refined/libraries.json";
    public static final String NPM_AUTHORS_FILE = "data/npm_refined/authors.json";
    public static final String NPM_MAINTAINERS_FILE = "data/npm_refined/maintainers.json";
    public static final String NPM_TRIPLES_FILE = "data/npm_refined/triples.json";
    public static final String TRIPLE_QUERY_STRUCT_FILE = "data/struct/TripleQueryStruct.json";
    public static final String NPM_LIBRARY_GRAPH_JSON_FILE = "data/npm_graph/library_graph.json";
    public static final String NPM_AUTHOR_GRAPH_JSON_FILE = "data/npm_graph/author_graph.json";
    public static final String NPM_GRAPH_NODES_CSV_FILE = "data/graph/nodes.csv";
    public static final String NPM_GRAPH_EDGES_CSV_FILE = "data/graph/edges.csv";
    public static final String D3_CSV_BUILDER_FILE = "data/struct/D3CsvBuilder.json";
    public static final String MEMORY_STATS_BASE = "out/memory_";

    public static final String IMDB_RAW_NAME_BASICS_FILE = "data/imdb_raw/name.basics.tsv";
    public static final String IMDB_RAW_TITLE_BASICS_FILE = "data/imdb_raw/title.basics.tsv";
    public static final String IMDB_RAW_TITLE_PRINCIPALS_FILE = "data/imdb_raw/title.principals.tsv";
    public static final String IMDB_MOVIES_DOCUMENTS_FILE = "data/imdb_refined/movies.json";
    public static final String IMDB_MOVIES_SEED_FILE      = "data/imdb_refined/movies_seed.json";
    public static final String IMDB_MOVIES_MAP_FILE = "data/imdb_refined/movies_map.json";
    public static final String IMDB_PEOPLE_DOCUMENTS_FILE = "data/imdb_refined/people.json";
    public static final String IMDB_PEOPLE_MAP_FILE = "data/imdb_refined/people_map.json";
    public static final String IMDB_MOVIES_OF_INTEREST_FILE = "data/imdb_refined/movies_of_interest.txt";
    public static final String IMDB_PEOPLE_OF_INTEREST_FILE = "data/imdb_refined/people_of_interest.txt";
    public static final String IMDB_SMALL_TRIPLES_DOCUMENTS_FILE = "data/imdb_refined/sm_triples.json";

    // Edge Labels
    public static final int GROUP_MAX = 100;
    public static final String PREDICATE_IN_MOVIE = "inMovie";
    public static final String PREDICATE_HAS_PERSON = "hasPerson";

    // Command-line args
    public static final String VERBOSE_FLAG = "--verbose";
    public static final String SILENT_FLAG = "--silent";
    public static final String PRETTY_FLAG = "--pretty";
    public static final String TENANT_FLAG = "--tenant";
    public static final String CONTAINER_FLAG = "--container";
    public static final String DO_WRITES_FLAG = "--do-writes";
    public static final String LOB_FLAG = "--lob";
}
