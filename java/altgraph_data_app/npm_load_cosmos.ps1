# Load Cosmos DB with the wrangled NPM files.
# Chris Joakim, Microsoft, November 2022

invoke-expression -Command .\build.ps1

echo "npm_load_cosmos..."
java -Xms1024m -Xmx2048m -jar build\libs\altgraph_data_app-2.0.0.jar npm_load_cosmos --do-writes --verbose > out/npm_load_cosmos.txt

echo "done"
