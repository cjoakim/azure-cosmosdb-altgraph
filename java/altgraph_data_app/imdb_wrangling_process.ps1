# Execute the entire IMDb data wrangling process.
# It is assumed that you have already downloaded the raw-data TSV files;
# altgraph_data_app/data/imdb_raw/readme.md in this repo.
# Chris Joakim, Microsoft, November 2022

invoke-expression -Command .\build.ps1

# These two parameters drive the IMDb data-extraction and wrangling process:
$startingYear    = 0
$movieMinMinutes = 30

echo "========================"
echo "imdb_wrangle_raw_data..."
java -Xms1024m -Xmx2048m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_wrangle_raw_data $startingYear $movieMinMinutes > out/imdb_wrangle_raw_data.txt

echo "====================="
echo "imdb_build_triples..."
java -Xms1024m -Xmx4096m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_build_triples > out/imdb_build_triples.txt

echo "==============================================="
echo "zip large output files for storage in github..."
ant -f .\zip_data.xml

echo "done"


# Sample output on 2022/11/21:
# EOJ Totals:
#   minYear:                  0
#   minMinutes:               30
#   totalMovieCount:          9275714
#   includedMovieCount:       380570
#   totalPrincipalCount:      52412047
#   includedPrincipalCount:   655653
#   totalPersonCount:         11978491
#   includedPersonCount:      655159
#   included Movies + People: 1035729
#   titleToPersonCount:       1957212
#   elapsedMs:                249697.0
#   elapsedMin:               4.161616666666666
