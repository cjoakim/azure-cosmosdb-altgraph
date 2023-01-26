invoke-expression -Command .\build.ps1

echo "npm_load_cosmos..."
dotnet run --project src/altgraph_data_app npm_load_cosmos --do-writes --verbose > out/npm_load_cosmos.txt

echo "done"
