# Execute the entire IMDb data wrangling process.
# It is assumed that you have already downloaded the raw-data TSV files;
# altgraph_data_app/data/imdb_raw/readme.md in this repo.
# Chris Joakim, Microsoft, November 2022

invoke-expression -Command .\build.ps1

# These two parameters drive the IMDb data-extraction and wrangling process:
$startingYear    = 0
$movieMinMinutes = 30

echo "imdb_wrangle_raw_data..."
java -Xms1024m -Xmx2048m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_wrangle_raw_data $startingYear $movieMinMinutes > out/imdb_wrangle_raw_data.txt

echo "imdb_build_triples..."
java -Xms1024m -Xmx4096m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_build_triples > out/imdb_build_triples.txt

echo "imdb_traversal..."
java -Xms2048m -Xmx4096m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_traversal nm0000102 nm0000210 false disk > out/imdb_traversal.txt

echo "zip large output files for storage in github..."
ant -f .\zip_data.xml

echo "done"

# =============================================================================
# Sample output with $startingYear 1980 and $movieMinMinutes 100
# wrangling:
#    creationEpoch|totalMb|freeMb|maxMb|pctFree|pctMax|note
#    1668702979298|1024|715|2048|0.698713116348|0.5|finish
#    EOJ Totals:
#    minYear:                  1980
#    minMinutes:               100
#    totalMovieCount:          9275714
#    includedMovieCount:       79808
#    totalPrincipalCount:      52412047
#    includedPrincipalCount:   189234
#    totalPersonCount:         11978491
#    includedPersonCount:      189049
#    included Movies + People: 268857
#    titleToPersonCount:       417014
#    elapsedMs:                226513.0
#    elapsedMin:               3.7752166666666667
# traversal:
#creationEpoch|totalMb|freeMb|maxMb|pctFree|pctMax|note
#1668703036479|1394|837|4096|0.600593824127|0.34033203125|loadImdbGraphFromDisk - after building graph
#11:37:16.480 [main] WARN  o.c.c.a.d.c.graph.v2.JGraphBuilder - loadImdbGraphFromDisk - movieNodesCreated:  79808
#11:37:16.480 [main] WARN  o.c.c.a.d.c.graph.v2.JGraphBuilder - loadImdbGraphFromDisk - personNodesCreated: 189049
#11:37:16.480 [main] WARN  o.c.c.a.d.c.graph.v2.JGraphBuilder - loadImdbGraphFromDisk - edgesCreated:       833988
#11:37:16.480 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - JGraph refresh() - replacing graph with newGraph, elapsed ms: 2498
#11:37:16.645 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - getShortestPath, v1: nm0000102 to v2: nm0001648
#11:37:16.676 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - elapsed milliseconds: 31
#11:37:16.677 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - path getLength:       4
#11:37:16.677 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - path getStartVertex:  nm0000102
#11:37:16.677 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - path getEndVertex:    nm0001648

# =============================================================================
# Sample output with $startingYear 0 and $movieMinMinutes 60
# wrangling:
#    creationEpoch|totalMb|freeMb|maxMb|pctFree|pctMax|note
#    1668703584648|2048|1088|2048|0.531394597143|1|finish
#    EOJ Totals:
#    minYear:                  0
#    minMinutes:               60
#    totalMovieCount:          9275714
#    includedMovieCount:       336185
#    totalPrincipalCount:      52412047
#    includedPrincipalCount:   600179
#    totalPersonCount:         11978491
#    includedPersonCount:      599722
#    included Movies + People: 935907
#    titleToPersonCount:       1779900
#    elapsedMs:                266614.0
#    elapsedMin:               4.443566666666666
# traversal:
#creationEpoch|totalMb|freeMb|maxMb|pctFree|pctMax|note
#1668703750678|3248|1310|4096|0.40331972996|0.79296875|loadImdbGraphFromDisk - after building graph
#11:49:10.679 [main] WARN  o.c.c.a.d.c.graph.v2.JGraphBuilder - loadImdbGraphFromDisk - movieNodesCreated:  336185
#11:49:10.679 [main] WARN  o.c.c.a.d.c.graph.v2.JGraphBuilder - loadImdbGraphFromDisk - personNodesCreated: 599722
#11:49:10.679 [main] WARN  o.c.c.a.d.c.graph.v2.JGraphBuilder - loadImdbGraphFromDisk - edgesCreated:       3559674
#11:49:10.679 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - JGraph refresh() - replacing graph with newGraph, elapsed ms: 12216
#11:49:11.293 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - getShortestPath, v1: nm0000102 to v2: nm0001648
#11:49:11.426 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - elapsed milliseconds: 133
#11:49:11.427 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - path getLength:       4
#11:49:11.427 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - path getStartVertex:  nm0000102
#11:49:11.427 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - path getEndVertex:    nm0001648

# =============================================================================
# Sample output with $startingYear 0 and $movieMinMinutes 30
# wrangling:
#    creationEpoch|totalMb|freeMb|maxMb|pctFree|pctMax|note
#    1668704596076|2048|998|2048|0.487100813538|1|finish
#    EOJ Totals:
#    minYear:                  0
#    minMinutes:               30
#    totalMovieCount:          9275714
#    includedMovieCount:       380570
#    totalPrincipalCount:      52412047
#    includedPrincipalCount:   655653
#    totalPersonCount:         11978491
#    includedPersonCount:      655159
#    included Movies + People: 1035729
#    titleToPersonCount:       1957212
#    elapsedMs:                290195.0
#    elapsedMin:               4.8365833333333335
# traversal:
#    creationEpoch|totalMb|freeMb|maxMb|pctFree|pctMax|note
#    1668704776993|3482|1552|4096|0.445678185075|0.85009765625|loadImdbGraphFromDisk - after building graph
#    12:06:16.994 [main] WARN  o.c.c.a.d.c.graph.v2.JGraphBuilder - loadImdbGraphFromDisk - movieNodesCreated:  380570
#    12:06:16.994 [main] WARN  o.c.c.a.d.c.graph.v2.JGraphBuilder - loadImdbGraphFromDisk - personNodesCreated: 655159
#    12:06:16.994 [main] WARN  o.c.c.a.d.c.graph.v2.JGraphBuilder - loadImdbGraphFromDisk - edgesCreated:       3914288
#    12:06:16.994 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - JGraph refresh() - replacing graph with newGraph, elapsed ms: 12202
#    12:06:17.678 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - getShortestPath, v1: nm0000102 to v2: nm0001648
#    12:06:17.780 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - elapsed milliseconds: 102
#    12:06:17.780 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - path getLength:       4
#    12:06:17.780 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - path getStartVertex:  nm0000102
#    12:06:17.781 [main] WARN  o.c.c.a.data.common.graph.v2.JGraph - path getEndVertex:    nm0001648
