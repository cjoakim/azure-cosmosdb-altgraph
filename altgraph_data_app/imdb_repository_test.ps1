# Test the in-memory graph loading and traversal process.
# Chris Joakim, Microsoft, November 2022

invoke-expression -Command .\build.ps1

echo "imdb_person_lookup..."
java -Xmx2048m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_person_lookup nm0000102 > out/lookup_nm0000102.txt

echo "imdb_movie_lookup..."
java -Xmx2048m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_movie_lookup tt0087277 > out/lookup_tt0087277.txt

echo "done"
