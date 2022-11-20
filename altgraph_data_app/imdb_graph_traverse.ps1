# Test the in-memory graph loading and traversal process.
# Chris Joakim, Microsoft, November 2022

invoke-expression -Command .\build.ps1

echo "imdb_traversal..."
java -Xmx4096m -jar build\libs\altgraph_data_app-2.0.0.jar imdb_traversal nm0000102 nm0000210 false > out/imdb_traversal.txt

echo "done"
