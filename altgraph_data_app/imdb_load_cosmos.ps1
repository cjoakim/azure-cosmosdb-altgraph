# Bulk load the IMDb data in CosmosDB - into the imdb_graph and imdb_index containers
# Chris Joakim, Microsoft, November 2022

invoke-expression -Command .\build.ps1

echo "imdb_bulk_load_movies..."
java -Xmx4096m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_bulk_load_movies imdb_graph > out/imdb_bulk_load_movies.txt

echo "imdb_bulk_load_people..."
java -Xmx4096m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_bulk_load_people imdb_graph > out/imdb_bulk_load_people.txt

echo "imdb_bulk_load_small_triples..."
java -Xmx4096m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_bulk_load_small_triples imdb_graph > out/imdb_bulk_load_small_triples.txt

echo "imdb_bulk_load_movies_idx..."
java -Xmx4096m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_bulk_load_movies_idx imdb_index > out/imdb_bulk_load_movies_idx.txt

echo "done"
