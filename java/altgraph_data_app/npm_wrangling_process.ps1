# Wrangle/transform the raw npm data into files for loading into Cosmos DB.
# Chris Joakim, Microsoft, November 2022

invoke-expression -Command .\build.ps1

echo "npm_wrangle_raw_data..."
java -Xms1024m -Xmx2048m -jar build\libs\altgraph_data_app-2.0.0.jar npm_wrangle_raw_data > out/npm_wrangle_raw_data.txt

echo "done"
