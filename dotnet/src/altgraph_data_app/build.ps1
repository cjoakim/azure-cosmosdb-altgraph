echo "build..."
dotnet build ../altgraph_data_app.sln

dotnet build ../altgraph_web_app.sln

$outDir = "out"
if (Test-Path $outDir) {
  Write-Host "outDir exists"
}
else {
  New-Item $outDir -ItemType Directory
}

echo "build script complete"
