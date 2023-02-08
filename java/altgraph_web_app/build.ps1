# Compile this app and produce an uberJar file.
# Chris Joakim, Microsoft, November 2022

echo "clean..."
gradle clean

echo "build..."
gradle build

echo "uberJar..."
gradle uberJar

echo "dir of build\libs..."
dir build\libs\

$outDir = "out"
if (Test-Path $outDir) {
    Write-Host "outDir exists"
}
else {
    New-Item $outDir -ItemType Directory
}

echo "gradle dependencies..."
gradle -q dependencies > tmp/dependencies.txt

echo "build script complete"
