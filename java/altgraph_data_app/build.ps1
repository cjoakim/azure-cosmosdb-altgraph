# Compile this app and produce an uberJar file.
# Chris Joakim, Microsoft, December 2023

echo "gradle version..."
gradle -v

echo "clean..."
gradle clean --warning-mode none

echo "build..."
# gradle build
gradle build --warning-mode none

echo "uberJar..."
gradle uberJar --warning-mode none

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
gradle -q dependencies > out/dependencies.txt

echo "build script complete"


# Output of this script on 2023/12/19 with Gradle 8.5
#
# PS ...\altgraph_data_app> .\build.ps1
# gradle version...

# ------------------------------------------------------------
# Gradle 8.5
# ------------------------------------------------------------

# Build time:   2023-11-29 14:08:57 UTC
# Revision:     28aca86a7180baa17117e0e5ba01d8ea9feca598

# Kotlin:       1.9.20
# Groovy:       3.0.17
# Ant:          Apache Ant(TM) version 1.10.13 compiled on January 4 2023
# JVM:          17.0.7 (Microsoft 17.0.7+7-LTS)
# OS:           Windows 11 10.0 amd64

# clean...

# BUILD SUCCESSFUL in 1s
# 1 actionable task: 1 executed
# build...

# > Task :compileJava
# Note: Some input files use unchecked or unsafe operations.
# Note: Recompile with -Xlint:unchecked for details.

# BUILD SUCCESSFUL in 4s
# 5 actionable tasks: 5 executed
# uberJar...

# BUILD SUCCESSFUL in 5s
# 3 actionable tasks: 1 executed, 2 up-to-date
# dir of build\libs...


#     Directory: C:\Users\chjoakim\github\azure-cosmosdb-altgraph\java\altgraph_data_app\build\libs


# Mode                 LastWriteTime         Length Name
# ----                 -------------         ------ ----
# -a----        12/19/2023   1:56 PM       46317733 altgraph_data_app-2.0.0-altgraph-wrangling.jar
# -a----        12/19/2023   1:56 PM         176853 altgraph_data_app-2.0.0-plain.jar
# -a----        12/19/2023   1:56 PM       48518111 altgraph_data_app-2.0.0.jar
# outDir exists
# gradle dependencies...
# build script complete
