#!/bin/bash

# Compile this app and produce an uberJar file.
# Chris Joakim, Microsoft, December 2023

echo "gradle version..."
gradle -v

echo "clean..."
gradle clean --warning-mode none

echo "build..."
gradle build --warning-mode none

echo "uberJar..."
gradle uberJar --warning-mode none

echo "dir of build\libs..."
ls -al build/libs/

echo "gradle dependencies..."
mkdir -p tmp/
gradle -q dependencies > tmp/dependencies.txt

echo "build script complete"
