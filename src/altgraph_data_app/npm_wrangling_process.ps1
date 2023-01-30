invoke-expression -Command .\build.ps1

echo "npm_wrangle_raw_data..."

dotnet run npm_wrangle_raw_data > out/npm_wrangling_process.txt

echo "done"
