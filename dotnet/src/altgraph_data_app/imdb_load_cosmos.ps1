invoke-expression -Command .\build.ps1

echo "========================"
echo "imdb_bulk_load_movies..."
dotnet run imdb_bulk_load_movies imdb_graph > out/imdb_bulk_load_movies.txt

# SELECT count(1) FROM c where c.doctype = 'movie' --> 380,570

echo "========================"
echo "imdb_bulk_load_people..."
dotnet run imdb_bulk_load_people imdb_graph > out/imdb_bulk_load_people.txt

# SELECT count(1) FROM c where c.doctype = 'person' --> 655,159

# Note: these "small triples" aren't actually needed for the current v2 implementation
# so this step can be omitted.  The concept of "triples" is used, however, in the v1/npm graph.
# echo "==============================="
# echo "imdb_bulk_load_small_triples..."
# dotnet run imdb_bulk_load_small_triples imdb_graph > out/imdb_bulk_load_small_triples.txt

# SELECT count(1) FROM c where c.doctype = 'sm_triple' --> 3,914,263

echo "============================="
echo "imdb_bulk_load_movies_seed..."
dotnet run imdb_bulk_load_movies_seed imdb_seed > out/imdb_bulk_load_movies_seed.txt

# SELECT count(1) FROM c where c.doctype = 'movie_seed' --> 380,570

echo "done"
